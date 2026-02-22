using NUnit.Framework;
using OrangeHRM_SeleniumCSharp.com.orangeHRM.qa.Base;
using OrangeHRM_SeleniumCSharp.com.orangeHRM.qa.Utilities;
using OrangeHRM_SeleniumCSharp.com.orangeHRM.qa.Pages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrangeHRM_SeleniumCSharp.com.orangeHRM.qa.TestCases
{
    [TestFixture]
    public class LoginPageTest : BaseClass
    {
        [Test]
        public void TC_001_VerifyLoginTestWithValidData()
        {
            Logger.Info("Starting TC_001_VerifyLoginTestWithValidData");
            try
            {
                var username = ConfigReader.GetConfigValue("Credentials:Username");
                var password = ConfigReader.GetConfigValue("Credentials:Password");

                Logger.Info($"Using username from config: {username}");
                Logger.Info($"Using password from config: {password}");

                // Get the initialized driver from the factory (initialized in BaseClass.SetUp)
                var driver = Drivers.DriverFactory.GetDriver();

                var loginPage = new LoginPage(driver);
                loginPage.EnterUserName(username);
                loginPage.EnterPassword(password);
                loginPage.Login();

                var dashboardPage = new DashboardPage(driver);
                var actualHeader = dashboardPage.getDashboardPageHeader();
                var expectedHeader = "Dashboard";

                // Use NUnit constraint model per NUnit2005 diagnostic
                Assert.That(actualHeader, Is.EqualTo(expectedHeader));

                Logger.Info("Assertion passed – Dashboard header verified");
            }
            catch (Exception ex)
            {
                Logger.Error("Test failed due to exception");
                Logger.Error(ex.Message);
                Logger.Error(ex);

                // Capture screenshot on failure
                ScreenshotHelper.CaptureScreenshot(
                    "TC_001_VerifyLoginTestWithValidData");

                // IMPORTANT: rethrow so NUnit marks test as FAILED
                throw;
            }
            finally
            {
                Logger.Info("Ending TC_001_VerifyLoginTestWithValidData");
            }
        }
    }
}
