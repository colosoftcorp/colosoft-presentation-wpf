using System;
using System.Windows.Data;

namespace Colosoft.Presentation.Converters
{
    public class UriResourceConverter : IValueConverter
    {
        private static UriResourceConverter instance;

        public static UriResourceConverter Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new UriResourceConverter();
                }

                return instance;
            }
        }

        public Themes.IThemeManager ThemeManager { get; set; }

        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            var uri = value as Uri;

            if (uri == null && parameter != null)
            {
                if (parameter is string text)
                {
                    uri = new Uri(text);
                }
                else
                {
                    uri = parameter as Uri;
                }
            }

            if (uri != null)
            {
                if (this.ThemeManager != null)
                {
                    return this.ThemeManager.CurrentTheme.GetResourceObject(uri);
                }
                else
                {
                    return null;
                }
            }

            return value;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
