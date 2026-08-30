using Microsoft.Owin.Security;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Security.Claims;
using System.Web;
using System.Web.Mvc;
using WebPortal.Application.DTO.User;
using WebPortal.Application.Interfaces;
using WebPortal.ASP.Models;
using WebPortal.ASP.Security;

namespace WebPortal.ASP.Controllers
{
    public class LoginController : Controller
    {
        private readonly IUserService _userService;


        public LoginController(
            IUserService userService)
        {
            _userService = userService;
        }


        private IAuthenticationManager AuthenticationManager
        {
            get
            {
                return HttpContext.GetOwinContext().Authentication;
            }
        }


        [HttpGet]
        [AllowAnonymous]
        [OutputCache(
            NoStore = true,
            Duration = 0,
            VaryByParam = "*")]
        public ActionResult Index()
        {

            if (!string.IsNullOrEmpty(App_Start.ContainerConfig.DbErrorMessage))
            {
                ViewBag.DbError = App_Start.ContainerConfig.DbErrorMessage;
            }


            return View("Index");
        }


        [HttpPost]
        [AllowAnonymous]
        public ActionResult Login(
            LoginViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View("Index", model);
            }

            var requestDto =
                new UserRequestDto
                {
                    BankName = model.BankName.Trim(),
                    UserName = model.Username.Trim(),
                    Password = model.Password.Trim()
                };

            try
            {
                var response = _userService.Login(requestDto);

                if (response == null)
                {
                    TempData["Error"] =
                        Resources.Resources.Unauthorized;

                    TempData["LoginModel"] = model;

                    return RedirectToAction("Index");
                }

                var tabCookie = Request.Cookies["X-Tab-Gatekeeper"];
                string tabId = tabCookie != null ? tabCookie.Value : "unknown";

                var claims =
                    new List<Claim>
                    {
                        new Claim(
                            ClaimTypes.NameIdentifier,
                            response.UserId.ToString(
                                CultureInfo.InvariantCulture)),

                        new Claim(
                            ClaimTypes.Name,
                            response.UserName),

                        new Claim(AuthenticationConstants.BankIdClaimType,
                            response.BankId.ToString(
                                CultureInfo.InvariantCulture)),

                        new Claim(AuthenticationConstants.BankNameClaimType,
                            response.BankName),

                        new Claim(AuthenticationConstants.UserAgentClaimType,
                            Request.UserAgent??""),
                        new Claim(AuthenticationConstants.ActiveTabId,
                            tabId)

                    };


                var identity =
                    new ClaimsIdentity(
                        claims,
                        AuthenticationConstants.AuthenticationType);

                AuthenticationManager.SignOut(AuthenticationConstants.AuthenticationType);

                AuthenticationManager.SignIn(
                    new AuthenticationProperties
                    {
                        IsPersistent = false,
                        AllowRefresh = true
                    },
                    identity);


                return RedirectToAction(
                    "Index",
                    "Dashboard");
            }
            catch (UnauthorizedAccessException)
            {
                TempData["Error"] =
                    Resources.Resources.Unauthorized;

                TempData["LoginModel"] = model;

                return RedirectToAction("Index");
            }
            catch (Exception)
            {
                TempData["Error"] =
                    Resources.Resources.GeneralError;

                TempData["LoginModel"] = model;

                return RedirectToAction("Index");
            }
        }


        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken]
        public ActionResult Logout()
        {
            AuthenticationManager.SignOut(AuthenticationConstants.AuthenticationType);


            return RedirectToAction(
                "Index",
                "Login");
        }
    }
}