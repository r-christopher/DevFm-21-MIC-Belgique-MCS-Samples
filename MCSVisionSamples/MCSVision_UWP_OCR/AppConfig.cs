using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MCSVision_UWP_OCR
{
    public static class AppConfig
    {
#if DEBUG
        public static string SubscriptionKey = "< your subsciption key>";
#else
        public static string SubscriptionKey = "< your subsciption key>";
#endif
    }
}
