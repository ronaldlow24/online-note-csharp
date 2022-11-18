using Microsoft.EntityFrameworkCore;
using OnlineNote.Common;
using OnlineNote.Entities;
using OnlineNote.Models;
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
                return account;
            }
            catch
            {
                throw;
            }
        }

    }
}
