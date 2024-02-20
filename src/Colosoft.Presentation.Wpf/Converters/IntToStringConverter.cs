using System;

namespace Colosoft.Presentation.Converters
{
    public class IntToStringConverter : GenericConverter<IntToStringConverter>
    {
        public override object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return value?.ToString();
        }

        public override object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            string input = value as string;
            if (string.IsNullOrWhiteSpace(input))
            {
                return 0;
            }

            return int.TryParse(input, out var result) ? result : 0;
        }
    }
}
