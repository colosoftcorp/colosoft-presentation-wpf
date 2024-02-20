using System;

namespace Colosoft.Presentation.Converters
{
    public class CollectionViewFactoryConverter : GenericConverter<CollectionViewFactoryConverter>
    {
        public override object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value is System.Collections.IEnumerable enumerable)
            {
                return new Collections.ObservableCollectionViewFactory(enumerable);
            }

            return value;
        }

        public override object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value is Collections.ObservableCollectionViewFactory factory)
            {
                return factory.Source;
            }

            return base.ConvertBack(value, targetType, parameter, culture);
        }
    }
}
