using System.Web.Mvc;

namespace WebPortal.ASP.Controllers
{
    public class DashboardController : BaseController
    {
        [HttpGet]
        public ActionResult Index()
        {
            return View();
        }
    }
}