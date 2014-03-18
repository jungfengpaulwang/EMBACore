using System;
using System.Collections.Generic;
using System.Windows.Forms;
using System.Xml;
using DevComponents.DotNetBar;
using FCode = FISCA.Permission.FeatureCodeAttribute;
using K12.Data;
using System.ComponentModel;
using FISCA.Data;
using System.Data;
using EMBACore.UDT;
using System.Dynamic;
using Campus.Windows;
using FISCA.UDT;
using System.Linq;
using System.Text;
using FISCA.LogAgent;
using FISCA.Permission;
using EMBACore.Log;
using FISCA.Presentation;

namespace EMBACore.DetailItems
{
    [AccessControl("ischool.EMBA.Course_SCAttend", "修課學生管理", "課程>資料項目")]
    internal partial class Course_SCAttend : DetailContentImproved 
    {
        private object Result;
        private List<string> _StudIDList;
        private AccessHelper Access;
        public Course_SCAttend()
        {
            InitializeComponent();
            this.Load += new System.EventHandler(this.Form_Load);
            this.Group = "修課學生管理";
        }

        private void Form_Load(object sender, EventArgs e)
        {
            this.WatchChange(new DataGridViewSource(this.dgvData));
            Course.AfterChange += (x, y) => ReInitialize();

            InitializeOthers();

            _StudIDList = new List<string>();
            Access = new AccessHelper();


            /* Check Permission */
            bool canEdit = Permission.Editable;
            this.btnAdd.Enabled = canEdit;
            this.btnRemove.Enabled = canEdit;
        }

        private void InitializeOthers()
        {
            this.dgvData.DataError += new DataGridViewDataErrorEventHandler(dgvData_DataError);
            this.dgvData.CurrentCellDirtyStateChanged += new EventHandler(dgvData_CurrentCellDirtyStateChanged);
            this.dgvData.CellEnter += new DataGridViewCellEventHandler(dgvData_CellEnter);
            this.dgvData.ColumnHeaderMouseClick += new System.Windows.Forms.DataGridViewCellMouseEventHandler(this.dgvData_ColumnHeaderMouseClick);
            this.dgvData.RowHeaderMouseClick += new System.Windows.Forms.DataGridViewCellMouseEventHandler(this.dgvData_RowHeaderMouseClick);
            this.dgvData.MouseClick += new System.Windows.Forms.MouseEventHandler(this.dgvData_MouseClick);
        }

        private void dgvData_DataError(object sender, DataGridViewDataErrorEventArgs e)
        {
            e.Cancel = true;
        }

        private void dgvData_CurrentCellDirtyStateChanged(object sender, EventArgs e)
        {
            dgvData.CommitEdit(DataGridViewDataErrorContexts.Commit);
        }

        private void dgvData_CellEnter(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0 && e.ColumnIndex >= 0 && dgvData.SelectedCells.Count == 1)
                dgvData.BeginEdit(true);
        }

