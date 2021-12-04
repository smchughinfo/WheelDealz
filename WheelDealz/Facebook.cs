using OpenQA.Selenium;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using WheelDealz.ExtensionMethods;

namespace WheelDealz
{
    public static class Facebook
    {
        public static List<string> GetFacebookUrls()
        {
            Program.Driver.Navigate().GoToUrl($"https://www.facebook.com/marketplace/category/vehicles?minPrice={Program.MinPrice}&maxPrice={Program.MaxPrice}&minYear={Program.MaxPrice}&exact=false");
            Thread.Sleep(Program.LongWaitTime);

            SetDistance();

            var vehicles = Program.Driver.FindElements(By.CssSelector("[href^='/marketplace/item/']"));
            return vehicles.Select(v => v.GetAttribute("href")).ToList();
        }

        private static void SetDistance()
        {
            Program.Driver.FindElement(By.XPath("//*[text()='Hamilton, Ohio']")).Click();
            Thread.Sleep(Program.ShortWaitTime);

            Program.Driver.FindElement(By.CssSelector("body")).SendKeys(Keys.Tab, 3);
            Program.Driver.FindElement(By.CssSelector(":focus")).SendKeys(Keys.Down);
            Thread.Sleep(Program.ShortWaitTime);
            Program.Driver.FindElement(By.XPath($"//*[text()='{Program.DistanceFromMe} ']")).Click();
            Thread.Sleep(Program.ShortWaitTime);
            Program.Driver.FindElement(By.CssSelector("body")).SendKeys(Keys.Tab, 3);
            Program.Driver.FindElement(By.CssSelector(":focus")).SendKeys(Keys.Enter);
            Thread.Sleep(Program.LongWaitTime);
        }

        public static Car ScrapeFacebookPage(string url)
        {
            Program.Driver.Navigate().GoToUrl(url);
            Thread.Sleep(Program.LongWaitTime);

            var priceText = Program.Driver.FindElement(By.XPath("//*[starts-with(.,'$')]")); // POSSIBLY USEFUL. IF YOU DO GETPARENT THREE TIMES HERE ALL THE INFORMATION BELOW (EXCEPT IMAGES) IS IN THE STRING THAT'S RETURNED. I DID THIS LAST SO I FIGURED IT OUT AFTER THAT CODE WAS DONE AND IM NOT CHANGING IT. 
            var price = Convert.ToDouble(priceText.Text.Replace("$", ""));
            var makeModelAndYear = priceText.GetParent().Text.Split('\n')[0];
            var year = makeModelAndYear.Split(' ')[0];
            var makeModel = string.Join(" ", makeModelAndYear.Split(' ').Skip(1)).Replace(@"\r", "");

            string sellersDescription = null;
            try
            {
                var seeMoreButton = Program.Driver.FindElement(By.XPath("//*[text()='See more']"));
                seeMoreButton.Click();
                Thread.Sleep(Program.ShortWaitTime);
                sellersDescription = seeMoreButton.GetParent().GetParent().Text;
            }
            catch { }

            string aboutText = null;
            int milesDriven = -1;
            try
            {
                aboutText = Program.Driver.FindElement(By.XPath("//*[starts-with(.,'Driven')]")).Text;
                milesDriven = Convert.ToInt32(aboutText.Split('\r')[0].Replace("Driven", "").Replace("miles", "").Replace(",", "").Trim());
            }
            catch
            { }

            var allImages = Program.Driver.FindElements(By.CssSelector("img[src^='https://scontent.fluk1-1.fna.fbcdn.net']")).Select(i => i.GetAttribute("src")).ToList();
            var thisVehiclesImagePathRoot = string.Join("/", allImages[0].Split('/').Take(5)); // theres other images on the page that start with this part of the url. first image happens to be image of the car we want. so use that to get the part of the url that only matches images of the car we want.
            var vehicleImages = allImages.Where(i => i.Contains(thisVehiclesImagePathRoot)).Distinct().ToList();

            return new Car
            {
                Url = url,
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
