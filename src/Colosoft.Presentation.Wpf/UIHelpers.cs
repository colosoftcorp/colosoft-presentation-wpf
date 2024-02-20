using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media;

namespace Colosoft.Presentation
{
    public static class UIHelpers
    {
        public static Control FindChildToFocus(DependencyObject parent)
        {
            if (parent == null)
            {
                return null;
            }

            Control result = null;

            if (parent is UserControl ctrl)
            {
                var content = ctrl.Content as DependencyObject;

                if (content is Control &&
                    ((content is TextBox && !((TextBox)content).IsReadOnly) ||
                     content is ComboBox || content is CheckBox || content is Button))
                {
                    result = (Control)content;

                    if (result.IsEnabled)
                    {
                        return result;
                    }
                }

                result = FindChildToFocus(content);
                if (result != null)
                {
                    return result;
                }
            }

            if ((parent is TextBox && !((TextBox)parent).IsReadOnly) ||
                     parent is ComboBox || parent is CheckBox || parent is Button)
            {
                var ctrl1 = (Control)parent;

                if (ctrl1.IsEnabled)
                {
                    return ctrl1;
                }
            }

            int count = VisualTreeHelper.GetChildrenCount(parent);

            for (int i = 0; i < count; i++)
            {
                var child = VisualTreeHelper.GetChild(parent, i);

                if (child is Control &&
                    ((child is TextBox && !((TextBox)child).IsReadOnly) ||
                     child is ComboBox || child is CheckBox || child is Button))
                {
                    result = (Control)child;

                    if (result.IsEnabled)
                    {
                        return result;
                    }
                }

                result = FindChildToFocus(child);
                if (result != null)
                {
                    return result;
                }
            }

            return null;
        }

        public static bool PlaceCaretOnTextBox(TextBox textBox, Point position)
        {
            if (textBox is null)
            {
                throw new ArgumentNullException(nameof(textBox));
            }

            int characterIndexFromPoint = textBox.GetCharacterIndexFromPoint(position, false);
            if (characterIndexFromPoint >= 0)
            {
                textBox.Select(characterIndexFromPoint, 0);
                return true;
            }

            return false;
        }

        public static IEnumerable<T> TryFindChildren<T>(DependencyObject parent)
            where T : DependencyObject
        {
            if (parent == null)
            {
                yield break;
            }

            var childrenCount = VisualTreeHelper.GetChildrenCount(parent);

            for (int i = 0; i < childrenCount; i++)
            {
                var child = VisualTreeHelper.GetChild(parent, i);

                if (child is T)
                {
                    yield return (T)child;
                }
                else
                {
                    foreach (var c in TryFindChildren<T>(child))
                    {
                        yield return c;
                    }
                }
            }
        }

        public static IEnumerable<DependencyObject> NavigateChildren(DependencyObject parent)
        {
            if (parent == null)
            {
                yield break;
            }

            var childrenCount = VisualTreeHelper.GetChildrenCount(parent);

            for (int i = 0; i < childrenCount; i++)
            {
                var child = VisualTreeHelper.GetChild(parent, i);

                yield return child;

                foreach (var c in NavigateChildren(child))
                {
                    yield return c;
                }
            }
        }

        public static T TryFindChild<T>(DependencyObject parent)
            where T : DependencyObject
        {
            if (parent == null)
            {
                return null;
            }

            var childrenCount = VisualTreeHelper.GetChildrenCount(parent);

            for (int i = 0; i < childrenCount; i++)
            {
                var child = VisualTreeHelper.GetChild(parent, i);

                if (child is T)
                {
                    return (T)child;
                }

                var result = TryFindChild<T>(child);

                if (child != null)
                {
                    return result;
                }
            }

            return null;
        }

        public static T TryFindParent<T>(DependencyObject child)
          where T : DependencyObject
        {
            DependencyObject parentObject = GetParentObject(child);

            if (parentObject == null)
            {
                return null;
            }

            var parent = parentObject as T;
            if (parent != null)
            {
                return parent;
            }
            else
            {
                return TryFindParent<T>(parentObject);
            }
        }

