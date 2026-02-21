using System;
using System.Globalization;
using System.Windows.Data;

namespace Catering.Converters
{
    public class OrderItemTotalConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            try
            {
                if (values == null || values.Length < 2)
                    return 0m;

                // Quantity may be int or other numeric type
                int qty = 0;
                if (values[0] is int) qty = (int)values[0];
                else if (values[0] is long) qty = System.Convert.ToInt32(values[0]);
                else if (values[0] is decimal) qty = System.Convert.ToInt32(values[0]);
                else if (!int.TryParse(values[0]?.ToString() ?? "0", out qty)) qty = 0;

                decimal price = 0m;
                if (values[1] is decimal) price = (decimal)values[1];
                else if (values[1] is double) price = System.Convert.ToDecimal((double)values[1]);
                else if (values[1] is float) price = System.Convert.ToDecimal((float)values[1]);
                else if (!decimal.TryParse(values[1]?.ToString() ?? "0", NumberStyles.Any, culture, out price)) price = 0m;

                decimal total = qty * price;

                // If target expects string, return formatted string
                if (targetType == typeof(string))
                    return total.ToString("0.##", culture);

                return total;
            }
            catch
            {
                return 0m;
            }
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotSupportedException();
        }
    }
}
