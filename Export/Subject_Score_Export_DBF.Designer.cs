namespace EMBACore.Export
{
    partial class Subject_Score_Export_DBF
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
            this.nudSchoolYear = new System.Windows.Forms.NumericUpDown();
            this.cboSemester = new DevComponents.DotNetBar.Controls.ComboBoxEx();
            this.lblSchoolYear = new DevComponents.DotNetBar.LabelX();
            this.lblSemester = new DevComponents.DotNetBar.LabelX();
            this.lnkScoreDegreeMapping = new System.Windows.Forms.LinkLabel();
            this.circularProgress = new DevComponents.DotNetBar.Controls.CircularProgress();
            ((System.ComponentModel.ISupportInitialize)(this.nudSchoolYear)).BeginInit();
            this.SuspendLayout();
            // 
            // chkSelectAll
            // 
            // 
            // 
            // 
            this.chkSelectAll.BackgroundStyle.Class = "";
            this.chkSelectAll.BackgroundStyle.CornerType = DevComponents.DotNetBar.eCornerType.Square;
            // 
            // lblExplanation
            // 
            // 
            // 
            // 
            this.lblExplanation.BackgroundStyle.Class = "";
            this.lblExplanation.BackgroundStyle.CornerType = DevComponents.DotNetBar.eCornerType.Square;
            this.lblExplanation.Size = new System.Drawing.Size(101, 21);
            // 
            // nudSchoolYear
            // 
            this.nudSchoolYear.Location = new System.Drawing.Point(261, 13);
            this.nudSchoolYear.Maximum = new decimal(new int[] {
            999,
            0,
            0,
            0});
            this.nudSchoolYear.Name = "nudSchoolYear";
            this.nudSchoolYear.Size = new System.Drawing.Size(66, 25);
            this.nudSchoolYear.TabIndex = 28;
            this.nudSchoolYear.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            // 
            // cboSemester
            // 
            this.cboSemester.DisplayMember = "Text";
            this.cboSemester.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawFixed;
            this.cboSemester.FormattingEnabled = true;
            this.cboSemester.ItemHeight = 19;
            this.cboSemester.Location = new System.Drawing.Point(377, 13);
            this.cboSemester.Name = "cboSemester";
            this.cboSemester.Size = new System.Drawing.Size(82, 25);
            this.cboSemester.Style = DevComponents.DotNetBar.eDotNetBarStyle.StyleManagerControlled;
            this.cboSemester.TabIndex = 26;
            // 
            // lblSchoolYear
            // 
            this.lblSchoolYear.AutoSize = true;
            this.lblSchoolYear.BackColor = System.Drawing.Color.Transparent;
            // 
            // 
            // 
            this.lblSchoolYear.BackgroundStyle.Class = "";
            this.lblSchoolYear.BackgroundStyle.CornerType = DevComponents.DotNetBar.eCornerType.Square;
            this.lblSchoolYear.Location = new System.Drawing.Point(211, 15);
            this.lblSchoolYear.Name = "lblSchoolYear";
            this.lblSchoolYear.Size = new System.Drawing.Size(47, 21);
            this.lblSchoolYear.TabIndex = 27;
            this.lblSchoolYear.Text = "學年度";
            // 
            // lblSemester
            // 
            this.lblSemester.AutoSize = true;
            this.lblSemester.BackColor = System.Drawing.Color.Transparent;
            // 
            // 
            // 
            this.lblSemester.BackgroundStyle.Class = "";
            this.lblSemester.BackgroundStyle.CornerType = DevComponents.DotNetBar.eCornerType.Square;
            this.lblSemester.Location = new System.Drawing.Point(340, 15);
            this.lblSemester.Name = "lblSemester";
            this.lblSemester.Size = new System.Drawing.Size(34, 21);
            this.lblSemester.TabIndex = 25;
            this.lblSemester.Text = "學期";
            // 
            // lnkScoreDegreeMapping
            // 
            this.lnkScoreDegreeMapping.AutoSize = true;
            this.lnkScoreDegreeMapping.BackColor = System.Drawing.Color.Transparent;
            this.lnkScoreDegreeMapping.Location = new System.Drawing.Point(184, 334);
            this.lnkScoreDegreeMapping.Name = "lnkScoreDegreeMapping";
            this.lnkScoreDegreeMapping.Size = new System.Drawing.Size(99, 17);
            this.lnkScoreDegreeMapping.TabIndex = 29;
            this.lnkScoreDegreeMapping.TabStop = true;
            this.lnkScoreDegreeMapping.Text = "等第分數對照表";
            this.lnkScoreDegreeMapping.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.lnkScoreDegreeMapping_LinkClicked);
            // 
            // circularProgress
            // 
            this.circularProgress.BackColor = System.Drawing.Color.Transparent;
            // 
            // 
            // 
            this.circularProgress.BackgroundStyle.Class = "";
            this.circularProgress.BackgroundStyle.CornerType = DevComponents.DotNetBar.eCornerType.Square;
            this.circularProgress.Location = new System.Drawing.Point(135, 329);
            this.circularProgress.Name = "circularProgress";
            this.circularProgress.Size = new System.Drawing.Size(43, 23);
            this.circularProgress.Style = DevComponents.DotNetBar.eDotNetBarStyle.OfficeXP;
            this.circularProgress.TabIndex = 30;
            this.circularProgress.Visible = false;
            // 
            // Subject_Score_Export_DBF
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 17F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(492, 366);
            this.Controls.Add(this.circularProgress);
            this.Controls.Add(this.lnkScoreDegreeMapping);
            this.Controls.Add(this.nudSchoolYear);
            this.Controls.Add(this.cboSemester);
            this.Controls.Add(this.lblSchoolYear);
            this.Controls.Add(this.lblSemester);
            this.DoubleBuffered = true;
            this.Name = "Subject_Score_Export_DBF";
            this.Text = "Subject_Score_Export_DBF";
            this.Controls.SetChildIndex(this.chkSelectAll, 0);
            this.Controls.SetChildIndex(this.btnExit, 0);
            this.Controls.SetChildIndex(this.btnExport, 0);
            this.Controls.SetChildIndex(this.FieldContainer, 0);
            this.Controls.SetChildIndex(this.lblExplanation, 0);
            this.Controls.SetChildIndex(this.lblSemester, 0);
            this.Controls.SetChildIndex(this.lblSchoolYear, 0);
            this.Controls.SetChildIndex(this.cboSemester, 0);
            this.Controls.SetChildIndex(this.nudSchoolYear, 0);
            this.Controls.SetChildIndex(this.lnkScoreDegreeMapping, 0);
            this.Controls.SetChildIndex(this.circularProgress, 0);
            ((System.ComponentModel.ISupportInitialize)(this.nudSchoolYear)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        public System.Windows.Forms.NumericUpDown nudSchoolYear;
        public DevComponents.DotNetBar.Controls.ComboBoxEx cboSemester;
        public DevComponents.DotNetBar.LabelX lblSchoolYear;
        public DevComponents.DotNetBar.LabelX lblSemester;
        private System.Windows.Forms.LinkLabel lnkScoreDegreeMapping;
        private DevComponents.DotNetBar.Controls.CircularProgress circularProgress;
    }
}