using System.Web.Mvc;

namespace WebPortal.ASP.Controllers
{
    public class ErrorController : Controller
    {
        [HttpGet]
        [AllowAnonymous]
        public ActionResult NotFound()
        {
            Response.StatusCode = 404;

            Response.TrySkipIisCustomErrors =
                true;

            return View();
        }
    }
}