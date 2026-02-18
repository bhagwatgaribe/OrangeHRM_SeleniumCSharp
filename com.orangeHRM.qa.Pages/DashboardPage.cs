using OpenQA.Selenium;
using OrangeHRM_SeleniumCSharp.com.orangeHRM.qa.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrangeHRM_SeleniumCSharp.com.orangeHRM.qa.Pages
{
    public  class DashboardPage
    {
        private IWebDriver driver;

        public DashboardPage(IWebDriver driver) {
            this.driver = driver;
        }

        private By dashboardHeader = By.XPath("//span[@class='oxd-topbar-header-breadcrumb']//h6");

        public string getDashboardPageHeader()
        {
            // Wait for the header element to be visible and return its text
            var el = WaitHelper.WaitForElementVisible(driver, dashboardHeader);
            return el?.Text?.Trim() ?? string.Empty;
        }
    }
}
