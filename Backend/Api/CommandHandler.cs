using System.Threading.Tasks;
using Flashcards.Commands;
using Flashcards.CQRS;
using Flashcards.Events;

namespace Flashcards.Api;

public class CommandHandler(IEventBus _eventBus) : 
    ICommandHandler<CreateDeckCommand>, 
    ICommandHandler<CreateCardCommand>,
    ICommandHandler<UpdateCardCommand>,
    ICommandHandler<DeleteCardCommand>,
    ICommandHandler<ChangeCardStatus>,
    ICommandHandler<UpdateDeckCommand>,
    ICommandHandler<DeleteDeckCommand>
{
    public async Task HandleAsync(CreateDeckCommand command)
    {
        await _eventBus.PublishAsync(new DeckCreated(command.DeckId, command.Name, command.Description){Creator = command.Creator});
    }

    public async Task HandleAsync(CreateCardCommand command)
    {
        await _eventBus.PublishAsync(new CardCreated(command.CardId, command.DeckId, command.Front, command.Back){Creator = command.Creator});
    }

    public async Task HandleAsync(UpdateCardCommand command)
    {
        await _eventBus.PublishAsync(new CardUpdated(command.CardId, command.Front, command.Back){Creator = command.Creator});
    }

    public async Task HandleAsync(DeleteCardCommand command)
    {
        await _eventBus.PublishAsync(new CardDeleted(command.CardId){Creator = command.Creator});
    }

    public async Task HandleAsync(ChangeCardStatus command)
    {
        await _eventBus.PublishAsync(new CardStatusChanged(command.CardId, command.Status){Creator = command.Creator});
    }

    public async Task HandleAsync(UpdateDeckCommand command)
    {
        await _eventBus.PublishAsync(new DeckUpdated(command.DeckId, command.Name, command.Description){Creator = command.Creator});
    }

    public async Task HandleAsync(DeleteDeckCommand command)
    {
        await _eventBus.PublishAsync(new DeckDeleted(command.DeckId){Creator = command.Creator});
    }
}