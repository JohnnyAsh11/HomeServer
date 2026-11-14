using Hangfire;
using HomeServer.Api.Controllers;
using HomeServer.Database;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.ChatCompletion;

namespace HomeServer.Api
{
    /// <summary>
    /// Interfaces with Hangfire to manage AI chats to ensuring no
    /// time outs occur and that AI chats can be handled properly.
    /// </summary>
    public class HangfireManager(
        IBackgroundJobClient client,
        HomeServerContext dbContext,
        AiClientService aiClient,
        ILogger<HangfireManager> logger)
    {
        /// <summary>
        /// Enqueues a request to AI to generate a title for a chat.
        /// </summary>
        public async Task EnqueueChatTitleRequest(string firstMessage, int chatId)
        {
            string? jobId = null;
            try
            {
                jobId = client.Enqueue(() => AiTitleResponseJob(chatId, firstMessage));
                logger.LogInformation("Successfully queued Chat Title job with ID: {id}", jobId);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error enqueuing chat title request");
                throw;
            }
        }

        /// <summary>
        /// Actually enqueues a job to get a response from the AI model for a chat.
        /// </summary>
        public async Task EnqueueChatRequest(
            List<MessageContent> messages,
            int chatId)
        {
            string? jobId = null;
            List<MessageContentDto> chatHistory = [.. messages.Select(m => m.ToMessageContentDto())];

            try
            {
                jobId = client.Enqueue(() => AiChatResponseJob(chatId, chatHistory));
                logger.LogInformation("Successfully queued Chat Message job with ID: {id}", jobId);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error enqueuing chat message request");
                throw;
            }
        }

        /// <summary>
        /// This is the actual method that interacts with the AI model to get a response for a chat.
        /// It takes in the ChatHistory, sends it to the AI model, and returns the updated
        /// ChatHistory with the AI's response.
        /// </summary>
        public async Task AiChatResponseJob(int chatId, List<MessageContentDto> messageDtos)
        {
            List<MessageContent> messages = [.. messageDtos.Select(m => m.ToMessageContent())];
            ChatHistory chatHistory = messages.ToChatHistory();

            // Receiving the AI's response for the chat.
            string? resp = await aiClient.GetChatResponse(chatHistory);
            MessageContent message = new()
            { 
                Message = resp ?? string.Empty,
                Role = OpenAI.Chat.ChatMessageRole.Assistant,
                Timestamp = DateTime.Now
            };

            // Getting the target chat from the database.
            StoredChatHistory? chat = await dbContext.ChatHistories.FindAsync(chatId);
            if (chat is null)
            {
                logger.LogInformation("Could not find a chat with ID {ChatId}", chatId);
                return;
            }

            // Adding the message and saving the content.
            chat.Messages.Add(message);
            await dbContext.SaveChangesAsync();
        }

        /// <summary>
        /// This is a wrapper around the AiClientService's GenerateChatTitle method.
        /// </summary>
        public async Task AiTitleResponseJob(int chatId, string firstMessage)
        {
            // Generating a title for the chat based on the first message.
            string title = await aiClient.GenerateChatTitle(firstMessage);

            // Ensuring that there was a title generated.
            if (string.IsNullOrEmpty(title))
            {
                title = "Untitled Chat";
            }

            // Getting the target chat from the database.
            StoredChatHistory? chat = await dbContext.ChatHistories.FindAsync(chatId);
            if (chat is null)
            {
                logger.LogInformation("Could not find a chat with ID {ChatId}", chatId);
                return;
            }

            // Setting the title and saving the changes to the database.
            chat.Title = title;
            await dbContext.SaveChangesAsync();
        }
    }
}
