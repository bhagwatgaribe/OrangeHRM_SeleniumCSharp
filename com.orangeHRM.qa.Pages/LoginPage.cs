using OpenQA.Selenium;
using OrangeHRM_SeleniumCSharp.Drivers;
using OrangeHRM_SeleniumCSharp.com.orangeHRM.qa.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrangeHRM_SeleniumCSharp.com.orangeHRM.qa.Pages
{
    public class LoginPage
    {
        private IWebDriver driver;

        public LoginPage(IWebDriver driver)
        {
            this.driver = driver;
        }

        private By txtUserName = By.Name("username");
        private By txtPassword = By.Name("password");
        private By btnLogin = By.XPath("//button[@type='submit']");

        public void EnterUserName(string username)
        {
            var el = WaitHelper.WaitForElementVisible(driver, txtUserName);
            el?.Clear();
            el?.SendKeys(username);
        }

        public void EnterPassword(string password)
        {
            var el = WaitHelper.WaitForElementVisible(driver, txtPassword);
            el?.Clear();
            el?.SendKeys(password);
        }

        public void Login()
        {
            var el = WaitHelper.WaitForElementClickable(driver, btnLogin);
            el?.Click();
        }
    }
}
