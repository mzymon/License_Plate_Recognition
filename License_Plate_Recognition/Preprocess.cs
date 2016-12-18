using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections;

using System.Data;
using System.Diagnostics;
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

namespace License_Plate_Recognition
{
    class Preprocess
    {

        // module level variables ''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
        const int GAUSSIAN_BLUR_FILTER_SIZE = 7;
        const int ADAPTIVE_THRESH_BLOCK_SIZE = 19;

        const int ADAPTIVE_THRESH_WEIGHT = 9;
        ///''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
        public static void preprocess(Mat imgOriginal, ref Mat imgGrayscale, ref Mat imgThresh)
        {
            imgGrayscale = extractValue(imgOriginal);
            //extract value channel only from original image to get imgGrayscale

            Mat imgMaxContrastGrayscale = maximizeContrast(imgGrayscale);
            //maximize contrast with top hat and black hat

            Mat imgBlurred = new Mat();

            CvInvoke.GaussianBlur(imgMaxContrastGrayscale, imgBlurred, new Size(GAUSSIAN_BLUR_FILTER_SIZE, GAUSSIAN_BLUR_FILTER_SIZE), 0);
            //gaussian blur

            //adaptive threshold to get imgThresh
            CvInvoke.AdaptiveThreshold(imgBlurred, imgThresh, 255.0, AdaptiveThresholdType.GaussianC, ThresholdType.BinaryInv, ADAPTIVE_THRESH_BLOCK_SIZE, ADAPTIVE_THRESH_WEIGHT);

            MCvScalar tempVal=CvInvoke.Mean(imgBlurred);
            double average = tempVal.V0;
            CvInvoke.Threshold(imgBlurred, imgThresh, 0, 255.0, Emgu.CV.CvEnum.ThresholdType.Otsu);
            CvInvoke.Dilate(imgThresh, imgThresh, null, Point.Empty, 1, BorderType.Default, new MCvScalar(0));
            CvInvoke.Erode(imgThresh, imgThresh,null, Point.Empty, 1, BorderType.Default, new MCvScalar(0));
            CvInvoke.BitwiseNot(imgThresh, imgThresh);
        }

        ///''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
        public static Mat extractValue(Mat imgOriginal)
        {
            Mat imgHSV = new Mat();
            VectorOfMat vectorOfHSVImages = new VectorOfMat();
            Mat imgValue = new Mat();

            CvInvoke.CvtColor(imgOriginal, imgHSV, ColorConversion.Bgr2Hsv);

            CvInvoke.Split(imgHSV, vectorOfHSVImages);

            imgValue = vectorOfHSVImages[2];

            return imgValue;
        }

        ///''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
        public static Mat maximizeContrast(Mat imgGrayscale)
        {
            Mat imgTopHat = new Mat();
            Mat imgBlackHat = new Mat();
            Mat imgGrayscalePlusTopHat = new Mat();
            Mat imgGrayscalePlusTopHatMinusBlackHat = new Mat();

            Mat structuringElement = CvInvoke.GetStructuringElement(ElementShape.Rectangle, new Size(3, 3), new Point(-1, -1));

            CvInvoke.MorphologyEx(imgGrayscale, imgTopHat, MorphOp.Tophat, structuringElement, new Point(-1, -1), 1, BorderType.Default, new MCvScalar());
            CvInvoke.MorphologyEx(imgGrayscale, imgBlackHat, MorphOp.Blackhat, structuringElement, new Point(-1, -1), 1, BorderType.Default, new MCvScalar());

            CvInvoke.Add(imgGrayscale, imgTopHat, imgGrayscalePlusTopHat);
            CvInvoke.Subtract(imgGrayscalePlusTopHat, imgBlackHat, imgGrayscalePlusTopHatMinusBlackHat);

            return imgGrayscalePlusTopHatMinusBlackHat;
        }

    }
}
