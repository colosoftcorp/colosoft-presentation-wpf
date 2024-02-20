using System;
using System.Diagnostics.Contracts;
using System.Reflection;
using System.Windows;

namespace Colosoft.Presentation
{
    internal static class DependencyHelper
    {
        internal static DependencyProperty GetDependencyProperty(Type type, string propertyName)
        {
            DependencyProperty prop = null;

            if (type != null)
            {
                FieldInfo fieldInfo = type.GetField(
                    $"{propertyName}Property",
                    BindingFlags.Static | BindingFlags.Public);

                if (fieldInfo != null)
                {
                    prop = fieldInfo.GetValue(null) as DependencyProperty;
                }
            }

            return prop;
        }

        internal static DependencyProperty GetDependencyProperty(DependencyObject o, string propertyName)
        {
            DependencyProperty prop = null;

            if (o != null)
            {
                prop = GetDependencyProperty(o.GetType(), propertyName);
            }

            return prop;
        }

        internal static bool SetIfNonLocal<T>(this DependencyObject o, DependencyProperty property, T value)
        {
            Contract.Requires(o != null);
            Contract.Requires(property != null);

            if (!property.PropertyType.IsAssignableFrom(typeof(T)))
            {
                throw new ArgumentException("Type of dependency property is incompatible with value.");
            }

            BaseValueSource source = DependencyPropertyHelper.GetValueSource(o, property).BaseValueSource;
            if (source != BaseValueSource.Local)
            {
                o.SetValue(property, value);

                return true;
            }

            return false;
        }
    }
}
