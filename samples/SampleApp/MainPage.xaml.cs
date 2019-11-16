using System.Collections.ObjectModel;
using System.Linq;
using Windows.UI.Xaml.Controls;

namespace SampleApp
{
    public sealed partial class MainPage : Page
    {
        public MainPage()
        {
            InitializeComponent();

            //var source = new ObservableCollection<string>(new[] { "First", "Second", "Third", "Fourth", "Fifth", "Sixth", "Seventh" });
            var source = new ObservableCollection<int>(Enumerable.Range(1, 17));
            carousel.ItemsSource = source;

            Loaded += (o, e) => source.Add(source.Count + 1);
        }
    }
}
