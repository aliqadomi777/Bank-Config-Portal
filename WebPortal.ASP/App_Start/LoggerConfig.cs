using Serilog;
using Serilog.Events;
using System;
using System.IO;

namespace WebPortal.ASP.App_Start
{
    public static class LoggerConfig
    {
        private const string EventLogSource = "Bank Configuration Portal";

        private const string EventLogName = "Application";

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
                .WriteTo.EventLog(
                    source:
                        EventLogSource,
                    logName:
                        EventLogName,
                    manageEventSource:
                        false,
                    restrictedToMinimumLevel:
                        LogEventLevel.Error)
                .CreateLogger();

            return Log.Logger;
        }
    }
}