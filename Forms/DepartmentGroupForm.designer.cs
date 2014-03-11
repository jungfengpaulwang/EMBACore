namespace EMBACore.Forms
{
    partial class DepartmentGroupForm
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
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle1 = new System.Windows.Forms.DataGridViewCellStyle();
            this.cmdDelete = new DevComponents.DotNetBar.ButtonX();
            this.cmdUpdate = new DevComponents.DotNetBar.ButtonX();
            this.cmdAdd = new DevComponents.DotNetBar.ButtonX();
            this.cmdSelect = new DevComponents.DotNetBar.ButtonX();
            this.dataGridView1 = new DevComponents.DotNetBar.Controls.DataGridViewX();
            this.DepartmentGroupName = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.DepartmentGroupEnglishName = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.DepartmentGroupCode = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Order = new System.Windows.Forms.DataGridViewTextBoxColumn();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).BeginInit();
            this.SuspendLayout();
            // 
            // cmdDelete
            // 
            this.cmdDelete.AccessibleRole = System.Windows.Forms.AccessibleRole.PushButton;
            this.cmdDelete.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.cmdDelete.AutoSize = true;
            this.cmdDelete.BackColor = System.Drawing.Color.Transparent;
            this.cmdDelete.ColorTable = DevComponents.DotNetBar.eButtonColor.OrangeWithBackground;
            this.cmdDelete.Location = new System.Drawing.Point(452, 336);
            this.cmdDelete.Name = "cmdDelete";
            this.cmdDelete.Size = new System.Drawing.Size(75, 25);
            this.cmdDelete.TabIndex = 6;
            this.cmdDelete.Text = "刪除";
            // 
            // cmdUpdate
            // 
            this.cmdUpdate.AccessibleRole = System.Windows.Forms.AccessibleRole.PushButton;
            this.cmdUpdate.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.cmdUpdate.AutoSize = true;
            this.cmdUpdate.BackColor = System.Drawing.Color.Transparent;
            this.cmdUpdate.ColorTable = DevComponents.DotNetBar.eButtonColor.OrangeWithBackground;
            this.cmdUpdate.Location = new System.Drawing.Point(366, 336);
            this.cmdUpdate.Name = "cmdUpdate";
            this.cmdUpdate.Size = new System.Drawing.Size(75, 25);
            this.cmdUpdate.TabIndex = 7;
            this.cmdUpdate.Text = "修改";
            // 
            // cmdAdd
            // 
            this.cmdAdd.AccessibleRole = System.Windows.Forms.AccessibleRole.PushButton;
            this.cmdAdd.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.cmdAdd.AutoSize = true;
            this.cmdAdd.BackColor = System.Drawing.Color.Transparent;
            this.cmdAdd.ColorTable = DevComponents.DotNetBar.eButtonColor.OrangeWithBackground;
            this.cmdAdd.Location = new System.Drawing.Point(280, 336);
            this.cmdAdd.Name = "cmdAdd";
            this.cmdAdd.Size = new System.Drawing.Size(75, 25);
            this.cmdAdd.TabIndex = 8;
            this.cmdAdd.Text = "新增";
            // 
            // cmdSelect
            // 
            this.cmdSelect.AccessibleRole = System.Windows.Forms.AccessibleRole.PushButton;
            this.cmdSelect.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.cmdSelect.AutoSize = true;
            this.cmdSelect.BackColor = System.Drawing.Color.Transparent;
            this.cmdSelect.ColorTable = DevComponents.DotNetBar.eButtonColor.OrangeWithBackground;
            this.cmdSelect.Location = new System.Drawing.Point(12, 336);
            this.cmdSelect.Name = "cmdSelect";
            this.cmdSelect.Size = new System.Drawing.Size(75, 25);
            this.cmdSelect.TabIndex = 9;
            this.cmdSelect.Text = "確定";
            // 
            // dataGridView1
            // 
            this.dataGridView1.AllowUserToAddRows = false;
            this.dataGridView1.AllowUserToDeleteRows = false;
            this.dataGridView1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.dataGridView1.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.AllCells;
            this.dataGridView1.AutoSizeRowsMode = System.Windows.Forms.DataGridViewAutoSizeRowsMode.AllCells;
            this.dataGridView1.BackgroundColor = System.Drawing.Color.White;
            this.dataGridView1.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.dataGridView1.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.DepartmentGroupName,
            this.DepartmentGroupEnglishName,
            this.DepartmentGroupCode,
            this.Order});
            dataGridViewCellStyle1.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle1.BackColor = System.Drawing.SystemColors.Window;
            dataGridViewCellStyle1.Font = new System.Drawing.Font("微軟正黑體", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            dataGridViewCellStyle1.ForeColor = System.Drawing.SystemColors.ControlText;
            dataGridViewCellStyle1.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle1.SelectionForeColor = System.Drawing.SystemColors.ControlText;
            dataGridViewCellStyle1.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
            this.dataGridView1.DefaultCellStyle = dataGridViewCellStyle1;
            this.dataGridView1.GridColor = System.Drawing.Color.FromArgb(((int)(((byte)(208)))), ((int)(((byte)(215)))), ((int)(((byte)(229)))));
            this.dataGridView1.Location = new System.Drawing.Point(12, 13);
            this.dataGridView1.Name = "dataGridView1";
            this.dataGridView1.ReadOnly = true;
            this.dataGridView1.RowHeadersWidth = 25;
            this.dataGridView1.RowTemplate.Height = 24;
            this.dataGridView1.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dataGridView1.Size = new System.Drawing.Size(515, 314);
            this.dataGridView1.TabIndex = 1;
            // 
            // DepartmentGroupName
            // 
            this.DepartmentGroupName.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.AllCells;
            this.DepartmentGroupName.HeaderText = "系所組別中文名稱";
            this.DepartmentGroupName.Name = "DepartmentGroupName";
            this.DepartmentGroupName.ReadOnly = true;
            this.DepartmentGroupName.Width = 137;
            // 
            // DepartmentGroupEnglishName
            // 
            this.DepartmentGroupEnglishName.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.AllCells;
            this.DepartmentGroupEnglishName.HeaderText = "系所組別英文名稱";
            this.DepartmentGroupEnglishName.Name = "DepartmentGroupEnglishName";
            this.DepartmentGroupEnglishName.ReadOnly = true;
            this.DepartmentGroupEnglishName.Width = 137;
            // 
            // DepartmentGroupCode
            // 
            this.DepartmentGroupCode.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.AllCells;
            this.DepartmentGroupCode.HeaderText = "系所組別代碼";
            this.DepartmentGroupCode.Name = "DepartmentGroupCode";
            this.DepartmentGroupCode.ReadOnly = true;
            this.DepartmentGroupCode.Resizable = System.Windows.Forms.DataGridViewTriState.True;
            this.DepartmentGroupCode.Width = 111;
            // 
            // Order
            // 
            this.Order.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.AllCells;
            this.Order.HeaderText = "排列順序";
            this.Order.Name = "Order";
            this.Order.ReadOnly = true;
            this.Order.Width = 85;
            // 
            // DepartmentGroupForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 17F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(537, 366);
            this.Controls.Add(this.cmdSelect);
            this.Controls.Add(this.cmdAdd);
            this.Controls.Add(this.cmdUpdate);
            this.Controls.Add(this.cmdDelete);
            this.Controls.Add(this.dataGridView1);
            this.DoubleBuffered = true;
            this.Name = "DepartmentGroupForm";
            this.Text = "系所組別";
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        public DevComponents.DotNetBar.ButtonX cmdDelete;
        public DevComponents.DotNetBar.ButtonX cmdUpdate;
        public DevComponents.DotNetBar.ButtonX cmdAdd;
        public DevComponents.DotNetBar.ButtonX cmdSelect;
        private DevComponents.DotNetBar.Controls.DataGridViewX dataGridViewX1;
        private DevComponents.DotNetBar.Controls.DataGridViewX dataGridView1;
        private System.Windows.Forms.DataGridViewTextBoxColumn DepartmentGroupName;
        private System.Windows.Forms.DataGridViewTextBoxColumn DepartmentGroupEnglishName;
        private System.Windows.Forms.DataGridViewTextBoxColumn DepartmentGroupCode;
        private System.Windows.Forms.DataGridViewTextBoxColumn Order;
    }
}