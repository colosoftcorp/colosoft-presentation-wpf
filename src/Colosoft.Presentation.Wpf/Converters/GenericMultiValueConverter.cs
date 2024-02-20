using System;
using System.Windows.Data;

namespace Colosoft.Presentation.Converters
{
    public abstract class GenericMultiValueConverter<T>
        : IMultiValueConverter
        where T : IMultiValueConverter, new()
    {
        private static T instance;

#pragma warning disable CA1000 // Do not declare static members on generic types
        public static T Instance
#pragma warning restore CA1000 // Do not declare static members on generic types
        {
            get
            {
                if (instance == null)
                {
                    instance = new T();
                }

                return instance;
            }
        }

        public abstract object Convert(object[] values, Type targetType, object parameter, System.Globalization.CultureInfo culture);

        public virtual object[] ConvertBack(object value, Type[] targetTypes, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
