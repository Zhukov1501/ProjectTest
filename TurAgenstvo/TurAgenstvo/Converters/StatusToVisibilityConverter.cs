using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace TurAgenstvo.Converters  // ВАЖНО: TurAgenstvo (с большой Т и А), а не TurAgentstvo
{
    public class StatusToVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null || parameter == null)
                return Visibility.Collapsed;

            string status = value.ToString();
            string targetStatus = parameter.ToString();

            return status == targetStatus ? Visibility.Visible : Visibility.Collapsed;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}