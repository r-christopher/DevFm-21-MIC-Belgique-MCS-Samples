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
        public static string SubscriptionKey = "2d51e19ca9e048ad86e8ed14199673fe";
#else
        public static string SubscriptionKey = "c05886327fde4626af92d7ece995a46b";
#endif
    }
}
