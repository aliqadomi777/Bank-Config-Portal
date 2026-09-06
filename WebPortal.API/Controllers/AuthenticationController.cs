using Microsoft.Owin.Security;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Net;
using System.Security.Claims;
using System.Web.Http;
using WebPortal.API.Models;
using WebPortal.API.Security;
using WebPortal.Application.DTO.User;
using WebPortal.Application.Interfaces;

namespace WebPortal.API.Controllers
{
    [RoutePrefix("api/v1/authentication")]
    public sealed class AuthenticationController : ApiController
    {
        private readonly IUserService _userService;
        private readonly MemoryCacheTokenStore _tokenStore;

        public AuthenticationController(IUserService userService)
        {
            _userService = userService;
            _tokenStore = MemoryCacheTokenStore.Instance;
        }

        [AllowAnonymous]
        [HttpPost]
        [Route("login")]
        public IHttpActionResult Login(UserRequestDto request)
        {
            if (request == null)
            {
                return BadRequest("Login details are required.");
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            UserResponseDto user;

            try
            {
                user = _userService.Login(request);
            }
            catch (UnauthorizedAccessException)
            {
                return Content(
                    HttpStatusCode.Unauthorized,
                    new ApiErrorModel
                    {
                        Code = "INVALID_CREDENTIALS",
                        Message = "The username or password is incorrect."
                    });
            }

            if (user == null)
            {
                return Content(
                    HttpStatusCode.Unauthorized,
                    new ApiErrorModel
                    {
                        Code = "INVALID_CREDENTIALS",
                        Message = "The username or password is incorrect."
                    });
            }

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier,user.UserId.ToString(CultureInfo.InvariantCulture)),
                new Claim(ClaimTypes.Name,user.UserName),
                new Claim(AuthenticationConstants.BankIdClaimType,user.BankId.ToString(CultureInfo.InvariantCulture)),
                new Claim(AuthenticationConstants.BankNameClaimType,user.BankName)
            };

            var identity = new ClaimsIdentity(claims, AuthenticationConstants.AuthenticationType);
            var ticket = new AuthenticationTicket(identity,
                new AuthenticationProperties
                {
                    IsPersistent = false,
                    AllowRefresh = false
                });

            string token = _tokenStore.Store(ticket);

            return Ok(
                new AccessTokenModel
                {
                    AccessToken = token,
                    TokenType = "Bearer",
                    IdleTimeoutSeconds =
                    AuthenticationConstants.SessionTimeoutMinutes * 60
                });
        }


        [HttpDelete]
        [Route("logout")]
        public IHttpActionResult Logout()
        {
            string token = GetBearerToken();

            if (string.IsNullOrWhiteSpace(token))
            {
                return BadRequest("A bearer token is required.");
            }

            _tokenStore.Remove(token);

            return StatusCode(HttpStatusCode.NoContent);
        }

        private string GetBearerToken()
        {
            if (Request.Headers.Authorization == null)
            {
                return null;
            }

            if (!string.Equals(Request.Headers.Authorization.Scheme,
                "Bearer",
                StringComparison.OrdinalIgnoreCase))
            {
                return null;
            }

            return Request.Headers.Authorization.Parameter;
        }
    }
}