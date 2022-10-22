using Microsoft.EntityFrameworkCore;
using OnlineNote.Common;
using OnlineNote.Entities;
using OnlineNote.Models;
using static OnlineNote.Common.Constant;

namespace OnlineNote.Repository
{
    public class HomeRepository
    {
        public async Task<Account> LoginAsync(Account account, ISession session)
        {
            try
            {
                using var db = new DataContext();
                var accountEntity = await db.Account.SingleOrDefaultAsync(a => a.Name == account.Name && a.SecretPhase == account.SecretPhase);
                if(accountEntity is not null)
                {
                    account = CustomMapper.MapperObject.Map<Account>(accountEntity);
                    var noteEntities = await db.Note.Where(a => a.AccountId == accountEntity.Id).ToListAsync();
                    account.Note = CustomMapper.MapperObject.Map<List<Note>>(noteEntities); 
                }

                session.Clear();
                session.SetInt32(SessionString.AccountId, account.Id);
                session.SetString(SessionString.AccountName, account.Name);

                return account;
            }
            catch
            {
                throw;
            }
        }
    }
}
