using System;

namespace Colosoft.Presentation.Converters
{
    public class NegativeToNullConverter : GenericConverter<NegativeToNullConverter>
    {
        public override object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value is int n && n < 0)
            {
                return null;
            }

            return value;
        }

#pragma warning disable S4144 // Methods should not have identical implementations
        public override object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
#pragma warning restore S4144 // Methods should not have identical implementations
        {
            if (value is int n && n < 0)
            {
                return null;
            }

            return value;
        }
    }
}
