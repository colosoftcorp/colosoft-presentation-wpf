using System;

namespace Colosoft.Presentation.Converters
{
    public class DateToStringConverter : GenericConverter<DateToStringConverter>
    {
        public override object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value is IFormattable formattable)
            {
                try
                {
                    return formattable.ToString("d", culture);
                }
                catch
                {
                    // ignore
                }
            }

            return null;
        }

        public override object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            var dateString = value as string;

            if (dateString != null)
            {
                return DateTime.Parse(dateString, culture);
            }

            return DateTime.Now;
        }
    }
}
