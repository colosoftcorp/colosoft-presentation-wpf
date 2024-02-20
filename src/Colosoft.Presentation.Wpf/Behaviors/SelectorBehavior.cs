using System;
using System.Windows;

namespace Colosoft.Presentation.Behaviors
{
    public static class SelectorBehavior
    {
        private static readonly CoerceValueCallback OldCoerceValueCallback;

#pragma warning disable S3963 // "static" fields should be initialized inline
#pragma warning disable CA1810 // Initialize reference type static fields inline
        static SelectorBehavior()
#pragma warning restore CA1810 // Initialize reference type static fields inline
        {
            var metadata = System.Windows.Controls.Primitives.Selector.SelectedItemProperty
                .GetMetadata(typeof(System.Windows.Controls.Primitives.Selector));

            OldCoerceValueCallback = metadata.CoerceValueCallback;

#pragma warning disable S3011 // Reflection should not be used to increase accessibility of classes, methods, or fields
            var coerceValueCallbackField = typeof(PropertyMetadata).GetField(
               "_coerceValueCallback",
               System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
#pragma warning restore S3011 // Reflection should not be used to increase accessibility of classes, methods, or fields

            coerceValueCallbackField.SetValue(metadata, new CoerceValueCallback(SelectedItemCoerceValue));
        }
#pragma warning restore S3963 // "static" fields should be initialized inline

        public static void Configure()
        {
            // ESSE MÉTODO ESTÁ VAZIO PORQUE ELE É UMA
            // FORMA DE FAZER REFENCIA PARA A EXECUÇÃO DO
            // CONSTRUTOR ESTÁTICO DA CLASSE
        }

        private static object SelectedItemCoerceValue(DependencyObject d, object baseValue)
        {
            object result = null;

            try
            {
                result = OldCoerceValueCallback(d, baseValue);
            }
            catch (ArgumentOutOfRangeException)
            {
                return null;
            }

            return result == DependencyProperty.UnsetValue ? null : result;
        }
    }
}
