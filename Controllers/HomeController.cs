using Microsoft.AspNetCore.Mvc;
using OnlineNote.Common;
using OnlineNote.Models;
using OnlineNote.Repository;
using System.Collections.Concurrent;
using System.Net.WebSockets;
using System.Text;
using System.Text.Json;
using static OnlineNote.Common.Constant;

namespace OnlineNote.Controllers
{
    public class HomeController : Controller
    {
        private readonly HomeRepository homeRepository;
        private ConcurrentDictionary<string, WebSocket> connections = new ConcurrentDictionary<string, WebSocket>();

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

        [SessionChecker]
        public async Task NoteWS()
        {
            try
            {
                if (HttpContext.WebSockets.IsWebSocketRequest)
                {
                    using var webSocket = await HttpContext.WebSockets.AcceptWebSocketAsync();
                    string wsID = Guid.NewGuid().ToString();

                    connections.TryAdd(wsID, webSocket);
                    
                    //clean connection
                    var deadConnection = new List<string>();
                    foreach (var item in connections)
                    {
                        if (item.Value.State != WebSocketState.Open)
                        {
                            deadConnection.Add(item.Key);
                        }
                    }
                    foreach (var item in deadConnection) 
                    {
                        connections.Remove(item, out var temp);
                    }

                    var buffer = new byte[1024 * 4];
                    var receiveResult = await webSocket.ReceiveAsync(
                        new ArraySegment<byte>(buffer), CancellationToken.None);

                    while (!receiveResult.CloseStatus.HasValue)
                    {
                        var messageJSON = Encoding.UTF8.GetString(buffer).TrimEnd('\0');
                        var message = JsonSerializer.Deserialize<NoteWebsocketModel>(messageJSON)!;

                        if (message.Action.ToUpper().Trim() == "SAVETITLE")
                        {
                            var note = JsonSerializer.Deserialize<Note>(message.Content);

                            await homeRepository.UpdateTitleAsync(note.Id,note.Title);

                            var noteString = JsonSerializer.Serialize(note);

                            var socketData = new NoteWebsocketModel
                            {
                                Action = "RenderTitle",
                                Content = noteString
                            };

                            var socketDataByte = Encoding.UTF8.GetBytes(JsonSerializer.Serialize(socketData));
                            foreach (var item in connections) // not working broadcast
                            {
                                if (item.Key == wsID)
                                {
                                    continue;
                                }

                                await item.Value.SendAsync(
                                   new ArraySegment<byte>(socketDataByte, 0, socketDataByte.Length),
                                   receiveResult.MessageType,
                                   receiveResult.EndOfMessage,
                                   CancellationToken.None);
                            }
                        }

                        if (message.Action.ToUpper().Trim() == "SAVECONTENT")
                        {
                            var note = JsonSerializer.Deserialize<Note>(message.Content);

                            await homeRepository.UpdateContentAsync(note.Id,note.Content);

                            var noteString = JsonSerializer.Serialize(note);

                            var socketData = new NoteWebsocketModel
                            {
                                Action = "RenderContent",
                                Content = noteString
                            };

                            var socketDataByte = Encoding.UTF8.GetBytes(JsonSerializer.Serialize(socketData));
                            foreach (var item in connections) // not working broadcast
                            {
                                if (item.Key == wsID)
                                {
                                    continue;
                                }

                                await item.Value.SendAsync(
                                   new ArraySegment<byte>(socketDataByte, 0, socketDataByte.Length),
                                   receiveResult.MessageType,
                                   receiveResult.EndOfMessage,
                                   CancellationToken.None);
                            }
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