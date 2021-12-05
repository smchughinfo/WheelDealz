using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WheelDealz
{
    public class Car
    {
        private string classDelimeter = Environment.NewLine + "--------------------13859161-31B4-4AEA-89FD-CDC34217B63D--------------------" + Environment.NewLine;
        private string arrayDelimeter = Environment.NewLine + "--------------------11D3E2E1-17C5-4EF4-9A8D-09A778BCCD60--------------------" + Environment.NewLine;

        public string Url;
        public int Year;
        public string MakeModel;
        public double Price;
        public int Mileage;
        public string Description1;
        public string Description2;
        public List<string> ImageUrls;
        public DateTime ScrapeDate;

        public Car()
        {

        }

        public Car(string stringified)
        {
            var parms = stringified.Split(new string[] { classDelimeter }, StringSplitOptions.None);
            Url = parms[0];
            Year = string.IsNullOrWhiteSpace(parms[1]) ? -1 : Convert.ToInt32(parms[1]);
            MakeModel = string.IsNullOrWhiteSpace(parms[2]) ? "" : parms[2];
            Price = string.IsNullOrWhiteSpace(parms[3]) ? -1 : Convert.ToDouble(parms[3]);
            Mileage = string.IsNullOrWhiteSpace(parms[4]) ? -1 : Convert.ToInt32(parms[4]);
            Description1 = string.IsNullOrWhiteSpace(parms[5]) ? "" : parms[5];
            Description2 = string.IsNullOrWhiteSpace(parms[6]) ? "" : parms[6];
            ImageUrls = string.IsNullOrWhiteSpace(parms[7]) ? null : parms[7].Split(new string[] { arrayDelimeter }, StringSplitOptions.None).ToList();
            ScrapeDate = string.IsNullOrWhiteSpace(parms[8]) ? DateTime.MinValue : Convert.ToDateTime(parms[8]);
        }

        public override string ToString()
        {
            var result = "";
            result += $"{Url}{classDelimeter}";
            result += $"{Year}{classDelimeter}";
            result += $"{MakeModel}{classDelimeter}";
            result += $"{Price}{classDelimeter}";
            result += $"{Mileage}{classDelimeter}";
            result += $"{Description1}{classDelimeter}";
            result += $"{Description2}{classDelimeter}";
            result += $"{string.Join(arrayDelimeter, ImageUrls)}{classDelimeter}";
            result += $"{ScrapeDate.ToLongDateString()}{classDelimeter}";
            return result;
        }
    }
}
