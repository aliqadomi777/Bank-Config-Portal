using System.Web.Mvc;

namespace WebPortal.ASP.Controllers
{
    public class DashboardController : BaseController
    {

        public ActionResult Index()
        {
            ViewBag.BankId = CurrentBankId;
            return View();
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }
    }
}