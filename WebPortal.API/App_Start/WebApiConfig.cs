using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System.Web.Http;
using System.Web.Http.ExceptionHandling;
using WebPortal.API.App_Start;
using WebPortal.API.ErrorHandling;
using WebPortal.API.Security;

namespace WebPortal.API
{
    public static class WebApiConfig
    {
        public static void Register(HttpConfiguration config)
        {
            config.MessageHandlers.Add(new OpaqueTokenAuthenticationHandler());
            config.Filters.Add(new ApiAuthorizeAttribute());
            config.Services.Replace(typeof(IExceptionHandler), new ApiExceptionHandler());
            config.MapHttpAttributeRoutes();

            config.Formatters.Remove(config.Formatters.XmlFormatter);
            config.Formatters.JsonFormatter.SerializerSettings.ContractResolver = new CamelCasePropertyNamesContractResolver();
            config.Formatters.JsonFormatter.SerializerSettings.NullValueHandling = NullValueHandling.Ignore;
            config.Formatters.JsonFormatter.SerializerSettings.DateTimeZoneHandling = DateTimeZoneHandling.Utc;

            ContainerConfig.Register(config);
        }
    }
}
