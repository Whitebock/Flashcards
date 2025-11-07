using CommandHandler.Handlers;
using Flashcards.Common.Hosting;
using Flashcards.Common.Options;
using KafkaFlow;
using KafkaFlow.Serializer;

var builder = Host.CreateApplicationBuilder(args);
builder.Configuration.AddCommonConfig(builder.Environment);

var kafkaOptions = new KafkaOptions();
builder.Configuration.Bind("Kafka", kafkaOptions);
builder.Services.AddKafkaFlowHostedService(kafka => kafka
    .UseMicrosoftLog()
    .AddCluster(cluster => cluster
        .WithBrokers(kafkaOptions.Brokers)
        .AddConsumer(consumer => consumer
            .Topic(kafkaOptions.Topics.Commands)
            .WithGroupId(AppDomain.CurrentDomain.FriendlyName)
            .WithBufferSize(30)
            .WithWorkersCount(1)
            .WithAutoOffsetReset(AutoOffsetReset.Earliest)
            .AddMiddlewares(middleware =>
            {
                middleware
                    .AddDeserializer<JsonCoreDeserializer>()
                    .AddTypedHandlers(handlers => handlers
                        .AddHandler<DeckHandler>());
            })
        )
        .AddProducer<DeckHandler>(producer => producer
            .DefaultTopic(kafkaOptions.Topics.Events)
            .WithAcks(Acks.All)
            .AddMiddlewares(middlewares => middlewares
                .AddSerializer<JsonCoreSerializer>())
        )
    )
);
var host = builder.Build();
await host.RunAsync();