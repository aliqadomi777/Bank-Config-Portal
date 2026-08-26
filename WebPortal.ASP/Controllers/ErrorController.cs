using System.Web.Mvc;

namespace WebPortal.ASP.Controllers
{
    public class ErrorController : Controller
    {
        [HttpGet]
        [AllowAnonymous]
        public ActionResult NotFound()
        {
            //Prevent user from manaully navigating into NotFound page
            if (Request.QueryString["aspxerrorpath"] == null)
            {
                return RedirectToAction("Index", "Dashboard");
            }
            Response.StatusCode = 404;

            Response.TrySkipIisCustomErrors = true;

            return View();
        }
        [HttpGet]
        [AllowAnonymous]
        public ActionResult Error()
        {
            //Prevent user from manaully navigating into Error page
            if (Server.GetLastError() == null)
            {
                return RedirectToAction("Index", "Dashboard");
            }

            Response.StatusCode = 500;
            Response.TrySkipIisCustomErrors = true;

            return View("Error");
        }

    }

}