using System.ComponentModel.DataAnnotations.Schema;

namespace OnlineNote.Models
{
    [Table("note")]
    public class NoteEntity
    {
        [Column("id")]
        public int Id { get; set; }
        [Column("title")]
        public string Title { get; set; }

        [Column("content")]
        public string Content { get; set; }

        [Column("account_id")]
        public int AccountId { get; set; }

    }
}
