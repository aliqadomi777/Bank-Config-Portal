using Autofac;
using Autofac.Integration.Mvc;
using System;
using System.Configuration;
using System.Web.Mvc;
using WebPortal.Infrastructure;

namespace WebPortal.ASP.App_Start
{
    public class ContainerConfig
    {
        internal static void RegisterContainer()
        {
            var builder = new ContainerBuilder();
            string connectionString = ConfigurationManager.ConnectionStrings["DefaultConnection"]?.ConnectionString;
            builder.RegisterModule(new InfrastructureModule(connectionString));
            builder.RegisterModule(new ApplicationModule());
            builder.RegisterControllers(typeof(MvcApplication).Assembly);
            var container = builder.Build();
            DependencyResolver.SetResolver(new AutofacDependencyResolver(container));
            Console.WriteLine(connectionString);

        }
    }
}