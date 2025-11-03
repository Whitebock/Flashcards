using Flashcards.Common.Messages;

namespace Flashcards.Common.Interfaces;

public interface ICardManager
{
    public Task CreateAsync(Guid userId, Guid deckId, string front, string back);
    public Task UpdateAsync(Guid userId, Guid cardId, string front, string back);
    public Task ChangeStatusAsync(Guid userId, Guid cardId, CardStatus status);
    public Task DeleteAsync(Guid userId, Guid cardId);
}