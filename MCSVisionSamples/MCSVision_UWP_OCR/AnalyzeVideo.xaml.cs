using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Media.MediaProperties;
using Windows.Storage.Streams;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using DeviceServices;
using MCSVision.Model;

namespace MCSVision_UWP_OCR
{
    public sealed partial class AnalyzeVideo : Page
    {
        private WebcamService _wbService;
        private McsVisionService _mcsVisionService;
        private const int LIMIT = 300;
        public AnalyzeVideo()
        {
            InitializeComponent();
            _mcsVisionService = new McsVisionService();
            _wbService = new WebcamService
            {
                CaptureElement = CaptureElement
            };

            StartVideoPreview();
            Unloaded += AnazlyzeVideo_Unloaded;
            
        }
        private async void AnazlyzeVideo_Unloaded(object sender, RoutedEventArgs e)
        {
            Debug.WriteLine("The camera is tuning off...");
            await _wbService.CleanUpAsync();
        }

        public async void StartVideoPreview()
        {
            Debug.WriteLine("The video is starting...");
            await _wbService.InitializeCameraAsync();

            await _wbService.StartCameraPreviewAsync();
            StartAnalyzing();
            // Getting the platform family
            var platformFamily = Windows.System.Profile.AnalyticsInfo.VersionInfo.DeviceFamily;

            Debug.WriteLine($"Platform Family : {platformFamily}");
            if (platformFamily == "Windows.Mobile")
                RotateVideo.Angle = 90;
        }

        private async void StartAnalyzing()
        {
            while (true)
            {
                await Task.Delay(LIMIT);
                using (var stream = new InMemoryRandomAccessStream())
                {
                    try
                    {
                        await _wbService.MediaCapture.CapturePhotoToStreamAsync(ImageEncodingProperties.CreateJpeg(), stream);
                        await stream.FlushAsync();
                        stream.Seek(0);

                        var plainText = await _mcsVisionService.AnalyzePicture(stream.AsStreamForRead());

                        PlainText.Text = plainText;
                    }
                    catch (Exception ex)
                    {
                        Debug.WriteLine(ex.Message);
                    }
                }
            }
        }
    }
}
