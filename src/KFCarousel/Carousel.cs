using Microsoft.UI.Xaml.Controls;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace KFCarousel
{
    public sealed class Carousel : ItemsControl
    {
        private ItemsRepeater PART_ItemsRepeater;

        public Carousel() => DefaultStyleKey = typeof(Carousel);

        protected override void OnApplyTemplate()
        {
            bool load<T>(ref T target, string name) where T : DependencyObject => (target = GetTemplateChild(name) as T) is object;

            base.OnApplyTemplate();
            load(ref PART_ItemsRepeater, nameof(PART_ItemsRepeater));
        }
    }
}
