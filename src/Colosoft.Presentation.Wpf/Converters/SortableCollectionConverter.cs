using System;
using System.Linq;

namespace Colosoft.Presentation.Converters
{
    public class SortableCollectionConverter : System.Windows.Data.IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value is Colosoft.Collections.IObservableCollection && parameter is string)
            {
                foreach (var i in value.GetType().GetInterfaces())
                {
                    if (i.Name.StartsWith("IObservableCollection", StringComparison.Ordinal) && i.IsGenericType)
                    {
                        var itemType = i.GetGenericArguments().First();

                        var sortableType = typeof(Colosoft.Collections.SortableObservableCollection<>).MakeGenericType(itemType);

                        var comparer = typeof(PropertiesComparer<>).MakeGenericType(itemType)
                            .GetConstructor(new Type[] { typeof(string[]) })
                            .Invoke(new object[] { ((string)parameter).Split(';').Where(f => !string.IsNullOrEmpty(f)).ToArray() });

                        try
                        {
                            return Activator.CreateInstance(sortableType, new object[] { value, comparer });
                        }
                        catch (System.Reflection.TargetInvocationException ex)
                        {
                            throw ex.InnerException;
                        }
                    }
                }
            }

            return value;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return value;
        }
    }
}
