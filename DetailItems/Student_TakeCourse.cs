using Campus.Windows;
using EMBACore.DataItems;
using EMBACore.Extension.UDT;
using EMBACore.UDT;
using FISCA.Data;
using FISCA.Permission;
using FISCA.UDT;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Dynamic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Xml;
using System.Xml.Linq;

namespace EMBACore.DetailItems
{
    [AccessControl("ischool.EMBA.Student_TakeCourse", "修課紀錄", "學生>資料項目")]
    public partial class Student_TakeCourse : FISCA.Presentation.DetailContent
    {
        //  驗證資料物件
        private ErrorProvider _Errors;

        //  背景載入 UDT 資料物件
        private BackgroundWorker _BGWLoadData;
        private BackgroundWorker _BGWSaveData;

        //  監控 UI 資料變更
        private ChangeListen _Listener;

        //  正在下載的資料之主鍵，用於檢查是否下載他人資料，若 _RunningKey != PrimaryKey 就再下載乙次
        private string _RunningKey;
        private decimal _CurrentSchoolYear;
        private string _CurrentSemester;

        private Dictionary<string, string> dicCourses;

        private AccessHelper Access;
        private QueryHelper queryHelper;
        private bool form_loaded;        

        public Student_TakeCourse()
        {
            InitializeComponent();

            this.Group = "修課紀錄";
            _RunningKey = "";

            this.dicCourses = new Dictionary<string, string>();

            this.Load += new EventHandler(Form_Load);
            this.form_loaded = false;
            _Errors = new ErrorProvider();
            _Listener = new ChangeListen();
            _Listener.Add(new DataGridViewSource(this.dgvData));
            //_Listener.Add(new ComboBoxSource(this.cboSemester, ComboBoxSource.ListenAttribute.Text));
            //this.IsPublic.ValueChanged += new EventHandler(IsPublic_ValueChanged);
            //_Listener.Add(new CheckBoxSource(this.IsPublic));
            _Listener.StatusChanged += new EventHandler<ChangeEventArgs>(Listener_StatusChanged);

            _BGWLoadData = new BackgroundWorker();
            _BGWLoadData.DoWork += new DoWorkEventHandler(_BGWLoadData_DoWork);
            _BGWLoadData.RunWorkerCompleted += new RunWorkerCompletedEventHandler(_BGWLoadData_RunWorkerCompleted);

            _BGWSaveData = new BackgroundWorker();
            _BGWSaveData.DoWork += new DoWorkEventHandler(_BGWSaveData_DoWork);
            _BGWSaveData.RunWorkerCompleted += new RunWorkerCompletedEventHandler(_BGWSaveData_RunWorkerCompleted);

            this.nudSchoolYear.TextChanged += new EventHandler(SchoolYear_TextChanged);
        }

        private void Form_Load(object sender, EventArgs e)
        {
            Access = new AccessHelper();
            queryHelper = new QueryHelper();

            this.InitSchoolYear();
            this.InitSemester();
            this.form_loaded = true;
            //this._BGWLoadData.RunWorkerAsync();
        }
        private void InitSchoolYear()
        {
            int school_year;
            if (int.TryParse(K12.Data.School.DefaultSchoolYear, out school_year))
            {
                this.nudSchoolYear.Value = decimal.Parse(school_year.ToString());
            }
            else
            {
                this.nudSchoolYear.Value = decimal.Parse((DateTime.Today.Year - 1911).ToString());
            }
            this._CurrentSchoolYear = this.nudSchoolYear.Value;
        }

        private void InitSemester()
        {
            this.cboSemester.DataSource = DataItems.SemesterItem.GetSemesterList();
            this.cboSemester.ValueMember = "Value";
            this.cboSemester.DisplayMember = "Name";

            this.cboSemester.SelectedValue = K12.Data.School.DefaultSemester;
            this._CurrentSemester = GetSemesterCode(this.cboSemester.Text);
        }

