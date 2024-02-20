using System;

namespace Colosoft.Presentation.Converters
{
    public class CloneableConverter : GenericConverter<CloneableConverter>
    {
        public override object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            var cloneable = value as ICloneable;

            if (cloneable != null)
            {
                return cloneable.Clone();
            }

            return value;
        }
    }
}
