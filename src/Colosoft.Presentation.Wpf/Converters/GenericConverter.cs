using System;

namespace Colosoft.Presentation.Converters
{
    public abstract class GenericConverter<T>
        : System.Windows.Data.IValueConverter
        where T : GenericConverter<T>, new()
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

        public abstract object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture);

        public virtual object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return value;
        }
    }
}
