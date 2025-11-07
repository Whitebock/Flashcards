using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using Flashcards.Common.Interfaces;

namespace Flashcards.Api.Models;

[Description("An activity of a feed.")]
public class Activity
{
    
    [RegularExpression("deck_added|deck_edited|cards_added")]
    public required string Type { get; set; }
    public required IUser User { get; set; }
    public required DateTime OccurredAt { get; set; }
    public required string DeckName { get; set; }
    public required int Count { get; set; }
}