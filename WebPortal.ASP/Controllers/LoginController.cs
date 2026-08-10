using System;
using System.Web.Mvc;
using WebPortal.Application.DTO.User;
using WebPortal.Application.Interfaces;
using WebPortal.ASP.Models;

namespace WebPortal.ASP.Controllers
{
    public class LoginController : Controller
    {
        private readonly IUserService _userService;

        public LoginController(IUserService userService)
        {
            _userService = userService;
        }
        [OutputCache(NoStore = true, Duration = 0, VaryByParam = "*")]
        public ActionResult Index()
        {
            Session.Clear();
            Session.Abandon();
            var model = TempData["LoginModel"] as LoginViewModel ?? new LoginViewModel();
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Login(LoginViewModel model)
        {
            if (!ModelState.IsValid) return View(model);

            var requestDto = new UserRequestDto
            {
                BankName = model.BankName,
                UserName = model.Username,
                Password = model.Password
            };


            try
            {
                var response = _userService.Login(requestDto);
                if (response == null)
                {
                    TempData["LoginModel"] = model;
                    return RedirectToAction("Index");
                }

                Session["BankId"] = response.BankId;
                Session["UserName"] = response.UserName;

                return RedirectToAction("Index", "Dashboard");

            }

            catch (UnauthorizedAccessException)
            {
                TempData["Error"] = Resources.Resources.Unauthorized;
                TempData["LoginModel"] = model;
                return RedirectToAction("Index");
            }
            catch (Exception)
            {
                TempData["Error"] = Resources.Resources.GeneralError;
                TempData["LoginModel"] = model;
                return RedirectToAction("Index");
            }
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Logout()
        {
            Session.Clear();
            Session.Abandon();
            return RedirectToAction("Index", "Login");
        }
    }

}
