using Flashcards;
using Flashcards.Commands;
using Flashcards.CQRS;
using Flashcards.Projections;
using Microsoft.AspNetCore.Mvc;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddOpenApi();
builder.Services.AddSingleton<IEventBus, InMemoryEventBus>();
builder.Services.AddSingleton<IEventStore, InMemoryEventStore>();
builder.Services.AddSingleton<ICommandBus, InMemoryCommandBus>();
builder.Services.AddSingleton<ICommandHandler, CommandHandler>();
builder.Services.AddSingleton<DeckListProjection>();
builder.Services.AddSingleton<IEventHandler>(provider => provider.GetRequiredService<DeckListProjection>());
builder.Services.AddSingleton<CardProjection>();
builder.Services.AddSingleton<IEventHandler>(provider => provider.GetRequiredService<CardProjection>());
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAllOrigins", builder =>
    {
        builder.AllowAnyOrigin()
               .AllowAnyHeader()
               .AllowAnyMethod();
    });
});
var app = builder.Build();

if (app.Environment.IsDevelopment() || true)
{
    app.MapOpenApi();
}
app.UseCors("AllowAllOrigins");

var commandBus = app.Services.GetRequiredService<ICommandBus>();
var mathDeckId = Guid.NewGuid();
commandBus.Send([
    new CreateDeckCommand(mathDeckId, "Maths"),
    new CreateCardCommand(mathDeckId, "What is 2 + 2?", "4"),
    new CreateCardCommand(mathDeckId, "What is 3 * 3?", "9"),
    new CreateCardCommand(mathDeckId, "What is 5 - 2?", "3"),
    new CreateCardCommand(mathDeckId, "What is 10 / 2?", "5"),
    new CreateCardCommand(mathDeckId, "What is 7 + 8?", "15")
]);

var spanishDeckId = Guid.NewGuid();
commandBus.Send([
    new CreateDeckCommand(spanishDeckId, "Spanish"),
    new CreateCardCommand(spanishDeckId, "¿Cómo se dice 'hello' en español?", "hola"),
    new CreateCardCommand(spanishDeckId, "¿Cómo se dice 'goodbye' en español?", "adiós")
]);

var japaneseDeckId = Guid.NewGuid();
commandBus.Send([
    new CreateDeckCommand(japaneseDeckId, "Japanese"),
    new CreateCardCommand(japaneseDeckId, "What is 'hello' in Japanese?", "こんにちは"),
    new CreateCardCommand(japaneseDeckId, "What is 'goodbye' in Japanese?", "さようなら"),
    new CreateCardCommand(japaneseDeckId, "What is 'thank you' in Japanese?", "ありがとう"),
    new CreateCardCommand(japaneseDeckId, "What is 'yes' in Japanese?", "はい"),
    new CreateCardCommand(japaneseDeckId, "What is 'no' in Japanese?", "いいえ"),
    new CreateCardCommand(japaneseDeckId, "What is 'please' in Japanese?", "お願いします"),
    new CreateCardCommand(japaneseDeckId, "What is 'excuse me' in Japanese?", "すみません")
]);

app.MapGet("/", () => new { Version = "1.0" });
app.MapGet("/decks", ([FromServices] DeckListProjection projection) => projection.Decks);
app.MapGet("/deck/{deckId}", ([FromRoute] string deckId, [FromServices] CardProjection projection) => {
    return projection.GetCards(Guid.Parse(deckId));
});
app.Run();