using System.ComponentModel;
using Flashcards.Common.Messages;

namespace Flashcards.Api.Models;

public class Card
{
    [ReadOnly(true)]
    public Guid Id { get; set; }
    public Guid? DeckId { get; set; }
    public string? Front { get; set; }
    public string? Back { get; set; }
    public CardStatus? Status { get; set; }
}