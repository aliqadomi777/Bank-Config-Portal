using App.Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Web.Mvc;
using WebPortal.Application.DTO.Allocations;
using WebPortal.Application.Interfaces;
using WebPortal.ASP.Models;

namespace WebPortal.ASP.Controllers
{
    public class AllocationController : BaseController
    {
        private readonly IAllocationService _allocationService;

        private readonly IServiceManager _serviceManager;

        private readonly ICounterService _counterService;
        private readonly IBranchService _branchService;

        public AllocationController(
            IAllocationService allocationService,
            IServiceManager serviceManager,
            ICounterService counterService,
            IBranchService branchService)
        {
            _allocationService = allocationService;

            _serviceManager = serviceManager;

            _counterService = counterService;

            _branchService = branchService;
        }


        [HttpGet]
        public ActionResult Index(
            int counterId,
            int page = 1)
        {
            const int pageSize = 10;

            try
            {
                var counter = _counterService.GetCounterById(counterId, CurrentBankId);

                if (counter == null)
                {
                    TempData["Message"] = Resources.Resources.ItemDeleted;

                    return RedirectToAction(
                        "Index",
                        "Branch");
                }
                if (!LoadAllocationContext(
                                counter.BranchId,
                                counter.CounterNameEN,
                                counter.CounterNameAR))
                {
                    TempData["Message"] = Resources.Resources.ItemDeleted;

                    return RedirectToAction(
                        "Index",
                        "Branch");
                }

                var allocationDTOs = _allocationService
                                        .GetAllAllocations(counterId, CurrentBankId)
                                        .ToList();

                int totalItems = allocationDTOs.Count;

                int totalPages = (int)Math.Ceiling(totalItems / (double)pageSize);

                if (page < 1)
                {
                    page = 1;
                }

                if (totalPages > 0 &&
                    page > totalPages)
                {
                    page = totalPages;
                }

                var model = allocationDTOs.Skip((page - 1) * pageSize).Take(pageSize).Select(dto =>
                            new CounterServiceViewModel
                            {
                                AllocationId = dto.AllocationId,
                                CounterId = dto.CounterId,
                                ServiceId = dto.ServiceId,
                                ServiceNameEN = dto.ServiceNameEN,
                                ServiceNameAR = dto.ServiceNameAR
                            }).ToList();

                ViewBag.CurrentPage = page;
                ViewBag.TotalPages = totalPages;

                ViewBag.CounterId = counterId;

                ViewBag.BranchId = counter.BranchId;

                return View(
                    "List",
                    model);
            }
            catch (UnauthorizedAccessException)
            {
                TempData["Message"] = Resources.Resources.ItemDeleted;

                return RedirectToAction(
                    "Index",
                    "Branch");
            }
            catch (Exception)
            {
                TempData["Message"] = Resources.Resources.GeneralError;

                return RedirectToAction(
                    "Index",
                    "Branch");
            }
        }


