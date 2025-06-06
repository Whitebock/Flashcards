using System.ComponentModel;
using Flashcards.Common.UserManagement;

namespace Flashcards.Api.Models;

public class Deck 
{
    [ReadOnly(true)]
    public Guid? Id { get; set; }
    public string? Name { get; set; }
    public string? Description { get; set; }
    public HashSet<string> Tags { get; set; } = [];
    
    [ReadOnly(true)]
    [Description("Creator of this deck")]
    public IUser? Creator { get; set; }

    [ReadOnly(true)]
    [Description("Url-friendly name used for routing.")]
    public string EncodedName => Name == null ? "" : Name.ToLower().Replace(' ', '_');

    [Description("Total amount of cards in this deck.")]
    [DefaultValue(0)]
    [ReadOnly(true)]
    public int CardCount { get; set; } = 0;
    [ReadOnly(true)]
    public DeckStatistics? Statistics { get; set; } = null;
}

public class DeckStatistics
{
    [Description("Number of cards that have not been seen yet.")]
    [DefaultValue(0)]
    public int NotSeen { get; set; } = 0;

    [Description("Number of cards answered correctly.")]
    [DefaultValue(0)]
    public int Correct { get; set; } = 0;

    [Description("Number of cards answered incorrectly.")]
    [DefaultValue(0)]
    public int Incorrect { get; set; } = 0;
}