using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Windows;
using System.Windows.Controls;

namespace Colosoft.Presentation.Behaviors
{
    public static class GridViewColumnResizeBehavior
    {
        public static readonly DependencyProperty WidthProperty =
            DependencyProperty.RegisterAttached(
                "Width",
                typeof(string),
                typeof(GridViewColumnResizeBehavior),
                new PropertyMetadata(OnSetWidthCallback));

        private static readonly DependencyProperty GridViewColumnResizeBehaviorProperty =
            DependencyProperty.RegisterAttached(
                "GridViewColumnResizeBehavior",
                typeof(GridViewColumnResizeBehavior2),
                typeof(GridViewColumnResizeBehavior),
                null);

        public static readonly DependencyProperty EnabledProperty =
            DependencyProperty.RegisterAttached(
                "Enabled",
                typeof(bool),
                typeof(GridViewColumnResizeBehavior),
                new PropertyMetadata(OnSetEnabledCallback));

        private static readonly DependencyProperty ListViewResizeBehaviorProperty =
            DependencyProperty.RegisterAttached(
                "ListViewResizeBehaviorProperty",
                typeof(ListViewResizeBehavior),
                typeof(GridViewColumnResizeBehavior),
                null);

        public static string GetWidth(DependencyObject obj)
        {
            if (obj is null)
            {
                throw new ArgumentNullException(nameof(obj));
            }

            return (string)obj.GetValue(WidthProperty);
        }

        public static void SetWidth(DependencyObject obj, string value)
        {
            if (obj is null)
            {
                throw new ArgumentNullException(nameof(obj));
            }

            obj.SetValue(WidthProperty, value);
        }

        public static bool GetEnabled(DependencyObject obj)
        {
            if (obj is null)
            {
                throw new ArgumentNullException(nameof(obj));
            }

            return (bool)obj.GetValue(EnabledProperty);
        }

        public static void SetEnabled(DependencyObject obj, bool value)
        {
            if (obj is null)
            {
                throw new ArgumentNullException(nameof(obj));
            }

            obj.SetValue(EnabledProperty, value);
        }

        private static void OnSetWidthCallback(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs e)
        {
            var element = dependencyObject as GridViewColumn;
            if (element != null)
            {
                GridViewColumnResizeBehavior2 behavior = GetOrCreateBehavior(element);
                behavior.Width = e.NewValue as string;
            }
            else
            {
                Console.Error.WriteLine($"Error: Expected type GridViewColumn but found {dependencyObject.GetType().Name}");
            }
        }

        private static void OnSetEnabledCallback(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs e)
        {
            var element = dependencyObject as ListView;
            if (element != null)
            {
#pragma warning disable CA2000 // Dispose objects before losing scope
                ListViewResizeBehavior behavior = GetOrCreateBehavior(element);
#pragma warning restore CA2000 // Dispose objects before losing scope
                behavior.Enabled = (bool)e.NewValue;
            }
            else
            {
                Console.Error.WriteLine("Error: Expected type ListView but found " + dependencyObject.GetType().Name);
            }
        }

        private static ListViewResizeBehavior GetOrCreateBehavior(ListView element)
        {
            var behavior = element.GetValue(GridViewColumnResizeBehaviorProperty) as ListViewResizeBehavior;
            if (behavior == null)
            {
                behavior = new ListViewResizeBehavior(element);
                element.SetValue(ListViewResizeBehaviorProperty, behavior);
            }

            return behavior;
        }

        private static GridViewColumnResizeBehavior2 GetOrCreateBehavior(GridViewColumn element)
        {
            var behavior = element.GetValue(GridViewColumnResizeBehaviorProperty) as GridViewColumnResizeBehavior2;
            if (behavior == null)
            {
                behavior = new GridViewColumnResizeBehavior2(element);
                element.SetValue(GridViewColumnResizeBehaviorProperty, behavior);
            }

            return behavior;
        }

        private sealed class GridViewColumnResizeBehavior2
        {
            private readonly GridViewColumn element;

            public string Width { get; set; }

            public bool IsStatic
            {
                get { return this.StaticWidth >= 0; }
            }

            public double StaticWidth
            {
                get
                {
                    double result;
                    return double.TryParse(this.Width, out result) ? result : -1;
                }
            }

