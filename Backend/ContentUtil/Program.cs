using System.CommandLine;
using Flashcards.Common;
using Flashcards.Common.EventStore;
using Flashcards.Common.Messages;
using Flashcards.Common.Messages.Commands;
using Flashcards.Common.Projections;
using Flashcards.Common.ServiceBus;
using Flashcards.ContentUtil;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.SemanticKernel;
using OpenAI;

var builder = Host.CreateApplicationBuilder(args);
builder.Configuration.AddIniFile("appsettings.ini");
builder.Services
    .AddSingleton<IServiceBus, JsonLinesServiceBus>()
    .Configure<JsonLinesServiceBusOptions>(options => options.Dictionary = "../")
    .AddSingleton<ICommandSender, ServiceBusCommandSender>()
    .AddSingleton<IEventStore, JsonLinesEventStore>()
    .Configure<JsonLinesEventStoreOptions>(options => { options.FilePath = "../events.jsonl"; })
    .AddHostedService(provider => provider.GetRequiredService<IServiceBus>())
    .AddProjection<DeckListProjection>()
    .AddSingleton(provider => {
        var config = provider.GetRequiredService<IConfiguration>();
        return new OpenAIClient(config["OpenAI:Key"]);
    })
    .AddOpenAIChatCompletion("gpt-4.1-mini")
    .AddTransient<DeckGenerator>();
var app = builder.Build();

var rootCommand = new RootCommand("Flashcard content utility.");

#region List
var listCommand = new Command("list", "Show entity information.");

var listDecksCommand = new Command("decks", "List all decks.");
listDecksCommand.SetHandler(async () =>
{
    var projection = app.Services.GetRequiredService<IProjection<DeckListProjection>>();
    var deckList = await projection.GetAsync();
    foreach (var deck in deckList.Decks)
        Console.WriteLine("{0} {1}", deck.Key.ToString()[..8], deck.Value);
});
listCommand.AddCommand(listDecksCommand);
#endregion

#region Generate
var generateCommand = new Command("generate", "Generate content using AI.");
var userOption = new Option<Guid>(name: "--user")
{
    Description = "The user to generate the deck for.",
    IsRequired = true,
};
generateCommand.AddGlobalOption(userOption);

var generateDeckCommand = new Command("deck", "Generate a new deck");
var topicOption = new Option<string>(name: "--topic")
{
    Description = "Topic of the deck to generate."
};
generateDeckCommand.AddOption(topicOption);
var amountOption = new Option<int>(name: "--amount", () => 20)
{
    Description = "Amount of cards to generate."
};
generateDeckCommand.AddOption(amountOption);
generateDeckCommand.SetHandler(async (userId, topic, amount) =>
{
    var commandSender = app.Services.GetRequiredService<ICommandSender>();
    var logger = app.Services.GetRequiredService<ILogger<Program>>();
    var generator = app.Services.GetRequiredService<DeckGenerator>();

    var (Name, Description) = await generator.GenerateForTopicAsync(topic);
    var cards = await generator.GenerateCardsAsync(Name, Description, amount);
    Console.ForegroundColor = ConsoleColor.Cyan;
    Console.WriteLine(Name);
    Console.WriteLine(Description);
    Console.ForegroundColor = ConsoleColor.White;
    foreach (var (Front, Back) in cards.Take(5))
    {
        Console.WriteLine($"{Front}: {Back}");
    }
    Console.ResetColor();
    Console.Write("Created generated Deck? (y/n): ");
    var create = Console.ReadKey().KeyChar == 'y';
    Console.WriteLine();
    if (!create) return;

    var deckId = Guid.NewGuid();
    var commands = new List<ICommand>([
        new CreateDeckCommand(Name, Description) {DeckId = deckId, Creator = userId},
        new AddTagCommand(deckId, "AI") {Creator = userId}
    ]);
    foreach (var (Front, Back) in cards)
    {
        commands.Add(new CreateCardCommand(deckId, Front, Back) { Creator = userId });
    }

    logger.LogInformation("Sending {Amount} commands", commands.Count);
    await commandSender.SendAsync([.. commands]);

}, userOption, topicOption, amountOption);
generateCommand.AddCommand(generateDeckCommand);
#endregion

rootCommand.AddCommand(listCommand);
rootCommand.AddCommand(generateCommand);
return await rootCommand.InvokeAsync(args);