using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Emgu.CV;
//
using Emgu.CV.CvEnum;
//Emgu Cv imports
using Emgu.CV.Structure;
//
using Emgu.CV.UI;
//


namespace License_Plate_Recognition
{
    class PossiblePlate
    {
        public Mat imgPlate;
        public Mat imgGrayscale;

        public Mat imgThresh;

        public RotatedRect rrLocationOfPlateInScene;

        public string strChars;
        // constructor '''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
        public PossiblePlate()
        {
            //initialize values
            imgPlate = new Mat();
            imgGrayscale = new Mat();
            imgThresh = new Mat();

            rrLocationOfPlateInScene = new RotatedRect();

            strChars = "";
        }
    }
}
