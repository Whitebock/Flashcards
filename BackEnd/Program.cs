var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseCors();

app.MapGet("/", () => new { Version = "1.0" });
app.MapGet("/cardstacks", () =>
{
    return new[] {
        new CardStack("Maths") { Cards = new List<Card> {
            new Card("What is 2 + 2?", "4") { Status = CardStatus.Correct },
            new Card("What is 3 * 3?", "9") { Status = CardStatus.Correct },
            new Card("What is 5 - 2?", "3") { Status = CardStatus.Correct },
            new Card("What is 10 / 2?", "5") { Status = CardStatus.NotSeen },
            new Card("What is 7 + 8?", "15") { Status = CardStatus.NotSeen },
        }},
        new CardStack("Spanish") { Cards = new List<Card> {
            new Card("¿Cómo se dice 'hello' en español?", "hola") { Status = CardStatus.NotSeen },
            new Card("¿Cómo se dice 'goodbye' en español?", "adiós") { Status = CardStatus.NotSeen },
        }},
        new CardStack("Japanese") { Cards = new List<Card> {
            new Card("What is 'hello' in Japanese?", "こんにちは") { Status = CardStatus.Correct },
            new Card("What is 'goodbye' in Japanese?", "さようなら") { Status = CardStatus.Correct },
            new Card("What is 'thank you' in Japanese?", "ありがとう") { Status = CardStatus.Incorrect },
            new Card("What is 'yes' in Japanese?", "はい") { Status = CardStatus.Incorrect },
            new Card("What is 'no' in Japanese?", "いいえ") { Status = CardStatus.Incorrect },
            new Card("What is 'please' in Japanese?", "お願いします") { Status = CardStatus.Incorrect },
            new Card("What is 'excuse me' in Japanese?", "すみません") { Status = CardStatus.NotSeen },
        }},
    };
})
.WithName("GetCardStacks");

app.Run();

record CardStack(string Name)
{
    public string Id => string.Join("_", Name.Split(' ')).ToLower();
    public List<Card> Cards { get; set; }
}

record Card(string Question, string Answer)
{
    public CardStatus Status { get; set; } = CardStatus.NotSeen;

}

enum CardStatus
{
    NotSeen,
    Correct,
    Incorrect
}