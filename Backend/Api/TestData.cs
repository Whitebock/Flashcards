using Flashcards.Commands;
using Flashcards.CQRS;

namespace Flashcards.Api;

public static class TestData
{
    public static List<ICommand> GetTestCommands()
    {
        var mathDeckId = Guid.NewGuid();
        var spanishDeckId = Guid.NewGuid();
        var japaneseDeckId = Guid.NewGuid();
        var geographyDeckId = Guid.NewGuid();

        return
        [
            new CreateDeckCommand("Maths", "A deck for practicing basic math problems.") { DeckId = mathDeckId },
            new CreateCardCommand(mathDeckId, "What is 2 + 2?", "4"),
            new CreateCardCommand(mathDeckId, "What is 3 * 3?", "9"),
            new CreateCardCommand(mathDeckId, "What is 5 - 2?", "3"),
            new CreateCardCommand(mathDeckId, "What is 10 / 2?", "5"),
            new CreateCardCommand(mathDeckId, "What is 7 + 8?", "15"),

            new CreateDeckCommand("Spanish", "A deck for practicing basic Spanish phrases.") { DeckId = spanishDeckId },
            new CreateCardCommand(spanishDeckId, "¿Cómo se dice 'hello' en español?", "hola"),
            new CreateCardCommand(spanishDeckId, "¿Cómo se dice 'goodbye' en español?", "adiós"),

            new CreateDeckCommand("Japanese", "A deck for practicing basic Japanese phrases.") { DeckId = japaneseDeckId },
            new CreateCardCommand(japaneseDeckId, "What is 'hello' in Japanese?", "こんにちは"),
            new CreateCardCommand(japaneseDeckId, "What is 'goodbye' in Japanese?", "さようなら"),
            new CreateCardCommand(japaneseDeckId, "What is 'thank you' in Japanese?", "ありがとう"),
            new CreateCardCommand(japaneseDeckId, "What is 'yes' in Japanese?", "はい"),
            new CreateCardCommand(japaneseDeckId, "What is 'no' in Japanese?", "いいえ"),
            new CreateCardCommand(japaneseDeckId, "What is 'please' in Japanese?", "お願いします"),
            new CreateCardCommand(japaneseDeckId, "What is 'excuse me' in Japanese?", "すみません"),

            new CreateDeckCommand("Geography", "A deck for practicing geography knowledge.") { DeckId = geographyDeckId },
            new CreateCardCommand(geographyDeckId, "What is the capital of France?", "Paris"),
            new CreateCardCommand(geographyDeckId, "What is the largest continent?", "Asia"),
            new CreateCardCommand(geographyDeckId, "What is the longest river in the world?", "Nile"),
            new CreateCardCommand(geographyDeckId, "What is the smallest country in the world?", "Vatican City"),
            new CreateCardCommand(geographyDeckId, "What is the highest mountain in the world?", "Mount Everest")
        ];
    }
}