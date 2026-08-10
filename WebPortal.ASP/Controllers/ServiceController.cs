using App.Shared;
using System;
using System.Web.Mvc;
using WebPortal.Application.DTO.Service;
using WebPortal.Application.Interfaces;
using WebPortal.ASP.Models;
namespace WebPortal.ASP.Controllers
{
    public class ServiceController : BaseController
    {
        private readonly IServiceManager _serviceManager;
        private ServiceResponseDto _service;

        public ServiceController(IServiceManager serviceManager)
        {
            _serviceManager = serviceManager;
        }

        [HttpGet]
        public ActionResult Create()
        {
            var model = new ServiceViewModel
            {
                ServiceStatus = true
            };

            return View("Index", model);
        }

        [HttpGet]
        public ActionResult Edit(int id)
        {
            try
            {
                _service = _serviceManager.GetServiceById(id);
            }

            catch (Exception)
            {
                TempData["Message"] = Resources.Resources.GeneralError;
                return RedirectToAction("Index", "Dashboard");
            }

            if (_service == null)
            {
                TempData["Message"] = Resources.Resources.ItemDeleted;
                return RedirectToAction("Index", "Dashboard");
            }

            var model = new ServiceViewModel
            {
                ServiceId = _service.ServiceId,
                ServiceNameEN = _service.ServiceNameEN,
                ServiceNameAR = _service.ServiceNameAR,
                MaxTicketsPerDay = _service.MaxTicketsPerDay,
                ServiceStatus = _service.ServiceStatus,
                ModifiedAt = _service.ModifiedAt,
                BankId = _service.BankId,
            };

            return View("Index", model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Save(ServiceViewModel model)
        {
            int bankId = Convert.ToInt32(Session["BankId"]);
            if (!ModelState.IsValid)
            {
                TempData["ServiceModel"] = model;
                return RedirectToSource(model.ServiceId);
            }

            try
            {
                if (model.ServiceId == 0)
                {
                    _serviceManager.CreateService(new ServiceCreateRequestDto
                    {
                        BankId = bankId,
                        ServiceNameEN = model.ServiceNameEN.Trim(),
                        ServiceNameAR = model.ServiceNameAR.Trim(),
                        ServiceStatus = model.ServiceStatus,
                        MaxTicketsPerDay = model.MaxTicketsPerDay
                    });

                }
                else
                {

                    _serviceManager.UpdateService(new ServiceUpdateRequestDto
                    {
                        ServiceId = model.ServiceId,
                        ServiceNameEN = model.ServiceNameEN.Trim(),
                        ServiceNameAR = model.ServiceNameAR.Trim(),
                        ServiceStatus = model.ServiceStatus,
                        MaxTicketsPerDay = model.MaxTicketsPerDay
                    });


                }
            }
            catch (DuplicateRecordException)
            {
                TempData["Error"] = Resources.Resources.ServiceDuplicateName;
                TempData["ServiceModel"] = model;
                return RedirectToSource(model.ServiceId);
            }

            catch (ParentDeletedWithChildConflictException)
            {
                TempData["Error"] = Resources.Resources.ServiceOrphan;
                TempData["ServiceModel"] = model;
                return RedirectToSource(model.ServiceId);
            }
            catch (Exception)
            {
                TempData["Error"] = Resources.Resources.GeneralError;
                TempData["ServiceModel"] = model;
                return RedirectToSource(model.ServiceId);
            }

            return RedirectToAction("Index", "Dashboard");
        }

        private ActionResult RedirectToSource(int serviceId)
        {
            if (serviceId == 0)
            {
                return RedirectToAction("Create");
            }
            return RedirectToAction("Edit", new { id = serviceId });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(int id)
        {
            try
            {
                bool isDeleted = _serviceManager.DeleteService(id);
            }
            catch (Exception)
            {
                TempData["Message"] = Resources.Resources.GeneralError;
            }

            return RedirectToAction("Index", "Dashboard");
        }
    }
}