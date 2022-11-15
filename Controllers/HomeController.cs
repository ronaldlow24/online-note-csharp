using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using OnlineNote.Common;
using OnlineNote.Models;
using OnlineNote.Repository;
using System.Net.WebSockets;
using static OnlineNote.Common.Constant;

namespace OnlineNote.Controllers
{
    public class HomeController : Controller
    {
        private readonly HomeRepository homeRepository;

        public HomeController()
        {
            homeRepository = new HomeRepository();
        }

        [SessionChecker]
        public async Task<IActionResult> Index()
        {
            var accountId = HttpContext.Session.GetInt32(SessionString.AccountId)!.Value;
            var account = await homeRepository.GetAccountAsync(accountId);
            return View(account);
        }

        [SessionChecker]
        public async Task<IActionResult> Note(int Id)
        {
            var accountId = HttpContext.Session.GetInt32(SessionString.AccountId)!.Value;
            var account = await homeRepository.GetAccountAsync(accountId);
            ViewBag.AccountId = accountId;
            Note note = account.Note.First(s => s.Id == Id);
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
                var newNote = new Note();
                newNote.AccountId = accountId;
                newNote.Title = "New Note";
                newNote.Content = "";
                return await homeRepository.PostNoteAsync(newNote);
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
                var account = await homeRepository.GetAccountAsync(accountId);
                var note = account.Note.FirstOrDefault(s => s.Id == Id);
                if(note is not null)
                    return await homeRepository.DeleteNoteAsync(Id);
                return false;
            }
            catch
            {
                throw;
            }
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

    }

    public class NoteHub : Hub
    {
        private readonly HomeRepository homeRepository;

        public NoteHub()
        {
            homeRepository = new HomeRepository();
        }

        [SessionChecker]
        public async Task AddToGroup(int noteId)
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, noteId.ToString());
        }

        [SessionChecker]
        public async Task RemoveFromGroup(int noteId)
        {
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, noteId.ToString());
        }

        [SessionChecker]
        public async Task SaveTitle(int noteId, string title)
        {
            await homeRepository.UpdateTitleAsync(noteId, title);

            await Clients.GroupExcept(noteId.ToString(), Context.ConnectionId).SendAsync("RenderTitle", title);
        }

        [SessionChecker]
        public async Task SaveContent(int noteId, string content, string updatedContent)
        {
            await homeRepository.UpdateContentAsync(noteId, content);

            await Clients.GroupExcept(noteId.ToString(), Context.ConnectionId).SendAsync("RenderContent", updatedContent);
        }
    }

}