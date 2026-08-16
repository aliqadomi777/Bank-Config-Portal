using System.Web.Mvc;
using System.Web.Routing;

namespace WebPortal.ASP
{
    public class RouteConfig
    {
        public static void RegisterRoutes(
            RouteCollection routes)
        {
            routes.IgnoreRoute(
                "{resource}.axd/{*pathInfo}");


            routes.MapRoute(
                name: "CreateBranchCounter",
                url: "Branch/{branchId}/Counters/Create",
                defaults: new
                {
                    controller = "Counter",
                    action = "Create"
                }
            );


            routes.MapRoute(
                name: "BranchCounters",
                url: "Branch/{branchId}/Counters",
                defaults: new
                {
                    controller = "Counter",
                    action = "Index"
                }
            );


            routes.MapRoute(
                name: "CreateCounterAllocation",
                url: "Counter/{counterId}/Allocations/Create",
                defaults: new
                {
                    controller = "Allocation",
                    action = "Create"
                }
            );


            routes.MapRoute(
                name: "CounterAllocations",
                url: "Counter/{counterId}/Allocations",
                defaults: new
                {
                    controller = "Allocation",
                    action = "Index"
                }
            );


            routes.MapRoute(
                name: "Default",
                url: "{controller}/{action}/{id}",
                defaults: new
                {
                    controller = "Login",
                    action = "Index",
                    id = UrlParameter.Optional
                }
            );
        }
    }
}