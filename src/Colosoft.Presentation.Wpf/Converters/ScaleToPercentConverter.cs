using System;
using System.Globalization;

namespace Colosoft.Presentation.Converters
{
    public class ScaleToPercentConverter : GenericConverter<ScaleToPercentConverter>
    {
        public override object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return (double)(int)((double)value * 100.0);
        }

        public override object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return (double)value / 100.0;
        }
    }
}
