using Flashcards;
using Flashcards.Api;
using Flashcards.CQRS;
using Scalar.AspNetCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.Configure<JsonLinesEventStoreOptions>(options => {
    options.FilePath = "events.jsonl";
});
builder.Services.AddSingleton<IEventBus, InMemoryEventBus>();
builder.Services.AddSingleton<IEventStore, JsonLinesEventStore>();
builder.Services.AddSingleton<ICommandBus, InMemoryCommandBus>();
builder.Services.AddCommandHandlers();
builder.Services.AddEventHandlers();

builder.Services.AddCors(options => {
    options.AddPolicy("AllowAllOrigins", builder =>  builder
        .AllowAnyOrigin()
        .AllowAnyHeader()
        .AllowAnyMethod()
    );
});

builder.Services.AddControllers();
builder.Services.AddOpenApi();

var app = builder.Build();

// Load Application Data.
var logger = app.Services.GetRequiredService<ILogger<Program>>();
var eventStore = app.Services.GetRequiredService<IEventStore>();
var eventBus = (InMemoryEventBus)app.Services.GetRequiredService<IEventBus>();

logger.LogInformation("Rehydrating Projections");
var eventCount = 0;
await foreach (var @event in eventStore.GetEventsAsync())
{
    eventBus.ApplyToHandlers(@event);
    eventCount++;
}
logger.LogInformation("Total events rehydrated: {EventCount}", eventCount);

if(app.Environment.IsDevelopment() && eventCount == 0) {
    var commandBus = app.Services.GetRequiredService<ICommandBus>();
    await commandBus.SendAsync(TestData.GetTestCommands().ToArray());
}

// Set up HTTP Request Pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapScalarApiReference("/", options => {
        options
        .WithTitle("Flashcards API")
        .WithClientButton(false)
        .WithDownloadButton(false)
        .WithModels(false);
    });
}
app.UseCors("AllowAllOrigins");
app.UseAuthorization();
app.MapControllers();

app.Run();
