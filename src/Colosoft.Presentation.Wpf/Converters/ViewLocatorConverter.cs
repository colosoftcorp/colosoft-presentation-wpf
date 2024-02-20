using System;
using System.Globalization;
using System.Windows.Data;

namespace Colosoft.Presentation.Converters
{
    public class ViewLocatorConverter : IValueConverter
    {
        public ViewLocatorConverter(IViewLocator viewLocator)
        {
            this.ViewLocator = viewLocator;
        }

        public IViewLocator ViewLocator { get; }

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value != null)
            {
                return this.ViewLocator.ResolveView(value, value.GetType());
            }

            return null;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
