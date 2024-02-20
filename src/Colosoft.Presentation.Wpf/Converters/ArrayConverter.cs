using System;
using System.Collections.Generic;

namespace Colosoft.Presentation.Converters
{
    public class ArrayConverter : GenericMultiValueConverter<ArrayConverter>
    {
        public override object Convert(object[] values, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return values == null ? null : new List<object>(values);
        }
    }
}
