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
using Emgu.CV.Util;
using System.Drawing;
//

namespace License_Plate_Recognition
{
    public class PossibleChar
    {
        public VectorOfPoint contour;
        public Rectangle boundingRect;

        public int intCenterX;

        public int intCenterY;
        public double dblDiagonalSize;
        public double dblAspectRatio;

        public int intRectArea;
        // constructor '''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
        public PossibleChar(VectorOfPoint _contour)
        {
            contour = _contour;

            boundingRect = CvInvoke.BoundingRectangle(contour);

            intCenterX = Convert.ToInt32((boundingRect.Left + boundingRect.Right) / 2);
            intCenterY = Convert.ToInt32((boundingRect.Top + boundingRect.Bottom) / 2);

            dblDiagonalSize = Math.Sqrt((Math.Pow(boundingRect.Width, 2)) + (Math.Pow(boundingRect.Height, 2)));

            dblAspectRatio = Convert.ToDouble(boundingRect.Width) / Convert.ToDouble(boundingRect.Height);

            intRectArea = boundingRect.Width * boundingRect.Height;
        }
    }
}
