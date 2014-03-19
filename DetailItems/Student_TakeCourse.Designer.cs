namespace EMBACore.DetailItems
{
    partial class Student_TakeCourse
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
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle2 = new System.Windows.Forms.DataGridViewCellStyle();
            this.nudSchoolYear = new System.Windows.Forms.NumericUpDown();
            this.labelX1 = new DevComponents.DotNetBar.LabelX();
            this.labelX2 = new DevComponents.DotNetBar.LabelX();
            this.cboSemester = new DevComponents.DotNetBar.Controls.ComboBoxEx();
            this.lblMessage = new System.Windows.Forms.Label();
            this.CourseInstructor = new DevComponents.DotNetBar.Controls.DataGridViewLabelXColumn();
            this.IsCancel = new DevComponents.DotNetBar.Controls.DataGridViewLabelXColumn();
            this.ReportGroup = new DevComponents.DotNetBar.Controls.DataGridViewLabelXColumn();
            this.Credit = new DevComponents.DotNetBar.Controls.DataGridViewLabelXColumn();
            this.IsRequired = new DevComponents.DotNetBar.Controls.DataGridViewLabelXColumn();
            this.CourseType = new DevComponents.DotNetBar.Controls.DataGridViewLabelXColumn();
            this.NewSubjectCode = new DevComponents.DotNetBar.Controls.DataGridViewLabelXColumn();
            this.CourseName = new DevComponents.DotNetBar.Controls.DataGridViewLabelXColumn();
            this.dgvData = new DevComponents.DotNetBar.Controls.DataGridViewX();
            ((System.ComponentModel.ISupportInitialize)(this.nudSchoolYear)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dgvData)).BeginInit();
            this.SuspendLayout();
            // 
            // nudSchoolYear
            // 
            this.nudSchoolYear.Location = new System.Drawing.Point(86, 8);
            this.nudSchoolYear.Maximum = new decimal(new int[] {
            500,
            0,
            0,
            0});
            this.nudSchoolYear.Name = "nudSchoolYear";
            this.nudSchoolYear.Size = new System.Drawing.Size(89, 25);
            this.nudSchoolYear.TabIndex = 10;
            this.nudSchoolYear.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.nudSchoolYear.ValueChanged += new System.EventHandler(this.nudSchoolYear_ValueChanged);
            // 
            // labelX1
            // 
            this.labelX1.BackColor = System.Drawing.Color.Transparent;
            // 
            // 
            // 
            this.labelX1.BackgroundStyle.Class = "";
            this.labelX1.BackgroundStyle.CornerType = DevComponents.DotNetBar.eCornerType.Square;
            this.labelX1.Location = new System.Drawing.Point(34, 9);
            this.labelX1.Name = "labelX1";
            this.labelX1.Size = new System.Drawing.Size(46, 23);
            this.labelX1.TabIndex = 9;
            this.labelX1.Text = "學年度";
            this.labelX1.TextAlignment = System.Drawing.StringAlignment.Center;
            // 
            // labelX2
            // 
            this.labelX2.BackColor = System.Drawing.Color.Transparent;
            // 
            // 
            // 
            this.labelX2.BackgroundStyle.Class = "";
            this.labelX2.BackgroundStyle.CornerType = DevComponents.DotNetBar.eCornerType.Square;
            this.labelX2.Location = new System.Drawing.Point(184, 9);
            this.labelX2.Name = "labelX2";
            this.labelX2.Size = new System.Drawing.Size(44, 23);
            this.labelX2.TabIndex = 7;
            this.labelX2.Text = "學期";
            this.labelX2.TextAlignment = System.Drawing.StringAlignment.Center;
            // 
            // cboSemester
            // 
            this.cboSemester.DisplayMember = "Text";
            this.cboSemester.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawFixed;
            this.cboSemester.FormattingEnabled = true;
            this.cboSemester.ItemHeight = 19;
            this.cboSemester.Location = new System.Drawing.Point(236, 8);
            this.cboSemester.Name = "cboSemester";
            this.cboSemester.Size = new System.Drawing.Size(89, 25);
            this.cboSemester.Style = DevComponents.DotNetBar.eDotNetBarStyle.StyleManagerControlled;
            this.cboSemester.TabIndex = 8;
            this.cboSemester.SelectedIndexChanged += new System.EventHandler(this.cboSemester_SelectedIndexChanged);
            // 
            // lblMessage
            // 
            this.lblMessage.Font = new System.Drawing.Font("微軟正黑體", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.lblMessage.ForeColor = System.Drawing.Color.Red;
            this.lblMessage.Location = new System.Drawing.Point(331, 6);
            this.lblMessage.Name = "lblMessage";
            this.lblMessage.Size = new System.Drawing.Size(189, 27);
            this.lblMessage.TabIndex = 11;
            this.lblMessage.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // CourseInstructor
            // 
            this.CourseInstructor.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.AllCells;
            this.CourseInstructor.HeaderText = "授課教師";
            this.CourseInstructor.Name = "CourseInstructor";
            this.CourseInstructor.ReadOnly = true;
            this.CourseInstructor.Resizable = System.Windows.Forms.DataGridViewTriState.True;
            this.CourseInstructor.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.Automatic;
            this.CourseInstructor.Width = 85;
            // 
            // IsCancel
            // 
            this.IsCancel.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.AllCells;
            this.IsCancel.HeaderText = "停修";
            this.IsCancel.Name = "IsCancel";
            this.IsCancel.ReadOnly = true;
            this.IsCancel.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.Automatic;
            this.IsCancel.Width = 59;
            // 
            // ReportGroup
            // 
            this.ReportGroup.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.AllCells;
            this.ReportGroup.HeaderText = "報告小組";
            this.ReportGroup.Name = "ReportGroup";
            this.ReportGroup.ReadOnly = true;
            this.ReportGroup.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.Automatic;
            this.ReportGroup.Width = 85;
            // 
            // Credit
            // 
            this.Credit.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.AllCells;
            this.Credit.HeaderText = "學分數";
            this.Credit.Name = "Credit";
            this.Credit.ReadOnly = true;
            this.Credit.Resizable = System.Windows.Forms.DataGridViewTriState.True;
            this.Credit.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.Automatic;
            this.Credit.Width = 72;
            // 
            // IsRequired
            // 
            this.IsRequired.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.AllCells;
            this.IsRequired.HeaderText = "必選修";
            this.IsRequired.Name = "IsRequired";
            this.IsRequired.ReadOnly = true;
            this.IsRequired.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.Automatic;
            this.IsRequired.Width = 72;
            // 
            // CourseType
            // 
            this.CourseType.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.AllCells;
            this.CourseType.HeaderText = "類別";
            this.CourseType.Name = "CourseType";
            this.CourseType.ReadOnly = true;
            this.CourseType.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.Automatic;
            this.CourseType.Width = 59;
            // 
            // NewSubjectCode
            // 
            this.NewSubjectCode.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.AllCells;
            this.NewSubjectCode.HeaderText = "課號";
            this.NewSubjectCode.Name = "NewSubjectCode";
            this.NewSubjectCode.ReadOnly = true;
            this.NewSubjectCode.Resizable = System.Windows.Forms.DataGridViewTriState.True;
            this.NewSubjectCode.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.Automatic;
            this.NewSubjectCode.Width = 59;
            // 
            // CourseName
            // 
            this.CourseName.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.DisplayedCells;
            this.CourseName.HeaderText = "課程名稱";
            this.CourseName.Name = "CourseName";
            this.CourseName.ReadOnly = true;
            this.CourseName.Resizable = System.Windows.Forms.DataGridViewTriState.True;
            this.CourseName.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.Automatic;
            this.CourseName.Width = 85;
            // 
            // dgvData
            // 
            this.dgvData.AllowUserToAddRows = false;
            this.dgvData.AllowUserToDeleteRows = false;
            this.dgvData.AllowUserToOrderColumns = true;
            this.dgvData.BackgroundColor = System.Drawing.Color.White;
            this.dgvData.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvData.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.CourseName,
            this.NewSubjectCode,
            this.CourseType,
            this.IsRequired,
            this.Credit,
            this.ReportGroup,
            this.IsCancel,
            this.CourseInstructor});
            dataGridViewCellStyle2.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle2.BackColor = System.Drawing.SystemColors.Window;
            dataGridViewCellStyle2.Font = new System.Drawing.Font("微軟正黑體", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            dataGridViewCellStyle2.ForeColor = System.Drawing.SystemColors.ControlText;
            dataGridViewCellStyle2.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle2.SelectionForeColor = System.Drawing.SystemColors.ControlText;
            dataGridViewCellStyle2.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
            this.dgvData.DefaultCellStyle = dataGridViewCellStyle2;
            this.dgvData.GridColor = System.Drawing.Color.FromArgb(((int)(((byte)(208)))), ((int)(((byte)(215)))), ((int)(((byte)(229)))));
            this.dgvData.Location = new System.Drawing.Point(30, 42);
            this.dgvData.Name = "dgvData";
            this.dgvData.ReadOnly = true;
            this.dgvData.RowHeadersWidth = 25;
            this.dgvData.RowTemplate.Height = 24;
            this.dgvData.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dgvData.Size = new System.Drawing.Size(490, 234);
            this.dgvData.TabIndex = 6;
            this.dgvData.UserDeletingRow += new System.Windows.Forms.DataGridViewRowCancelEventHandler(this.dgvData_UserDeletingRow);
            // 
            // Student_TakeCourse
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 17F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.lblMessage);
            this.Controls.Add(this.nudSchoolYear);
            this.Controls.Add(this.labelX1);
            this.Controls.Add(this.cboSemester);
            this.Controls.Add(this.labelX2);
            this.Controls.Add(this.dgvData);
            this.Name = "Student_TakeCourse";
            this.Size = new System.Drawing.Size(550, 290);
            ((System.ComponentModel.ISupportInitialize)(this.nudSchoolYear)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dgvData)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.NumericUpDown nudSchoolYear;
        private DevComponents.DotNetBar.LabelX labelX1;
        private DevComponents.DotNetBar.LabelX labelX2;
        private DevComponents.DotNetBar.Controls.ComboBoxEx cboSemester;
        private System.Windows.Forms.Label lblMessage;
        private DevComponents.DotNetBar.Controls.DataGridViewLabelXColumn CourseInstructor;
        private DevComponents.DotNetBar.Controls.DataGridViewLabelXColumn IsCancel;
        private DevComponents.DotNetBar.Controls.DataGridViewLabelXColumn ReportGroup;
        private DevComponents.DotNetBar.Controls.DataGridViewLabelXColumn Credit;
        private DevComponents.DotNetBar.Controls.DataGridViewLabelXColumn IsRequired;
        private DevComponents.DotNetBar.Controls.DataGridViewLabelXColumn CourseType;
        private DevComponents.DotNetBar.Controls.DataGridViewLabelXColumn NewSubjectCode;
        private DevComponents.DotNetBar.Controls.DataGridViewLabelXColumn CourseName;
        private DevComponents.DotNetBar.Controls.DataGridViewX dgvData;
    }
}
