using System;
using System.Globalization;
using System.Windows.Data;

namespace TurAgenstvo.Converters  // То же самое namespace
{
    public class PriceConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null)
                return "0 руб";

            if (value is decimal decimalPrice)
                return $"{decimalPrice:N0} руб";

            if (value is int intPrice)
                return $"{intPrice:N0} руб";

            if (value is double doublePrice)
                return $"{doublePrice:N0} руб";

            return "0 руб";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}