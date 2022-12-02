namespace OnlineNote.Models
{
    public class Account
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string SecretPhase { get; set; }
        public string? Email { get; set; }

        public List<Note> Note { get; set; }
        public List<Reminder> Reminder { get; set; }

    }

    public class EmailInputModel 
    { 
        public string Email { get; set; }
    }

}
