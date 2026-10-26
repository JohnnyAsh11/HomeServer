using Microsoft.SemanticKernel.ChatCompletion;
using OpenAI.Chat;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HomeServer.Database
{

    /// <summary>
    /// Defines the data held by a single message sent to or from the AI assistant.
    /// </summary>
    public class MessageContent
    {
        /// <summary>
        /// When this message was sent.
        /// </summary>
        public DateTime Timestamp { get; set; }

        /// <summary>
        /// The role that this message was sent as. (User, System, Assistant, Tool, Developer)
        /// This comes from Semantic Kernel's ChatMessageRole enum.
        /// </summary>
        public ChatMessageRole Role { get; set; }

        /// <summary>
        /// The content sent within this particular message.
        /// </summary>
        public string Message { get; set; } = string.Empty;
    }

    /// <summary>
    /// Defines the entire chat history of a single conversation with the AI assistant.
    /// </summary>
    public class StoredChatHistory
    {
        /// <summary>
        /// Unique ID of this chat.
        /// </summary>
        [Key]
        public int Id { get; set; }

        /// <summary>
        /// When this chat was opened.
        /// </summary>
        public DateTime StartDate { get; set; }

        /// <summary>
        /// All of the messages sent to and from the assistant.
        /// </summary>
        public List<MessageContent> Messages { get; set; } = [];
    }
}
