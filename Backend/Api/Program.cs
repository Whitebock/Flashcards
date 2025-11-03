using System.ComponentModel;
using Flashcards.Common.Infrastructure.Consumers;
using Flashcards.Common.Infrastructure.Producers;
using Flashcards.Common.Infrastructure.UserStore;
using Flashcards.Common.Interfaces;
using Flashcards.Common.Messages;
using KafkaFlow;
using KafkaFlow.Serializer;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Logging;
using Microsoft.OpenApi.Any;
using Microsoft.OpenApi.Models;
using Scalar.AspNetCore;

var builder = WebApplication.CreateBuilder(args);
builder.Configuration.AddIniFile("appsettings.ini");
builder.Services.AddKafkaFlowHostedService(kafka =>
{
    kafka.AddCluster(cluster =>
    {
        cluster.WithBrokers(["localhost"])
            .AddConsumer(consumer => consumer
                .Topic("Flashcards.Events")
                .WithGroupId("Api")
                .WithBufferSize(50)
                .WithWorkersCount(1)
                .WithAutoOffsetReset(AutoOffsetReset.Earliest)
                .WithoutStoringOffsets()
                .AddMiddlewares(middleware =>
                {
                    middleware
                        .AddDeserializer<JsonCoreDeserializer>()
                        .AddTypedHandlers(handlers => handlers
                            .AddHandler<DecksProjection>()
                            .AddHandler<DeckActivityProjection>()
                            .AddHandler<DeckUserStatsProjection>()
                            .AddHandler<CardProjection>()
                            .AddHandler<FeedProjection>()
                            .AddHandler<SpacedRepetitionProjection>()
                    );
                })
            )
            .AddProducer<ICommand>(producer => producer
                .DefaultTopic("Flashcards.Commands")
                .WithAcks(Acks.All)
                .AddMiddlewares(middleware => middleware
                    .AddSerializer<JsonCoreSerializer>())
            );
    });
});
// KafkaFlow already injects the Projection, and we only want to decorate them with an Interface.
builder.Services
    .AddSingleton<IDeckRepository, DecksProjection>(provider => 
        provider.GetRequiredService<DecksProjection>())
    .AddSingleton<IDeckActivityRepository, DeckActivityProjection>(provider => 
        provider.GetRequiredService<DeckActivityProjection>())
    .AddSingleton<IDeckUserStatsRepository, DeckUserStatsProjection>(provider => 
        provider.GetRequiredService<DeckUserStatsProjection>())
    .AddSingleton<ICardRepository, CardProjection>(provider => 
        provider.GetRequiredService<CardProjection>())
    .AddSingleton<IFeedRepository, FeedProjection>(provider => 
        provider.GetRequiredService<FeedProjection>())
    .AddSingleton<IRepetitionAlgorithm, SpacedRepetitionProjection>(provider => 
        provider.GetRequiredService<SpacedRepetitionProjection>())
    .AddSingleton<IDeckManager, DeckCommandProducer>()
    .AddSingleton<ICardManager, CardCommandProducer>();
builder.Services
    .Configure<Auth0UserStoreOptions>(options =>
    {
        options.Domain =  builder.Configuration["Auth0:Domain"];
        options.ClientId = builder.Configuration["Auth0:ClientId"];
        options.ClientSecret = builder.Configuration["Auth0:ClientSecret"];
    })
    .AddHttpClient()
    .AddSingleton<IUserStore, Auth0UserStore>()
    .Decorate<IUserStore, CachedUserStore>()
    .AddMemoryCache();

builder.Services.AddCors(options => {
    options.AddPolicy("AllowAllOrigins", builder =>  builder
        .AllowAnyOrigin()
        .AllowAnyHeader()
        .AllowAnyMethod()
    );
});

builder.Services.AddControllers();
builder.Services.AddOpenApi(options =>
{
    options.AddDocumentTransformer((document, context, cancellationToken) =>
    {
        document.Info = new OpenApiInfo
        {
            Title = "Flashcards API",
            Version = "v1",
            Description = "An API for managing decks of flashcards."
        };
        return Task.CompletedTask;
    });
    // Tranformer for enabling readOnly (only used for GET requests) properties.
    options.AddSchemaTransformer((schema, context, cancellationToken) =>
    {
        var attr = (ReadOnlyAttribute?) context.JsonPropertyInfo?.AttributeProvider?
            .GetCustomAttributes(typeof(ReadOnlyAttribute), false)
            .FirstOrDefault();

        if(attr != null && attr.IsReadOnly) {
            schema.ReadOnly = true;
        }
        return Task.CompletedTask;
    });
    // Transformer for fixing unexpected null value for enums.
    // https://github.com/dotnet/aspnetcore/issues/60986
    options.AddSchemaTransformer((schema, context, cancellationToken) =>
    {
        if (schema.Enum is [.., OpenApiString { Value: null }])
        {
            schema.Enum.RemoveAt(schema.Enum.Count - 1);
            schema.Nullable = true;
            //schema.Title = schema.Title.Replace("NullableOf", "");
        }

        return Task.CompletedTask;
    });
});

builder.Services
    .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(JwtBearerDefaults.AuthenticationScheme, options =>  {
        options.Authority = builder.Configuration["OIDC:Authority"];
        options.Audience = builder.Configuration["OIDC:Audience"];
    });
builder.Services.AddAuthorization(options => {
    options.AddPolicy("HasUser", policy => {
        policy.RequireAuthenticatedUser();
    });
});

var app = builder.Build();

app.UseAuthentication();
app.UseAuthorization();
if (app.Environment.IsDevelopment())
{
    IdentityModelEventSource.ShowPII = true;
    IdentityModelEventSource.LogCompleteSecurityArtifact = true;
    app.MapOpenApi();
    app.MapScalarApiReference("/", options => {
        options
        .WithTitle("Flashcards API")
        .WithClientButton(false)
        .WithDefaultOpenAllTags(true)
        .WithOperationSorter(OperationSorter.Method);
    });
}
app.UseCors("AllowAllOrigins");
app.MapControllers();

app.Run();
