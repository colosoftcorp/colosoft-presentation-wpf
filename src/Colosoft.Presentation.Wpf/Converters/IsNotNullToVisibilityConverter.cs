using System;

namespace Colosoft.Presentation.Converters
{
    public class IsNotNullToVisibilityConverter : GenericConverter<IsNotNullToVisibilityConverter>
    {
        public override object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value != null)
            {
                return System.Windows.Visibility.Visible;
            }
            else
            {
                return System.Windows.Visibility.Collapsed;
            }
        }
    }
}
