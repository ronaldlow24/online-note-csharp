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
        public DbSet<ReminderEntity> Reminder { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            //optionsBuilder.UseSqlServer(
            //    @"Server=(localdb)\mssqllocaldb;Database=Blogging;Trusted_Connection=True");

            optionsBuilder.UseNpgsql(
               @"Host=db.bit.io;Database=ronaldlow24/online-note;Username=ronaldlow24;Password=v2_3v49k_dVBRqDqpkedU8WguWgjdy9n");
        }
    }
}
