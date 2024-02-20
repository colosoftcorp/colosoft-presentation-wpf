using System;

namespace Colosoft.Presentation.Converters
{
    public class BooleanReverseToVisibilityConverter : GenericConverter<BooleanReverseToVisibilityConverter>
    {
        public override object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (!(bool)value)
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
