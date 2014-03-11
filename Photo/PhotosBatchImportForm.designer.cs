namespace EMBA.Photo
{
    partial class PhotosBatchImportForm
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
            this.btnExit = new DevComponents.DotNetBar.ButtonX();
            this.btnUpload = new DevComponents.DotNetBar.ButtonX();
            this.cbxByStudentNum = new DevComponents.DotNetBar.Controls.CheckBoxX();
            this.cbxByStudentIDNumber = new DevComponents.DotNetBar.Controls.CheckBoxX();
            this.cbxByClassNameSeatNo = new DevComponents.DotNetBar.Controls.CheckBoxX();
            this.groupPanel2 = new DevComponents.DotNetBar.Controls.GroupPanel();
            this.cbxEnroll = new DevComponents.DotNetBar.Controls.CheckBoxX();
            this.cbxGraduate = new DevComponents.DotNetBar.Controls.CheckBoxX();
            this.groupPanel1 = new DevComponents.DotNetBar.Controls.GroupPanel();
            this.btnBrowse = new DevComponents.DotNetBar.ButtonX();
            this.txtFilePath = new DevComponents.DotNetBar.Controls.TextBoxX();
            this.labelX1 = new DevComponents.DotNetBar.LabelX();
            this.groupPanel2.SuspendLayout();
            this.groupPanel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // btnExit
            // 
            this.btnExit.AccessibleRole = System.Windows.Forms.AccessibleRole.PushButton;
            this.btnExit.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnExit.AutoSize = true;
            this.btnExit.BackColor = System.Drawing.Color.Transparent;
            this.btnExit.ColorTable = DevComponents.DotNetBar.eButtonColor.OrangeWithBackground;
            this.btnExit.Location = new System.Drawing.Point(201, 169);
            this.btnExit.Margin = new System.Windows.Forms.Padding(2);
            this.btnExit.Name = "btnExit";
            this.btnExit.Size = new System.Drawing.Size(51, 25);
            this.btnExit.TabIndex = 6;
            this.btnExit.Text = "離開";
            this.btnExit.Click += new System.EventHandler(this.btnExit_Click);
            // 
            // btnUpload
            // 
            this.btnUpload.AccessibleRole = System.Windows.Forms.AccessibleRole.PushButton;
            this.btnUpload.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnUpload.AutoSize = true;
            this.btnUpload.BackColor = System.Drawing.Color.Transparent;
            this.btnUpload.ColorTable = DevComponents.DotNetBar.eButtonColor.OrangeWithBackground;
            this.btnUpload.Location = new System.Drawing.Point(142, 169);
            this.btnUpload.Margin = new System.Windows.Forms.Padding(2);
            this.btnUpload.Name = "btnUpload";
            this.btnUpload.Size = new System.Drawing.Size(51, 25);
            this.btnUpload.TabIndex = 5;
            this.btnUpload.Text = "匯入";
            this.btnUpload.Click += new System.EventHandler(this.btnUpload_Click);
            // 
            // cbxByStudentNum
            // 
            this.cbxByStudentNum.AutoSize = true;
            // 
            // 
            // 
            this.cbxByStudentNum.BackgroundStyle.Class = "";
            this.cbxByStudentNum.BackgroundStyle.CornerType = DevComponents.DotNetBar.eCornerType.Square;
            this.cbxByStudentNum.CheckBoxStyle = DevComponents.DotNetBar.eCheckBoxStyle.RadioButton;
            this.cbxByStudentNum.Checked = true;
            this.cbxByStudentNum.CheckState = System.Windows.Forms.CheckState.Checked;
            this.cbxByStudentNum.CheckValue = "Y";
            this.cbxByStudentNum.Enabled = false;
            this.cbxByStudentNum.Location = new System.Drawing.Point(11, 5);
            this.cbxByStudentNum.Margin = new System.Windows.Forms.Padding(2);
            this.cbxByStudentNum.Name = "cbxByStudentNum";
            this.cbxByStudentNum.Size = new System.Drawing.Size(54, 21);
            this.cbxByStudentNum.TabIndex = 1;
            this.cbxByStudentNum.Text = "學號";
            // 
            // cbxByStudentIDNumber
            // 
            this.cbxByStudentIDNumber.AutoSize = true;
            // 
            // 
            // 
            this.cbxByStudentIDNumber.BackgroundStyle.Class = "";
            this.cbxByStudentIDNumber.BackgroundStyle.CornerType = DevComponents.DotNetBar.eCornerType.Square;
            this.cbxByStudentIDNumber.CheckBoxStyle = DevComponents.DotNetBar.eCheckBoxStyle.RadioButton;
            this.cbxByStudentIDNumber.Location = new System.Drawing.Point(97, 5);
            this.cbxByStudentIDNumber.Margin = new System.Windows.Forms.Padding(2);
            this.cbxByStudentIDNumber.Name = "cbxByStudentIDNumber";
            this.cbxByStudentIDNumber.Size = new System.Drawing.Size(80, 21);
            this.cbxByStudentIDNumber.TabIndex = 2;
            this.cbxByStudentIDNumber.Text = "身分證號";
            this.cbxByStudentIDNumber.Visible = false;
            // 
            // cbxByClassNameSeatNo
            // 
            this.cbxByClassNameSeatNo.AutoSize = true;
            this.cbxByClassNameSeatNo.BackColor = System.Drawing.Color.Transparent;
            // 
            // 
            // 
            this.cbxByClassNameSeatNo.BackgroundStyle.Class = "";
            this.cbxByClassNameSeatNo.BackgroundStyle.CornerType = DevComponents.DotNetBar.eCornerType.Square;
            this.cbxByClassNameSeatNo.CheckBoxStyle = DevComponents.DotNetBar.eCheckBoxStyle.RadioButton;
            this.cbxByClassNameSeatNo.Location = new System.Drawing.Point(23, 173);
            this.cbxByClassNameSeatNo.Margin = new System.Windows.Forms.Padding(2);
            this.cbxByClassNameSeatNo.Name = "cbxByClassNameSeatNo";
            this.cbxByClassNameSeatNo.Size = new System.Drawing.Size(80, 21);
            this.cbxByClassNameSeatNo.TabIndex = 3;
            this.cbxByClassNameSeatNo.Text = "班級姓名";
            this.cbxByClassNameSeatNo.Visible = false;
            // 
            // groupPanel2
            // 
            this.groupPanel2.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.groupPanel2.BackColor = System.Drawing.Color.Transparent;
            this.groupPanel2.CanvasColor = System.Drawing.SystemColors.Control;
            this.groupPanel2.ColorSchemeStyle = DevComponents.DotNetBar.eDotNetBarStyle.Office2007;
            this.groupPanel2.Controls.Add(this.cbxByStudentIDNumber);
            this.groupPanel2.Controls.Add(this.cbxByStudentNum);
            this.groupPanel2.Location = new System.Drawing.Point(9, 100);
            this.groupPanel2.Margin = new System.Windows.Forms.Padding(2);
            this.groupPanel2.Name = "groupPanel2";
            this.groupPanel2.Size = new System.Drawing.Size(243, 59);
            // 
            // 
            // 
            this.groupPanel2.Style.BackColor2SchemePart = DevComponents.DotNetBar.eColorSchemePart.PanelBackground2;
            this.groupPanel2.Style.BackColorGradientAngle = 90;
            this.groupPanel2.Style.BackColorSchemePart = DevComponents.DotNetBar.eColorSchemePart.PanelBackground;
            this.groupPanel2.Style.BorderBottom = DevComponents.DotNetBar.eStyleBorderType.Solid;
            this.groupPanel2.Style.BorderBottomWidth = 1;
            this.groupPanel2.Style.BorderColorSchemePart = DevComponents.DotNetBar.eColorSchemePart.PanelBorder;
            this.groupPanel2.Style.BorderLeft = DevComponents.DotNetBar.eStyleBorderType.Solid;
            this.groupPanel2.Style.BorderLeftWidth = 1;
            this.groupPanel2.Style.BorderRight = DevComponents.DotNetBar.eStyleBorderType.Solid;
            this.groupPanel2.Style.BorderRightWidth = 1;
            this.groupPanel2.Style.BorderTop = DevComponents.DotNetBar.eStyleBorderType.Solid;
            this.groupPanel2.Style.BorderTopWidth = 1;
            this.groupPanel2.Style.Class = "";
            this.groupPanel2.Style.CornerDiameter = 4;
            this.groupPanel2.Style.CornerType = DevComponents.DotNetBar.eCornerType.Rounded;
            this.groupPanel2.Style.TextColorSchemePart = DevComponents.DotNetBar.eColorSchemePart.PanelText;
            this.groupPanel2.Style.TextLineAlignment = DevComponents.DotNetBar.eStyleTextAlignment.Near;
            // 
            // 
            // 
            this.groupPanel2.StyleMouseDown.Class = "";
            this.groupPanel2.StyleMouseDown.CornerType = DevComponents.DotNetBar.eCornerType.Square;
            // 
            // 
            // 
            this.groupPanel2.StyleMouseOver.Class = "";
            this.groupPanel2.StyleMouseOver.CornerType = DevComponents.DotNetBar.eCornerType.Square;
            this.groupPanel2.TabIndex = 4;
            this.groupPanel2.Text = "命名方式";
            // 
            // cbxEnroll
            // 
            this.cbxEnroll.AutoSize = true;
            // 
            // 
            // 
            this.cbxEnroll.BackgroundStyle.Class = "";
            this.cbxEnroll.BackgroundStyle.CornerType = DevComponents.DotNetBar.eCornerType.Square;
            this.cbxEnroll.CheckBoxStyle = DevComponents.DotNetBar.eCheckBoxStyle.RadioButton;
            this.cbxEnroll.Location = new System.Drawing.Point(11, 5);
            this.cbxEnroll.Margin = new System.Windows.Forms.Padding(2);
            this.cbxEnroll.Name = "cbxEnroll";
            this.cbxEnroll.Size = new System.Drawing.Size(80, 21);
            this.cbxEnroll.TabIndex = 0;
            this.cbxEnroll.Text = "入學照片";
            // 
            // cbxGraduate
            // 
            this.cbxGraduate.AutoSize = true;
            // 
            // 
            // 
            this.cbxGraduate.BackgroundStyle.Class = "";
            this.cbxGraduate.BackgroundStyle.CornerType = DevComponents.DotNetBar.eCornerType.Square;
            this.cbxGraduate.CheckBoxStyle = DevComponents.DotNetBar.eCheckBoxStyle.RadioButton;
            this.cbxGraduate.Location = new System.Drawing.Point(97, 5);
            this.cbxGraduate.Margin = new System.Windows.Forms.Padding(2);
            this.cbxGraduate.Name = "cbxGraduate";
            this.cbxGraduate.Size = new System.Drawing.Size(80, 21);
            this.cbxGraduate.TabIndex = 1;
            this.cbxGraduate.Text = "畢業照片";
            // 
            // groupPanel1
            // 
            this.groupPanel1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.groupPanel1.BackColor = System.Drawing.Color.Transparent;
            this.groupPanel1.CanvasColor = System.Drawing.SystemColors.Control;
            this.groupPanel1.ColorSchemeStyle = DevComponents.DotNetBar.eDotNetBarStyle.Office2007;
            this.groupPanel1.Controls.Add(this.cbxGraduate);
            this.groupPanel1.Controls.Add(this.cbxEnroll);
            this.groupPanel1.Location = new System.Drawing.Point(9, 37);
            this.groupPanel1.Margin = new System.Windows.Forms.Padding(2);
            this.groupPanel1.Name = "groupPanel1";
            this.groupPanel1.Size = new System.Drawing.Size(243, 56);
            // 
            // 
            // 
            this.groupPanel1.Style.BackColor2SchemePart = DevComponents.DotNetBar.eColorSchemePart.PanelBackground2;
            this.groupPanel1.Style.BackColorGradientAngle = 90;
            this.groupPanel1.Style.BackColorSchemePart = DevComponents.DotNetBar.eColorSchemePart.PanelBackground;
            this.groupPanel1.Style.BorderBottom = DevComponents.DotNetBar.eStyleBorderType.Solid;
            this.groupPanel1.Style.BorderBottomWidth = 1;
            this.groupPanel1.Style.BorderColorSchemePart = DevComponents.DotNetBar.eColorSchemePart.PanelBorder;
            this.groupPanel1.Style.BorderLeft = DevComponents.DotNetBar.eStyleBorderType.Solid;
            this.groupPanel1.Style.BorderLeftWidth = 1;
            this.groupPanel1.Style.BorderRight = DevComponents.DotNetBar.eStyleBorderType.Solid;
            this.groupPanel1.Style.BorderRightWidth = 1;
            this.groupPanel1.Style.BorderTop = DevComponents.DotNetBar.eStyleBorderType.Solid;
            this.groupPanel1.Style.BorderTopWidth = 1;
            this.groupPanel1.Style.Class = "";
            this.groupPanel1.Style.CornerDiameter = 4;
            this.groupPanel1.Style.CornerType = DevComponents.DotNetBar.eCornerType.Rounded;
            this.groupPanel1.Style.TextColorSchemePart = DevComponents.DotNetBar.eColorSchemePart.PanelText;
            this.groupPanel1.Style.TextLineAlignment = DevComponents.DotNetBar.eStyleTextAlignment.Near;
            // 
            // 
            // 
            this.groupPanel1.StyleMouseDown.Class = "";
            this.groupPanel1.StyleMouseDown.CornerType = DevComponents.DotNetBar.eCornerType.Square;
            // 
            // 
            // 
            this.groupPanel1.StyleMouseOver.Class = "";
            this.groupPanel1.StyleMouseOver.CornerType = DevComponents.DotNetBar.eCornerType.Square;
            this.groupPanel1.TabIndex = 3;
            this.groupPanel1.Text = "照片項目";
            // 
            // btnBrowse
            // 
            this.btnBrowse.AccessibleRole = System.Windows.Forms.AccessibleRole.PushButton;
            this.btnBrowse.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnBrowse.AutoSize = true;
            this.btnBrowse.BackColor = System.Drawing.Color.Transparent;
            this.btnBrowse.ColorTable = DevComponents.DotNetBar.eButtonColor.OrangeWithBackground;
            this.btnBrowse.Location = new System.Drawing.Point(206, 9);
            this.btnBrowse.Margin = new System.Windows.Forms.Padding(2);
            this.btnBrowse.Name = "btnBrowse";
            this.btnBrowse.Size = new System.Drawing.Size(44, 25);
            this.btnBrowse.TabIndex = 2;
            this.btnBrowse.Text = "瀏覽";
            this.btnBrowse.Click += new System.EventHandler(this.btnBrowse_Click);
            // 
            // txtFilePath
            // 
            this.txtFilePath.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            // 
            // 
            // 
            this.txtFilePath.Border.Class = "TextBoxBorder";
            this.txtFilePath.Border.CornerType = DevComponents.DotNetBar.eCornerType.Square;
            this.txtFilePath.Location = new System.Drawing.Point(73, 9);
            this.txtFilePath.Margin = new System.Windows.Forms.Padding(2);
            this.txtFilePath.Name = "txtFilePath";
            this.txtFilePath.Size = new System.Drawing.Size(119, 25);
            this.txtFilePath.TabIndex = 1;
            // 
            // labelX1
            // 
            this.labelX1.AutoSize = true;
            this.labelX1.BackColor = System.Drawing.Color.Transparent;
            // 
            // 
            // 
            this.labelX1.BackgroundStyle.Class = "";
            this.labelX1.BackgroundStyle.CornerType = DevComponents.DotNetBar.eCornerType.Square;
            this.labelX1.Location = new System.Drawing.Point(10, 11);
            this.labelX1.Margin = new System.Windows.Forms.Padding(2);
            this.labelX1.Name = "labelX1";
            this.labelX1.Size = new System.Drawing.Size(60, 21);
            this.labelX1.TabIndex = 0;
            this.labelX1.Text = "資料來源";
            // 
            // PhotosBatchImportForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 17F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(259, 198);
            this.Controls.Add(this.cbxByClassNameSeatNo);
            this.Controls.Add(this.btnExit);
            this.Controls.Add(this.btnUpload);
            this.Controls.Add(this.groupPanel2);
            this.Controls.Add(this.groupPanel1);
            this.Controls.Add(this.btnBrowse);
            this.Controls.Add(this.txtFilePath);
            this.Controls.Add(this.labelX1);
            this.DoubleBuffered = true;
            this.Margin = new System.Windows.Forms.Padding(2);
            this.Name = "PhotosBatchImportForm";
            this.Text = "批次匯入學生照片(jpg格式)";
            this.groupPanel2.ResumeLayout(false);
            this.groupPanel2.PerformLayout();
            this.groupPanel1.ResumeLayout(false);
            this.groupPanel1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private DevComponents.DotNetBar.ButtonX btnExit;
        private DevComponents.DotNetBar.ButtonX btnUpload;
        private DevComponents.DotNetBar.Controls.CheckBoxX cbxByStudentNum;
        private DevComponents.DotNetBar.Controls.CheckBoxX cbxByStudentIDNumber;
        private DevComponents.DotNetBar.Controls.CheckBoxX cbxByClassNameSeatNo;
        private DevComponents.DotNetBar.Controls.GroupPanel groupPanel2;
        private DevComponents.DotNetBar.Controls.CheckBoxX cbxEnroll;
        private DevComponents.DotNetBar.Controls.CheckBoxX cbxGraduate;
        private DevComponents.DotNetBar.Controls.GroupPanel groupPanel1;
        private DevComponents.DotNetBar.ButtonX btnBrowse;
        private DevComponents.DotNetBar.Controls.TextBoxX txtFilePath;
        private DevComponents.DotNetBar.LabelX labelX1;

    }
}