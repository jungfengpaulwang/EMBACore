namespace EMBACore
{
    partial class Student_PaymentHistory
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
            this.PaiedTimes = new DevComponents.DotNetBar.Controls.TextBoxX();
            this.lblMoney = new DevComponents.DotNetBar.LabelX();
            this.dgvData = new DevComponents.DotNetBar.Controls.DataGridViewX();
            this.buttonX1 = new DevComponents.DotNetBar.ButtonX();
            this.SchoolYear = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Semester = new System.Windows.Forms.DataGridViewComboBoxColumn();
            this.IsPaied = new System.Windows.Forms.DataGridViewCheckBoxColumn();
            this.LastModifiedDate = new DevComponents.DotNetBar.Controls.DataGridViewLabelXColumn();
            this.UID = new DevComponents.DotNetBar.Controls.DataGridViewLabelXColumn();
            ((System.ComponentModel.ISupportInitialize)(this.dgvData)).BeginInit();
            this.SuspendLayout();
            // 
            // PaiedTimes
            // 
            // 
            // 
            // 
            this.PaiedTimes.Border.Class = "TextBoxBorder";
            this.PaiedTimes.Border.CornerType = DevComponents.DotNetBar.eCornerType.Square;
            this.PaiedTimes.Location = new System.Drawing.Point(420, 226);
            this.PaiedTimes.Name = "PaiedTimes";
            this.PaiedTimes.ReadOnly = true;
            this.PaiedTimes.Size = new System.Drawing.Size(100, 25);
            this.PaiedTimes.TabIndex = 24;
            // 
            // lblMoney
            // 
            this.lblMoney.AutoSize = true;
            // 
            // 
            // 
            this.lblMoney.BackgroundStyle.Class = "";
            this.lblMoney.BackgroundStyle.CornerType = DevComponents.DotNetBar.eCornerType.Square;
            this.lblMoney.Location = new System.Drawing.Point(355, 226);
            this.lblMoney.Name = "lblMoney";
            this.lblMoney.Size = new System.Drawing.Size(60, 21);
            this.lblMoney.TabIndex = 23;
            this.lblMoney.Text = "繳費次數";
            this.lblMoney.TextAlignment = System.Drawing.StringAlignment.Far;
            // 
            // dgvData
            // 
            this.dgvData.BackgroundColor = System.Drawing.Color.White;
            this.dgvData.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.dgvData.ClipboardCopyMode = System.Windows.Forms.DataGridViewClipboardCopyMode.EnableAlwaysIncludeHeaderText;
            this.dgvData.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvData.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.SchoolYear,
            this.Semester,
            this.IsPaied,
            this.LastModifiedDate,
            this.UID});
            dataGridViewCellStyle1.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle1.BackColor = System.Drawing.SystemColors.Window;
            dataGridViewCellStyle1.Font = new System.Drawing.Font("微軟正黑體", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            dataGridViewCellStyle1.ForeColor = System.Drawing.SystemColors.ControlText;
            dataGridViewCellStyle1.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle1.SelectionForeColor = System.Drawing.SystemColors.ControlText;
            dataGridViewCellStyle1.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
            this.dgvData.DefaultCellStyle = dataGridViewCellStyle1;
            this.dgvData.GridColor = System.Drawing.Color.FromArgb(((int)(((byte)(208)))), ((int)(((byte)(215)))), ((int)(((byte)(229)))));
            this.dgvData.Location = new System.Drawing.Point(30, 22);
            this.dgvData.Name = "dgvData";
            this.dgvData.RowTemplate.Height = 24;
            this.dgvData.RowTemplate.Resizable = System.Windows.Forms.DataGridViewTriState.True;
            this.dgvData.Size = new System.Drawing.Size(490, 186);
            this.dgvData.TabIndex = 22;
            // 
            // buttonX1
            // 
            this.buttonX1.AccessibleRole = System.Windows.Forms.AccessibleRole.PushButton;
            this.buttonX1.Location = new System.Drawing.Point(0, 0);
            this.buttonX1.Name = "buttonX1";
            this.buttonX1.Size = new System.Drawing.Size(0, 0);
            this.buttonX1.TabIndex = 0;
            // 
            // SchoolYear
            // 
            this.SchoolYear.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.None;
            this.SchoolYear.HeaderText = "學年度";
            this.SchoolYear.Name = "SchoolYear";
            this.SchoolYear.Width = 80;
            // 
            // Semester
            // 
            this.Semester.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.None;
            this.Semester.DisplayStyle = System.Windows.Forms.DataGridViewComboBoxDisplayStyle.ComboBox;
            this.Semester.HeaderText = "學期";
            this.Semester.Name = "Semester";
            this.Semester.Resizable = System.Windows.Forms.DataGridViewTriState.True;
            this.Semester.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.Automatic;
            this.Semester.Width = 80;
            // 
            // IsPaied
            // 
            this.IsPaied.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.DisplayedCells;
            this.IsPaied.HeaderText = "繳費";
            this.IsPaied.Name = "IsPaied";
            this.IsPaied.Resizable = System.Windows.Forms.DataGridViewTriState.True;
            this.IsPaied.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.Automatic;
            this.IsPaied.Width = 59;
            // 
            // LastModifiedDate
            // 
            this.LastModifiedDate.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.LastModifiedDate.HeaderText = "更新日期";
            this.LastModifiedDate.Name = "LastModifiedDate";
            this.LastModifiedDate.ReadOnly = true;
            this.LastModifiedDate.TextAlignment = System.Drawing.StringAlignment.Center;
            // 
            // UID
            // 
            this.UID.HeaderText = "UDT自動編號";
            this.UID.Name = "UID";
            this.UID.Resizable = System.Windows.Forms.DataGridViewTriState.True;
            this.UID.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.Automatic;
            this.UID.Visible = false;
            // 
            // Student_PaymentHistory
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 17F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.buttonX1);
            this.Controls.Add(this.PaiedTimes);
            this.Controls.Add(this.lblMoney);
            this.Controls.Add(this.dgvData);
            this.Name = "Student_PaymentHistory";
            this.Size = new System.Drawing.Size(550, 270);
            ((System.ComponentModel.ISupportInitialize)(this.dgvData)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private DevComponents.DotNetBar.Controls.TextBoxX PaiedTimes;
        private DevComponents.DotNetBar.LabelX lblMoney;
        private DevComponents.DotNetBar.Controls.DataGridViewX dgvData;
        private DevComponents.DotNetBar.ButtonX buttonX1;
        private System.Windows.Forms.DataGridViewTextBoxColumn SchoolYear;
        private System.Windows.Forms.DataGridViewComboBoxColumn Semester;
        private System.Windows.Forms.DataGridViewCheckBoxColumn IsPaied;
        private DevComponents.DotNetBar.Controls.DataGridViewLabelXColumn LastModifiedDate;
        private DevComponents.DotNetBar.Controls.DataGridViewLabelXColumn UID;
    }
}
