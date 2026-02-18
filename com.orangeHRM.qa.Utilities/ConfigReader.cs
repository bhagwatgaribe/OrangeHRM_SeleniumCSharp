using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrangeHRM_SeleniumCSharp.com.orangeHRM.qa.Utilities
{
    public static class ConfigReader
    {
        private static IConfiguration configuration;

        static ConfigReader()
        {
            configuration = new ConfigurationBuilder()
                .SetBasePath(AppDomain.CurrentDomain.BaseDirectory)
                .AddJsonFile("Config/appsettings.json", optional: false)
                .Build();
        }

        public static string GetConfigValue(string key)
        {
            // Retrieve the value and provide a non-null fallback to satisfy the non-nullable return type.
            // If you'd rather fail when a key is missing, replace the fallback with:
            // throw new InvalidOperationException($"Configuration key '{key}' not found.");
            var value = configuration[key];
            return value ?? string.Empty;
        }
    }
}
