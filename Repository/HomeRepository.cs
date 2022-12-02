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
                if (accountEntity is null)
                {
                    accountEntity = CustomMapper.MapperObject.Map<AccountEntity>(account);
                    await db.Account.AddAsync(accountEntity);
                    await db.SaveChangesAsync();
                }
                account = CustomMapper.MapperObject.Map<Account>(accountEntity);

                session.Clear();
                session.SetInt32(SessionString.AccountId, account.Id);
                session.SetString(SessionString.AccountName, account.Name);
                session.SetString(SessionString.AccountEmail, account.Email);
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

        internal async Task<ResultDataPairBase> UpdateEmailAsync(int accountId, string email, ISession session)
        {
            try
            {
                using var db = new DataContext();
                var accountEntity = await db.Account.FirstAsync(a => a.Id == accountId);

                var isEmailValid = MailHelper.IsValidEmail(email);
                if(!isEmailValid)
                    return new ResultDataPairBase { Result = false,CustomData="Email is not valid!" };


                accountEntity.Email = email;
                await db.SaveChangesAsync();
                session.SetString(SessionString.AccountEmail, email);

                return new ResultDataPairBase { Result = true };
            }
            catch
            {
                throw;
            }
        }
    }
}
