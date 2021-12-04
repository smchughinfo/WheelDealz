using OpenQA.Selenium;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace WheelDealz.ExtensionMethods
{
    public static class IWebElementExtensions
    {
        public static IWebElement GetParent(this IWebElement webElement)
        {
            return webElement.FindElement(By.XPath("./.."));
        }

        public static void SendKeys(this IWebElement webElement, string key, int numberOfTimes)
        {
            for(int i = 0; i < numberOfTimes; i++)
            {
                webElement.SendKeys(key);
                Thread.Sleep(Program.ShortWaitTime);
            }
        }
    }
}
