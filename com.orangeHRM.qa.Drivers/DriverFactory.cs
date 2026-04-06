using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Edge;
using OpenQA.Selenium.Firefox;
using OrangeHRM_SeleniumCSharp.com.orangeHRM.qa.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using System.IO;
// Rely on Selenium Manager included with Selenium WebDriver to resolve driver binaries

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
            try
            {
                var browser = ConfigReader.GetConfigValue("Browser")?.Trim() ?? "Chrome";
                Logger.Info($"Initializing WebDriver for browser: {browser}");

                switch (browser.ToLowerInvariant())
                {
                    case "chrome":
                        // Use Selenium Manager via default ChromeDriver constructor to obtain matching driver
                        var chromeOptions = new ChromeOptions();
                        chromeOptions.AddArgument("--start-maximized");
                        driver.Value = new ChromeDriver(chromeOptions);
                        break;

                    case "firefox":
                        var firefoxOptions = new FirefoxOptions();
                        firefoxOptions.AddArgument("--start-maximized");
                        driver.Value = new FirefoxDriver(firefoxOptions);
                        break;

                    case "edge":
                        var edgeOptions = new EdgeOptions();
                        edgeOptions.AddArgument("--start-maximized");
                        driver.Value = new EdgeDriver(edgeOptions);
                        break;

                    default:
                        Logger.Warn($"Unknown browser '{browser}'. Falling back to Chrome.");
                        driver.Value = new ChromeDriver();
                        break;
                }

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
            catch (Exception ex)
            {
                Logger.Error("❌ WebDriver initialization FAILED", ex);
                throw; // VERY IMPORTANT – lets NUnit mark test as failed with real cause
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
