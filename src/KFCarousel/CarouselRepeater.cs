using Microsoft.UI.Xaml.Controls;
using System;
using System.Collections.Generic;
using Windows.Foundation;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;

namespace KFCarousel
{
    public sealed class CarouselRepeater : ItemsRepeater, IScrollSnapPointsInfo
    {
        public CarouselRepeater()
        {
            Layout = new CarouselLayout();
            EffectiveViewportChanged += CarouselRepeater_EffectiveViewportChanged;
        }

        private void CarouselRepeater_EffectiveViewportChanged(FrameworkElement sender, EffectiveViewportChangedEventArgs args)
        {
            if (Layout is CarouselLayout layout)
                layout.Viewport = args.MaxViewport;
        }

        #region IScrollSnapPointsInfo

        public IReadOnlyList<float> GetIrregularSnapPoints(Orientation orientation, SnapPointsAlignment alignment) => new List<float>();

        public float GetRegularSnapPoints(Orientation orientation, SnapPointsAlignment alignment, out float offset)
        {
            offset = 0;
            if (orientation == Orientation.Vertical)
                return 0f;

            var viewport = ((CarouselLayout)Layout).Viewport;
            offset = (float)Math.Round(viewport.Width * 0.5);
            return (float)Math.Round(viewport.Width * 0.9);
        }

        public bool AreHorizontalSnapPointsRegular => true;
        public bool AreVerticalSnapPointsRegular => true;

#pragma warning disable CS0067  //The event is never used
        public event EventHandler<object> HorizontalSnapPointsChanged;  
        public event EventHandler<object> VerticalSnapPointsChanged;
#pragma warning restore CS0067

        #endregion IScrollSnapPointsInfo

        private class CarouselLayout : VirtualizingLayout
        {
            public static readonly DependencyProperty ViewportProperty = DependencyProperty.Register(nameof(Viewport), typeof(Rect), typeof(CarouselLayout),
                new PropertyMetadata(Rect.Empty, (o, e) =>
                {
                    var viewport = (Rect)e.NewValue;
                    o.SetValue(ItemWidthProperty, Math.Round(viewport.Width * 0.9));
                }));
            public Rect Viewport
            {
                get => (Rect)GetValue(ViewportProperty);
                set => SetValue(ViewportProperty, value);
            }

            private static readonly DependencyProperty ItemWidthProperty = DependencyProperty.Register(nameof(ItemWidth), typeof(double), typeof(CarouselLayout),
                new PropertyMetadata(double.PositiveInfinity, (o, e) => ((CarouselLayout)o).InvalidateMeasure()));
            private double ItemWidth
            {
                get => (double)GetValue(ItemWidthProperty);
                set => SetValue(ItemWidthProperty, value);
            }

            protected override Size MeasureOverride(VirtualizingLayoutContext context, Size availableSize)
            {
                if (double.IsInfinity(ItemWidth))
                    return new Size(0, 0);

                /*TODO:
                 *  - Virtualizing
                 *  - Wrapping (it's a layout, so why not?)
                 */
                var itemSize = new Size(ItemWidth, availableSize.Height);

                for (int i = 0; i < context.ItemCount; i++)
                {
                    var element = context.GetOrCreateElementAt(i);
                    element.Measure(itemSize);
                }

                var desiredSize = new Size(
                    width: context.ItemCount * ItemWidth + Math.Round(Viewport.Width * .05) * 2,
                    height: availableSize.Height
                );
                return desiredSize;
            }

            protected override Size ArrangeOverride(VirtualizingLayoutContext context, Size finalSize)
            {
                if (double.IsInfinity(ItemWidth))
                    return finalSize;

                var gutter = Math.Round(Viewport.Width * .05);
                
                var x = gutter;
                for (int i = 0; i < context.ItemCount; i++)
                {
                    var element = context.GetOrCreateElementAt(i);
                    element.Arrange(new Rect(x, 0, ItemWidth, finalSize.Height));
                    x += ItemWidth;
                }
                x += gutter;

                return new Size(x, finalSize.Height);
            }
        }
    }
}
