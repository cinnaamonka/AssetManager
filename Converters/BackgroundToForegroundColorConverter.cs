using System.Globalization;
using System.Windows.Media;
using System.Windows.Data;

namespace AssetManager.Converters
{
    internal class BackgroundToForegroundColorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {

            SolidColorBrush backgroundBrush;

            if (value is string colorString)
            {
                try
                {
                    backgroundBrush = (SolidColorBrush)new BrushConverter().ConvertFromString(colorString);
                }
                catch
                {
                    // Default to Transparent if the color string is invalid
                    backgroundBrush = System.Windows.Media.Brushes.Transparent;
                }
            }
            else if (value is SolidColorBrush brush)
            {
                backgroundBrush = brush;
            }
            else
            {
                // Default to Transparent for unsupported types
                backgroundBrush = System.Windows.Media.Brushes.Transparent;
            }

            // Calculate luminance from the brush color
            System.Windows.Media.Color color = backgroundBrush.Color;
            double luminance = (0.2126 * color.R + 0.7152 * color.G + 0.0722 * color.B);

            // Return White for dark backgrounds, Black for light backgrounds
            return luminance < 128 ? System.Windows.Media.Brushes.White : System.Windows.Media.Brushes.Black;

        }
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
