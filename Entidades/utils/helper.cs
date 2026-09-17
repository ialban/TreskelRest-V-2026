using System;

namespace Entidades.Utils
{
    public class Helper
    {
        public static double StringToDouble(string value)
        {
            value = value ?? "0";
            value = value.Replace(",", ".").Trim();
            value = value == "" ? "0" : value;
            var culture = new System.Globalization.CultureInfo("es-EC");
            culture.NumberFormat.CurrencyDecimalSeparator = ".";
            culture.NumberFormat.NumberDecimalSeparator = ".";
            double number = Convert.ToSingle(value, culture);
            return number;
        }
    }
}
