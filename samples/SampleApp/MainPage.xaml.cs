using System.Collections.ObjectModel;
using Windows.UI.Xaml.Controls;

namespace SampleApp
{
    public sealed partial class MainPage : Page
    {
        public MainPage()
        {
            InitializeComponent();

            var source = new ObservableCollection<string>(new[] { "First", "Second", "Third", "Fourth", "Fifth", "Sixth", "Seventh" });
            carousel.ItemsSource = source;

            Loaded += (o, e) =>
            {
                //source.Add("Eighth");
            };
        }
    }
}
