using System.Globalization;
using System.Windows;
using System.Windows.Data;
using static AssetManager.AssetHelpers.AssetHelpers;

namespace AssetManager.Converters
{
    public class ExtenstionToVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {

            if (value.ToString() == AssetType.Model.ToString())
            {
                return Visibility.Visible;
            }
            else
            {
                return Visibility.Collapsed;
            }

        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
