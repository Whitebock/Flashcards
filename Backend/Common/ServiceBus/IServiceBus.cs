using Microsoft.Extensions.Hosting;

namespace Flashcards.Common.ServiceBus;

public interface IServiceBus : IHostedService
{
    Task<IServiceBusQueue> GetQueueAsync(string name, CancellationToken cancellationToken = default);
}