namespace Flashcards.Common.Interfaces;

public interface IDeckActivityRepository
{
    int GetDeckActivity(Guid deckId);
}