using System;
using System.Collections;
using System.Windows;
using System.Windows.Documents;
using System.Windows.Media;

namespace Colosoft.Presentation.Controls
{
    public class FrameworkElementAdorner : Adorner
    {
        private readonly FrameworkElement child;

        private readonly AdornerPlacement horizontalAdornerPlacement = AdornerPlacement.Inside;
        private readonly AdornerPlacement verticalAdornerPlacement = AdornerPlacement.Inside;

        private readonly double offsetX;
        private readonly double offsetY;

        public FrameworkElementAdorner(FrameworkElement adornerChildElement, FrameworkElement adornedElement)
            : base(adornedElement)
        {
            this.child = adornerChildElement;

            this.AddLogicalChild(adornerChildElement);
            this.AddVisualChild(adornerChildElement);
        }

        public FrameworkElementAdorner(
            FrameworkElement adornerChildElement,
            FrameworkElement adornedElement,
            AdornerPlacement horizontalAdornerPlacement,
            AdornerPlacement verticalAdornerPlacement,
            double offsetX,
            double offsetY)
            : base(adornedElement)
        {
            if (adornedElement is null)
            {
                throw new ArgumentNullException(nameof(adornedElement));
            }

            this.child = adornerChildElement;
            this.horizontalAdornerPlacement = horizontalAdornerPlacement;
            this.verticalAdornerPlacement = verticalAdornerPlacement;
            this.offsetX = offsetX;
            this.offsetY = offsetY;

            adornedElement.SizeChanged += new SizeChangedEventHandler(this.AdornedElement_SizeChanged);

            this.AddLogicalChild(adornerChildElement);
            this.AddVisualChild(adornerChildElement);
        }

        private void AdornedElement_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            this.InvalidateMeasure();
        }

        public double PositionX { get; set; } = double.NaN;

        public double PositionY { get; set; } = double.NaN;

        protected override Size MeasureOverride(Size constraint)
        {
            this.child.Measure(constraint);
            return this.child.DesiredSize;
        }

        private double DetermineX()
        {
            switch (this.child.HorizontalAlignment)
            {
                case HorizontalAlignment.Left:
                    {
                        if (this.horizontalAdornerPlacement == AdornerPlacement.Outside)
                        {
                            return -this.child.DesiredSize.Width + this.offsetX;
                        }
                        else
                        {
                            return this.offsetX;
                        }
                    }

                case HorizontalAlignment.Right:
                    {
                        if (this.horizontalAdornerPlacement == AdornerPlacement.Outside)
                        {
                            double adornedWidth = this.AdornedElement.ActualWidth;
                            return adornedWidth + this.offsetX;
                        }
                        else
                        {
                            double adornerWidth = this.child.DesiredSize.Width;
                            double adornedWidth = this.AdornedElement.ActualWidth;
                            double x = adornedWidth - adornerWidth;
                            return x + this.offsetX;
                        }
                    }

                case HorizontalAlignment.Center:
                    {
                        double adornerWidth = this.child.DesiredSize.Width;
                        double adornedWidth = this.AdornedElement.ActualWidth;
                        double x = (adornedWidth / 2) - (adornerWidth / 2);
                        return x + this.offsetX;
                    }

                case HorizontalAlignment.Stretch:
                    {
                        return 0.0;
                    }
            }

            return 0.0;
        }

        /// <summary>
        /// Determine the Y coordinate of the child.
        /// </summary>
        private double DetermineY()
        {
            switch (this.child.VerticalAlignment)
            {
                case VerticalAlignment.Top:
                    {
                        if (this.verticalAdornerPlacement == AdornerPlacement.Outside)
                        {
                            return -this.child.DesiredSize.Height + this.offsetY;
                        }
                        else
                        {
                            return this.offsetY;
                        }
                    }

                case VerticalAlignment.Bottom:
                    {
                        if (this.verticalAdornerPlacement == AdornerPlacement.Outside)
                        {
                            double adornedHeight = this.AdornedElement.ActualHeight;
                            return adornedHeight + this.offsetY;
                        }
                        else
                        {
                            double adornerHeight = this.child.DesiredSize.Height;
                            double adornedHeight = this.AdornedElement.ActualHeight;
                            double x = adornedHeight - adornerHeight;
                            return x + this.offsetY;
                        }
                    }

                case VerticalAlignment.Center:
                    {
                        double adornerHeight = this.child.DesiredSize.Height;
                        double adornedHeight = this.AdornedElement.ActualHeight;
                        double x = (adornedHeight / 2) - (adornerHeight / 2);
                        return x + this.offsetY;
                    }

                case VerticalAlignment.Stretch:
                    {
                        return 0.0;
                    }
            }

            return 0.0;
        }

        private double DetermineWidth()
        {
            if (!double.IsNaN(this.PositionX))
            {
                return this.child.DesiredSize.Width;
            }

            switch (this.child.HorizontalAlignment)
            {
                case HorizontalAlignment.Left:
                    return this.child.DesiredSize.Width;
                case HorizontalAlignment.Right:
                    return this.child.DesiredSize.Width;
                case HorizontalAlignment.Center:
                    return this.child.DesiredSize.Width;
                case HorizontalAlignment.Stretch:
                    return this.AdornedElement.ActualWidth;
            }

            return 0.0;
        }

        private double DetermineHeight()
        {
            if (!double.IsNaN(this.PositionY))
            {
                return this.child.DesiredSize.Height;
            }

            switch (this.child.VerticalAlignment)
            {
                case VerticalAlignment.Top:
                    return this.child.DesiredSize.Height;

                case VerticalAlignment.Bottom:
                    return this.child.DesiredSize.Height;

                case VerticalAlignment.Center:
                    return this.child.DesiredSize.Height;

                case VerticalAlignment.Stretch:
                    return this.AdornedElement.ActualHeight;
            }

            return 0.0;
        }

        protected override Size ArrangeOverride(Size finalSize)
        {
            double x = this.PositionX;
            if (double.IsNaN(x))
            {
                x = this.DetermineX();
            }

            double y = this.PositionY;
            if (double.IsNaN(y))
            {
                y = this.DetermineY();
            }

            double adornerWidth = this.DetermineWidth();
            double adornerHeight = this.DetermineHeight();
            this.child.Arrange(new Rect(x, y, adornerWidth, adornerHeight));
            return finalSize;
        }

        protected override int VisualChildrenCount
        {
            get { return 1; }
        }

        protected override Visual GetVisualChild(int index)
        {
            return this.child;
        }

        protected override IEnumerator LogicalChildren
        {
            get
            {
                ArrayList list = new ArrayList();
                list.Add(this.child);
                return list.GetEnumerator();
            }
        }

        public void DisconnectChild()
        {
            this.RemoveLogicalChild(this.child);
            this.RemoveVisualChild(this.child);
        }

        public new FrameworkElement AdornedElement
        {
            get
            {
                return (FrameworkElement)base.AdornedElement;
            }
        }
    }
}
