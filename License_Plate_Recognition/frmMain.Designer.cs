namespace License_Plate_Recognition
{
    partial class frmMain
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
            this.components = new System.ComponentModel.Container();
            this.tableLayoutPanel = new System.Windows.Forms.TableLayoutPanel();
            this.btnOpenFile = new System.Windows.Forms.Button();
            this.lblChosenFile = new System.Windows.Forms.Label();
            this.ibOriginal = new Emgu.CV.UI.ImageBox();
            this.cbShowSteps = new System.Windows.Forms.CheckBox();
            this.txtInfo = new System.Windows.Forms.TextBox();
            this.ofdOpenFile = new System.Windows.Forms.OpenFileDialog();
            this.tableLayoutPanel.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.ibOriginal)).BeginInit();
            this.SuspendLayout();
            // 
            // tableLayoutPanel
            // 
            this.tableLayoutPanel.ColumnCount = 3;
            this.tableLayoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanel.Controls.Add(this.btnOpenFile, 0, 0);
            this.tableLayoutPanel.Controls.Add(this.lblChosenFile, 1, 0);
            this.tableLayoutPanel.Controls.Add(this.ibOriginal, 0, 1);
            this.tableLayoutPanel.Controls.Add(this.cbShowSteps, 2, 0);
            this.tableLayoutPanel.Controls.Add(this.txtInfo, 0, 2);
            this.tableLayoutPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel.Location = new System.Drawing.Point(0, 0);
            this.tableLayoutPanel.Name = "tableLayoutPanel";
            this.tableLayoutPanel.RowCount = 3;
            this.tableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 70F));
            this.tableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 30F));
            this.tableLayoutPanel.Size = new System.Drawing.Size(608, 356);
            this.tableLayoutPanel.TabIndex = 0;
            // 
            // btnOpenFile
            // 
            this.btnOpenFile.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.btnOpenFile.AutoSize = true;
            this.btnOpenFile.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.btnOpenFile.Location = new System.Drawing.Point(3, 3);
            this.btnOpenFile.Name = "btnOpenFile";
            this.btnOpenFile.Size = new System.Drawing.Size(75, 23);
            this.btnOpenFile.TabIndex = 0;
            this.btnOpenFile.Text = "Open Image";
            this.btnOpenFile.UseVisualStyleBackColor = true;
            this.btnOpenFile.Click += new System.EventHandler(this.btnOpenFile_Click);
            // 
            // lblChosenFile
            // 
            this.lblChosenFile.Dock = System.Windows.Forms.DockStyle.Right;
            this.lblChosenFile.Location = new System.Drawing.Point(370, 0);
            this.lblChosenFile.Name = "lblChosenFile";
            this.lblChosenFile.Size = new System.Drawing.Size(125, 30);
            this.lblChosenFile.TabIndex = 1;
            this.lblChosenFile.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // ibOriginal
            // 
            this.ibOriginal.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.tableLayoutPanel.SetColumnSpan(this.ibOriginal, 3);
            this.ibOriginal.Dock = System.Windows.Forms.DockStyle.Fill;
            this.ibOriginal.Enabled = false;
            this.ibOriginal.Location = new System.Drawing.Point(3, 33);
            this.ibOriginal.Name = "ibOriginal";
            this.ibOriginal.Size = new System.Drawing.Size(602, 222);
            this.ibOriginal.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.ibOriginal.TabIndex = 2;
            this.ibOriginal.TabStop = false;
            // 
            // cbShowSteps
            // 
            this.cbShowSteps.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.cbShowSteps.Location = new System.Drawing.Point(501, 3);
            this.cbShowSteps.Name = "cbShowSteps";
            this.cbShowSteps.Size = new System.Drawing.Size(104, 24);
            this.cbShowSteps.TabIndex = 3;
            this.cbShowSteps.Text = "Show steps";
            this.cbShowSteps.UseVisualStyleBackColor = true;
            this.cbShowSteps.CheckedChanged += new System.EventHandler(this.cbShowSteps_CheckedChanged);
            // 
            // txtInfo
            // 
            this.tableLayoutPanel.SetColumnSpan(this.txtInfo, 3);
            this.txtInfo.Dock = System.Windows.Forms.DockStyle.Fill;
            this.txtInfo.Location = new System.Drawing.Point(3, 261);
            this.txtInfo.Multiline = true;
            this.txtInfo.Name = "txtInfo";
            this.txtInfo.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.txtInfo.Size = new System.Drawing.Size(602, 92);
            this.txtInfo.TabIndex = 4;
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
            this.ClientSize = new System.Drawing.Size(608, 356);
            this.Controls.Add(this.tableLayoutPanel);
            this.Name = "frmMain";
            this.Text = "Form1";
            this.Load += new System.EventHandler(this.frmMain_Load);
            this.tableLayoutPanel.ResumeLayout(false);
            this.tableLayoutPanel.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.ibOriginal)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel;
        private System.Windows.Forms.Button btnOpenFile;
        private System.Windows.Forms.Label lblChosenFile;
        private Emgu.CV.UI.ImageBox ibOriginal;
        private System.Windows.Forms.CheckBox cbShowSteps;
        private System.Windows.Forms.TextBox txtInfo;
        private System.Windows.Forms.OpenFileDialog ofdOpenFile;
    }
}

