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
                var accountIds = reminderEntity.Select(s => s.AccountId).Distinct().ToList();
                var accountData = await db.Account.Where(s => accountIds.Contains(s.Id)).AsNoTracking().Select(s => new {s.Id,s.Email}).ToListAsync();

                await Parallel.ForEachAsync(reminderEntity, new ParallelOptions(), async (item, token) =>
                {
                    var targetEmail = accountData.FirstOrDefault(a => a.Id == item.AccountId)?.Email;

                    if (targetEmail is null)
                        return;

                    TimeZoneInfo tzi = TimeZoneInfo.FindSystemTimeZoneById(item.TimezoneId);

                    var localDatetimeString = TimeZoneInfo.ConvertTimeFromUtc(item.TargetDatetime, tzi).ToString("yyyy-MM-dd HH:mm:ss");

                    var body = $@" 
                        <p>This is a reminder for:<p>
                        <p><i><strong>{item.Title}</strong></i><p>
                        <p><i><strong>{localDatetimeString} ({item.TimezoneId})</strong></i><p>
                        <p>Please do not reply to this email.<p>
                    ";

                    var subject = $"Reminder : {item.Title} ({localDatetimeString})";

                    await MailHelper.SendMailAsync(subject, body, targetEmail!, token);

                    item.IsTriggered = true;
                    item.TriggeredDatetime = DateTime.UtcNow;
                });

                var result = await db.SaveChangesAsync();

                await db.Reminder.Where(a => a.TargetDatetime <= DateTime.UtcNow.AddDays(-2) && a.IsTriggered == true).ExecuteDeleteAsync();

                return result > 0;
            }
            catch
            {
                throw;
            }
        }
    }
}
