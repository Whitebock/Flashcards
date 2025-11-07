using Flashcards.Common.Interfaces;
using Flashcards.Common.Messages;
using Flashcards.Common.Messages.Commands;
using KafkaFlow;

namespace Flashcards.Common.Infrastructure.Producers;

public class DeckCommandProducer(IMessageProducer<ICommand> producer) : IDeckManager
{
    public async Task CreateAsync(Guid userId, string name, string description, Guid? deckId = null)
    {
        deckId ??= Guid.NewGuid();
        await producer.ProduceAsync(deckId.ToString(), new CreateDeckCommand(name, description)
        {
            Creator = userId,
            DeckId = deckId.Value
        });
    }

    public async Task UpdateAsync(Guid userId, Guid deckId, string name, string description)
    {
        await producer.ProduceAsync(deckId.ToString(), new UpdateDeckCommand(deckId, name, description)
        {
            Creator = userId
        });
    }

    public async Task RenameAsync(Guid userId, Guid deckId, string name)
    {
        throw new NotImplementedException();
    }

    public async Task ChangeDescriptionAsync(Guid userId, Guid deckId, string description)
    {
        throw new NotImplementedException();
    }

    public async Task DeleteAsync(Guid userId, Guid deckId)
    {
        await producer.ProduceAsync(deckId.ToString(), new DeleteDeckCommand(deckId) { Creator = userId });
    }

    public async Task AddTagAsync(Guid userId, Guid deckId, string tag)
    {
        await producer.ProduceAsync(deckId.ToString(), new AddTagCommand(deckId, tag) { Creator = userId });
    }
}