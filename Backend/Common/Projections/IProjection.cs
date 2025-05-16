namespace Flashcards.Common.Projections;

public interface IProjection<T> where T : class
{
    Task<T> GetAsync();
}
