using System.Text.Json;
using Flashcards.Common.Messages;
using Microsoft.Extensions.Logging;

namespace Flashcards.Common.ServiceBus;

public class JsonLinesServiceBusQueue(string filePath, ILogger logger) : IServiceBusQueue
{
    private JsonSerializerOptions SerializerOptions { get; } = new() { TypeInfoResolver = new JsonMessageTypeResolver() };

    public event Action<IMessage> MessageReceived = delegate {};
    public bool IsProcessing { get; private set; } = false;
    private SemaphoreSlim semaphore = new(1, 1);

    public async Task PublishAsync(IMessage message, CancellationToken cancellationToken = default)
    {
        await semaphore.WaitAsync(cancellationToken);
        if (cancellationToken.IsCancellationRequested)
        {
            semaphore.Release();
            return;
        }
        var json = JsonSerializer.Serialize(message, SerializerOptions);
        await using var stream = await GetFileLockAsync(filePath, options =>
        {
            options.Access = FileAccess.Write;
            options.Mode = FileMode.Append;
        }, cancellationToken);
        if (cancellationToken.IsCancellationRequested) return;

        using var writer = new StreamWriter(stream);
        await writer.WriteLineAsync(json);
        await writer.FlushAsync(cancellationToken);
        writer.Close();
        semaphore.Release();
    }

    internal async Task ProcessAsync(CancellationToken cancellationToken = default)
    {
        IsProcessing = true;
        await semaphore.WaitAsync(cancellationToken);
        await using var stream = await GetFileLockAsync(filePath, null, cancellationToken);
        if (cancellationToken.IsCancellationRequested) {
            semaphore.Release();
            IsProcessing = false;
            return;
        };

        using var reader = new StreamReader(stream);
        string? json;
        while ((json = await reader.ReadLineAsync(cancellationToken)) is not null)
        {
            if (string.IsNullOrWhiteSpace(json)) continue;
            var message = JsonSerializer.Deserialize<IMessage>(json, SerializerOptions) ??
                    throw new InvalidOperationException("Failed to deserialize message.");

            logger.LogDebug("Messaged recieved on queue {Queue}: {MessageType}", Path.GetFileNameWithoutExtension(filePath), message.GetType().Name);
            MessageReceived.Invoke(message);
        }
        stream.SetLength(0); // Truncate file to empty the queue
        await stream.FlushAsync(cancellationToken);
        semaphore.Release();
        IsProcessing = false;
    }

    private async Task<FileStream> GetFileLockAsync(string filePath, Action<FileStreamOptions>? configure = null, CancellationToken cancellationToken = default)
    {
        var options = new FileStreamOptions()
        {
            Mode = FileMode.Open,
            Access = FileAccess.ReadWrite,
            Share = FileShare.None
        };
        configure?.Invoke(options);
        var retryCount = 0;
        while (!cancellationToken.IsCancellationRequested)
        {
            try
            {
                return new FileStream(filePath, options);
            }
            catch (IOException)
            {
                retryCount++;
            }

            var retryDelay = retryCount * 200;
            if(retryCount >= 3) 
                logger.LogWarning("Failed to get file lock for {FilePath}, retrying in {RetryDelay}", Path.GetFileName(filePath), retryDelay);

            await Task.Delay(retryDelay, cancellationToken);
        }
        return (FileStream)Stream.Null;
    }
}