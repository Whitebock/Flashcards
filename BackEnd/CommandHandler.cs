using Flashcards.Commands;
using Flashcards.CQRS;
using Flashcards.Events;

namespace Flashcards;

public class CommandHandler(IEventBus _eventBus) : 
    ICommandHandler<CreateDeckCommand>, 
    ICommandHandler<CreateCardCommand>,
    ICommandHandler<ChangeCardStatus>
{
    public void Handle(CreateDeckCommand command)
    {
        _eventBus.Publish(new DeckCreated(command.DeckId, command.Name));
    }

    public void Handle(CreateCardCommand command)
    {
        _eventBus.Publish(new CardCreated(command.CardId, command.DeckId, command.Front, command.Back));
        _eventBus.Publish(new CardStatusChanged(command.CardId, CardStatus.NotSeen));
    }

    public void Handle(ChangeCardStatus command)
    {
        _eventBus.Publish(new CardStatusChanged(command.CardId, command.Status));
    }
}