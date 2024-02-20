using System;

namespace Colosoft.Presentation.Converters
{
    public class IsNullConverter : GenericConverter<IsNullConverter>
    {
        public override object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return value == null;
        }
    }
}
