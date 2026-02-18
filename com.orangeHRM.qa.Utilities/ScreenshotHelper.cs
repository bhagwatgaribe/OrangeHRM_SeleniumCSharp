using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenQA.Selenium;

namespace OrangeHRM_SeleniumCSharp.com.orangeHRM.qa.Utilities
{
    public static class ScreenshotHelper
    {
        public static string CaptureScreenshot(string testName)
        {
            try
            {
                var driver = Drivers.DriverFactory.GetDriverOrNull();
                if (driver == null)
                {
                    // If no WebDriver is present, create a small placeholder PNG so
                    // downstream reporting (Extent reports) can still attach a file.
                    Logger.Warn($"Cannot capture screenshot for test '{testName}': WebDriver has not been initialized. Creating placeholder image.");

                    var placeholderTimestamp = DateTime.Now.ToString("yyyyMMdd_HHmmss");
                    var placeholderFileName = $"{testName}_{placeholderTimestamp}_nodriver.png";
                    var placeholderFilePath = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Screenshots", placeholderFileName);

                    try
                    {
                        var placeholderDir = System.IO.Path.GetDirectoryName(placeholderFilePath);
                        if (!string.IsNullOrEmpty(placeholderDir))
                        {
                            System.IO.Directory.CreateDirectory(placeholderDir);
                        }

                        // 1x1 transparent PNG (base64 encoded)
                        var pngBase64 = "iVBORw0KGgoAAAANSUhEUgAAAAEAAAABCAQAAAC1HAwCAAAAC0lEQVR4nGNgYAAAAAMAASsJTYQAAAAASUVORK5CYII=";
                        var placeholderBytes = Convert.FromBase64String(pngBase64);
                        System.IO.File.WriteAllBytes(placeholderFilePath, placeholderBytes);

                        Logger.Info($"Placeholder screenshot created for test '{testName}' at path: {placeholderFilePath}");
                        return placeholderFilePath;
                    }
                    catch (Exception ex)
                    {
                        Logger.Error($"Error creating placeholder screenshot for test '{testName}': {ex.Message}", ex);
                        return null;
                    }
                }

                var screenshot = ((OpenQA.Selenium.ITakesScreenshot)driver).GetScreenshot();
                var timestamp = DateTime.Now.ToString("yyyyMMdd_HHmmss");
                var fileName = $"{testName}_{timestamp}.png";
                var filePath = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Screenshots", fileName);

                // Ensure the Screenshots directory exists
                var dir = System.IO.Path.GetDirectoryName(filePath);
                if (!string.IsNullOrEmpty(dir))
                {
                    System.IO.Directory.CreateDirectory(dir);
                }

                // Save screenshot by writing the raw PNG bytes to file (works regardless of Selenium enum availability)
                var bytes = screenshot.AsByteArray;
                System.IO.File.WriteAllBytes(filePath, bytes);
                Logger.Info($"Screenshot captured for test '{testName}' at path: {filePath}");
                return filePath;
            }
            catch (Exception ex)
            {
                // Log the exception using the new Logger utility
                Logger.Error($"Error capturing screenshot for test '{testName}': {ex.Message}", ex);
                return null;
            }
        }
    }
}
