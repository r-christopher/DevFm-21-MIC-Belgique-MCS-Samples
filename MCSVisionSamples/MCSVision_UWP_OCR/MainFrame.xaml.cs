using System;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;
using Windows.Graphics.Display;
using Windows.Media.MediaProperties;
using Windows.Storage.Streams;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using MCSVision.Model;
using DeviceServices;

namespace MCSVision_UWP_OCR
{
    public sealed partial class MainFrame : Page
    {
        private WebcamService _wbService;
        private McsVisionService _mcsVisionService;
        public MainFrame()
        {
            this.InitializeComponent();
            _mcsVisionService = new McsVisionService();
            _wbService = new WebcamService
            {
                CaptureElement = CaptureElement
            };

            StartVideoPreview();
        
            Unloaded += MainFrame_Unloaded;
        }

        private async void MainFrame_Unloaded(object sender, RoutedEventArgs e)
        {
            Debug.WriteLine("The camera is tuning off...");
            await _wbService.CleanUpAsync();
        }

        public async void StartVideoPreview()
        {
            Debug.WriteLine("The camera is starting...");
            await _wbService.InitializeCameraAsync();
            await _wbService.StartCameraPreviewAsync();
            // Getting the platform family
            var platformFamily = Windows.System.Profile.AnalyticsInfo.VersionInfo.DeviceFamily;
           
            if (platformFamily == "Windows.Mobile")
            {
                await _wbService.SetFocusAsync(100);
                RotateVideo.Angle = 90;
            }
                
        }

        private async void SendButton_OnClick(object sender, RoutedEventArgs e)
        {
            using (var stream = new InMemoryRandomAccessStream())
            {
                try
                {
                    await _wbService.MediaCapture.CapturePhotoToStreamAsync(ImageEncodingProperties.CreateJpeg(), stream);
                    await stream.FlushAsync();
                    stream.Seek(0);
                    QuestionMark.Visibility = Visibility.Visible;
                    var plainText = await _mcsVisionService.OpticalCharacterRecognition(stream.AsStreamForRead());
                    QuestionMark.Visibility = Visibility.Collapsed;
                    PlainText.Text = plainText.Text;
                    PositionText.Text = plainText.Position;
                }
                catch (Exception ex)
                {
                    Frame.Navigate(typeof (MainFrame));
                    Debug.WriteLine(ex.Message);
                }
            }
        }
    }
}
