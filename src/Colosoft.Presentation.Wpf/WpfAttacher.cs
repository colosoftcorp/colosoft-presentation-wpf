using Colosoft.Presentation.PresentationData;
using System;
using System.Collections.Generic;
using System.Windows;

namespace Colosoft.Presentation
{
    public class WpfAttacher : DependencyObject, IAttacher
    {
        private readonly List<ICommandData> pendingControls = new List<ICommandData>();
        private readonly ICommandDataConfigurator commandDataConfigurator;
        private UIElement mainUIElement;

        public WpfAttacher(ICommandDataConfigurator commandDataConfigurator)
        {
            this.commandDataConfigurator = commandDataConfigurator;
        }

        public UIElement MainUIElement
        {
            get { return this.mainUIElement; }
        }

        private static IEnumerable<object> ProcessElement(object element)
        {
            if (element is Behaviors.IWindowContainer container)
            {
                var window = container.Window;

                if (window is Behaviors.WindowWrapper windowWrapper)
                {
                    yield return windowWrapper.Window;
                }
            }

            if (element is UIElement)
            {
                yield return element;
            }
        }

        private void AttachInternal(ICommandData value)
        {
            if (value is null)
            {
                throw new ArgumentNullException(nameof(value));
            }

            this.commandDataConfigurator.Configure(value);

            this.mainUIElement.CommandBindings.Add(value.RoutedCommand.CreateCommandBinding());
        }

        private void AttachInternal(object element, ICommandData value)
        {
            if (value is null)
            {
                throw new ArgumentNullException(nameof(value));
            }

            this.commandDataConfigurator.Configure(value);

            foreach (UIElement i in ProcessElement(element))
            {
                i.CommandBindings.Add(value.RoutedCommand.CreateCommandBinding());
            }
        }

        private void DetachInternal(ICommandData value)
        {
            if (this.mainUIElement == null)
            {
                lock (this.pendingControls)
                {
                    this.pendingControls.Remove(value);
                }
            }
            else
            {
                for (int i = 0; i < this.mainUIElement.CommandBindings.Count; i++)
                {
                    if (this.mainUIElement.CommandBindings[i].Command == value.RoutedCommand)
                    {
                        this.mainUIElement.CommandBindings.RemoveAt(i);
                        break;
                    }
                }
            }
        }

        private void DetachInternal(object element, ICommandData value)
        {
#pragma warning disable S3267 // Loops should be simplified with "LINQ" expressions
            foreach (UIElement element2 in ProcessElement(element))
            {
                for (int i = 0; i < element2.CommandBindings.Count; i++)
                {
                    if (element2.CommandBindings[i].Command == value.RoutedCommand)
                    {
                        element2.CommandBindings.RemoveAt(i);
                        break;
                    }
                }
            }
#pragma warning restore S3267 // Loops should be simplified with "LINQ" expressions
        }

        public void Attach(ICommandData value)
        {
            if (value is null)
            {
                throw new ArgumentNullException(nameof(value));
            }

            if (this.mainUIElement == null)
            {
                lock (this.pendingControls)
                {
                    this.pendingControls.Add(value);
                }
            }
            else
            {
                this.AttachInternal(value);
            }
        }

        public void Attach(object element, ICommandData value)
        {
            if (element is null)
            {
                throw new ArgumentNullException(nameof(element));
            }

            if (value is null)
            {
                throw new ArgumentNullException(nameof(value));
            }

            this.AttachInternal(element, value);
        }

        public void AttachViewModel(object element, object viewModel)
        {
            if (element == null)
            {
                throw new ArgumentNullException(nameof(element));
            }

            if (viewModel == null)
            {
                throw new ArgumentNullException(nameof(viewModel));
            }

            var commandDataType = typeof(ICommandData);
            var attachAttributeType = typeof(AttachAttribute);
            foreach (var property in viewModel.GetType().GetProperties(System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Public))
            {
                if (property.CanRead &&
                    commandDataType.IsAssignableFrom(property.PropertyType) &&
                    property.GetCustomAttributes(attachAttributeType, true).Length > 0)
                {
                    var value = property.GetValue(viewModel) as PresentationData.ControlData;
                    if (value != null)
                    {
                        this.Attach(element, value);
                    }
                }
            }

            if (viewModel is IAttachableViewModel attachableViewModel)
            {
                attachableViewModel.Attach(element);
            }
        }

        public void Detach(ICommandData value)
        {
            if (value == null)
            {
                return;
            }

            this.DetachInternal(value);
        }

        public void Detach(object element, ICommandData value)
        {
            if (element == null)
            {
                throw new ArgumentNullException(nameof(element));
            }

            if (value == null)
            {
                return;
            }

            this.DetachInternal(element, value);
        }

        public void DetachViewModel(object element, object viewModel)
        {
            if (element == null)
            {
                throw new ArgumentNullException(nameof(element));
            }

            if (viewModel == null)
            {
                throw new ArgumentNullException(nameof(viewModel));
            }

            var commandDataType = typeof(ICommandData);
            var attachAttributeType = typeof(AttachAttribute);
            foreach (var property in viewModel.GetType().GetProperties(System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Public))
            {
                if (property.CanRead &&
                    commandDataType.IsAssignableFrom(property.PropertyType) &&
                    property.GetCustomAttributes(attachAttributeType, true).Length > 0)
                {
                    var value = property.GetValue(viewModel) as PresentationData.ControlData;
                    if (value != null)
                    {
                        this.Detach(element, value);
                    }
                }
            }

            if (viewModel is IAttachableViewModel attachableViewModel)
            {
                attachableViewModel.Detach(element);
            }
        }

        public void Initialize(UIElement mainUIElement)
        {
            this.mainUIElement = mainUIElement;

            if (this.mainUIElement != null)
            {
                lock (this.pendingControls)
                {
                    foreach (var i in this.pendingControls)
                    {
                        this.AttachInternal(i);
                    }
                }

                this.pendingControls.Clear();
            }
        }
    }
}
