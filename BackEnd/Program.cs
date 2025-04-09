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
builder.Services.Scan(scan => scan.FromEntryAssembly()
    // Add all CommandHandlers.
    .AddClasses(classes => classes.AssignableTo<ICommandHandler>())
    .As<ICommandHandler>()
    .WithSingletonLifetime()
    // Add all EventHandlers (Projections).
    .AddClasses(classes => classes.AssignableTo<IEventHandler>())
    .AsSelfWithInterfaces()
    .WithSingletonLifetime()
);
builder.Services.AddCors(options => {
    options.AddPolicy("AllowAllOrigins", builder =>  builder
        .AllowAnyOrigin()
        .AllowAnyHeader()
        .AllowAnyMethod()
    );
});
var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}
app.UseCors("AllowAllOrigins");

var commandBus = app.Services.GetRequiredService<ICommandBus>();
var mathDeckId = Guid.NewGuid();
commandBus.Send([
    new CreateDeckCommand("Maths"){DeckId=mathDeckId},
    new CreateCardCommand(mathDeckId, "What is 2 + 2?", "4"),
    new CreateCardCommand(mathDeckId, "What is 3 * 3?", "9"),
    new CreateCardCommand(mathDeckId, "What is 5 - 2?", "3"),
    new CreateCardCommand(mathDeckId, "What is 10 / 2?", "5"),
    new CreateCardCommand(mathDeckId, "What is 7 + 8?", "15")
]);

var spanishDeckId = Guid.NewGuid();
commandBus.Send([
    new CreateDeckCommand("Spanish") {DeckId=spanishDeckId},
    new CreateCardCommand(spanishDeckId, "¿Cómo se dice 'hello' en español?", "hola"),
    new CreateCardCommand(spanishDeckId, "¿Cómo se dice 'goodbye' en español?", "adiós")
]);

var japaneseDeckId = Guid.NewGuid();
commandBus.Send([
    new CreateDeckCommand("Japanese") {DeckId = japaneseDeckId},
    new CreateCardCommand(japaneseDeckId, "What is 'hello' in Japanese?", "こんにちは"),
    new CreateCardCommand(japaneseDeckId, "What is 'goodbye' in Japanese?", "さようなら"),
    new CreateCardCommand(japaneseDeckId, "What is 'thank you' in Japanese?", "ありがとう"),
    new CreateCardCommand(japaneseDeckId, "What is 'yes' in Japanese?", "はい"),
    new CreateCardCommand(japaneseDeckId, "What is 'no' in Japanese?", "いいえ"),
    new CreateCardCommand(japaneseDeckId, "What is 'please' in Japanese?", "お願いします"),
    new CreateCardCommand(japaneseDeckId, "What is 'excuse me' in Japanese?", "すみません")
]);

app.MapGet("/", () => new { Version = "1.0" });
app.MapGet("/decks", ([FromServices] DeckListProjection projection) => projection.GetAllDecks());
app.MapGet("/decks/{deckId:guid}/study", ([FromRoute] Guid deckId, [FromServices] CardProjection projection) => {
    var card = projection.GetCardsForDeck(deckId)[0];
    var left = 7;
    return new { card, left };
});
app.MapPost("/decks", ([FromBody] string name) => {
    commandBus.Send(new CreateDeckCommand(name));
});
app.MapPut("/decks/{deckId:guid}", ([FromRoute] Guid deckId, [FromBody] string name) => {
    commandBus.Send(new UpdateDeckCommand(deckId, name));
});
app.MapDelete("/decks/{deckId:guid}", ([FromRoute] Guid deckId, [FromServices] ICommandBus commandBus) => {
    commandBus.Send(new DeleteDeckCommand(deckId));
});
app.MapPost("/cards", () => {});
app.MapPut("/cards/{cardId:guid}", ([FromRoute] Guid cardId, [FromBody] CardChoice choice) => {
    commandBus.Send(new ChangeCardStatus(cardId, choice.Choice switch
    {
        "again" => CardStatus.Again,
        "good" => CardStatus.Good,
        "easy" => CardStatus.Easy,
        _ => CardStatus.Again
    }));
});
app.MapDelete("/cards/{cardId:guid}", ([FromRoute] Guid cardId) => {});
app.Run();

record CardChoice(string Choice);