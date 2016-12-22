using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

using Emgu.CV;             //usual Emgu Cv using
using Emgu.CV.CvEnum;
using Emgu.CV.Structure;
using Emgu.CV.UI;
using Emgu.CV.Util;
using System.Xml.Serialization;
using System.IO;
//using System.Windows.Forms;

namespace License_Plate_Recognition
{
    public partial class frmTrain : Form
    {
        const int MIN_CONTOUR_AREA = 100;
        const int RESIZED_IMAGE_WIDTH = 20;
        const int RESIZED_IMAGE_HEIGHT = 30;

        public frmTrain()
        {
            InitializeComponent();
        }

        private void frmTrain_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            CvInvoke.DestroyAllWindows();
        }

        private void btnOpenTrainingImage_Click(object sender, EventArgs e)
        {
            DialogResult drChosenFile = default(DialogResult);

            drChosenFile = ofdOpenFile.ShowDialog();
            //open file dialog

            //if user chose Cancel or filename is blank . . .
            if ((drChosenFile != DialogResult.OK | string.IsNullOrEmpty(ofdOpenFile.FileName)))
            {
                lblChosenFile.Text = "file not chosen";
                //show error message on label
                return;
                //and exit function
            }

            Mat imgTrainingNumbers = default(Mat);

            try
            {
                imgTrainingNumbers = CvInvoke.Imread(ofdOpenFile.FileName, LoadImageType.Color);
                //if error occurred
            }
            catch (Exception ex)
            {
                lblChosenFile.Text = "unable to open image, error: " + ex.Message;
                //show error message on label
                return;
                //and exit function
            }

            //if image could not be opened
            if ((imgTrainingNumbers == null))
            {
                lblChosenFile.Text = "unable to open image";
                //show error message on label
                return;
                //and exit function
            }

            lblChosenFile.Text = ofdOpenFile.FileName;
            //update label with file name

            Mat imgGrayscale = new Mat();
            //
            Mat imgBlurred = new Mat();
            //declare various images
            Mat imgThresh = new Mat();
            //
            Mat imgThreshCopy = new Mat();
            //

            VectorOfVectorOfPoint contours = new VectorOfVectorOfPoint();

            Matrix<float> mtxClassifications = default(Matrix<float>);
            Matrix<float> mtxTrainingImages = default(Matrix<float>);

            Mat matTrainingImagesAsFlattenedFloats = new Mat();

            //possible chars we are interested in are digits 0 through 9 and capital letters A through Z, put these in list intValidChars
            List<int> intValidChars = new List<int>{
                (int)'0', (int)'1', (int)'2', (int)'3', (int)'4', (int)'5', (int)'6', (int)'7', (int)'8', (int)'9',
                                                                  (int)'A', (int)'B', (int)'C', (int)'D', (int)'E', (int)'F', (int)'G', (int)'H', (int)'I', (int)'J',
                                                                  (int)'K', (int)'L', (int)'M', (int)'N', (int)'O', (int)'P', (int)'Q', (int)'R', (int)'S', (int)'T',
                                                                  (int)'U', (int)'V', (int)'W', (int)'X', (int)'Y', (int)'Z' };

            Preprocess.preprocess(imgTrainingNumbers, ref imgGrayscale, ref imgThresh);
            

            CvInvoke.Imshow("imgThresh", imgThresh);
            //show threshold image for reference

            imgThreshCopy = imgThresh.Clone();
            //make a copy of the thresh image, this in necessary b/c findContours modifies the image

            //get external countours only
            CvInvoke.FindContours(imgThreshCopy, contours, null, RetrType.External, ChainApproxMethod.ChainApproxSimple);

            int intNumberOfTrainingSamples = contours.Size;

            mtxClassifications = new Matrix<float>(intNumberOfTrainingSamples, 1);
            //this is our classifications data structure

            //this is our training images data structure, note we will have to perform some conversions to write to this later
            mtxTrainingImages = new Matrix<float>(intNumberOfTrainingSamples, RESIZED_IMAGE_WIDTH * RESIZED_IMAGE_HEIGHT);

            //this keeps track of which row we are on in both classifications and training images,
            int intTrainingDataRowToAdd = 0;
            //note that each sample will correspond to one row in
            //both the classifications XML file and the training images XML file

            //for each contour
            for (int i = 0; i <= contours.Size - 1; i++)
            {
                //if contour is big enough to consider
                if ((CvInvoke.ContourArea(contours[i]) > MIN_CONTOUR_AREA))
                {
                    Rectangle boundingRect = CvInvoke.BoundingRectangle(contours[i]);
                    //get the bounding rect

                    CvInvoke.Rectangle(imgTrainingNumbers, boundingRect, new MCvScalar(0.0, 0.0, 255.0), 2);
                    //draw red rectangle around each contour as we ask user for input

                    Mat imgROItoBeCloned = new Mat(imgThresh, boundingRect);
                    //get ROI image of current char

                    Mat imgROI = imgROItoBeCloned.Clone();
                    //make a copy so we do not change the ROI area of the original image

                    Mat imgROIResized = new Mat();
                    //resize image, this is necessary for recognition and storage
                    CvInvoke.Resize(imgROI, imgROIResized, new Size(RESIZED_IMAGE_WIDTH, RESIZED_IMAGE_HEIGHT));

                    CvInvoke.Imshow("imgROI", imgROI);
                    //show ROI image for reference
                    CvInvoke.Imshow("imgROIResized", imgROIResized);
                    //show resized ROI image for reference
                    CvInvoke.Imshow("imgTrainingNumbers", imgTrainingNumbers);
                    //show training numbers image, this will now have red rectangles drawn on it

                    int intChar = CvInvoke.WaitKey(0);
                    //get key press

                    //if esc key was pressed
                    if ((intChar == 27))
                    {
                        CvInvoke.DestroyAllWindows();
                        return;
                        //exit the function
                        //else if the char is in the list of chars we are looking for . . .
                    }
                    else if ((intValidChars.Contains(intChar)))
                    {

                        mtxClassifications[intTrainingDataRowToAdd, 0] = Convert.ToSingle(intChar);
                        //write classification char to classifications Matrix

                        //now add the training image (some conversion is necessary first) . . .
                        //note that we have to covert the images to Matrix(Of Single) type, this is necessary to pass into the KNearest object call to train
                        Matrix<float> mtxTemp = new Matrix<float>(imgROIResized.Size);
                        Matrix<float> mtxTempReshaped = new Matrix<float>(1, RESIZED_IMAGE_WIDTH * RESIZED_IMAGE_HEIGHT);

                        imgROIResized.ConvertTo(mtxTemp, DepthType.Cv32F);
                        //convert Image to a Matrix of Singles with the same dimensions

                        //flatten Matrix into one row by RESIZED_IMAGE_WIDTH * RESIZED_IMAGE_HEIGHT number of columns
                        for (int intRow = 0; intRow <= RESIZED_IMAGE_HEIGHT - 1; intRow++)
                        {
                            for (int intCol = 0; intCol <= RESIZED_IMAGE_WIDTH - 1; intCol++)
                            {
                                mtxTempReshaped[0, (intRow * RESIZED_IMAGE_WIDTH) + intCol] = mtxTemp[intRow, intCol];
                            }
                        }

                        //write flattened Matrix into one row of training images Matrix
                        for (int intCol = 0; intCol <= (RESIZED_IMAGE_WIDTH * RESIZED_IMAGE_HEIGHT) - 1; intCol++)
                        {
                            mtxTrainingImages[intTrainingDataRowToAdd, intCol] = mtxTempReshaped[0, intCol];
                        }

                        intTrainingDataRowToAdd = intTrainingDataRowToAdd + 1;
                        //increment which row, i.e. sample we are on
                    }
                }
            }

            txtInfo.Text = txtInfo.Text + "training complete !!\n";

            //save classifications to file '''''''''''''''''''''''''''''''''''''''''''''''''''''

            XmlSerializer xmlSerializer = new XmlSerializer(mtxClassifications.GetType());
            StreamWriter streamWriter = default(StreamWriter);

            try
            {
                streamWriter = new StreamWriter("classifications.xml");
                //attempt to open classifications file
                //if error is encountered, show error and return
            }
            catch (Exception ex)
            {
                txtInfo.Text = "\n" + txtInfo.Text + "unable to open //classifications.xml//, error:\n";
                txtInfo.Text = txtInfo.Text + ex.Message + "\n";
                return;
            }

            xmlSerializer.Serialize(streamWriter, mtxClassifications);
            streamWriter.Close();

            //save training images to file '''''''''''''''''''''''''''''''''''''''''''''''''''''

            xmlSerializer = new XmlSerializer(mtxTrainingImages.GetType());

            try
            {
                streamWriter = new StreamWriter("images.xml");
                //attempt to open images file
                //if error is encountered, show error and return
            }
            catch (Exception ex)
            {
                txtInfo.Text = "\n" + txtInfo.Text + "unable to open //images.xml//, error:\n";
                txtInfo.Text = txtInfo.Text + ex.Message + "\n";
                return;
            }

            xmlSerializer.Serialize(streamWriter, mtxTrainingImages);
            streamWriter.Close();

            txtInfo.Text = "\n" + txtInfo.Text + "file writing done\n";

            MessageBox.Show("Training complete, file writing done !!");




        }

        private void frmTrain_Load(object sender, EventArgs e)
        {

        }
    }
}
