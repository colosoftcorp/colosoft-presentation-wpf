using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Media;

namespace Colosoft.Presentation.Format
{
    public static class DependencyObjectExtensions
    {
        public static T FindVisualChild<T>(this DependencyObject obj)
            where T : DependencyObject
        {
            for (int i = 0; i < VisualTreeHelper.GetChildrenCount(obj); i++)
            {
                DependencyObject child = VisualTreeHelper.GetChild(obj, i);
                if (child is T)
                {
                    return (T)child;
                }
                else
                {
                    T grandChild = child.FindVisualChild<T>();
                    if (grandChild != null)
                    {
                        return grandChild;
                    }
                }
            }

            return null;
        }

        public static UIElement FindFirstFocusableChild(this DependencyObject obj)
        {
            return FindFirstFocusableChild(obj, null);
        }

        public static UIElement FindFirstFocusableChild(this DependencyObject obj, IEnumerable<Type> ignoreTypes)
        {
            for (int i = 0; i < VisualTreeHelper.GetChildrenCount(obj); i++)
            {
                var childInstance = VisualTreeHelper.GetChild(obj, i);

                if (childInstance is UIElement child)
                {
                    var childType = child.GetType();
                    if (child.Focusable)
                    {
                        return child;
                    }
                    else if (ignoreTypes == null || !ignoreTypes.Any(f => f.IsAssignableFrom(childType)))
                    {
                        UIElement grandChild = child.FindFirstFocusableChild(ignoreTypes);
                        if (grandChild != null)
                        {
                            return grandChild;
                        }
                    }
                }
            }

            return null;
        }
    }
}
