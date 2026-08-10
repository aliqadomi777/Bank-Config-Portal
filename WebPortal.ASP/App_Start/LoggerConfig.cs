using Serilog;
using System;
using System.IO;

namespace WebPortal.ASP.App_Start
{
    public static class LoggerConfig
    {
        public static ILogger CreateLogger()
        {
            string logDirectory = Path.Combine(
                AppDomain.CurrentDomain.BaseDirectory,
                "Logs");

            if (!Directory.Exists(logDirectory))
            {
                Directory.CreateDirectory(logDirectory);
            }

            string logFilePath = Path.Combine(
                logDirectory,
                "errors.json");

            Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Error()
                .WriteTo.File(
                    formatter: new InvariantExpressionTemplate(
                        "{ { Timestamp: ToString(@t, 'yyyy-MM-dd HH:mm:ss zzz'), Message: @m, Exception: @x, Parameters: @p } }\n"
                    ),
                    path: logFilePath,
                    rollingInterval: RollingInterval.Day)
                .CreateLogger();

            return Log.Logger;
        }
    }
}