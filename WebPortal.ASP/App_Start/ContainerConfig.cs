using Autofac;
using Autofac.Integration.Mvc;
using Microsoft.Extensions.Logging;
using Serilog;
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
            var serilogLogger = LoggerConfig.CreateLogger();
            var loggerFactory = new LoggerFactory();
            loggerFactory.AddSerilog(serilogLogger);
            builder.RegisterInstance(loggerFactory).As<ILoggerFactory>().SingleInstance();
            builder.RegisterGeneric(typeof(Logger<>)).As(typeof(ILogger<>)).InstancePerDependency();
            builder.RegisterModule(new InfrastructureModule(connectionString));
            builder.RegisterModule(new ApplicationModule());
            builder.RegisterControllers(typeof(MvcApplication).Assembly);
            var container = builder.Build();
            DependencyResolver.SetResolver(new AutofacDependencyResolver(container));
            Console.WriteLine(connectionString);

        }
    }
}