

namespace OnlineNote.Models
{
    public class Reminder
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public DateTime TargetDatetime { get; set; }
        public int AccountId { get; set; }
        public DateTime CreatedDateTime { get; set; }
        public string TimezoneId { get; set; }
    }

}
