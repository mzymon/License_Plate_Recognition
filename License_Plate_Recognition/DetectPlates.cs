using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
//DetectPlates.vb
//
//Emgu CV 3.0.0

//require explicit declaration of variables, this is NOT Python !!
//restrict implicit data type conversions to only widening conversions

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
    static class DetectPlates
    {


        // module level variables ''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
        const double PLATE_WIDTH_PADDING_FACTOR = 1.3;

        const double PLATE_HEIGHT_PADDING_FACTOR = 1.5;
        static MCvScalar SCALAR_WHITE = new MCvScalar(255.0, 255.0, 255.0);

        static MCvScalar SCALAR_RED = new MCvScalar(0.0, 0.0, 255.0);
        ///''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
        public static List<PossiblePlate> detectPlatesInScene(Mat imgOriginalScene)
        {
            List<PossiblePlate> listOfPossiblePlates = new List<PossiblePlate>();
            //this will be the return value

            Mat imgGrayscaleScene = new Mat();
            Mat imgThreshScene = new Mat();
            Mat imgContours = new Mat(imgOriginalScene.Size, DepthType.Cv8U, 3);

            Random random = new Random();

            CvInvoke.DestroyAllWindows();

            // show steps '''''''''''''''''''''''''''''''''
            if ((frmMain.cbShowSteps.Checked == true))
            {
                CvInvoke.Imshow("0", imgOriginalScene);
            }
            // show steps '''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''

            Preprocess.preprocess(imgOriginalScene, imgGrayscaleScene, imgThreshScene);
            //preprocess to get grayscale and threshold images

            // show steps '''''''''''''''''''''''''''''''''
            if ((frmMain.cbShowSteps.Checked == true))
            {
                CvInvoke.Imshow("1a", imgGrayscaleScene);
                CvInvoke.Imshow("1b", imgThreshScene);
            }
            // show steps '''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''

            //find all possible chars in the scene,
            //this function first finds all contours, then only includes contours that could be chars (without comparison to other chars yet)
            List<PossibleChar> listOfPossibleCharsInScene = findPossibleCharsInScene(imgThreshScene);

            // show steps '''''''''''''''''''''''''''''''''
            if ((frmMain.cbShowSteps.Checked == true))
            {
                frmMain.txtInfo.AppendText("step 2 - listOfPossibleCharsInScene.Count = " + listOfPossibleCharsInScene.Count.ToString() + "\r\n");
                //131 with MCLRNF1 image

                VectorOfVectorOfPoint contours = new VectorOfVectorOfPoint();

                foreach (PossibleChar possibleChar in listOfPossibleCharsInScene)
                {
                    contours.Push(possibleChar.contour);
                }

                CvInvoke.DrawContours(imgContours, contours, -1, SCALAR_WHITE);
                CvInvoke.Imshow("2b", imgContours);
            }
            // show steps '''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''

            //given a list of all possible chars, find groups of matching chars
            //in the next steps each group of matching chars will attempt to be recognized as a plate
            List<List<PossibleChar>> listOfListsOfMatchingCharsInScene = findListOfListsOfMatchingChars[listOfPossibleCharsInScene];

            // show steps '''''''''''''''''''''''''''''''''
            if ((frmMain.cbShowSteps.Checked == true))
            {
                frmMain.txtInfo.AppendText("step 3 - listOfListsOfMatchingCharsInScene.Count = " + listOfListsOfMatchingCharsInScene.Count.ToString() + "\r\n");
                //13 with MCLRNF1 image

                imgContours = new Mat(imgOriginalScene.Size, DepthType.Cv8U, 3);

                foreach (List<PossibleChar> listOfMatchingChars in listOfListsOfMatchingCharsInScene)
                {
                    object intRandomBlue = random.Next(0, 256);
                    object intRandomGreen = random.Next(0, 256);
                    object intRandomRed = random.Next(0, 256);

                    VectorOfVectorOfPoint contours = new VectorOfVectorOfPoint();

                    foreach (PossibleChar matchingChar in listOfMatchingChars)
                    {
                        contours.Push(matchingChar.contour);
                    }

                    CvInvoke.DrawContours(imgContours, contours, -1, new MCvScalar(Convert.ToDouble(intRandomBlue), Convert.ToDouble(intRandomGreen), Convert.ToDouble(intRandomRed)));

                }
                CvInvoke.Imshow("3", imgContours);
            }
            // show steps '''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''

            //for each group of matching chars
            foreach (List<PossibleChar> listOfMatchingChars in listOfListsOfMatchingCharsInScene)
            {
                object possiblePlate = extractPlate(imgOriginalScene, listOfMatchingChars);
                //attempt to extract plate

                //if plate was found
                if (((possiblePlate.imgPlate != null)))
                {
                    listOfPossiblePlates.Add(possiblePlate);
                    //add to list of possible plates
                }
            }

            frmMain.txtInfo.AppendText("\r\n" + listOfPossiblePlates.Count.ToString() + " possible plates found" + "\r\n");
            //update text box with # of plates found

            // show steps '''''''''''''''''''''''''''''''''
            if ((frmMain.cbShowSteps.Checked == true))
            {
                frmMain.txtInfo.AppendText("\r\n");
                CvInvoke.Imshow("4a", imgContours);

                for (int i = 0; i <= listOfPossiblePlates.Count - 1; i++)
                {
                    PointF[] ptfRectPoints = new PointF[5];

                    ptfRectPoints = listOfPossiblePlates[i].rrLocationOfPlateInScene.GetVertices();

                    Point pt0 = new Point(Convert.ToInt32(ptfRectPoints[0].X), Convert.ToInt32(ptfRectPoints[0].Y));
                    Point pt1 = new Point(Convert.ToInt32(ptfRectPoints[1].X), Convert.ToInt32(ptfRectPoints[1].Y));
                    Point pt2 = new Point(Convert.ToInt32(ptfRectPoints[2].X), Convert.ToInt32(ptfRectPoints[2].Y));
                    Point pt3 = new Point(Convert.ToInt32(ptfRectPoints[3].X), Convert.ToInt32(ptfRectPoints[3].Y));

                    CvInvoke.Line(imgContours, pt0, pt1, SCALAR_RED, 2);
                    CvInvoke.Line(imgContours, pt1, pt2, SCALAR_RED, 2);
                    CvInvoke.Line(imgContours, pt2, pt3, SCALAR_RED, 2);
                    CvInvoke.Line(imgContours, pt3, pt0, SCALAR_RED, 2);

                    CvInvoke.Imshow("4a", imgContours);
                    frmMain.txtInfo.AppendText("possible plate " + i.ToString() + ", click on any image and press a key to continue . . ." + "\r\n");
                    CvInvoke.Imshow("4b", listOfPossiblePlates[i].imgPlate);
                    CvInvoke.WaitKey(0);
                }
                frmMain.txtInfo.AppendText("\r\n" + "plate detection complete, click on any image and press a key to begin char recognition . . ." + "\r\n" + "\r\n");
                CvInvoke.WaitKey(0);
            }
            // show steps '''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''

            return listOfPossiblePlates;
        }

        ///''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
        public static List<PossibleChar> findPossibleCharsInScene(Mat imgThresh)
        {
            List<PossibleChar> listOfPossibleChars = new List<PossibleChar>();
            //this is the return value

            Mat imgContours = new Mat(imgThresh.Size, DepthType.Cv8U, 3);
            int intCountOfPossibleChars = 0;

            Mat imgThreshCopy = imgThresh.Clone(); 

            VectorOfVectorOfPoint contours = new VectorOfVectorOfPoint();

            CvInvoke.FindContours(imgThreshCopy, contours, null, RetrType.List, ChainApproxMethod.ChainApproxSimple);
            //find all contours

            //for each contour
            for (int i = 0; i <= contours.Size - 1; i++)
            {
                // show steps '''''''''''''''''''''''''''''
                if ((frmMain.cbShowSteps.Checked == true))
                {
                    CvInvoke.DrawContours(imgContours, contours, i, SCALAR_WHITE);
                }
                // show steps '''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''

                PossibleChar possibleChar = new PossibleChar(contours[i]);

                //if contour is a possible char, note this does not compare to other chars (yet) . . .
                if ((DetectChars.checkIfPossibleChar(possibleChar)))
                {
                    intCountOfPossibleChars = intCountOfPossibleChars + 1;
                    //increment count of possible chars
                    listOfPossibleChars.Add(possibleChar);
                    //and add to list of possible chars
                }

            }

            // show steps '''''''''''''''''''''''''''''''''
            if ((frmMain.cbShowSteps.Checked == true))
            {
                frmMain.txtInfo.AppendText("\r\n" + "step 2 - contours.Size() = " + contours.Size.ToString() + "\r\n");
                //2362 with MCLRNF1 image
                frmMain.txtInfo.AppendText("step 2 - intCountOfPossibleChars = " + intCountOfPossibleChars.ToString() + "\r\n");
                //131 with MCLRNF1 image 
                CvInvoke.imshow("2a", imgContours);
            } 
            // show steps '''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''

            return listOfPossibleChars;
        }

        ///''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
        public static PossiblePlate extractPlate(Mat imgOriginal, List<PossibleChar> listOfMatchingChars)
        {
            PossiblePlate possiblePlate = new PossiblePlate();
            //this will be the return value

            //sort chars from left to right based on x position
            listOfMatchingChars.Sort((firstChar, secondChar) => firstChar.intCenterX.CompareTo(secondChar.intCenterX));

            //calculate the center point of the plate
            double dblPlateCenterX = Convert.ToDouble(listOfMatchingChars(0).intCenterX + listOfMatchingChars(listOfMatchingChars.Count - 1).intCenterX) / 2.0;
            double dblPlateCenterY = Convert.ToDouble(listOfMatchingChars(0).intCenterY + listOfMatchingChars(listOfMatchingChars.Count - 1).intCenterY) / 2.0;
            PointF ptfPlateCenter = new PointF(Convert.ToSingle(dblPlateCenterX), Convert.ToSingle(dblPlateCenterY));

            //calculate plate width and height
            int intPlateWidth = Convert.ToInt32(Convert.ToDouble(listOfMatchingChars(listOfMatchingChars.Count - 1).boundingRect.X + listOfMatchingChars(listOfMatchingChars.Count - 1).boundingRect.Width - listOfMatchingChars(0).boundingRect.X) * PLATE_WIDTH_PADDING_FACTOR);

            int intTotalOfCharHeights = 0;

            foreach (PossibleChar matchingChar in listOfMatchingChars)
            {
                intTotalOfCharHeights = intTotalOfCharHeights + matchingChar.boundingRect.Height;
            }

            object dblAverageCharHeight = Convert.ToDouble(intTotalOfCharHeights) / Convert.ToDouble(listOfMatchingChars.Count);

            object intPlateHeight = Convert.ToInt32(dblAverageCharHeight * PLATE_HEIGHT_PADDING_FACTOR);

            //calculate correction angle of plate region
            double dblOpposite = listOfMatchingChars(listOfMatchingChars.Count - 1).intCenterY - listOfMatchingChars(0).intCenterY;
            double dblHypotenuse = DetectChars.distanceBetweenChars(listOfMatchingChars(0), listOfMatchingChars(listOfMatchingChars.Count - 1));
            double dblCorrectionAngleInRad = Math.Asin(dblOpposite / dblHypotenuse);
            double dblCorrectionAngleInDeg = dblCorrectionAngleInRad * (180.0 / Math.PI);

            //assign rotated rect member variable of possible plate
            possiblePlate.rrLocationOfPlateInScene = new RotatedRect(ptfPlateCenter, new SizeF(Convert.ToSingle(intPlateWidth), Convert.ToSingle(intPlateHeight)), Convert.ToSingle(dblCorrectionAngleInDeg));

            Mat rotationMatrix = new Mat();
            //final steps are to perform the actual rotation
            Mat imgRotated = new Mat();
            Mat imgCropped = new Mat();

            CvInvoke.GetRotationMatrix2D(ptfPlateCenter, dblCorrectionAngleInDeg, 1.0, rotationMatrix);
            //get the rotation matrix for our calculated correction angle

            CvInvoke.WarpAffine(imgOriginal, imgRotated, rotationMatrix, imgOriginal.Size);
            //rotate the entire image

            //crop out the actual plate portion of the rotated image
            CvInvoke.GetRectSubPix(imgRotated, possiblePlate.rrLocationOfPlateInScene.MinAreaRect.Size, possiblePlate.rrLocationOfPlateInScene.Center, imgCropped);

            possiblePlate.imgPlate = imgCropped;
            //copy the cropped plate image into the applicable member variable of the possible plate

            return possiblePlate;
        } 
    }
}
