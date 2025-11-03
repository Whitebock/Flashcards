using CommandHandler.Handlers;
using KafkaFlow;
using KafkaFlow.Serializer;

var builder = Host.CreateApplicationBuilder(args);
builder.Services.AddKafkaFlowHostedService(kafka => kafka
    .UseMicrosoftLog()
    .AddCluster(cluster => cluster
        .WithBrokers(["localhost"])
        .AddConsumer(consumer => consumer
            .Topic("Flashcards.Commands")
            .WithGroupId("CommandHandler")
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
            .DefaultTopic("Flashcards.Events")
            .WithAcks(Acks.All)
            .AddMiddlewares(middlewares => middlewares
                .AddSerializer<JsonCoreSerializer>())
        )
    )
);
var host = builder.Build();
await host.RunAsync();