using MCSVision_UWP_Face.Model;
using Microsoft.ProjectOxford.Vision;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace MCSVision_UWP_Face.ServiceHelpers
{
    public class VisionService
    {
        private HttpClient _httpClient;
        private IVisionServiceClient _vServiceClient;
        public VisionService()
        {
            _httpClient = new HttpClient();
            _vServiceClient = new VisionServiceClient(AppConfig.VisionSubscriptionKey);
        }

        public VisionService(string key)
        {
            _httpClient = new HttpClient();
            _vServiceClient = new VisionServiceClient(key);
        }

        //public async Task<ImageResults> OpticalCharacterRecognition(Stream picture)
        //{
        //    ImageResults response = new ImageResults();
        //    StringBuilder position = new StringBuilder();
        //    StringBuilder text = new StringBuilder();
        //    try
        //    {
        //        Debug.WriteLine("The picture is under processing...");

        //        var result = await _vServiceClient.RecognizeTextAsync(picture, "unk", false);

        //        result.Regions.Select(p => p.Lines).ToList().ForEach(p =>
        //        {
        //            p.ToList().ForEach(pp =>
        //            {
        //                pp.Words.ToList().ForEach(ppp =>
        //                {
        //                    position.Append(ppp.Text);
        //                    position.Append(" - ");
        //                    position.Append($"Left : {ppp.Rectangle.Left}, Top : {ppp.Rectangle.Top}, Width : {ppp.Rectangle.Width}, Height : {ppp.Rectangle.Height}");
        //                    position.AppendLine("");
        //                });
        //            });
        //        });

        //        result.Regions.Select(p => p.Lines).ToList().ForEach(p =>
        //        {
        //            p.ToList().ForEach(pp =>
        //            {
        //                pp.Words.ToList().ForEach(ppp =>
        //                {
        //                    text.Append(ppp.Text);
        //                    text.Append(" ");
        //                });
        //            });
        //        });
        //    }
        //    catch (HttpRequestException)
        //    {
        //        Debug.WriteLine("HTTP REQUEST exception");
        //    }
        //    response.TextTop = position.ToString();
        //    response.TextBottom = text.ToString();
        //    return response;
        //}

        public async Task<string> AnalyzePicture(Stream picture)
        {
            StringBuilder sb = new StringBuilder();
            try
            {
                Debug.WriteLine("The picture is under processing...");

                var result = await _vServiceClient.AnalyzeImageAsync(picture, new string[] { "Tags", "Adult", "Categories", "Description" });

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

        //public async Task<ImageResults> DescribePicture(Stream picture)
        //{
        //    ImageResults response = new ImageResults();
        //    StringBuilder tag = new StringBuilder();
        //    StringBuilder captions = new StringBuilder();
        //    StringBuilder cat = new StringBuilder();
        //    StringBuilder adult = new StringBuilder();


        //    try
        //    {
        //        Debug.WriteLine("The picture is under processing...");

        //        //var result = await _vServiceClient.DescribeAsync(picture);
        //        IEnumerable<VisualFeature> visual = new List<VisualFeature>() { VisualFeature.Description , VisualFeature.Tags , VisualFeature.Categories , VisualFeature.Adult };
        //        var result = await _vServiceClient.AnalyzeImageAsync(picture, visual);


        //        result.Tags?.Select(p => p).ToList().ForEach(ppp =>
        //        {
        //            tag.Append(ppp.Name);
        //            tag.Append(" ");
        //        });
        //        adult.Append(result.Adult?.IsAdultContent);

        //        result.Categories?.Select(p => p).ToList().ForEach(ppp =>
        //        {
        //            cat.Append(ppp.Name);
        //            cat.AppendLine(ppp.Detail.ToString());
        //            cat.AppendLine(" ");
        //        });

        //        result.Description.Captions?.ToList().ForEach(p =>
        //        {
        //            captions.Append($"I am {Math.Round(p.Confidence * 100)}% sure that I see ");

        //            captions.Append(p.Text);

        //            captions.Append(". ");
        //        });

        //    }
        //    catch (HttpRequestException)
        //    {
        //        Debug.WriteLine("HTTP REQUEST exception");
        //    }
        //    catch(Exception e)
        //    {
        //        Debug.WriteLine(e.Message);

        //    }

        //    response.Adult = adult.ToString();
        //    response.TextCat = cat.ToString();
        //    response.TextBottom = captions.ToString();
        //    response.TextTop = tag.ToString();

        //    return response;
        //}
    }
}