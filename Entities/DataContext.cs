using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;
using OnlineNote.Models;
using System.Collections.Generic;
using System.Reflection.Metadata;

namespace OnlineNote.Entities
{
    public class DataContext : DbContext
    {
        public DbSet<AccountEntity> Account { get; set; }
        public DbSet<NoteEntity> Note { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer(
                @"Server=(localdb)\mssqllocaldb;Database=Blogging;Trusted_Connection=True");
        }
    }
}
