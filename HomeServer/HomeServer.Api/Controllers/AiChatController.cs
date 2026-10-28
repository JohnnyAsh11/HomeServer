using HomeServer.Database;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.AI;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.ChatCompletion;
using OpenAI.Chat;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HomeServer.Api.Controllers
{
    /// <summary>
    /// Provides extension methods for working with stored chat histories.
    /// </summary>
    public static class AiChatExtensions
    {
        public static ChatInfoDto ToChatInfo(this StoredChatHistory dbModel)
        {
            return new ChatInfoDto
            {
                Id = dbModel.Id,
                Title = dbModel.Title,
                IsDeleted = dbModel.IsDeleted
            };
        }

        public static MessageContentDto ToMessageContentDto(this MessageContent message)
        {
            return new MessageContentDto
            {
                Timestamp = message.Timestamp.ToString(),
                Message = message.Message,
                UserRole = (int)message.Role
            };
        }

        public static MessageContent ToMessageContent(this MessageContentDto dto)
        {
            // Setting a default User Role if null.
            if (dto.UserRole is null)
            {
                dto.UserRole = (int)ChatMessageRole.User;
            }

            // Setting a default date time if null.
            if (dto.Timestamp is null)
            {
                dto.Timestamp = DateTime.MinValue.ToString();
            }

            return new MessageContent
            {
                Message = dto.Message ?? string.Empty,
                Role = (ChatMessageRole)dto.UserRole,
                Timestamp = DateTime.Parse(dto.Timestamp)
            };
        }

        public static ChatHistory ToChatHistory(this List<MessageContent> messages)
        {
            ChatHistory history = [];
            foreach (MessageContent message in messages)
            {
                Microsoft.SemanticKernel.ChatMessageContent content = new(message.Role.ToChatRole(), message.Message);
                history.Add(content);
            }
            return history;
        }

        public static AuthorRole ToChatRole(this ChatMessageRole role)
        {
            return role switch
            {
                ChatMessageRole.System => AuthorRole.System,
                ChatMessageRole.User => AuthorRole.User,
                ChatMessageRole.Assistant => AuthorRole.Assistant,
                ChatMessageRole.Tool => AuthorRole.Tool,
                _ => throw new ArgumentOutOfRangeException(nameof(role), role, null)
            };
        }
    }

    public class AiChatController(
        HomeServerContext dbContext,
        AiClientService aiController,
        ILogger<AiChatController> logger) : IChatController
    {
        private PromptExecutionSettings _settings = new() { };

        /// <inheritdoc/>
        public async Task<ActionResult<ICollection<ChatInfoDto>>> GetAiChatsAsync(CancellationToken cancellationToken = default)
        {
            logger.LogInformation("GET chats called.");

            // Getting the chat info from the database.
            List<ChatInfoDto> chats = await dbContext.ChatHistories
                .Select(chat => chat.ToChatInfo())
                .ToListAsync(cancellationToken);

            return new ActionResult<ICollection<ChatInfoDto>>(chats);
        }

        /// <inheritdoc/>
        public async Task<ActionResult<int>> PostAiChatAsync(ChatInfoDto? body, CancellationToken cancellationToken = default)
        {
            logger.LogInformation("POST chat called.");

            if (body is null)
            {
                logger.LogInformation("POST task failed: Bad Request");
                return new BadRequestResult();
            }

            if (body.FirstMessage is null)
            {
                logger.LogInformation("POST chat failed: First message is required, Bad Request");
                return new BadRequestResult();
            }

            List<MessageContent> messages = [];

            // Auto-entered system prompt.
            messages.Add(new MessageContent()
            {
                Message = AiClientService.SystemPrompt,
                Role = ChatMessageRole.System,
                Timestamp = DateTime.Now
            });

            // The first user submitted message.
            messages.Add(new MessageContent()
            {
                Message = body.FirstMessage,
                Role = ChatMessageRole.User,
                Timestamp = DateTime.Now
            });

            // The first response from the AI.
            string? firstResponse = await aiController.GetChatResponse(messages.ToChatHistory());
            messages.Add(new MessageContent()
            {
                Message = firstResponse ?? string.Empty,
                Role = ChatMessageRole.Assistant,
                Timestamp = DateTime.Now
            });

            // Building the ChatHistory object & adding it to the database.
            StoredChatHistory newChat = new()
            {
                Title = await aiController.GenerateChatTitle(body.FirstMessage),
                IsDeleted = false,
                Messages = messages,
                StartDate = DateTime.Now
            };

            foreach (MessageContent message in newChat.Messages)
            {
                logger.LogInformation($"{message.Role.ToString()}: {message.Message}");
            }

            await dbContext.ChatHistories.AddAsync(newChat, cancellationToken);
            await dbContext.SaveChangesAsync(cancellationToken);
            return new OkObjectResult(newChat.Id);
        }

        /// <inheritdoc/>
        public async Task<IActionResult> DeleteAiChatByIdAsync(int id, CancellationToken cancellationToken = default)
        {
            logger.LogInformation("DELETE chat called with ID: {Id}", id);

            try
            {
                StoredChatHistory? chat = await dbContext.ChatHistories.FindAsync([id], cancellationToken);

                if (chat is null)
                {
                    return new NotFoundResult();
                }

                // Mark the chat as deleted.
                chat.IsDeleted = true;
                await dbContext.SaveChangesAsync(cancellationToken);
            }
            catch (Exception err)
            {
                logger.LogInformation("DELETE chat by ID was cancelled: {message}", err.Message);
            }

            logger.LogInformation("DELETE completed successfully.");
            return new NoContentResult();
        }

        /// <inheritdoc/>
        public async Task<ActionResult<int>> PostMessageToAiChatByIdAsync(int id, MessageContentDto? body, CancellationToken cancellationToken = default)
        {
            logger.LogInformation("POST message to chat called with ID: {Id}", id);

            StoredChatHistory? chat = null;
            try
            {
                chat = await dbContext.ChatHistories
                    .Include(chat => chat.Messages)
                    .SingleOrDefaultAsync(chat => chat.Id == id, cancellationToken);
            }
            catch (Exception err)
            {
                logger.LogInformation("POST message to chat by ID was cancelled or not found: {message}", err.Message);
            }

            if (chat is null)
            {
                logger.LogInformation("POST message to chat by ID was not found");
                return new NotFoundResult();
            }

            if (body is null)
            {
                logger.LogInformation("POST message to chat by ID was not found");
                return new BadRequestResult();
            }

            // Adding the new user message (http request body) to the chat history.
            MessageContent newMessage = body.ToMessageContent();
            chat.Messages.Add(newMessage);

            // Get the AI response to the new message.
            string? response = await aiController.GetChatResponse(chat.Messages.ToChatHistory());

            // Adding the AI response to the chat history.
            chat.Messages.Add(new MessageContent()
            {
                Message = response ?? string.Empty,
                Role = ChatMessageRole.Assistant,
                Timestamp = DateTime.Now
            });

            await dbContext.SaveChangesAsync(cancellationToken);
            return new OkObjectResult(chat.Id);
        }

        /// <inheritdoc/>
        public async Task<ActionResult<ChatContentDto>> GetAiChatByIdAsync(int id, CancellationToken cancellationToken = default)
        {
            logger.LogInformation("GET chat called with ID: {Id}", id);

            StoredChatHistory? chat = null;
            try
            {
                chat = await dbContext.ChatHistories
                    .Include(chat => chat.Messages)
                    .SingleOrDefaultAsync(chat => chat.Id == id, cancellationToken);
            }
            catch (Exception err)
            {
                logger.LogInformation("POST message to chat by ID was cancelled or not found: {message}", err.Message);
            }

            if (chat is null)
            {
                logger.LogInformation("POST message to chat by ID was not found");
                return new NotFoundResult();
            }

            ChatContentDto response = new()
            {
                Id = chat.Id,
                Title = chat.Title,
                Messages = [.. chat.Messages.Select(message => message.ToMessageContentDto())],
                IsDeleted = chat.IsDeleted
            };

            return new OkObjectResult(response);
        }
    }
}