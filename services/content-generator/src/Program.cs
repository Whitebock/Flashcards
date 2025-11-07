using System.CommandLine;
using Flashcards.Common.Hosting;
using Flashcards.Common.Infrastructure.Producers;
using Flashcards.Common.Interfaces;
using Flashcards.Common.Messages;
using Flashcards.Common.Options;
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
builder.Configuration.AddCommonConfig(builder.Environment);

var kafkaOptions = new KafkaOptions();
builder.Configuration.Bind("Kafka", kafkaOptions);
builder.Services.AddKafka(kafka => kafka
    .UseMicrosoftLog()
    .AddCluster(cluster => cluster
        .WithBrokers(kafkaOptions.Brokers)
        .AddProducer<ICommand>(producer => producer
            .DefaultTopic(kafkaOptions.Topics.Commands)
            .WithAcks(Acks.All)
            .AddMiddlewares(middlewares => middlewares
                .AddSerializer<JsonCoreSerializer>())
        )
    )
);
builder.Services
    .AddSingleton<ICardManager, CardCommandProducer>()
    .AddSingleton<IDeckManager, DeckCommandProducer>()
    .AddSingleton(provider =>
    {
        var config = provider.GetRequiredService<IConfiguration>();
        return new OpenAIClient(config["OpenAI:Key"]);
    })
    .AddOpenAIChatCompletion("gpt-4.1-mini")
    .AddSingleton<DeckGenerator>();
var app = builder.Build();

var rootCommand = new RootCommand("Flashcard content generator.");

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

rootCommand.AddCommand(generateCommand);

return await rootCommand.InvokeAsync(args);