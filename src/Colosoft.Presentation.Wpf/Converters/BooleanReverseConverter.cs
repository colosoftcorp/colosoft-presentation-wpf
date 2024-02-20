using System;

namespace Colosoft.Presentation.Converters
{
    public class BooleanReverseConverter : GenericConverter<BooleanReverseConverter>
    {
        public override object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value == null)
            {
                return true;
            }

            if (value is bool)
            {
                return !(bool)value;
            }

            return value;
        }

        public override object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return this.Convert(value, targetType, parameter, culture);
        }
    }
}
