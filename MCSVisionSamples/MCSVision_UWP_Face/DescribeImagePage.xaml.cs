using MCSVision_UWP_Face.Model;
using DeviceServices;
using MCSVision_UWP_Face.Model.Emotion;
using MCSVision_UWP_Face.Services;
using Microsoft.ProjectOxford.Face;
using Microsoft.ProjectOxford.Face.Contract;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.Media.MediaProperties;
using Windows.Storage.Streams;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace MCSVision_UWP_Face
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class DescribeImagePage : Page
    {
        IFaceServiceClient _faceServiceClient = new FaceServiceClient(AppConfig.FaceSubscriptionKey);
        ComputerVisionService _visionService = new ComputerVisionService(AppConfig.VisionSubscriptionKey);
        EmotionService _emotionService = new EmotionService(AppConfig.EmotionSubscriptionKey);
        List<FaceResults> Results = new List<FaceResults>();
        List<ImageResult> ImgResults = new List<ImageResult>();
        List<EmotionResult> FaceEmotionsResults = new List<EmotionResult>();
        Person _personResult;

        private WebcamService _wbService;
        public DescribeImagePage()
        {
            this.InitializeComponent();

            _wbService = new WebcamService
            {
                CaptureElement = CaptureElement
            };

            TagsImageResults.ItemsSource = ImgResults;
            FaceResults.ItemsSource = Results;
            FaceEmotionResults.ItemsSource = FaceEmotionsResults;

            StartVideoPreview();
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
                    ProgressLoadRing.IsActive = true;
                    Results = new List<FaceResults>();
                    ImgResults = new List<ImageResult>();
                    FaceEmotionsResults = new List<EmotionResult>();
                    IEnumerable<FaceAttributeType> types = new List<FaceAttributeType> { FaceAttributeType.Age, FaceAttributeType.FacialHair, FaceAttributeType.Gender, FaceAttributeType.Glasses, FaceAttributeType.HeadPose, FaceAttributeType.Smile };

                    await _wbService.MediaCapture.CapturePhotoToStreamAsync(ImageEncodingProperties.CreateJpeg(), stream);
                    await stream.FlushAsync();
                    stream.Seek(0);
                    var pictureImage = stream.CloneStream();
                    var pictureEmotion = stream.CloneStream();

                    try
                    {
                        var faceResult = await _faceServiceClient.DetectAsync(stream.AsStreamForRead(), true, false, types);

                        Guid[] faces = faceResult.Select(p => p.FaceId).ToArray();

                        Debug.WriteLine("IdentifyAsync :");
                        var identifyResult = await _faceServiceClient.IdentifyAsync(AppConfig.PersonGroupID, faces);

                        if (identifyResult.Any())
                        {
                            foreach (var item in identifyResult)
                            {
                                Debug.WriteLine("GetPersonAsync :");
                                _personResult = await _faceServiceClient.GetPersonAsync(AppConfig.PersonGroupID, item.Candidates.First().PersonId);
                                Results.Add(new FaceResults
                                {
                                    PersonResult = _personResult,
                                    FaceAttributes = faceResult.Where(p => p.FaceId == item.FaceId).First().FaceAttributes
                                });
                            }
                        }
                    }
                    catch(FaceAPIException fex)
                    {
                        Debug.WriteLine(fex.ErrorMessage);
                    }

                    Task.Run(async () =>
                    {
                        try
                        {
                            var imgRes = await _visionService.AnalyseImageStreamAsync(pictureImage.AsStreamForRead());
                            if (_personResult != null)
                            {                                                                
                                imgRes.Description.Captions?.Select(p => p.Text).ToList().ForEach(pp =>
                                {

                                    pp = pp.Replace("a man", _personResult.Name);
                                    pp = pp.Replace("a woman", _personResult.Name);
                                    pp = pp.Replace("a person", _personResult.Name);
                                    pp = pp.Replace("person", _personResult.Name);
                                });

                            }
                            ImgResults.Add(imgRes);
                            await Dispatcher.RunAsync(CoreDispatcherPriority.Normal,
                                   () =>
                                   {
                                       try
                                       {
                                           TagsImageResults.ItemsSource = ImgResults;
                                           ProgressLoadRing.IsActive = false;
                                       }
                                       catch (Exception exx)
                                       {
                                           Debug.WriteLine(exx.Message);

                                       }
                                   }
                             );
                        }
                        catch (Exception exx)
                        {
                            Debug.WriteLine(exx.Message);
                        }

                    });

                    Task.Run(async () =>
                    {
                        var res = await _emotionService.RecognizeEmotionsFromImageStreamAsync(pictureEmotion.AsStreamForRead());
                        FaceEmotionsResults.Add(res.First());

                        await Dispatcher.RunAsync(CoreDispatcherPriority.Normal,
                               () =>
                               {
                                   FaceEmotionResults.ItemsSource = FaceEmotionsResults;                                   
                               }
                         );
                    });

                    await Dispatcher.RunAsync(CoreDispatcherPriority.Normal,
                        () =>
                        {
                            FaceResults.ItemsSource = Results;
                        }
                    );
                }
                catch (Exception ex)
                {
                    Debug.WriteLine(ex.Message);
                }
            }
        }
    }
}
