using System.Text.Json.Serialization;

namespace Flashcards.Common.Messages;

[JsonConverter(typeof(JsonStringEnumConverter<CardStatus>))]
public enum CardStatus {
    NotSeen,
    Again,
    Good,
    Easy
}