        private void Listener_StatusChanged(object sender, ChangeEventArgs e)
        {
            if (UserAcl.Current[typeof(Student_Paper)].Editable)
                SaveButtonVisible = e.Status == ValueStatus.Dirty;
            else
                this.SaveButtonVisible = false;

            CancelButtonVisible = e.Status == ValueStatus.Dirty;
        }

        private void _BGWLoadData_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if (e.Error != null)
            {
                this.RefreshUI(null);
                this.lblMessage.Text = e.Error.Message;
                this.Loading = false;
                return;
            }

            if (_RunningKey != PrimaryKey || this._CurrentSchoolYear != this.nudSchoolYear.Value || this._CurrentSemester != GetSemesterCode(this.cboSemester.Text))
            {
                this.Loading = true;
                this._RunningKey = PrimaryKey;
                this._CurrentSchoolYear = this.nudSchoolYear.Value;
                this._CurrentSemester = GetSemesterCode(this.cboSemester.Text);
                this._BGWLoadData.RunWorkerAsync();
            }
            else
            {
                this.RefreshUI(e.Result);
            }
        }

        private void _BGWLoadData_DoWork(object sender, DoWorkEventArgs e)
        {
            string SQL = string.Format(@"select table_a.course_name, table_a.new_subject_code, table_a.course_type, case table_a.is_required when true then '必修' when false then '選修' else '' end as is_required , table_a.credit, table_a.report_group, case table_a.is_cancel when true then '是' else '' end as is_cancel, table_b.teacher_name, table_a.student_id, table_a.course_id from
(select course.course_name, ce.new_subject_code, ce.course_type, ce.is_required, course.credit, sc.report_group, course.id as course_id, is_cancel, student.id as student_id, course.school_year, course.semester from $ischool.emba.scattend_ext as sc join student on student.id=sc.ref_student_id
join course on course.id=sc.ref_course_id
left join $ischool.emba.course_ext as ce on ce.ref_course_id=course.id) as table_a
left join 
(
select  course.id as course_id, array_to_string(array_agg(teacher_name), '、') as teacher_name from course 
LEFT JOIN $ischool.emba.course_instructor ON $ischool.emba.course_instructor.ref_course_id = course.id 
LEFT JOIN teacher ON teacher.id = $ischool.emba.course_instructor.ref_teacher_id 
where teacher.id in (
select teacher.id from teacher
JOIN tag_teacher ON tag_teacher.ref_teacher_id = teacher.id 
JOIN tag ON tag.id = tag_teacher.ref_tag_id 
WHERE tag.category = 'Teacher' AND tag.prefix = '教師' group by teacher.id) group by course_id
order by course_id
) as table_b
on table_a.course_id=table_b.course_id
where table_a.student_id={2} and table_a.school_year='{0}' and table_a.semester='{1}'
order by new_subject_code", this._CurrentSchoolYear, this._CurrentSemester, this._RunningKey);

            DataTable dataTable = queryHelper.Select(SQL);

            e.Result = dataTable;
        }

        //  檢視不同資料項目即呼叫此方法，PrimaryKey 為資料項目的 Key 值。
        protected override void OnPrimaryKeyChanged(EventArgs e)
        {
            if (!this._BGWLoadData.IsBusy)
            {
                this.Loading = true;
                this._RunningKey = PrimaryKey;
                this._CurrentSchoolYear = this.nudSchoolYear.Value;
                this._CurrentSemester = GetSemesterCode(this.cboSemester.Text);
                this._BGWLoadData.RunWorkerAsync();
            }
        }

        //  更新資料項目內 UI 的資料
        private void RefreshUI(object result)
        {
            _Listener.SuspendListen();

            this.dgvData.Rows.Clear();
            this.dicCourses.Clear();

            if (result == null)
                return;
            else
                this.lblMessage.Text = string.Empty;

            DataTable dataTable = result as DataTable;
            foreach(DataRow row in dataTable.Rows)
            {
                List<object> source = new List<object>();

                source.Add(row["course_name"] + "");
                source.Add(row["new_subject_code"] + "");
                source.Add(row["course_type"] + "");
                source.Add(row["is_required"] + "");
                source.Add(row["credit"] + "");
                source.Add(row["report_group"] + "");
                source.Add(row["is_cancel"] + "");
                source.Add(row["teacher_name"] + "");

                int idx = this.dgvData.Rows.Add(source.ToArray());
                this.dgvData.Rows[idx].Tag = row["course_id"] + "";

                this.dicCourses.Add(row["course_id"] + "", row["course_name"] + "");
            }
            this.dgvData.CurrentCell = null;
            this.Loading = false;
            ResetOverrideButton();
        }

        protected override void OnCancelButtonClick(EventArgs e)
        {
            if (!_BGWLoadData.IsBusy)
                this._BGWLoadData.RunWorkerAsync();
        }

        protected override void OnSaveButtonClick(EventArgs e)
        {
            this.Loading = true;

            if (ShouldWeGo())
                this._BGWSaveData.RunWorkerAsync();
        }

        private void _BGWSaveData_DoWork(object sender, DoWorkEventArgs e)
        {
            SaveUDT(e.Argument.ToString());
        }

        private void _BGWSaveData_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            this._BGWLoadData.RunWorkerAsync();
        }

