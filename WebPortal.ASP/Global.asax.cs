using Serilog;
using System.Web;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using WebPortal.ASP.App_Start;

namespace WebPortal.ASP
{
    public class MvcApplication : HttpApplication
    {
        protected void Application_Start()
        {
            ContainerConfig.RegisterContainer();
            AreaRegistration.RegisterAllAreas();
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);
        }
        protected void Application_End()
        {
            Log.CloseAndFlush();
        }
    }
}
