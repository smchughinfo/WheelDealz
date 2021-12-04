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
        static IWebDriver driver = new ChromeDriver();
        static int shortWaitTime = 500;
        static int longWaitTime = 5000;
        static List<Car> cars = new List<Car>();
        static void Main(string[] args)
        {
            var facebookUrls = GetFacebookUrls().Take(3);
            foreach(var url in facebookUrls)
            {
                var car = ScrapeFacebookPage(url);

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

        static List<string> GetFacebookUrls()
        {
            driver.Navigate().GoToUrl("https://www.facebook.com/marketplace/category/vehicles?minPrice=500&maxPrice=1500&minYear=2000&exact=false");
            Thread.Sleep(longWaitTime);
            var vehicles = driver.FindElements(By.CssSelector("[href^='/marketplace/item/']"));

            return vehicles.Select(v => v.GetAttribute("href")).ToList();
        }

        static Car ScrapeFacebookPage(string url)
        {
            driver.Navigate().GoToUrl(url);
            Thread.Sleep(longWaitTime);

            var priceText = driver.FindElement(By.XPath("//*[starts-with(.,'$')]")); // POSSIBLY USEFUL. IF YOU DO GETPARENT THREE TIMES HERE ALL THE INFORMATION BELOW (EXCEPT IMAGES) IS IN THE STRING THAT'S RETURNED. I DID THIS LAST SO I FIGURED IT OUT AFTER THAT CODE WAS DONE AND IM NOT CHANGING IT. 
            var price = Convert.ToDouble(priceText.Text.Replace("$", ""));
            var makeModelAndYear = priceText.GetParent().Text.Split('\n')[0];
            var year = makeModelAndYear.Split(' ')[0];
            var makeModel = string.Join(" ", makeModelAndYear.Split(' ').Skip(1)).Replace(@"\r", "");

            string sellersDescription = null;
            try
            {
                var seeMoreButton = driver.FindElement(By.XPath("//*[text()='See more']"));
                seeMoreButton.Click();
                Thread.Sleep(shortWaitTime);
                sellersDescription = seeMoreButton.GetParent().GetParent().Text;
            }
            catch { }

            string aboutText = null;
            int milesDriven = -1;
            try
            {
                aboutText = driver.FindElement(By.XPath("//*[starts-with(.,'Driven')]")).Text;
                milesDriven = Convert.ToInt32(aboutText.Split('\r')[0].Replace("Driven", "").Replace("miles", "").Replace(",", "").Trim());
            }
            catch 
            { }

            var allImages = driver.FindElements(By.CssSelector("img[src^='https://scontent.fluk1-1.fna.fbcdn.net']")).Select(i => i.GetAttribute("src")).ToList();
            var thisVehiclesImagePathRoot = string.Join("/", allImages[0].Split('/').Take(5)); // theres others images on the page that start with this part of the url. first image happens to be image of the car we want. so use that to get the part of the url that only matches images of the car we want.
            var vehicleImages = allImages.Where(i => i.Contains(thisVehiclesImagePathRoot)).Distinct().ToList();

            return new Car
            {
                Year = Convert.ToInt32(year),
                Price = price,
                MakeModel = makeModel,
                Mileage = Convert.ToInt32(milesDriven),
                Description1 = sellersDescription,
                Description2 = aboutText,
                ImageUrls = vehicleImages
            };
        }
    }
}
