using App.Shared;
using Autofac;
using Autofac.Integration.Mvc;
using Microsoft.Extensions.Logging;
using Serilog;
using System.Web.Mvc;
using WebPortal.Infrastructure;
namespace WebPortal.ASP.App_Start
{
    public class ContainerConfig
    {
        public static string DbErrorMessage { get; private set; } = null;

        internal static void RegisterContainer()
        {
            var builder = new ContainerBuilder();
            string connectionString;
            string dbErrorMessage;

            var serilogLogger = LoggerConfig.CreateLogger("Bank Configuration Portal", "BankPortalLog");
            var loggerFactory = new LoggerFactory();
            loggerFactory.AddSerilog(serilogLogger);

            builder.RegisterInstance(loggerFactory).As<ILoggerFactory>().SingleInstance();
            builder.RegisterGeneric(typeof(Logger<>)).As(typeof(ILogger<>)).InstancePerDependency();

            bool isConnectionStringValid = DatabaseConnectionValidator.TryGetValidConnectionString(
                serilogLogger,
                out connectionString,
                out dbErrorMessage);

            DbErrorMessage = dbErrorMessage;
            if (string.IsNullOrEmpty(DbErrorMessage))
            {
                builder.RegisterModule(new InfrastructureModule(connectionString));
                builder.RegisterModule(new ApplicationModule());
            }
            else
            {
                builder.RegisterModule(new InfrastructureModule(""));
                builder.RegisterModule(new ApplicationModule());
            }

            builder.RegisterControllers(typeof(MvcApplication).Assembly);

            var container = builder.Build();
            DependencyResolver.SetResolver(new AutofacDependencyResolver(container));
        }



    }
}
