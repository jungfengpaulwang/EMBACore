namespace EMBACore.DetailItems
{
    partial class Student_EducationBackground_SingleForm
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
            this.chkIsTop = new DevComponents.DotNetBar.Controls.CheckBoxX();
            this.Department = new DevComponents.DotNetBar.Controls.TextBoxX();
            this.labelX2 = new DevComponents.DotNetBar.LabelX();
            this.SchoolName = new DevComponents.DotNetBar.Controls.TextBoxX();
            this.labelX1 = new DevComponents.DotNetBar.LabelX();
            this.errorProvider1 = new System.Windows.Forms.ErrorProvider(this.components);
            this.labelX3 = new DevComponents.DotNetBar.LabelX();
            this.Degree = new DevComponents.DotNetBar.Controls.ComboBoxEx();
            this.comboItem1 = new DevComponents.Editors.ComboItem();
            this.comboItem2 = new DevComponents.Editors.ComboItem();
            this.comboItem3 = new DevComponents.Editors.ComboItem();
            this.comboItem4 = new DevComponents.Editors.ComboItem();
            ((System.ComponentModel.ISupportInitialize)(this.errorProvider1)).BeginInit();
            this.SuspendLayout();
            // 
            // btnSave
            // 
            this.btnSave.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnSave.Location = new System.Drawing.Point(161, 120);
            this.btnSave.TabIndex = 5;
            this.btnSave.Click += new System.EventHandler(this.btnSave_Click);
            // 
            // btnCancel
            // 
            this.btnCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnCancel.Location = new System.Drawing.Point(223, 120);
            this.btnCancel.TabIndex = 6;
            // 
            // chkIsTop
            // 
            this.chkIsTop.BackColor = System.Drawing.Color.Transparent;
            // 
            // 
            // 
            this.chkIsTop.BackgroundStyle.Class = "";
            this.chkIsTop.BackgroundStyle.CornerType = DevComponents.DotNetBar.eCornerType.Square;
            this.chkIsTop.Location = new System.Drawing.Point(-8, 106);
            this.chkIsTop.Name = "chkIsTop";
            this.chkIsTop.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
            this.chkIsTop.Size = new System.Drawing.Size(100, 23);
            this.chkIsTop.Style = DevComponents.DotNetBar.eDotNetBarStyle.StyleManagerControlled;
            this.chkIsTop.TabIndex = 4;
            this.chkIsTop.Text = "最高學歷";
            // 
            // Department
            // 
            // 
            // 
            // 
            this.Department.Border.Class = "TextBoxBorder";
            this.Department.Border.CornerType = DevComponents.DotNetBar.eCornerType.Square;
            this.Department.Location = new System.Drawing.Point(77, 43);
            this.Department.Name = "Department";
            this.Department.Size = new System.Drawing.Size(203, 25);
            this.Department.TabIndex = 2;
            // 
            // labelX2
            // 
            this.labelX2.BackColor = System.Drawing.Color.Transparent;
            // 
            // 
            // 
            this.labelX2.BackgroundStyle.Class = "";
            this.labelX2.BackgroundStyle.CornerType = DevComponents.DotNetBar.eCornerType.Square;
            this.labelX2.Location = new System.Drawing.Point(16, 43);
            this.labelX2.Name = "labelX2";
            this.labelX2.Size = new System.Drawing.Size(67, 23);
            this.labelX2.TabIndex = 11;
            this.labelX2.Text = "系        所";
            // 
            // SchoolName
            // 
            // 
            // 
            // 
            this.SchoolName.Border.Class = "TextBoxBorder";
            this.SchoolName.Border.CornerType = DevComponents.DotNetBar.eCornerType.Square;
            this.SchoolName.Location = new System.Drawing.Point(77, 12);
            this.SchoolName.Name = "SchoolName";
            this.SchoolName.Size = new System.Drawing.Size(203, 25);
            this.SchoolName.TabIndex = 1;
            // 
            // labelX1
            // 
            this.labelX1.BackColor = System.Drawing.Color.Transparent;
            // 
            // 
            // 
            this.labelX1.BackgroundStyle.Class = "";
            this.labelX1.BackgroundStyle.CornerType = DevComponents.DotNetBar.eCornerType.Square;
            this.labelX1.Location = new System.Drawing.Point(16, 12);
            this.labelX1.Name = "labelX1";
            this.labelX1.Size = new System.Drawing.Size(67, 23);
            this.labelX1.TabIndex = 9;
            this.labelX1.Text = "學校名稱";
            // 
            // errorProvider1
            // 
            this.errorProvider1.ContainerControl = this;
            // 
            // labelX3
            // 
            this.labelX3.BackColor = System.Drawing.Color.Transparent;
            // 
            // 
            // 
            this.labelX3.BackgroundStyle.Class = "";
            this.labelX3.BackgroundStyle.CornerType = DevComponents.DotNetBar.eCornerType.Square;
            this.labelX3.Location = new System.Drawing.Point(16, 72);
            this.labelX3.Name = "labelX3";
            this.labelX3.Size = new System.Drawing.Size(67, 23);
            this.labelX3.TabIndex = 16;
            this.labelX3.Text = "學        位";
            // 
            // Degree
            // 
            this.Degree.DisplayMember = "Text";
            this.Degree.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawFixed;
            this.Degree.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.Degree.FormattingEnabled = true;
            this.Degree.ItemHeight = 19;
            this.Degree.Items.AddRange(new object[] {
            this.comboItem1,
            this.comboItem2,
            this.comboItem3,
            this.comboItem4});
            this.Degree.Location = new System.Drawing.Point(77, 74);
            this.Degree.Name = "Degree";
            this.Degree.Size = new System.Drawing.Size(203, 25);
            this.Degree.Style = DevComponents.DotNetBar.eDotNetBarStyle.StyleManagerControlled;
            this.Degree.TabIndex = 3;
            // 
            // comboItem1
            // 
            this.comboItem1.Text = "博士";
            // 
            // comboItem2
            // 
            this.comboItem2.Text = "碩士";
            // 
            // comboItem3
            // 
            this.comboItem3.Text = "學士";
            // 
            // comboItem4
            // 
            this.comboItem4.Text = "其它";
            // 
            // Student_EducationBackground_SingleForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 17F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(287, 152);
            this.Controls.Add(this.Degree);
            this.Controls.Add(this.labelX3);
            this.Controls.Add(this.chkIsTop);
            this.Controls.Add(this.Department);
            this.Controls.Add(this.labelX2);
            this.Controls.Add(this.SchoolName);
            this.Controls.Add(this.labelX1);
            this.Name = "Student_EducationBackground_SingleForm";
            this.Text = "編輯學歷資料";
            this.Controls.SetChildIndex(this.btnSave, 0);
            this.Controls.SetChildIndex(this.btnCancel, 0);
            this.Controls.SetChildIndex(this.labelX1, 0);
            this.Controls.SetChildIndex(this.SchoolName, 0);
            this.Controls.SetChildIndex(this.labelX2, 0);
            this.Controls.SetChildIndex(this.Department, 0);
            this.Controls.SetChildIndex(this.chkIsTop, 0);
            this.Controls.SetChildIndex(this.labelX3, 0);
            this.Controls.SetChildIndex(this.Degree, 0);
            ((System.ComponentModel.ISupportInitialize)(this.errorProvider1)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private DevComponents.DotNetBar.Controls.CheckBoxX chkIsTop;
        private DevComponents.DotNetBar.Controls.TextBoxX Department;
        private DevComponents.DotNetBar.LabelX labelX2;
        private DevComponents.DotNetBar.Controls.TextBoxX SchoolName;
        private DevComponents.DotNetBar.LabelX labelX1;
        private System.Windows.Forms.ErrorProvider errorProvider1;
        private DevComponents.DotNetBar.Controls.ComboBoxEx Degree;
        private DevComponents.Editors.ComboItem comboItem1;
        private DevComponents.Editors.ComboItem comboItem2;
        private DevComponents.Editors.ComboItem comboItem3;
        private DevComponents.Editors.ComboItem comboItem4;
        private DevComponents.DotNetBar.LabelX labelX3;
    }
}