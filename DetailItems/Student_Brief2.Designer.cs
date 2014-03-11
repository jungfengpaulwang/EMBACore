namespace EMBACore.DetailItems
{
    partial class Student_Brief2
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
            this.labelX1 = new DevComponents.DotNetBar.LabelX();
            this.txtGraduateYear = new DevComponents.DotNetBar.Controls.TextBoxX();
            this.txtGraduateSemester = new DevComponents.DotNetBar.Controls.TextBoxX();
            this.labelX2 = new DevComponents.DotNetBar.LabelX();
            this.SuspendLayout();
            // 
            // labelX1
            // 
            this.labelX1.BackColor = System.Drawing.Color.PaleGreen;
            // 
            // 
            // 
            this.labelX1.BackgroundStyle.Class = "";
            this.labelX1.BackgroundStyle.CornerType = DevComponents.DotNetBar.eCornerType.Square;
            this.labelX1.Location = new System.Drawing.Point(29, 21);
            this.labelX1.Name = "labelX1";
            this.labelX1.Size = new System.Drawing.Size(78, 23);
            this.labelX1.TabIndex = 1;
            this.labelX1.Text = "畢業學年度";
            // 
            // txtGraduateYear
            // 
            // 
            // 
            // 
            this.txtGraduateYear.Border.Class = "TextBoxBorder";
            this.txtGraduateYear.Border.CornerType = DevComponents.DotNetBar.eCornerType.Square;
            this.txtGraduateYear.Enabled = false;
            this.txtGraduateYear.Location = new System.Drawing.Point(113, 21);
            this.txtGraduateYear.Name = "txtGraduateYear";
            this.txtGraduateYear.Size = new System.Drawing.Size(138, 25);
            this.txtGraduateYear.TabIndex = 1;
            // 
            // txtGraduateSemester
            // 
            // 
            // 
            // 
            this.txtGraduateSemester.Border.Class = "TextBoxBorder";
            this.txtGraduateSemester.Border.CornerType = DevComponents.DotNetBar.eCornerType.Square;
            this.txtGraduateSemester.Enabled = false;
            this.txtGraduateSemester.Location = new System.Drawing.Point(371, 23);
            this.txtGraduateSemester.Name = "txtGraduateSemester";
            this.txtGraduateSemester.Size = new System.Drawing.Size(138, 25);
            this.txtGraduateSemester.TabIndex = 2;
            // 
            // labelX2
            // 
            this.labelX2.BackColor = System.Drawing.Color.PaleGreen;
            // 
            // 
            // 
            this.labelX2.BackgroundStyle.Class = "";
            this.labelX2.BackgroundStyle.CornerType = DevComponents.DotNetBar.eCornerType.Square;
            this.labelX2.Location = new System.Drawing.Point(301, 23);
            this.labelX2.Name = "labelX2";
            this.labelX2.Size = new System.Drawing.Size(69, 23);
            this.labelX2.TabIndex = 3;
            this.labelX2.Text = "畢業學期";
            // 
            // Student_Brief2
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 17F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.txtGraduateSemester);
            this.Controls.Add(this.labelX2);
            this.Controls.Add(this.txtGraduateYear);
            this.Controls.Add(this.labelX1);
            this.Name = "Student_Brief2";
            this.Size = new System.Drawing.Size(550, 70);
            this.Load += new System.EventHandler(this.Student_Brief2_Load);
            this.ResumeLayout(false);

        }

        #endregion

        private DevComponents.DotNetBar.LabelX labelX1;
        private DevComponents.DotNetBar.Controls.TextBoxX txtGraduateYear;
        private DevComponents.DotNetBar.Controls.TextBoxX txtGraduateSemester;
        private DevComponents.DotNetBar.LabelX labelX2;

    }
}
