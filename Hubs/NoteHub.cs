using Microsoft.AspNetCore.SignalR;
using OnlineNote.Common;
using OnlineNote.Repository;
using System.Collections.Concurrent;
using System.Text.RegularExpressions;
using static OnlineNote.Common.Constant;

namespace OnlineNote.Hubs
{
    public class NoteHub : Hub
    {
        private readonly NoteRepository noteRepository;

        internal static ConcurrentDictionary<string, int> ClientConnectionInfo = new ConcurrentDictionary<string, int>();

        public override async Task OnDisconnectedAsync(Exception exception)
        {
            ClientConnectionInfo.TryRemove(Context.ConnectionId, out var temp);

            await base.OnDisconnectedAsync(exception);
        }

        public NoteHub()
        {
            noteRepository = new NoteRepository();
        }

        [SessionChecker]
        public async Task AddToGroup(int noteId)
        {
            ClientConnectionInfo.TryAdd(Context.ConnectionId, noteId);

            await Groups.AddToGroupAsync(Context.ConnectionId, noteId.ToString());
        }

        [SessionChecker]
        public async Task RemoveFromGroup(int noteId)
        {
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, noteId.ToString());
        }

        [SessionChecker]
        public async Task SaveTitle(int accountId, int noteId, string title)
        {

            await noteRepository.UpdateTitleAsync(accountId, noteId, title);

            await Clients.GroupExcept(noteId.ToString(), Context.ConnectionId).SendAsync("RenderTitle", title);
        }

        [SessionChecker]
        public async Task SaveContent(int accountId ,int noteId, string content, string updatedContent)
        {
            await noteRepository.UpdateContentAsync(accountId, noteId, content);

            await Clients.GroupExcept(noteId.ToString(), Context.ConnectionId).SendAsync("RenderContent", updatedContent);
        }


        [SessionChecker]
        public async Task GetConnectionNumber(int noteId)
        {
            var temp = ClientConnectionInfo.Count(s => s.Value == noteId);
            await Clients.Caller.SendAsync("RenderConnectionNumber", temp);
        }
    }
}
