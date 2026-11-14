using Microsoft.Extensions.AI;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.ChatCompletion;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace HomeServer.Api
{
    /// <summary>
    /// Dictates the structured output format for the AI's response in a chat.
    /// </summary>
    internal sealed record TitleOutput([property: JsonPropertyName("title")] string Title);

    /// <summary>
    /// Manages interactions with the AI model.
    /// </summary>
    public class AiClientService(
        IChatCompletionService aiClient,
        ILogger<AiClientService> logger)
    {
        public const string SystemPrompt =
            """
            # IDENTITY

            You are an AI assistant named Caide.
            """;
        private PromptExecutionSettings _settings = new() { };

        /// <summary>
        /// Generates a title for a chat based on the first message.
        /// </summary>
        public async Task<string> GenerateChatTitle(string firstMessage)
        {
            string prompt =
                $"""
                Generate a concise title for a chat whose first message is below.
                {firstMessage}
                """;

            // Sending the prompt as a message to the AI to respond with structured JSON output.
            ChatMessage message = new(ChatRole.User, prompt);
            ChatResponse response = await aiClient.AsChatClient().GetResponseAsync(message, new ChatOptions
            {
                // Setting up the structured output format.
                ResponseFormat = ChatResponseFormat.ForJsonSchema<TitleOutput>(
                    JsonSerializerOptions.Web,
                    "chat_title",
                    "A concise title for the chat")
            });

            // Parsing the output to extract the title.
            TitleOutput? output = string.IsNullOrWhiteSpace(response.Text)
                ? null
                : JsonSerializer.Deserialize<TitleOutput>(response.Text, JsonSerializerOptions.Web);

            // Returning the title or a default value if parsing failed.
            string? title = output?.Title?.Trim();
            if (string.IsNullOrWhiteSpace(title))
            {
                logger.LogWarning("No title could be generated for this chat.");
                return string.Empty;
            }
            return title;
        }

        /// <summary>
        /// Receives a chat history and returns a response from the AI model based on the history.
        /// </summary>
        /// <param name="chat">The chat history of the conversation.</param>
        /// <returns>The next response in the chat.</returns>
        public async Task<string?> GetChatResponse(ChatHistory chat)
        {
            // Providing the AI with the chat history and settings to generate a response.
            ChatMessageContent response = await aiClient.GetChatMessageContentAsync(chat, _settings);

            // Ensuring a response was received.
            if (response.Content is null)
            {
                logger.LogWarning("AI response content is null.");
                return null;
            }

            // Building the response as a string.
            StringBuilder str = new();
            foreach (char message in response.Content)
            {
                str.Append(message);
            }

            // Returning the final constructed response.
            return str.ToString();
        }
    }
}
