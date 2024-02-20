using Colosoft.Presentation.Converters;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics.Contracts;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Markup;

namespace Colosoft.Presentation
{
    public static class BindingHelper
    {
        private static readonly List<DependencyProperty> DefaultProperties;

#pragma warning disable S3963 // "static" fields should be initialized inline
#pragma warning disable CA1810 // Initialize reference type static fields inline
        static BindingHelper()
#pragma warning restore CA1810 // Initialize reference type static fields inline
        {
            DefaultProperties = new List<DependencyProperty>();

            DefaultProperties.Add(TextBox.TextProperty);
            DefaultProperties.Add(System.Windows.Controls.Primitives.Selector.SelectedValueProperty);

            EventManager.RegisterClassHandler(typeof(PasswordBox), PasswordBox.PasswordChangedEvent, new RoutedEventHandler(OnPasswordBoxChanged), true);
        }
#pragma warning restore S3963 // "static" fields should be initialized inline

        public static void UpdateSourceDefaultProperty(this DependencyObject o)
        {
            if (o is null)
            {
                throw new ArgumentNullException(nameof(o));
            }

            Type type = o.GetType();
            DependencyProperty prop = GetDefaultDependencyProperty(type);
            BindingExpression exp = BindingOperations.GetBindingExpression(o, prop);
            if (exp != null)
            {
                exp.UpdateSource();
            }
        }

        public static void UpdateSourceProperty(this DependencyObject o, DependencyProperty prop)
        {
            var exp = BindingOperations.GetBindingExpressionBase(o, prop);
            if (exp != null)
            {
                exp.UpdateSource();
            }
        }

        public static void UpdateTarget(this DependencyObject o, DependencyProperty prop)
        {
            var exp = BindingOperations.GetBindingExpressionBase(o, prop);
            if (exp != null)
            {
                exp.UpdateTarget();
            }
        }

        public static bool HasError(this DependencyObject o)
        {
            if (o is null)
            {
                throw new ArgumentNullException(nameof(o));
            }

            var type = o.GetType();
            var prop = GetDefaultDependencyProperty(type);

            return HasError(o, prop);
        }

        public static bool HasError(this DependencyObject o, DependencyProperty p)
        {
            var exp = BindingOperations.GetBindingExpression(o, p);
            return exp.HasError;
        }

        public static ReadOnlyCollection<ValidationError> GetErrors(this DependencyObject root, bool markInvalid)
        {
            var errors = new List<ValidationError>();

            UIHelper2.FindVisualDescendant(root, delegate(DependencyObject o)
            {
                errors.AddRange(System.Windows.Controls.Validation.GetErrors(o));
                return false;
            });

            if (markInvalid)
            {
                foreach (ValidationError error in errors)
                {
                    Validation.MarkInvalid(
                        (BindingExpressionBase)error.BindingInError, error);
                }
            }

            return errors.AsReadOnly();
        }

        public static DependencyProperty GetDefaultDependencyProperty(Type type)
        {
            DependencyProperty prop = null;

#pragma warning disable S3267 // Loops should be simplified with "LINQ" expressions
            foreach (DependencyProperty defaultProp in DefaultProperties)
            {
                if (defaultProp.OwnerType.IsAssignableFrom(type))
                {
                    prop = defaultProp;
                    break;
                }
            }
#pragma warning restore S3267 // Loops should be simplified with "LINQ" expressions

            if (prop == null)
            {
                string propertyName = GetDefaultPropertyName(type);
                if (propertyName != null)
                {
                    prop = DependencyHelper.GetDependencyProperty(type, GetDefaultPropertyName(type));
                }
            }

            return prop;
        }

        public static string GetDefaultPropertyName(Type type)
        {
            if (type is null)
            {
                throw new ArgumentNullException(nameof(type));
            }

            var attrs = type.GetCustomAttributes(false);

            foreach (object attr in attrs)
            {
                if (attr is ContentPropertyAttribute)
                {
                    return (attr as ContentPropertyAttribute).Name;
                }

                if (attr is DefaultPropertyAttribute)
                {
                    return (attr as DefaultPropertyAttribute).Name;
                }
            }

            return null;
        }

