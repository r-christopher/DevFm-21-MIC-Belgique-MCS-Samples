using MCSVision_UWP_Face.Model.Emotion;
using Microsoft.ProjectOxford.Face.Contract;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Data;

namespace MCSVision_UWP_Face.Converters
{
    public class EmotionsToStringConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            var scores = double.Parse(value.ToString());
            scores = scores * 100;
            var str = string.Empty;
            str = Math.Round(scores, 2).ToString();
            return str;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}
