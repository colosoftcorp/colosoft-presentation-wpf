using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Threading;

namespace Colosoft.Presentation
{
    public static class DependencyPropertyExtensions
    {
        public static void AddChangeListener(this DependencyObject item, DependencyProperty property, EventHandler handler)
        {
            if ((item == null) || (property == null) || (handler == null))
            {
                return;
            }

            var descriptor = DependencyPropertyDescriptor.FromProperty(property, item.GetType());
            descriptor.AddValueChanged(item, handler);
        }

        public static void RemoveChangeListener(this DependencyObject item, DependencyProperty property, EventHandler handler)
        {
            if ((item == null) || (property == null) || (handler == null))
            {
                return;
            }

            var descriptor = DependencyPropertyDescriptor.FromProperty(property, item.GetType());
            descriptor.RemoveValueChanged(item, handler);
        }

        private static bool TryCastAs<T>(this object instance, out T result, T stdVal = null)
            where T : class
        {
            var valid = instance != null;
            result = valid ? (instance as T) : stdVal;
            return valid && (result != null);
        }

        public static IEnumerable<T> GetChildren<T>(this DependencyObject item)
            where T : DependencyObject
        {
            T itemAsT;
            ContentControl itemAsC;
            if (item.TryCastAs<T>(out itemAsT))
            {
                yield return itemAsT;
            }
            else if (item.TryCastAs<ContentControl>(out itemAsC))
            {
                if (itemAsC?.Content != null)
                {
                    var children = ((DependencyObject)itemAsC.Content).GetChildren<T>();
                    foreach (var child in children)
                    {
                        yield return child;
                    }
                }
            }
            else if (item != null)
            {
                var total = VisualTreeHelper.GetChildrenCount(item);
                for (var i = 0; i < total; ++i)
                {
                    var children = VisualTreeHelper.GetChild(item, i).GetChildren<T>();
                    foreach (var child in children)
                    {
                        yield return child;
                    }
                }
            }
        }

        public static T ConditionalInvoke<T>(this DispatcherObject item, Func<DispatcherObject, T> call)
        {
            if (call is null)
            {
                throw new ArgumentNullException(nameof(call));
            }

            if (item == null)
            {
                return default(T);
            }

            return (item.Dispatcher.Thread != System.Threading.Thread.CurrentThread)
                ? (T)item.Dispatcher.Invoke(call, item)
                : call(item);
        }
    }
}
