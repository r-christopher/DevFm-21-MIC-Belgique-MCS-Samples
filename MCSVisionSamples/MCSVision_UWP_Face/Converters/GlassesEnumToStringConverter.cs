using Microsoft.ProjectOxford.Face.Contract;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Data;

namespace MCSVision_UWP_Face.Converters
{
    public class GlassesEnumToStringConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            var glasses = (Glasses)value;

            var str = string.Empty;

            switch (glasses)
            {
                case Glasses.ReadingGlasses:
                    str = "Reading Glasses";
                    break;
                case Glasses.Sunglasses:
                    str = "Sun Glasses";
                    break;
                case Glasses.SwimmingGoggles:
                    str = "Swimming Goggles";
                    break;
                default:
                    str = "No Glasses";
                    break;
            }

            return str;
        }



        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}
