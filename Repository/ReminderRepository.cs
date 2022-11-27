using Microsoft.EntityFrameworkCore;
using OnlineNote.Common;
using OnlineNote.Entities;
using OnlineNote.Models;
using System.Collections.Concurrent;

namespace OnlineNote.Repository
{
    public class ReminderRepository
    {

        internal async Task<List<Reminder>> GetAllReminderAsync(int accountId)
        {
            try
            {
                using var db = new DataContext();
                var reminderEntity = await db.Reminder.Where(a => a.AccountId == accountId && a.TargetDatetime >= DateTime.UtcNow).AsNoTracking().ToListAsync();
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

        internal async Task<bool> TriggerReminderAsync()
        {
            try
            {
                using var db = new DataContext();
                var currentDatetime = DateTime.UtcNow;
                var reminderEntity = await db.Reminder.Where(a => a.TargetDatetime <= DateTime.UtcNow && a.IsTriggered == false).ToListAsync();
                var accountIds = reminderEntity.Select(s => s.AccountId).ToList();
                var accountData = await db.Account.Where(s => accountIds.Contains(s.Id)).AsNoTracking().Select(s => new {s.Id,s.Email}).ToListAsync();

                var body = $@" 
                    <p>This is a reminder.<p>
                    <p>Please do not reply to this email.<p>
                ";

                await Parallel.ForEachAsync(reminderEntity, new ParallelOptions(), async (item, token) =>
                {
                    var targetEmail = accountData.First(a => a.Id == item.AccountId).Email;

                    await Mail.SendMailAsync($"Reminder : {item.Title}", body, targetEmail!, token);

                    item.IsTriggered = true;
                    item.TriggeredDatetime = DateTime.UtcNow;
                });

                return await db.SaveChangesAsync() > 0;
            }
            catch
            {
                throw;
            }
        }
    }
}
