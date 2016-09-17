using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace MCSVision_UWP_OCR
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
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
