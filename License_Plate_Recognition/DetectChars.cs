using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


using System.Collections;

using System.Data;
using System.Diagnostics;
//DetectChars.vb
//
//Emgu CV 2.4.10

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
using Emgu.CV.ML;
//
using Emgu.CV.Util;

using System.Xml;
using System.Xml.Serialization;
//these imports are for reading Matrix objects from file
using System.IO;
using System.Drawing;

namespace License_Plate_Recognition
{
    public  class DetectChars
    {
        frmMain frm;
        // module level variables ''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
        //constants for checkIfPossibleChar, this checks one possible char only (does not compare to another char)
        const int MIN_PIXEL_WIDTH = 2;

        const int MIN_PIXEL_HEIGHT = 8;
        const double MIN_ASPECT_RATIO = 0.15;

        const double MAX_ASPECT_RATIO = 1.0;

        const int MIN_RECT_AREA = 80;
        //constants for comparing two chars
        const double MIN_DIAG_SIZE_MULTIPLE_AWAY = 0.2;

        const double MAX_DIAG_SIZE_MULTIPLE_AWAY = 5.0;

        const double MAX_CHANGE_IN_AREA = 0.5;
        const double MAX_CHANGE_IN_WIDTH = 0.9;

        const double MAX_CHANGE_IN_HEIGHT = 0.2;

        const double MAX_ANGLE_BETWEEN_CHARS = 12.0;
        //other constants

        const int MIN_NUMBER_OF_MATCHING_CHARS = 3;
        const int RESIZED_CHAR_IMAGE_WIDTH = 20;
        const int RESIZED_CHAR_IMAGE_HEIGHT = 30;

        static MCvScalar SCALAR_WHITE = new MCvScalar(255.0, 255.0, 255.0);

        static MCvScalar SCALAR_GREEN = new MCvScalar(0.0, 255.0, 0.0);
        //variables
        static KNearest kNearest = new KNearest();
        public DetectChars(frmMain frmmain)
        {
             frm = frmmain;
        }
        ///''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
        public bool loadKNNDataAndTrainKNN()
        {
            //note: we effectively have to read the first XML file twice
            //first, we read the file to get the number of rows (which is the same as the number of samples)
            //the first time reading the file we can't get the data yet, since we don't know how many rows of data there are
            //next, reinstantiate our classifications Matrix and training images Matrix with the correct number of rows
            //then, read the file again and this time read the data into our resized classifications Matrix and training images Matrix

            Matrix<float> mtxClassifications = new Matrix<float>(1, 1);
            //for the first time through, declare these to be 1 row by 1 column
            Matrix<float> mtxTrainingImages = new Matrix<float>(1, 1);
            //we will resize these when we know the number of rows (i.e. number of training samples)

            List<int> intValidChars = new List<int>(new int[] {
           (int)'0',
           (int)'1',
           (int)'2',
           (int)'3',
           (int)'4',
           (int)'5',
           (int)'6',
           (int)'7',
           (int)'8',
           (int)'9',
           (int)'A',
           (int)'B',
           (int)'C',
           (int)'D',
           (int)'E',
           (int)'F',
           (int)'G',
           (int)'H',
           (int)'I',
           (int)'J',
           (int)'K',
           (int)'L',
           (int)'M',
           (int)'N',
           (int)'O',
           (int)'P',
           (int)'Q',
           (int)'R',
           (int)'S',
           (int)'T',
           (int)'U',
           (int)'V',
           (int)'W',
           (int)'X',
           (int)'Y',
           (int)'Z'
        });

            XmlSerializer xmlSerializer = new XmlSerializer(mtxClassifications.GetType());
            //these variables are for
            StreamReader streamReader = default(StreamReader);
            //reading from the XML files

            try
            {
                streamReader = new StreamReader("classifications.xml");
                //attempt to open classifications file
                //if error is encountered, show error and return
            }
            catch (Exception ex)
            {
                frm.txtInfo.AppendText("\r\n" + "unable to open 'classifications.xml', error: ");
                frm.txtInfo.AppendText(ex.Message + "\r\n");
                return false;
            }

            //read from the classifications file the 1st time, this is only to get the number of rows, not the actual data
            mtxClassifications = (Matrix<float>)xmlSerializer.Deserialize(streamReader);

            streamReader.Close();
            //close the classifications XML file

            int intNumberOfTrainingSamples = mtxClassifications.Rows;
            //get the number of rows, i.e. the number of training samples

            //now that we know the number of rows, reinstantiate classifications Matrix and training images Matrix with the actual number of rows
            mtxClassifications = new Matrix<float>(intNumberOfTrainingSamples, 1);
            mtxTrainingImages = new Matrix<float>(intNumberOfTrainingSamples, RESIZED_CHAR_IMAGE_WIDTH * RESIZED_CHAR_IMAGE_HEIGHT);

            try
            {
                streamReader = new StreamReader("classifications.xml");
                //reinitialize the stream reader
                //if error is encountered, show error and return
            }
            catch (Exception ex)
            {
                frm.txtInfo.AppendText("\r\n" + "unable to open 'classifications.xml', error:" + "\r\n");
                frm.txtInfo.AppendText(ex.Message + "\r\n" + "\r\n");
                return false;
            }
            //read from the classifications file again, this time we can get the actual data
            mtxClassifications = (Matrix<float>)xmlSerializer.Deserialize(streamReader);

            streamReader.Close();
            //close the classifications XML file

            xmlSerializer = new XmlSerializer(mtxTrainingImages.GetType());
            //reinstantiate file reading variable

            try
            {
                streamReader = new StreamReader("images.xml");
                //if error is encountered, show error and return
            }
            catch (Exception ex)
            {
                frm.txtInfo.AppendText("unable to open 'images.xml', error:" + "\r\n");
                frm.txtInfo.AppendText(ex.Message + "\r\n" + "\r\n");
                return false;
            }

            mtxTrainingImages = (Matrix<float>)xmlSerializer.Deserialize(streamReader);
            //read from training images file
            streamReader.Close();
            //close the training images XML file

            // train '''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''

            kNearest.DefaultK = 1;

            kNearest.Train(mtxTrainingImages, Emgu.CV.ML.MlEnum.DataLayoutType.RowSample, mtxClassifications);

            return true;
            //if we got here training was successful so return true
        }

