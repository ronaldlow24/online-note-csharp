namespace OnlineNote.Models
{
    public class Account
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string SecretPhase { get; set; }

        public List<Note> Note { get; set; }

    }
}
