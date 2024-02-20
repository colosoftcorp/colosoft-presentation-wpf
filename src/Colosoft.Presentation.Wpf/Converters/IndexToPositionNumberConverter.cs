using System;

namespace Colosoft.Presentation.Converters
{
    public class IndexToPositionNumberConverter : GenericConverter<IndexToPositionNumberConverter>
    {
        public override object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value is int n)
            {
                return n + 1;
            }

            return value;
        }
    }
}