            public double Percentage
            {
                get
                {
                    if (!this.IsStatic)
                    {
                        return this.Mulitplier * 100;
                    }

                    return 0;
                }
            }

            public double Mulitplier
            {
                get
                {
                    if (this.Width == "*" || this.Width == "1*")
                    {
                        return 1;
                    }

                    if (this.Width.EndsWith("*", StringComparison.Ordinal))
                    {
                        double perc;
                        if (double.TryParse(this.Width.Substring(0, this.Width.Length - 1), out perc))
                        {
                            return perc;
                        }
                    }

                    return 1;
                }
            }

            public GridViewColumnResizeBehavior2(GridViewColumn element)
            {
                this.element = element;
            }

#pragma warning disable S3218 // Inner class members should not shadow outer class "static" or type members
            public void SetWidth(double allowedSpace, double totalPercentage)
#pragma warning restore S3218 // Inner class members should not shadow outer class "static" or type members
            {
                if (this.IsStatic)
                {
                    this.element.Width = this.StaticWidth;
                }
                else
                {
                    double width = allowedSpace * (this.Percentage / totalPercentage);
                    this.element.Width = width;
                }
            }
        }

        private sealed class ListViewResizeBehavior : IDisposable
        {
            private const int Margin = 25;
            private const long RefreshTime = Timeout.Infinite;
            private const long Delay = 500;

            private readonly ListView element;
            private readonly Timer timer;

            public ListViewResizeBehavior(ListView element)
            {
                if (element == null)
                {
                    throw new ArgumentNullException(nameof(element));
                }

                this.element = element;
                element.Loaded += this.OnLoaded;

                Action resizeAndEnableSize = () =>
                {
                    this.Resize();
                    this.element.SizeChanged += this.OnSizeChanged;
                };

                this.timer = new Timer(
                    x => Application.Current.Dispatcher.BeginInvoke(resizeAndEnableSize),
                    null,
                    Delay,
                    RefreshTime);
            }

            public bool Enabled { get; set; }

            private static IEnumerable<GridViewColumnResizeBehavior2> GridViewColumnResizeBehaviors(GridView gv)
            {
                foreach (GridViewColumn t in gv.Columns)
                {
                    var gridViewColumnResizeBehavior =
                        t.GetValue(GridViewColumnResizeBehaviorProperty) as GridViewColumnResizeBehavior2;
                    if (gridViewColumnResizeBehavior != null)
                    {
                        yield return gridViewColumnResizeBehavior;
                    }
                }
            }

            private static double GetAllocatedSpace(GridView gv)
            {
                double totalWidth = 0;
                foreach (GridViewColumn t in gv.Columns)
                {
                    var gridViewColumnResizeBehavior =
                        t.GetValue(GridViewColumnResizeBehaviorProperty) as GridViewColumnResizeBehavior2;
                    if (gridViewColumnResizeBehavior != null)
                    {
                        if (gridViewColumnResizeBehavior.IsStatic)
                        {
                            totalWidth += gridViewColumnResizeBehavior.StaticWidth;
                        }
                    }
                    else
                    {
                        totalWidth += t.ActualWidth;
                    }
                }

                return totalWidth;
            }

            private void OnLoaded(object sender, System.Windows.RoutedEventArgs e)
            {
                this.element.SizeChanged += this.OnSizeChanged;
            }

            private void OnSizeChanged(object sender, SizeChangedEventArgs e)
            {
                if (e.WidthChanged)
                {
                    this.element.SizeChanged -= this.OnSizeChanged;
                    this.timer.Change(Delay, RefreshTime);
                }
            }

            private void Resize()
            {
                if (this.Enabled)
                {
                    double totalWidth = this.element.ActualWidth;
                    var gv = this.element.View as GridView;
                    if (gv != null)
                    {
                        double allowedSpace = totalWidth - GetAllocatedSpace(gv);
                        allowedSpace = allowedSpace - Margin;
                        double totalPercentage = GridViewColumnResizeBehaviors(gv).Sum(x => x.Percentage);
                        foreach (GridViewColumnResizeBehavior2 behavior in GridViewColumnResizeBehaviors(gv))
                        {
                            behavior.SetWidth(allowedSpace, totalPercentage);
                        }
                    }
                }
            }

            public void Dispose()
            {
                this.timer.Dispose();
            }
        }
    }
}
