using Prism.Regions;
using System;
using System.ComponentModel;
using System.Windows;

namespace Colosoft.Presentation.Behaviors
{
    public static class RegionPopupBehaviors
    {
        public static readonly DependencyProperty CreatePopupRegionWithNameProperty =
            DependencyProperty.RegisterAttached(
                "CreatePopupRegionWithName",
                typeof(string),
                typeof(RegionPopupBehaviors),
                new PropertyMetadata(CreatePopupRegionWithNamePropertyChanged));

        public static readonly DependencyProperty ContainerWindowStyleProperty =
          DependencyProperty.RegisterAttached("ContainerWindowStyle", typeof(Style), typeof(RegionPopupBehaviors), null);

        public static IRegionManager RegionManager { get; set; }

        public static string GetCreatePopupRegionWithName(DependencyObject owner)
        {
            if (owner is null)
            {
                throw new ArgumentNullException(nameof(owner));
            }

            return owner.GetValue(CreatePopupRegionWithNameProperty) as string;
        }

        public static void SetCreatePopupRegionWithName(DependencyObject owner, string value)
        {
            if (owner is null)
            {
                throw new ArgumentNullException(nameof(owner));
            }

            owner.SetValue(CreatePopupRegionWithNameProperty, value);
        }

        public static Style GetContainerWindowStyle(DependencyObject owner)
        {
            if (owner is null)
            {
                throw new ArgumentNullException(nameof(owner));
            }

            return owner.GetValue(ContainerWindowStyleProperty) as Style;
        }

        public static void SetContainerWindowStyle(DependencyObject owner, Style style)
        {
            if (owner is null)
            {
                throw new ArgumentNullException(nameof(owner));
            }

            owner.SetValue(ContainerWindowStyleProperty, style);
        }

        public static void RegisterNewPopupRegion(DependencyObject owner, string regionName)
        {
            if (RegionManager != null)
            {
                var region = new Region();
                DialogActivationBehavior behavior;
#if SILVERLIGHT
                behavior = new PopupDialogActivationBehavior();
#else
                behavior = new WindowDialogActivationBehavior();
#endif
                behavior.HostControl = owner;

                region.Behaviors.Add(DialogActivationBehavior.BehaviorKey, behavior);
                RegionManager.Regions.Add(regionName, region);
            }
        }

        private static void CreatePopupRegionWithNamePropertyChanged(DependencyObject hostControl, DependencyPropertyChangedEventArgs e)
        {
            if (IsInDesignMode(hostControl))
            {
                return;
            }

            RegisterNewPopupRegion(hostControl, e.NewValue as string);
        }

        private static bool IsInDesignMode(DependencyObject element)
        {
            // Due to a known issue in Cider, GetIsInDesignMode attached property value is not enough to know if it's in design mode.
            return DesignerProperties.GetIsInDesignMode(element) || Application.Current == null
                   || Application.Current.GetType() == typeof(Application);
        }
    }
}
