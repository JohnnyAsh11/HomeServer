namespace HomeServer.WebUi.ViewModels
{
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
    }
}
