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

            CultureInfo culture = new CultureInfo(language);

            Thread.CurrentThread.CurrentCulture = culture;
            Thread.CurrentThread.CurrentUICulture = culture;
        }

        public void OnActionExecuted(ActionExecutedContext filterContext)
        {
        }
    }
}