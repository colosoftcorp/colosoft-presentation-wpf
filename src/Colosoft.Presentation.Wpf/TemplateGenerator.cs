using System;
using System.Windows;
using System.Windows.Controls;

namespace Colosoft.Presentation
{
    public static class TemplateGenerator
    {
        private sealed class TemplateGeneratorControl : ContentControl
        {
            public static readonly DependencyProperty FactoryProperty =
                DependencyProperty.Register("Factory", typeof(Func<object>), typeof(TemplateGeneratorControl), new PropertyMetadata(null, FactoryChanged));

            private static void FactoryChanged(DependencyObject instance, DependencyPropertyChangedEventArgs args)
            {
                if (instance is TemplateGeneratorControl control &&
                    args.NewValue is Func<object> factory)
                {
                    control.Content = factory();
                }
            }
        }

        public static DataTemplate CreateDataTemplate(Func<object> factory)
        {
            if (factory == null)
            {
                throw new ArgumentNullException(nameof(factory));
            }

            var frameworkElementFactory = new FrameworkElementFactory(typeof(TemplateGeneratorControl));
            frameworkElementFactory.SetValue(TemplateGeneratorControl.FactoryProperty, factory);

            var dataTemplate = new DataTemplate(typeof(DependencyObject));
            dataTemplate.VisualTree = frameworkElementFactory;
            return dataTemplate;
        }

        public static ControlTemplate CreateControlTemplate(Type controlType, Func<object> factory)
        {
            if (controlType == null)
            {
                throw new ArgumentNullException(nameof(controlType));
            }

            if (factory == null)
            {
                throw new ArgumentNullException(nameof(factory));
            }

            var frameworkElementFactory = new FrameworkElementFactory(typeof(TemplateGeneratorControl));
            frameworkElementFactory.SetValue(TemplateGeneratorControl.FactoryProperty, factory);

            var controlTemplate = new ControlTemplate(controlType);
            controlTemplate.VisualTree = frameworkElementFactory;
            return controlTemplate;
        }
    }
}
