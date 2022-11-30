using Microsoft.EntityFrameworkCore;
using OnlineNote.Common;
using OnlineNote.Entities;
using OnlineNote.Models;

namespace OnlineNote.Repository
{
    public class NoteRepository
    {

        internal async Task<List<Note>> GetAllNoteAsync(int accountId)
        {
            try
            {
                using var db = new DataContext();
                var noteEntities = await db.Note.Where(a => a.AccountId == accountId).ToListAsync();
                return CustomMapper.MapperObject.Map<List<Note>>(noteEntities);
            }
            catch
            {
                throw;
            }
        }

        internal async Task<Note> GetNoteAsync(int accountId, int noteId)
        {
            try
            {
                using var db = new DataContext();
                var noteEntity = await db.Note.FirstAsync(a => a.Id == noteId && a.AccountId == accountId);
                var note = CustomMapper.MapperObject.Map<Note>(noteEntity);
                return note;
            }
            catch
            {
                throw;
            }
        }

        internal async Task<int> PostNewNoteAsync(int accountId)
        {
            try
            {
                using var db = new DataContext();
                var noteEntity = new NoteEntity();
                noteEntity.AccountId = accountId;
                noteEntity.Title = "New Note";
                noteEntity.Content = "";
                await db.Note.AddAsync(noteEntity);
                await db.SaveChangesAsync();
                return noteEntity.Id;
            }
            catch
            {
                throw;
            }
        }

        internal async Task<int> UpdateTitleAsync(int accountId, int noteId, string Title)
        {
            try
            {
                using var db = new DataContext();
                var noteEntity = await db.Note.FirstAsync(a => a.Id == noteId && a.AccountId == accountId);
                noteEntity.Title = Title.Trim();
                await db.SaveChangesAsync();
                return noteEntity.Id;
            }
            catch
            {
                throw;
            }
        }

        internal async Task<int> UpdateContentAsync(int accountId, int noteId, string Content)
        {
            try
            {
                using var db = new DataContext();
                var noteEntity = await db.Note.FirstAsync(a => a.Id == noteId && a.AccountId == accountId );
                noteEntity.Content = Content;
                await db.SaveChangesAsync();
                return noteEntity.Id;
            }
            catch
            {
                throw;
            }
        }

        internal async Task<bool> DeleteNoteAsync(int accountId, int noteId)
        {
            try
            {
                using var db = new DataContext();
                var noteEntities = await db.Note.FirstOrDefaultAsync(a => a.Id == noteId && a.AccountId == accountId);
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
