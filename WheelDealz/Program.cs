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
        public static IWebDriver Driver = new ChromeDriver();
        public static int ShortWaitTime = 500;
        public static int LongWaitTime = 5000;

        public static int MinYear = 2000;
        public static int MinPrice = 500;
        public static int MaxPrice = 2000;
        public static int DistanceFromMe = 20; // can only use the numbers on facebooks distance chooser

        static List<Car> Cars = new List<Car>();
        static int MaxCarsToScrape = 2;
        static void Main(string[] args)
        {
            Cars.AddRange(GetFacebookCars());
            Cars.AddRange(GetCraigslistCars());

            SaveCarListToDisk();
        }

        static List<Car> GetFacebookCars()
        {
            var cars = new List<Car>();
            var facebookUrls = Facebook.GetFacebookUrls().Take(MaxCarsToScrape);
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
            var craigslistUrls = Craigslist.GetCraigslistUrls().Take(MaxCarsToScrape);
            foreach(var url in craigslistUrls)
            {
                var car = Craigslist.ScrapeCraigslistPage(url);
            }
            return cars;
        }

        static void SaveCarListToDisk()
        {
            var result = "";
            foreach (var car in Cars)
            {
                result += car.ToString() + Environment.NewLine + "<><><><><><><><><><><><><><><><><><><><><><><><><><>" + Environment.NewLine;
            }
            File.WriteAllText(@"c:\users\sweetrelish\desktop\idk.txt", result);
        }
    }
}
