using System.Windows.Forms;
using FISCA.Presentation.Controls;
using System.Data;
using System.Linq;
using Aspose.Cells;
using System;
using System.Collections.Generic;
using FISCA.UDT;
using FISCA.Data;
using System.Threading.Tasks;
using K12.Data;

namespace EMBACore.Forms
{
    public partial class NotYetUploadSubjectSemesterScoreTeacherDetector : BaseForm
    {
        private AccessHelper Access;
        private QueryHelper Query;

        private Dictionary<string, ScoreInputDetector> dicScoreInputDetectors;
        private Dictionary<string, List<string>> dicScoreManagers;
        private Dictionary<string, List<string>> dicCourseTeachers;

        public NotYetUploadSubjectSemesterScoreTeacherDetector()
        {
            InitializeComponent();

            this.Access = new AccessHelper();
            this.Query = new QueryHelper();

            this.dicScoreInputDetectors = new Dictionary<string, ScoreInputDetector>();
            this.dicScoreManagers = new Dictionary<string, List<string>>();
            this.dicCourseTeachers = new Dictionary<string, List<string>>();
            
            this.circularProgress.Visible = true;
            this.circularProgress.IsRunning = true;

            this.InitSchoolYear();
            this.InitSemester();
            
            Task task = Task.Factory.StartNew(()=>
            {
                //  成績管理者
                DataTable dataTable_Teacher = (new QueryHelper()).Select(string.Format(@"select ref_course_id, ref_teacher_id, teacher_name, ref_tag_id from $ischool.emba.course_instructor as ci join teacher on teacher.id=ci.ref_teacher_id where is_scored=true"));
                foreach (DataRow row in dataTable_Teacher.Rows)
                {
                    if (!dicCourseTeachers.ContainsKey(row["ref_course_id"] + ""))
                        dicCourseTeachers.Add(row["ref_course_id"] + "", new List<string>());

                    dicCourseTeachers[row["ref_course_id"] + ""].Add(row["teacher_name"] + "");
                }

                //  授課教師
//select course.id as ref_course_id, teacher.id as ref_teacher_id, course.course_name, teacher.teacher_name from course join $ischool.emba.course_instructor as ci on course.id=ci.ref_course_id join teacher on teacher.id=ci.ref_teacher_id where teacher.id in (SELECT  distinct teacher.id FROM course LEFT JOIN $ischool.emba.course_instructor ON $ischool.emba.course_instructor.ref_course_id = course.id LEFT JOIN teacher ON teacher.id = $ischool.emba.course_instructor.ref_teacher_id LEFT JOIN tag_teacher ON tag_teacher.ref_teacher_id = teacher.id LEFT JOIN tag ON tag.id = tag_teacher.ref_tag_id WHERE tag.category = 'Teacher' AND tag.prefix = '教師') order by course.course_name, teacher.teacher_name"));

                //  課程成績
                string SQL = string.Format(@"select table_s.subject_semester_score_uid as 課程學期成績系統編號, table_c.student_id as 學生系統編號, table_c.class_name as 教學分班, table_c.student_number as 學號, table_c.name as 姓名, table_c.school_year as 學年度, table_c.semester as 學期, table_s.subject_id as 課程系統編號, table_c.ce_class_name as 班次, table_s.subject_code as 課程識別碼, table_c.new_subject_code as 課號, table_c.course_name as 開課, table_s.score as 等第成績, table_s.offset_course as 抵免課程, table_c.id as 開課系統編號, table_c.score_confirmed as 是否上傳, table_c.serial_no as 流水號 from 
(
select student.id as student_id, class.class_name, student.student_number, student.name, ce.ref_subject_id as subject_id, course.course_name, ce.class_name as ce_class_name, subject.new_subject_code, course.id, ce.score_confirmed, course.school_year, course.semester, ce.serial_no from course join $ischool.emba.course_ext as ce on course.id=ce.ref_course_id
join $ischool.emba.scattend_ext as se on se.ref_course_id=course.id
join student on student.id=se.ref_student_id
left join class on class.id=student.ref_class_id
left join $ischool.emba.subject as subject on subject.uid=ce.ref_subject_id
--where course.school_year=102 and course.semester=0
) as table_c
left join 
(
select student.id as student_id, sss.ref_subject_id as subject_id, sss.uid as subject_semester_score_uid, sss.school_year, sss.semester, sss.subject_code, sss.new_subject_code, sss.score, sss.remark, sss.credit, sss.is_pass, sss.is_required, sss.offset_course, sss.ref_course_id from $ischool.emba.subject_semester_score as sss 
join student on student.id=sss.ref_student_id 
) as table_s on table_s.ref_course_id=table_c.id and table_s.student_id=table_c.student_id
order by table_c.course_name, table_c.student_number, table_s.school_year, table_s.semester, table_s.subject_id");

                DataTable dataTable = Query.Select(SQL);

                foreach (DataRow row in dataTable.Rows)
                {
                    string course_id = row["開課系統編號"] + "";
                    bool score_confirmed = bool.Parse(row["是否上傳"] + "");
                    string score = row["等第成績"] + "";

                    if (!this.dicScoreInputDetectors.ContainsKey(course_id))
                    {
                        ScoreInputDetector scoreInputDetector = new ScoreInputDetector();

                        scoreInputDetector.CourseID = course_id;

                        int school_year = 0;
                        int semester = 0;
                        int serial_no = 0;
                        int.TryParse(row["學年度"] + "", out school_year);
                        int.TryParse(row["學期"] + "", out semester);
                        int.TryParse(row["流水號"] + "", out serial_no);

                        scoreInputDetector.SerialNo = serial_no;
                        scoreInputDetector.SchoolYear = school_year;
                        scoreInputDetector.Semester = semester;
                        scoreInputDetector.CourseName = row["開課"] + "";
                        scoreInputDetector.NewSubjectCode = row["課號"] + "";
                        scoreInputDetector.ClassName = row["班次"] + "";
                        scoreInputDetector.Progress = "未輸入";
                        scoreInputDetector.ScoreManager = new List<string>();
                        if (dicCourseTeachers.ContainsKey(course_id))
                            scoreInputDetector.ScoreManager = dicCourseTeachers[course_id];
                        
                        this.dicScoreInputDetectors.Add(course_id, scoreInputDetector);
                    }
                    if (score_confirmed)
                    {
                        this.dicScoreInputDetectors[course_id].Progress = "已確認並上傳";
                    }
                    else if (!string.IsNullOrEmpty(score))
                    {
                        this.dicScoreInputDetectors[course_id].Progress = "已暫存";
                    }
                }
            });
            task.ContinueWith((x) =>
            {
                this.DataBind();

                this.circularProgress.Visible = false;
                this.circularProgress.IsRunning = false;
            }, TaskScheduler.FromCurrentSynchronizationContext());
        }

        private void DataBind()
        {
            this.dgvData.Rows.Clear();

            string school_year = this.nudSchoolYear.Value.ToString();
            string semester = (this.cboSemester.SelectedItem == null ? "0" : (this.cboSemester.SelectedItem as EMBACore.DataItems.SemesterItem).Value);

            foreach (ScoreInputDetector detector in dicScoreInputDetectors.Values.OrderBy(x=>x.SerialNo))
            {
                if (detector.SchoolYear.ToString() != school_year || detector.Semester.ToString() != semester)
                    continue;

                List<object> sources = new List<object>();

                sources.Add(detector.SerialNo);
                sources.Add(detector.NewSubjectCode);
                sources.Add(detector.CourseName);
                sources.Add(detector.ClassName);
                sources.Add(string.Join("；", detector.ScoreManager));
                sources.Add(detector.Progress);

                int idx = this.dgvData.Rows.Add(sources.ToArray());
                this.dgvData.Rows[idx].Tag = detector;
            }
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
        }

        private void InitSemester()
        {
            this.cboSemester.DataSource = DataItems.SemesterItem.GetSemesterList();
            this.cboSemester.ValueMember = "Value";
            this.cboSemester.DisplayMember = "Name";

            this.cboSemester.SelectedValue = K12.Data.School.DefaultSemester;
        }

        private void Exit_Click(object sender, System.EventArgs e)
        {
            this.Close();
        }

        private void Export_Click(object sender, System.EventArgs e)
        {
            DataTable dataTable = new DataTable();
            this.dgvData.Columns.Cast<DataGridViewColumn>().ToList().ForEach(x => dataTable.Columns.Add(x.HeaderText));
            this.dgvData.Rows.Cast<DataGridViewRow>().ToList().ForEach(x =>
            {
                DataRow row = dataTable.NewRow();
                foreach (DataGridViewColumn Column in this.dgvData.Columns)
                {
                    string content = (x.Cells[Column.Index].Value + "").Trim();
                    row[Column.HeaderText] = content;
                }

                dataTable.Rows.Add(row);
            });

            Workbook wb = new Workbook();
            foreach (Worksheet sheet in wb.Worksheets.Cast<Worksheet>().ToList())
                wb.Worksheets.RemoveAt(sheet.Name);

            int sheet_index = wb.Worksheets.Add();
            wb.Worksheets[sheet_index].Cells.ImportDataTable(dataTable, true, "A1");

            wb.Worksheets.Cast<Worksheet>().ToList().ForEach(y => y.AutoFitColumns());
            SaveFileDialog sd = new SaveFileDialog();
            sd.Title = "另存新檔";
            sd.FileName = nudSchoolYear.Value + "-"+ this.cboSemester.SelectedValue + "-未確認並上傳成績教師.xls";
            sd.Filter = "Excel 2003 相容檔案 (*.xls)|*.xls|所有檔案 (*.*)|*.*";
            if (sd.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    wb.Save(sd.FileName, FileFormatType.Excel2003);
                    System.Diagnostics.Process.Start(sd.FileName);
                }
                catch
                {
                    MessageBox.Show("指定路徑無法存取。", "建立檔案失敗", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
            }
        }

        private void nudSchoolYear_ValueChanged(object sender, EventArgs e)
        {
            this.DataBind();
        }

        private void cboSemester_SelectedIndexChanged(object sender, EventArgs e)
        {
            this.DataBind();
        }
    }

    public class ScoreInputDetector
    {
        //  課號、開課、班次、成績管理者、成績輸入進度
        public string CourseID { get; set; }
        public string NewSubjectCode { get; set; }
        public string CourseName { get; set; }
        public string ClassName { get; set; }
        public List<string> ScoreManager { get; set; }
        public string Progress { get; set; }
        public int SchoolYear { get; set; }
        public int Semester { get; set; }
        public int SerialNo { get; set; } 
    }
}
