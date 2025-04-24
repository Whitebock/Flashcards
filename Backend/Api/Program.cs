using Flashcards.Api;
using Flashcards.Common;
using Flashcards.Common.EventStore;
using Flashcards.Common.ServiceBus;
using Flashcards.Common.UserManagement;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Logging;
using Scalar.AspNetCore;

var builder = WebApplication.CreateBuilder(args);

builder.Configuration.AddIniFile("appsettings.ini");

builder.Services
    .AddSingleton<IEventBus, InMemoryEventBus>()
    .AddSingleton<IEventStore, JsonLinesEventStore>()
    .Configure<JsonLinesEventStoreOptions>(options => { options.FilePath = "events.jsonl"; })
    .AddSingleton<ICommandBus, InMemoryCommandBus>()
    .AddCommandHandlers()
    .AddEventHandlers()
    .Configure<Auth0UserStoreOptions>(options => {
        options.Token = builder.Configuration["Auth0:Token"];
        options.Endpoint = new Uri(builder.Configuration["Auth0:Endpoint"]!);
    })
    .AddSingleton<IUserStore, Auth0UserStore>();

builder.Services.AddCors(options => {
    options.AddPolicy("AllowAllOrigins", builder =>  builder
        .AllowAnyOrigin()
        .AllowAnyHeader()
        .AllowAnyMethod()
    );
});

builder.Services.AddControllers();
builder.Services.AddOpenApi();

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
        .WithDownloadButton(false)
        .WithModels(false);
    });
}
app.UseCors("AllowAllOrigins");
app.MapControllers();

app.Run();
