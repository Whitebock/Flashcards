using System.ComponentModel;
using Flashcards.Api;
using Flashcards.Api.CommandHandler;
using Flashcards.Api.Projections;
using Flashcards.Common;
using Flashcards.Common.EventStore;
using Flashcards.Common.Projections;
using Flashcards.Common.ServiceBus;
using Flashcards.Common.UserManagement;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Logging;
using Microsoft.OpenApi.Any;
using Microsoft.OpenApi.Models;
using Scalar.AspNetCore;

var builder = WebApplication.CreateBuilder(args);

builder.Configuration.AddIniFile("appsettings.ini");

builder.Services
    .AddSingleton<IServiceBus, InMemoryServiceBus>()
    .AddHostedService(provider => provider.GetRequiredService<IServiceBus>())
    .AddSingleton<ICommandSender, ServiceBusCommandSender>()
    .AddSingleton<IEventSender, ServiceBusEventSender>()
    .AddSingleton<IEventStore, JsonLinesEventStore>()
    .Configure<JsonLinesEventStoreOptions>(options => { options.FilePath = "../events.jsonl"; })
    .AddProjection<DeckListProjection>()
    .AddProjection<CardProjection>()
    .AddProjection<UserIdProjection>()
    .AddHostedService<CommandHandlerService>()
    .AddCommandHandler<DeckCommandHandler>()
    .Configure<Auth0UserStoreOptions>(options => {
        options.Token = builder.Configuration["Auth0:Token"];
        options.Endpoint = new Uri(builder.Configuration["Auth0:Endpoint"]!);
    })
    .AddSingleton<IUserStore, Auth0UserStore>()
    .Decorate<IUserStore, CachedUserStore>()
    .Decorate<IUserStore, DeletedUserStore>()
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
