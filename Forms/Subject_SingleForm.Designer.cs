namespace EMBACore.Forms
{
    partial class Subject_SingleForm
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
            this.labelX1 = new DevComponents.DotNetBar.LabelX();
            this.txtDept = new DevComponents.DotNetBar.Controls.TextBoxX();
            this.txtDeptCode = new DevComponents.DotNetBar.Controls.TextBoxX();
            this.labelX2 = new DevComponents.DotNetBar.LabelX();
            this.txtSubjectName = new DevComponents.DotNetBar.Controls.TextBoxX();
            this.labelX3 = new DevComponents.DotNetBar.LabelX();
            this.labelX5 = new DevComponents.DotNetBar.LabelX();
            this.txtEnglishName = new DevComponents.DotNetBar.Controls.TextBoxX();
            this.labelX6 = new DevComponents.DotNetBar.LabelX();
            this.txtRemark = new DevComponents.DotNetBar.Controls.TextBoxX();
            this.labelX4 = new DevComponents.DotNetBar.LabelX();
            this.txtWebUrl = new DevComponents.DotNetBar.Controls.TextBoxX();
            this.labelX7 = new DevComponents.DotNetBar.LabelX();
            this.txtDescription = new DevComponents.DotNetBar.Controls.TextBoxX();
            this.labelX8 = new DevComponents.DotNetBar.LabelX();
            this.txtSubjectCode1 = new DevComponents.DotNetBar.Controls.TextBoxX();
            this.labelX10 = new DevComponents.DotNetBar.LabelX();
            this.chkIsRequired = new DevComponents.DotNetBar.Controls.CheckBoxX();
            this.labelX9 = new DevComponents.DotNetBar.LabelX();
            this.nudCredit = new System.Windows.Forms.NumericUpDown();
            this.errorProvider1 = new System.Windows.Forms.ErrorProvider(this.components);
            this.txtNewSubjectCode = new DevComponents.DotNetBar.Controls.TextBoxX();
            this.labelX11 = new DevComponents.DotNetBar.LabelX();
            ((System.ComponentModel.ISupportInitialize)(this.nudCredit)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.errorProvider1)).BeginInit();
            this.SuspendLayout();
            // 
            // btnSave
            // 
            this.btnSave.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnSave.Location = new System.Drawing.Point(259, 377);
            this.btnSave.Click += new System.EventHandler(this.btnSave_Click);
            // 
            // btnCancel
            // 
            this.btnCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnCancel.Location = new System.Drawing.Point(321, 377);
            // 
            // labelX1
            // 
            this.labelX1.BackColor = System.Drawing.Color.Transparent;
            // 
            // 
            // 
            this.labelX1.BackgroundStyle.Class = "";
            this.labelX1.BackgroundStyle.CornerType = DevComponents.DotNetBar.eCornerType.Square;
            this.labelX1.Location = new System.Drawing.Point(12, 77);
            this.labelX1.Name = "labelX1";
            this.labelX1.Size = new System.Drawing.Size(62, 23);
            this.labelX1.TabIndex = 2;
            this.labelX1.Text = "開課系所";
            // 
            // txtDept
            // 
            // 
            // 
            // 
            this.txtDept.Border.Class = "TextBoxBorder";
            this.txtDept.Border.CornerType = DevComponents.DotNetBar.eCornerType.Square;
            this.txtDept.Location = new System.Drawing.Point(71, 77);
            this.txtDept.Name = "txtDept";
            this.txtDept.Size = new System.Drawing.Size(116, 25);
            this.txtDept.TabIndex = 3;
            // 
            // txtDeptCode
            // 
            // 
            // 
            // 
            this.txtDeptCode.Border.Class = "TextBoxBorder";
            this.txtDeptCode.Border.CornerType = DevComponents.DotNetBar.eCornerType.Square;
            this.txtDeptCode.Location = new System.Drawing.Point(71, 108);
            this.txtDeptCode.Name = "txtDeptCode";
            this.txtDeptCode.Size = new System.Drawing.Size(116, 25);
            this.txtDeptCode.TabIndex = 5;
            // 
            // labelX2
            // 
            this.labelX2.BackColor = System.Drawing.Color.Transparent;
            // 
            // 
            // 
            this.labelX2.BackgroundStyle.Class = "";
            this.labelX2.BackgroundStyle.CornerType = DevComponents.DotNetBar.eCornerType.Square;
            this.labelX2.Location = new System.Drawing.Point(12, 108);
            this.labelX2.Name = "labelX2";
            this.labelX2.Size = new System.Drawing.Size(62, 23);
            this.labelX2.TabIndex = 4;
            this.labelX2.Text = "系所代碼";
            // 
            // txtSubjectName
            // 
            // 
            // 
            // 
            this.txtSubjectName.Border.Class = "TextBoxBorder";
            this.txtSubjectName.Border.CornerType = DevComponents.DotNetBar.eCornerType.Square;
            this.txtSubjectName.Location = new System.Drawing.Point(71, 14);
            this.txtSubjectName.Name = "txtSubjectName";
            this.txtSubjectName.Size = new System.Drawing.Size(116, 25);
            this.txtSubjectName.TabIndex = 7;
            // 
            // labelX3
            // 
            this.labelX3.BackColor = System.Drawing.Color.Transparent;
            // 
            // 
            // 
            this.labelX3.BackgroundStyle.Class = "";
            this.labelX3.BackgroundStyle.CornerType = DevComponents.DotNetBar.eCornerType.Square;
            this.labelX3.Location = new System.Drawing.Point(12, 14);
            this.labelX3.Name = "labelX3";
            this.labelX3.Size = new System.Drawing.Size(62, 23);
            this.labelX3.TabIndex = 6;
            this.labelX3.Text = "課程名稱";
            // 
            // labelX5
            // 
            this.labelX5.BackColor = System.Drawing.Color.Transparent;
            // 
            // 
            // 
            this.labelX5.BackgroundStyle.Class = "";
            this.labelX5.BackgroundStyle.CornerType = DevComponents.DotNetBar.eCornerType.Square;
            this.labelX5.Location = new System.Drawing.Point(204, 47);
            this.labelX5.Name = "labelX5";
            this.labelX5.Size = new System.Drawing.Size(62, 23);
            this.labelX5.TabIndex = 10;
            this.labelX5.Text = "學分數";
            // 
            // txtEnglishName
            // 
            // 
            // 
            // 
            this.txtEnglishName.Border.Class = "TextBoxBorder";
            this.txtEnglishName.Border.CornerType = DevComponents.DotNetBar.eCornerType.Square;
            this.txtEnglishName.Location = new System.Drawing.Point(71, 45);
            this.txtEnglishName.Name = "txtEnglishName";
            this.txtEnglishName.Size = new System.Drawing.Size(116, 25);
            this.txtEnglishName.TabIndex = 9;
            // 
            // labelX6
            // 
            this.labelX6.BackColor = System.Drawing.Color.Transparent;
            // 
            // 
            // 
            this.labelX6.BackgroundStyle.Class = "";
            this.labelX6.BackgroundStyle.CornerType = DevComponents.DotNetBar.eCornerType.Square;
            this.labelX6.Location = new System.Drawing.Point(12, 45);
            this.labelX6.Name = "labelX6";
            this.labelX6.Size = new System.Drawing.Size(62, 23);
            this.labelX6.TabIndex = 8;
            this.labelX6.Text = "英文名稱";
            // 
            // txtRemark
            // 
            // 
            // 
            // 
            this.txtRemark.Border.Class = "TextBoxBorder";
            this.txtRemark.Border.CornerType = DevComponents.DotNetBar.eCornerType.Square;
            this.txtRemark.Location = new System.Drawing.Point(71, 289);
            this.txtRemark.Multiline = true;
            this.txtRemark.Name = "txtRemark";
            this.txtRemark.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.txtRemark.Size = new System.Drawing.Size(308, 73);
            this.txtRemark.TabIndex = 21;
            // 
            // labelX4
            // 
            this.labelX4.BackColor = System.Drawing.Color.Transparent;
            // 
            // 
            // 
            this.labelX4.BackgroundStyle.Class = "";
            this.labelX4.BackgroundStyle.CornerType = DevComponents.DotNetBar.eCornerType.Square;
            this.labelX4.Location = new System.Drawing.Point(12, 289);
            this.labelX4.Name = "labelX4";
            this.labelX4.Size = new System.Drawing.Size(62, 23);
            this.labelX4.TabIndex = 20;
            this.labelX4.Text = "備註";
            // 
            // txtWebUrl
            // 
            // 
            // 
            // 
            this.txtWebUrl.Border.Class = "TextBoxBorder";
            this.txtWebUrl.Border.CornerType = DevComponents.DotNetBar.eCornerType.Square;
            this.txtWebUrl.Location = new System.Drawing.Point(71, 139);
            this.txtWebUrl.Name = "txtWebUrl";
            this.txtWebUrl.Size = new System.Drawing.Size(308, 25);
            this.txtWebUrl.TabIndex = 19;
            // 
            // labelX7
            // 
            this.labelX7.BackColor = System.Drawing.Color.Transparent;
            // 
            // 
            // 
            this.labelX7.BackgroundStyle.Class = "";
            this.labelX7.BackgroundStyle.CornerType = DevComponents.DotNetBar.eCornerType.Square;
            this.labelX7.Location = new System.Drawing.Point(12, 139);
            this.labelX7.Name = "labelX7";
            this.labelX7.Size = new System.Drawing.Size(62, 23);
            this.labelX7.TabIndex = 18;
            this.labelX7.Text = "網頁連結";
            // 
            // txtDescription
            // 
            // 
            // 
            // 
            this.txtDescription.Border.Class = "TextBoxBorder";
            this.txtDescription.Border.CornerType = DevComponents.DotNetBar.eCornerType.Square;
            this.txtDescription.Location = new System.Drawing.Point(71, 170);
            this.txtDescription.Multiline = true;
            this.txtDescription.Name = "txtDescription";
            this.txtDescription.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.txtDescription.Size = new System.Drawing.Size(308, 113);
            this.txtDescription.TabIndex = 17;
            // 
            // labelX8
            // 
            this.labelX8.BackColor = System.Drawing.Color.Transparent;
            // 
            // 
            // 
            this.labelX8.BackgroundStyle.Class = "";
            this.labelX8.BackgroundStyle.CornerType = DevComponents.DotNetBar.eCornerType.Square;
            this.labelX8.Location = new System.Drawing.Point(12, 167);
            this.labelX8.Name = "labelX8";
            this.labelX8.Size = new System.Drawing.Size(62, 23);
            this.labelX8.TabIndex = 16;
            this.labelX8.Text = "內容簡介";
            // 
            // txtSubjectCode1
            // 
            // 
            // 
            // 
            this.txtSubjectCode1.Border.Class = "TextBoxBorder";
            this.txtSubjectCode1.Border.CornerType = DevComponents.DotNetBar.eCornerType.Square;
            this.txtSubjectCode1.Location = new System.Drawing.Point(263, 16);
            this.txtSubjectCode1.Name = "txtSubjectCode1";
            this.txtSubjectCode1.Size = new System.Drawing.Size(114, 25);
            this.txtSubjectCode1.TabIndex = 13;
            // 
            // labelX10
            // 
            this.labelX10.BackColor = System.Drawing.Color.Transparent;
            // 
            // 
            // 
            this.labelX10.BackgroundStyle.Class = "";
            this.labelX10.BackgroundStyle.CornerType = DevComponents.DotNetBar.eCornerType.Square;
            this.labelX10.Location = new System.Drawing.Point(204, 16);
            this.labelX10.Name = "labelX10";
            this.labelX10.Size = new System.Drawing.Size(62, 23);
            this.labelX10.TabIndex = 12;
            this.labelX10.Text = "識別碼";
            // 
            // chkIsRequired
            // 
            this.chkIsRequired.BackColor = System.Drawing.Color.Transparent;
            // 
            // 
            // 
            this.chkIsRequired.BackgroundStyle.Class = "";
            this.chkIsRequired.BackgroundStyle.CornerType = DevComponents.DotNetBar.eCornerType.Square;
            this.chkIsRequired.CheckBoxPosition = DevComponents.DotNetBar.eCheckBoxPosition.Right;
            this.chkIsRequired.Location = new System.Drawing.Point(358, 50);
            this.chkIsRequired.Name = "chkIsRequired";
            this.chkIsRequired.Size = new System.Drawing.Size(23, 23);
            this.chkIsRequired.Style = DevComponents.DotNetBar.eDotNetBarStyle.StyleManagerControlled;
            this.chkIsRequired.TabIndex = 22;
            // 
            // labelX9
            // 
            this.labelX9.BackColor = System.Drawing.Color.Transparent;
            // 
            // 
            // 
            this.labelX9.BackgroundStyle.Class = "";
            this.labelX9.BackgroundStyle.CornerType = DevComponents.DotNetBar.eCornerType.Square;
            this.labelX9.Location = new System.Drawing.Point(317, 50);
            this.labelX9.Name = "labelX9";
            this.labelX9.Size = new System.Drawing.Size(62, 23);
            this.labelX9.TabIndex = 14;
            this.labelX9.Text = "必    修";
            // 
            // nudCredit
            // 
            this.nudCredit.Location = new System.Drawing.Point(263, 48);
            this.nudCredit.Maximum = new decimal(new int[] {
            10,
            0,
            0,
            0});
            this.nudCredit.Name = "nudCredit";
            this.nudCredit.Size = new System.Drawing.Size(44, 25);
            this.nudCredit.TabIndex = 23;
            this.nudCredit.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // errorProvider1
            // 
            this.errorProvider1.ContainerControl = this;
            // 
            // txtNewSubjectCode
            // 
            // 
            // 
            // 
            this.txtNewSubjectCode.Border.Class = "TextBoxBorder";
            this.txtNewSubjectCode.Border.CornerType = DevComponents.DotNetBar.eCornerType.Square;
            this.txtNewSubjectCode.Location = new System.Drawing.Point(262, 79);
            this.txtNewSubjectCode.Name = "txtNewSubjectCode";
            this.txtNewSubjectCode.Size = new System.Drawing.Size(115, 25);
            this.txtNewSubjectCode.TabIndex = 26;
            // 
            // labelX11
            // 
            this.labelX11.BackColor = System.Drawing.Color.Transparent;
            // 
            // 
            // 
            this.labelX11.BackgroundStyle.Class = "";
            this.labelX11.BackgroundStyle.CornerType = DevComponents.DotNetBar.eCornerType.Square;
            this.labelX11.Location = new System.Drawing.Point(204, 79);
            this.labelX11.Name = "labelX11";
            this.labelX11.Size = new System.Drawing.Size(61, 23);
            this.labelX11.TabIndex = 25;
            this.labelX11.Text = "課    號：";
            // 
            // Subject_SingleForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 17F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(391, 413);
            this.Controls.Add(this.txtNewSubjectCode);
            this.Controls.Add(this.labelX11);
            this.Controls.Add(this.nudCredit);
            this.Controls.Add(this.chkIsRequired);
            this.Controls.Add(this.txtRemark);
            this.Controls.Add(this.labelX4);
            this.Controls.Add(this.txtWebUrl);
            this.Controls.Add(this.labelX7);
            this.Controls.Add(this.txtDescription);
            this.Controls.Add(this.labelX8);
            this.Controls.Add(this.labelX9);
            this.Controls.Add(this.txtSubjectCode1);
            this.Controls.Add(this.labelX10);
            this.Controls.Add(this.labelX5);
            this.Controls.Add(this.txtEnglishName);
            this.Controls.Add(this.labelX6);
            this.Controls.Add(this.txtSubjectName);
            this.Controls.Add(this.labelX3);
            this.Controls.Add(this.txtDeptCode);
            this.Controls.Add(this.labelX2);
            this.Controls.Add(this.txtDept);
            this.Controls.Add(this.labelX1);
            this.Name = "Subject_SingleForm";
            this.Text = "編輯課程資訊";
            this.Load += new System.EventHandler(this.Subject_SingleForm_Load);
            this.Controls.SetChildIndex(this.labelX1, 0);
            this.Controls.SetChildIndex(this.txtDept, 0);
            this.Controls.SetChildIndex(this.labelX2, 0);
            this.Controls.SetChildIndex(this.txtDeptCode, 0);
            this.Controls.SetChildIndex(this.labelX3, 0);
            this.Controls.SetChildIndex(this.txtSubjectName, 0);
            this.Controls.SetChildIndex(this.labelX6, 0);
            this.Controls.SetChildIndex(this.txtEnglishName, 0);
            this.Controls.SetChildIndex(this.labelX5, 0);
            this.Controls.SetChildIndex(this.labelX10, 0);
            this.Controls.SetChildIndex(this.txtSubjectCode1, 0);
            this.Controls.SetChildIndex(this.labelX9, 0);
            this.Controls.SetChildIndex(this.labelX8, 0);
            this.Controls.SetChildIndex(this.txtDescription, 0);
            this.Controls.SetChildIndex(this.labelX7, 0);
            this.Controls.SetChildIndex(this.txtWebUrl, 0);
            this.Controls.SetChildIndex(this.labelX4, 0);
            this.Controls.SetChildIndex(this.txtRemark, 0);
            this.Controls.SetChildIndex(this.btnSave, 0);
            this.Controls.SetChildIndex(this.btnCancel, 0);
            this.Controls.SetChildIndex(this.chkIsRequired, 0);
            this.Controls.SetChildIndex(this.nudCredit, 0);
            this.Controls.SetChildIndex(this.labelX11, 0);
            this.Controls.SetChildIndex(this.txtNewSubjectCode, 0);
            ((System.ComponentModel.ISupportInitialize)(this.nudCredit)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.errorProvider1)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private DevComponents.DotNetBar.LabelX labelX1;
        private DevComponents.DotNetBar.Controls.TextBoxX txtDept;
        private DevComponents.DotNetBar.Controls.TextBoxX txtDeptCode;
        private DevComponents.DotNetBar.LabelX labelX2;
        private DevComponents.DotNetBar.Controls.TextBoxX txtSubjectName;
        private DevComponents.DotNetBar.LabelX labelX3;
        private DevComponents.DotNetBar.LabelX labelX5;
        private DevComponents.DotNetBar.Controls.TextBoxX txtEnglishName;
        private DevComponents.DotNetBar.LabelX labelX6;
        private DevComponents.DotNetBar.Controls.TextBoxX txtRemark;
        private DevComponents.DotNetBar.LabelX labelX4;
        private DevComponents.DotNetBar.Controls.TextBoxX txtWebUrl;
        private DevComponents.DotNetBar.LabelX labelX7;
        private DevComponents.DotNetBar.Controls.TextBoxX txtDescription;
        private DevComponents.DotNetBar.LabelX labelX8;
        private DevComponents.DotNetBar.Controls.TextBoxX txtSubjectCode1;
        private DevComponents.DotNetBar.LabelX labelX10;
        private DevComponents.DotNetBar.Controls.CheckBoxX chkIsRequired;
        private DevComponents.DotNetBar.LabelX labelX9;
        private System.Windows.Forms.NumericUpDown nudCredit;
        private System.Windows.Forms.ErrorProvider errorProvider1;
        private DevComponents.DotNetBar.Controls.TextBoxX txtNewSubjectCode;
        private DevComponents.DotNetBar.LabelX labelX11;
    }
}