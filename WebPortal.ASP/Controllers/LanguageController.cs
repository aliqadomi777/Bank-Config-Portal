using System;
using System.Web;
using System.Web.Mvc;

namespace WebPortal.ASP.Controllers
{
    public class LanguageController : Controller
    {
        [HttpGet]
        [AllowAnonymous]
        public ActionResult ChangeLanguage(
            string language,
            string returnUrl)
        {
            if (language != "en" &&
                language != "ar")
            {
                language = "en";
            }

            HttpCookie cultureCookie =
                new HttpCookie(
                    "Culture",
                    language)
                {
                    Expires =
                        DateTime.Now.AddYears(1),

                    HttpOnly = true
                };

            Response.Cookies.Add(
                cultureCookie);

            if (!string.IsNullOrEmpty(returnUrl) &&
                Url.IsLocalUrl(returnUrl))
            {
                return Redirect(returnUrl);
            }

            return RedirectToAction(
                "Index",
                "Dashboard");
        }
    }
}