        private void SaveUDT(string semester)
        {
            List<string> attendRecordIDs = new List<string>();
            Condition condition = new Condition(new Dictionary<string, List<string>>() { { this._RunningKey, this.dicCourses.Keys.ToList() } });
            List<SCAttendExt> scattendExts = Access.Select<SCAttendExt>(condition);

            if (scattendExts.Count > 0)
            {
                scattendExts.ForEach(x => x.Deleted = true);
                List<string> deletedRecordUIDs = scattendExts.SaveAll();

                //if (scattendExts.Count > 0)
                //{
                //    CourseRecord courseRecord = Course.SelectByID(PrimaryKey);
                //    Dictionary<string, StudentRecord> dicStudentRecords = Student.SelectByIDs(scattendExts.Select(x => x.StudentID.ToString())).ToDictionary(x => x.ID);

                //    LogSaver logBatch = ApplicationLog.CreateLogSaverInstance();
                //    foreach (SCAttendExt deletedRecord in scattendExts)
                //    {
                //        StudentRecord student = dicStudentRecords[deletedRecord.StudentID.ToString()];

                //        StringBuilder sb = new StringBuilder();
                //        sb.Append("學生「" + student.Name + "」，學號「" + student.StudentNumber + "」");
                //        sb.AppendLine("被刪除一筆「修課記錄」。");
                //        sb.AppendLine("詳細資料：");
                //        sb.Append("開課「" + courseRecord.Name + "」\n");
                //        sb.Append("報告小組「" + deletedRecord.Group + "」\n");
                //        sb.Append("停修「" + (deletedRecord.IsCancel ? "是" : "否") + "」\n");

                //        logBatch.AddBatch("管理學生修課.刪除", "刪除", "course", PrimaryKey, sb.ToString());
                //    }
                //    logBatch.LogBatch(true);
                //}
            }
        }

        private bool ShouldWeGo()
        {
            this.dgvData.Rows.Cast<DataGridViewRow>().ToList().ForEach(x =>
            {
                string course_id = x.Tag + "";

                if (this.dicCourses.ContainsKey(course_id))
                    this.dicCourses.Remove(course_id);
            });

            if (this.dicCourses.Count() == 0)
                return false;

            //  ischool.emba.subject_semester_score
            AccessHelper Access = new AccessHelper();

            List<SubjectSemesterScore> subjectSemesterScores = Access.Select<SubjectSemesterScore>(string.Format("ref_student_id = {1} And ref_course_id in  ({0})", String.Join(",", this.dicCourses.Keys), this._RunningKey));

            string msg = string.Empty;
            string school_year = this.nudSchoolYear.Value + "";
            string semester = this.cboSemester.Text;
            if (subjectSemesterScores.Count > 0)
            {
                subjectSemesterScores.ForEach(x => msg += string.Format("學年度：{0}，學期：{1}，開課：{2}", school_year, semester, this.dicCourses[x.CourseID.ToString()] + "\n"));
                MsgBox.Show("請先刪除下列課程學期成績再移除學生修課記錄。\n\n" + msg);

                return false;
            }

            if (MsgBox.Show("確定移除學生修課記錄？", MessageBoxButtons.OKCancel) == DialogResult.Cancel)
                return false;

            return true;
        }

