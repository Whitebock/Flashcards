namespace Flashcards.Common.Options;

public class KafkaOptions
{
    public string[]? Brokers { get; set; }
    public KafkaTopicOptions Topics { get; set; } = new();
}

public class KafkaTopicOptions
{
    public string? Commands { get; set; }
    public string? Events { get; set; }
}