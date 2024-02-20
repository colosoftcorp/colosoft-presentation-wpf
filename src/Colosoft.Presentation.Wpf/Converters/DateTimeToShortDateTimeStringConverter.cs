using System;

namespace Colosoft.Presentation.Converters
{
    public class DateTimeToShortDateTimeStringConverter : Colosoft.Presentation.Converters.GenericConverter<DateTimeToShortDateTimeStringConverter>
    {
        public override object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value is DateTime date1)
            {
                return date1.ToString(culture);
            }
            else if (value is DateTimeOffset date2)
            {
                return date2.DateTime.ToString(culture);
            }

            return null;
        }
    }
}
