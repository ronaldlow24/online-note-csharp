using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace OnlineNote.Models
{
    [Table("reminder")]
    public class ReminderEntity
    {
        [Column("id")]
        public int Id { get; set; }
        [Column("title")]
        public string Title { get; set; }

        [Column("target_datetime")]
        public DateTimeOffset TargetDatetime { get; set; }

        [Column("account_id")]
        public int AccountId { get; set; }

        [Column("created_datetime")]
        public DateTimeOffset CreatedDatetime { get; set; }

    }
}
