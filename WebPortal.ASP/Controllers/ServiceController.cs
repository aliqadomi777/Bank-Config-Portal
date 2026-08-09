using System.Web.Mvc;
using WebPortal.Application.Interfaces;

namespace WebPortal.ASP.Controllers
{
    public class ServiceController : BaseController
    {
        private readonly IServiceManager _serviceManager;

        public ServiceController(IServiceManager serviceMangager)
        {
            _serviceManager = serviceMangager;
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(int id)
        {
            _serviceManager.DeleteService(id);

            return RedirectToAction("Index", "Dashboard");
        }
    }
}