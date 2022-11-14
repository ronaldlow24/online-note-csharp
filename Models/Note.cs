namespace OnlineNote.Models
{
    public class Note
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Content { get; set; }
        public string UpdatedContent { get; set; }
        public int AccountId { get; set; }

    }

    public class NoteWebsocketModel
    {
        public string Action { get; set; }
        public string Content { get; set; }

    }
}
