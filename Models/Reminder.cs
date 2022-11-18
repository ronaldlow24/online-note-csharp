
namespace OnlineNote.Models
{
    public class Reminder
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public DateTimeOffset TargetDatetime { get; set; }
        public int AccountId { get; set; }
        public DateTimeOffset CreatedDateTime { get; set; }

    }

}
