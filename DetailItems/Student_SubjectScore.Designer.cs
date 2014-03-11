namespace EMBACore.DetailItems
{
    partial class Student_SubjectScore
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
            this.colSchoolYear = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colSemester = new System.Windows.Forms.DataGridViewComboBoxColumn();
            this.colCourse = new System.Windows.Forms.DataGridViewComboBoxColumn();
            this.colCourseName = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.ClassName = new DevComponents.DotNetBar.Controls.DataGridViewLabelXColumn();
            this.colIsRequired = new System.Windows.Forms.DataGridViewCheckBoxColumn();
            this.colCredit = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.ScoredProgress = new DevComponents.DotNetBar.Controls.DataGridViewLabelXColumn();
            this.colScore = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colIsPass = new System.Windows.Forms.DataGridViewCheckBoxColumn();
            this.colOffsetCourse = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colRemark = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.SerialNo = new DevComponents.DotNetBar.Controls.DataGridViewLabelXColumn();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridViewX1)).BeginInit();
            this.SuspendLayout();
            // 
            // dataGridViewX1
            // 
            this.dataGridViewX1.BackgroundColor = System.Drawing.Color.White;
            this.dataGridViewX1.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.dataGridViewX1.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridViewX1.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.colSchoolYear,
            this.colSemester,
            this.colCourse,
            this.colCourseName,
            this.ClassName,
            this.colIsRequired,
            this.colCredit,
            this.ScoredProgress,
            this.colScore,
            this.colIsPass,
            this.colOffsetCourse,
            this.colRemark,
            this.SerialNo});
            dataGridViewCellStyle1.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle1.BackColor = System.Drawing.SystemColors.Window;
            dataGridViewCellStyle1.Font = new System.Drawing.Font("微軟正黑體", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            dataGridViewCellStyle1.ForeColor = System.Drawing.SystemColors.ControlText;
            dataGridViewCellStyle1.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle1.SelectionForeColor = System.Drawing.SystemColors.ControlText;
            dataGridViewCellStyle1.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
            this.dataGridViewX1.DefaultCellStyle = dataGridViewCellStyle1;
            this.dataGridViewX1.GridColor = System.Drawing.Color.FromArgb(((int)(((byte)(208)))), ((int)(((byte)(215)))), ((int)(((byte)(229)))));
            this.dataGridViewX1.Location = new System.Drawing.Point(30, 8);
            this.dataGridViewX1.Name = "dataGridViewX1";
            this.dataGridViewX1.RowHeadersWidth = 25;
            this.dataGridViewX1.RowTemplate.Height = 24;
            this.dataGridViewX1.Size = new System.Drawing.Size(490, 564);
            this.dataGridViewX1.TabIndex = 12;
            this.dataGridViewX1.CellContentClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.dataGridViewX1_CellContentClick);
            this.dataGridViewX1.CellContentDoubleClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.dataGridViewX1_CellContentDoubleClick);
            this.dataGridViewX1.CellEndEdit += new System.Windows.Forms.DataGridViewCellEventHandler(this.dataGridViewX1_CellEndEdit);
            this.dataGridViewX1.CurrentCellDirtyStateChanged += new System.EventHandler(this.dataGridViewX1_CurrentCellDirtyStateChanged);
            this.dataGridViewX1.UserDeletedRow += new System.Windows.Forms.DataGridViewRowEventHandler(this.dataGridViewX1_UserDeletedRow);
            this.dataGridViewX1.UserDeletingRow += new System.Windows.Forms.DataGridViewRowCancelEventHandler(this.dataGridViewX1_UserDeletingRow);
            // 
            // colSchoolYear
            // 
            this.colSchoolYear.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.AllCells;
            this.colSchoolYear.HeaderText = "學年度";
            this.colSchoolYear.Name = "colSchoolYear";
            this.colSchoolYear.Width = 72;
            // 
            // colSemester
            // 
            this.colSemester.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.AllCells;
            this.colSemester.DisplayStyle = System.Windows.Forms.DataGridViewComboBoxDisplayStyle.ComboBox;
            this.colSemester.HeaderText = "學期";
            this.colSemester.Name = "colSemester";
            this.colSemester.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.Automatic;
            this.colSemester.Width = 59;
            // 
            // colCourse
            // 
            this.colCourse.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.AllCells;
            this.colCourse.DisplayStyle = System.Windows.Forms.DataGridViewComboBoxDisplayStyle.ComboBox;
            this.colCourse.HeaderText = "課號";
            this.colCourse.Name = "colCourse";
            this.colCourse.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.Automatic;
            this.colCourse.Width = 59;
            // 
            // colCourseName
            // 
            this.colCourseName.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.AllCells;
            this.colCourseName.HeaderText = "課程名稱";
            this.colCourseName.Name = "colCourseName";
            this.colCourseName.ReadOnly = true;
            this.colCourseName.Resizable = System.Windows.Forms.DataGridViewTriState.True;
            this.colCourseName.Width = 85;
            // 
            // ClassName
            // 
            this.ClassName.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.AllCells;
            this.ClassName.HeaderText = "班次";
            this.ClassName.Name = "ClassName";
            this.ClassName.Resizable = System.Windows.Forms.DataGridViewTriState.True;
            this.ClassName.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.Automatic;
            this.ClassName.TextAlignment = System.Drawing.StringAlignment.Center;
            this.ClassName.Width = 59;
            // 
            // colIsRequired
            // 
            this.colIsRequired.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.AllCells;
            this.colIsRequired.HeaderText = "必修";
            this.colIsRequired.Name = "colIsRequired";
            this.colIsRequired.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.Automatic;
            this.colIsRequired.Width = 59;
            // 
            // colCredit
            // 
            this.colCredit.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.AllCells;
            this.colCredit.HeaderText = "學分數";
            this.colCredit.Name = "colCredit";
            this.colCredit.Width = 72;
            // 
            // ScoredProgress
            // 
            this.ScoredProgress.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.AllCells;
            this.ScoredProgress.HeaderText = "成績輸入進度";
            this.ScoredProgress.Name = "ScoredProgress";
            this.ScoredProgress.Resizable = System.Windows.Forms.DataGridViewTriState.True;
            this.ScoredProgress.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.Automatic;
            this.ScoredProgress.Width = 111;
            // 
            // colScore
            // 
            this.colScore.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.AllCells;
            this.colScore.HeaderText = "成績";
            this.colScore.Name = "colScore";
            this.colScore.Width = 59;
            // 
            // colIsPass
            // 
            this.colIsPass.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.AllCells;
            this.colIsPass.HeaderText = "取得學分";
            this.colIsPass.Name = "colIsPass";
            this.colIsPass.ReadOnly = true;
            this.colIsPass.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.Automatic;
            this.colIsPass.Width = 85;
            // 
            // colOffsetCourse
            // 
            this.colOffsetCourse.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.AllCells;
            this.colOffsetCourse.HeaderText = "抵免課程";
            this.colOffsetCourse.Name = "colOffsetCourse";
            this.colOffsetCourse.Width = 85;
            // 
            // colRemark
            // 
            this.colRemark.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.AllCells;
            this.colRemark.HeaderText = "備註";
            this.colRemark.Name = "colRemark";
            this.colRemark.Width = 59;
            // 
            // SerialNo
            // 
            this.SerialNo.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.AllCells;
            this.SerialNo.HeaderText = "流水號";
            this.SerialNo.Name = "SerialNo";
            this.SerialNo.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.Automatic;
            this.SerialNo.Width = 72;
            // 
            // Student_SubjectScore
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 17F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.dataGridViewX1);
            this.Name = "Student_SubjectScore";
            this.Padding = new System.Windows.Forms.Padding(5);
            this.Size = new System.Drawing.Size(550, 580);
            this.Load += new System.EventHandler(this.Student_SubjectScore_Load);
            ((System.ComponentModel.ISupportInitialize)(this.dataGridViewX1)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private DevComponents.DotNetBar.Controls.DataGridViewX dataGridViewX1;
        private System.Windows.Forms.DataGridViewTextBoxColumn colSchoolYear;
        private System.Windows.Forms.DataGridViewComboBoxColumn colSemester;
        private System.Windows.Forms.DataGridViewComboBoxColumn colCourse;
        private System.Windows.Forms.DataGridViewTextBoxColumn colCourseName;
        private DevComponents.DotNetBar.Controls.DataGridViewLabelXColumn ClassName;
        private System.Windows.Forms.DataGridViewCheckBoxColumn colIsRequired;
        private System.Windows.Forms.DataGridViewTextBoxColumn colCredit;
        private DevComponents.DotNetBar.Controls.DataGridViewLabelXColumn ScoredProgress;
        private System.Windows.Forms.DataGridViewTextBoxColumn colScore;
        private System.Windows.Forms.DataGridViewCheckBoxColumn colIsPass;
        private System.Windows.Forms.DataGridViewTextBoxColumn colOffsetCourse;
        private System.Windows.Forms.DataGridViewTextBoxColumn colRemark;
        private DevComponents.DotNetBar.Controls.DataGridViewLabelXColumn SerialNo;
    }
}
