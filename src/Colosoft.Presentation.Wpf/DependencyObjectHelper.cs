using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Windows;
using System.Windows.Markup.Primitives;

namespace Colosoft.Presentation
{
    public static class DependencyObjectHelper
    {
        public static IEnumerable<DependencyProperty> GetDependencyProperties(object element)
        {
            if (element == null)
            {
                return System.Array.Empty<DependencyProperty>();
            }

            var dependencyPropertyType = typeof(DependencyProperty);

            return element.GetType()
                .GetFields(BindingFlags.Public | BindingFlags.FlattenHierarchy | BindingFlags.Instance | BindingFlags.Static)
                .Where(f => f.FieldType == dependencyPropertyType)
                .Select(f => (DependencyProperty)f.GetValue(null));
        }

        public static IEnumerable<DependencyProperty> GetAttachedProperties(object element)
        {
            MarkupObject markupObject = MarkupWriter.GetMarkupObjectFor(element);

            if (markupObject != null)
            {
                foreach (MarkupProperty mp in markupObject.Properties)
                {
                    if (mp.IsAttached)
                    {
                        yield return mp.DependencyProperty;
                    }
                }
            }
        }
    }
}
