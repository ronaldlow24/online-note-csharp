using Microsoft.AspNetCore.Mvc;
using OnlineNote.Common;
using OnlineNote.Models;
using OnlineNote.Repository;
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
}