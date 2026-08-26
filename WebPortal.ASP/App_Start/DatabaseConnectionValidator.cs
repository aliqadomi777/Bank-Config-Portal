using Serilog;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;

namespace WebPortal.ASP.App_Start
{
    public static class DatabaseConnectionValidator
    {
        public static bool TryGetValidConnectionString(
            ILogger logger,
            out string connectionString,
            out string errorMessage)
        {
            connectionString = null;
            errorMessage = null;

            try
            {
                ConnectionStringSettings settings = ConfigurationManager.ConnectionStrings["DefaultConnection"];

                if (settings == null)
                {
                    errorMessage = "DefaultConnection is missing from Web.config.";

                    return false;
                }

                if (string.IsNullOrWhiteSpace(settings.ConnectionString))
                {
                    errorMessage = "Database connection string is empty.";

                    return false;
                }


                SqlConnectionStringBuilder builder;

                try
                {
                    builder = new SqlConnectionStringBuilder(settings.ConnectionString);
                }
                catch (Exception ex)
                {
                    logger.Error(ex, "Database connection string has an invalid format.");
                    errorMessage = "Database connection string has an invalid format.";

                    return false;
                }



                var missing = new List<string>();

                if (string.IsNullOrWhiteSpace(builder.DataSource))
                {
                    missing.Add("Server");
                }

                if (string.IsNullOrWhiteSpace(builder.InitialCatalog))
                {
                    missing.Add("Database");
                }

                if (!builder.IntegratedSecurity)
                {
                    if (string.IsNullOrWhiteSpace(builder.UserID))
                    {
                        missing.Add("User ID");
                    }


                    if (string.IsNullOrEmpty(builder.Password))
                    {
                        missing.Add("Password");
                    }
                }

                if (missing.Count > 0)
                {
                    errorMessage = "Missing database configuration: " + string.Join(", ", missing) + ".";
                    logger.Error(errorMessage);
                    return false;
                }

                if (builder.IntegratedSecurity &&
                    (!string.IsNullOrWhiteSpace(builder.UserID) ||
                    !string.IsNullOrEmpty(builder.Password)))
                {
                    errorMessage = "Integrated Security is enabled, so User ID and Password should not be provided.";
                    logger.Error(errorMessage);
                    return false;
                }

                builder.ConnectTimeout = 5;
                connectionString = builder.ConnectionString;


                using (var connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                }


                return true;
            }
            catch (SqlException ex)
            {
                logger.Error(ex, "Database connection failed.");


                switch (ex.Number)
                {
                    case -2:
                        errorMessage = "Database connection timed out.";
                        break;
                    case 2:
                    case 26:
                    case 53:
                        errorMessage = "SQL Server was not found or is not accessible.";
                        break;
                    case 4060:
                        errorMessage = "SQL Server was found, but the configured database could not be opened." +
                            " Check the database name and permissions.";
                        break;

                    case 18452:
                        errorMessage = "Windows authentication failed. Check the Windows account and SQL Server permissions.";
                        break;

                    case 18456:
                        errorMessage = "SQL Server authentication failed. Check the User ID, Password, and authentication mode.";
                        break;
                    default:
                        errorMessage =
                            "Unable to connect to the database." +
                            " Check the server, database, authentication settings, and network connection.";

                        break;
                }
                logger.Error(errorMessage);
                return false;
            }
            catch (ConfigurationErrorsException ex)
            {
                logger.Error(ex, "Database configuration in Web.config is invalid.");
                errorMessage = "Database configuration in Web.config is invalid.";
                return false;
            }
            catch (Exception ex)
            {
                logger.Error(ex, "An unexpected error occurred while configuring the database.");
                errorMessage = "An unexpected error occurred while configuring the database.";
                return false;
            }
        }
    }
}