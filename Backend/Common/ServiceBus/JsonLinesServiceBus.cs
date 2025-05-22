using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Flashcards.Common.ServiceBus;

public class JsonLinesServiceBus(ILogger<JsonLinesServiceBus> logger, IOptions<JsonLinesServiceBusOptions> options) : IServiceBus, IDisposable
{
    private readonly Dictionary<string, JsonLinesServiceBusQueue> _queues = [];
    private FileSystemWatcher? watcher;

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        watcher = new(options.Value.Dictionary, "servicebus.*.jsonl")
        {
            EnableRaisingEvents = true,
            InternalBufferSize = 24000,
            IncludeSubdirectories = false,
            NotifyFilter = NotifyFilters.LastWrite
        };
        watcher.Changed += async (source, args) =>
        {
            var name = args.Name?.Split('.').ElementAtOrDefault(1) ??
                throw new InvalidOperationException("Recieved update from unknown queue.");

            SyncQueues();
            if(!_queues[name].IsProcessing)
                await _queues[name].ProcessAsync();
        };
        
        SyncQueues();
        foreach (var queue in _queues.Values)
        {
            await queue.ProcessAsync();
        }
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        watcher!.EnableRaisingEvents = false;

        var fileInfos = Directory.GetFiles(options.Value.Dictionary, "servicebus.*.jsonl")
            .Select(file => new FileInfo(file));
        foreach (var file in fileInfos)
        {
            if (file.Length == 0)
            {
                file.Delete();
            }
        }
        return Task.CompletedTask;
    }

    public void Dispose()
    {
        watcher?.Dispose();
    }

    private void SyncQueues()
    {
        var fileQueues = Directory.GetFiles(options.Value.Dictionary, "servicebus.*.jsonl")
            .Select(file => Path.GetFileName(file).Split('.')[1]);
        
        // Add new queues.
        foreach (var name in fileQueues.Except(_queues.Keys))
        {
            logger.LogDebug("Adding existing Queue: {Queue}", name);
            var queue = new JsonLinesServiceBusQueue(Path.Combine(options.Value.Dictionary, $"servicebus.{name}.jsonl"), logger);
            _queues.Add(name, queue);
        }
        
        // Remove deleted queues.
        foreach (var name in _queues.Keys.Except(fileQueues))
        {
            logger.LogDebug("Removing deleted Queue: {Queue}", name);
            _queues.Remove(name);
        }
    }

    public Task<IServiceBusQueue> GetQueueAsync(string name, CancellationToken cancellationToken = default)
    {
        SyncQueues();

        if (!_queues.TryGetValue(name, out var queue))
        {
            logger.LogDebug("Creating new Queue: {Queue}", name);
            var filePath = Path.Combine(options.Value.Dictionary, $"servicebus.{name}.jsonl");
            File.Create(filePath).Close();
            queue = new JsonLinesServiceBusQueue(filePath, logger);
            _queues.Add(name, queue);
        }
        return Task.FromResult((IServiceBusQueue) queue);
    }
}