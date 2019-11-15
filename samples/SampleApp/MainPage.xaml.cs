using Windows.UI.Xaml.Controls;

namespace SampleApp
{
    public sealed partial class MainPage : Page
    {
        public MainPage()
        {
            InitializeComponent();
            carousel.ItemsSource = new[] { "First", "Second", "Third", "Fourth", "Fifth", "Sixth", "Seventh", "Eighth" };
        }
    }
}
