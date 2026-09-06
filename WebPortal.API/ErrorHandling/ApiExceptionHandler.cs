using System.Net;
using System.Net.Http;
using System.Web.Http.ExceptionHandling;
using System.Web.Http.Results;
using WebPortal.API.Models;

namespace WebPortal.API.ErrorHandling
{
    public sealed class ApiExceptionHandler : ExceptionHandler
    {
        public override void Handle(
            ExceptionHandlerContext context)
        {
            context.Result = new ResponseMessageResult(
                context.Request.CreateResponse(
                    HttpStatusCode.InternalServerError,
                    new ApiErrorModel
                    {
                        Code = "INTERNAL_SERVER_ERROR",
                        Message =
                            "An unexpected error occurred " +
                            "while processing the request."
                    }));
        }
    }
}