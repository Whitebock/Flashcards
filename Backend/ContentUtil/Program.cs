using System.CommandLine;
using Flashcards.Common.Infrastructure.Producers;
using Flashcards.Common.Interfaces;
using Flashcards.Common.Messages;
using Flashcards.ContentUtil;
using KafkaFlow;
using KafkaFlow.Serializer;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.SemanticKernel;
using OpenAI;

var builder = Host.CreateApplicationBuilder(args);
builder.Configuration.AddIniFile("appsettings.ini");
builder.Services.AddKafka(kafka =>
{
    kafka.UseMicrosoftLog()
        .AddCluster(cluster =>
        {
            cluster.WithBrokers(["localhost"])
                .AddConsumer(consumer => consumer
                    .Topic("Flashcards.Events")
                    .WithGroupId("ContentUtil")
                    .WithBufferSize(50)
                    .WithWorkersCount(1)
                    .WithAutoOffsetReset(AutoOffsetReset.Earliest)
                    .AddMiddlewares(middleware =>
                    {
                        middleware
                            .AddDeserializer<JsonCoreDeserializer>()
                            .AddTypedHandlers(handlers => handlers
                                .AddHandler<DeckListProjection>()
                            );
                    })
                )
                .AddProducer<ICommand>(producer => producer
                    .DefaultTopic("Flashcards.Commands")
                    .WithAcks(Acks.All)
                    .AddMiddlewares(middlewares => middlewares
                        .AddSerializer<JsonCoreSerializer>())
                );
        });
});
builder.Services
    .AddSingleton<ICardManager, CardCommandProducer>()
    .AddSingleton<IDeckManager, DeckCommandProducer>()
    .AddSingleton(provider =>
    {
        var config = provider.GetRequiredService<IConfiguration>();
        return new OpenAIClient(config["OpenAI:Key"]);
    })
    .AddOpenAIChatCompletion("gpt-4.1-mini")
    .AddTransient<DeckGenerator>();
var app = builder.Build();

var bus = app.Services.CreateKafkaBus();
await bus.StartAsync();
var rootCommand = new RootCommand("Flashcard content utility.");

#region List

var listCommand = new Command("list", "Show entity information.");

var listDecksCommand = new Command("decks", "List all decks.");
listDecksCommand.SetHandler(async () =>
{
    var projection = app.Services.GetRequiredService<DeckListProjection>();
    foreach (var deck in projection.Decks)
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
    var cardManager = app.Services.GetRequiredService<ICardManager>();
    var deckManager = app.Services.GetRequiredService<IDeckManager>();
    var logger = app.Services.GetRequiredService<ILogger<Program>>();
    var generator = app.Services.GetRequiredService<DeckGenerator>();

    var (name, description) = await generator.GenerateForTopicAsync(topic);
    var cards = await generator.GenerateCardsAsync(name, description, amount);
    Console.WriteLine(name);
    Console.WriteLine(description);
    foreach (var (front, back) in cards.Take(5))
    {
        Console.WriteLine($"{front}: {back}");
    }

    var deckId = Guid.NewGuid();
    await deckManager.CreateAsync(userId, name, description, deckId);
    await deckManager.AddTagAsync(userId, deckId, "AI");
    foreach (var (front, back) in cards)
    {
        await cardManager.CreateAsync(userId, deckId, front, back);
    }

    logger.LogInformation("Send {Amount} commands", cards.Count() + 2);
}, userOption, topicOption, amountOption);
generateCommand.AddCommand(generateDeckCommand);

#endregion

rootCommand.AddCommand(listCommand);
rootCommand.AddCommand(generateCommand);
var exitCode = await rootCommand.InvokeAsync(args);
await bus.StopAsync();
return exitCode;