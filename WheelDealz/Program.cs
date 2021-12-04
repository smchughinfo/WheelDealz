using System;
using System.Linq;
using System.Collections.Generic;
using System.Threading;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Firefox;
using OpenQA.Selenium.Support.UI;
using WheelDealz.ExtensionMethods;
using System.Text.RegularExpressions;
using System.IO;

namespace WheelDealz
{
    class Program
    {
        public static IWebDriver driver = new ChromeDriver();
        public static int shortWaitTime = 500;
        public static int longWaitTime = 5000;
        static List<Car> cars = new List<Car>();
        static void Main(string[] args)
        {
            var facebookUrls = Facebook.GetFacebookUrls().Take(5);
            foreach(var url in facebookUrls)
            {
                var car = Facebook.ScrapeFacebookPage(url);

                var newCar = !cars.Any(c => c.ToString() == car.ToString());
                if(newCar)
                {
                    cars.Add(car);
                }
            }
            SaveCarListToDisk();
        }

        static void SaveCarListToDisk()
        {
            var result = "";
            foreach (var car in cars)
            {
                result += car.ToString() + Environment.NewLine + "<><><><><><><><><><><><><><><><><><><><><><><><><><>" + Environment.NewLine;
            }
            File.WriteAllText(@"c:\users\sweetrelish\desktop\idk.txt", result);
        }
    }
}
