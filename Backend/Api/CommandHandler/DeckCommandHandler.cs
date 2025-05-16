using Flashcards.Common.Messages;
using Flashcards.Common.Messages.Commands;
using Flashcards.Common.Messages.Events;
using Flashcards.Common.ServiceBus;

namespace Flashcards.Api.CommandHandler;

public class DeckCommandHandler(IEventSender eventSender) : 
    ICommandHandler<CreateDeckCommand>, 
    ICommandHandler<CreateCardCommand>,
    ICommandHandler<UpdateCardCommand>,
    ICommandHandler<DeleteCardCommand>,
    ICommandHandler<ChangeCardStatusCommand>,
    ICommandHandler<UpdateDeckCommand>,
    ICommandHandler<DeleteDeckCommand>,
    ICommandHandler<AddTagCommand>,
    ICommandHandler<RemoveTagCommand>
{
    public async Task HandleAsync(CreateDeckCommand command)
    {
        await eventSender.PublishAsync(new DeckCreated(command.DeckId, command.Name, command.Description){Creator = command.Creator});
    }

    public async Task HandleAsync(CreateCardCommand command)
    {
        await eventSender.PublishAsync(new CardCreated(command.CardId, command.DeckId, command.Front, command.Back){Creator = command.Creator});
    }

    public async Task HandleAsync(UpdateCardCommand command)
    {
        await eventSender.PublishAsync(new CardUpdated(command.CardId, command.Front, command.Back){Creator = command.Creator});
    }

    public async Task HandleAsync(DeleteCardCommand command)
    {
        await eventSender.PublishAsync(new CardDeleted(command.CardId){Creator = command.Creator});
    }

    public async Task HandleAsync(ChangeCardStatusCommand command)
    {
        await eventSender.PublishAsync(new CardStatusChanged(command.CardId, command.Status){Creator = command.Creator});
    }

    public async Task HandleAsync(UpdateDeckCommand command)
    {
        await eventSender.PublishAsync(new DeckUpdated(command.DeckId, command.Name, command.Description){Creator = command.Creator});
    }

    public async Task HandleAsync(DeleteDeckCommand command)
    {
        await eventSender.PublishAsync(new DeckDeleted(command.DeckId){Creator = command.Creator});
    }

    public async Task HandleAsync(AddTagCommand command)
    {
        await eventSender.PublishAsync(new TagAdded(command.DeckId, command.Tag){Creator = command.Creator});
    }

    public async Task HandleAsync(RemoveTagCommand command)
    {
        await eventSender.PublishAsync(new TagRemoved(command.DeckId, command.Tag){Creator = command.Creator});
    }
}