        [HttpGet]
        public ActionResult Create(int counterId)
        {
            try
            {
                var counter = _counterService.GetCounterById(counterId, CurrentBankId);

                if (counter == null)
                {
                    TempData["Message"] = Resources.Resources.ItemDeleted;

                    return RedirectToAction(
                        "Index",
                        "Branch");
                }
                if (!LoadAllocationContext(
                counter.BranchId,
                counter.CounterNameEN,
                counter.CounterNameAR))
                {
                    TempData["Message"] = Resources.Resources.ItemDeleted;

                    return RedirectToAction(
                        "Index",
                        "Branch");
                }
                var model =
                    new AllocationFormViewModel
                    {
                        CounterId = counterId
                    };

                LoadAvailableServices(model);

                ViewBag.LanguageReturnUrl =
                    Url.Action(
                        "Create",
                        "Allocation",
                        new
                        {
                            counterId = counterId
                        });

                return View(
                    "Form",
                    model);
            }
            catch (UnauthorizedAccessException)
            {
                TempData["Message"] = Resources.Resources.ItemDeleted;

                return RedirectToAction(
                    "Index",
                    "Branch");
            }
            catch (Exception)
            {
                TempData["Message"] = Resources.Resources.GeneralError;

                return RedirectToAction(
                    "Index",
                    "Allocation",
                    new
                    {
                        counterId = counterId
                    });
            }
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Save(
            AllocationFormViewModel model)
        {
            try
            {
                var counter = _counterService.GetCounterById(model.CounterId, CurrentBankId);

                if (counter == null)
                {
                    TempData["Message"] = Resources.Resources.ItemDeleted;

                    return RedirectToAction(
                        "Index",
                        "Branch");
                }

                model.CounterId = counter.CounterId;
            }
            catch (UnauthorizedAccessException)
            {
                TempData["Message"] = Resources.Resources.ItemDeleted;

                return RedirectToAction(
                    "Index",
                    "Branch");
            }
            catch (Exception)
            {
                TempData["Message"] = Resources.Resources.GeneralError;

                return RedirectToAction(
                    "Index",
                    "Branch");
            }

            ViewBag.LanguageReturnUrl =
                Url.Action(
                    "Create",
                    "Allocation",
                    new
                    {
                        counterId = model.CounterId
                    });

            if (!ModelState.IsValid)
            {
                LoadAvailableServicesSafely(model);

                return View(
                    "Form",
                    model);
            }

            try
            {
                _allocationService.CreateAllocation(
                    new AllocationCreateRequestDto
                    {
                        CounterId =
                            model.CounterId,

                        ServiceId =
                            model.ServiceId
                    },
                    CurrentBankId);
            }
            catch (UnauthorizedAccessException)
            {
                TempData["Message"] = Resources.Resources.ItemDeleted;

                return RedirectToAction(
                    "Index",
                    "Branch");
            }
            catch (DuplicateRecordException)
            {
                ModelState.AddModelError(
                    "",
                    Resources.Resources
                        .AllocationDuplicate);

                LoadAvailableServicesSafely(model);

                return View(
                    "Form",
                    model);
            }
            catch (ParentDeletedWithChildConflictException)
            {
                ModelState.AddModelError(
                    "",
                    Resources.Resources
                        .AllocationOrphan);

                LoadAvailableServicesSafely(model);

                return View(
                    "Form",
                    model);
            }
            catch (Exception)
            {
                ModelState.AddModelError(
                    "",
                    Resources.Resources
                        .GeneralError);

                LoadAvailableServicesSafely(model);

                return View(
                    "Form",
                    model);
            }

            return RedirectToAction(
                "Index",
                "Allocation",
                new
                {
                    counterId =
                        model.CounterId
                });
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(int id, int counterId)
        {
            try
            {
                var allocation = _allocationService.GetAllocationById(id, CurrentBankId);

                if (allocation == null)
                {
                    TempData["Message"] = Resources.Resources.ItemDeleted;

                    return RedirectToAction(
                        "Index",
                        "Branch");
                }

                counterId = allocation.CounterId;

                _allocationService.DeleteAllocation(id, CurrentBankId);
            }
            catch (UnauthorizedAccessException)
            {
                TempData["Message"] = Resources.Resources.ItemDeleted;

                return RedirectToAction(
                    "Index",
                    "Branch");
            }
            catch (Exception)
            {
                TempData["Message"] =
                    Resources.Resources.GeneralError;
            }

            return RedirectToAction(
                "Index",
                "Allocation",
                new
                {
                    counterId = counterId
                });
        }


        private void LoadAvailableServices(
            AllocationFormViewModel model)
        {
            string currentLanguage =
                Thread.CurrentThread
                    .CurrentUICulture
                    .TwoLetterISOLanguageName;

            var allServices =
                _serviceManager
                    .GetAllServices(CurrentBankId)
                    .ToList();

            var allocatedServiceIds =
                _allocationService
                    .GetAllAllocations(model.CounterId, CurrentBankId)
                    .Select(allocation =>
                        allocation.ServiceId)
                    .ToList();

            model.Services =
                allServices
                    .Where(service =>
                        !allocatedServiceIds.Contains(
                            service.ServiceId))
                    .Select(service =>
                        new ServiceOptionViewModel
                        {
                            ServiceId =
                                service.ServiceId,

                            ServiceName =
                                currentLanguage == "ar"
                                    ? service.ServiceNameAR
                                    : service.ServiceNameEN
                        })
                    .ToList();
        }


        private void LoadAvailableServicesSafely(
            AllocationFormViewModel model)
        {
            try
            {
                LoadAvailableServices(model);
            }
            catch
            {
                model.Services =
                    new List<ServiceOptionViewModel>();
            }
        }
        private bool LoadAllocationContext(
                    int branchId,
                    string counterNameEN,
                    string counterNameAR)
        {
            var branch = _branchService.GetBranchById(
                    branchId,
                    CurrentBankId);

            if (branch == null)
            {
                return false;
            }

            bool isArabic = Thread.CurrentThread
                    .CurrentUICulture
                    .TwoLetterISOLanguageName == "ar";

            ViewBag.BranchName = isArabic
                    ? branch.BranchNameAR
                    : branch.BranchNameEN;

            ViewBag.CounterName = isArabic
                    ? counterNameAR
                    : counterNameEN;

            ViewBag.BranchId = branchId;

            return true;
        }
    }
}