        ///''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
        public List<PossiblePlate> detectCharsInPlates(List<PossiblePlate> listOfPossiblePlates)
        {
            int intPlateCounter = 0;
            //this is only for showing steps
            Mat imgContours = default(Mat);
            Random random = new Random();
            //this is only for showing steps

            //if list of possible plates is null,
            if ((listOfPossiblePlates == null))
            {
                return listOfPossiblePlates;
                //return
                //if list of possible plates has zero plates
            }
            else if ((listOfPossiblePlates.Count == 0))
            {
                return listOfPossiblePlates;
                //return
            }
            //at this point we can be sure list of possible plates has at least one plate

            // for each possible plate, this is a big for loop that takes up most of the function
            foreach (PossiblePlate possiblePlate in listOfPossiblePlates)
            {
                Preprocess.preprocess(possiblePlate.imgPlate, ref possiblePlate.imgGrayscale, ref possiblePlate.imgThresh);
                //preprocess to get grayscale and threshold images

                // show steps '''''''''''''''''''''''''''''
                if ((frm.cbShowSteps.Checked == true))
                {
                    CvInvoke.Imshow("5a", possiblePlate.imgPlate);
                    CvInvoke.Imshow("5b", possiblePlate.imgGrayscale);
                    CvInvoke.Imshow("5c", possiblePlate.imgThresh);
                }
                // show steps '''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''

                CvInvoke.Resize(possiblePlate.imgThresh, possiblePlate.imgThresh, new Size(), 1.6, 1.6);
                //upscale size by 60% for better viewing and character recognition

                CvInvoke.Threshold(possiblePlate.imgThresh, possiblePlate.imgThresh, 0.0, 255.0, ThresholdType.Binary | ThresholdType.Otsu);
                //threshold again to eliminate any gray areas

                // show steps '''''''''''''''''''''''''''''
                if ((frm.cbShowSteps.Checked == true))
                {
                    CvInvoke.Imshow("5d", possiblePlate.imgThresh);
                }
                // show steps '''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''

                //find all possible chars in the plate,
                //this function first finds all contours, then only includes contours that could be chars (without comparison to other chars yet)
                List<PossibleChar> listOfPossibleCharsInPlate = findPossibleCharsInPlate(possiblePlate.imgGrayscale, possiblePlate.imgThresh);

                // show steps '''''''''''''''''''''''''''''
                if ((frm.cbShowSteps.Checked == true))
                {
                    imgContours = new Mat(possiblePlate.imgThresh.Size, DepthType.Cv8U, 3);
                    VectorOfVectorOfPoint contours = new VectorOfVectorOfPoint();

                    foreach (PossibleChar possibleChar in listOfPossibleCharsInPlate)
                    {
                        contours.Push(possibleChar.contour);
                    }

                    CvInvoke.DrawContours(imgContours, contours, -1, SCALAR_WHITE);

                    CvInvoke.Imshow("6", imgContours);

                }
                // show steps '''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''

                //given a list of all possible chars, find groups of matching chars within the plate
                List<List<PossibleChar>> listOfListsOfMatchingCharsInPlate = findListOfListsOfMatchingChars(listOfPossibleCharsInPlate);

                // show steps '''''''''''''''''''''''''''''
                if ((frm.cbShowSteps.Checked == true))
                {
                    imgContours = new Mat(possiblePlate.imgThresh.Size, DepthType.Cv8U, 3);

                    VectorOfVectorOfPoint contours = new VectorOfVectorOfPoint();

                    foreach (List<PossibleChar> listOfMatchingChars in listOfListsOfMatchingCharsInPlate)
                    {
                        object intRandomBlue = random.Next(0, 256);
                        object intRandomGreen = random.Next(0, 256);
                        object intRandomRed = random.Next(0, 256);

                        foreach (PossibleChar matchingChar in listOfMatchingChars)
                        {
                            contours.Push(matchingChar.contour);
                        }
                        CvInvoke.DrawContours(imgContours, contours, -1, new MCvScalar(Convert.ToDouble(intRandomBlue), Convert.ToDouble(intRandomGreen), Convert.ToDouble(intRandomRed)));
                    }

                    CvInvoke.Imshow("7", imgContours);
                }
                // show steps '''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''

                //if no matching chars were found
                if ((listOfListsOfMatchingCharsInPlate == null))
                {
                    // show steps '''''''''''''''''''''''''
                    if ((frm.cbShowSteps.Checked == true))
                    {
                        frm.txtInfo.AppendText("chars found in plate number " + intPlateCounter.ToString() + " = (none), click on any image and press a key to continue . . ." + "\r\n");
                        intPlateCounter = intPlateCounter + 1;
                        CvInvoke.DestroyWindow("8");
                        CvInvoke.DestroyWindow("9");
                        CvInvoke.DestroyWindow("10");
                        CvInvoke.WaitKey(0);
                    }
                    // show steps '''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''

                    possiblePlate.strChars = "";
                    //set plate string member variable to empty string
                    continue;
                    //and jump back to top of big for loop
                }
                else if ((listOfListsOfMatchingCharsInPlate.Count == 0))
                {
                    // show steps '''''''''''''''''''''''''
                    if ((frm.cbShowSteps.Checked == true))
                    {
                        frm.txtInfo.AppendText("chars found in plate number " + intPlateCounter.ToString() + " = (none), click on any image and press a key to continue . . ." + "\r\n");
                        intPlateCounter = intPlateCounter + 1;
                        CvInvoke.DestroyWindow("8");
                        CvInvoke.DestroyWindow("9");
                        CvInvoke.DestroyWindow("10");
                        CvInvoke.WaitKey(0);
                    }
                    // show steps '''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''

                    possiblePlate.strChars = "";
                    //set plate string member variable to empty string
                    continue;
                    //and jump back to top of big for loop
                }

                //for each group of chars within the plate
                for (int i = 0; i <= listOfListsOfMatchingCharsInPlate.Count - 1; i++)
                {

                    //sort chars from left to right
                    listOfListsOfMatchingCharsInPlate[i].Sort((oneChar, otherChar) => oneChar.boundingRect.X.CompareTo(otherChar.boundingRect.X));

                    //remove inner overlapping chars
                    listOfListsOfMatchingCharsInPlate[i] = removeInnerOverlappingChars(listOfListsOfMatchingCharsInPlate[i]);
                }

                // show steps '''''''''''''''''''''''''''''
                if ((frm.cbShowSteps.Checked == true))
                {
                    imgContours = new Mat(possiblePlate.imgThresh.Size, DepthType.Cv8U, 3);

                    foreach (List<PossibleChar> listOfMatchingChars in listOfListsOfMatchingCharsInPlate)
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
                    CvInvoke.Imshow("8", imgContours);
                }
                // show steps '''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''

                //within each possible plate, suppose the longest list of potential matching chars is the actual list of chars
                int intLenOfLongestListOfChars = 0;
                int intIndexOfLongestListOfChars = 0;
                //loop through all the lists of matching chars, get the index of the one with the most chars
                for (int i = 0; i <= listOfListsOfMatchingCharsInPlate.Count - 1; i++)
                {
                    if ((listOfListsOfMatchingCharsInPlate[i].Count > intLenOfLongestListOfChars))
                    {
                        intLenOfLongestListOfChars = listOfListsOfMatchingCharsInPlate[i].Count;
                        intIndexOfLongestListOfChars = i;
                    }
                }

                //suppose that the longest list of matching chars within the plate is the actual list of chars
                List<PossibleChar> longestListOfMatchingCharsInPlate = listOfListsOfMatchingCharsInPlate[intIndexOfLongestListOfChars];

                // show steps '''''''''''''''''''''''''''''
                if ((frm.cbShowSteps.Checked == true))
                {
                    imgContours = new Mat(possiblePlate.imgThresh.Size, DepthType.Cv8U, 3);

                    VectorOfVectorOfPoint contours = new VectorOfVectorOfPoint();

                    foreach (PossibleChar matchingChar in longestListOfMatchingCharsInPlate)
                    {
                        contours.Push(matchingChar.contour);
                    }

                    CvInvoke.DrawContours(imgContours, contours, -1, SCALAR_WHITE);

                    CvInvoke.Imshow("9", imgContours);
                }
                // show steps '''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''

                possiblePlate.strChars = recognizeCharsInPlate(possiblePlate.imgThresh, longestListOfMatchingCharsInPlate);
                //perform char recognition on the longest list of matching chars in the plate

                // show steps '''''''''''''''''''''''''''''
                if ((frm.cbShowSteps.Checked == true))
                {
                    frm.txtInfo.AppendText("chars found in plate number " + intPlateCounter.ToString() + " = " + possiblePlate.strChars + ", click on any image and press a key to continue . . ." + "\r\n");
                    intPlateCounter = intPlateCounter + 1;
                    CvInvoke.WaitKey(0);
                }
                // show steps '''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
            }
            //end for each possible plate big for loop that takes up most of the function

            // show steps '''''''''''''''''''''''''''''''''
            if ((frm.cbShowSteps.Checked == true))
            {
                frm.txtInfo.AppendText("\r\n" + "char detection complete, click on any image and press a key to continue . . ." + "\r\n");
                CvInvoke.WaitKey(0);
            }
            // show steps '''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''

            return listOfPossiblePlates;
        }

