namespace Flashcards.Common.Infrastructure.Consumers;

/// <summary>
/// Implements the FSRS (Free Spaced Repetition Scheduler) algorithm.
/// </summary>
public class FreeSpacedRepetitionScheduler()
{
    public const double STABILITY_MIN = 0.001;
    public static readonly double[] PARAMETERS =
    [
        0.2172,
        1.1771,
        3.2602,
        16.1507,
        7.0114,
        0.57,
        2.0966,
        0.0069,
        1.5261,
        0.112,
        1.0178,
        1.849,
        0.1133,
        0.3127,
        2.2934,
        0.2191,
        3.0004,
        0.7536,
        0.3332,
        0.1437,
        0.2
    ];
    public static readonly double[][] FUZZ_RANGES =
    [
        [2.5, 7.0, 0.15],
        [7.0, 20.0, 0.10],
        [20.0, double.PositiveInfinity, 0.05]
    ];
    public enum Rating
    {
        Again = 1,
        Hard = 2,
        Good = 3,
        Easy = 4
    }

    public enum State
    {
        Learning = 1,
        Review = 2,
        Relearning = 3
    }

    public class RepititionInfo()
    {
        public State State { get; set; } = State.Learning;
        public int? Step { get; set; } = 0;
        public double? Stability { get; set; }
        public double? Difficulty { get; set; }
        public DateTime Due { get; set; } = DateTime.UtcNow;
        public DateTime? LastReview { get; set; }
    };

    private readonly Dictionary<Guid, Dictionary<Guid, RepititionInfo>> _repetitionInfo = [];

    private double DesiredRetention { get; init; } = 0.9;
    private TimeSpan[] LearningSteps { get; init; } =
    [
        TimeSpan.FromMinutes(1),
        TimeSpan.FromMinutes(10)
    ];
    private TimeSpan[] RelearningSteps { get; init; } =
    [
        TimeSpan.FromMinutes(10)
    ];
    private int MaximumInterval { get; init; } = 36500;
    private bool EnableFuzzing { get; init; } = true;

    private readonly double DECAY = -PARAMETERS[20];
    private readonly double FACTOR = Math.Pow(0.9, 1.0 / -PARAMETERS[20]) - 1;

    private double GetCardRetrievability(RepititionInfo info, DateTime? currentDate = null)
    {
        if (info.LastReview == null) return 0;

        if (currentDate == null) currentDate = DateTime.UtcNow;

        var elapsedDays = Math.Max(0, (currentDate.Value - info.LastReview.Value).TotalDays);
        return Math.Pow(1 + FACTOR * elapsedDays / (info.Stability ?? STABILITY_MIN), DECAY); // CHECK: Min check was added
    }

    public Dictionary<Guid, RepititionInfo> GetCardInfos(Guid userId)
    {
        return _repetitionInfo.TryGetValue(userId, out var infos) ? infos : [];
    }

