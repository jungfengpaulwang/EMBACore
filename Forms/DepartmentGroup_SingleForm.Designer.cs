namespace EMBACore.Forms
{
    partial class DepartmentGroup_SingleForm
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
            this.txtDeptCode = new DevComponents.DotNetBar.Controls.TextBoxX();
            this.labelX2 = new DevComponents.DotNetBar.LabelX();
            this.txtSubjectName = new DevComponents.DotNetBar.Controls.TextBoxX();
            this.labelX3 = new DevComponents.DotNetBar.LabelX();
            this.txtEnglishName = new DevComponents.DotNetBar.Controls.TextBoxX();
            this.labelX6 = new DevComponents.DotNetBar.LabelX();
            this.errorProvider1 = new System.Windows.Forms.ErrorProvider(this.components);
            this.txtOrder = new DevComponents.DotNetBar.Controls.TextBoxX();
            this.labelX1 = new DevComponents.DotNetBar.LabelX();
            ((System.ComponentModel.ISupportInitialize)(this.errorProvider1)).BeginInit();
            this.SuspendLayout();
            // 
            // btnSave
            // 
            this.btnSave.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnSave.Location = new System.Drawing.Point(259, 145);
            // 
            // btnCancel
            // 
            this.btnCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnCancel.Location = new System.Drawing.Point(321, 145);
            // 
            // txtDeptCode
            // 
            // 
            // 
            // 
            this.txtDeptCode.Border.Class = "TextBoxBorder";
            this.txtDeptCode.Border.CornerType = DevComponents.DotNetBar.eCornerType.Square;
            this.txtDeptCode.Location = new System.Drawing.Point(136, 76);
            this.txtDeptCode.Name = "txtDeptCode";
            this.txtDeptCode.Size = new System.Drawing.Size(243, 25);
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
            this.labelX2.Location = new System.Drawing.Point(13, 78);
            this.labelX2.Name = "labelX2";
            this.labelX2.Size = new System.Drawing.Size(107, 23);
            this.labelX2.TabIndex = 4;
            this.labelX2.Text = "系所組別代碼";
            this.labelX2.TextAlignment = System.Drawing.StringAlignment.Far;
            // 
            // txtSubjectName
            // 
            // 
            // 
            // 
            this.txtSubjectName.Border.Class = "TextBoxBorder";
            this.txtSubjectName.Border.CornerType = DevComponents.DotNetBar.eCornerType.Square;
            this.txtSubjectName.Location = new System.Drawing.Point(136, 14);
            this.txtSubjectName.Name = "txtSubjectName";
            this.txtSubjectName.Size = new System.Drawing.Size(243, 25);
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
            this.labelX3.Size = new System.Drawing.Size(118, 23);
            this.labelX3.TabIndex = 6;
            this.labelX3.Text = "系所組別中文名稱";
            // 
            // txtEnglishName
            // 
            // 
            // 
            // 
            this.txtEnglishName.Border.Class = "TextBoxBorder";
            this.txtEnglishName.Border.CornerType = DevComponents.DotNetBar.eCornerType.Square;
            this.txtEnglishName.Location = new System.Drawing.Point(136, 45);
            this.txtEnglishName.Name = "txtEnglishName";
            this.txtEnglishName.Size = new System.Drawing.Size(243, 25);
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
            this.labelX6.Size = new System.Drawing.Size(118, 23);
            this.labelX6.TabIndex = 8;
            this.labelX6.Text = "系所組別英文名稱";
            // 
            // errorProvider1
            // 
            this.errorProvider1.ContainerControl = this;
            // 
            // txtOrder
            // 
            // 
            // 
            // 
            this.txtOrder.Border.Class = "TextBoxBorder";
            this.txtOrder.Border.CornerType = DevComponents.DotNetBar.eCornerType.Square;
            this.txtOrder.Location = new System.Drawing.Point(136, 107);
            this.txtOrder.Name = "txtOrder";
            this.txtOrder.Size = new System.Drawing.Size(243, 25);
            this.txtOrder.TabIndex = 11;
            // 
            // labelX1
            // 
            this.labelX1.BackColor = System.Drawing.Color.Transparent;
            // 
            // 
            // 
            this.labelX1.BackgroundStyle.Class = "";
            this.labelX1.BackgroundStyle.CornerType = DevComponents.DotNetBar.eCornerType.Square;
            this.labelX1.Location = new System.Drawing.Point(13, 109);
            this.labelX1.Name = "labelX1";
            this.labelX1.Size = new System.Drawing.Size(107, 23);
            this.labelX1.TabIndex = 10;
            this.labelX1.Text = "排列順序";
            this.labelX1.TextAlignment = System.Drawing.StringAlignment.Far;
            // 
            // DepartmentGroup_SingleForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 17F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(391, 181);
            this.Controls.Add(this.txtOrder);
            this.Controls.Add(this.labelX1);
            this.Controls.Add(this.txtEnglishName);
            this.Controls.Add(this.labelX6);
            this.Controls.Add(this.txtSubjectName);
            this.Controls.Add(this.labelX3);
            this.Controls.Add(this.txtDeptCode);
            this.Controls.Add(this.labelX2);
            this.Name = "DepartmentGroup_SingleForm";
            this.Text = "編輯系所組別資訊";
            this.Load += new System.EventHandler(this.Subject_SingleForm_Load);
            this.Controls.SetChildIndex(this.labelX2, 0);
            this.Controls.SetChildIndex(this.txtDeptCode, 0);
            this.Controls.SetChildIndex(this.labelX3, 0);
            this.Controls.SetChildIndex(this.txtSubjectName, 0);
            this.Controls.SetChildIndex(this.labelX6, 0);
            this.Controls.SetChildIndex(this.txtEnglishName, 0);
            this.Controls.SetChildIndex(this.btnSave, 0);
            this.Controls.SetChildIndex(this.btnCancel, 0);
            this.Controls.SetChildIndex(this.labelX1, 0);
            this.Controls.SetChildIndex(this.txtOrder, 0);
            ((System.ComponentModel.ISupportInitialize)(this.errorProvider1)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private DevComponents.DotNetBar.Controls.TextBoxX txtDeptCode;
        private DevComponents.DotNetBar.LabelX labelX2;
        private DevComponents.DotNetBar.Controls.TextBoxX txtSubjectName;
        private DevComponents.DotNetBar.LabelX labelX3;
        private DevComponents.DotNetBar.Controls.TextBoxX txtEnglishName;
        private DevComponents.DotNetBar.LabelX labelX6;
        private System.Windows.Forms.ErrorProvider errorProvider1;
        private DevComponents.DotNetBar.Controls.TextBoxX txtOrder;
        private DevComponents.DotNetBar.LabelX labelX1;
    }
}