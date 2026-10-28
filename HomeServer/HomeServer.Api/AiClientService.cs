using Microsoft.Extensions.AI;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.ChatCompletion;
using System.Text;
using System.Text.Json;

namespace HomeServer.Api
{
    internal sealed record TitleOutput(string Title);

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
            // Creating the schema for output from the AI model.
            JsonElement outputSchema = JsonSerializer.SerializeToElement(new
            {
                type = "object",
                properties = new
                {
                    title = new { type = "string" }
                },
                required = new[] { "title" },
                additionalProperties = false
            });

            // Creating the prompt for the AI model.
            ChatMessage message = new(
                ChatRole.User,
                $"Generate a title for a chat with the following first message: {firstMessage}");

            // Passing the prompt through to the AI model.
            ChatResponse response = await aiClient.AsChatClient().GetResponseAsync(message, new ChatOptions
            {
                ResponseFormat = ChatResponseFormatJson.ForJsonSchema(outputSchema, "chat_title", ""),
            });

            if (response is null)
            {
                logger.LogWarning("No title could be generated for this chat.");
                return string.Empty;
            }

            // Deserializing the response from the AI model into a TitleOutput object.
            TitleOutput? title = JsonSerializer.Deserialize<TitleOutput>(response.Text);

            // Returning the title if it was successfully generated, or an empty string if not.
            return title?.Title ?? string.Empty;
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
