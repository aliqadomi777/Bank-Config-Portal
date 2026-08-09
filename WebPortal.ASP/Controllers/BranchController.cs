using System.Web.Mvc;
using WebPortal.Application.Interfaces;
namespace WebPortal.ASP.Controllers
{

    public class BranchController : BaseController
    {
        private readonly IBranchService _branchService;

        public BranchController(IBranchService branchService)
        {
            _branchService = branchService;
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(int id)
        {
            _branchService.DeleteBranch(id);

            return RedirectToAction("Index", "Dashboard");
        }
    }
}