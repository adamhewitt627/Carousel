using Microsoft.UI.Xaml.Controls;
using System;
using System.Collections.Generic;
using Windows.Foundation;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;

namespace KFCarousel
{
    public sealed class CarouselRepeater : ItemsRepeater, IScrollSnapPointsInfo
    {
        public CarouselRepeater()
        {
            Layout = new StackLayout { Orientation = Orientation.Horizontal };
            EffectiveViewportChanged += CarouselRepeater_EffectiveViewportChanged;
            ElementPrepared += CarouselRepeater_ElementPrepared;
        }

        private static readonly DependencyProperty ViewportProperty = DependencyProperty.Register(nameof(Viewport), typeof(Size), typeof(CarouselRepeater), new PropertyMetadata(Size.Empty));
        public Size Viewport
        {
            get => (Size)GetValue(ViewportProperty);
            set => SetValue(ViewportProperty, value);
        }

        private void CarouselRepeater_EffectiveViewportChanged(FrameworkElement sender, EffectiveViewportChangedEventArgs args)
        {
            var vp = args.MaxViewport;
            Viewport = new Size(Math.Round(vp.Width), Math.Round(vp.Height));
        }

        private void CarouselRepeater_ElementPrepared(ItemsRepeater sender, ItemsRepeaterElementPreparedEventArgs args)
        {
            if (!(args.Element is FrameworkElement))
                return;

            BindingOperations.SetBinding(args.Element, WidthProperty, new Binding
            {
                Source = this,
                Path = new PropertyPath(nameof(Viewport)),
                Converter = new ItemWidthConverter(),
            });
        }

        class ItemWidthConverter : IValueConverter
        {
            public object Convert(object value, Type targetType, object parameter, string language)
            {
                if (!(value is Size size) || double.IsInfinity(size.Width))
                    return 0d;
                
                return size.Width * 0.9;
            }

            public object ConvertBack(object value, Type targetType, object parameter, string language) => throw new NotSupportedException();
        }

        #region IScrollSnapPointsInfo

        public IReadOnlyList<float> GetIrregularSnapPoints(Orientation orientation, SnapPointsAlignment alignment) => new List<float>();

        public float GetRegularSnapPoints(Orientation orientation, SnapPointsAlignment alignment, out float offset)
        {
            offset = 0;
            if (orientation == Orientation.Vertical)
                return 0f;

            return (float)Math.Round(Viewport.Width * 0.9);
        }

        public bool AreHorizontalSnapPointsRegular => true;
        public bool AreVerticalSnapPointsRegular => true;

#pragma warning disable CS0067  //The event is never used
        public event EventHandler<object> HorizontalSnapPointsChanged;
        public event EventHandler<object> VerticalSnapPointsChanged;
#pragma warning restore CS0067

        #endregion IScrollSnapPointsInfo
    }
#if false
    public sealed class CarouselRepeater : ItemsRepeater, IScrollSnapPointsInfo
    {
        public CarouselRepeater()
        {
            Layout = new CarouselLayout();
            BindingOperations.SetBinding(Layout, CarouselLayout.ViewportProperty, new Binding
            {
                Source = this,
                Path = new PropertyPath(nameof(Viewport)),
            });

            EffectiveViewportChanged += CarouselRepeater_EffectiveViewportChanged;
        }

        private static readonly DependencyProperty ViewportProperty = DependencyProperty.Register(nameof(Viewport), typeof(Size), typeof(CarouselRepeater), new PropertyMetadata(Size.Empty));
        public Size Viewport
        {
            get => (Size)GetValue(ViewportProperty);
            set => SetValue(ViewportProperty, value);
        }

        private void CarouselRepeater_EffectiveViewportChanged(FrameworkElement sender, EffectiveViewportChangedEventArgs args)
        {
            var vp = args.MaxViewport;
            Viewport = new Size(Math.Round(vp.Width), Math.Round(vp.Height));
        }



        private class CarouselLayout : NonVirtualizingLayout
        {
            public static readonly DependencyProperty ViewportProperty = DependencyProperty.Register(nameof(Viewport), typeof(Size), typeof(CarouselLayout),
                new PropertyMetadata(Size.Empty, (o, e) => ((CarouselLayout)o).InvalidateMeasure()));
            public Size Viewport
            {
                get => (Size)GetValue(ViewportProperty);
                set => SetValue(ViewportProperty, value);
            }

            protected override Size MeasureOverride(NonVirtualizingLayoutContext context, Size availableSize)
            {
                if (availableSize.IsEmpty || Viewport.IsEmpty)
                    return new Size(0, 0);

                var itemWidth = Math.Round(Viewport.Width * 0.9);
                var itemSize = new Size(itemWidth, availableSize.Height);

                var children = context.Children;
                foreach (var element in children)
                    element.Measure(itemSize);

                var desiredSize = new Size(
                    width: children.Count * itemWidth + Math.Round(Viewport.Width * .05) * 2,
                    height: availableSize.Height
                );
                return desiredSize;
            }

            protected override Size ArrangeOverride(NonVirtualizingLayoutContext context, Size finalSize)
            {
                if (Viewport.IsEmpty)
                    return new Size(0, 0);

                var gutter = Math.Round(Viewport.Width * .05);
                var itemWidth = Math.Round(Viewport.Width * 0.9);
                var children = context.Children;

                var x = gutter;
                foreach (var element in children)
                {
                    element.Arrange(new Rect(x, 0, itemWidth, finalSize.Height));
                    x += itemWidth;
                }
                x += gutter;

                return new Size(x, finalSize.Height);
            }
        }
    }
#endif
}
