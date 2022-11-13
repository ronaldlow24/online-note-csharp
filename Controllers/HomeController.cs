using Microsoft.AspNetCore.Mvc;
using OnlineNote.Common;
using OnlineNote.Models;
using OnlineNote.Repository;
using System.Net.WebSockets;
using System.Text;
using System.Text.Json;
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

            return View(note);
        }

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
        public async Task<int> PostNote([FromBody] Note note)
        {
            try
            {
                return await homeRepository.PostNoteAsync(note);
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


        public async Task NoteWS()
        {
            try
            {
                if (HttpContext.WebSockets.IsWebSocketRequest)
                {
                    using var webSocket = await HttpContext.WebSockets.AcceptWebSocketAsync();

                    var buffer = new byte[1024 * 4];
                    var receiveResult = await webSocket.ReceiveAsync(
                        new ArraySegment<byte>(buffer), CancellationToken.None);

                    while (!receiveResult.CloseStatus.HasValue)
                    {
                        var messageJSON = buffer.ToString()!;
                        var message = JsonSerializer.Deserialize<NoteWebsocketModel>(messageJSON)!;

                        if(message.Action.ToUpper().Trim() == "LOAD")
                        {
                            var noteId = Convert.ToInt32(message.Content);
                            var note = await homeRepository.GetNoteAsync(noteId);
                            var noteString = JsonSerializer.Serialize(note);
                            var noteByte = Encoding.UTF8.GetBytes(noteString);
                            await webSocket.SendAsync(
                           new ArraySegment<byte>(noteByte, 0, noteByte.Length),
                           receiveResult.MessageType,
                           receiveResult.EndOfMessage,
                           CancellationToken.None);
                        }

                        if (message.Action.ToUpper().Trim() == "SAVE")
                        {
                            var note = JsonSerializer.Deserialize<Note>(message.Content);
                            await homeRepository.PostNoteAsync(note);
                        }

                        receiveResult = await webSocket.ReceiveAsync(
                            new ArraySegment<byte>(buffer), CancellationToken.None);
                    }

                    await webSocket.CloseAsync(
                        receiveResult.CloseStatus.Value,
                        receiveResult.CloseStatusDescription,
                        CancellationToken.None);
                }
                else
                {
                    HttpContext.Response.StatusCode = StatusCodes.Status400BadRequest;
                }
            }
            catch
            {
                HttpContext.Response.StatusCode = StatusCodes.Status500InternalServerError;
            }
        }
    }
}