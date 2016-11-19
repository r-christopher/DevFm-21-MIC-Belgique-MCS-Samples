using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using MCSVision_UWP_OCR.Model;
using Microsoft.ProjectOxford.Vision;

namespace MCSVision_UWP_OCR.ServiceHelpers
{
    public class McsVisionService
    {
        private HttpClient _httpClient;
        private IVisionServiceClient _vServiceClient;
        public McsVisionService()
        {
            _httpClient = new HttpClient();
            _vServiceClient = new VisionServiceClient(AppConfig.SubscriptionKey);
        }

        public McsVisionService(string key)
        {
            _httpClient = new HttpClient();
            _vServiceClient = new VisionServiceClient(key);
        }

        public async Task<TextResult> OpticalCharacterRecognition(Stream picture)
        {
            TextResult response = new TextResult();
            StringBuilder position = new StringBuilder();
            StringBuilder text = new StringBuilder();
            try
            {
                Debug.WriteLine("The picture is under processing...");

                var result = await _vServiceClient.RecognizeTextAsync(picture, "unk", false);

                result.Regions.Select(p => p.Lines).ToList().ForEach(p =>
                {
                    p.ToList().ForEach(pp =>
                    {
                        pp.Words.ToList().ForEach(ppp =>
                        {
                            position.Append(ppp.Text);
                            position.Append(" - ");
                            position.Append($"Left : {ppp.Rectangle.Left}, Top : {ppp.Rectangle.Top}, Width : {ppp.Rectangle.Width}, Height : {ppp.Rectangle.Height}");
                            position.AppendLine("");
                        });
                    });
                });

                result.Regions.Select(p => p.Lines).ToList().ForEach(p =>
                {
                    p.ToList().ForEach(pp =>
                    {
                        pp.Words.ToList().ForEach(ppp =>
                        {
                            text.Append(ppp.Text);
                            text.Append(" ");
                        });
                    });
                });
            }
            catch (HttpRequestException)
            {
                Debug.WriteLine("HTTP REQUEST exception");
            }
            response.TextTop = position.ToString();
            response.TextBottom = text.ToString();
            return response;
        }

        public async Task<string> AnalyzePicture(Stream picture)
        {
            StringBuilder sb = new StringBuilder();
            try
            {
                Debug.WriteLine("The picture is under processing...");

                var result = await _vServiceClient.AnalyzeImageAsync(picture, new string[] { "Tags" });

                result.Tags.Select(p => p.Name).ToList().ForEach(p =>
                {
                    sb.Append(p);
                    sb.Append(" ");
                });
            }
            catch (HttpRequestException)
            {
                Debug.WriteLine("HTTP REQUEST exception");
            }

            return sb.ToString();
        }

        public async Task<TextResult> DescribePicture(Stream picture)
        {
            TextResult response = new TextResult();
            StringBuilder top = new StringBuilder();
            StringBuilder bottom = new StringBuilder();
            try
            {
                Debug.WriteLine("The picture is under processing...");

                var result = await _vServiceClient.DescribeAsync(picture);

                result.Description.Tags.Select(p => p).ToList().ForEach(ppp =>
                {
                    top.Append(ppp);
                    top.Append(" ");
                });

                result.Description.Captions.ToList().ForEach(p =>
                {
                    bottom.Append($"I am {Math.Round(p.Confidence*100)}% sure that I see ");

                    bottom.Append(p.Text);

                    bottom.Append(". ");
                });

            }
            catch (HttpRequestException)
            {
                Debug.WriteLine("HTTP REQUEST exception");
            }

            response.TextBottom = bottom.ToString();
            response.TextTop = top.ToString();

            return response;
        }
    }
}
