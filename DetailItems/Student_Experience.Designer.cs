namespace EMBACore.DetailItems
{
    partial class Student_Experience
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
            this.btnUpdate = new DevComponents.DotNetBar.ButtonX();
            this.btnDelete = new DevComponents.DotNetBar.ButtonX();
            this.btnAddNew = new DevComponents.DotNetBar.ButtonX();
            this.dg = new DevComponents.DotNetBar.Controls.DataGridViewX();
            this.Company = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Position = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Industry = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.WorkStatus = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.DepartmentCategory = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.PostLevel = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.WorkPlace = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.WorkBeginDate = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.WorkEndDate = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Column1 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            ((System.ComponentModel.ISupportInitialize)(this.dg)).BeginInit();
            this.SuspendLayout();
            // 
            // btnUpdate
            // 
            this.btnUpdate.AccessibleRole = System.Windows.Forms.AccessibleRole.PushButton;
            this.btnUpdate.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnUpdate.ColorTable = DevComponents.DotNetBar.eButtonColor.OrangeWithBackground;
            this.btnUpdate.Location = new System.Drawing.Point(364, 153);
            this.btnUpdate.Name = "btnUpdate";
            this.btnUpdate.Size = new System.Drawing.Size(75, 23);
            this.btnUpdate.Style = DevComponents.DotNetBar.eDotNetBarStyle.StyleManagerControlled;
            this.btnUpdate.TabIndex = 0;
            this.btnUpdate.Text = "修改";
            this.btnUpdate.Click += new System.EventHandler(this.btnUpdate_Click);
            // 
            // btnDelete
            // 
            this.btnDelete.AccessibleRole = System.Windows.Forms.AccessibleRole.PushButton;
            this.btnDelete.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnDelete.ColorTable = DevComponents.DotNetBar.eButtonColor.OrangeWithBackground;
            this.btnDelete.Location = new System.Drawing.Point(445, 153);
            this.btnDelete.Name = "btnDelete";
            this.btnDelete.Size = new System.Drawing.Size(75, 23);
            this.btnDelete.Style = DevComponents.DotNetBar.eDotNetBarStyle.StyleManagerControlled;
            this.btnDelete.TabIndex = 1;
            this.btnDelete.Text = "刪除";
            this.btnDelete.Click += new System.EventHandler(this.btnDelete_Click);
            // 
            // btnAddNew
            // 
            this.btnAddNew.AccessibleRole = System.Windows.Forms.AccessibleRole.PushButton;
            this.btnAddNew.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnAddNew.ColorTable = DevComponents.DotNetBar.eButtonColor.OrangeWithBackground;
            this.btnAddNew.Location = new System.Drawing.Point(283, 153);
            this.btnAddNew.Name = "btnAddNew";
            this.btnAddNew.Size = new System.Drawing.Size(75, 23);
            this.btnAddNew.Style = DevComponents.DotNetBar.eDotNetBarStyle.StyleManagerControlled;
            this.btnAddNew.TabIndex = 2;
            this.btnAddNew.Text = "新增";
            this.btnAddNew.Click += new System.EventHandler(this.btnAddNew_Click);
            // 
            // dg
            // 
            this.dg.AllowUserToAddRows = false;
            this.dg.AllowUserToDeleteRows = false;
            this.dg.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.dg.BackgroundColor = System.Drawing.Color.White;
            this.dg.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.dg.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dg.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.Company,
            this.Position,
            this.Industry,
            this.WorkStatus,
            this.DepartmentCategory,
            this.PostLevel,
            this.WorkPlace,
            this.WorkBeginDate,
            this.WorkEndDate,
            this.Column1});
            dataGridViewCellStyle1.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle1.BackColor = System.Drawing.SystemColors.Window;
            dataGridViewCellStyle1.Font = new System.Drawing.Font("微軟正黑體", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            dataGridViewCellStyle1.ForeColor = System.Drawing.SystemColors.ControlText;
            dataGridViewCellStyle1.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle1.SelectionForeColor = System.Drawing.SystemColors.ControlText;
            dataGridViewCellStyle1.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
            this.dg.DefaultCellStyle = dataGridViewCellStyle1;
            this.dg.GridColor = System.Drawing.Color.FromArgb(((int)(((byte)(208)))), ((int)(((byte)(215)))), ((int)(((byte)(229)))));
            this.dg.Location = new System.Drawing.Point(30, 12);
            this.dg.Name = "dg";
            this.dg.ReadOnly = true;
            this.dg.RowHeadersWidth = 25;
            this.dg.RowTemplate.Height = 24;
            this.dg.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dg.Size = new System.Drawing.Size(490, 133);
            this.dg.TabIndex = 3;
            // 
            // Company
            // 
            this.Company.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.AllCells;
            this.Company.HeaderText = "公司名稱";
            this.Company.Name = "Company";
            this.Company.ReadOnly = true;
            this.Company.Width = 85;
            // 
            // Position
            // 
            this.Position.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.AllCells;
            this.Position.HeaderText = "職稱";
            this.Position.Name = "Position";
            this.Position.ReadOnly = true;
            this.Position.Width = 59;
            // 
            // Industry
            // 
            this.Industry.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.AllCells;
            this.Industry.HeaderText = "產業別";
            this.Industry.Name = "Industry";
            this.Industry.ReadOnly = true;
            this.Industry.Width = 72;
            // 
            // WorkStatus
            // 
            this.WorkStatus.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.AllCells;
            this.WorkStatus.HeaderText = "工作狀態";
            this.WorkStatus.Name = "WorkStatus";
            this.WorkStatus.ReadOnly = true;
            this.WorkStatus.Width = 85;
            // 
            // DepartmentCategory
            // 
            this.DepartmentCategory.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.AllCells;
            this.DepartmentCategory.HeaderText = "部門類別";
            this.DepartmentCategory.Name = "DepartmentCategory";
            this.DepartmentCategory.ReadOnly = true;
            this.DepartmentCategory.Width = 85;
            // 
            // PostLevel
            // 
            this.PostLevel.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.AllCells;
            this.PostLevel.HeaderText = "層級別";
            this.PostLevel.Name = "PostLevel";
            this.PostLevel.ReadOnly = true;
            this.PostLevel.Width = 72;
            // 
            // WorkPlace
            // 
            this.WorkPlace.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.AllCells;
            this.WorkPlace.HeaderText = "工作地點";
            this.WorkPlace.Name = "WorkPlace";
            this.WorkPlace.ReadOnly = true;
            this.WorkPlace.Width = 85;
            // 
            // WorkBeginDate
            // 
            this.WorkBeginDate.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.AllCells;
            this.WorkBeginDate.HeaderText = "工作起日";
            this.WorkBeginDate.Name = "WorkBeginDate";
            this.WorkBeginDate.ReadOnly = true;
            this.WorkBeginDate.Width = 85;
            // 
            // WorkEndDate
            // 
            this.WorkEndDate.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.AllCells;
            this.WorkEndDate.HeaderText = "工作迄日";
            this.WorkEndDate.Name = "WorkEndDate";
            this.WorkEndDate.ReadOnly = true;
            this.WorkEndDate.Width = 85;
            // 
            // Column1
            // 
            this.Column1.HeaderText = "資料異動人";
            this.Column1.Name = "Column1";
            this.Column1.ReadOnly = true;
            // 
            // Student_Experience
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 17F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.dg);
            this.Controls.Add(this.btnAddNew);
            this.Controls.Add(this.btnDelete);
            this.Controls.Add(this.btnUpdate);
            this.Name = "Student_Experience";
            this.Size = new System.Drawing.Size(550, 185);
            this.Load += new System.EventHandler(this.Student_Experience_Load);
            ((System.ComponentModel.ISupportInitialize)(this.dg)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private DevComponents.DotNetBar.ButtonX btnUpdate;
        private DevComponents.DotNetBar.ButtonX btnDelete;
        private DevComponents.DotNetBar.ButtonX btnAddNew;
        private DevComponents.DotNetBar.Controls.DataGridViewX dg;
        private System.Windows.Forms.DataGridViewTextBoxColumn Company;
        private System.Windows.Forms.DataGridViewTextBoxColumn Position;
        private System.Windows.Forms.DataGridViewTextBoxColumn Industry;
        private System.Windows.Forms.DataGridViewTextBoxColumn WorkStatus;
        private System.Windows.Forms.DataGridViewTextBoxColumn DepartmentCategory;
        private System.Windows.Forms.DataGridViewTextBoxColumn PostLevel;
        private System.Windows.Forms.DataGridViewTextBoxColumn WorkPlace;
        private System.Windows.Forms.DataGridViewTextBoxColumn WorkBeginDate;
        private System.Windows.Forms.DataGridViewTextBoxColumn WorkEndDate;
        private System.Windows.Forms.DataGridViewTextBoxColumn Column1;
    }
}
