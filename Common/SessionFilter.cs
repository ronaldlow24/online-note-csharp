using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc;
using static OnlineNote.Common.Constant;

namespace OnlineNote.Common
{
    public class SessionChecker : Attribute, IActionFilter
    {


        public void OnActionExecuting(ActionExecutingContext context)
        {
            if (!context.HttpContext.Session.GetInt32(SessionString.AccountId).HasValue)
            {
                context.Result = new RedirectToActionResult("Login", "Home", null);
            }
        }

        public void OnActionExecuted(ActionExecutedContext context)
        {

        }
    }
}
