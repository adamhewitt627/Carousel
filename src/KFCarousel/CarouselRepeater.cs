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
            Layout = new StackLayout
            {
                Orientation = Orientation.Horizontal,
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
            HorizontalSnapPointsChanged?.Invoke(this, EventArgs.Empty);
        }

        private void CarouselRepeater_ElementPrepared(ItemsRepeater sender, ItemsRepeaterElementPreparedEventArgs args) 
            => HorizontalSnapPointsChanged?.Invoke(this, EventArgs.Empty);

        #region IScrollSnapPointsInfo

        public IReadOnlyList<float> GetIrregularSnapPoints(Orientation orientation, SnapPointsAlignment alignment)
        {
            if (orientation == Orientation.Vertical)
                return new List<float>();

            var result = (alignment switch
            {
                SnapPointsAlignment.Near => near(),
                SnapPointsAlignment.Center => center(),
                SnapPointsAlignment.Far => far(),
                _ => Enumerable.Empty<float>(), //TODO
            }).Distinct().OrderBy(x => x).ToList();

            return result;

            #region Factories 

            IEnumerable<float> near()
            {
                yield return 0;

                for (var i = 0; i < ItemsSourceView.Count; i++)
                {
                    if (TryGetElement(i) is UIElement e)
                        yield return e.ActualOffset.X;
                }

                yield return ActualSize.X - (float)Viewport.Width;
            }

            IEnumerable<float> center()
            {
                var center = (float)Viewport.Width / 2;
                yield return center;

                for (var i = 0; i < ItemsSourceView.Count; i++)
                {
                    if (TryGetElement(i) is UIElement e)
                        yield return e.ActualOffset.X + (e.ActualSize.X / 2);
                }

                yield return ActualSize.X - center;
            }

            IEnumerable<float> far()
            {
                yield return (float)Viewport.Width;

                for (var i = 0; i < ItemsSourceView.Count; i++)
                {
                    if (TryGetElement(i) is UIElement e)
                        yield return e.ActualOffset.X + e.ActualSize.X;
                }

                yield return ActualSize.X;
            }

            #endregion
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
