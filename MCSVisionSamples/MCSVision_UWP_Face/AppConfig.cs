using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MCSVision_UWP_Face
{
    public static class AppConfig
    {
#if DEBUG
        public static string FaceSubscriptionKey = "2527f87fa29e420f8cac1c25dcaae94f";
        public static string EmotionSubscriptionKey = "ffd34b159d794856984aba291760f5fd";
        public static string VisionSubscriptionKey = "c05886327fde4626af92d7ece995a46b";
#else
        public static string FaceSubscriptionKey = "2527f87fa29e420f8cac1c25dcaae94f";
        public static string VisionSubscriptionKey = "c05886327fde4626af92d7ece995a46b";
        public static string EmotionSubscriptionKey = "ffd34b159d794856984aba291760f5fd";
#endif

        public const string PersonGroupID = "0a43aaaa-d085-4dad-91ef-00caa08f4544";
    }
}
