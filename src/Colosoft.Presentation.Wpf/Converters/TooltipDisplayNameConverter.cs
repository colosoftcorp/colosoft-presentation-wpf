using System;

namespace Colosoft.Presentation.Converters
{
    public class TooltipDisplayNameConverter : GenericConverter<TooltipDisplayNameConverter>
    {
        public override object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value != null)
            {
                if (value.ToString().StartsWith("[", StringComparison.Ordinal) && value.ToString().EndsWith("]", StringComparison.Ordinal))
                {
                    var split = value.ToString().Split(',');
                    if (split.Length > 0)
                    {
                        var displayValue = split[split.Length - 1];
                        return displayValue.Length > 1 ? displayValue.Remove(displayValue.Length - 1, 1).TrimStart() : displayValue;
                    }
                }

                return value.ToString();
            }

            return string.Empty;
        }
    }
}
