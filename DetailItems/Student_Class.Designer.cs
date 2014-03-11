namespace EMBACore.DetailItems
{
    partial class Student_Class
    {
        /// <summary> 
        /// 設計工具所需的變數。
        /// </summary>
        private System.ComponentModel.IContainer components = null; 

        /// <summary> 
        /// 清除任何使用中的資源。
        /// </summary>
        /// <param name="disposing">如果應該處置 Managed 資源則為 true，否則為 false。</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region 元件設計工具產生的程式碼

        /// <summary> 
        /// 此為設計工具支援所需的方法 - 請勿使用程式碼編輯器
        /// 修改這個方法的內容。
        /// </summary>
        private void InitializeComponent()
        {
            this.txtStudentNumber = new DevComponents.DotNetBar.Controls.TextBoxX();
            this.label41 = new System.Windows.Forms.Label();
            this.label38 = new System.Windows.Forms.Label();
            this.cboClass = new DevComponents.DotNetBar.Controls.ComboBoxEx();
            this.Circular = new DevComponents.DotNetBar.Controls.CircularProgress();
            this.cboSeatNo = new DevComponents.DotNetBar.Controls.ComboBoxEx();
            this.txtSchoolYear = new DevComponents.DotNetBar.Controls.TextBoxX();
            this.label1 = new System.Windows.Forms.Label();
            this.txtClassYear = new DevComponents.DotNetBar.Controls.TextBoxX();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.cboDepartmentGroup = new DevComponents.DotNetBar.Controls.ComboBoxEx();
            this.label4 = new System.Windows.Forms.Label();
            this.Code = new DevComponents.DotNetBar.Controls.TextBoxX();
            this.SuspendLayout();
            // 
            // txtStudentNumber
            // 
            // 
            // 
            // 
            this.txtStudentNumber.Border.Class = "TextBoxBorder";
            this.txtStudentNumber.Border.CornerType = DevComponents.DotNetBar.eCornerType.Square;
            this.txtStudentNumber.Enabled = false;
            this.txtStudentNumber.Location = new System.Drawing.Point(96, 15);
            this.txtStudentNumber.Margin = new System.Windows.Forms.Padding(4);
            this.txtStudentNumber.Name = "txtStudentNumber";
            this.txtStudentNumber.Size = new System.Drawing.Size(119, 25);
            this.txtStudentNumber.TabIndex = 1;
            // 
            // label41
            // 
            this.label41.AutoSize = true;
            this.label41.BackColor = System.Drawing.Color.Transparent;
            this.label41.ForeColor = System.Drawing.Color.Black;
            this.label41.Location = new System.Drawing.Point(300, 19);
            this.label41.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label41.Name = "label41";
            this.label41.Size = new System.Drawing.Size(60, 17);
            this.label41.TabIndex = 207;
            this.label41.Text = "入學年度";
            this.label41.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // label38
            // 
            this.label38.AutoSize = true;
            this.label38.BackColor = System.Drawing.Color.Transparent;
            this.label38.ForeColor = System.Drawing.Color.Black;
            this.label38.Location = new System.Drawing.Point(300, 56);
            this.label38.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label38.Name = "label38";
            this.label38.Size = new System.Drawing.Size(60, 17);
            this.label38.TabIndex = 206;
            this.label38.Text = "教學分班";
            this.label38.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // cboClass
            // 
            this.cboClass.DisplayMember = "Text";
            this.cboClass.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawFixed;
            this.cboClass.FormattingEnabled = true;
            this.cboClass.ItemHeight = 19;
            this.cboClass.Location = new System.Drawing.Point(368, 52);
            this.cboClass.Margin = new System.Windows.Forms.Padding(4);
            this.cboClass.Name = "cboClass";
            this.cboClass.Size = new System.Drawing.Size(119, 25);
            this.cboClass.TabIndex = 4;
            this.cboClass.SelectedValueChanged += new System.EventHandler(this.cboClass_SelectedValueChanged);
            this.cboClass.TextChanged += new System.EventHandler(this.cboClass_TextChanged);
            // 
            // Circular
            // 
            // 
            // 
            // 
            this.Circular.BackgroundStyle.Class = "";
            this.Circular.BackgroundStyle.CornerType = DevComponents.DotNetBar.eCornerType.Square;
            this.Circular.Location = new System.Drawing.Point(513, 47);
            this.Circular.Name = "Circular";
            this.Circular.Size = new System.Drawing.Size(30, 23);
            this.Circular.Style = DevComponents.DotNetBar.eDotNetBarStyle.OfficeXP;
            this.Circular.TabIndex = 211;
            this.Circular.Visible = false;
            // 
            // cboSeatNo
            // 
            this.cboSeatNo.DisplayMember = "Text";
            this.cboSeatNo.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawFixed;
            this.cboSeatNo.FormattingEnabled = true;
            this.cboSeatNo.ItemHeight = 19;
            this.cboSeatNo.Location = new System.Drawing.Point(513, 20);
            this.cboSeatNo.Margin = new System.Windows.Forms.Padding(4);
            this.cboSeatNo.Name = "cboSeatNo";
            this.cboSeatNo.Size = new System.Drawing.Size(29, 25);
            this.cboSeatNo.TabIndex = 212;
            this.cboSeatNo.Visible = false;
            // 
            // txtSchoolYear
            // 
            // 
            // 
            // 
            this.txtSchoolYear.Border.Class = "TextBoxBorder";
            this.txtSchoolYear.Border.CornerType = DevComponents.DotNetBar.eCornerType.Square;
            this.txtSchoolYear.Location = new System.Drawing.Point(368, 15);
            this.txtSchoolYear.Margin = new System.Windows.Forms.Padding(4);
            this.txtSchoolYear.Name = "txtSchoolYear";
            this.txtSchoolYear.Size = new System.Drawing.Size(119, 25);
            this.txtSchoolYear.TabIndex = 2;
            this.txtSchoolYear.Leave += new System.EventHandler(this.txtSchoolYear_Leave);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.BackColor = System.Drawing.Color.PaleGreen;
            this.label1.ForeColor = System.Drawing.Color.Black;
            this.label1.Location = new System.Drawing.Point(28, 18);
            this.label1.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(60, 17);
            this.label1.TabIndex = 214;
            this.label1.Text = "學　　號";
            this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // txtClassYear
            // 
            // 
            // 
            // 
            this.txtClassYear.Border.Class = "TextBoxBorder";
            this.txtClassYear.Border.CornerType = DevComponents.DotNetBar.eCornerType.Square;
            this.txtClassYear.Enabled = false;
            this.txtClassYear.Location = new System.Drawing.Point(96, 54);
            this.txtClassYear.Margin = new System.Windows.Forms.Padding(4);
            this.txtClassYear.Name = "txtClassYear";
            this.txtClassYear.Size = new System.Drawing.Size(119, 25);
            this.txtClassYear.TabIndex = 3;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.BackColor = System.Drawing.Color.PaleGreen;
            this.label2.ForeColor = System.Drawing.Color.Black;
            this.label2.Location = new System.Drawing.Point(28, 58);
            this.label2.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(60, 17);
            this.label2.TabIndex = 216;
            this.label2.Text = "年　　級";
            this.label2.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.BackColor = System.Drawing.Color.Transparent;
            this.label3.ForeColor = System.Drawing.Color.Black;
            this.label3.Location = new System.Drawing.Point(300, 98);
            this.label3.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(60, 17);
            this.label3.TabIndex = 218;
            this.label3.Text = "系所組別";
            this.label3.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // cboDepartmentGroup
            // 
            this.cboDepartmentGroup.DisplayMember = "Text";
            this.cboDepartmentGroup.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawFixed;
            this.cboDepartmentGroup.FormattingEnabled = true;
            this.cboDepartmentGroup.ItemHeight = 19;
            this.cboDepartmentGroup.Location = new System.Drawing.Point(368, 94);
            this.cboDepartmentGroup.Margin = new System.Windows.Forms.Padding(4);
            this.cboDepartmentGroup.Name = "cboDepartmentGroup";
            this.cboDepartmentGroup.Size = new System.Drawing.Size(119, 25);
            this.cboDepartmentGroup.TabIndex = 5;
            this.cboDepartmentGroup.TextChanged += new System.EventHandler(this.cboDepartmentGroup_TextChanged);
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.BackColor = System.Drawing.Color.PaleGreen;
            this.label4.ForeColor = System.Drawing.Color.Black;
            this.label4.Location = new System.Drawing.Point(28, 98);
            this.label4.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(60, 17);
            this.label4.TabIndex = 219;
            this.label4.Text = "系所代碼";
            this.label4.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // Code
            // 
            // 
            // 
            // 
            this.Code.Border.Class = "TextBoxBorder";
            this.Code.Border.CornerType = DevComponents.DotNetBar.eCornerType.Square;
            this.Code.Enabled = false;
            this.Code.Location = new System.Drawing.Point(96, 94);
            this.Code.Margin = new System.Windows.Forms.Padding(4);
            this.Code.Name = "Code";
            this.Code.Size = new System.Drawing.Size(119, 25);
            this.Code.TabIndex = 220;
            this.Code.Leave += new System.EventHandler(this.Code_Leave);
            // 
            // Student_Class
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 17F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.Code);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.cboDepartmentGroup);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.txtClassYear);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.txtSchoolYear);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.cboSeatNo);
            this.Controls.Add(this.cboClass);
            this.Controls.Add(this.txtStudentNumber);
            this.Controls.Add(this.label41);
            this.Controls.Add(this.label38);
            this.Controls.Add(this.Circular);
            this.Name = "Student_Class";
            this.Size = new System.Drawing.Size(550, 135);
            this.Load += new System.EventHandler(this.Student_Class_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        internal DevComponents.DotNetBar.Controls.TextBoxX txtStudentNumber;
        private System.Windows.Forms.Label label41;
        private System.Windows.Forms.Label label38;
        private DevComponents.DotNetBar.Controls.ComboBoxEx cboClass;
        private DevComponents.DotNetBar.Controls.CircularProgress Circular;
        private DevComponents.DotNetBar.Controls.ComboBoxEx cboSeatNo;
        internal DevComponents.DotNetBar.Controls.TextBoxX txtSchoolYear;
        private System.Windows.Forms.Label label1;
        internal DevComponents.DotNetBar.Controls.TextBoxX txtClassYear;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private DevComponents.DotNetBar.Controls.ComboBoxEx cboDepartmentGroup;
        private System.Windows.Forms.Label label4;
        internal DevComponents.DotNetBar.Controls.TextBoxX Code;
    }
}
