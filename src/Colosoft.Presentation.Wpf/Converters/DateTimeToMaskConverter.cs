using System;

namespace Colosoft.Presentation.Converters
{
    public class DateTimeToMaskConverter : GenericConverter<DateTimeToMaskConverter>
    {
        public override object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            bool hasValue = value != null;
            if (!hasValue)
            {
                return null;
            }

            if (value is DateTime)
            {
                var date = (DateTime)value;
                if (date == DateTime.MinValue)
                {
                    return null;
                }

                return date.ToString("dd/MM/yyyy", culture);
            }
            else if (value is DateTimeOffset date)
            {
                if (date == DateTimeOffset.MinValue)
                {
                    return null;
                }

                return date.ToString("dd/MM/yyyy", culture);
            }

            return null;
        }

        public override object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            bool hasValue = value != null;
            if (!hasValue)
            {
                return null;
            }

            if (value is string text)
            {
                if (targetType is null)
                {
                    throw new ArgumentNullException(nameof(targetType));
                }

                if (targetType.Equals(typeof(DateTime)))
                {
                    if (string.IsNullOrEmpty(text))
                    {
                        return DateTime.MinValue;
                    }

                    if ("__/__/____".Equals(text))
                    {
                        return System.Windows.Data.Binding.DoNothing;
                    }

                    DateTime resultDate;
                    return DateTime.TryParse(text, culture?.DateTimeFormat, System.Globalization.DateTimeStyles.None, out resultDate)
                        ? resultDate
                        : DateTime.MinValue;
                }
                else if (targetType.Equals(typeof(DateTimeOffset)))
                {
                    if (string.IsNullOrEmpty(text))
                    {
                        return DateTimeOffset.MinValue;
                    }

                    if ("__/__/____".Equals(text))
                    {
                        return System.Windows.Data.Binding.DoNothing;
                    }

                    DateTimeOffset resultOffset;
                    return DateTimeOffset.TryParse(text, culture?.DateTimeFormat, System.Globalization.DateTimeStyles.None, out resultOffset)
                        ? resultOffset
                        : DateTimeOffset.MinValue;
                }
            }

            return DateTime.MinValue;
        }
    }
}