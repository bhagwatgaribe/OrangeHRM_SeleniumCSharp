using log4net;
using log4net.Config;
using log4net.Repository.Hierarchy;
using System;
using System.IO;

namespace OrangeHRM_SeleniumCSharp.com.orangeHRM.qa.Utilities
{
    public static class Logger
    {
        private static readonly ILog log;

        static Logger()
        {
            try
            {
                var projectRoot = ResolveProjectRoot();
                var logsDir = Path.Combine(projectRoot, "Logs");
                Directory.CreateDirectory(logsDir);

                var configPath = Path.Combine(projectRoot, "Config", "log4net.config");

                if (!File.Exists(configPath))
                    throw new FileNotFoundException("log4net.config not found", configPath);

                // 🔥 Load config
                XmlConfigurator.Configure(new FileInfo(configPath));

                // 🔥 FORCE absolute path for file appenders
                var hierarchy = (Hierarchy)LogManager.GetRepository();
                foreach (var appender in hierarchy.Root.Appenders)
                {
                    if (appender is log4net.Appender.RollingFileAppender rolling)
                    {
                        rolling.File = Path.Combine(logsDir, "automation.log");
                        rolling.ActivateOptions(); // VERY IMPORTANT
                    }
                }
            }
            catch (Exception ex)
            {
                BasicConfigurator.Configure();
                Console.WriteLine("Logger init failed: " + ex);
            }

            log = LogManager.GetLogger(typeof(Logger));
        }

        private static string ResolveProjectRoot()
        {
            var dir = new DirectoryInfo(AppDomain.CurrentDomain.BaseDirectory);

            while (dir != null && !Directory.Exists(Path.Combine(dir.FullName, "Config")))
            {
                dir = dir.Parent;
            }

            return dir?.FullName ?? Directory.GetCurrentDirectory();
        }

        public static void Info(string message) => log.Info(message);
        public static void Debug(string message) => log.Debug(message);
        public static void Warn(string message) => log.Warn(message);
        public static void Error(string message) => log.Error(message);
        public static void Error(string message, Exception ex) => log.Error(message, ex);
        public static void Error(Exception ex) => log.Error(ex.Message, ex);
    }
}
