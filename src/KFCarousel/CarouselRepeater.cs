using Microsoft.UI.Xaml.Controls;
using System;
using System.Collections.Generic;
using System.Linq;
using Windows.Foundation;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;

namespace KFCarousel
{
    internal class CarouselRepeater : ItemsRepeater, IScrollSnapPointsInfo
    {
        public CarouselRepeater()
        {
            Layout = new UniformGridLayout
            {
                MinItemWidth = 400,
                MinItemHeight = 300,
            };

            EffectiveViewportChanged += CarouselRepeater_EffectiveViewportChanged;
            ElementPrepared += CarouselRepeater_ElementPrepared;
        }

        private static readonly DependencyProperty ViewportProperty =
            DependencyProperty.Register(nameof(Viewport), typeof(Rect), typeof(CarouselRepeater), new PropertyMetadata(Rect.Empty));
        private Rect Viewport
        {
            get { return (Rect)GetValue(ViewportProperty); }
            set { SetValue(ViewportProperty, value); }
        }

        private void CarouselRepeater_EffectiveViewportChanged(FrameworkElement sender, EffectiveViewportChangedEventArgs args)
        {
            Viewport = args.MaxViewport;
            if (Layout is UniformGridLayout layout)
                layout.MinItemWidth = Math.Round(args.MaxViewport.Width * 0.9);

            HorizontalSnapPointsChanged?.Invoke(this, EventArgs.Empty);
        }

        private void CarouselRepeater_ElementPrepared(ItemsRepeater sender, ItemsRepeaterElementPreparedEventArgs args)
        {
            HorizontalSnapPointsChanged?.Invoke(this, EventArgs.Empty);
        }

        #region IScrollSnapPointsInfo

        public IReadOnlyList<float> GetIrregularSnapPoints(Orientation orientation, SnapPointsAlignment alignment)
        {
            return alignment switch
            {
                SnapPointsAlignment.Center => center().ToList(),
                _ => new List<float>(), //TODO
            };

            IEnumerable<float> center()
            {
                for (var i = 0; i < ItemsSourceView.Count; i++)
                {
                    if (!(TryGetElement(i) is UIElement e))
                        continue;

                    var center = i switch
                    {
                        0 => (float)Viewport.Width / 2,
                        int j when j == ItemsSourceView.Count - 1 =>
                            e.ActualOffset.X + e.ActualSize.X - (float)Viewport.Width / 2,
                        _ => e.ActualOffset.X + (e.ActualSize.X / 2),
                    };

                    yield return center;
                }
            }
        }

        public float GetRegularSnapPoints(Orientation orientation, SnapPointsAlignment alignment, out float offset) => offset = 0f;
        public bool AreHorizontalSnapPointsRegular => false;
        public bool AreVerticalSnapPointsRegular => false;

#pragma warning disable CS0067
        public event EventHandler<object> HorizontalSnapPointsChanged;
        public event EventHandler<object> VerticalSnapPointsChanged;
#pragma warning restore CS0067

        #endregion IScrollSnapPointsInfo
    }
}
