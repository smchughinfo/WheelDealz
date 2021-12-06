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
using System.Net.Http;
using System.Net;
using Newtonsoft.Json;
using System.Diagnostics;
using System.Runtime.InteropServices;

namespace WheelDealz
{
    class Program
    {
        public static IWebDriver Driver;
        public static int ShortWaitTime = 500;
        public static int LongWaitTime = 5000;

        public static int MinYear = 2000;
        public static int MinPrice = 500;
        public static int MaxPrice = 2000;
        public static int DistanceFromMe = 20; // can only use the numbers on facebooks distance chooser

        public static int PollPeriod = 1000 * 60 * 60; // every hour

        static List<Car> Cars = new List<Car>();
        static int MaxCarsToScrapePerSite = Int32.MaxValue;
        
        static string dataDir = @"C:\Users\sweetrelish\Desktop\smchughinfo.github.io\WheelDealz\";
        static string dataFileName = "data.txt";
        static string logFileName = "log.txt";
        static string jsonFileName = "data.js";
        static string dataFilePath = Path.Combine(dataDir, dataFileName);
        static string logFilePath = Path.Combine(dataDir, logFileName);
        static string jsonFilePath = Path.Combine(dataDir, jsonFileName);

        static string stringifiedCarDelimeter = Environment.NewLine + Environment.NewLine + Environment.NewLine + Environment.NewLine + Environment.NewLine + Environment.NewLine + "EEA7B7D1-011A-4117-90CC-142FA482C8E1" + Environment.NewLine + Environment.NewLine + Environment.NewLine + Environment.NewLine + Environment.NewLine + Environment.NewLine;

        [DllImport("user32.dll")]
        public static extern bool ShowWindow(IntPtr hWnd, int nCmdShow);
        public const int SW_SHOWMINIMIZED = 2;
        static IntPtr winHandle = System.Diagnostics.Process.GetCurrentProcess().MainWindowHandle;
        
        static void Main(string[] args)
        {
            ShowWindow(winHandle, SW_SHOWMINIMIZED);

            while (true)
            {
                Driver = new ChromeDriver();
                Driver.Manage().Window.Minimize();
                Thread.Sleep(LongWaitTime);
                Scrape();
                Driver.Quit();
                Thread.Sleep(PollPeriod);
            }
            
        }

        static void Scrape()
        {
            LoadCarListFromDisk();

            Cars.AddRange(GetCars(Facebook.GetFacebookUrls, Facebook.ScrapeFacebookPage));
            Cars.AddRange(GetCars(Craigslist.GetCraigslistUrls, Craigslist.ScrapeCraigslistPage));

            SaveCarListToDisk();
            ExportJsonAndUpload();
        }

        static List<Car> GetCars(Func<List<string>> getCarUrls, Func<string, Car> getCarFromUrl)
        {
            var cars = new List<Car>();

            List<string> newUrls = new List<string>();
            try
            {
                newUrls = getCarUrls();
            }
            catch (Exception ex)
            {
                File.AppendAllText(logFilePath, $"ERROR GETTING THE URLS - {ex.Message}{Environment.NewLine}{Environment.NewLine}{Environment.NewLine}");
                return cars;
            }

            var existingUrls = Cars.Select(c => c.Url).ToList();
            existingUrls.AddRange(GetLogFileErrorUrls());
            var urlsToScrape = newUrls.Where(newUrl => existingUrls.Contains(newUrl) == false).ToList();

            var maxCarsToScrape = Math.Min(urlsToScrape.Count, MaxCarsToScrapePerSite);

            for(var i = 0; i < maxCarsToScrape; i++)
            {
                var url = urlsToScrape[i];

                try
                {
                    var car = getCarFromUrl(url);
                    cars.Add(car);
                }
                catch (Exception ex)
                {
                    var isReallyAnError = !CanIgnoreScrapeError(url);
                    if (isReallyAnError)
                    {
                        File.AppendAllText(logFilePath, $"ERROR GETTING A CAR - {url}{Environment.NewLine}{ex.Message}{Environment.NewLine}{ex.StackTrace}{Environment.NewLine}{Environment.NewLine}");
                    }
                }
            }

            return cars;
        }

        static bool CanIgnoreScrapeError(string url)
        {
            var canIgnoreStrings = new List<string>()
            {
                "404 Error" // found on criaglist. 
            };

            try
            {
                var html = (new WebClient()).DownloadString(url);
                return canIgnoreStrings.Any(s => html.Contains(s));
            }
            catch // this can happen if 404. ...probably other things too.
            {
                return false; 
            }
        }

        static void LoadCarListFromDisk()
        {
            if(!File.Exists(dataFilePath))
            {
                return;
            }

            var stringifiedCars = File.ReadAllText(dataFilePath).Split(new string[] { Program.stringifiedCarDelimeter }, StringSplitOptions.None);
            stringifiedCars = stringifiedCars.Where(c => !string.IsNullOrWhiteSpace(c)).ToArray();
            foreach (var stringifiedCar in stringifiedCars)
            {
                Cars.Add(new Car(stringifiedCar));
            }
        }

        static void SaveCarListToDisk()
        {
            var result = "";
            foreach (var car in Cars)
            {
                result += car.ToString() + stringifiedCarDelimeter;
            }
            
            File.WriteAllText(dataFilePath, result);
        }

        static List<string> GetLogFileErrorUrls()
        {
            var regexPattern = "https://.*?$";
            var lines = File.ReadAllLines(logFilePath).ToList();
            lines = lines.Where(l => Regex.IsMatch(l, regexPattern)).ToList();
            lines = lines.Select(l => Regex.Match(l, regexPattern).Value).ToList();

            return lines;
        }

        static void ExportJsonAndUpload()
        {
            var json = JsonConvert.SerializeObject(Cars);
            File.WriteAllText(jsonFilePath, $"var carData={json}");

            Process.Start("git-upload-script.bat");
        }
    }
}
