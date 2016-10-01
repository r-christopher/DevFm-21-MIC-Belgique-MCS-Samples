using Windows.Media.SpeechRecognition;

namespace MCSVision_UWP_OCR.Model
{
    public class ConstraintsDictionnary
    {

        public static ISpeechRecognitionConstraint ConstraintDescribe => new SpeechRecognitionListConstraint(new[]
        {
            "Describe me", "Discribe mi", "Décris moi", "Décris", "Tell me", "dis moi", "que vois tu", "what do you see"
        },
        "constraint_describe");
    }
}