    public void ReviewCard(Guid userId, Guid cardId, Rating rating, DateTime reviewTime)
    {
        if (!_repetitionInfo.ContainsKey(userId))
            _repetitionInfo[userId] = [];

        if (!_repetitionInfo[userId].ContainsKey(cardId))
            _repetitionInfo[userId][cardId] = new RepititionInfo();

        var info = _repetitionInfo[userId][cardId];

        // Figure out the next review date
        TimeSpan nextInterval = TimeSpan.Zero;
        int? daysSinceLastReview = info.LastReview.HasValue ? (reviewTime - info.LastReview.Value).Days : null;
        switch (info.State)
        {
            case State.Learning:
                // Update stability and difficulty
                if (info.Stability == null || info.Difficulty == null)
                {
                    info.Stability = InitialStability(rating);
                    info.Difficulty = InitialDifficulty(rating);
                }
                else if (daysSinceLastReview != null && daysSinceLastReview < 1)
                {
                    info.Stability = ShortTermStability(info.Stability.Value, rating);
                    info.Difficulty = NextDifficulty(info.Difficulty.Value, rating);
                }
                else
                {
                    info.Stability = NextStability(info.Difficulty.Value, info.Stability.Value,
                        GetCardRetrievability(info, reviewTime), rating);
                    info.Difficulty = NextDifficulty(info.Difficulty.Value, rating);
                }

                // Determine next interval
                if (LearningSteps.Length == 0 ||
                    (info.Step >= LearningSteps.Length && (rating == Rating.Hard || rating == Rating.Good || rating == Rating.Easy)))
                {
                    info.State = State.Review;
                    info.Step = null;
                    var nextIntervalDays = NextInterval(info.Stability.Value);
                    nextInterval = TimeSpan.FromDays(nextIntervalDays);
                }
                else
                {
                    switch (rating)
                    {
                        case Rating.Again:
                            info.Step = 0;
                            nextInterval = LearningSteps[info.Step.Value];
                            break;

                        case Rating.Hard:
                            if (info.Step == 0 && LearningSteps.Length == 1)
                                nextInterval = TimeSpan.FromTicks((long)(LearningSteps[0].Ticks * 1.5));
                            else if (info.Step == 0 && LearningSteps.Length >= 2)
                                nextInterval = TimeSpan.FromTicks((LearningSteps[0].Ticks + LearningSteps[1].Ticks) / 2);
                            else
                                nextInterval = LearningSteps[info.Step.Value];
                            break;

                        case Rating.Good:
                            if (info.Step + 1 == LearningSteps.Length)
                            {
                                info.State = State.Review;
                                info.Step = null;
                                var nextIntervalDays = NextInterval(info.Stability.Value);
                                nextInterval = TimeSpan.FromDays(nextIntervalDays);
                            }
                            else
                            {
                                info.Step++;
                                nextInterval = LearningSteps[info.Step.Value];
                            }
                            break;

                        case Rating.Easy:
                            info.State = State.Review;
                            info.Step = null;
                            var easyNextIntervalDays = NextInterval(info.Stability.Value);
                            nextInterval = TimeSpan.FromDays(easyNextIntervalDays);
                            break;
                    }
                }
                break;

            case State.Review:
                if (daysSinceLastReview != null && daysSinceLastReview < 1)
                {
                    info.Stability = ShortTermStability(info.Stability.Value, rating);
                    info.Difficulty = NextDifficulty(info.Difficulty.Value, rating);
                }
                else
                {
                    info.Stability = NextStability(info.Difficulty.Value, info.Stability.Value,
                        GetCardRetrievability(info, reviewTime), rating);
                    info.Difficulty = NextDifficulty(info.Difficulty.Value, rating);
                }

                switch (rating)
                {
                    case Rating.Again:
                        if (RelearningSteps.Length == 0)
                        {
                            var intervalDays = NextInterval(info.Stability.Value);
                            nextInterval = TimeSpan.FromDays(intervalDays);
                        }
                        else
                        {
                            info.State = State.Relearning;
                            info.Step = 0;
                            nextInterval = RelearningSteps[info.Step.Value];
                        }
                        break;

                    case Rating.Hard:
                    case Rating.Good:
                    case Rating.Easy:
                        var reviewIntervalDays = NextInterval(info.Stability.Value);
                        nextInterval = TimeSpan.FromDays(reviewIntervalDays);
                        break;
                }
                break;

            case State.Relearning:
                if (daysSinceLastReview != null && daysSinceLastReview < 1)
                {
                    info.Stability = ShortTermStability(info.Stability.Value, rating);
                    info.Difficulty = NextDifficulty(info.Difficulty.Value, rating);
                }
                else
                {
                    info.Stability = NextStability(info.Difficulty.Value, info.Stability.Value,
                        GetCardRetrievability(info, reviewTime), rating);
                    info.Difficulty = NextDifficulty(info.Difficulty.Value, rating);
                }

                if (RelearningSteps.Length == 0 ||
                    (info.Step >= RelearningSteps.Length && (rating == Rating.Hard || rating == Rating.Good || rating == Rating.Easy)))
                {
                    info.State = State.Review;
                    info.Step = null;
                    var nextIntervalDays = NextInterval(info.Stability.Value);
                    nextInterval = TimeSpan.FromDays(nextIntervalDays);
                }
                else
                {
                    switch (rating)
                    {
                        case Rating.Again:
                            info.Step = 0;
                            nextInterval = RelearningSteps[info.Step.Value];
                            break;

                        case Rating.Hard:
                            if (info.Step == 0 && RelearningSteps.Length == 1)
                                nextInterval = TimeSpan.FromTicks((long)(RelearningSteps[0].Ticks * 1.5));
                            else if (info.Step == 0 && RelearningSteps.Length >= 2)
                                nextInterval = TimeSpan.FromTicks((RelearningSteps[0].Ticks + RelearningSteps[1].Ticks) / 2);
                            else
                                nextInterval = RelearningSteps[info.Step.Value];
                            break;

                        case Rating.Good:
                            if (info.Step + 1 == RelearningSteps.Length)
                            {
                                info.State = State.Review;
                                info.Step = null;
                                var nextIntervalDays = NextInterval(info.Stability.Value);
                                nextInterval = TimeSpan.FromDays(nextIntervalDays);
                            }
                            else
                            {
                                info.Step++;
                                nextInterval = RelearningSteps[info.Step.Value];
                            }
                            break;

                        case Rating.Easy:
                            info.State = State.Review;
                            info.Step = null;
                            var easyIntervalDays = NextInterval(info.Stability.Value);
                            nextInterval = TimeSpan.FromDays(easyIntervalDays);
                            break;
                    }
                }
                break;
        }

        if (EnableFuzzing && info.State == State.Review)
        {
            nextInterval = GetFuzzedInterval(nextInterval);
        }

        info.Due = reviewTime + nextInterval;
        info.LastReview = DateTime.UtcNow;
    }

    private static double InitialDifficulty(Rating rating)
    {
        var difficulty = PARAMETERS[4] - Math.Exp(PARAMETERS[5] * ((int)rating - 1)) + 1;
        difficulty = ClampDifficulty(difficulty);
        return difficulty;
    }

    private static double ClampDifficulty(double difficulty)
    {
        return Math.Min(Math.Max(difficulty, 1.0), 10.0);
    }

