using System;

namespace Colosoft.Presentation.Converters
{
    public class DateTimeToShortDateStringConverter : Colosoft.Presentation.Converters.GenericConverter<DateTimeToShortDateStringConverter>
    {
        public override object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value is DateTime date1)
            {
                return date1.ToShortDateString();
            }
            else if (value is DateTimeOffset date2)
            {
                return date2.DateTime.ToShortDateString();
            }

            return null;
        }
    }
}
