using HomeServer.Database;
using Microsoft.AspNetCore.Mvc;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.ChatCompletion;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HomeServer.Api.Controllers
{
    public class AiChatController(
        HomeServerContext DbContext,
        IChatCompletionService AiClient,
        ILogger<AiChatController> Logger) : IChatController
    {
        private PromptExecutionSettings _settings = new()
        {
            //FunctionChoiceBehavior = FunctionChoiceBehavior.Auto()
        };

        /// <inheritdoc/>
        public async Task<ActionResult<ICollection<ChatInfoDto>>> GetAiChatsAsync(CancellationToken cancellationToken = default)
        {
            Logger.LogInformation("Called GetAiChatsAsync");

            ChatHistory history = [];
            history.AddUserMessage("This is a test message.  Please give me the name and details of a random Magic: The Gathering Mythic or Rare card.");

            ChatMessageContent response = await AiClient.GetChatMessageContentAsync(history, _settings);

            if (response.Content is null)
            {
                Logger.LogWarning("AI response content is null.");
                return new BadRequestResult();
            }

            string str = "";
            foreach (char message in response.Content)
            {
                str += message;
            }
            Logger.LogInformation("AI response: " + str);


            return new ActionResult<ICollection<ChatInfoDto>>(new List<ChatInfoDto>());
        }

        /// <inheritdoc/>
        public Task<ActionResult<int>> PostAiChatAsync(ChatInfoDto? body, CancellationToken cancellationToken = default)
        {
            Logger.LogInformation("Called PostAiChatAsync");
            return Task.FromResult<ActionResult<int>>(new ActionResult<int>(1));
        }

        /// <inheritdoc/>
        public Task<IActionResult> DeleteAiChatByIdAsync(int id, CancellationToken cancellationToken = default)
        {
            Logger.LogInformation("Called DeleteAiChatByIdAsync");
            return Task.FromResult<IActionResult>(new NoContentResult());
        }

        /// <inheritdoc/>
        public Task<ActionResult<int>> PostMessageToAiChatByIdAsync(int id, MessageContentDto? body, CancellationToken cancellationToken = default)
        {
            Logger.LogInformation("Called PostMessageToAiChatByIdAsync");
            return Task.FromResult<ActionResult<int>>(new ActionResult<int>(1));
        }

        /// <inheritdoc/>
        public Task<ActionResult<ChatContentDto>> GetAiChatByIdAsync(int id, CancellationToken cancellationToken = default)
        {
            Logger.LogInformation("Called GetAiChatByIdAsync");
            return Task.FromResult<ActionResult<ChatContentDto>>(new ActionResult<ChatContentDto>(new ChatContentDto()));
        }
    }
}