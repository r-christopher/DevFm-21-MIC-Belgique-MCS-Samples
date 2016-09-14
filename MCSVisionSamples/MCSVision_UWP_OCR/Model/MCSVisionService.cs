using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Microsoft.ProjectOxford.Vision;

namespace MCSVision.Model
{
    public class McsVisionService
    {
        private HttpClient _httpClient;
        private IVisionServiceClient _vServiceClient;
        public McsVisionService()
        {
            _httpClient = new HttpClient();
            _vServiceClient = new VisionServiceClient("2d51e19ca9e048ad86e8ed14199673fe");
        }
        public async Task<string> OpticalCharacterRecognition(Stream picture)
        {
            StringBuilder sb = new StringBuilder();
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
                            sb.Append(ppp.Text);
                            sb.Append(" ");
                        });
                    });
                });
            }
            catch (HttpRequestException)
            {
                Debug.WriteLine("HTTP REQUEST exception");
            }

            return sb.ToString();
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
    }
}
