using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Controllers;
using WebPortal.API.Models;

namespace WebPortal.API.Security
{
    public sealed class ApiAuthorizeAttribute : AuthorizeAttribute
    {
        protected override void HandleUnauthorizedRequest(
            HttpActionContext actionContext)
        {
            actionContext.Response =
                actionContext.Request.CreateResponse(
                    HttpStatusCode.Unauthorized,
                    new ApiErrorModel
                    {
                        Code = "INVALID_AUTHENTICATION",
                        Message =
                            "A valid bearer token is required. " +
                            "The token may be missing, invalid, expired, or logged out."
                    });
        }
    }
}