using System;

namespace Colosoft.Presentation.Converters
{
    public class DateTimeToStringConverter : GenericConverter<DateTimeToStringConverter>
    {
        public override object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value is DateTime date)
            {
                if (date == DateTime.MinValue)
                {
                    return null;
                }

                return date.ToString(culture);
            }

            return null;
        }

        public override object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value is string text)
            {
                if (string.IsNullOrEmpty(text))
                {
                    return DateTime.MinValue;
                }

                try
                {
                    return DateTime.Parse(text, culture);
                }
                catch (FormatException)
                {
                    return DateTime.MinValue;
                }
            }

            return DateTime.MinValue;
        }
    }
}
