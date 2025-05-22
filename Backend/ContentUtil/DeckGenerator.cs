using Microsoft.SemanticKernel.ChatCompletion;

namespace Flashcards.ContentUtil;

public class DeckGenerator(IChatCompletionService chatCompletion)
{
    public async Task<(string Name, string Description)> GenerateForTopicAsync(string topic)
    {
        var response = await chatCompletion.GetChatMessageContentAsync(new ChatHistory("""
        Generate a name and description for a flashcard deck for the given topic.
        The description should be a single sentence.

        Topic:
        #TOPIC

        Example Response:
        JLPT N5 - Grammar
        All the required Grammar to pass the Japanese-Language Proficiency Test (N5 Level).
        """.Replace("#TOPIC", topic)));
        var parts = response.Content!.Split("\n").Select(p => p.Trim()).ToArray();
        return (parts[0], parts[1]);
    }

    public async Task<IEnumerable<(string Front, string Back)>> GenerateCardsAsync(string name, string description, int amount)
    {
        var prompt = """
        Generate cards for a flashcard deck with the given name and description.
        Each cards has a front and a backside.

        Name: #NAME
        Description: #DESC
        Amount: #AMOU

        Response Format:
        FRONT;BACK
        FRONT;BACK
        FRONT;BACK
        """
        .Replace("#NAME", name)
        .Replace("#DESC", description)
        .Replace("#AMOU", amount.ToString());
        var response = await chatCompletion.GetChatMessageContentAsync(new ChatHistory(prompt));
        return response.Content!.Split('\n').Select(line => line.Split(';')).Select(parts => (parts[0], parts[1]));
    }
}