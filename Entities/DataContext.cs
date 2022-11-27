using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using OnlineNote.Common;
using OnlineNote.Models;

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

            optionsBuilder.UseNpgsql(ApplicationSetting.ConnectionStrings.Database);
        }
    }
}
