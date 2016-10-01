using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace MCSVision_UWP_OCR
{
    public sealed partial class MainPage : Page
    {
        public MainPage()
        {
            this.InitializeComponent();
            MainFrame.Navigate(typeof (MainFrame));
        }

        private void HamburgerButton_Click(object sender, RoutedEventArgs e)
        {
            MySplitView.IsPaneOpen = !MySplitView.IsPaneOpen;
        }

        private void MenuHome_Click(object sender, RoutedEventArgs e)
        {
            MainFrame.Navigate(typeof(MainFrame));
        }

        private void MenuVideoAnalyse_Click(object sender, RoutedEventArgs e)
        {
            MainFrame.Navigate(typeof(AnalyzeVideo));
        }

        private void MenuDescribeVideo_Click(object sender, RoutedEventArgs e)
        {
            MainFrame.Navigate(typeof(DescribeVideo));
        }
    }
}
