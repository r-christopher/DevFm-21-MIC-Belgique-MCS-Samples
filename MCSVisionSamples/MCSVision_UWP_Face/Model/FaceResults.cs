
using Microsoft.ProjectOxford.Face.Contract;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MCSVision_UWP_Face.Model
{
    public class FaceResults
    {
        public FaceAttributes FaceAttributes { get; set; }

        public Person PersonResult { get; set; }
    }
}
