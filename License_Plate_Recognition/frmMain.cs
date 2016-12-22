using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Emgu.CV;
//
using Emgu.CV.CvEnum;
//usual Emgu Cv imports
using Emgu.CV.Structure;
//
using Emgu.CV.UI;
//
using Emgu.CV.Util;

namespace License_Plate_Recognition
{
    public partial class frmMain : Form
    {
        // module level variables ''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
        //these are for changing the proportion of image box to text box based on if we are showing steps or not
        const float IMAGE_BOX_PCT_SHOW_STEPS_NOT_CHECKED = 75;

        const float TEXT_BOX_PCT_SHOW_STEPS_NOT_CHECKED = 25;
        //the idea is to show more of the text box if we are showing steps since there is more text to display
        const float IMAGE_BOX_PCT_SHOW_STEPS_CHECKED = 55;

        const float TEXT_BOX_PCT_SHOW_STEPS_CHECKED = 45;
        MCvScalar SCALAR_RED = new MCvScalar(0.0, 0.0, 255.0);

        MCvScalar SCALAR_YELLOW = new MCvScalar(0.0, 255.0, 255.0);
        public bool cbShowStepsChecked;
        public DetectChars detectChars;
        public DetectPlates detectPlates;
        public frmMain()
        {
            InitializeComponent();
        }

        private void frmMain_Load(object sender, EventArgs e)
        {
            cbShowSteps_CheckedChanged(new object(), new EventArgs());
            //call check box event to update form based on check box initial state
            detectChars = new DetectChars(this);
            detectPlates = new DetectPlates(this);
            bool blnKNNTrainingSuccessful = detectChars.loadKNNDataAndTrainKNN();
            //attempt KNN training

            //if KNN training was not successful
            if ((blnKNNTrainingSuccessful == false))
            {
                txtInfo.AppendText("\r\n error: KNN traning was not successful \r\n");
                //show message on text box
                MessageBox.Show("error: KNN traning was not successful");
                //also show message box
                btnOpenFile.Enabled = false;
                //disable open file button
                return;
                //and bail
            }

        }

        private void btnOpenFile_Click(object sender, EventArgs e)
        {
            Mat imgOriginalScene = new Mat();
            //this is the original image scene

            bool blnImageOpenedSuccessfully = openImageWithErrorHandling(ref imgOriginalScene);
            //attempt to open image

            //if image was not opened successfully
            if ((!blnImageOpenedSuccessfully))
            {
                ibOriginal.Image = null;
                //set the image box on the form to blank
                return;
                //and bail
            }

            lblChosenFile.Text = ofdOpenFile.FileName;
            //update label with file name

            CvInvoke.DestroyAllWindows();
            //close any windows that are open from previous button press

            ibOriginal.Image = imgOriginalScene;
            //show original image on main form

            List<PossiblePlate> listOfPossiblePlates = detectPlates.detectPlatesInScene(imgOriginalScene);
            //detect plates

            listOfPossiblePlates = detectChars.detectCharsInPlates(listOfPossiblePlates);
            //detect chars in plates

            //check if list of plates is null or zero
            if ((listOfPossiblePlates == null))
            {
                txtInfo.AppendText("\r\n" + "no license plates were detected" + "\r\n");
            }
            else if ((listOfPossiblePlates.Count == 0))
            {
                txtInfo.AppendText("\r\n" + "no license plates were detected" + "\r\n");
            }
            else
            {
                //if we get in here list of possible plates has at leat one plate

                //sort the list of possible plates in DESCENDING order (most number of chars to least number of chars)
                listOfPossiblePlates.Sort((onePlate, otherPlate) => otherPlate.strChars.Length.CompareTo(onePlate.strChars.Length));

                //suppose the plate with the most recognized chars
                PossiblePlate licPlate = listOfPossiblePlates[0];
                //(the first plate in sorted by string length descending order)
                //is the actual plate

                CvInvoke.Imshow("final imgPlate", licPlate.imgPlate);
                //show the final color plate image 
                CvInvoke.Imshow("final imgThresh", licPlate.imgThresh);
                //show the final thresh plate image

                //if no chars are present in the lic plate,
                if ((licPlate.strChars.Length == 0))
                {
                    txtInfo.AppendText("\r\n" + "no characters were detected" + licPlate.strChars + "\r\n");
                    //update info text box
                    return;
                    //and return
                }

                drawRedRectangleAroundPlate(imgOriginalScene, licPlate);
                //draw red rectangle around plate

                txtInfo.AppendText("\r\n" + "license plate read from image = " + licPlate.strChars + "\r\n");
                //write license plate text to text box
                txtInfo.AppendText("\r\n" + "----------------------------------------" + "\r\n");

                writeLicensePlateCharsOnImage(ref imgOriginalScene, licPlate);
                //write license plate text on the image

                ibOriginal.Image = imgOriginalScene;
                //update image on main form

                CvInvoke.Imwrite("imgOriginalScene.png", imgOriginalScene);
                //write image out to file
            }

        }

