using System.Text.Json;
using Flashcards.Common.Messages;
using Microsoft.Extensions.Options;

namespace Flashcards.Common.EventStore;

public class JsonLinesEventStore(IOptions<JsonLinesEventStoreOptions> options) : IEventStore
{
    public string FullPath => Path.GetFullPath(options.Value.FilePath);
    public JsonSerializerOptions SerializerOptions {get;} = new() {
        TypeInfoResolver = new JsonMessageTypeResolver()
    };
    
    public async IAsyncEnumerable<IEvent> GetEventsAsync() {
        if (!File.Exists(FullPath)) yield break;
        
        var lines = await File.ReadAllLinesAsync(FullPath);
        foreach (var json in lines)
        {
            var @event = JsonSerializer.Deserialize<IEvent>(json, SerializerOptions) ??
                throw new InvalidOperationException("Failed to deserialize event.");
            yield return @event;
        }
    }

    public async Task SaveAsync(IEvent @event)
    {
        var json = JsonSerializer.Serialize(@event, SerializerOptions);
        await File.AppendAllLinesAsync(FullPath, [json]);
    }
}
