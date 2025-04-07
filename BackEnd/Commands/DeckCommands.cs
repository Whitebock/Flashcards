namespace Flashcards.Commands;

public record CreateDeckCommand(Guid Id, string Name) : CommandBase;

public record UpdateDeckCommand(Guid Id, string Name) : CommandBase;

public record DeleteDeckCommand(Guid Id) : CommandBase;