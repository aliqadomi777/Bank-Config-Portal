using Autofac;
using Autofac.Integration.Mvc;
using Microsoft.Extensions.Logging;
using Serilog;
using System.Configuration;
using System.Data.SqlClient;
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
            string connectionString = ConfigurationManager.ConnectionStrings["DefaultConnection"]?.ConnectionString;

            var serilogLogger = LoggerConfig.CreateLogger();
            var loggerFactory = new LoggerFactory();
            loggerFactory.AddSerilog(serilogLogger);

            builder.RegisterInstance(loggerFactory).As<ILoggerFactory>().SingleInstance();
            builder.RegisterGeneric(typeof(Logger<>)).As(typeof(ILogger<>)).InstancePerDependency();

            TestDatabaseConnection(connectionString, serilogLogger);

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

        private static void TestDatabaseConnection(string connString, Serilog.ILogger logger)
        {
            if (string.IsNullOrEmpty(connString))
            {
                DbErrorMessage = "Database configuration missing in Web.config.";
                return;
            }

            SqlConnectionStringBuilder timeoutBuilder = new SqlConnectionStringBuilder(connString) { ConnectTimeout = 5 };

            using (SqlConnection conn = new SqlConnection(timeoutBuilder.ConnectionString))
            {
                try
                {
                    conn.Open();
                }
                catch (SqlException ex)
                {
                    string message = "Database Error: ";
                    switch (ex.Number)
                    {
                        case -2: message += "Connection timed out."; break;
                        case 4060: message += "Database name not found."; break;
                        case 18456: message += "Wrong password or username."; break;
                        case 26:
                        case 53: message += "Server not found or inaccessible."; break;
                        default: message += ex.Message; break;
                    }

                    logger.Fatal(ex, ex.Message);

                    DbErrorMessage = message;
                }
            }
        }
    }
}
