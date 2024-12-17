using System;
using System.Collections.Generic;
using System.Globalization;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace AssetManager.Converters
{
    public class BooleanToContentConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is bool isEnabled)
            {
                return isEnabled ? "Enabled" : "Enable";
            }
            return string.Empty;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is bool isEnabled)
            {
                return isEnabled ? "Enabled" : "Enable";
            }
            return string.Empty;
        }
    }
}
