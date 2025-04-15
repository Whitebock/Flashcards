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
    public void Handle(CreateDeckCommand command)
    {
        _eventBus.Publish(new DeckCreated(command.DeckId, command.Name, command.Description));
    }

    public void Handle(CreateCardCommand command)
    {
        _eventBus.Publish(new CardCreated(command.CardId, command.DeckId, command.Front, command.Back));
        _eventBus.Publish(new CardStatusChanged(command.CardId, CardStatus.NotSeen));
    }

    public void Handle(UpdateCardCommand command)
    {
        _eventBus.Publish(new CardUpdated(command.CardId, command.Front, command.Back));
    }

    public void Handle(DeleteCardCommand command)
    {
        _eventBus.Publish(new CardDeleted(command.CardId));
    }

    public void Handle(ChangeCardStatus command)
    {
        _eventBus.Publish(new CardStatusChanged(command.CardId, command.Status));
    }

    public void Handle(UpdateDeckCommand command)
    {
        _eventBus.Publish(new DeckUpdated(command.DeckId, command.Name, command.Description));
    }

    public void Handle(DeleteDeckCommand command)
    {
        _eventBus.Publish(new DeckDeleted(command.DeckId));
    }
}