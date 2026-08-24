using App.Shared;
using System;
using System.Linq;
using System.Web.Mvc;
using WebPortal.Application.DTO.Service;
using WebPortal.Application.Interfaces;
using WebPortal.ASP.Models;

namespace WebPortal.ASP.Controllers
{
    public class ServiceController : BaseController
    {
        private readonly IServiceManager _serviceManager;


        public ServiceController(
            IServiceManager serviceManager)
        {
            _serviceManager = serviceManager;
        }


        [HttpGet]
        public ActionResult Index(int page = 1)
        {
            const int pageSize = 10;

            try
            {
                var serviceDTOs = _serviceManager.GetAllServices(CurrentBankId).ToList();

                int totalItems = serviceDTOs.Count;

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

                var model = serviceDTOs.Skip((page - 1) * pageSize).Take(pageSize).Select(dto =>
                            new ServiceViewModel
                            {
                                BankId = dto.BankId,
                                ServiceId = dto.ServiceId,
                                ServiceNameEN = dto.ServiceNameEN,
                                ServiceNameAR = dto.ServiceNameAR,
                                MaxTicketsPerDay = dto.MaxTicketsPerDay,
                                ServiceStatus = dto.ServiceStatus,
                                ModifiedAt = dto.ModifiedAt,
                                MaximumServiceTime = dto.MaximumServiceTime,
                                MinimumServiceTime = dto.MinimumServiceTime
                            })
                        .ToList();

                ViewBag.CurrentPage = page;
                ViewBag.TotalPages = totalPages;

                return View("List", model);
            }
            catch (Exception)
            {
                TempData["Message"] =
                    Resources.Resources.GeneralError;

                return View(
                    "List",
                    Enumerable.Empty<ServiceViewModel>());
            }
        }


        [HttpGet]
        public ActionResult Create()
        {
            var model = new ServiceViewModel
            {
                ServiceStatus = true,
                MaxTicketsPerDay = 1,
                MinimumServiceTime = 45,
                MaximumServiceTime = 300

            };

            ViewBag.LanguageReturnUrl =
                Url.Action(
                    "Create",
                    "Service");

            return View("Form", model);
        }


        [HttpGet]
        public ActionResult Edit(int id)
        {
            try
            {
                var service = _serviceManager.GetServiceById(id, CurrentBankId);

                if (service == null)
                {
                    TempData["Message"] = Resources.Resources.ItemDeleted;

                    return RedirectToAction(
                        "Index",
                        "Service");
                }

                var model =
                    new ServiceViewModel
                    {
                        ServiceId = service.ServiceId,
                        ServiceNameEN = service.ServiceNameEN,
                        ServiceNameAR = service.ServiceNameAR,
                        MaxTicketsPerDay = service.MaxTicketsPerDay,
                        ServiceStatus = service.ServiceStatus,
                        ModifiedAt = service.ModifiedAt,
                        BankId = service.BankId,
                        MinimumServiceTime = service.MinimumServiceTime,
                        MaximumServiceTime = service.MaximumServiceTime,
                    };

                ViewBag.LanguageReturnUrl =
                        Url.Action(
                            "Edit",
                            "Service",
                            new
                            {
                                id = model.ServiceId
                            });

                return View("Form", model);
            }
            catch (UnauthorizedAccessException)
            {
                TempData["Message"] = Resources.Resources.ItemDeleted;

                return RedirectToAction(
                    "Index",
                    "Service");
            }
            catch (Exception)
            {
                TempData["Message"] =
                    Resources.Resources.GeneralError;

                return RedirectToAction(
                    "Index",
                    "Service");
            }
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Save(
            ServiceViewModel model)
        {

            ViewBag.LanguageReturnUrl =
                model.ServiceId == 0
                    ? Url.Action(
                        "Create",
                        "Service")
                    : Url.Action(
                        "Edit",
                        "Service",
                        new
                        {
                            id = model.ServiceId
                        });

            if (!ModelState.IsValid)
            {
                return View(
                    "Form",
                    model);
            }

            try
            {
                if (model.ServiceId == 0)
                {
                    _serviceManager.CreateService(
                        new ServiceCreateRequestDto
                        {
                            ServiceNameEN = model.ServiceNameEN.Trim(),
                            ServiceNameAR = model.ServiceNameAR.Trim(),
                            ServiceStatus = model.ServiceStatus,
                            MaxTicketsPerDay = model.MaxTicketsPerDay,
                            MinimumServiceTime = model.MinimumServiceTime,
                            MaximumServiceTime = model.MaximumServiceTime,
                        },
                        CurrentBankId);
                }
                else
                {
                    bool isUpdated = _serviceManager.UpdateService(
                        new ServiceUpdateRequestDto
                        {
                            ServiceId = model.ServiceId,
                            ServiceNameEN = model.ServiceNameEN.Trim(),
                            ServiceNameAR = model.ServiceNameAR.Trim(),
                            ServiceStatus = model.ServiceStatus,
                            MaxTicketsPerDay = model.MaxTicketsPerDay,
                            MinimumServiceTime = model.MinimumServiceTime,
                            MaximumServiceTime = model.MaximumServiceTime,
                        },
                        CurrentBankId);

                    if (!isUpdated)
                    {
                        TempData["Message"] = Resources.Resources.ItemDeleted;

                        return RedirectToAction(
                            "Index",
                            "Service");
                    }
                }
            }
            catch (UnauthorizedAccessException)
            {
                TempData["Message"] = Resources.Resources.ItemDeleted;

                return RedirectToAction(
                    "Index",
                    "Service");
            }
            catch (DuplicateRecordException)
            {
                ModelState.AddModelError(
                    "",
                    Resources.Resources
                        .ServiceDuplicateName);

                return View(
                    "Form",
                    model);
            }
            catch (ParentDeletedWithChildConflictException)
            {
                ModelState.AddModelError(
                    "",
                    Resources.Resources
                        .ServiceOrphan);

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

                return View(
                    "Form",
                    model);
            }

            return RedirectToAction(
                "Index",
                "Service");
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(int id)
        {
            try
            {
                _serviceManager.DeleteService(id, CurrentBankId);
            }
            catch (UnauthorizedAccessException)
            {
                TempData["Message"] = Resources.Resources.ItemDeleted;
            }
            catch (Exception)
            {
                TempData["Message"] =
                    Resources.Resources.GeneralError;
            }

            return RedirectToAction(
                "Index",
                "Service");
        }
    }
}