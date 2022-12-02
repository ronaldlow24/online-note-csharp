using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using OnlineNote.Common;
using OnlineNote.Models;
using OnlineNote.Repository;
using System.Dynamic;
using static OnlineNote.Common.Constant;

namespace OnlineNote.Controllers
{
    public class HomeController : Controller
    {
        private readonly HomeRepository homeRepository;
        private readonly NoteRepository noteRepository;
        private readonly ReminderRepository reminderRepository;

        public HomeController()
        {
            homeRepository = new HomeRepository();
            noteRepository = new NoteRepository();
            reminderRepository = new ReminderRepository();
        }

        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public async Task<Account> Login([FromBody] Account account)
        {
            try
            {
                return await homeRepository.LoginAsync(account, HttpContext.Session);
            }
            catch
            {
                throw;
            }
        }

        [HttpPost]
        public bool Logout()
        {
            try
            {
                HttpContext.Session.Clear();
                return true;
            }
            catch
            {
                throw;
            }
        }

        [SessionChecker]
        public async Task<IActionResult> Index()
        {
            var accountId = HttpContext.Session.GetInt32(SessionString.AccountId)!.Value;
            var notes = await noteRepository.GetAllNoteAsync(accountId);
            return View(notes);
        }


        [SessionChecker]
        public async Task<IActionResult> Note(int Id)
        {
            var accountId = HttpContext.Session.GetInt32(SessionString.AccountId)!.Value;
            var note = await noteRepository.GetNoteAsync(accountId, Id);
            ViewBag.AccountId = accountId;
            ViewBag.Content = note.Content;
            return View(note);
        }

        [SessionChecker]
        [HttpPost]
        public async Task<int> NewNote()
        {
            try
            {
                var accountId = HttpContext.Session.GetInt32(SessionString.AccountId)!.Value;
                return await noteRepository.PostNewNoteAsync(accountId);
            }
            catch
            {
                throw;
            }
        }

        [SessionChecker]
        [HttpPost]
        public async Task<bool> DeleteNote(int Id)
        {
            try
            {
                var accountId = HttpContext.Session.GetInt32(SessionString.AccountId)!.Value;
                return await noteRepository.DeleteNoteAsync(accountId, Id);
            }
            catch
            {
                throw;
            }
        }

        [SessionChecker]
        public async Task<List<Reminder>> GetAllReminder()
        {
            var accountId = HttpContext.Session.GetInt32(SessionString.AccountId)!.Value;
            var reminder = await reminderRepository.GetAllReminderAsync(accountId);
            return reminder;
        }


        [SessionChecker]
        public async Task<IActionResult> Reminder()
        {
            ViewBag.Email = HttpContext.Session.GetString(SessionString.AccountEmail);
            return View();
        }

        [SessionChecker]
        [HttpPost]
        public async Task<ResultDataPair<Reminder>> PostReminder([FromBody] Reminder model)
        {
            try
            {
                var email = HttpContext.Session.GetString(SessionString.AccountEmail);
                //if(email is null)
                //{
                //    return new ResultDataPair<Reminder> { Result = false, CustomData = "Please configure email!" };
                //}

                var accountId = HttpContext.Session.GetInt32(SessionString.AccountId)!.Value;
                model.AccountId = accountId;
                var result = await reminderRepository.CreateOrUpdateReminderAsync(model);
                return new ResultDataPair<Reminder> { Result = true, Data = result };
            }
            catch
            {
                throw;
            }
        }

        [SessionChecker]
        [HttpPost]
        public async Task<bool> DeleteReminder(int Id)
        {
            try
            {
                var accountId = HttpContext.Session.GetInt32(SessionString.AccountId)!.Value;
                return await reminderRepository.DeleteReminderAsync(accountId, Id);
            }
            catch
            {
                throw;
            }
        }

        [SessionChecker]
        [HttpPost]
        public async Task<ResultDataPairBase> UpdateEmail([FromBody] EmailInputModel model)
        {
            try
            {
                var accountId = HttpContext.Session.GetInt32(SessionString.AccountId)!.Value;
                return await homeRepository.UpdateEmailAsync(accountId, model.Email, HttpContext.Session);
            }
            catch
            {
                throw;
            }
        }
    }
}