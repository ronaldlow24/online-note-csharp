using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using OnlineNote.Common;
using OnlineNote.Models;
using OnlineNote.Repository;
using static OnlineNote.Common.Constant;

namespace OnlineNote.Controllers
{
    public class CronController : Controller
    {
        private readonly ReminderRepository reminderRepository;

        public CronController()
        {
            reminderRepository = new ReminderRepository();
        }

        public async Task<bool> TriggerReminder()
        {
            try
            {
                return await reminderRepository.TriggerReminderAsync();
            }
            catch
            {
                return false;
            }
        }
    }
}