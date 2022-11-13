using Microsoft.EntityFrameworkCore;
using OnlineNote.Common;
using OnlineNote.Entities;
using OnlineNote.Models;
using System.Security.Principal;
using static OnlineNote.Common.Constant;

namespace OnlineNote.Repository
{
    public class HomeRepository
    {
        internal async Task<Account> LoginAsync(Account account, ISession session)
        {
            try
            {
                using var db = new DataContext();
                var accountEntity = await db.Account.SingleOrDefaultAsync(a => a.Name == account.Name && a.SecretPhase == account.SecretPhase);
                if(accountEntity is not null)
                {
                    account = CustomMapper.MapperObject.Map<Account>(accountEntity);
                    var noteEntities = await db.Note.Where(a => a.AccountId == accountEntity.Id).AsNoTracking().ToListAsync();
                    account.Note = CustomMapper.MapperObject.Map<List<Note>>(noteEntities); 
                }
                else
                {
                    accountEntity = CustomMapper.MapperObject.Map<AccountEntity>(account);
                    await db.Account.AddAsync(accountEntity);
                    await db.SaveChangesAsync();
                    account = CustomMapper.MapperObject.Map<Account>(accountEntity);
                }

                session.Clear();
                session.SetInt32(SessionString.AccountId, account.Id);
                session.SetString(SessionString.AccountName, account.Name);
                await session.CommitAsync();
                return account;
            }
            catch
            {
                throw;
            }
        }

        internal async Task<Account> GetAccountAsync(int accountId)
        {
            try
            {
                using var db = new DataContext();
                var accountEntity = await db.Account.FirstAsync(a => a.Id == accountId);
                var account = CustomMapper.MapperObject.Map<Account>(accountEntity);
                var noteEntities = await db.Note.Where(a => a.AccountId == accountEntity.Id).AsNoTracking().ToListAsync();
                account.Note = CustomMapper.MapperObject.Map<List<Note>>(noteEntities);
                return account;
            }
            catch
            {
                throw;
            }
        }

        internal async Task<Note> GetNoteAsync(int noteId)
        {
            try
            {
                using var db = new DataContext();
                var noteEntity = await db.Note.FirstAsync(a => a.Id == noteId);
                var note = CustomMapper.MapperObject.Map<Note>(noteEntity);
                return note;
            }
            catch
            {
                throw;
            }
        }

        internal async Task<int> PostNoteAsync(Note note)
        {
            try
            {
                using var db = new DataContext();
                var noteEntity = await db.Note.FirstOrDefaultAsync(a => a.AccountId == note.AccountId && a.Id == note.Id);
                if(noteEntity is not null)
                {
                    noteEntity.Title = note.Title;
                    noteEntity.Content = note.Content;
                }
                else
                {
                    noteEntity = CustomMapper.MapperObject.Map<NoteEntity>(note);
                    await db.Note.AddAsync(noteEntity);
                }
                await db.SaveChangesAsync();
                return noteEntity.Id;
            }
            catch
            {
                throw;
            }
        }

        internal async Task<bool> DeleteNoteAsync(int id)
        {
            try
            {
                using var db = new DataContext();
                var noteEntities = await db.Note.FirstOrDefaultAsync(a => a.Id == id);
                if (noteEntities is null)
                    return false;
                db.Note.Remove(noteEntities);
                return await db.SaveChangesAsync() > 0;
            }
            catch
            {
                throw;
            }
        }
    }
}
