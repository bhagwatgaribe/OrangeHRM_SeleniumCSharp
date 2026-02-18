using NUnit.Framework;
using NUnit.Framework.Interfaces;
using OrangeHRM_SeleniumCSharp.com.orangeHRM.qa.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrangeHRM_SeleniumCSharp.com.orangeHRM.qa.Base
{
    public class BaseClass
    {
        // Use the centralized Logger utility for logging

        [SetUp]
        public void SetUp()
        {
            // Initialize the driver and log the operation
            Drivers.DriverFactory.InitializeDriver();
            Logger.Info("Driver initialized successfully.");

            // Create an Extent test for the current test
            try
            {
                var testName = TestContext.CurrentContext.Test.Name;
                ExtentReportManager.CreateTest(testName);
                ExtentReportManager.LogPass($"Starting test: {testName}");
            }
            catch (Exception ex)
            {
                Logger.Error("Error creating extent test: " + ex.Message, ex);
            }
        }

        [TearDown]
        public void TearDown()
        {
            try
            {
                var status = TestContext.CurrentContext.Result.Outcome.Status;
                var testName = TestContext.CurrentContext.Test.Name;

                if (status == TestStatus.Failed)
                {
                    var path = ScreenshotHelper.CaptureScreenshot(testName);
                    if (!string.IsNullOrEmpty(path))
                    {
                        Logger.Error($"Test '{testName}' failed. Screenshot saved to: {path}");
                        ExtentReportManager.AddScreenCapture(path);
                    }
                    else
                    {
                        Logger.Error($"Test '{testName}' failed. Screenshot could not be captured.");
                    }

                    var message = TestContext.CurrentContext.Result.Message;
                    ExtentReportManager.LogFail($"{testName} - {message}");
                }
                else if (status == TestStatus.Passed)
                {
                    ExtentReportManager.LogPass($"{testName} passed.");
                }
                else
                {
                    ExtentReportManager.LogFail($"{testName} - status: {status}");
                }
            }
            catch (Exception ex)
            {
                Logger.Error("Error during TearDown: " + ex.Message, ex);
            }
            finally
            {
                // Ensure driver is always quit to clean up resources
                Drivers.DriverFactory.QuitDriver();
                Logger.Info("Driver quit successfully.");
            }
        }

        [OneTimeTearDown]
        public void OneTimeTearDown()
        {
            // Flush and open the report when all tests in the fixture are done
            try
            {
                ExtentReportManager.Flush();
            }
            catch (Exception ex)
            {
                Logger.Error("Error flushing Extent report: " + ex.Message, ex);
            }
        }
    }
}
