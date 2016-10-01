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
using Windows.Media.SpeechRecognition;
using Windows.Storage.Streams;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using DeviceServices;
using MCSVision.Model;
using MCSVision_UWP_OCR.Model;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace MCSVision_UWP_OCR
{
    public sealed partial class DescribeVideo : Page
    {
        private WebcamService _wbService;
        private McsVisionService _mcsVisionService;
        private SpeechRecognizer _speechRecognizer;

        public DescribeVideo()
        {
            this.InitializeComponent();
            _mcsVisionService = new McsVisionService();
            _wbService = new WebcamService{CaptureElement = CaptureElement };
            _speechRecognizer = new SpeechRecognizer();

            StartVideoPreview();

            _speechRecognizer.ContinuousRecognitionSession.ResultGenerated += ContinuousRecognitionSession_ResultGenerated;  
            Unloaded += AnalyzeVideo_Unloaded;
        }

        private async void AnalyzeVideo_Unloaded(object sender, RoutedEventArgs e)
        {
            Debug.WriteLine("The camera is tuning off...");
            await _wbService.CleanUpAsync();
            await StopLinstening();
        }

        public async void StartVideoPreview()
        {
            Debug.WriteLine("The video is starting...");
            await _wbService.InitializeCameraAsync();

            await _wbService.StartCameraPreviewAsync();
            StartListening();
            
            // Getting the platform family
            var platformFamily = Windows.System.Profile.AnalyticsInfo.VersionInfo.DeviceFamily;

            Debug.WriteLine($"Platform Family : {platformFamily}");
            if (platformFamily == "Windows.Mobile")
                RotateVideo.Angle = 90;
        }

        private async void StartListening()
        {
            await ListeningDescribe();
        }
        private async void StartAnalyzing()
        {
            using (var stream = new InMemoryRandomAccessStream())
            {
                try
                {
                    await _wbService.MediaCapture.CapturePhotoToStreamAsync(ImageEncodingProperties.CreateJpeg(), stream);
                    await stream.FlushAsync();
                    stream.Seek(0);

                    var plainText = await _mcsVisionService.DescribePicture(stream.AsStreamForRead());
                   
                    await Dispatcher.RunAsync(CoreDispatcherPriority.Normal,
                        async () =>
                        {
                            PlainText.Text = plainText;
                            await TtsService.SayAsync(plainText);
                        }
                    );
                }
                catch (Exception ex)
                {
                    Debug.WriteLine(ex.Message);
                }
            }
        }
        private void ContinuousRecognitionSession_ResultGenerated(Windows.Media.SpeechRecognition.SpeechContinuousRecognitionSession sender, 
            Windows.Media.SpeechRecognition.SpeechContinuousRecognitionResultGeneratedEventArgs args)
        {
            Debug.WriteLine($"Speech detected: {args.Result.Text}");
            try
            {
                if (args.Result.Constraint.Tag == "constraint_describe")
                {
                    StartAnalyzing();
                }
            }
            catch (Exception e)
            {
                Debug.WriteLine($"{e.Message}");
            }

        }

        public async Task StopLinstening()
        {
            if (_speechRecognizer != null)
            {
                if (await CancelContinuousRecognitionAsync())
                {
                    _speechRecognizer.Dispose();
                    _speechRecognizer = null;
                }
            }
        }
        public async Task ListeningDescribe()
        {
            await AddConstraintAsync(ConstraintsDictionnary.ConstraintDescribe);
            await StartContinuousRecognitionAsync();
        }

        /// <summary>
        /// Asynchronously start continuous speech recognition 
        /// </summary>
        public async Task StartContinuousRecognitionAsync()
        {
            await _speechRecognizer.ContinuousRecognitionSession.StartAsync();
        }

        /// <summary>
        /// Asynchronously add constraint in speech recognizer
        /// </summary>
        public async Task<SpeechRecognitionCompilationResult> AddConstraintAsync(ISpeechRecognitionConstraint constraint, bool compile = true)
        {
            if (_speechRecognizer.State != SpeechRecognizerState.Idle)
            {
                Debug.WriteLine($"STTService Failed to add constraint because of speech recognizer state : {_speechRecognizer.State}");
                return null;
            }

            _speechRecognizer.Constraints.Add(constraint);

            if (!compile) return null;

            return await _speechRecognizer.CompileConstraintsAsync();
        }
        /// <summary>
        /// Asynchronously cancal continuous speech recognition
        /// </summary>
        public async Task<bool> CancelContinuousRecognitionAsync()
        {
            try
            {
                await _speechRecognizer.ContinuousRecognitionSession.CancelAsync();
                return true;
            }
            catch (Exception)
            {
                Debug.WriteLine("STTService failed to cancel continuous recognition");
                return false;
            }
        }
    }
}
