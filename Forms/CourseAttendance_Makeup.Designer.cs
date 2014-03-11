namespace EMBACore.Forms
{
    partial class CourseAttendance_Makeup
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
            this.labelX1 = new DevComponents.DotNetBar.LabelX();
            this.txtMakeup = new DevComponents.DotNetBar.Controls.TextBoxX();
            this.chkIsMakeup = new DevComponents.DotNetBar.Controls.CheckBoxX();
            this.btnOk = new DevComponents.DotNetBar.ButtonX();
            this.SuspendLayout();
            // 
            // labelX1
            // 
            this.labelX1.BackColor = System.Drawing.Color.Transparent;
            // 
            // 
            // 
            this.labelX1.BackgroundStyle.Class = "";
            this.labelX1.BackgroundStyle.CornerType = DevComponents.DotNetBar.eCornerType.Square;
            this.labelX1.Enabled = false;
            this.labelX1.Location = new System.Drawing.Point(1, 32);
            this.labelX1.Name = "labelX1";
            this.labelX1.Size = new System.Drawing.Size(75, 23);
            this.labelX1.TabIndex = 0;
            this.labelX1.Text = "補課描述";
            // 
            // txtMakeup
            // 
            // 
            // 
            // 
            this.txtMakeup.Border.Class = "TextBoxBorder";
            this.txtMakeup.Border.CornerType = DevComponents.DotNetBar.eCornerType.Square;
            this.txtMakeup.Enabled = false;
            this.txtMakeup.Location = new System.Drawing.Point(59, 34);
            this.txtMakeup.Multiline = true;
            this.txtMakeup.Name = "txtMakeup";
            this.txtMakeup.Size = new System.Drawing.Size(197, 124);
            this.txtMakeup.TabIndex = 1;
            // 
            // chkIsMakeup
            // 
            this.chkIsMakeup.BackColor = System.Drawing.Color.Transparent;
            // 
            // 
            // 
            this.chkIsMakeup.BackgroundStyle.Class = "";
            this.chkIsMakeup.BackgroundStyle.CornerType = DevComponents.DotNetBar.eCornerType.Square;
            this.chkIsMakeup.CheckBoxPosition = DevComponents.DotNetBar.eCheckBoxPosition.Right;
            this.chkIsMakeup.Location = new System.Drawing.Point(-1, 3);
            this.chkIsMakeup.Name = "chkIsMakeup";
            this.chkIsMakeup.Size = new System.Drawing.Size(77, 23);
            this.chkIsMakeup.Style = DevComponents.DotNetBar.eDotNetBarStyle.StyleManagerControlled;
            this.chkIsMakeup.TabIndex = 2;
            this.chkIsMakeup.Text = "是否補課";
            this.chkIsMakeup.CheckedChanged += new System.EventHandler(this.chkIsMakeup_CheckedChanged);
            // 
            // btnOk
            // 
            this.btnOk.AccessibleRole = System.Windows.Forms.AccessibleRole.PushButton;
            this.btnOk.BackColor = System.Drawing.Color.Transparent;
            this.btnOk.ColorTable = DevComponents.DotNetBar.eButtonColor.OrangeWithBackground;
            this.btnOk.Location = new System.Drawing.Point(180, 165);
            this.btnOk.Name = "btnOk";
            this.btnOk.Size = new System.Drawing.Size(75, 34);
            this.btnOk.Style = DevComponents.DotNetBar.eDotNetBarStyle.StyleManagerControlled;
            this.btnOk.TabIndex = 3;
            this.btnOk.Text = "確定";
            this.btnOk.Click += new System.EventHandler(this.btnOk_Click);
            // 
            // CourseAttendance_Makeup
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 17F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(261, 205);
            this.Controls.Add(this.btnOk);
            this.Controls.Add(this.chkIsMakeup);
            this.Controls.Add(this.txtMakeup);
            this.Controls.Add(this.labelX1);
            this.DoubleBuffered = true;
            this.Name = "CourseAttendance_Makeup";
            this.Text = "輸入補課資訊";
            this.Load += new System.EventHandler(this.CourseAttendance_Makeup_Load);
            this.ResumeLayout(false);

        }

        #endregion

        private DevComponents.DotNetBar.LabelX labelX1;
        private DevComponents.DotNetBar.Controls.TextBoxX txtMakeup;
        private DevComponents.DotNetBar.Controls.CheckBoxX chkIsMakeup;
        private DevComponents.DotNetBar.ButtonX btnOk;
    }
}