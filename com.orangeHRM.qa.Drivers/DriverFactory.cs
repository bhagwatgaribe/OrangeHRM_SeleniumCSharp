using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Firefox;
using OpenQA.Selenium.Edge;
using OrangeHRM_SeleniumCSharp.com.orangeHRM.qa.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrangeHRM_SeleniumCSharp.Drivers
{
    public static class DriverFactory
    {
        // ThreadLocal to ensure thread safety when running tests in parallel
        private static ThreadLocal<IWebDriver> driver = new ThreadLocal<IWebDriver>();

        public static IWebDriver GetDriver()
        {
            if (driver.Value == null)
            {
                throw new InvalidOperationException("WebDriver instance has not been initialized. Call InitializeDriver() first.");
            }
            return driver.Value;
        }

        // Returns the current driver instance or null if it hasn't been initialized.
        public static IWebDriver? GetDriverOrNull()
        {
            return driver.Value;
        }

        // Convenience check to see if a driver is present for the current thread.
        public static bool IsDriverInitialized()
        {
            return driver.Value != null;
        }

        public static void InitializeDriver()
        {
            // Read desired browser from configuration (defaults to Chrome)
            var browser = ConfigReader.GetConfigValue("Browser")?.Trim() ?? "Chrome";

            Logger.Info($"Initializing WebDriver for browser: {browser}");

            switch (browser.ToLowerInvariant())
            {
                case "chrome":
                    var chromeOptions = new ChromeOptions();
                    // add any default Chrome options here if needed
                    driver.Value = new ChromeDriver(chromeOptions);
                    break;
                case "firefox":
                    var firefoxOptions = new FirefoxOptions();
                    driver.Value = new FirefoxDriver(firefoxOptions);
                    break;
                case "edge":
                    var edgeOptions = new EdgeOptions();
                    driver.Value = new EdgeDriver(edgeOptions);
                    break;
                default:
                    // Fallback to Chrome if unknown value is provided
                    Logger.Warn($"Unknown browser '{browser}' specified in configuration. Falling back to Chrome.");
                    driver.Value = new ChromeDriver();
                    break;
            }

            driver.Value.Manage().Window.Maximize();
            var baseUrl = ConfigReader.GetConfigValue("BaseUrl");
            if (!string.IsNullOrEmpty(baseUrl))
            {
                driver.Value.Navigate().GoToUrl(baseUrl);
                Logger.Info($"Navigated to base URL: {baseUrl}");
            }
            else
            {
                Logger.Warn("No BaseUrl specified in configuration.");
            }
        }

        public static void QuitDriver()
        {
            if (driver.Value != null)
            {
                try
                {
                    Logger.Info("Quitting WebDriver instance.");
                    driver.Value.Quit();
                    driver.Value.Dispose();
                }
                catch (Exception ex)
                {
                    Logger.Error("Error while quitting WebDriver.", ex);
                }
                finally
                {
                    driver.Value = null; // Clear the ThreadLocal value to prevent memory leaks
                }
            }
            else
            {
                Logger.Debug("QuitDriver called but no WebDriver instance was present.");
            }
        }
    }
}
