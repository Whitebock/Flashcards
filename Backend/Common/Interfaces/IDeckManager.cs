namespace Flashcards.Common.Interfaces;

public interface IDeckManager
{
    public Task CreateAsync(Guid userId, string name, string description, Guid? deckId = null);
    public Task UpdateAsync(Guid userId, Guid deckId, string name, string description);
    public Task RenameAsync(Guid userId, Guid deckId, string name);
    public Task ChangeDescriptionAsync(Guid userId, Guid deckId, string description);
    public Task DeleteAsync(Guid userId, Guid deckId);
    public Task AddTagAsync(Guid userId, Guid deckId, string tag);
}