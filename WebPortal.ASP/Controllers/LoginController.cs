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

                Session["BankId"] = response.BankId;
                Session["UserName"] = response.UserName;

                return RedirectToAction("Index", "Dashboard");
            }

            catch (UnauthorizedAccessException ex)
            {
                ModelState.AddModelError("", ex.Message);
                return View("Index", model);
            }
            catch (Exception)
            {
                ModelState.AddModelError("", "An error occurred while processing your request.");
                return View("Index", model);
            }
        }


        //This will be used for future adding of logout button through the app
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
