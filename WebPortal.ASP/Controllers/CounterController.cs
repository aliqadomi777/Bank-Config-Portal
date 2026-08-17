using App.Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using WebPortal.Application.DTO.Counter;
using WebPortal.Application.DTO.CounterType;
using WebPortal.Application.Interfaces;
using WebPortal.ASP.Models;

namespace WebPortal.ASP.Controllers
{
    public class CounterController : BaseController
    {
        private readonly ICounterService _counterService;

        private readonly ICounterTypeService
            _counterTypeService;


        public CounterController(
            ICounterService counterService,
            ICounterTypeService counterTypeService)
        {
            _counterService = counterService;

            _counterTypeService = counterTypeService;
        }


        [HttpGet]
        public ActionResult Index(
            int branchId,
            int page = 1)
        {
            const int pageSize = 10;

            try
            {
                var counterDTOs = _counterService.GetAllCounters(branchId, CurrentBankId).ToList();

                int totalItems =
                    counterDTOs.Count;

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

                var model = counterDTOs.Skip((page - 1) * pageSize).Take(pageSize).Select(dto =>
                            new CounterViewModel
                            {
                                CounterId = dto.CounterId,
                                BranchId = dto.BranchId,
                                CounterNameEN = dto.CounterNameEN,
                                CounterNameAR = dto.CounterNameAR,
                                CounterStatus = dto.CounterStatus,
                                CounterTypeId = dto.TypeID,
                                CounterTypeName = dto.TypeName,
                                ModifiedAt = dto.ModifiedAt
                            }).ToList();

                ViewBag.CurrentPage = page;
                ViewBag.TotalPages = totalPages;
                ViewBag.BranchId = branchId;

                return View(
                    "Index",
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
                TempData["Message"] =
                    Resources.Resources.GeneralError;

                return RedirectToAction(
                    "Index",
                    "Branch");
            }
        }


        [HttpGet]
        public ActionResult Create(int branchId)
        {
            var model =
                new CounterViewModel
                {
                    BranchId = branchId,
                    CounterStatus = true
                };

            try
            {
                _counterService.GetAllCounters(branchId, CurrentBankId);

                LoadCounterTypes(model);

                ViewBag.LanguageReturnUrl =
                    Url.Action(
                        "Create",
                        "Counter",
                        new
                        {
                            branchId = branchId
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
                TempData["Message"] =
                    Resources.Resources.GeneralError;

                return RedirectToAction(
                    "Index",
                    "Counter",
                    new
                    {
                        branchId = branchId
                    });
            }
        }


        [HttpGet]
        public ActionResult Edit(int id)
        {
            try
            {
                var counter = _counterService.GetCounterById(id, CurrentBankId);

                if (counter == null)
                {
                    TempData["Message"] =
                        Resources.Resources.ItemDeleted;

                    return RedirectToAction(
                        "Index",
                        "Branch");
                }

                var model =
                    new CounterViewModel
                    {
                        CounterId = counter.CounterId,
                        BranchId = counter.BranchId,
                        CounterNameEN = counter.CounterNameEN,
                        CounterNameAR = counter.CounterNameAR,
                        CounterStatus = counter.CounterStatus,
                        CounterTypeId = counter.TypeID,
                        CounterTypeName = counter.TypeName,
                        ModifiedAt = counter.ModifiedAt
                    };

                LoadCounterTypes(model);

                ViewBag.LanguageReturnUrl =
                    Url.Action(
                        "Edit",
                        "Counter",
                        new
                        {
                            id = model.CounterId
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
                TempData["Message"] =
                    Resources.Resources.GeneralError;

                return RedirectToAction(
                    "Index",
                    "Branch");
            }
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Save(
            CounterViewModel model)
        {
            try
            {
                if (model.CounterId == 0)
                {
                    _counterService.GetAllCounters(model.BranchId, CurrentBankId);
                }
                else
                {
                    var counter = _counterService.GetCounterById(model.CounterId, CurrentBankId);

                    if (counter == null)
                    {
                        TempData["Message"] = Resources.Resources.ItemDeleted;

                        return RedirectToAction(
                            "Index",
                            "Branch");
                    }

                    model.BranchId = counter.BranchId;
                }
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
                model.CounterId == 0
                    ? Url.Action(
                        "Create",
                        "Counter",
                        new
                        {
                            branchId = model.BranchId
                        })
                    : Url.Action(
                        "Edit",
                        "Counter",
                        new
                        {
                            id = model.CounterId
                        });

            if (!ModelState.IsValid)
            {
                LoadCounterTypesSafely(model);

                return View(
                    "Form",
                    model);
            }

            try
            {
                if (model.CounterId == 0)
                {
                    _counterService.CreateCounter(
                        new CounterCreateRequestDto
                        {
                            BranchId = model.BranchId,
                            CounterNameEN = model.CounterNameEN.Trim(),
                            CounterNameAR = model.CounterNameAR.Trim(),
                            CounterStatus = model.CounterStatus,
                            TypeID = model.CounterTypeId
                        },
                        CurrentBankId);
                }
                else
                {
                    bool isUpdated = _counterService.UpdateCounter(
                         new CounterUpdateRequestDto
                         {
                             CounterId = model.CounterId,
                             CounterNameEN = model.CounterNameEN.Trim(),
                             CounterNameAR = model.CounterNameAR.Trim(),
                             CounterStatus = model.CounterStatus,
                             TypeID = model.CounterTypeId
                         },
                         CurrentBankId);

                    if (!isUpdated)
                    {
                        TempData["Message"] = Resources.Resources.ItemDeleted;

                        return RedirectToAction(
                            "Index",
                            "Branch");
                    }
                }
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
                        .CounterDuplicateName);

                LoadCounterTypesSafely(model);

                return View(
                    "Form",
                    model);
            }
            catch (ParentDeletedWithChildConflictException)
            {
                ModelState.AddModelError(
                    "",
                    Resources.Resources
                        .CounterOrphan);

                LoadCounterTypesSafely(model);

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

                LoadCounterTypesSafely(model);

                return View(
                    "Form",
                    model);
            }

            return RedirectToAction(
                "Index",
                "Counter",
                new
                {
                    branchId = model.BranchId
                });
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(
            int id,
            int branchId)
        {
            try
            {
                var counter = _counterService.GetCounterById(id, CurrentBankId);

                if (counter == null)
                {
                    TempData["Message"] = Resources.Resources.ItemDeleted;

                    return RedirectToAction(
                        "Index",
                        "Branch");
                }

                branchId = counter.BranchId;

                _counterService.DeleteCounter(id, CurrentBankId);
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
                "Counter",
                new
                {
                    branchId = branchId
                });
        }


        private void LoadCounterTypes(
            CounterViewModel model)
        {
            IEnumerable<CounterTypeResponseDto>
                counterTypes = _counterTypeService.GetAllCounterTypes();

            model.Types =
                counterTypes
                    .Select(type =>
                        new CounterTypeViewModel
                        {
                            TypeId = type.TypeId,
                            TypeName = type.TypeName
                        })
                    .ToList();
        }


        private void LoadCounterTypesSafely(
            CounterViewModel model)
        {
            try
            {
                LoadCounterTypes(model);
            }
            catch
            {
                model.Types =
                    new List<CounterTypeViewModel>();
            }
        }
    }
}