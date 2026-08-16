using App.Shared;
using System;
using System.Linq;
using System.Web.Mvc;
using WebPortal.Application.DTO.Branch;
using WebPortal.Application.Interfaces;
using WebPortal.ASP.Models;

namespace WebPortal.ASP.Controllers
{
    public class BranchController : BaseController
    {
        private readonly IBranchService _branchService;


        public BranchController(
            IBranchService branchService)
        {
            _branchService = branchService;
        }


        [HttpGet]
        public ActionResult Index(int page = 1)
        {
            const int pageSize = 10;

            try
            {
                var branchDTOs = _branchService
                        .GetAllBranches(CurrentBankId)
                        .ToList();

                int totalItems =
                    branchDTOs.Count;

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

                var model = branchDTOs.Skip((page - 1) * pageSize).Take(pageSize).Select(dto =>
                            new BranchViewModel
                            {
                                BankId = dto.BankId,
                                BranchId = dto.BranchId,
                                BranchNameEN = dto.BranchNameEN,
                                BranchNameAR = dto.BranchNameAR,
                                BranchStatus = dto.BranchStatus,
                                ModifiedAt = dto.ModifiedAt
                            }).ToList();

                ViewBag.CurrentPage = page;
                ViewBag.TotalPages = totalPages;

                return View(
                    "Index",
                    model);
            }
            catch (Exception)
            {
                TempData["Message"] =
                    Resources.Resources.GeneralError;

                return View(
                    "Index",
                    Enumerable.Empty<BranchViewModel>());
            }
        }


        [HttpGet]
        public ActionResult Create()
        {
            var model =
                new BranchViewModel
                {
                    BranchStatus = true
                };
            ViewBag.LanguageReturnUrl =
                Url.Action(
                    "Create",
                    "Branch");
            return View(
                "Form",
                model);
        }


        [HttpGet]
        public ActionResult Edit(int id)
        {
            try
            {
                var branch =
                    _branchService
                        .GetBranchById(id);

                if (branch == null)
                {
                    TempData["Message"] =
                        Resources.Resources.ItemDeleted;

                    return RedirectToAction(
                        "Index",
                        "Branch");
                }

                var model =
                    new BranchViewModel
                    {
                        BranchId = branch.BranchId,
                        BranchNameEN = branch.BranchNameEN,
                        BranchNameAR = branch.BranchNameAR,
                        BranchStatus = branch.BranchStatus,
                        ModifiedAt = branch.ModifiedAt,
                        BankId = branch.BankId
                    };
                ViewBag.LanguageReturnUrl =
                    Url.Action(
                        "Edit",
                        "Branch",
                        new
                        {
                            id = model.BranchId
                        });
                return View(
                    "Form",
                    model);
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
            BranchViewModel model)
        {

            ViewBag.LanguageReturnUrl =
                model.BranchId == 0
                    ? Url.Action(
                        "Create",
                        "Branch")
                    : Url.Action(
                        "Edit",
                        "Branch",
                        new
                        {
                            id = model.BranchId
                        });
            if (!ModelState.IsValid)
            {
                return View(
                    "Form",
                    model);
            }

            try
            {
                if (model.BranchId == 0)
                {
                    _branchService.CreateBranch(
                        new BranchCreateRequestDto
                        {
                            BankId = CurrentBankId,
                            BranchNameEN = model.BranchNameEN.Trim(),
                            BranchNameAR = model.BranchNameAR.Trim(),
                            BranchStatus = model.BranchStatus
                        });
                }
                else
                {
                    bool isUpdated = _branchService.UpdateBranch(
                        new BranchUpdateRequestDto
                        {
                            BranchId = model.BranchId,
                            BranchNameEN = model.BranchNameEN.Trim(),
                            BranchNameAR = model.BranchNameAR.Trim(),
                            BranchStatus = model.BranchStatus
                        });

                    if (!isUpdated)
                    {
                        TempData["Message"] = Resources.Resources.ItemDeleted;

                        return RedirectToAction(
                            "Index",
                            "Branch");
                    }
                }
            }
            catch (DuplicateRecordException)
            {
                ModelState.AddModelError(
                    "",
                    Resources.Resources
                        .BranchDuplicateName);

                return View(
                    "Form",
                    model);
            }
            catch (ParentDeletedWithChildConflictException)
            {
                ModelState.AddModelError(
                    "",
                    Resources.Resources
                        .BranchOrphan);

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
                "Branch");
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(int id)
        {
            try
            {
                _branchService
                    .DeleteBranch(id);
            }
            catch (Exception)
            {
                TempData["Message"] =
                    Resources.Resources.GeneralError;
            }

            return RedirectToAction(
                "Index",
                "Branch");
        }
    }
}