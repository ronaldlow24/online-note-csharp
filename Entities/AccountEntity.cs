﻿using System.ComponentModel.DataAnnotations.Schema;

namespace OnlineNote.Models
{
    [Table("account")]
    public class AccountEntity
    {
        [Column("id")]
        public int Id { get; set; }

        [Column("name")]
        public string Name { get; set; }
        
        [Column("secret_phase")]
        public string SecretPhase { get; set; }

    }
}