        public static T Eval<T>(object source, string path)
        {
            Contract.Requires(path != null);

            if (source == null)
            {
                return default(T);
            }

            Binding binding = new Binding(path);
            binding.Source = source;

            return Eval<T>(binding);
        }

        public static T Eval<T>(BindingBase binding)
        {
            Contract.Requires(binding != null);

            EvalHelper helper = new EvalHelper();

            BindingOperations.SetBinding(helper, EvalHelper.ValueProperty, binding);

            T result = (T)helper.Value;

            BindingOperations.ClearBinding(helper, EvalHelper.ValueProperty);

            return result;
        }

        private static bool passwordLock;

        public static string GetPassword(this PasswordBox obj)
        {
            if (obj is null)
            {
                throw new ArgumentNullException(nameof(obj));
            }

            return (string)obj.GetValue(PasswordProperty);
        }

        public static void SetPassword(this PasswordBox obj, string value)
        {
            if (obj is null)
            {
                throw new ArgumentNullException(nameof(obj));
            }

            obj.SetValue(PasswordProperty, value);
        }

        public static readonly DependencyProperty PasswordProperty =
            DependencyProperty.RegisterAttached(
                "Password",
                typeof(string),
                typeof(BindingHelper),
                new FrameworkPropertyMetadata(string.Empty, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault | FrameworkPropertyMetadataOptions.Journal, OnPasswordChanged));

        private static void OnPasswordChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            if (passwordLock)
            {
                return;
            }

            PasswordBox passwordBox = o as PasswordBox;
            if (passwordBox != null)
            {
                passwordLock = true;
                passwordBox.Password = (string)e.NewValue;
                passwordLock = false;
            }
        }

        private static void OnPasswordBoxChanged(object sender, RoutedEventArgs e)
        {
            if (passwordLock)
            {
                return;
            }

            var passwordBox = sender as PasswordBox;
            if (passwordBox != null)
            {
                passwordLock = true;
                SetPassword(passwordBox, passwordBox.Password);
                passwordLock = false;
            }
        }

        [TypeConverter(typeof(FormattedTextTypeConverter))]
        public static IEnumerable<Inline> GetInlines(this TextBlock obj)
        {
            if (obj is null)
            {
                throw new ArgumentNullException(nameof(obj));
            }

            return (IEnumerable<Inline>)obj.GetValue(InlinesProperty);
        }

        [TypeConverter(typeof(FormattedTextTypeConverter))]
        public static void SetInlines(this TextBlock obj, IEnumerable<Inline> value)
        {
            if (obj is null)
            {
                throw new ArgumentNullException(nameof(obj));
            }

            obj.SetValue(InlinesProperty, value);
        }

        public static readonly DependencyProperty InlinesProperty =
            DependencyProperty.RegisterAttached(
                "Inlines",
                typeof(IEnumerable<Inline>),
                typeof(BindingHelper),
                new FrameworkPropertyMetadata(OnInlinesChanged));

        private static void OnInlinesChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            if (e.NewValue != null)
            {
                TextBlock textBlock = o as TextBlock;
                if (textBlock != null)
                {
                    textBlock.Inlines.AddRange((IEnumerable<Inline>)e.NewValue);
                }
            }
        }

        private sealed class EvalHelper : DependencyObject
        {
            public object Value
            {
                get => this.GetValue(ValueProperty);
#pragma warning disable S1144 // Unused private types or members should be removed
                set => this.SetValue(ValueProperty, value);
#pragma warning restore S1144 // Unused private types or members should be removed
            }

            public static readonly DependencyProperty ValueProperty =
                DependencyProperty.Register("Value", typeof(object), typeof(EvalHelper));
        }
    }
}
