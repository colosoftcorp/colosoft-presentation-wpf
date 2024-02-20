using System;

namespace Colosoft.Presentation.Converters
{
    public class DateTimeOffsetToDateTimeConverter : GenericConverter<DateTimeOffsetToDateTimeConverter>
    {
        public override object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value is DateTimeOffset)
            {
                return ((DateTimeOffset)value).DateTime;
            }

            return value;
        }

        public override object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value is DateTime)
            {
                return (DateTimeOffset)((DateTime)value);
            }

            return value;
        }
    }
}
