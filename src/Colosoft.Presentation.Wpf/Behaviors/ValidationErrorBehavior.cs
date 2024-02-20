using System;
using System.Collections.Generic;
using System.Windows;

namespace Colosoft.Presentation.Behaviors
{
    public static class ValidationErrorBehavior
    {
        public static readonly DependencyProperty ErrorsProperty =
            DependencyProperty.RegisterAttached(
                "Errors",
                typeof(List<IError>),
                typeof(ValidationErrorBehavior),
                new PropertyMetadata(null));

        private static List<IError> GetErrorsInternal(DependencyObject owner)
        {
            var result = (List<IError>)owner.GetValue(ValidationErrorBehavior.ErrorsProperty);

            if (result == null)
            {
                result = new List<IError>();
                owner.SetValue(ValidationErrorBehavior.ErrorsProperty, result);
            }

            return result;
        }

        public static bool AddError(DependencyObject owner, IError error)
        {
            if (owner is null)
            {
                throw new ArgumentNullException(nameof(owner));
            }

            if (error is null)
            {
                throw new ArgumentNullException(nameof(error));
            }

            var errors = GetErrorsInternal(owner);
            if (!errors.Contains(error))
            {
                errors.Add(error);
                return true;
            }

            return false;
        }

        public static bool RemoveError(DependencyObject owner, IError error)
        {
            if (owner is null)
            {
                throw new ArgumentNullException(nameof(owner));
            }

            if (error is null)
            {
                throw new ArgumentNullException(nameof(error));
            }

            var errors = GetErrorsInternal(owner);
            return errors.Remove(error);
        }

        public static void ClearErrorsByType(DependencyObject owner, Type errorType)
        {
            if (owner is null)
            {
                throw new ArgumentNullException(nameof(owner));
            }

            if (errorType is null)
            {
                throw new ArgumentNullException(nameof(errorType));
            }

            var errors = GetErrorsInternal(owner);

            for (var i = 0; i < errors.Count; i++)
            {
                if (errorType.IsInstanceOfType(errors[i]))
                {
                    errors.RemoveAt(i--);
                }
            }
        }

        public static bool ContainsErrorsByType(DependencyObject owner, Type errorType)
        {
            if (owner is null)
            {
                throw new ArgumentNullException(nameof(owner));
            }

            if (errorType is null)
            {
                throw new ArgumentNullException(nameof(errorType));
            }

            var errors = GetErrorsInternal(owner);

            for (var i = 0; i < errors.Count; i++)
            {
                if (errorType.IsInstanceOfType(errors[i]))
                {
                    return true;
                }
            }

            return false;
        }

        public static void ClearErrors(DependencyObject owner)
        {
            if (owner is null)
            {
                throw new ArgumentNullException(nameof(owner));
            }

            GetErrorsInternal(owner).Clear();
        }

        public static bool HasErrors(DependencyObject owner)
        {
            if (owner is null)
            {
                throw new ArgumentNullException(nameof(owner));
            }

            return GetErrorsInternal(owner).Count > 0;
        }

        public static IEnumerable<IError> GetErrors(DependencyObject owner)
        {
            if (owner is null)
            {
                throw new ArgumentNullException(nameof(owner));
            }

            var result = (List<IError>)owner.GetValue(ValidationErrorBehavior.ErrorsProperty);

            if (result == null)
            {
                return Array.Empty<IError>();
            }
            else
            {
                return result;
            }
        }

#pragma warning disable CA1034 // Nested types should not be visible
        public interface IError
#pragma warning restore CA1034 // Nested types should not be visible
        {
            string Key { get; }

            IMessageFormattable Message { get; }

            Exception Exception { get; }
        }
    }
}
