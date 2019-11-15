using Microsoft.UI.Xaml.Controls;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;

namespace KFCarousel
{
    public sealed class Carousel : ItemsControl
    {
        private ItemsRepeater PART_ItemsRepeater;
        private ScrollViewer PART_ScrollViewer;

        public Carousel() => DefaultStyleKey = typeof(Carousel);

        protected override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            load(ref PART_ItemsRepeater, nameof(PART_ItemsRepeater));

            if (load(ref PART_ScrollViewer, nameof(PART_ScrollViewer)))
                PART_ScrollViewer.AddHandler(PointerWheelChangedEvent, new PointerEventHandler(ScrollViewer_PointerWheelChanged), true);

            bool load<T>(ref T target, string name) where T : DependencyObject
            {
                target = GetTemplateChild(name) as T;
                return target is object;
            }
        }
        private void ScrollViewer_PointerWheelChanged(object sender, PointerRoutedEventArgs e)
        {
            if (!(sender is ScrollViewer scrollViewer))
                return;

            var pointer = e.GetCurrentPoint(scrollViewer);
            int delta = pointer.Properties.MouseWheelDelta;

            if ((delta < 0 && scrollViewer.HorizontalOffset >= scrollViewer.ScrollableWidth)
                || (delta > 0 && PART_ScrollViewer.HorizontalOffset == 0))
                e.Handled = false;
        }
    }
}
