using Microsoft.Extensions.DependencyInjection;
using Flashcards.Common.Messages;

namespace Flashcards.Common;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddCommandHandlers(this IServiceCollection services)
    {
        services.Scan(scan => scan.FromEntryAssembly()
            .AddClasses(classes => classes.AssignableTo<ICommandHandler>())
            .As<ICommandHandler>()
            .WithSingletonLifetime()
        );

        return services;
    }

    public static IServiceCollection AddEventHandlers(this IServiceCollection services)
    {
        services.Scan(scan => scan.FromEntryAssembly()
            .AddClasses(classes => classes.AssignableTo<IEventHandler>())
            .AsSelfWithInterfaces()
            .WithSingletonLifetime()
        );

        return services;
    }
}