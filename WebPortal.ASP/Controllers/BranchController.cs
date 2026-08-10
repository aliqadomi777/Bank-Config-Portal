using App.Shared;
using System;
using System.Web.Mvc;
using WebPortal.Application.DTO.Branch;
using WebPortal.Application.Interfaces;
using WebPortal.ASP.Models;


namespace WebPortal.ASP.Controllers
{
    public class BranchController : BaseController
    {
        private readonly IBranchService _branchService;
        private BranchResponseDto _branch;
        public BranchController(IBranchService branchService)
        {
            _branchService = branchService;
        }

        [HttpGet]
        public ActionResult Create()
        {
            var model = new BranchViewModel
            {
                BranchStatus = true
            };

            return View("Index", model);
        }

        [HttpGet]
        public ActionResult Edit(int id)
        {
            try
            {
                _branch = _branchService.GetBranchById(id);

            }
            catch (Exception)
            {
                TempData["Message"] = Resources.Resources.GeneralError;
                return RedirectToAction("Index", "Dashboard");
            }
            if (_branch == null)
            {
                TempData["Message"] = Resources.Resources.ItemDeleted;

                return RedirectToAction("Index", "Dashboard");
            }

            var model = new BranchViewModel
            {
                BranchId = _branch.BranchId,
                BranchNameEN = _branch.BranchNameEN,
                BranchNameAR = _branch.BranchNameAR,
                BranchStatus = _branch.BranchStatus,
                ModifiedAt = _branch.ModifiedAt,
                BankId = _branch.BankId
            };

            return View("Index", model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Save(BranchViewModel model)
        {
            int bankId = Convert.ToInt32(Session["BankId"]);

            if (!ModelState.IsValid)
            {
                TempData["BranchModel"] = model;
                return RedirectToSource(model.BranchId);

            }

            try
            {
                if (model.BranchId == 0)
                {
                    _branchService.CreateBranch(new BranchCreateRequestDto
                    {
                        BankId = bankId,
                        BranchNameEN = model.BranchNameEN.Trim(),
                        BranchNameAR = model.BranchNameAR.Trim(),
                        BranchStatus = model.BranchStatus
                    });
                }
                else
                {
                    _branchService.UpdateBranch(new BranchUpdateRequestDto
                    {
                        BranchId = model.BranchId,
                        BranchNameEN = model.BranchNameEN.Trim(),
                        BranchNameAR = model.BranchNameAR.Trim(),
                        BranchStatus = model.BranchStatus

                    });
                }
            }
            catch (DuplicateRecordException)
            {
                TempData["Error"] = Resources.Resources.BranchDuplicateName;
                TempData["BranchModel"] = model;
                return RedirectToSource(model.BranchId);
            }

            catch (ParentDeletedWithChildConflictException)
            {
                TempData["Error"] = Resources.Resources.BranchOrphan;
                TempData["BranchModel"] = model;
                return RedirectToSource(model.BranchId);
            }
            catch (Exception)
            {
                TempData["Error"] = Resources.Resources.GeneralError;
                TempData["BranchModel"] = model;
                return RedirectToSource(model.BranchId);
            }

            return RedirectToAction("Index", "Dashboard");
        }

        private ActionResult RedirectToSource(int branchId)
        {
            if (branchId == 0)
            {
                return RedirectToAction("Create");
            }
            return RedirectToAction("Edit", new { id = branchId });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(int id)
        {
            // if and try catch
            try
            {
                bool isDeleted = _branchService.DeleteBranch(id);
            }
            catch (Exception)
            {
                TempData["Message"] = Resources.Resources.GeneralError;
            }

            return RedirectToAction("Index", "Dashboard");
        }
    }
}