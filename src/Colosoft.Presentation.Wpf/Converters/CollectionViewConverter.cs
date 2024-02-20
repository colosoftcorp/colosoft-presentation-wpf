using System;

namespace Colosoft.Presentation.Converters
{
    public class CollectionViewConverter : GenericConverter<CollectionViewConverter>
    {
        public override object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return value is System.Collections.IEnumerable
                ? new Collections.ObservableCollectionView((System.Collections.IEnumerable)value)
                : value;
        }

        public override object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value is Collections.ObservableCollectionView view)
            {
                return view.SourceCollection;
            }

            return base.ConvertBack(value, targetType, parameter, culture);
        }
    }
}
