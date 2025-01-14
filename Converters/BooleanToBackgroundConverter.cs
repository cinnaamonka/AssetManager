using System;
using System.Collections.Generic;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace AssetManager.Converters
{
    public class BooleanToBackgroundConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is bool isEnabled)
            {
                return isEnabled ? System.Windows.Media.Brushes.Green : System.Windows.Media.Brushes.Transparent; // Change colors as needed
            }

            return Brushes.Transparent;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is bool isEnabled)
            {
                return isEnabled ? System.Windows.Media.Brushes.Green : System.Windows.Media.Brushes.Transparent; // Change colors as needed
            }
            return Brushes.Transparent;
        }
    }

}
