using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Markup;

namespace Colosoft.Presentation.Converters
{
    public abstract class ValueConverter : MarkupExtension, IValueConverter, IMultiValueConverter
    {
        object IValueConverter.Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return this.Convert(value, targetType, parameter, culture);
        }

        object IValueConverter.ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return this.ConvertBack(value, targetType, parameter, culture);
        }

        object IMultiValueConverter.Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            return this.MultiConvert(values, targetType, parameter, culture);
        }

        object[] IMultiValueConverter.ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            return this.MultiConvertBack(value, targetTypes, parameter, culture);
        }

        public virtual object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        public object Convert(object value)
        {
            return this.Convert(value, null);
        }

        public object Convert(object value, object parameter)
        {
            return this.Convert(value, null, parameter, null);
        }

        public virtual object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        public object ConvertBack(object value)
        {
            return this.ConvertBack(value, null);
        }

        public object ConvertBack(object value, object parameter)
        {
            return this.ConvertBack(value, null, parameter, null);
        }

        public virtual object MultiConvert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        public object MultiConvert(object[] values)
        {
            return this.MultiConvert(values, null);
        }

        public object MultiConvert(object[] values, object parameter)
        {
            return this.MultiConvert(values, null, parameter, null);
        }

        public virtual object[] MultiConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        public object[] MultiConvertBack(object value)
        {
            return this.MultiConvertBack(value, null);
        }

        public object[] MultiConvertBack(object value, object parameter)
        {
            return this.MultiConvertBack(value, null, parameter, null);
        }

        public sealed override object ProvideValue(IServiceProvider serviceProvider)
        {
            return this;
        }
    }
}
