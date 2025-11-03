using Flashcards.Common.Interfaces;
using Flashcards.Common.Messages;
using Flashcards.Common.Messages.Commands;
using KafkaFlow;

namespace Flashcards.Common.Infrastructure.Producers;

public class CardCommandProducer(IMessageProducer<ICommand> producer) : ICardManager
{
    public async Task CreateAsync(Guid userId, Guid deckId, string front, string back)
    {
        var cardId = Guid.NewGuid();
        await producer.ProduceAsync(cardId.ToString(), new CreateCardCommand(deckId, front, back)
        {
            Creator = userId,
            CardId = cardId
        });
    }

    public async Task UpdateAsync(Guid userId, Guid cardId, string front, string back)
    {
        await producer.ProduceAsync(cardId.ToString(), new UpdateCardCommand(cardId, front, back)
        {
            Creator = userId
        });
    }

    public async Task ChangeStatusAsync(Guid userId, Guid cardId, CardStatus status)
    {
        await producer.ProduceAsync(cardId.ToString(), new ChangeCardStatusCommand(cardId, status)
        {
            Creator = userId
        });
    }

    public async Task DeleteAsync(Guid userId, Guid cardId)
    {
        await producer.ProduceAsync(cardId.ToString(), new DeleteCardCommand(cardId)
        {
            Creator = userId
        });
    }
}