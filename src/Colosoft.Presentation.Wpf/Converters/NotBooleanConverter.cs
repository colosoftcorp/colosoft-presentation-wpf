using System;

namespace Colosoft.Presentation.Converters
{
    public class NotBooleanConverter : GenericConverter<NotBooleanConverter>
    {
        public override object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return (value == null) || (!(bool)value);
        }

        public override object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return (value == null) || (!(bool)value);
        }
    }
}
