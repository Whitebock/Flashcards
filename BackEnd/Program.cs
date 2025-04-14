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
    new CreateDeckCommand("Maths", "A deck for practicing basic math problems.") {DeckId=mathDeckId},
    new CreateCardCommand(mathDeckId, "What is 2 + 2?", "4"),
    new CreateCardCommand(mathDeckId, "What is 3 * 3?", "9"),
    new CreateCardCommand(mathDeckId, "What is 5 - 2?", "3"),
    new CreateCardCommand(mathDeckId, "What is 10 / 2?", "5"),
    new CreateCardCommand(mathDeckId, "What is 7 + 8?", "15")
]);

var spanishDeckId = Guid.NewGuid();
commandBus.Send([
    new CreateDeckCommand("Spanish", "A deck for practicing basic Spanish phrases.") {DeckId=spanishDeckId},
    new CreateCardCommand(spanishDeckId, "¿Cómo se dice 'hello' en español?", "hola"),
    new CreateCardCommand(spanishDeckId, "¿Cómo se dice 'goodbye' en español?", "adiós")
]);

var japaneseDeckId = Guid.NewGuid();
commandBus.Send([
    new CreateDeckCommand("Japanese", "A deck for practicing basic Japanese phrases.") {DeckId = japaneseDeckId},
    new CreateCardCommand(japaneseDeckId, "What is 'hello' in Japanese?", "こんにちは"),
    new CreateCardCommand(japaneseDeckId, "What is 'goodbye' in Japanese?", "さようなら"),
    new CreateCardCommand(japaneseDeckId, "What is 'thank you' in Japanese?", "ありがとう"),
    new CreateCardCommand(japaneseDeckId, "What is 'yes' in Japanese?", "はい"),
    new CreateCardCommand(japaneseDeckId, "What is 'no' in Japanese?", "いいえ"),
    new CreateCardCommand(japaneseDeckId, "What is 'please' in Japanese?", "お願いします"),
    new CreateCardCommand(japaneseDeckId, "What is 'excuse me' in Japanese?", "すみません")
]);

var geographyDeckId = Guid.NewGuid();
commandBus.Send([
    new CreateDeckCommand("Geography", "A deck for practicing geography knowledge.") { DeckId = geographyDeckId },
    new CreateCardCommand(geographyDeckId, "What is the capital of France?", "Paris"),
    new CreateCardCommand(geographyDeckId, "What is the largest continent?", "Asia"),
    new CreateCardCommand(geographyDeckId, "What is the longest river in the world?", "Nile"),
    new CreateCardCommand(geographyDeckId, "What is the smallest country in the world?", "Vatican City"),
    new CreateCardCommand(geographyDeckId, "What is the highest mountain in the world?", "Mount Everest")
]);

app.MapGet("/", () => new { Version = "1.0" });
app.MapGet("/decks", ([FromServices] DeckListProjection projection) => projection.GetAllDecks());
app.MapGet("/decks/search", ([FromQuery] string user, [FromQuery] string name, [FromServices] DeckListProjection projection) => {
    return projection.GetAllDecks().Where(d => d.FriendlyId.Equals(name)).FirstOrDefault();
});
app.MapGet("/decks/{deckId:guid}/study", ([FromRoute] Guid deckId, [FromServices] CardProjection projection) => {
    var cards = projection
        .GetCardsForDeck(deckId)
        .Where(c => c.Status == CardStatus.NotSeen || c.Status == CardStatus.Again)
        .OrderBy(c => c.Status);
    var card = cards.FirstOrDefault();
    var left = cards.Count();
    return new { card, left };
});
app.MapGet("/decks/{deckId:guid}", ([FromRoute] Guid deckId, [FromServices] DeckListProjection deckProjection) => {
    return deckProjection.GetDeck(deckId);
});
app.MapGet("/decks/{deckId:guid}/cards", ([FromRoute] Guid deckId, [FromServices] CardProjection cardProjection) => {
    return cardProjection.GetCardsForDeck(deckId);
});
app.MapPost("/decks", ([FromBody] DeckUpdateModel model) => {
    commandBus.Send(new CreateDeckCommand(model.Name, model.Description));
});
app.MapPut("/decks/{deckId:guid}", ([FromRoute] Guid deckId, [FromBody] DeckUpdateModel model) => {
    commandBus.Send(new UpdateDeckCommand(deckId, model.Name, model.Description));
});
app.MapDelete("/decks/{deckId:guid}", ([FromRoute] Guid deckId, [FromServices] ICommandBus commandBus) => {
    commandBus.Send(new DeleteDeckCommand(deckId));
});

app.MapPost("/cards", ([FromBody] CardAddModel model) => {
    commandBus.Send(new CreateCardCommand(model.DeckId, model.Front, model.Back));
});
app.MapPut("/cards/{cardId:guid}", ([FromRoute] Guid cardId, [FromBody] CardUpdateModel model, [FromServices] CardProjection cardProjection) => {
    commandBus.Send(new UpdateCardCommand(cardId, model.Front, model.Back));
});
app.MapPatch("/cards/{cardId:guid}", ([FromRoute] Guid cardId, [FromBody] CardStatusModel model) => {
    commandBus.Send(new ChangeCardStatus(cardId, model.Choice switch
    {
        "again" => CardStatus.Again,
        "good" => CardStatus.Good,
        "easy" => CardStatus.Easy,
        _ => CardStatus.Again
    }));
});
app.MapDelete("/cards/{cardId:guid}", ([FromRoute] Guid cardId) => {
    commandBus.Send(new DeleteCardCommand(cardId));
});
app.Run();

record CardAddModel(Guid DeckId, string Front, string Back);
record CardUpdateModel(string Front, string Back);
record CardStatusModel(string Choice);
record DeckUpdateModel(string Name, string Description);