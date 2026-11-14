using HomeServer.Client;

namespace HomeServer.WebUi.ViewModels
{
    /// <summary>
    /// Defines the Role of the message sender in a chat.
    /// Essentially a view copy of the ChatMessageRole enum from HomeServer.Api.
    /// </summary>
    public enum UserRole
    {
        System = 0,
        User = 1,
        Assistant = 2,
        Tool = 3,
        Function =4,
        Developer = 5
    }

    /// <summary>
    /// View model representing a single message in a chat.
    /// </summary>
    public class MessageModel(
        string message,
        int userRole)
    {
        public string Message { get; set; } = message;
        public int UserRole { get; set; } = userRole;

        /// <summary>
        /// Converts dtos to view models.
        /// </summary>
        public static MessageModel ToViewModel(MessageContentDto dto)
        {
            // Defaulting to the user role (1).
            return new(
                dto.Message ?? string.Empty,
                dto.UserRole ?? 1);
        }
    }

    /// <summary>
    /// View model representing a chat.
    /// </summary>
    public class ChatModel(
        int Id,
        string Title,
        bool IsDeleted,
        bool IsSelected)
    {
        public int Id { get; set; } = Id;
        public string Title { get; set; } = Title;
        public bool IsDeleted { get; set; } = IsDeleted;
        public bool IsSelected { get; set; } = IsSelected;

        /// <summary>
        /// Converts dtos to view models.
        /// </summary>
        public static ChatModel ToViewModel(ChatInfoDto dto)
        {
            return new ChatModel(
                dto.Id ?? 0,
                dto.Title ?? string.Empty,
                dto.IsDeleted ?? true,
                false);
        }
    }
}
