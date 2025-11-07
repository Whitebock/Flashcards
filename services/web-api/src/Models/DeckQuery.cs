using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;

namespace Flashcards.Api.Models;

public class DeckQuery
{
        [Description("Username to filter decks by. Will return an error if the user was not found.")]
        [FromQuery(Name = "user")]
        public string? User { get; set; }

        [Description("Deckname to filter decks by.")]
        [FromQuery(Name = "name")]
        public string? Name { get; set; }

        [Description("Amount of results to return.")] 
        [DefaultValue(20)]
        [Range(1, 100, ErrorMessage = "Amount must be between 1 and 100.")]
        [FromQuery(Name = "amount")]
        public int Amount { get; set; } = 20;

        [Description("Sort order of the results.")]
        [DefaultValue("last-updated")]
        [RegularExpression("last-updated|popular|new", ErrorMessage = "Sort must be either 'updated', 'popular' or 'new'.")]
        [FromQuery(Name = "sort")]
        public string Sort { get; set; } = "last-updated";
}