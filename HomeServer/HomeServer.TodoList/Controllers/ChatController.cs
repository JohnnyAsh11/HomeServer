using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using HomeServer.Database;
using Microsoft.AspNetCore.Mvc;

namespace HomeServer.TodoList.Controllers
{
    public class ChatController(
        HomeServerContext DbContext,
        ILogger<ChatController> Logger) : IChatController
    {

        public Task<IActionResult> DeleteAiChatByIdAsync(int id, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public Task<ActionResult<ChatContentDto>> GetAiChatByIdAsync(int id, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public Task<ActionResult<ICollection<ChatInfoDto>>> GetAiChatsAsync(CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public Task<ActionResult<int>> PostAiChatAsync(ChatInfoDto? body, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public Task<ActionResult<int>> PostMessageToAiChatByIdAsync(int id, MessageContentDto? body, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }
    }
}