    private static double InitialStability(Rating rating)
    {
        var stability = PARAMETERS[(int)(rating - 1)];
        stability = ClampStability(stability);
        return stability;
    }

    private static double ClampStability(double stability)
    {
        return Math.Max(stability, STABILITY_MIN);
    }

    private int NextInterval(double stability)
    {
        var nextInterval = stability / FACTOR * (
            Math.Pow(DesiredRetention, 1 / DECAY) - 1
        );

        // Intervals are full days
        nextInterval = Math.Round(nextInterval);

        // Must be at least 1 day long
        nextInterval = Math.Max(nextInterval, 1);

        // Can not be longer than the maximum interval
        nextInterval = Math.Min(nextInterval, MaximumInterval);

        return  Convert.ToInt32(nextInterval);
    }

    private static double ShortTermStability(double stability, Rating rating)
    {
        double shortTermStabilityIncrease = 
            Math.Exp(PARAMETERS[17] * ((int)rating - 3 + PARAMETERS[18])) *
            Math.Pow(stability, -PARAMETERS[19]);

        if (rating == Rating.Good || rating == Rating.Easy)
        {
            shortTermStabilityIncrease = Math.Max(shortTermStabilityIncrease, 1.0);
        }

        double shortTermStability = stability * shortTermStabilityIncrease;

        shortTermStability = ClampStability(shortTermStability);

        return shortTermStability;
    }

    private static double NextDifficulty(double difficulty, Rating rating)
    {
        double LinearDamping(double deltaDifficulty, double diff)
        {
            return (10.0 - diff) * deltaDifficulty / 9.0;
        }

        double MeanReversion(double arg1, double arg2)
        {
            return PARAMETERS[7] * arg1 + (1 - PARAMETERS[7]) * arg2;
        }

        double arg1 = InitialDifficulty(Rating.Easy);

        double deltaDifficulty = -PARAMETERS[6] * ((int)rating - 3);
        double arg2 = difficulty + LinearDamping(deltaDifficulty, difficulty);

        double nextDifficulty = MeanReversion(arg1, arg2);

        nextDifficulty = ClampDifficulty(nextDifficulty);

        return nextDifficulty;
    }

    private static double NextStability(double difficulty, double stability, double retrievability, Rating rating)
    {
        double nextStability;
        if (rating == Rating.Again)
        {
            nextStability = NextForgetStability(difficulty, stability, retrievability);
        }
        else
        {
            nextStability = NextRecallStability(difficulty, stability, retrievability, rating);
        }

        nextStability = ClampStability(nextStability);

        return nextStability;
    }

    private static double NextForgetStability(double difficulty, double stability, double retrievability)
    {
        double longTermStability = 
            PARAMETERS[11] *
            Math.Pow(difficulty, -PARAMETERS[12]) *
            (Math.Pow(stability + 1, PARAMETERS[13]) - 1) *
            Math.Exp((1 - retrievability) * PARAMETERS[14]);

        double shortTermStability = stability / Math.Exp(PARAMETERS[17] * PARAMETERS[18]);

        return Math.Min(longTermStability, shortTermStability);
    }

    private static double NextRecallStability(double difficulty, double stability, double retrievability, Rating rating)
    {
        double hardPenalty = rating == Rating.Hard ? PARAMETERS[15] : 1;
        double easyBonus = rating == Rating.Easy ? PARAMETERS[16] : 1;

        double nextStability = stability * (
            1
            + Math.Exp(PARAMETERS[8])
            * (11 - difficulty)
            * Math.Pow(stability, -PARAMETERS[9])
            * (Math.Exp((1 - retrievability) * PARAMETERS[10]) - 1)
            * hardPenalty
            * easyBonus
        );

        return nextStability;
    }

    private TimeSpan GetFuzzedInterval(TimeSpan interval)
    {
        int intervalDays = (int)interval.TotalDays;

        // Fuzz is not applied to intervals less than 2.5
        if (intervalDays < 2.5) return interval;

        // Helper function that computes the possible upper and lower bounds of the interval after fuzzing.
        (int min, int max) GetFuzzRange(int days)
        {
            double delta = 1.0;
            
            foreach (var fuzzRange in FUZZ_RANGES)
            {
                double start = fuzzRange[0];
                double end = fuzzRange[1];
                double factor = fuzzRange[2];

                delta += factor * Math.Max(Math.Min(days, end) - start, 0.0);
                
            }

            int minIvl = (int)Math.Round(days - delta);
            int maxIvl = (int)Math.Round(days + delta);

            minIvl = Math.Max(2, minIvl);
            maxIvl = Math.Min(maxIvl, MaximumInterval);
            minIvl = Math.Min(minIvl, maxIvl);

            return (minIvl, maxIvl);
        }

        var (minIvl, maxIvl) = GetFuzzRange(intervalDays);

        int fuzzedIntervalDays = (int)Math.Round(Random.Shared.NextDouble() * (maxIvl - minIvl + 1) + minIvl);
        fuzzedIntervalDays = Math.Min(fuzzedIntervalDays, MaximumInterval);

        return TimeSpan.FromDays(fuzzedIntervalDays);
    }

}