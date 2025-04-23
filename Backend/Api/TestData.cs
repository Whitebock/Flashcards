using Flashcards.Commands;
using Flashcards.CQRS;

namespace Flashcards.Api;

public static class TestData
{
    public static List<ICommand> GetTestCommands()
    {
        var creator = Guid.NewGuid();
        var mathDeckId = Guid.NewGuid();
        var spanishDeckId = Guid.NewGuid();
        var japaneseDeckId = Guid.NewGuid();
        var geographyDeckId = Guid.NewGuid();

        return new List<ICommand>([
            new CreateDeckCommand("Maths", "A deck for practicing basic math problems.") { DeckId = mathDeckId, Creator = creator },
            new CreateCardCommand(mathDeckId, "What is 2 + 2?", "4") { Creator = creator },
            new CreateCardCommand(mathDeckId, "What is 3 * 3?", "9") { Creator = creator },
            new CreateCardCommand(mathDeckId, "What is 5 - 2?", "3") { Creator = creator },
            new CreateCardCommand(mathDeckId, "What is 10 / 2?", "5") { Creator = creator },
            new CreateCardCommand(mathDeckId, "What is 7 + 8?", "15") { Creator = creator },

            new CreateDeckCommand("Spanish", "A deck for practicing basic Spanish phrases.") { DeckId = spanishDeckId, Creator = creator },
            new CreateCardCommand(spanishDeckId, "¿Cómo se dice 'hello' en español?", "hola") { Creator = creator },
            new CreateCardCommand(spanishDeckId, "¿Cómo se dice 'goodbye' en español?", "adiós") { Creator = creator },

            new CreateDeckCommand("Japanese", "A deck for practicing basic Japanese phrases.") { DeckId = japaneseDeckId, Creator = creator },
            new CreateCardCommand(japaneseDeckId, "What is 'hello' in Japanese?", "こんにちは") { Creator = creator },
            new CreateCardCommand(japaneseDeckId, "What is 'goodbye' in Japanese?", "さようなら") { Creator = creator },
            new CreateCardCommand(japaneseDeckId, "What is 'thank you' in Japanese?", "ありがとう") { Creator = creator },
            new CreateCardCommand(japaneseDeckId, "What is 'yes' in Japanese?", "はい") { Creator = creator },
            new CreateCardCommand(japaneseDeckId, "What is 'no' in Japanese?", "いいえ") { Creator = creator },
            new CreateCardCommand(japaneseDeckId, "What is 'please' in Japanese?", "お願いします") { Creator = creator },
            new CreateCardCommand(japaneseDeckId, "What is 'excuse me' in Japanese?", "すみません") { Creator = creator },

            new CreateDeckCommand("Geography", "A deck for practicing geography knowledge.") { DeckId = geographyDeckId, Creator = creator },
            new CreateCardCommand(geographyDeckId, "What is the capital of France?", "Paris") { Creator = creator },
            new CreateCardCommand(geographyDeckId, "What is the largest continent?", "Asia") { Creator = creator },
            new CreateCardCommand(geographyDeckId, "What is the longest river in the world?", "Nile") { Creator = creator },
            new CreateCardCommand(geographyDeckId, "What is the smallest country in the world?", "Vatican City") { Creator = creator },
            new CreateCardCommand(geographyDeckId, "What is the highest mountain in the world?", "Mount Everest") { Creator = creator }
        ]);
    }
}