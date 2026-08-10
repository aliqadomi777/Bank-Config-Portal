using System.Globalization;
using System.Threading;
using System.Web.Mvc;

namespace WebPortal.ASP
{
    public class FilterConfig
    {
        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            filters.Add(new HandleErrorAttribute());
            filters.Add(new LanguageFilter());
        }
    }

    public class LanguageFilter : IActionFilter
    {
        public void OnActionExecuting(ActionExecutingContext filterContext)
        {
            string language = "en";

            var cultureCookie = filterContext.HttpContext.Request.Cookies["Culture"];

            if (cultureCookie != null &&
                (cultureCookie.Value == "en" || cultureCookie.Value == "ar"))
            {
                language = cultureCookie.Value;
            }

            CultureInfo uiCulture = new CultureInfo(language == "ar" ? "ar" : "en");
            Thread.CurrentThread.CurrentUICulture = uiCulture;


            CultureInfo systemCulture = CultureInfo.InvariantCulture;
            Thread.CurrentThread.CurrentCulture = systemCulture;
        }

        public void OnActionExecuted(ActionExecutedContext filterContext)
        {
        }
    }
}