        private void dgvData_RowHeaderMouseClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            dgvData.CurrentCell = null;
            dgvData.Rows[e.RowIndex].Selected = true;
        }

        private void dgvData_ColumnHeaderMouseClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            dgvData.CurrentCell = null;
            dgvData.Columns[e.ColumnIndex].Selected = true;
        }

        private void dgvData_MouseClick(object sender, MouseEventArgs e)
        {
            MouseEventArgs args = (MouseEventArgs)e;
            DataGridView dgv = (DataGridView)sender;
            DataGridView.HitTestInfo hit = dgv.HitTest(args.X, args.Y);

            if (hit.Type == DataGridViewHitTestType.TopLeftHeader)
            {
                dgvData.CurrentCell = null;
                dgvData.SelectAll();
            }
        }

        //載入修課學生
        private void LoadData()
        {
            string strSQL = string.Format(@"select $ischool.emba.scattend_ext.uid as sc_attend_id, $ischool.emba.scattend_ext.ref_student_id as ref_student_id, student.name as name, class_name, student_number, gender, report_group, is_cancel from $ischool.emba.scattend_ext join student on student.id=$ischool.emba.scattend_ext.ref_student_id left outer join class on class.id=student.ref_class_id where $ischool.emba.scattend_ext.ref_course_id='{0}';", PrimaryKey);

            QueryHelper helper = new QueryHelper();

            this.Result = helper.Select(strSQL);
        }

        protected override void OnInitializeAsync()
        {
            //LoadData();
        }

        protected override void OnInitializeComplete(Exception error)
        {
            if (error != null) //有錯就直接丟出去吧。
                throw error;

            //BindDataToForm(this.Result);
        }

        protected override void OnPrimaryKeyChangedAsync()
        {
            LoadData();
        }

        protected override void OnPrimaryKeyChangedComplete(Exception error)
        {
            if (error != null) //有錯就直接丟出去吧。
                throw error;

            ErrorTip.Clear();

            BindDataToForm(this.Result);

            ResetDirtyStatus();
        }

        protected override void OnDirtyStatusChanged(ChangeEventArgs e)
        {
            if (UserAcl.Current[this.GetType()].Editable)
                SaveButtonVisible = e.Status == ValueStatus.Dirty;
            else
                this.SaveButtonVisible = false;

            CancelButtonVisible = e.Status == ValueStatus.Dirty;
        }

        private void BindDataToForm(object dataTable)
        {
            this.dgvData.Rows.Clear();

            if (dataTable == null)
                return;

            foreach (DataRow row in (dataTable as DataTable).Rows)
            {
                List<object> list = new List<object>();

                list.Add(row["class_name"].ToString());
                list.Add(row["student_number"].ToString());
                list.Add(row["name"].ToString());
                list.Add((row["gender"].ToString() == "1" ? "男" : "女"));
                list.Add(row["report_group"].ToString());
                bool isCancel = false;
                bool.TryParse(row["is_cancel"].ToString(), out isCancel);
                list.Add(isCancel);

                int idx = this.dgvData.Rows.Add(list.ToArray());

                dynamic SCAttend = new ExpandoObject();
                SCAttend.SCAttendID = row["sc_attend_id"].ToString();
                SCAttend.StudentID = row["ref_student_id"].ToString();
                SCAttend.Group = row["report_group"].ToString();
                SCAttend.CourseID = PrimaryKey;
                SCAttend.IsCancel = isCancel;

                this.dgvData.Rows[idx].Tag = SCAttend;

                //    _dicUTDs.Add(ticr.UID, ticr.Clone());

                //    DataGridViewComboBoxCell dataGridViewComboBoxCell = (this.dgvData.Rows[idx].Cells[1] as DataGridViewComboBoxCell);

                //    if (udtRecords.Count > 0 && udtRecords.Where(x => x.ItemType == ticr.ItemType).Count() > 0)
                //    {
                //        IEnumerable<string> items = udtRecords.Where(x => x.ItemType == ticr.ItemType).Select(x => x.Item);
                //        for (int i = 0; i < items.Count(); i++)
                //        {
                //            dataGridViewComboBoxCell.Items.Add(items.ElementAt(i));
                //        }
                //        dataGridViewComboBoxCell.Value = ticr.Item;
                //    }
                //}
                //this.dgvData.AutoResizeColumns();
                //this.dgvData.CurrentCell = null;
            }
            this.label2.Text = this.dgvData.Rows.Count.ToString();
        }

        private void AppendDataToForm(object dataTable)
        {
            if (dataTable == null)
                return;

            foreach (DataRow row in (dataTable as DataTable).Rows)
            {
                List<object> list = new List<object>();

                list.Add(row["class_name"].ToString());
                list.Add(row["student_number"].ToString());
                list.Add(row["name"].ToString());
                list.Add((row["gender"].ToString() == "1" ? "男" : "女"));
                list.Add(row["report_group"].ToString());
                bool isCancel = false;
                bool.TryParse(row["is_cancel"].ToString(), out isCancel);
                list.Add(isCancel);

                int idx = this.dgvData.Rows.Add(list.ToArray());

                dynamic SCAttend = new ExpandoObject();
                SCAttend.SCAttendID = row["sc_attend_id"].ToString();
                SCAttend.StudentID = row["ref_student_id"].ToString();
                SCAttend.Group = row["report_group"].ToString();
                SCAttend.CourseID = PrimaryKey;
                SCAttend.IsCancel = isCancel;

                this.dgvData.Rows[idx].Tag = SCAttend;
            }
            this.label2.Text = this.dgvData.Rows.Count.ToString();
        }

        private bool RemoveSelected()
        {
            List<string> attendRecordIDs = new List<string>();
            IEnumerable<DataGridViewRow> selectedRows = this.dgvData.SelectedRows.Cast<DataGridViewRow>();

            if (selectedRows.Count() == 0)
                return false;
            //  ischool.emba.subject_semester_score
            AccessHelper Access = new AccessHelper();

            List<SubjectSemesterScore> subjectSemesterScores = Access.Select<SubjectSemesterScore>(string.Format("ref_student_id in {0} And ref_course_id='{1}'", "(" + String.Join(",", selectedRows.Select(x => (x.Tag as dynamic).StudentID)) + ")", PrimaryKey));

            List<StudentRecord> studentRecords = Student.SelectByIDs(subjectSemesterScores.Where(x => !string.IsNullOrWhiteSpace(x.Score)).Select(x => x.StudentID.ToString()));
            string msg = string.Empty;
            if (studentRecords.Count > 0)
            {
                studentRecords.ForEach((x) => msg += ("教學分班：" + x.Class.Name + "，學生姓名：" + x.Name + "\n"));
                MsgBox.Show("下列學生已經有課程學期成績，請先刪除課程學期成績再移除學生修課記錄。\n\n" + msg);

                return false;
            }

            foreach (DataGridViewRow row in selectedRows)
            {
                this.dgvData.Rows.Remove(row);
                attendRecordIDs.Add((row.Tag as dynamic).SCAttendID);
            }
            List<SCAttendExt> scattendExts = Access.Select<SCAttendExt>(string.Format("uid in {0}", "(" + String.Join(",", attendRecordIDs.Select(x => x)) + ")"));

            if (scattendExts.Count > 0)
            {
                if (MsgBox.Show("確定移除學生修課記錄？", MessageBoxButtons.OKCancel) == DialogResult.Cancel)
                    return false;

                scattendExts.ForEach(x => x.Deleted = true);
                List<string> deletedRecordUIDs = scattendExts.SaveAll();

                if (scattendExts.Count > 0)
                {
                    CourseRecord courseRecord = Course.SelectByID(PrimaryKey);
                    Dictionary<string, StudentRecord> dicStudentRecords = Student.SelectByIDs(scattendExts.Select(x => x.StudentID.ToString())).ToDictionary(x=>x.ID);

                    LogSaver logBatch = ApplicationLog.CreateLogSaverInstance();
                    foreach (SCAttendExt deletedRecord in scattendExts)
                    {
                        StudentRecord student = dicStudentRecords[deletedRecord.StudentID.ToString()];

                        StringBuilder sb = new StringBuilder();
                        sb.Append("學生「" + student.Name + "」，學號「" + student.StudentNumber + "」");
                        sb.AppendLine("被刪除一筆「修課記錄」。");
                        sb.AppendLine("詳細資料：");
                        sb.Append("開課「" + courseRecord.Name + "」\n");
                        sb.Append("報告小組「" + deletedRecord.Group + "」\n");
                        sb.Append("停修「" + (deletedRecord.IsCancel ? "是" : "否")  + "」\n");

                        logBatch.AddBatch("管理學生修課.刪除", "刪除", "course", PrimaryKey, sb.ToString());
                    }
                    logBatch.LogBatch(true);
                }
            }

            this.label2.Text = this.dgvData.Rows.Count.ToString();
            return true;
        }

        private void btnRemove_Click(object sender, EventArgs e)
        {
            if (this.dgvData.SelectedRows.Count > 0)
            {
                BeginChangeControlData();
                RemoveSelected();
                EndChangeControlData();
            }
            label2.Text = this.dgvData.Rows.Count.ToString();
        }

        private void btnAdd_PopupOpen(object sender, EventArgs e)
        {
            //依學生的「待處理」清單建立功能表項目。
            CreateStudentMenuItem();
            //同步功能表狀態，將已存在於 ListView 中的項目  Disable。
            SyncStudentMenuItemStatus();
        }

        private void CreateStudentMenuItem()
        {
            btnAdd.SubItems.Clear();

            if (K12.Presentation.NLDPanels.Student.TempSource.Count == 0)
            {
                LabelItem item = new LabelItem("No", "沒有任何學生在待處理");
                btnAdd.SubItems.Add(item);
                return;
            }

            foreach (StudentRecord each in Student.SelectByIDs(K12.Presentation.NLDPanels.Student.TempSource))
            {
                ButtonItem item = new ButtonItem(each.ID, each.Name + " (" + (each.Class == null ? "" : each.Class.Name) + ")");
                item.Tooltip = "班級：" + (each.Class == null ? "" : each.Class.Name) + "\n學號：" + each.StudentNumber;
                item.Tag = each;
                item.Click += new EventHandler(AttendStudent_Click);

                btnAdd.SubItems.Add(item);
            }
        }

        private void btnAdd_Click(object sender, EventArgs e)
        {
            CreateStudentMenuItem();
            SyncStudentMenuItemStatus();

            _StudIDList.Clear();
            List<StudentRecord> students = new List<StudentRecord>();
            foreach (object obj in btnAdd.SubItems)
            {
                ButtonItem each = obj as ButtonItem;
                if (each != null && each.Enabled)
                {
                    StudentRecord student = each.Tag as StudentRecord;
                    if (student != null)
                    {
                        students.Add(student);
                        _StudIDList.Add(student.ID);
                    }
                }
            }

            BeginChangeControlData();
            BatchAddAddend(students);
            EndChangeControlData();
            SyncStudentMenuItemStatus();
        }

        private void BatchAddAddend(List<StudentRecord> students)
        {
            // 檢查是否要加入
            List<string> willAdd = new List<string>();
            IEnumerable<DataGridViewRow> rows = this.dgvData.Rows.Cast<DataGridViewRow>();
            foreach (StudentRecord student in students)
            {
                if (rows.Where(x => ((x.Tag as dynamic).StudentID == student.ID)).Count() > 0)
                    continue;

                willAdd.Add(student.ID);
            }

            if (willAdd.Count == 0)
                return;

            Save(willAdd);

            string strSQL = string.Format(@"select $ischool.emba.scattend_ext.uid as sc_attend_id, student.id as ref_student_id, student.name as name, class_name, student_number, gender, report_group, is_cancel from student left join class on class.id=student.ref_class_id join $ischool.emba.scattend_ext on $ischool.emba.scattend_ext.ref_student_id=student.id join course on $ischool.emba.scattend_ext.ref_course_id=course.id where student.id in {0} and course.id='{1}';", "(" + String.Join(",", willAdd.Select(x => ("'" + x + "'"))) + ")", PrimaryKey);

            QueryHelper helper = new QueryHelper();

            DataTable dataTable = helper.Select(strSQL);

            AppendDataToForm(dataTable);
        }

        private void SyncStudentMenuItemStatus()
        {
            Dictionary<string, ButtonItem> _students = new Dictionary<string, ButtonItem>();
            foreach (object obj in btnAdd.SubItems)
            {
                ButtonItem each = obj as ButtonItem;
                if (each == null) continue;

                StudentRecord stu = each.Tag as StudentRecord;

                if (stu != null)
                {
                    if (stu.Status != StudentRecord.StudentStatus.一般)
                    {
                        each.Enabled = false;
                        each.Tooltip = "需為在校生才能加入課程。";
                    }
                    else
                        _students.Add(stu.ID, each);
                }
            }
            foreach (DataGridViewRow row in this.dgvData.Rows)
            {
                dynamic SCAttend = (row.Tag as dynamic);
                if (_students.ContainsKey(SCAttend.StudentID))
                {
                    _students[SCAttend.StudentID].Enabled = false;
                    _students[SCAttend.StudentID].Tooltip = "此學生已在修課清單中";
                }
            }
        }

        private void AttendStudent_Click(object sender, EventArgs e)
        {
            StudentRecord student = (sender as ButtonItem).Tag as StudentRecord;
            _StudIDList.Clear();
            if (student != null)
            {
                BeginChangeControlData();
                AddAddend(student);
                EndChangeControlData();
                _StudIDList.Add(student.ID);
            }
        }

        private void AddAddend(StudentRecord student)
        {
            // 檢查是否要加入
            bool CheckAdd = true;
            IEnumerable<DataGridViewRow> students = this.dgvData.Rows.Cast<DataGridViewRow>();

            if (students.Count() > 0)
                if (students.Where(x => (x.Tag as dynamic).StudentID == student.ID).Count() > 0)
                    CheckAdd = false;

            if (CheckAdd)
            {
                Save(new List<string>() { student.ID });

                string strSQL = string.Format(@"select $ischool.emba.scattend_ext.uid as sc_attend_id, student.id as ref_student_id, student.name as name, class_name, student_number, gender, report_group, is_cancel from student left join class on class.id=student.ref_class_id join $ischool.emba.scattend_ext on $ischool.emba.scattend_ext.ref_student_id=student.id join course on $ischool.emba.scattend_ext.ref_course_id=course.id where student.id='{0}' and $ischool.emba.scattend_ext.ref_course_id='{1}';", student.ID, PrimaryKey);

                QueryHelper helper = new QueryHelper();

                DataTable dataTable = helper.Select(strSQL);

                AppendDataToForm(dataTable);
            }
        }

        public void Save(List<string> students)
        {
            List<SCAttendExt> scattendRecords = new List<SCAttendExt>();
            foreach (string student in students)
            {
                SCAttendExt scattendRecord = new SCAttendExt();
                scattendRecord.StudentID = int.Parse(student);
                scattendRecord.CourseID = int.Parse(PrimaryKey);
                scattendRecord.IsCancel = false;
                scattendRecords.Add(scattendRecord);
            }
            try
            {
                List<string> insertedRecordUIDs = scattendRecords.SaveAll();
                List<K12.Data.StudentRecord> studentRecords = new List<StudentRecord>();
                if (insertedRecordUIDs != null && insertedRecordUIDs.Count > 0)
                {
                    //  寫入「新增」的 Log
                    List<UDT.SCAttendExt> insertedRecords = Access.Select<UDT.SCAttendExt>(insertedRecordUIDs);
                    Dictionary<string, UDT.SCAttendExt> dicInsertedRecords = new Dictionary<string, SCAttendExt>();
                    if (insertedRecords.Count > 0)
                        dicInsertedRecords = insertedRecords.ToDictionary(x => x.UID);
                    CourseRecord courseRecord = Course.SelectByID(PrimaryKey);
                    studentRecords = K12.Data.Student.SelectByIDs(insertedRecords.Select(x => x.StudentID.ToString()));
                    Dictionary<string, StudentRecord> dicStudentRecords = new Dictionary<string, StudentRecord>();
                    if (studentRecords.Count > 0)
                        dicStudentRecords = studentRecords.ToDictionary(x => x.ID);
                    LogSaver logBatch = ApplicationLog.CreateLogSaverInstance();
                    foreach (string iRecords in dicInsertedRecords.Keys)
                    {
                        UDT.SCAttendExt insertedRecord = dicInsertedRecords[iRecords];
                        StudentRecord student = dicStudentRecords[insertedRecord.StudentID.ToString()];

                        StringBuilder sb = new StringBuilder();
                        sb.Append("學生「" + student.Name + "」，學號「" + student.StudentNumber + "」");
                        sb.AppendLine("被新增一筆「修課記錄」。");
                        sb.AppendLine("詳細資料：");
                        sb.Append("開課「" + courseRecord.Name + "」\n");
                        logBatch.AddBatch("管理學生修課.新增", "新增", "course", PrimaryKey, sb.ToString());
                    }
                    logBatch.LogBatch(true);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        protected override void OnSaveData()
        {
            //SCAttend.SCAttendID = row["sc_attend_id"].ToString();
            //SCAttend.StudentID = row["ref_student_id"].ToString();
            //SCAttend.Group = row["report_group"].ToString();
            //SCAttend.CourseID = PrimaryKey;
            //SCAttend.IsCancel = isCancel;
            //  1、先取得 SCAttendExt 物件
            List<dynamic> SCAttendExts = new List<dynamic>();
            foreach (DataGridViewRow row in this.dgvData.Rows)
            {
                if (row.IsNewRow)
                    continue;

                dynamic SCAttend = (row.Tag as dynamic);
                SCAttendExts.Add(SCAttend);
            }
            if (SCAttendExts.Count == 0)
                return;

            List<string> studentIDs = new List<string>();
            foreach (dynamic SCAttend in SCAttendExts)
                studentIDs.Add(SCAttend.StudentID);

            List<SCAttendExt> scattendExts = Access.Select<SCAttendExt>(string.Format("ref_student_id in ({0}) And ref_course_id='{1}'", string.Join(",", studentIDs), PrimaryKey));
            List<SCAttendExt> originalExts = new List<SCAttendExt>();
            if (scattendExts != null && scattendExts.Count>0)
                scattendExts.ForEach(x => originalExts.Add(x.Clone()));
            //  2、更新 SCAttendExt 物件內容
            List<SCAttendExt> updateRecords = new List<SCAttendExt>();
            foreach (DataGridViewRow row in this.dgvData.Rows)
            {
                if (row.IsNewRow)
                    continue;

                dynamic SCAttend = (row.Tag as dynamic);

                IEnumerable<SCAttendExt> SCAttendExtRecords = scattendExts.Where(x => (x.UID == SCAttend.SCAttendID));

                SCAttendExt scattendExtRecord = SCAttendExtRecords.ElementAt(0);

                scattendExtRecord.Group = (row.Cells["ReportGroup"].Value == null ? "" : row.Cells["ReportGroup"].Value.ToString());
                bool isCancel = false;
                bool.TryParse((row.Cells["IsCancel"].Value == null ? "" : row.Cells["IsCancel"].Value.ToString()), out isCancel);
                scattendExtRecord.IsCancel = isCancel;

                if (scattendExtRecord.RecordStatus == FISCA.UDT.RecordStatus.Update)
                    updateRecords.Add(scattendExtRecord);
            }

            List<string> updatedRecordUIDs = updateRecords.SaveAll();
            BatchLogAgent batchLogAgent = new BatchLogAgent();
            if (updatedRecordUIDs != null && updatedRecordUIDs.Count > 0)
            {
                List<SCAttendExt> updatedRecords = Access.Select<SCAttendExt>(updatedRecordUIDs);
                Dictionary<string, UDT.SCAttendExt> dicUpdatedRecords = new Dictionary<string, SCAttendExt>();
                if (updatedRecords.Count > 0)
                    dicUpdatedRecords = updatedRecords.ToDictionary(x => x.UID);
                CourseRecord courseRecord = Course.SelectByID(PrimaryKey);
                List<StudentRecord> studentRecords = K12.Data.Student.SelectByIDs(updatedRecords.Select(x => x.StudentID.ToString()));
                Dictionary<string, StudentRecord> dicStudentRecords = new Dictionary<string, StudentRecord>();
                if (studentRecords.Count > 0)
                    dicStudentRecords = studentRecords.ToDictionary(x => x.ID); 
                foreach (string iRecords in dicUpdatedRecords.Keys)
                {
                    UDT.SCAttendExt updatedRecord = dicUpdatedRecords[iRecords];
                    StudentRecord student = dicStudentRecords[updatedRecord.StudentID.ToString()];
                    IEnumerable<UDT.SCAttendExt> originalRecords = originalExts.Where(x => (x.StudentID.ToString() == student.ID && x.CourseID == updatedRecord.CourseID));

                    LogAgent log = new LogAgent();
                    this.AddLog(log, originalRecords.ElementAt(0));
                    this.AddLog(log, updatedRecord);
                    batchLogAgent.AddLogAgent(log);
                    //StringBuilder sb = new StringBuilder();
                    //sb.Append("學生「" + student.Name + "」，學號「" + student.StudentNumber + "」");
                    //sb.AppendLine("被修改一筆「修課記錄」。");
                    //sb.AppendLine("詳細資料：");
                    //sb.Append("開課「" + courseRecord.Name + "」\n");
                    //if (!updatedRecord.Group.Equals(originalRecords.ElementAt(0).Group))
                    //    sb.Append("報告小組由「" + originalRecords.ElementAt(0).Group + "」改為「" + updatedRecord.Group + "」\n");
                    //if (!updatedRecord.IsCancel.Equals(originalRecords.ElementAt(0).IsCancel))
                    //    sb.Append("停修由「" + originalRecords.ElementAt(0).IsCancel + "」改為「" + updatedRecord.IsCancel + "」\n");
                }
            }
            if (batchLogAgent.Count > 0)
                batchLogAgent.Save();

            LoadData();

            SaveButtonVisible = false;
            CancelButtonVisible = false;
        }

        void AddLog(LogAgent logAgent, SCAttendExt obj)
        {
            logAgent.SetLogValue("報告小組", obj.Group);
            logAgent.SetLogValue("停修", obj.IsCancel ? "是" : "否");
        }
    }
}