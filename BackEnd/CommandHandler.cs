using Flashcards.Commands;
using Flashcards.CQRS;
using Flashcards.Events;

namespace Flashcards;

public class CommandHandler : ICommandHandler<CreateDeckCommand>, ICommandHandler<CreateCardCommand>
{
    private readonly IEventBus _eventBus;

    public CommandHandler(IEventBus eventBus)
    {
        _eventBus = eventBus;
    }

    public void Handle(CreateDeckCommand command)
    {
        _eventBus.Publish(new DeckCreated(command.Id, command.Name));
    }

    public void Handle(CreateCardCommand command)
    {
        _eventBus.Publish(new CardCreated(command.DeckId, command.Front, command.Back));
    }
}