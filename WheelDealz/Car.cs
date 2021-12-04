using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WheelDealz
{
    public class Car
    {
        public int Year;
        public string MakeModel;
        public double Price;
        public int Mileage;
        public string Description1;
        public string Description2;
        public List<string> ImageUrls;

        public override string ToString()
        {
            var result = "";
            result += $"{Year}{Environment.NewLine}";
            result += $"{MakeModel}{Environment.NewLine}";
            result += $"{Price}{Environment.NewLine}";
            result += $"{Mileage}{Environment.NewLine}";
            result += $"{Description1}{Environment.NewLine}";
            result += $"{Description2}{Environment.NewLine}";
            result += $"{string.Join(" HEY THIS IS A NEW URL ", ImageUrls)}{Environment.NewLine}";
            return result;
        }
    }
}
