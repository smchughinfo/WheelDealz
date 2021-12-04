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
        static int maxCarsToScrape = 2;
        static void Main(string[] args)
        {
            cars.AddRange(GetFacebookCars());
            cars.AddRange(GetCraigslistCars());

            SaveCarListToDisk();
        }

        static List<Car> GetFacebookCars()
        {
            var cars = new List<Car>();
            var facebookUrls = Facebook.GetFacebookUrls().Take(maxCarsToScrape);
            foreach (var url in facebookUrls)
            {
                var car = Facebook.ScrapeFacebookPage(url);
                cars.Add(car);
            }
            return cars;
        }

        static List<Car> GetCraigslistCars()
        {
            var cars = new List<Car>();
            var craigslistUrls = Craigslist.GetCraigslistUrls().Take(maxCarsToScrape);
            foreach(var url in craigslistUrls)
            {
                var car = Craigslist.ScrapeCraigslistPage(url);
            }
            return cars;
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
