using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace OnlineNote.Models
{
    [Table("reminder")]
    public class ReminderEntity
    {
        [Key]
        [Column("id")]
        public int Id { get; set; }
        [Column("title")]
        public string Title { get; set; }

        [Column("target_datetime")]
        public DateTime TargetDatetime { get; set; }

        [Column("account_id")]
        public int AccountId { get; set; }

        [Column("created_datetime")]
        public DateTime CreatedDatetime { get; set; }

        [Column("timezone_id")]
        public string TimezoneId { get; set; }
    }
}
