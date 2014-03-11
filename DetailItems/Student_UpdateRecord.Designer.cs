namespace EMBACore.DetailItems
{
    partial class Student_UpdateRecord
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
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle1 = new System.Windows.Forms.DataGridViewCellStyle();
            this.dataGridViewX1 = new DevComponents.DotNetBar.Controls.DataGridViewX();
            this.reg_no = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.tr1 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.tr2 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.tr = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Defer = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.r_qr1 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.r_qr2 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.r_qr3 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.r_qr4 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.r_qr5 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.r_qr6 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.r_qr7 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.r_qr8 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.create_time = new System.Windows.Forms.DataGridViewTextBoxColumn();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridViewX1)).BeginInit();
            this.SuspendLayout();
            // 
            // dataGridViewX1
            // 
            this.dataGridViewX1.AllowUserToAddRows = false;
            this.dataGridViewX1.AllowUserToDeleteRows = false;
            this.dataGridViewX1.BackgroundColor = System.Drawing.Color.White;
            this.dataGridViewX1.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.dataGridViewX1.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridViewX1.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.reg_no,
            this.tr1,
            this.tr2,
            this.tr,
            this.Defer,
            this.r_qr1,
            this.r_qr2,
            this.r_qr3,
            this.r_qr4,
            this.r_qr5,
            this.r_qr6,
            this.r_qr7,
            this.r_qr8,
            this.create_time});
            dataGridViewCellStyle1.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle1.BackColor = System.Drawing.SystemColors.Window;
            dataGridViewCellStyle1.Font = new System.Drawing.Font("微軟正黑體", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            dataGridViewCellStyle1.ForeColor = System.Drawing.SystemColors.ControlText;
            dataGridViewCellStyle1.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle1.SelectionForeColor = System.Drawing.SystemColors.ControlText;
            dataGridViewCellStyle1.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
            this.dataGridViewX1.DefaultCellStyle = dataGridViewCellStyle1;
            this.dataGridViewX1.GridColor = System.Drawing.Color.FromArgb(((int)(((byte)(208)))), ((int)(((byte)(215)))), ((int)(((byte)(229)))));
            this.dataGridViewX1.Location = new System.Drawing.Point(30, 13);
            this.dataGridViewX1.Name = "dataGridViewX1";
            this.dataGridViewX1.ReadOnly = true;
            this.dataGridViewX1.RowHeadersWidth = 25;
            this.dataGridViewX1.RowTemplate.Height = 24;
            this.dataGridViewX1.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dataGridViewX1.Size = new System.Drawing.Size(490, 188);
            this.dataGridViewX1.TabIndex = 0;
            // 
            // reg_no
            // 
            this.reg_no.HeaderText = "學號";
            this.reg_no.Name = "reg_no";
            this.reg_no.ReadOnly = true;
            // 
            // tr1
            // 
            this.tr1.HeaderText = "是否在校";
            this.tr1.Name = "tr1";
            this.tr1.ReadOnly = true;
            this.tr1.Width = 70;
            // 
            // tr2
            // 
            this.tr2.HeaderText = "異動類別";
            this.tr2.Name = "tr2";
            this.tr2.ReadOnly = true;
            this.tr2.Width = 70;
            // 
            // tr
            // 
            this.tr.HeaderText = "異動學期";
            this.tr.Name = "tr";
            this.tr.ReadOnly = true;
            this.tr.Width = 60;
            // 
            // Defer
            // 
            this.Defer.HeaderText = "是否延畢";
            this.Defer.Name = "Defer";
            this.Defer.ReadOnly = true;
            this.Defer.Width = 70;
            // 
            // r_qr1
            // 
            this.r_qr1.HeaderText = "休學一";
            this.r_qr1.Name = "r_qr1";
            this.r_qr1.ReadOnly = true;
            this.r_qr1.Width = 60;
            // 
            // r_qr2
            // 
            this.r_qr2.HeaderText = "休學二";
            this.r_qr2.Name = "r_qr2";
            this.r_qr2.ReadOnly = true;
            this.r_qr2.Width = 60;
            // 
            // r_qr3
            // 
            this.r_qr3.HeaderText = "休學三";
            this.r_qr3.Name = "r_qr3";
            this.r_qr3.ReadOnly = true;
            this.r_qr3.Width = 60;
            // 
            // r_qr4
            // 
            this.r_qr4.HeaderText = "休學四";
            this.r_qr4.Name = "r_qr4";
            this.r_qr4.ReadOnly = true;
            this.r_qr4.Width = 60;
            // 
            // r_qr5
            // 
            this.r_qr5.HeaderText = "休學五";
            this.r_qr5.Name = "r_qr5";
            this.r_qr5.ReadOnly = true;
            this.r_qr5.Width = 60;
            // 
            // r_qr6
            // 
            this.r_qr6.HeaderText = "休學六";
            this.r_qr6.Name = "r_qr6";
            this.r_qr6.ReadOnly = true;
            this.r_qr6.Width = 60;
            // 
            // r_qr7
            // 
            this.r_qr7.HeaderText = "休學七";
            this.r_qr7.Name = "r_qr7";
            this.r_qr7.ReadOnly = true;
            this.r_qr7.Width = 60;
            // 
            // r_qr8
            // 
            this.r_qr8.HeaderText = "休學八";
            this.r_qr8.Name = "r_qr8";
            this.r_qr8.ReadOnly = true;
            this.r_qr8.Width = 60;
            // 
            // create_time
            // 
            this.create_time.HeaderText = "紀錄時間";
            this.create_time.Name = "create_time";
            this.create_time.ReadOnly = true;
            // 
            // Student_UpdateRecord
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 17F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.dataGridViewX1);
            this.Name = "Student_UpdateRecord";
            this.Size = new System.Drawing.Size(550, 215);
            this.Load += new System.EventHandler(this.Student_UpdateRecord_Load);
            ((System.ComponentModel.ISupportInitialize)(this.dataGridViewX1)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private DevComponents.DotNetBar.Controls.DataGridViewX dataGridViewX1;
        private System.Windows.Forms.DataGridViewTextBoxColumn reg_no;
        private System.Windows.Forms.DataGridViewTextBoxColumn tr1;
        private System.Windows.Forms.DataGridViewTextBoxColumn tr2;
        private System.Windows.Forms.DataGridViewTextBoxColumn tr;
        private System.Windows.Forms.DataGridViewTextBoxColumn Defer;
        private System.Windows.Forms.DataGridViewTextBoxColumn r_qr1;
        private System.Windows.Forms.DataGridViewTextBoxColumn r_qr2;
        private System.Windows.Forms.DataGridViewTextBoxColumn r_qr3;
        private System.Windows.Forms.DataGridViewTextBoxColumn r_qr4;
        private System.Windows.Forms.DataGridViewTextBoxColumn r_qr5;
        private System.Windows.Forms.DataGridViewTextBoxColumn r_qr6;
        private System.Windows.Forms.DataGridViewTextBoxColumn r_qr7;
        private System.Windows.Forms.DataGridViewTextBoxColumn r_qr8;
        private System.Windows.Forms.DataGridViewTextBoxColumn create_time;
    }
}
