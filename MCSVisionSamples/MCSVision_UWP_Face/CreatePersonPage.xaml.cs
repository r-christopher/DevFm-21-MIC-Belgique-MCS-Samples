using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Microsoft.ProjectOxford.Face;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using Windows.Storage.Streams;
using DeviceServices;
using System.Diagnostics;
using Windows.UI.ViewManagement;
using Windows.Media.MediaProperties;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace MCSVision_UWP_Face
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class CreatePersonPage : Page
    {
        IFaceServiceClient _faceServiceClient = new FaceServiceClient(AppConfig.FaceSubscriptionKey);
        private WebcamService _wbService;

        public CreatePersonPage()
        {
            this.InitializeComponent();
            _wbService = new WebcamService
            {
                CaptureElement = CaptureElement
            };

            StartVideoPreview();

            Unloaded += CreatePersonPage_Unloaded;

            Loaded += CreatePersonPage_Loaded;
            ApplicationView.GetForCurrentView().SetPreferredMinSize(new Size(200, 100));
            ApplicationView.PreferredLaunchViewSize = new Size(1000, 500);
            ApplicationView.PreferredLaunchWindowingMode = ApplicationViewWindowingMode.PreferredLaunchViewSize;
        }

        private void CreatePersonPage_Loaded(object sender, RoutedEventArgs e)
        {
          
        }

        private async void CreatePersonPage_Unloaded(object sender, RoutedEventArgs e)
        {
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
                    bool failed = false;
                    await _wbService.MediaCapture.CapturePhotoToStreamAsync(ImageEncodingProperties.CreateJpeg(), stream);
                    await stream.FlushAsync();
                    stream.Seek(0);

                    var detectResult = await _faceServiceClient.DetectAsync(stream.AsStreamForRead());

                    Guid[] faces = detectResult.Select(p => p.FaceId).ToArray();

                    var identifyResult = await _faceServiceClient.IdentifyAsync(AppConfig.PersonGroupID, faces);

                    if ((identifyResult != null && identifyResult.First().Candidates.Any()) && !failed)
                    {
                        ErrorMessage.Text = "This Person already exist";
                    }
                    else
                    {
                        Debug.WriteLine("3. Create Person");
                        string personName = FirstName.Text + " " + LastName.Text;
                        
                        var createPersonResult = await _faceServiceClient.CreatePersonAsync(AppConfig.PersonGroupID, personName);
                        var addPersistedFaceResult = await _faceServiceClient.AddPersonFaceAsync(AppConfig.PersonGroupID, createPersonResult.PersonId, stream.AsStreamForRead());
                        await _faceServiceClient.TrainPersonGroupAsync(AppConfig.PersonGroupID);
                        ErrorMessage.Text = $"{personName} has been added.";
                    }
                }
                catch (FaceAPIException ex)
                {
                    Debug.WriteLine(ex.ErrorMessage);
                }
            }
        }
    }
}
