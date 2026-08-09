using System;
using System.Linq;
using System.Web.Mvc;
using WebPortal.Application.Interfaces;
using WebPortal.ASP.Models;

namespace WebPortal.ASP.Controllers
{
    public class DashboardController : BaseController
    {
        private readonly IServiceManager _serviceManager;
        private readonly IBranchService _branchService;

        public DashboardController(IBranchService branchService,
                                   IServiceManager serviceManager)
        {
            _serviceManager = serviceManager;
            _branchService = branchService;
        }
        public ActionResult Index()
        {
            if (Session["BankId"] == null)
            {
                //Logout
            }
            int bankId = Convert.ToInt32(Session["BankId"]);
            DashboardViewModel model = new DashboardViewModel();
            var branchDTOs = _branchService.GetAllBranches(bankId);
            var serviceDTOs = _serviceManager.GetAllServices(bankId);

            model.Branches = branchDTOs.Select(dto => new BranchViewModel
            {
                BankId = dto.BankId,
                BranchId = dto.BranchId,
                BranchNameEN = dto.BranchNameEN,
                BranchNameAR = dto.BranchNameAR,
                BranchStatus = dto.BranchStatus,
                ModifiedAt = dto.ModifiedAt
            }).ToList();

            model.Services = serviceDTOs.Select(dto => new ServiceViewModel
            {
                BankId = dto.BankId,
                ServiceId = dto.ServiceId,
                ServiceNameEN = dto.ServiceNameEN,
                ServiceNameAR = dto.ServiceNameAR,
                MaxTicketsPerDay = dto.MaxTicketsPerDay,
                ServiceStatus = dto.ServiceStatus,
                ModifiedAt = dto.ModifiedAt
            }).ToList();

            return View(model);
        }


    }
}