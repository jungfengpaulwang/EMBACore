using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using FISCA.Presentation.Controls;
using FISCA.UDT;
using EMBACore.UDT;
using System.Xml;
using EMBACore.DataItems;
using Campus.Configuration;
using SHSchool.Data;
using FISCA.Data;

namespace EMBACore.Forms
{
    public partial class GenerateCourse : BaseForm
    {
        private AccessHelper Access;
        private List<Subject> subjects;
        private BackgroundWorker bgWorker; 
        
        XmlDocument xmlSystemConfig;
        XmlElement elmSchoolYear;
        XmlElement elmSemester;

        public GenerateCourse()
        {
            InitializeComponent();

            this.dgvMotherData.CurrentCellDirtyStateChanged += new EventHandler(dgvMotherData_CurrentCellDirtyStateChanged);
            this.dgvMotherData.CellEnter += new DataGridViewCellEventHandler(dgvMotherData_CellEnter);
            this.dgvMotherData.ColumnHeaderMouseClick += new System.Windows.Forms.DataGridViewCellMouseEventHandler(this.dgvMotherData_ColumnHeaderMouseClick);
            this.dgvMotherData.RowHeaderMouseClick += new System.Windows.Forms.DataGridViewCellMouseEventHandler(this.dgvMotherData_RowHeaderMouseClick);
            this.dgvMotherData.MouseClick += new System.Windows.Forms.MouseEventHandler(this.dgvMotherData_MouseClick);
            this.dgvMotherData.EditingControlShowing += new System.Windows.Forms.DataGridViewEditingControlShowingEventHandler(this.dgvMotherData_EditingControlShowing);

            this.dgvChildData.DataError += new DataGridViewDataErrorEventHandler(dgvMotherData_DataError);
            this.dgvChildData.CurrentCellDirtyStateChanged += new EventHandler(dgvChildData_CurrentCellDirtyStateChanged);
            this.dgvChildData.CellEnter += new DataGridViewCellEventHandler(dgvChildData_CellEnter);
            this.dgvChildData.ColumnHeaderMouseClick += new System.Windows.Forms.DataGridViewCellMouseEventHandler(this.dgvChildData_ColumnHeaderMouseClick);
            this.dgvChildData.RowHeaderMouseClick += new System.Windows.Forms.DataGridViewCellMouseEventHandler(this.dgvChildData_RowHeaderMouseClick);
            this.dgvChildData.MouseClick += new System.Windows.Forms.MouseEventHandler(this.dgvChildData_MouseClick);

            this.txtSubjectCode.TextChanged += new System.EventHandler(this.txtSubjectCode_TextChanged);
            this.txtNewSubjectCode.TextChanged += new System.EventHandler(this.txtNewSubjectCode_TextChanged);
            this.cmdSelect.Click += new System.EventHandler(this.cmdSelect_Click);
            this.Save.Click += new System.EventHandler(this.Save_Click);

            this.Load += new System.EventHandler(this.GenerateCourse_Load);
        }

        private void GenerateCourse_Load(object sender, EventArgs e)
        {
            this.cboSemester.Items.Clear();

            foreach (SemesterItem semester in SemesterItem.GetSemesterList())
                this.cboSemester.Items.Add(semester);

            xmlSystemConfig = new XmlDocument();
            xmlSystemConfig.LoadXml(Config.App["系統設定"].PreviousData.OuterXml);
            elmSchoolYear = (XmlElement)xmlSystemConfig.DocumentElement.SelectSingleNode("DefaultSchoolYear");
            elmSemester = (XmlElement)xmlSystemConfig.DocumentElement.SelectSingleNode("DefaultSemester");

            this.nudSchoolYear.Value = int.Parse(elmSchoolYear.InnerText);
            this.cboSemester.SelectedItem = SemesterItem.GetSemesterByCode(elmSemester.InnerText);

            Access = new AccessHelper();
            subjects = new List<Subject>();

            bgWorker = new BackgroundWorker();
            bgWorker.DoWork += new DoWorkEventHandler(bgWorker_DoWork);
            bgWorker.RunWorkerCompleted += new RunWorkerCompletedEventHandler(bgWorker_RunWorkerCompleted);

            bgWorker.RunWorkerAsync();
        }

        private void bgWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            try
            {
                e.Result = Access.Select<Subject>();
            }
            catch (Exception ex)
            {
                e.Result = new Exception(ex.Message);
            }
        }