        private void ResetOverrideButton()
        {
            SaveButtonVisible = false;
            CancelButtonVisible = false;

            _Listener.Reset();
            _Listener.ResumeListen();
        }

        private string GetSemesterName(string code)
        {
            if (string.IsNullOrEmpty(code))
                return string.Empty;
            else
                return SemesterItem.GetSemesterByCode(code).Name;
        }

        private string GetSemesterCode(string name)
        {
            if (string.IsNullOrEmpty(name))
                return string.Empty;
            else
                return SemesterItem.GetSemesterByName(name) == null ? string.Empty : SemesterItem.GetSemesterByName(name).Value;
        }

        private void SchoolYear_TextChanged(object sender, EventArgs e)
        {
            uint d;
            if (!string.IsNullOrEmpty(this.nudSchoolYear.Text) && !uint.TryParse(nudSchoolYear.Text, out d))
            {
                _Errors.SetError(this.nudSchoolYear, "僅允許正整數。");
                return;
            }
            else
                _Errors.SetError(this.nudSchoolYear, string.Empty);
        }

        private void nudSchoolYear_ValueChanged(object sender, EventArgs e)
        {
            if (!this._BGWLoadData.IsBusy && this.form_loaded)
            {
                this.Loading = true;
                this._RunningKey = PrimaryKey;
                this._CurrentSchoolYear = this.nudSchoolYear.Value;
                this._CurrentSemester = GetSemesterCode(this.cboSemester.Text);
                this._BGWLoadData.RunWorkerAsync();
            }
        }

        private void cboSemester_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (!this._BGWLoadData.IsBusy && this.form_loaded)
            {
                this.Loading = true;
                this._RunningKey = PrimaryKey;
                this._CurrentSchoolYear = this.nudSchoolYear.Value;
                this._CurrentSemester = GetSemesterCode(this.cboSemester.Text);
                this._BGWLoadData.RunWorkerAsync();
            }
        }
    }

    public class Condition : FISCA.UDT.Condition.ICondition
    {
        //<Condition>
        //    <In FieldName="ref_course_id">
        //        <Value>391</Value>
        //        <Value>385</Value>
        //    </In>
        //    <In FieldName="ref_student_id">
        //        <Value>10416</Value>
        //    </In>
        //</Condition>
        private Dictionary<string, List<string>> dicFields;
        public Condition(Dictionary<string, List<string>> Fields) 
        {
            this.dicFields = Fields;
        }
        
        System.Xml.XmlElement FISCA.UDT.Condition.ICondition.GetCondtionElement()
        {
            if (this.dicFields.Count == 0)
                return null;

            StringBuilder sb = new StringBuilder();
            foreach(KeyValuePair<string, List<string>> kv in this.dicFields)
            {
                sb.Append(string.Format("<In FieldName='{0}'>", kv.Key));
                foreach(string value in kv.Value)
                {
                    sb.Append(string.Format("<Value>{0}</Value>", value));
                }   
                sb.Append("</In>");
            }
            XDocument xDocument = XDocument.Parse("<Condition>" + sb.ToString() + "</Condition>");

            XmlDocument xmlDocument = ToXmlDocument(xDocument);
            return xmlDocument.DocumentElement;
        }
        public static XmlDocument ToXmlDocument(XDocument xDocument)
        {
            var xmlDocument = new XmlDocument();
            using (var xmlReader = xDocument.CreateReader())
            {
                xmlDocument.Load(xmlReader);
            }
            return xmlDocument;
        }
    }
}
