using System.CommandLine;
using Flashcards.Common;
using Flashcards.Common.EventStore;
using Flashcards.Common.Messages.Commands;
using Flashcards.Common.Projections;
using Flashcards.Common.ServiceBus;
using Flashcards.ContentUtil;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

var builder = Host.CreateApplicationBuilder(args);
builder.Configuration.AddIniFile("appsettings.ini");
builder.Services
    .AddSingleton<IServiceBus, JsonLinesServiceBus>()
    .Configure<JsonLinesServiceBusOptions>(options => options.Dictionary = "../")
    .AddSingleton<ICommandSender, ServiceBusCommandSender>()
    .AddSingleton<IEventStore, JsonLinesEventStore>()
    .Configure<JsonLinesEventStoreOptions>(options => { options.FilePath = "../events.jsonl"; })
    .AddHostedService(provider => provider.GetRequiredService<IServiceBus>())
    .AddProjection<DeckListProjection>();
var app = builder.Build();

var rootCommand = new RootCommand("Flashcard content utility.");

var deckCommand = new Command("decks", "Manage flashcard decks.");

var listCommand = new Command("list", "List all flashcard decks.");
listCommand.SetHandler(async () =>
{
    var projection = app.Services.GetRequiredService<IProjection<DeckListProjection>>();
    var deckList = await projection.GetAsync();
    foreach (var deck in deckList.Decks)
        Console.WriteLine("{0} {1}", deck.Key.ToString()[..8], deck.Value);
});

var generateCommand = new Command("generate", "Generate a flashcard deck using AI.");
var userOption = new Option<Guid>(name: "--user")
{
    Description = "The user to generate the deck for.",
    IsRequired = true,
};
var topicOption = new Option<string>(name: "--topic")
{
    Description = "Topic of the deck to generate."
};
generateCommand.AddOption(userOption);
generateCommand.AddOption(topicOption);
generateCommand.SetHandler(async (userId, topic) =>
{
    var commandSender = app.Services.GetRequiredService<ICommandSender>();
    var logger = app.Services.GetRequiredService<ILogger<Program>>();
    logger.LogInformation("Generating deck for user {UserId} with topic {Topic}", userId, topic);
    var deckId = Guid.NewGuid();
    await commandSender.SendAsync([
        new CreateDeckCommand(topic, "Deck about " + topic) { Creator = userId, DeckId = deckId },
        new AddTagCommand(deckId, "AI") { Creator = userId }
    ]);
}, userOption, topicOption);

deckCommand.Add(listCommand);
deckCommand.Add(generateCommand);

rootCommand.AddCommand(deckCommand);
return await rootCommand.InvokeAsync(args);