
using OnlineNote.Repository;
using Quartz;
using System.Diagnostics;

namespace OnlineNote.Cron
{
    [DisallowConcurrentExecution]
    public class TriggerReminderCronJob : IJob
    {
        private ReminderRepository reminderRepository => new ReminderRepository();

        public Task Execute(IJobExecutionContext context)
        {
            return reminderRepository.TriggerReminderAsync();
        }
    }
}
