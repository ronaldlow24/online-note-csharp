using Microsoft.EntityFrameworkCore;
using OnlineNote.Common;
using OnlineNote.Entities;
using OnlineNote.Models;

namespace OnlineNote.Repository
{
    public class ReminderRepository
    {

        internal async Task<List<Reminder>> GetAllReminderAsync(int accountId)
        {
            try
            {
                using var db = new DataContext();
                var reminderEntity = await db.Reminder.Where(a => a.AccountId == accountId).AsNoTracking().ToListAsync();
                return CustomMapper.MapperObject.Map<List<Reminder>>(reminderEntity);
            }
            catch
            {
                throw;
            }
        }


        internal async Task<Reminder> CreateOrUpdateReminderAsync(Reminder model)
        {
            try
            {
                using var db = new DataContext();
                var reminderEntity = await db.Reminder.FirstOrDefaultAsync(a => a.Id == model.Id);
                Reminder? returnReminder = null;
                if(reminderEntity is not null)
                {
                    reminderEntity.Title = model.Title;
                    reminderEntity.TargetDatetime = model.TargetDatetime;
                    reminderEntity.CreatedDatetime = DateTime.UtcNow;
                    reminderEntity.TimezoneId = model.TimezoneId;
                    await db.SaveChangesAsync();
                    returnReminder = CustomMapper.MapperObject.Map<Reminder>(reminderEntity);
                }
                else
                {
                    var newEntity = CustomMapper.MapperObject.Map<ReminderEntity>(model);
                    newEntity.CreatedDatetime = DateTime.UtcNow;
                    await db.Reminder.AddAsync(newEntity);
                    await db.SaveChangesAsync();
                    returnReminder = CustomMapper.MapperObject.Map<Reminder>(newEntity);
                }

                return returnReminder;

            }
            catch
            {
                throw;
            }
        }

        internal async Task<bool> DeleteReminderAsync(int accountId, int reminderId)
        {
            try
            {
                using var db = new DataContext();
                var reminderEntity = await db.Reminder.FirstAsync(a => a.Id == reminderId && a.AccountId == accountId);
                db.Reminder.Remove(reminderEntity);
                return await db.SaveChangesAsync() > 0;
            }
            catch
            {
                throw;
            }
        }
    }
}
