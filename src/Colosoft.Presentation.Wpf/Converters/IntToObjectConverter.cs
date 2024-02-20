using System;

namespace Colosoft.Presentation.Converters
{
    public class IntToObjectConverter : GenericConverter<IntToObjectConverter>
    {
        public override object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return value ?? (object)0;
        }

        public override object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return value ?? (object)0;
        }
    }
}