        public static DependencyObject GetParentObject(DependencyObject child)
        {
            if (child == null)
            {
                return null;
            }

            var contentElement = child as ContentElement;

            if (contentElement != null)
            {
                DependencyObject parent = ContentOperations.GetParent(contentElement);
                if (parent != null)
                {
                    return parent;
                }

                var fce = contentElement as FrameworkContentElement;
                return fce?.Parent;
            }

            return VisualTreeHelper.GetParent(child);
        }

        public static IEnumerable<DependencyObject> NavigateParents(DependencyObject obj)
        {
            DependencyObject parentObject = GetParentObject(obj);

            if (parentObject == null)
            {
                yield break;
            }

            yield return parentObject;

            foreach (var i in NavigateParents(parentObject))
            {
                yield return i;
            }
        }

        public static void UpdateBindingTargets(DependencyObject obj)
        {
            foreach (DependencyProperty depProperty in DependencyObjectHelper.GetDependencyProperties(obj))
            {
                BindingExpression be = BindingOperations.GetBindingExpression(obj, depProperty);
                if (be != null)
                {
                    be.UpdateTarget();
                }
            }

            int count = VisualTreeHelper.GetChildrenCount(obj);
            for (int i = 0; i < count; i++)
            {
                DependencyObject childObject = VisualTreeHelper.GetChild(obj, i);
                UpdateBindingTargets(childObject);
            }
        }

        public static void UpdateBindingSources(DependencyObject obj)
        {
            foreach (DependencyProperty depProperty in DependencyObjectHelper.GetDependencyProperties(obj))
            {
                BindingExpression be = BindingOperations.GetBindingExpression(obj, depProperty);
                if (be != null)
                {
                    be.UpdateSource();
                }
            }

            int count = VisualTreeHelper.GetChildrenCount(obj);
            for (int i = 0; i < count; i++)
            {
                DependencyObject childObject = VisualTreeHelper.GetChild(obj, i);
                UpdateBindingSources(childObject);
            }
        }

        public static void UpdateBindingSources(DependencyObject obj, DependencyProperty[] properties)
        {
            if (properties is null)
            {
                throw new ArgumentNullException(nameof(properties));
            }

            foreach (DependencyProperty depProperty in properties)
            {
                BindingExpression be = BindingOperations.GetBindingExpression(obj, depProperty);
                if (be != null)
                {
                    be.UpdateSource();
                }
            }

            int count = VisualTreeHelper.GetChildrenCount(obj);
            for (int i = 0; i < count; i++)
            {
                DependencyObject childObject = VisualTreeHelper.GetChild(obj, i);
                UpdateBindingSources(childObject, properties);
            }
        }

        public static void DesactivateBindingExpressions(DependencyObject parent)
        {
#pragma warning disable S3011 // Reflection should not be used to increase accessibility of classes, methods, or fields
            var desactiveMethod = typeof(BindingExpression)
                .GetMethod(
                    "Deactivate",
                    System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance,
                    null,
                    Array.Empty<Type>(),
                    null);
#pragma warning restore S3011 // Reflection should not be used to increase accessibility of classes, methods, or fields

            DesactivateBindingExpressions(parent, desactiveMethod);
        }

        private static void DesactivateBindingExpressions(DependencyObject parent, System.Reflection.MethodInfo desactiveMethod)
        {
            foreach (DependencyProperty depProperty in DependencyObjectHelper.GetDependencyProperties(parent))
            {
                BindingExpression be = BindingOperations.GetBindingExpression(parent, depProperty);
                if (be != null)
                {
                    desactiveMethod.Invoke(be, Array.Empty<object>());
                }
            }

            int count = VisualTreeHelper.GetChildrenCount(parent);
            for (int i = 0; i < count; i++)
            {
                DependencyObject childObject = VisualTreeHelper.GetChild(parent, i);
                DesactivateBindingExpressions(childObject, desactiveMethod);
            }
        }

        public static T TryFindFromPoint<T>(UIElement reference, Point point)
          where T : DependencyObject
        {
            if (reference is null)
            {
                throw new ArgumentNullException(nameof(reference));
            }

            var element = reference.InputHitTest(point) as DependencyObject;
            if (element == null)
            {
                return null;
            }
            else if (element is T)
            {
                return (T)element;
            }
            else
            {
                return TryFindParent<T>(element);
            }
        }
    }
}
