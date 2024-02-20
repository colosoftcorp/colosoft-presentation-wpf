using System;
using System.Windows;

namespace Colosoft.Presentation.Behaviors
{
    public static class ToolBarBehavior
    {
        public static readonly DependencyProperty ConfigureControlDataProperty =
            DependencyProperty.RegisterAttached(
                "ConfigureControlData",
                typeof(bool),
                typeof(ToolBarBehavior),
                new PropertyMetadata(ConfigureControlDataChanged));

        private static readonly DependencyProperty ConfigureControlDataStateProperty =
            DependencyProperty.RegisterAttached(
                "ConfigureControlDataState",
                typeof(ConfigureControlDataState),
                typeof(ToolBarBehavior),
                new PropertyMetadata());

        public static IAttacher Attacher { get; set; }

        public static void SetConfigureControlData(DependencyObject owner, bool value)
        {
            if (owner is null)
            {
                throw new ArgumentNullException(nameof(owner));
            }

            owner.SetValue(ConfigureControlDataProperty, value);
        }

        public static bool GetConfigureControlData(DependencyObject owner)
        {
            if (owner is null)
            {
                throw new ArgumentNullException(nameof(owner));
            }

            var value = owner.GetValue(ConfigureControlDataProperty);
            if (value == null)
            {
                return false;
            }

            return (bool)value;
        }

        private static void ConfigureControlDataChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var toolBar = (System.Windows.Controls.ToolBar)d;

            if (toolBar.GetValue(ConfigureControlDataStateProperty) == null)
            {
#pragma warning disable CA2000 // Dispose objects before losing scope
                toolBar.SetValue(ConfigureControlDataStateProperty, new ConfigureControlDataState(toolBar, Attacher));
#pragma warning restore CA2000 // Dispose objects before losing scope
            }
        }

        private sealed class ConfigureControlDataState : IDisposable
        {
            private readonly IAttacher attacher;
            private readonly System.Windows.Controls.ToolBar toolBar;

            public ConfigureControlDataState(
                System.Windows.Controls.ToolBar toolBar,
                IAttacher attacher)
            {
                this.attacher = attacher;
                this.toolBar = toolBar;

                foreach (var i in this.toolBar.Items)
                {
                    var controlData = i as PresentationData.ControlData;

                    if (controlData != null && this.attacher != null)
                    {
                        this.attacher.Attach(this.toolBar, controlData);
                    }
                }

                ((System.Collections.Specialized.INotifyCollectionChanged)toolBar.Items).CollectionChanged += this.ToolBarItemsCollectionChanged;
            }

            ~ConfigureControlDataState()
            {
                this.Dispose();
            }

            private void ToolBarItemsCollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
            {
                if (e.Action == System.Collections.Specialized.NotifyCollectionChangedAction.Add)
                {
                    foreach (var i in e.NewItems)
                    {
                        var controlData = i as PresentationData.ControlData;

                        if (controlData != null)
                        {
                            this.attacher.Attach(this.toolBar, controlData);
                        }
                    }
                }
                else if (e.Action == System.Collections.Specialized.NotifyCollectionChangedAction.Remove)
                {
                    // Percorre os itens removidos
                    foreach (var i in e.OldItems)
                    {
                        var controlData = i as PresentationData.ControlData;

                        if (controlData != null)
                        {
                            this.attacher.Detach(this.toolBar, controlData);
                        }
                    }
                }
                else if (e.Action == System.Collections.Specialized.NotifyCollectionChangedAction.Reset)
                {
                    foreach (var i in this.toolBar.Items)
                    {
                        var controlData = i as PresentationData.ControlData;

                        if (controlData != null)
                        {
                            this.attacher.Attach(this.toolBar, controlData);
                        }
                    }
                }
            }

            public void Dispose()
            {
                GC.SuppressFinalize(this);
            }
        }
    }
}
