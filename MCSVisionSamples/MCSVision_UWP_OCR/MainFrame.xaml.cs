using System;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;
using Windows.Media.MediaProperties;
using Windows.Storage.Streams;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using MCSVision.Model;
using DeviceServices;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace MCSVision_UWP_OCR
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
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
        
            //DisplayInformation.AutoRotationPreferences = DisplayOrientations.Landscape;
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
            await CameraExecute();
        }

        public async Task CameraExecute()
        {
            await _wbService.InitializeCameraAsync();
            await _wbService.StartCameraPreviewAsync();
            var b = await _wbService.StartFaceDetectionAsync(500);

            //if (_wbService.IsInitialized && await _wbService.StartFaceDetectionAsync(300))
            //{
            //    _wbService.FaceDetectionEffect.FaceDetected += OnFaceDetected;
            //}
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
                    PlainText.Text = plainText;
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
