using Microsoft.Extensions.DependencyInjection;
using Flashcards.Common.Messages;
using System.Reflection;
using Flashcards.Common.Projections;

namespace Flashcards.Common;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddCommandHandler<T>(this IServiceCollection services) where T : class, ICommandHandler
    {
        services.AddSingleton<ICommandHandler, T>();
        return services;
    }
    private static IServiceCollection AddCommandHandlers(this IServiceCollection services)
    {
        services.Scan(scan => scan.FromEntryAssembly()
            .AddClasses(classes => classes.AssignableTo<ICommandHandler>())
            .As<ICommandHandler>()
            .WithSingletonLifetime()
        );

        return services;
    }

    public static IServiceCollection AddProjection<T>(this IServiceCollection services) where T : class
    {
        services.AddTransient<T>();
        services.AddTransient<IProjection<T>, HydratedProjection<T>>();
        return services;
    }

    private static IServiceCollection AddEventHandlers(this IServiceCollection services)
    {
        services.Scan(scan => scan.FromAssemblies(Assembly.GetExecutingAssembly(), Assembly.GetEntryAssembly()!)
            .AddClasses(classes => classes.AssignableTo<IEventHandler>())
            .AsSelfWithInterfaces()
            .WithSingletonLifetime()
        );

        return services;
    }
}