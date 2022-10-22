using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc;

namespace OnlineNote.Common
{
    public class SessionChecker : Attribute, IActionFilter
    {


        public void OnActionExecuting(ActionExecutingContext context)
        {
            //if there is no session whitch key is "register", user will not access to specified action and redirect to login page.

            if(context.HttpContext.Session.IsAvailable)
            {
                context.Result = new RedirectToActionResult("Login", "Home", null);
            }
        }

        public void OnActionExecuted(ActionExecutedContext context)
        {

        }
    }
}
