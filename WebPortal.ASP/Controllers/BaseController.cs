using Microsoft.Owin.Security;
using System;
using System.Globalization;
using System.Security.Claims;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using WebPortal.ASP.Security;

namespace WebPortal.ASP.Controllers
{
    [Authorize]
    public class BaseController : Controller
    {
        protected int CurrentBankId
        {
            get
            {
                int? bankId = GetCurrentBankId();

                if (!bankId.HasValue)
                {
                    throw new UnauthorizedAccessException("Authenticated user does not contain a valid BankId claim.");
                }


                return bankId.Value;
            }
        }


        protected string CurrentBankName
        {
            get
            {
                var principal = User as ClaimsPrincipal;

                if (principal == null)
                {
                    return null;
                }

                Claim claim = principal.FindFirst(AuthenticationConstants.BankNameClaimType);

                return claim?.Value;
            }
        }


        protected override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            if (!GetCurrentBankId().HasValue)
            {
                HttpContext.GetOwinContext()
                           .Authentication
                           .SignOut(AuthenticationConstants.AuthenticationType);


                filterContext.Result =
                    new RedirectToRouteResult(
                        new RouteValueDictionary
                        {
                            { "controller", "Login" },
                            { "action", "Index" }
                        });

                return;
            }


            base.OnActionExecuting(filterContext);
        }

        private int? GetCurrentBankId()
        {
            var principal = User as ClaimsPrincipal;

            if (principal == null)
            {
                return null;
            }

            Claim bankIdClaim = principal.FindFirst(AuthenticationConstants.BankIdClaimType);

            if (bankIdClaim == null)
            {
                return null;
            }

            int bankId;

            bool parsed = int.TryParse(bankIdClaim.Value,
                        NumberStyles.Integer,
                        CultureInfo.InvariantCulture,
                        out bankId);


            if (!parsed ||
                bankId <= 0)
            {
                return null;
            }


            return bankId;
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