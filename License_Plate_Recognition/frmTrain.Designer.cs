namespace License_Plate_Recognition
{
    partial class frmTrain
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.tableLayoutPanel = new System.Windows.Forms.TableLayoutPanel();
            this.btnOpenTrainingImage = new System.Windows.Forms.Button();
            this.lblChosenFile = new System.Windows.Forms.Label();
            this.txtInfo = new System.Windows.Forms.TextBox();
            this.ofdOpenFile = new System.Windows.Forms.OpenFileDialog();
            this.tableLayoutPanel.SuspendLayout();
            this.SuspendLayout();
            // 
            // tableLayoutPanel
            // 
            this.tableLayoutPanel.ColumnCount = 2;
            this.tableLayoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel.Controls.Add(this.btnOpenTrainingImage, 0, 0);
            this.tableLayoutPanel.Controls.Add(this.lblChosenFile, 1, 0);
            this.tableLayoutPanel.Controls.Add(this.txtInfo, 0, 1);
            this.tableLayoutPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel.Location = new System.Drawing.Point(0, 0);
            this.tableLayoutPanel.Name = "tableLayoutPanel";
            this.tableLayoutPanel.RowCount = 2;
            this.tableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel.Size = new System.Drawing.Size(1158, 719);
            this.tableLayoutPanel.TabIndex = 0;
            // 
            // btnOpenTrainingImage
            // 
            this.btnOpenTrainingImage.AutoSize = true;
            this.btnOpenTrainingImage.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.btnOpenTrainingImage.Dock = System.Windows.Forms.DockStyle.Fill;
            this.btnOpenTrainingImage.Location = new System.Drawing.Point(3, 3);
            this.btnOpenTrainingImage.Name = "btnOpenTrainingImage";
            this.btnOpenTrainingImage.Size = new System.Drawing.Size(116, 23);
            this.btnOpenTrainingImage.TabIndex = 0;
            this.btnOpenTrainingImage.Text = "Open Training Image";
            this.btnOpenTrainingImage.UseVisualStyleBackColor = true;
            this.btnOpenTrainingImage.Click += new System.EventHandler(this.btnOpenTrainingImage_Click);
            // 
            // lblChosenFile
            // 
            this.lblChosenFile.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.lblChosenFile.AutoSize = true;
            this.lblChosenFile.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this.lblChosenFile.Location = new System.Drawing.Point(125, 4);
            this.lblChosenFile.Name = "lblChosenFile";
            this.lblChosenFile.Size = new System.Drawing.Size(1030, 20);
            this.lblChosenFile.TabIndex = 1;
            this.lblChosenFile.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // txtInfo
            // 
            this.tableLayoutPanel.SetColumnSpan(this.txtInfo, 2);
            this.txtInfo.Dock = System.Windows.Forms.DockStyle.Fill;
            this.txtInfo.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this.txtInfo.Location = new System.Drawing.Point(3, 32);
            this.txtInfo.Multiline = true;
            this.txtInfo.Name = "txtInfo";
            this.txtInfo.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.txtInfo.Size = new System.Drawing.Size(1152, 684);
            this.txtInfo.TabIndex = 2;
            this.txtInfo.WordWrap = false;
            // 
            // ofdOpenFile
            // 
            this.ofdOpenFile.FileName = "openFileDialog1";
            // 
            // frmMain
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1158, 719);
            this.Controls.Add(this.tableLayoutPanel);
            this.Name = "frmMain";
            this.Text = "Form1";
            this.tableLayoutPanel.ResumeLayout(false);
            this.tableLayoutPanel.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel;
        private System.Windows.Forms.Button btnOpenTrainingImage;
        private System.Windows.Forms.Label lblChosenFile;
        private System.Windows.Forms.TextBox txtInfo;
        private System.Windows.Forms.OpenFileDialog ofdOpenFile;
    }
}

