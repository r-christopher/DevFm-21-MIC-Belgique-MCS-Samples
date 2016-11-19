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
    public class ListOfStringToOneStringConverter : IValueConverter
    {
        /// <summary>
        /// Converts a list of strings to one string containing all strings 
        /// separated with a comma.
        /// </summary>
        /// <param name="value"></param>
        /// <param name="targetType"></param>
        /// <param name="parameter"></param>
        /// <param name="culture"></param>
        /// <returns></returns>
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            Debug.WriteLine("Aa");
            var list = value as List<string>;

            Debug.WriteLine("Aa" + list.First());
            var str = string.Empty;

            if (list != null)
            {
                foreach (var s in list)
                {
                    str += s + ", " ;
                }
            }

            return str;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