        private void cbShowSteps_CheckedChanged(object sender, EventArgs e)
        {
            if ((cbShowSteps.Checked == false))
            {
                cbShowStepsChecked = false;
                //tableLayoutPanel.RowStyles. = IMAGE_BOX_PCT_SHOW_STEPS_NOT_CHECKED;
                ////if showing steps, show more of the text box
                //tableLayoutPanel.RowStyles.Item(2).Height = TEXT_BOX_PCT_SHOW_STEPS_NOT_CHECKED;
            }
            else if ((cbShowSteps.Checked == true))
            {
                cbShowStepsChecked = true;
                //tableLayoutPanel.RowStyles.Item(1).Height = IMAGE_BOX_PCT_SHOW_STEPS_CHECKED;
                ////if not showing steps, show less of the text box
                //tableLayoutPanel.RowStyles.Item(2).Height = TEXT_BOX_PCT_SHOW_STEPS_CHECKED;
            }
        }
        public bool openImageWithErrorHandling(ref Mat imgOriginalScene)
        {
            DialogResult drChosenFile = default(DialogResult);

            drChosenFile = ofdOpenFile.ShowDialog();
            //open file dialog

            //if user did not choose anything
            if ((drChosenFile != DialogResult.OK | string.IsNullOrEmpty(ofdOpenFile.FileName)))
            {
                lblChosenFile.Text = "file not chosen";
                //update label
                return false;
                //and bail
            }

            try
            {
                imgOriginalScene = CvInvoke.Imread(ofdOpenFile.FileName, LoadImageType.Color);
                //open image
                //if error occurred
            }
            catch (Exception ex)
            {
                lblChosenFile.Text = "unable to open image, error: " + ex.Message;
                //show error message on label
                return false;
                //and exit function
            }

            //if image could not be opened
            if ((imgOriginalScene == null))
            {
                lblChosenFile.Text = "unable to open image, image was null";
                //show error message on label
                return false;
                //and exit function
            }

            //if image opened as empty
            if ((imgOriginalScene.IsEmpty))
            {
                lblChosenFile.Text = "unable to open image, image was empty";
                //show error message on label
                return false;
                //and exit function
            }

            return true;
        }

        ///''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
        public void drawRedRectangleAroundPlate(Mat imgOriginalScene, PossiblePlate licPlate)
        {
            PointF[] ptfRectPoints = new PointF[5];
            //declare array of 4 points, floating point type

            ptfRectPoints = licPlate.rrLocationOfPlateInScene.GetVertices();
            //get 4 vertices of rotated rect

            Point pt0 = new Point(Convert.ToInt32(ptfRectPoints[0].X), Convert.ToInt32(ptfRectPoints[0].Y));
            //declare 4 points, integer type
            Point pt1 = new Point(Convert.ToInt32(ptfRectPoints[1].X), Convert.ToInt32(ptfRectPoints[1].Y));
            Point pt2 = new Point(Convert.ToInt32(ptfRectPoints[2].X), Convert.ToInt32(ptfRectPoints[2].Y));
            Point pt3 = new Point(Convert.ToInt32(ptfRectPoints[3].X), Convert.ToInt32(ptfRectPoints[3].Y));

            CvInvoke.Line(imgOriginalScene, pt0, pt1, SCALAR_RED, 2);
            //draw 4 red lines
            CvInvoke.Line(imgOriginalScene, pt1, pt2, SCALAR_RED, 2);
            CvInvoke.Line(imgOriginalScene, pt2, pt3, SCALAR_RED, 2);
            CvInvoke.Line(imgOriginalScene, pt3, pt0, SCALAR_RED, 2);
        }

        ///''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
        public void writeLicensePlateCharsOnImage(ref Mat imgOriginalScene, PossiblePlate licPlate)
        {
            Point ptCenterOfTextArea = new Point();
            //this will be the center of the area the text will be written to
            Point ptLowerLeftTextOrigin = new Point();
            //this will be the bottom left of the area that the text will be written to

            FontFace fontFace = FontFace.HersheySimplex;
            //choose a plain jane font
            double dblFontScale = licPlate.imgPlate.Height / 30;
            //base font scale on height of plate area
            int intFontThickness = Convert.ToInt32(dblFontScale * 1.5);
            //base font thickness on font scale
            Size textSize = new Size();

            //to get the text size, we should use the OpenCV function getTextSize, but for some reason Emgu CV does not include this
            //we can instead estimate the test size based on the font scale, this will not be especially accurate but is good enough for our purposes here
            textSize.Width = Convert.ToInt32(dblFontScale * 18.5 * licPlate.strChars.Length);
            textSize.Height = Convert.ToInt32(dblFontScale * 25);

            ptCenterOfTextArea.X = Convert.ToInt32(licPlate.rrLocationOfPlateInScene.Center.X);
            //the horizontal location of the text area is the same as the plate

            //if the license plate is in the upper 3/4 of the image, we will write the chars in below the plate
            if ((licPlate.rrLocationOfPlateInScene.Center.Y < (imgOriginalScene.Height * 0.75)))
            {
                ptCenterOfTextArea.Y = Convert.ToInt32(licPlate.rrLocationOfPlateInScene.Center.Y + Convert.ToInt32(Convert.ToDouble(licPlate.rrLocationOfPlateInScene.MinAreaRect().Height) * 1.6));
                //else if the license plate is in the lower 1/4 of the image, we will write the chars in above the plate
            }
            else
            {
                ptCenterOfTextArea.Y = Convert.ToInt32(licPlate.rrLocationOfPlateInScene.Center.Y - Convert.ToInt32(Convert.ToDouble(licPlate.rrLocationOfPlateInScene.MinAreaRect().Height) * 1.6));
            }

            ptLowerLeftTextOrigin.X = Convert.ToInt32(ptCenterOfTextArea.X - (textSize.Width / 2));
            //calculate the lower left origin of the text area
            ptLowerLeftTextOrigin.Y = Convert.ToInt32(ptCenterOfTextArea.Y + (textSize.Height / 2));
            //based on the text area center, width, and height

            CvInvoke.PutText(imgOriginalScene, licPlate.strChars, ptLowerLeftTextOrigin, fontFace, dblFontScale, SCALAR_YELLOW, intFontThickness);
            //write the text on the image
        }

        private void btnTrain_Click(object sender, EventArgs e)
        {
            frmTrain train = new frmTrain();
            train.Show();
        }
    }
}
