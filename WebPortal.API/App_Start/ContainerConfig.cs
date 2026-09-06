using App.Shared;
using Autofac;
using Autofac.Integration.WebApi;
using Microsoft.Extensions.Logging;
using Serilog;
using System;
using System.Configuration;
using System.Reflection;
using System.Web.Http;
using WebPortal.Infrastructure;

namespace WebPortal.API.App_Start
{
    public static class ContainerConfig
    {
        public static string DbErrorMessage { get; private set; } = null;

        public static void Register(HttpConfiguration config)
        {
            var builder = new ContainerBuilder();
            string connectionString;
            string dbErrorMessage;

            var serilogLogger = LoggerConfig.CreateLogger("Ticket Screen API", "TicketScreenApiLog");
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
            builder.RegisterApiControllers(Assembly.GetExecutingAssembly());

            IContainer container = builder.Build();

            config.DependencyResolver = new AutofacWebApiDependencyResolver(container);
        }
    }
}