        ///''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
        public  List<PossibleChar> findPossibleCharsInPlate(Mat imgGrayscale, Mat imgThresh)
        {
            List<PossibleChar> listOfPossibleChars = new List<PossibleChar>();
            //this will be the return value

            Mat imgThreshCopy = new Mat();

            VectorOfVectorOfPoint contours = new VectorOfVectorOfPoint();

            imgThreshCopy = imgThresh.Clone();

            CvInvoke.FindContours(imgThreshCopy, contours, null, RetrType.List, ChainApproxMethod.ChainApproxSimple);
            //find all contours in plate

            //for each contour
            for (int i = 0; i <= contours.Size - 1; i++)
            {
                PossibleChar possibleChar = new PossibleChar(contours[i]);

                //if contour is a possible char, note this does not compare to other chars (yet) . . .
                if ((checkIfPossibleChar(possibleChar)))
                {
                    listOfPossibleChars.Add(possibleChar);
                    //add to list of possible chars
                }

            }

            return listOfPossibleChars;
        }

        ///''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
        public   bool checkIfPossibleChar(PossibleChar possibleChar)
        {
            //this function is a 'first pass' that does a rough check on a contour to see if it could be a char,
            //note that we are not (yet) comparing the char to other chars to look for a group
            if ((possibleChar.intRectArea > MIN_RECT_AREA & possibleChar.boundingRect.Width > MIN_PIXEL_WIDTH & possibleChar.boundingRect.Height > MIN_PIXEL_HEIGHT & MIN_ASPECT_RATIO < possibleChar.dblAspectRatio & possibleChar.dblAspectRatio < MAX_ASPECT_RATIO))
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        ///''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
        public   List<List<PossibleChar>> findListOfListsOfMatchingChars(List<PossibleChar> listOfPossibleChars)
        {
            //with this function, we start off with all the possible chars in one big list
            //the purpose of this function is to re-arrange the one big list of chars into a list of lists of matching chars,
            //note that chars that are not found to be in a group of matches do not need to be considered further
            List<List<PossibleChar>> listOfListsOfMatchingChars = new List<List<PossibleChar>>();
            //this will be the return value

            //for each possible char in the one big list of chars
            foreach (PossibleChar possibleChar in listOfPossibleChars)
            {

                //find all chars in the big list that match the current char
                List<PossibleChar> listOfMatchingChars = findListOfMatchingChars(possibleChar, listOfPossibleChars);

                listOfMatchingChars.Add(possibleChar);
                //also add the current char to current possible list of matching chars

                //if current possible list of matching chars is not long enough to constitute a possible plate
                if ((listOfMatchingChars.Count < MIN_NUMBER_OF_MATCHING_CHARS))
                {
                    continue;
                    //jump back to the top of the for loop and try again with next char, note that it's not necessary
                    //to save the list in any way since it did not have enough chars to be a possible plate
                }
                //if we get here, the current list passed test as a "group" or "cluster" of matching chars
                listOfListsOfMatchingChars.Add(listOfMatchingChars);
                //so add to our list of lists of matching chars

                //remove the current list of matching chars from the big list so we don't use those same chars twice,
                //make sure to make a new big list for this since we don't want to change the original big list
                List<PossibleChar> listOfPossibleCharsWithCurrentMatchesRemoved = listOfPossibleChars.Except(listOfMatchingChars).ToList();

                //declare new list of lists of chars to get result from recursive call
                List<List<PossibleChar>> recursiveListOfListsOfMatchingChars = new List<List<PossibleChar>>();

                recursiveListOfListsOfMatchingChars = findListOfListsOfMatchingChars(listOfPossibleCharsWithCurrentMatchesRemoved);
                //recursive call

                //for each list of matching chars found by recursive call
                foreach (List<PossibleChar> recursiveListOfMatchingChars in recursiveListOfListsOfMatchingChars)
                {
                    listOfListsOfMatchingChars.Add(recursiveListOfMatchingChars);
                    //add to our original list of lists of matching chars
                }
                break; // TODO: might not be correct. Was : Exit For
                       //jump out of for loop
            }

            return listOfListsOfMatchingChars;
            //return result
        }

        ///''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
        public   List<PossibleChar> findListOfMatchingChars(PossibleChar possibleChar, List<PossibleChar> listOfChars)
        {
            //the purpose of this function is, given a possible char and a big list of possible chars,
            //find all chars in the big list that are a match for the single possible char, and return those matching chars as a list
            List<PossibleChar> listOfMatchingChars = new List<PossibleChar>();
            //this will be the return value

            //for each char in big list
            foreach (PossibleChar possibleMatchingChar in listOfChars)
            {

                //if the char we attempting to find matches for is the exact same char as the char in the big list we are currently checking
                if ((possibleMatchingChar.Equals(possibleChar)))
                {
                    //then we should not include it in the list of matches b/c that would end up double including the current char
                    continue;
                    //so do not add to list of matches and jump back to top of for loop
                }
                //compute stuff to see if chars are a match
                double dblDistanceBetweenChars = distanceBetweenChars(possibleChar, possibleMatchingChar);

                double dblAngleBetweenChars = angleBetweenChars(possibleChar, possibleMatchingChar);

                double dblChangeInArea = Math.Abs(possibleMatchingChar.intRectArea - possibleChar.intRectArea) / possibleChar.intRectArea;

                double dblChangeInWidth = Math.Abs(possibleMatchingChar.boundingRect.Width - possibleChar.boundingRect.Width) / possibleChar.boundingRect.Width;
                double dblChangeInHeight = Math.Abs(possibleMatchingChar.boundingRect.Height - possibleChar.boundingRect.Height) / possibleChar.boundingRect.Height;

                //check if chars match

                if ((dblDistanceBetweenChars < (possibleChar.dblDiagonalSize * MAX_DIAG_SIZE_MULTIPLE_AWAY) 
                    & dblAngleBetweenChars < MAX_ANGLE_BETWEEN_CHARS 
                    & dblChangeInArea < MAX_CHANGE_IN_AREA 
                    & dblChangeInWidth < MAX_CHANGE_IN_WIDTH 
                    & dblChangeInHeight < MAX_CHANGE_IN_HEIGHT))
                {
                    listOfMatchingChars.Add(possibleMatchingChar);
                    //if the chars are a match, add the current char to list of matching chars
                }

            }

            return listOfMatchingChars;
            //return result
        }

        ///''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
        //use Pythagorean theorem to calculate distance between two chars
        public   double distanceBetweenChars(PossibleChar firstChar, PossibleChar secondChar)
        {
            int intX = Math.Abs(firstChar.intCenterX - secondChar.intCenterX);
            int intY = Math.Abs(firstChar.intCenterY - secondChar.intCenterY);

            return Math.Sqrt((Math.Pow(intX, 2)) + (Math.Pow(intY, 2)));
        }

        ///''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
        //use basic trigonometry (SOH CAH TOA) to calculate angle between chars
        public   double angleBetweenChars(PossibleChar firstChar, PossibleChar secondChar)
        {
            double dblAdj = Convert.ToDouble(Math.Abs(firstChar.intCenterX - secondChar.intCenterX));
            double dblOpp = Convert.ToDouble(Math.Abs(firstChar.intCenterY - secondChar.intCenterY));

            double dblAngleInRad = Math.Atan(dblOpp / dblAdj);

            double dblAngleInDeg = dblAngleInRad * (180.0 / Math.PI);

            return dblAngleInDeg;
        }

        ///''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
        //if we have two chars overlapping or to close to each other to possibly be separate chars, remove the inner (smaller) char,
        //this is to prevent including the same char twice if two contours are found for the same char,
        //for example for the letter 'O' both the inner ring and the outer ring may be found as contours, but we should only include the char once
        public   List<PossibleChar> removeInnerOverlappingChars(List<PossibleChar> listOfMatchingChars)
        {
            List<PossibleChar> listOfMatchingCharsWithInnerCharRemoved = new List<PossibleChar>(listOfMatchingChars);

            foreach (PossibleChar currentChar in listOfMatchingChars)
            {
                foreach (PossibleChar otherChar in listOfMatchingChars)
                {
                    //if current char and other char are not the same char . . .
                    if ((!currentChar.Equals(otherChar)))
                    {
                        //if current char and other char have center points at almost the same location . . .
                        if ((distanceBetweenChars(currentChar, otherChar) < (currentChar.dblDiagonalSize * MIN_DIAG_SIZE_MULTIPLE_AWAY)))
                        {
                            //if we get in here we have found overlapping chars
                            //next we identify which char is smaller, then if that char was not already removed on a previous pass, remove it
                            //if current char is smaller than other char
                            if ((currentChar.intRectArea < otherChar.intRectArea))
                            {
                                //if current char was not already removed on a previous pass . . .
                                if ((listOfMatchingCharsWithInnerCharRemoved.Contains(currentChar)))
                                {
                                    listOfMatchingCharsWithInnerCharRemoved.Remove(currentChar);
                                    //then remove current char
                                }
                                //else if other char is smaller than current char
                            }
                            else
                            {
                                //if other char was not already removed on a previous pass . . .
                                if ((listOfMatchingCharsWithInnerCharRemoved.Contains(otherChar)))
                                {
                                    listOfMatchingCharsWithInnerCharRemoved.Remove(otherChar);
                                    //then remove other char
                                }

                            }
                        }
                    }
                }
            }

            return listOfMatchingCharsWithInnerCharRemoved;
        }

        ///''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
        //this is where we apply the actual char recognition
        public   string recognizeCharsInPlate(Mat imgThresh, List<PossibleChar> listOfMatchingChars)
        {
            string strChars = "";
            //this will be the return value, the chars in the lic plate

            Mat imgThreshColor = new Mat();

            listOfMatchingChars.Sort((oneChar, otherChar) => oneChar.boundingRect.X.CompareTo(otherChar.boundingRect.X));
            //sort chars from left to right

            CvInvoke.CvtColor(imgThresh, imgThreshColor, ColorConversion.Gray2Bgr);

            //for each char in plate
            foreach (PossibleChar currentChar in listOfMatchingChars)
            {
                CvInvoke.Rectangle(imgThreshColor, currentChar.boundingRect, SCALAR_GREEN, 2);
                //draw green box around the char

                Mat imgROItoBeCloned = new Mat(imgThresh, currentChar.boundingRect);
                //get ROI image of bounding rect

                Mat imgROI = imgROItoBeCloned.Clone();
                //clone ROI image so we don't change original when we resize

                Mat imgROIResized = new Mat();

                //resize image, this is necessary for char recognition
                CvInvoke.Resize(imgROI, imgROIResized, new Size(RESIZED_CHAR_IMAGE_WIDTH, RESIZED_CHAR_IMAGE_HEIGHT));

                //declare a Matrix of the same dimensions as the Image we are adding to the data structure of training images
                Matrix<float> mtxTemp = new Matrix<float>(imgROIResized.Size);

                //declare a flattened (only 1 row) matrix of the same total size
                Matrix<float> mtxTempReshaped = new Matrix<float>(1, RESIZED_CHAR_IMAGE_WIDTH * RESIZED_CHAR_IMAGE_HEIGHT);

                imgROIResized.ConvertTo(mtxTemp, DepthType.Cv32F);
                //convert Image to a Matrix of Singles with the same dimensions

                //flatten Matrix into one row by RESIZED_IMAGE_WIDTH * RESIZED_IMAGE_HEIGHT number of columns
                for (int intRow = 0; intRow <= RESIZED_CHAR_IMAGE_HEIGHT - 1; intRow++)
                {
                    for (int intCol = 0; intCol <= RESIZED_CHAR_IMAGE_WIDTH - 1; intCol++)
                    {
                        mtxTempReshaped[0, (intRow * RESIZED_CHAR_IMAGE_WIDTH) + intCol] = mtxTemp[intRow, intCol];
                    }
                }

                float sngCurrentChar = 0;

                sngCurrentChar = kNearest.Predict(mtxTempReshaped);
                //finally we can call Predict !!!

                strChars = strChars + (char)sngCurrentChar;
                //append current char to full string of chars
            }

            // show steps '''''''''''''''''''''''''''''''''
            if ((frm.cbShowSteps.Checked == true))
            {
                CvInvoke.Imshow("10", imgThreshColor);
            }
            // show steps '''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''

            return strChars;
            //return result
        }
    }
    }