        private void bgWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if (e.Result != null && e.Result.GetType().Equals(Type.GetType("System.Exception")))
                MessageBox.Show(((System.Exception)e.Result).Message, "開課", MessageBoxButtons.OK, MessageBoxIcon.Information, MessageBoxDefaultButton.Button1, MessageBoxOptions.DefaultDesktopOnly);
            else
            {
                subjects = (e.Result as List<Subject>);
            }
        }

        private void txtSubjectCode_TextChanged(object sender, EventArgs e)
        {
            if (!bgWorker.IsBusy && subjects.Count > 0)
            {
                string lookUpValue = (sender as TextBox).Text.Trim();
                Subject matchSubject = subjects.Where(x=>x.SubjectCode.Trim().Length>0).ToList().FindLast(x => x.SubjectCode == lookUpValue);

                if (matchSubject == null)
                    return;

                if (this.dgvMotherData.Rows.Cast<DataGridViewRow>().Where(x => (x.Cells["SubjectCode"] != null && x.Cells["SubjectCode"].Value.ToString().Trim() == lookUpValue)).Count() == 0)
                {
                    AddSubjectToDataGridView(matchSubject);
                    (sender as TextBox).Text = "";
                }
            }
        }

        private void txtNewSubjectCode_TextChanged(object sender, EventArgs e)
        {
            if (!bgWorker.IsBusy && subjects.Count > 0)
            {
                string lookUpValue = (sender as TextBox).Text.Trim();
                Subject matchSubject = subjects.Where(x=>x.NewSubjectCode.Trim().Length>0).ToList().FindLast(x => x.NewSubjectCode == lookUpValue);

                if (matchSubject == null)
                    return;

                if (this.dgvMotherData.Rows.Cast<DataGridViewRow>().Where(x => (x.Cells["NewSubjectCode"] != null && x.Cells["NewSubjectCode"].Value.ToString().Trim() == lookUpValue)).Count() == 0)
                {
                    AddSubjectToDataGridView(matchSubject);
                    (sender as TextBox).Text = "";
                }
            }
        }

        private int AddSubjectToDataGridView(Subject matchSubject)
        {
            List<object> list = new List<object>();

            list.Add(matchSubject.SubjectCode);
            list.Add(matchSubject.Name);
            list.Add(matchSubject.EnglishName);
            list.Add(matchSubject.NewSubjectCode);
            list.Add(matchSubject.Credit);
            list.Add(matchSubject.IsRequired ? "必" : "選");
            list.Add("");
            list.Add(matchSubject.Description);
            list.Add(matchSubject.WebUrl);
            list.Add(matchSubject.Remark);
            list.Add(matchSubject.UID);

            int index = this.dgvMotherData.Rows.Add(list.ToArray());
            this.dgvMotherData.Rows[index].Tag = matchSubject;
            return index;
        }

        private void dgvMotherData_CurrentCellDirtyStateChanged(object sender, EventArgs e)
        {
            dgvMotherData.CommitEdit(DataGridViewDataErrorContexts.Commit);
        }

        private void dgvChildData_CurrentCellDirtyStateChanged(object sender, EventArgs e)
        {
            dgvChildData.CommitEdit(DataGridViewDataErrorContexts.Commit);
        }

        private void dgvChildData_CellEnter(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0 && e.ColumnIndex >= 0 && dgvChildData.SelectedCells.Count == 1)
            {
                dgvChildData.BeginEdit(true);

                if (e.ColumnIndex == 5 || e.ColumnIndex == 7)
                {
                    if (dgvChildData.CurrentCell != null && dgvChildData.CurrentCell.GetType().ToString() == "System.Windows.Forms.DataGridViewComboBoxCell")
                        (dgvChildData.EditingControl as ComboBox).DroppedDown = true;
                }
            }
        }

        private void dgvMotherData_CellEnter(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0 && e.ColumnIndex >= 0 && dgvMotherData.SelectedCells.Count == 1)
            {
                dgvMotherData.BeginEdit(true);
            }
        }

        private void dgvChildData_RowHeaderMouseClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            dgvChildData.CurrentCell = null;
            dgvChildData.Rows[e.RowIndex].Selected = true;
        }

        private void dgvChildData_ColumnHeaderMouseClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            dgvChildData.CurrentCell = null;
            dgvChildData.Columns[e.ColumnIndex].Selected = true;
        }

        private void dgvMotherData_RowHeaderMouseClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            dgvMotherData.CurrentCell = null;
            dgvMotherData.Rows[e.RowIndex].Selected = true;
        }

        private void dgvMotherData_ColumnHeaderMouseClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            dgvMotherData.CurrentCell = null;
            dgvMotherData.Columns[e.ColumnIndex].Selected = true;
        }

        private void dgvChildData_MouseClick(object sender, MouseEventArgs e)
        {
            MouseEventArgs args = (MouseEventArgs)e;
            DataGridView dgv = (DataGridView)sender;
            DataGridView.HitTestInfo hit = dgv.HitTest(args.X, args.Y);

            if (hit.Type == DataGridViewHitTestType.TopLeftHeader)
            {
                dgvChildData.CurrentCell = null;
                dgvChildData.SelectAll();
            }
        }

        private void dgvMotherData_MouseClick(object sender, MouseEventArgs e)
        {
            MouseEventArgs args = (MouseEventArgs)e;
            DataGridView dgv = (DataGridView)sender;
            DataGridView.HitTestInfo hit = dgv.HitTest(args.X, args.Y);

            if (hit.Type == DataGridViewHitTestType.TopLeftHeader)
            {
                dgvMotherData.CurrentCell = null;
                dgvMotherData.SelectAll();
            }
        }

        private void dgvMotherData_EditingControlShowing(object sender, DataGridViewEditingControlShowingEventArgs e)
        {
            //if (this.dgvMotherData.CurrentCell.ColumnIndex == 0)
            //{
            //    ComboBox comboBox = e.Control as ComboBox;

            //    comboBox.SelectedIndexChanged += new EventHandler(comboBox_SelectedIndexChanged);
            //}
        }

        private void dgvMotherData_DataError(object sender, DataGridViewDataErrorEventArgs e)
        {
            e.Cancel = true;
        }

        private void dgvChildData_DataError(object sender, DataGridViewDataErrorEventArgs e)
        {
            e.Cancel = true;
        }

        private void cmdSelect_Click(object sender, EventArgs e)
        {
            IEnumerable<DataGridViewRow> dgvrSubjects = this.dgvMotherData.Rows.Cast<DataGridViewRow>();

            if (dgvrSubjects.Count() == 0)
                return;

            bool err = false;
            foreach (DataGridViewRow dataGridViewRow in dgvrSubjects)
            {
                int intSubjectAmount = 0;
                int.TryParse((dataGridViewRow.Cells["SubjectAmount"].Value + ""), out intSubjectAmount);

                if (intSubjectAmount > 3 || intSubjectAmount < 1)
                    err = true;
            }
            if (err)
            {
                MsgBox.Show("開課數僅允許「1、2、3」");
                return;
            }
            this.dgvChildData.Rows.Clear();
            foreach (DataGridViewRow dataGridViewRow in dgvrSubjects)
            {
                int intSubjectAmount = 0;
                int.TryParse(dataGridViewRow.Cells["SubjectAmount"].Value.ToString(), out intSubjectAmount);

                //if (intSubjectAmount == 0)
                //    intSubjectAmount = 1;

                for (int i = 1; i <= intSubjectAmount; i++)
                {
                    if (intSubjectAmount == 1)
                        MakeCourse("", dataGridViewRow);
                    else
                        MakeCourse(i.ToString("00"), dataGridViewRow);
                }
            }
        }

        private int MakeCourse(string courseItemNo, DataGridViewRow dataGridViewRow)
        {
            IEnumerable<DataGridViewRow> dgvrCourses = this.dgvChildData.Rows.Cast<DataGridViewRow>();

            if (dgvrCourses.Where(x => (x.Cells["CourseSubjectName"].Value.ToString() == dataGridViewRow.Cells["SubjectChineseName"].Value.ToString() && courseItemNo == x.Cells["CourseItemNo"].Value.ToString())).Count() == 0)
            {
                List<object> list = new List<object>();

                list.Add(dataGridViewRow.Cells["SubjectChineseName"].Value.ToString() + " " + courseItemNo);
                list.Add(dataGridViewRow.Cells["SubjectChineseName"].Value.ToString());
                list.Add(dataGridViewRow.Cells["SubjectCode"].Value.ToString());
                list.Add(dataGridViewRow.Cells["NewSubjectCode"].Value.ToString());
                list.Add(dataGridViewRow.Cells["Credit"].Value.ToString());
                list.Add(dataGridViewRow.Cells["Required"].Value.ToString());
                list.Add(courseItemNo);
                list.Add("");
                list.Add("");
                list.Add("");
                list.Add(dataGridViewRow.Cells["SubjectID"].Value.ToString());

                int rowIndex = this.dgvChildData.Rows.Add(list.ToArray());
                return rowIndex;
            }
            return 0;
        }

        private void Save_Click(object sender, EventArgs e)
        {
            List<DataGridViewRow> giveupRows = new List<DataGridViewRow>();
            List<DataGridViewRow> insertRows = new List<DataGridViewRow>();

            IEnumerable<DataGridViewRow> dgvrCourses = this.dgvChildData.Rows.Cast<DataGridViewRow>();
            
            if (dgvrCourses.Count() == 0)
                return;

            QueryHelper helper = new QueryHelper();

            DataTable courseTable = helper.Select(string.Format("select course_name from course left join $ischool.emba.course_ext on course.id=$ischool.emba.course_ext.ref_course_id where course.school_year='{0}' and course.semester='{1}'", this.nudSchoolYear.Value, (this.cboSemester.SelectedItem as SemesterItem).Value));

            IEnumerable<DataRow> tableCourses = courseTable.Rows.Cast<DataRow>();
            
            foreach (DataGridViewRow dataGridViewRow in dgvrCourses)
            {
                if (tableCourses.Where(x => dataGridViewRow.Cells["CourseName"].Value.ToString() == x["course_name"].ToString()).Count() == 0)
                    insertRows.Add(dataGridViewRow);
                else
                    giveupRows.Add(dataGridViewRow);
            }

            foreach (DataGridViewRow dataGridViewRow in insertRows)
            {
                SHCourseRecord course = new SHCourseRecord();
                course.Name = dataGridViewRow.Cells["CourseName"].Value.ToString();
                course.SchoolYear = int.Parse(this.nudSchoolYear.Value.ToString());
                course.Semester = int.Parse((this.cboSemester.SelectedItem as SemesterItem).Value);
                int intSubjectID = 0;
                int.TryParse(dataGridViewRow.Cells["RefSubjectID"].Value.ToString(), out intSubjectID);
                course.Subject = intSubjectID.ToString();
                decimal credit = 0;
                decimal.TryParse(dataGridViewRow.Cells["Credits"].Value.ToString(), out credit);
                course.Credit = credit;

                string courseID = SHCourse.Insert(course);

                CourseExt courseExt = new CourseExt();

                int intCapacity = 0;
                int.TryParse(dataGridViewRow.Cells["Capacity"].Value.ToString(), out intCapacity);
                courseExt.Capacity = intCapacity;
                courseExt.ClassName = dataGridViewRow.Cells["CourseItemNo"].Value.ToString();
                courseExt.CourseID = int.Parse(courseID);
                courseExt.CourseType = dataGridViewRow.Cells["CourseType"].Value.ToString();
                courseExt.IsRequired = (dataGridViewRow.Cells["CourseRequired"].Value.ToString() == "必" ? true : false) ;
                courseExt.NewSubjectCode = dataGridViewRow.Cells["CourseNewSubjectCode"].Value.ToString();
                int intSerialNo = 0;
                int.TryParse(dataGridViewRow.Cells["SerialNo"].Value.ToString(), out intSerialNo);
                courseExt.SerialNo = intSerialNo;
                courseExt.SubjectCode = dataGridViewRow.Cells["CourseSubjectCode"].Value.ToString();
                courseExt.SubjectID = intSubjectID;
                courseExt.Save();
            }
            string message = string.Empty;
            if (giveupRows.Count > 0)
            {
                message = "下列課程已存在並不儲存：\n";
                giveupRows.ForEach((x) =>
                {
                    message += x.Cells["CourseName"].Value.ToString() + "\n";
                });
            }
            if (insertRows.Count > 0)
            {
                if (!string.IsNullOrEmpty(message))
                    message += "\n\n";

                message += "下列課程儲存成功：\n";
                insertRows.ForEach((x) =>
                {
                    message += x.Cells["CourseName"].Value.ToString() + "\n";
                });
            }
            if (!string.IsNullOrEmpty(message))
                MsgBox.Show(message, "開課");

            this.Close();
                //MessageBox.Show(message, "開課", MessageBoxButtons.OK, MessageBoxIcon.Information, MessageBoxDefaultButton.Button1, MessageBoxOptions.DefaultDesktopOnly);
        }

        public int AddSubject(UDT.Subject subject)
        {
            IEnumerable<DataGridViewRow> dgvrs = this.dgvMotherData.Rows.Cast<DataGridViewRow>();

            if (dgvrs.Where(x => ((x.Tag as UDT.Subject).UID == subject.UID)).Count() == 0)
            {
                return AddSubjectToDataGridView(subject);
            }
            return 0;
        }

        private void cmdSelectSubjects_Click(object sender, EventArgs e)
        {
            SubjectForm frm = new SubjectForm(this);
            this.Left = 20;
            frm.Show(this);
            frm.Top = this.Top;
            frm.Left = this.Left + this.Width + 20;
            //if (frm.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            //{
            //    List<UDT.Subject> subjects = frm.Subjects;

            //    if (subjects == null || subjects.Count == 0)
            //        return;

            //    IEnumerable<DataGridViewRow> dgvrs = this.dgvMotherData.Rows.Cast<DataGridViewRow>();
            //    subjects.ForEach((y) =>
            //    {
            //        if (dgvrs.Where(x => ((x.Tag as UDT.Subject).UID == y.UID)).Count() == 0)
            //        {
            //            AddSubjectToDataGridView(y);
            //        }
            //    });
            //}
        }

        private void Save_Click_1(object sender, EventArgs e)
        {

        }
    }
}