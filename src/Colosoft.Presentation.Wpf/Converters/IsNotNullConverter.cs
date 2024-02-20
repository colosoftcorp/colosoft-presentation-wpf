using System;

namespace Colosoft.Presentation.Converters
{
    public class IsNotNullConverter : GenericConverter<IsNotNullConverter>
    {
        public override object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return value != null;
        }
    }
}
