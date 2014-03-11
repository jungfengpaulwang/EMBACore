namespace EMBACore.Forms.SeatTable
{
    partial class ucContentCell
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
            this.label1 = new System.Windows.Forms.Label();
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.ucTransparentPanel1 = new EMBACore.Forms.SeatTable.ucTransparentPanel();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.label1.Location = new System.Drawing.Point(0, 50);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(60, 15);
            this.label1.TabIndex = 0;
            this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // pictureBox1
            // 
            this.pictureBox1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pictureBox1.Location = new System.Drawing.Point(0, 0);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(60, 50);
            this.pictureBox1.TabIndex = 1;
            this.pictureBox1.TabStop = false;
            // 
            // ucTransparentPanel1
            // 
            this.ucTransparentPanel1.AllowDrop = true;
            this.ucTransparentPanel1.Location = new System.Drawing.Point(1, 1);
            this.ucTransparentPanel1.Name = "ucTransparentPanel1";
            this.ucTransparentPanel1.Size = new System.Drawing.Size(56, 65);
            this.ucTransparentPanel1.TabIndex = 3;
            this.ucTransparentPanel1.DragDrop += new System.Windows.Forms.DragEventHandler(this.ucTransparentPanel1_DragDrop);
            this.ucTransparentPanel1.DragEnter += new System.Windows.Forms.DragEventHandler(this.ucTransparentPanel1_DragEnter);
            this.ucTransparentPanel1.DoubleClick += new System.EventHandler(this.ucTransparentPanel1_DoubleClick);
            // 
            // ucContentCell
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.Controls.Add(this.ucTransparentPanel1);
            this.Controls.Add(this.pictureBox1);
            this.Controls.Add(this.label1);
            this.Name = "ucContentCell";
            this.Size = new System.Drawing.Size(60, 65);
            this.Load += new System.EventHandler(this.ucContentCell_Load);
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.PictureBox pictureBox1;
        private ucTransparentPanel ucTransparentPanel1;
    }
}
