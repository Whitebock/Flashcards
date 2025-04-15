using Flashcards;
using Flashcards.Api;
using Flashcards.CQRS;
using Scalar.AspNetCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddOpenApi();
builder.Services.AddSingleton<IEventBus, InMemoryEventBus>();
builder.Services.AddSingleton<IEventStore, InMemoryEventStore>();
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

var commandBus = app.Services.GetRequiredService<ICommandBus>();
foreach (var command in TestData.GetTestCommands()) commandBus.Send(command);

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
