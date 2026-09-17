using System;

namespace LibControls
{
    public class Utils
    {
        public static double StringToDouble(string value)
        {
            value = value.Replace(",", ".");
            var culture = new System.Globalization.CultureInfo("es-EC");
            culture.NumberFormat.CurrencyDecimalSeparator = ".";
            culture.NumberFormat.NumberDecimalSeparator = ".";
            double number = Convert.ToSingle(value, culture);
            return number;
        }
    }
}
