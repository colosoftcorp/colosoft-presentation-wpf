using System;

namespace Colosoft.Presentation.Converters
{
    public class EnumMatchToBooleanConverter : GenericConverter<EnumMatchToBooleanConverter>
    {
        public override object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value == null || parameter == null)
            {
                return false;
            }

            string checkValue = value.ToString();
            string targetValue = parameter.ToString();
            return checkValue.Equals(targetValue, StringComparison.InvariantCultureIgnoreCase);
        }

        public override object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value == null || parameter == null)
            {
                return null;
            }

            bool useValue = (bool)value;
            string targetValue = parameter.ToString();
            if (useValue)
            {
                return Enum.Parse(targetType, targetValue);
            }

            return null;
        }
    }
}
