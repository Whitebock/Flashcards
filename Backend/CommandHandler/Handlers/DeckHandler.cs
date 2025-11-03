using Flashcards.Common.Messages.Commands;
using Flashcards.Common.Messages.Events;
using KafkaFlow;

namespace CommandHandler.Handlers;

public class DeckHandler(IMessageProducer<DeckHandler> producer) : 
    IMessageHandler<CreateDeckCommand>, 
    IMessageHandler<CreateCardCommand>,
    IMessageHandler<UpdateCardCommand>,
    IMessageHandler<DeleteCardCommand>,
    IMessageHandler<ChangeCardStatusCommand>,
    IMessageHandler<UpdateDeckCommand>,
    IMessageHandler<DeleteDeckCommand>,
    IMessageHandler<AddTagCommand>,
    IMessageHandler<RemoveTagCommand>
{
    public async Task Handle(IMessageContext context, CreateDeckCommand command)
    {
        await producer.ProduceAsync(command.DeckId.ToString(), new DeckCreated(command.DeckId, command.Name, command.Description)
        {
            Creator = command.Creator
        });
    }

    public async Task Handle(IMessageContext context, CreateCardCommand command)
    {
        await producer.ProduceAsync(command.CardId.ToString(), new CardCreated(command.CardId, command.DeckId, command.Front, command.Back){Creator = command.Creator});
    }

    public async Task Handle(IMessageContext context, UpdateCardCommand command)
    {
        await producer.ProduceAsync(command.CardId.ToString(), new CardUpdated(command.CardId, command.Front, command.Back){Creator = command.Creator});
    }

    public async Task Handle(IMessageContext context, DeleteCardCommand command)
    {
        await producer.ProduceAsync(command.CardId.ToString(), new CardDeleted(command.CardId){Creator = command.Creator});
    }

    public async Task Handle(IMessageContext context, ChangeCardStatusCommand command)
    {
        await producer.ProduceAsync(command.CardId.ToString(), new CardStatusChanged(command.CardId, command.Status){Creator = command.Creator});
    }

    public async Task Handle(IMessageContext context, UpdateDeckCommand command)
    {
        await producer.ProduceAsync(command.DeckId.ToString(), new DeckUpdated(command.DeckId, command.Name, command.Description){Creator = command.Creator});
    }

    public async Task Handle(IMessageContext context, DeleteDeckCommand command)
    {
        await producer.ProduceAsync(command.DeckId.ToString(), new DeckDeleted(command.DeckId){Creator = command.Creator});
    }

    public async Task Handle(IMessageContext context, AddTagCommand command)
    {
        await producer.ProduceAsync(command.DeckId.ToString(), new TagAdded(command.DeckId, command.Tag){Creator = command.Creator});
    }

    public async Task Handle(IMessageContext context, RemoveTagCommand command)
    {
        await producer.ProduceAsync(command.DeckId.ToString(), new TagRemoved(command.DeckId, command.Tag){Creator = command.Creator});
    }
}