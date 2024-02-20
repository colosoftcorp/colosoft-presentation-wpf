using System;
using System.Windows.Data;

namespace Colosoft.Presentation.Converters
{
    public class MessageFormattableConverter : IValueConverter
    {
        private static IValueConverter instance;

        public static IValueConverter Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new MessageFormattableConverter();
                }

                return instance;
            }
        }

        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            var message = value as IMessageFormattable;

            if (message != null)
            {
                return message.Format(culture);
            }

            return string.Empty;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
