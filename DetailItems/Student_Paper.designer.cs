namespace EMBACore.DetailItems
{
    partial class Student_Paper
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
            this.txtPublishedDate = new DevComponents.DotNetBar.Controls.TextBoxX();
            this.label3 = new System.Windows.Forms.Label();
            this.label10 = new System.Windows.Forms.Label();
            this.txtPaperName = new DevComponents.DotNetBar.Controls.TextBoxX();
            this.label9 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.txtDescription = new DevComponents.DotNetBar.Controls.TextBoxX();
            this.txtAdvisor1 = new DevComponents.DotNetBar.Controls.TextBoxX();
            this.txtAdvisor2 = new DevComponents.DotNetBar.Controls.TextBoxX();
            this.label4 = new System.Windows.Forms.Label();
            this.txtAdvisor3 = new DevComponents.DotNetBar.Controls.TextBoxX();
            this.label5 = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.label7 = new System.Windows.Forms.Label();
            this.cboSemester = new DevComponents.DotNetBar.Controls.ComboBoxEx();
            this.nudSchoolYear = new DevComponents.DotNetBar.Controls.TextBoxX();
            this.IsPublic = new DevComponents.DotNetBar.Controls.SwitchButton();
            this.label2 = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // txtPublishedDate
            // 
            // 
            // 
            // 
            this.txtPublishedDate.Border.Class = "TextBoxBorder";
            this.txtPublishedDate.Border.CornerType = DevComponents.DotNetBar.eCornerType.Square;
            this.txtPublishedDate.Location = new System.Drawing.Point(363, 246);
            this.txtPublishedDate.Name = "txtPublishedDate";
            this.txtPublishedDate.Size = new System.Drawing.Size(140, 25);
            this.txtPublishedDate.TabIndex = 9;
            this.txtPublishedDate.WatermarkFont = new System.Drawing.Font("新細明體", 8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.txtPublishedDate.WatermarkText = "範例：延後至106年1月1日";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(259, 251);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(86, 17);
            this.label3.TabIndex = 19;
            this.label3.Text = "延後公開期限";
            // 
            // label10
            // 
            this.label10.AutoSize = true;
            this.label10.Location = new System.Drawing.Point(28, 55);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(60, 17);
            this.label10.TabIndex = 30;
            this.label10.Text = "書籍狀況";
            // 
            // txtPaperName
            // 
            // 
            // 
            // 
            this.txtPaperName.Border.Class = "TextBoxBorder";
            this.txtPaperName.Border.CornerType = DevComponents.DotNetBar.eCornerType.Square;
            this.txtPaperName.Location = new System.Drawing.Point(106, 16);
            this.txtPaperName.Name = "txtPaperName";
            this.txtPaperName.Size = new System.Drawing.Size(397, 25);
            this.txtPaperName.TabIndex = 1;
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Location = new System.Drawing.Point(28, 18);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(60, 17);
            this.label9.TabIndex = 36;
            this.label9.Text = "論文題目";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(19, 93);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(68, 17);
            this.label1.TabIndex = 41;
            this.label1.Text = "指導教授1";
            // 
            // txtDescription
            // 
            // 
            // 
            // 
            this.txtDescription.Border.Class = "TextBoxBorder";
            this.txtDescription.Border.CornerType = DevComponents.DotNetBar.eCornerType.Square;
            this.txtDescription.Location = new System.Drawing.Point(106, 53);
            this.txtDescription.Name = "txtDescription";
            this.txtDescription.Size = new System.Drawing.Size(397, 25);
            this.txtDescription.TabIndex = 2;
            // 
            // txtAdvisor1
            // 
            // 
            // 
            // 
            this.txtAdvisor1.Border.Class = "TextBoxBorder";
            this.txtAdvisor1.Border.CornerType = DevComponents.DotNetBar.eCornerType.Square;
            this.txtAdvisor1.Location = new System.Drawing.Point(106, 89);
            this.txtAdvisor1.Name = "txtAdvisor1";
            this.txtAdvisor1.Size = new System.Drawing.Size(397, 25);
            this.txtAdvisor1.TabIndex = 3;
            // 
            // txtAdvisor2
            // 
            // 
            // 
            // 
            this.txtAdvisor2.Border.Class = "TextBoxBorder";
            this.txtAdvisor2.Border.CornerType = DevComponents.DotNetBar.eCornerType.Square;
            this.txtAdvisor2.Location = new System.Drawing.Point(106, 129);
            this.txtAdvisor2.Name = "txtAdvisor2";
            this.txtAdvisor2.Size = new System.Drawing.Size(397, 25);
            this.txtAdvisor2.TabIndex = 4;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(20, 133);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(68, 17);
            this.label4.TabIndex = 45;
            this.label4.Text = "指導教授2";
            // 
            // txtAdvisor3
            // 
            // 
            // 
            // 
            this.txtAdvisor3.Border.Class = "TextBoxBorder";
            this.txtAdvisor3.Border.CornerType = DevComponents.DotNetBar.eCornerType.Square;
            this.txtAdvisor3.Location = new System.Drawing.Point(106, 169);
            this.txtAdvisor3.Name = "txtAdvisor3";
            this.txtAdvisor3.Size = new System.Drawing.Size(397, 25);
            this.txtAdvisor3.TabIndex = 5;
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(20, 173);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(68, 17);
            this.label5.TabIndex = 47;
            this.label5.Text = "指導教授3";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(15, 210);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(73, 17);
            this.label6.TabIndex = 49;
            this.label6.Text = "畢業學年度";
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(285, 210);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(60, 17);
            this.label7.TabIndex = 51;
            this.label7.Text = "畢業學期";
            // 
            // cboSemester
            // 
            this.cboSemester.DisplayMember = "Text";
            this.cboSemester.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawFixed;
            this.cboSemester.FormattingEnabled = true;
            this.cboSemester.ItemHeight = 19;
            this.cboSemester.Location = new System.Drawing.Point(363, 209);
            this.cboSemester.Name = "cboSemester";
            this.cboSemester.Size = new System.Drawing.Size(140, 25);
            this.cboSemester.Style = DevComponents.DotNetBar.eDotNetBarStyle.StyleManagerControlled;
            this.cboSemester.TabIndex = 7;
            // 
            // nudSchoolYear
            // 
            // 
            // 
            // 
            this.nudSchoolYear.Border.Class = "TextBoxBorder";
            this.nudSchoolYear.Border.CornerType = DevComponents.DotNetBar.eCornerType.Square;
            this.nudSchoolYear.Location = new System.Drawing.Point(106, 209);
            this.nudSchoolYear.Name = "nudSchoolYear";
            this.nudSchoolYear.Size = new System.Drawing.Size(140, 25);
            this.nudSchoolYear.TabIndex = 6;
            // 
            // IsPublic
            // 
            // 
            // 
            // 
            this.IsPublic.BackgroundStyle.Class = "";
            this.IsPublic.BackgroundStyle.CornerType = DevComponents.DotNetBar.eCornerType.Square;
            this.IsPublic.Location = new System.Drawing.Point(106, 248);
            this.IsPublic.Name = "IsPublic";
            this.IsPublic.OffBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(192)))), ((int)(((byte)(192)))));
            this.IsPublic.OffText = "否";
            this.IsPublic.OnBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(255)))), ((int)(((byte)(128)))));
            this.IsPublic.OnText = "是";
            this.IsPublic.Size = new System.Drawing.Size(140, 23);
            this.IsPublic.Style = DevComponents.DotNetBar.eDotNetBarStyle.StyleManagerControlled;
            this.IsPublic.TabIndex = 53;
            this.IsPublic.Value = true;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(27, 242);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(60, 34);
            this.label2.TabIndex = 49;
            this.label2.Text = "是否公開\n紙本論文";
            // 
            // Student_Paper
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 17F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.IsPublic);
            this.Controls.Add(this.nudSchoolYear);
            this.Controls.Add(this.cboSemester);
            this.Controls.Add(this.label7);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label6);
            this.Controls.Add(this.txtAdvisor3);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.txtAdvisor2);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.txtAdvisor1);
            this.Controls.Add(this.txtDescription);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.txtPaperName);
            this.Controls.Add(this.label9);
            this.Controls.Add(this.label10);
            this.Controls.Add(this.txtPublishedDate);
            this.Controls.Add(this.label3);
            this.Group = "教師其他資訊";
            this.Name = "Student_Paper";
            this.Size = new System.Drawing.Size(550, 285);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private DevComponents.DotNetBar.Controls.TextBoxX txtPublishedDate;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label10;
        private DevComponents.DotNetBar.Controls.TextBoxX txtPaperName;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.Label label1;
        private DevComponents.DotNetBar.Controls.TextBoxX txtDescription;
        private DevComponents.DotNetBar.Controls.TextBoxX txtAdvisor1;
        private DevComponents.DotNetBar.Controls.TextBoxX txtAdvisor2;
        private System.Windows.Forms.Label label4;
        private DevComponents.DotNetBar.Controls.TextBoxX txtAdvisor3;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Label label7;
        private DevComponents.DotNetBar.Controls.ComboBoxEx cboSemester;
        private DevComponents.DotNetBar.Controls.TextBoxX nudSchoolYear;
        private DevComponents.DotNetBar.Controls.SwitchButton IsPublic;
        private System.Windows.Forms.Label label2;
    }
}
