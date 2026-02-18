using log4net;
using log4net.Config;
using log4net.Repository.Hierarchy;
using System;
using System.IO;

namespace OrangeHRM_SeleniumCSharp.com.orangeHRM.qa.Utilities
{
    public static class Logger
    {
        private static readonly ILog? log;

        static Logger()
        {
            try
            {
                // Prefer an external 'log4net.config' file located in the test output directory.
                var baseDir = AppDomain.CurrentDomain.BaseDirectory ?? Directory.GetCurrentDirectory();
                var configPath = Path.Combine(baseDir, "log4net.config");

                if (File.Exists(configPath))
                {
                    XmlConfigurator.Configure(new FileInfo(configPath));
                }
                else
                {
                    // Avoid calling XmlConfigurator.Configure() against a missing <log4net> section in the
                    // default app config (which causes a log4net error). Use a basic configuration when no
                    // explicit config file is present.
                    BasicConfigurator.Configure();
                }
            }
            catch
            {
                // Ensure at least a minimal configuration exists
                try { BasicConfigurator.Configure(); } catch { }
            }

            try
            {
                log = LogManager.GetLogger(typeof(Logger));
            }
            catch (Exception ex)
            {
                // If log4net completely fails to initialize, fall back to console logging to avoid
                // throwing TypeInitializationException for the Logger type. Store null in 'log' and
                // write an initial message to the console so users know something went wrong.
                try { Console.WriteLine($"[Logger] Failed to initialize log4net: {ex.Message}"); } catch { }
                log = null;
            }
        }

        public static void Info(string message)
        {
            if (log?.IsInfoEnabled ?? false) log.Info(message);
            else Console.WriteLine($"INFO: {message}");
        }

        public static void Debug(string message)
        {
            if (log?.IsDebugEnabled ?? false) log.Debug(message);
            else Console.WriteLine($"DEBUG: {message}");
        }

        public static void Warn(string message)
        {
            if (log?.IsWarnEnabled ?? false) log.Warn(message);
            else Console.WriteLine($"WARN: {message}");
        }

        public static void Error(string message)
        {
            if (log?.IsErrorEnabled ?? false) log.Error(message);
            else Console.WriteLine($"ERROR: {message}");
        }

        public static void Error(string message, Exception ex)
        {
            if (log?.IsErrorEnabled ?? false) log.Error(message, ex);
            else Console.WriteLine($"ERROR: {message} - {ex}");
        }

        public static void Error(Exception ex)
        {
            if (log?.IsErrorEnabled ?? false) log.Error(ex.Message, ex);
            else Console.WriteLine($"ERROR: {ex}");
        }
    }
}
