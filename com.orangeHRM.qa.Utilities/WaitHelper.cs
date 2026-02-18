using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;
using System;

namespace OrangeHRM_SeleniumCSharp.com.orangeHRM.qa.Utilities
{
    public static class WaitHelper
    {
        private static TimeSpan GetTimeout(int? seconds)
        {
            if (seconds.HasValue) return TimeSpan.FromSeconds(seconds.Value);

            var cfg = ConfigReader.GetConfigValue("DefaultTimeoutInSeconds");
            if (int.TryParse(cfg, out var s) && s > 0) return TimeSpan.FromSeconds(s);

            return TimeSpan.FromSeconds(30);
        }

        public static IWebElement WaitForElementExists(IWebDriver driver, By locator, int? timeoutSeconds = null)
        {
            var wait = new WebDriverWait(driver, GetTimeout(timeoutSeconds));
            try
            {
                return wait.Until(d =>
                {
                    try
                    {
                        return d.FindElement(locator);
                    }
                    catch (NoSuchElementException)
                    {
                        return null;
                    }
                });
            }
            catch (Exception ex)
            {
                Logger.Error($"Error waiting for element existence '{locator}': {ex.Message}", ex);
                throw;
            }
        }

        public static IWebElement WaitForElementVisible(IWebDriver driver, By locator, int? timeoutSeconds = null)
        {
            var wait = new WebDriverWait(driver, GetTimeout(timeoutSeconds));
            try
            {
                return wait.Until(d =>
                {
                    try
                    {
                        var el = d.FindElement(locator);
                        return el.Displayed ? el : null;
                    }
                    catch (NoSuchElementException)
                    {
                        return null;
                    }
                    catch (StaleElementReferenceException)
                    {
                        return null;
                    }
                });
            }
            catch (Exception ex)
            {
                Logger.Error($"Error waiting for element visible '{locator}': {ex.Message}", ex);
                throw;
            }
        }

        public static IWebElement WaitForElementClickable(IWebDriver driver, By locator, int? timeoutSeconds = null)
        {
            var wait = new WebDriverWait(driver, GetTimeout(timeoutSeconds));
            try
            {
                return wait.Until(d =>
                {
                    try
                    {
                        var el = d.FindElement(locator);
                        return (el.Displayed && el.Enabled) ? el : null;
                    }
                    catch (NoSuchElementException)
                    {
                        return null;
                    }
                    catch (StaleElementReferenceException)
                    {
                        return null;
                    }
                });
            }
            catch (Exception ex)
            {
                Logger.Error($"Error waiting for element clickable '{locator}': {ex.Message}", ex);
                throw;
            }
        }

        public static bool WaitForElementInvisible(IWebDriver driver, By locator, int? timeoutSeconds = null)
        {
            var wait = new WebDriverWait(driver, GetTimeout(timeoutSeconds));
            try
            {
                return wait.Until(d =>
                {
                    try
                    {
                        var el = d.FindElement(locator);
                        return !el.Displayed;
                    }
                    catch (NoSuchElementException)
                    {
                        return true;
                    }
                    catch (StaleElementReferenceException)
                    {
                        return true;
                    }
                });
            }
            catch (Exception ex)
            {
                Logger.Error($"Error waiting for element invisible '{locator}': {ex.Message}", ex);
                throw;
            }
        }

        public static void WaitForPageLoad(IWebDriver driver, int? timeoutSeconds = null)
        {
            var wait = new WebDriverWait(driver, GetTimeout(timeoutSeconds));
            try
            {
                wait.Until(d =>
                {
                    try
                    {
                        var js = d as IJavaScriptExecutor;
                        var ready = js.ExecuteScript("return document.readyState") as string;
                        return string.Equals(ready, "complete", StringComparison.OrdinalIgnoreCase);
                    }
                    catch (Exception)
                    {
                        return false;
                    }
                });
            }
            catch (Exception ex)
            {
                Logger.Error($"Error waiting for page load: {ex.Message}", ex);
                throw;
            }
        }
    }
}
