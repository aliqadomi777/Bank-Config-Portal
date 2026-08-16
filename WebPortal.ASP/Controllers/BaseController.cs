using System;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace WebPortal.ASP.Controllers
{
    public class BaseController : Controller
    {
        protected int CurrentBankId
        {
            get
            {
                return Session["BankId"] != null ? (int)Session["BankId"] : 0;
            }
        }


        protected override void OnActionExecuting(
            ActionExecutingContext filterContext)
        {
            if (Session["BankId"] == null)
            {
                filterContext.Result =
                    new RedirectToRouteResult(
                        new RouteValueDictionary
                        {
                            { "controller", "Login" },
                            { "action", "Index" }
                        });
            }

            base.OnActionExecuting(filterContext);
        }


        // Prevent page caching on browsers
        protected override void OnResultExecuting(
            ResultExecutingContext filterContext)
        {
            filterContext.HttpContext.Response.Cache
                .SetCacheability(HttpCacheability.NoCache);

            filterContext.HttpContext.Response.Cache
                .SetNoStore();

            filterContext.HttpContext.Response.Cache
                .SetExpires(DateTime.UtcNow.AddDays(-1));

            filterContext.HttpContext.Response.Cache
                .SetMaxAge(TimeSpan.Zero);

            filterContext.HttpContext.Response.Headers
                .Set("Pragma", "no-cache");

            base.OnResultExecuting(filterContext);
        }
    }
}