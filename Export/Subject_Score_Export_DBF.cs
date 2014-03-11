using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml.Linq;
using EMBA.Export;
using EMBACore.DataItems;
using FISCA.Data;
using FISCA.UDT;

namespace EMBACore.Export
{
    public partial class Subject_Score_Export_DBF : EMBA.Export.ExportProxyForm
    {
        private string exportType;
        private BackgroundWorker _BackgroundWorker;
        private AccessHelper __Access;
        private Dictionary<string, KeyValuePair<string, string>> dicGradeScoreMappings;
        private bool form_loaded;

        public Subject_Score_Export_DBF(string pExportType)
        {
            InitializeComponent();
            base.HideMeWhenProcessStart = false;
            __Access = new AccessHelper();
            dicGradeScoreMappings = new Dictionary<string, KeyValuePair<string, string>>();
            this.exportType = pExportType;

            if (exportType.ToUpper() == "COURSE")
                this.HideSemesterControls();

            InitializeData();
            this.InitSchoolYear();
            this.InitSemester();

            this.Load += new EventHandler(Subject_Score_Export_DBF_Load);
            
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

        private void HideSemesterControls()
        {
            this.lblSchoolYear.Visible = false;
            this.nudSchoolYear.Visible = false;
            this.lblSemester.Visible = false;
            this.cboSemester.Visible = false;
        }

        private void Subject_Score_Export_DBF_Load(object sender, EventArgs e)
        {
            this.circularProgress.Visible = false;
            this.circularProgress.IsRunning = false;
            this.form_loaded = false;

            Task task = Task.Factory.StartNew(() =>
            {
                List<UDT.GradeScoreMappingTable> GradeScoreMappings = __Access.Select<UDT.GradeScoreMappingTable>();

                if (GradeScoreMappings.Count == 0)
                {
                    XDocument xDocument = XDocument.Parse(Properties.Resources.GradeScoreMappingTable, LoadOptions.None);
                    IEnumerable<XElement> xElements = xDocument.Descendants("Grade");
                    foreach (XElement xElement in xElements)
                    {
                        UDT.GradeScoreMappingTable GradeScoreMappingTable = new UDT.GradeScoreMappingTable();

                        GradeScoreMappingTable.Grade = xElement.Value;
                        GradeScoreMappingTable.GP = decimal.Parse(xElement.Attribute("GP").Value);
                        GradeScoreMappingTable.Score = decimal.Parse(xElement.Attribute("Score").Value);

                        GradeScoreMappings.Add(GradeScoreMappingTable);
                    }
                    GradeScoreMappings.SaveAll();
                }
                foreach (UDT.GradeScoreMappingTable GradeScoreMappingTable in GradeScoreMappings)
                {
                    if (!this.dicGradeScoreMappings.ContainsKey(GradeScoreMappingTable.Grade))
                        this.dicGradeScoreMappings.Add(GradeScoreMappingTable.Grade, new KeyValuePair<string, string>(GradeScoreMappingTable.GP.ToString(), GradeScoreMappingTable.Score.ToString()));
                }
            });
            task.ContinueWith((x) =>
            {
                if (x.Exception != null)
                {
                    MessageBox.Show(x.Exception.InnerException.Message);
                }
                this.circularProgress.IsRunning = false;
                this.circularProgress.Visible = false;
                this.form_loaded = true;
            }, System.Threading.CancellationToken.None, TaskContinuationOptions.None, TaskScheduler.FromCurrentSynchronizationContext());
        }

        private void InitializeData()
        {
            this.AutoSaveFile = false;
            this.AutoSaveLog = true;
            this.KeyField = "課程學期成績系統編號";
            this.InvisibleFields = new List<string>() { "課程學期成績系統編號" };
            this.ReplaceFields = null;
            this.QuerySQL = SetQueryString();
            this.Text = "匯出成績檔(DBF 格式)";
            this.lblExplanation.Text = "以下欄位將被寫入DBF檔";
            this.chkSelectAll.Visible = false;
            this.FieldContainer.Enabled = false;
        }

        protected override void OnExportButtonClick(object sender, EventArgs e)
        {
            _BackgroundWorker = new BackgroundWorker();
            _BackgroundWorker.RunWorkerCompleted += new RunWorkerCompletedEventHandler(SalvageOperation_RunWorkerCompleted);
            _BackgroundWorker.DoWork += new DoWorkEventHandler(SalvageOperation_DoWork);
            string school_year = this.nudSchoolYear.Value.ToString();
            string semester = (this.cboSemester.SelectedItem == null ? "0" : (this.cboSemester.SelectedItem as SemesterItem).Value);
            _BackgroundWorker.RunWorkerAsync(new object[] { school_year, semester });
            this.btnExport.Enabled = false;
            this.circularProgress.Visible = true;
            this.circularProgress.IsRunning = true;
            //this.Close();
        }

        private void SalvageOperation_DoWork(object sender, DoWorkEventArgs e)
        {            
            string school_year = ((object[])e.Argument)[0].ToString();
            string semester = ((object[])e.Argument)[1].ToString();

            string strSQL = string.Empty;
            QueryHelper queryHelper = new QueryHelper();
            DataTable dataTable;

            DataSet dataSet = new DataSet();

            //  課程學期成績  
            if (this.exportType.ToUpper() == "STUDENT")
                strSQL = string.Format(@"select sss.uid, sss.ref_student_id, sss.ref_course_id, sss.school_year, sss.semester, sss.score, sss.ref_subject_id, sss.subject_name, subject.subject_code, subject.new_subject_code, sss.credit, sss.is_required, 
case sss.score when 'X' then false else sss.is_pass end as is_pass, sss.offset_course, sss.remark from student 
join $ischool.emba.subject_semester_score as sss on student.id=sss.ref_student_id 
left join class on class.id=student.ref_class_id 
left join course on course.id = sss.ref_course_id left join $ischool.emba.course_ext as ce on ce.ref_course_id=course.id 
left join $ischool.emba.subject as subject on subject.uid = sss.ref_subject_id 
where sss.uid in 
(select sss.uid from $ischool.emba.subject_semester_score as sss join student on student.id=sss.ref_student_id where not (offset_course is null or offset_course='') and student.id in ({0})
union select sss.uid from $ischool.emba.subject_semester_score as sss join student on student.id=sss.ref_student_id join course on course.id = sss.ref_course_id 
left join $ischool.emba.course_ext as ce on ce.ref_course_id=course.id where (offset_course is null or offset_course='') and ce.score_confirmed=true and student.id in ({0})) 
and student.id in ({0}) and ((sss.school_year=({1}) and sss.semester=({2})));", String.Join(",", K12.Presentation.NLDPanels.Student.SelectedSource), school_year, semester);

            if (this.exportType.ToUpper() == "COURSE")
                strSQL = string.Format(@"select sss.uid, sss.ref_student_id, sss.ref_course_id, sss.school_year, sss.semester, sss.score, sss.ref_subject_id, sss.subject_name, subject.subject_code, subject.new_subject_code, sss.credit, sss.is_required, sss.is_pass, sss.offset_course, sss.remark from $ischool.emba.subject_semester_score as sss join $ischool.emba.subject as subject on  subject.uid=sss.ref_subject_id where sss.ref_course_id in ({0});", String.Join(",", K12.Presentation.NLDPanels.Course.SelectedSource));

            dataTable = queryHelper.Select(strSQL);
            dataTable.TableName = "課程學期成績";
            dataSet.Tables.Add(dataTable);


            //  開課延伸資料
            if (this.exportType.ToUpper() == "STUDENT")
                strSQL = string.Format(@"select uid, ref_course_id, subject_code, new_subject_code, course_type, is_required, ref_subject_id, serial_no, class_name, score_confirmed from $ischool.emba.course_ext;");

            if (this.exportType.ToUpper() == "COURSE")
                strSQL = string.Format(@"select uid, ref_course_id, subject_code, new_subject_code, course_type, is_required, ref_subject_id, serial_no, class_name, score_confirmed from $ischool.emba.course_ext where ref_course_id in ({0});", String.Join(",", K12.Presentation.NLDPanels.Course.SelectedSource));

            dataTable = queryHelper.Select(strSQL);
            dataTable.TableName = "開課延伸資料";
            dataSet.Tables.Add(dataTable);


            //  學生基本資料
            if (this.exportType.ToUpper() == "STUDENT")
                strSQL = string.Format(@"select id, name, english_name, id_number, student_number, status, gender from student where id in ({0});", String.Join(",", K12.Presentation.NLDPanels.Student.SelectedSource));

            if (this.exportType.ToUpper() == "COURSE")
                strSQL = string.Format(@"select id, name, english_name, id_number, student_number, status, gender from student;");

            dataTable = queryHelper.Select(strSQL);
            dataTable.TableName = "學生基本資料";
            dataSet.Tables.Add(dataTable);

           
            //  學生延伸資料
            if (this.exportType.ToUpper() == "STUDENT")
                strSQL = string.Format(@"select uid, ref_student_id, enroll_year, ref_department_group_id, department_group_code, grade_year from $ischool.emba.student_brief2 where ref_student_id in ({0});", String.Join(",", K12.Presentation.NLDPanels.Student.SelectedSource));

            if (this.exportType.ToUpper() == "COURSE")
                strSQL = string.Format(@"select uid, ref_student_id, enroll_year, ref_department_group_id, department_group_code, grade_year from $ischool.emba.student_brief2;");

            dataTable = queryHelper.Select(strSQL);
            dataTable.TableName = "學生延伸資料";
            dataSet.Tables.Add(dataTable);


            //  教師基本資料
            dataTable = queryHelper.Select(@"select id, teacher_name, gender, id_number, status from teacher;");
            dataTable.TableName = "教師基本資料";
            dataSet.Tables.Add(dataTable);
            
            //  授課教師
            dataTable = queryHelper.Select(string.Format(@"select course.id as ref_course_id, teacher.id as ref_teacher_id, course.course_name, teacher.teacher_name from course join $ischool.emba.course_instructor as ci on course.id=ci.ref_course_id join teacher on teacher.id=ci.ref_teacher_id where teacher.id in (SELECT  distinct teacher.id FROM course LEFT JOIN $ischool.emba.course_instructor ON $ischool.emba.course_instructor.ref_course_id = course.id LEFT JOIN teacher ON teacher.id = $ischool.emba.course_instructor.ref_teacher_id LEFT JOIN tag_teacher ON tag_teacher.ref_teacher_id = teacher.id LEFT JOIN tag ON tag.id = tag_teacher.ref_tag_id WHERE tag.category = 'Teacher' AND tag.prefix = '教師') order by course.course_name, teacher.teacher_name"));

            dataTable.TableName = "授課教師";
            dataSet.Tables.Add(dataTable);

            //  課程之停修學生
            dataTable = queryHelper.Select(string.Format(@"select ref_student_id, ref_course_id from $ischool.emba.scattend_ext as se where is_cancel = true"));

            dataTable.TableName = "停修學生";
            dataSet.Tables.Add(dataTable);

            e.Result = dataSet;
        }

        private string GetInstructorName(DataRow dataRow_Score, DataSet dataSet)
        {
            DataTable dataTable_Teacher = dataSet.Tables["教師基本資料"];
            DataTable dataTable_Instructor = dataSet.Tables["授課教師"];

            if (dataTable_Teacher.Rows.Count == 0 || dataTable_Instructor.Rows.Count == 0)
                return string.Empty;

            IEnumerable<DataRow> dataRow_Instructor = dataTable_Instructor.Rows.Cast<DataRow>().Where(x => ((x["ref_course_id"] + "") == dataRow_Score["ref_course_id"] + ""));

            if (dataRow_Instructor.Count() == 0)
                return string.Empty;

            string strTeacherName = string.Empty;
            IEnumerable<DataRow> dataRow_Teacher = dataTable_Teacher.Rows.Cast<DataRow>();
            foreach(DataRow dataRow in dataRow_Instructor)
            {
                IEnumerable<DataRow> queryRow = dataRow_Teacher.Where(x=>((x["id"] + "") == (dataRow["ref_teacher_id"] + "")));

                if (queryRow.Count() > 0)
                    strTeacherName += (queryRow.ElementAt(0)["teacher_name"] + "") + "；";
            }
            if (strTeacherName.EndsWith("；"))
                strTeacherName = strTeacherName.Remove(strTeacherName.Length - 1);

            return strTeacherName;
        }

        private string GetStudentName(DataRow dataRow_Score, DataSet dataSet)
        {
            DataTable dataTable_Student = dataSet.Tables["學生基本資料"];

            if (dataTable_Student.Rows.Count == 0)
                return string.Empty;

            IEnumerable<DataRow> dataRow_Student = dataTable_Student.Rows.Cast<DataRow>().Where(x => ((x["id"] + "") == dataRow_Score["ref_student_id"] + ""));

            if (dataRow_Student.Count() > 0)
                return dataRow_Student.ElementAt(0)["name"] + "";
            else
                return string.Empty;
        }

        private string GetDeptCode(DataRow dataRow_Score, DataSet dataSet)
        {
            DataTable dataTable_Student = dataSet.Tables["學生延伸資料"];

            if (dataTable_Student.Rows.Count == 0)
                return string.Empty;

            IEnumerable<DataRow> dataRow_Student = dataTable_Student.Rows.Cast<DataRow>().Where(x => ((x["ref_student_id"] + "") == dataRow_Score["ref_student_id"] + ""));

            if (dataRow_Student.Count() > 0)
                return dataRow_Student.ElementAt(0)["department_group_code"] + "";
            else
                return string.Empty;
        }

        private string GetGradeYear(DataRow dataRow_Score, DataSet dataSet)
        {
            DataTable dataTable_Student = dataSet.Tables["學生延伸資料"];

            if (dataTable_Student.Rows.Count == 0)
                return string.Empty;

            IEnumerable<DataRow> dataRow_Student = dataTable_Student.Rows.Cast<DataRow>().Where(x => ((x["ref_student_id"] + "") == dataRow_Score["ref_student_id"] + ""));

            if (dataRow_Student.Count() > 0)
                return dataRow_Student.ElementAt(0)["grade_year"] + "";
            else
                return string.Empty;
        }

        private string GetStudentNumber(DataRow dataRow_Score, DataSet dataSet)
        {
            DataTable dataTable_Student = dataSet.Tables["學生基本資料"];

            if (dataTable_Student.Rows.Count == 0)
                return string.Empty;

            IEnumerable<DataRow> dataRow_Student = dataTable_Student.Rows.Cast<DataRow>().Where(x => ((x["id"] + "") == dataRow_Score["ref_student_id"] + ""));

            if (dataRow_Student.Count() > 0)
                return dataRow_Student.ElementAt(0)["student_number"] + "";
            else
                return string.Empty;
        }

        //  開課之班次，非「教學分班」
        private string GetClassName(DataRow dataRow_Score, DataSet dataSet)
        {
            DataTable dataTable_Course = dataSet.Tables["開課延伸資料"];

            if (dataTable_Course.Rows.Count == 0)
                return string.Empty;

            IEnumerable<DataRow> dataRow_Course = dataTable_Course.Rows.Cast<DataRow>().Where(x => ((x["ref_course_id"] + "") == dataRow_Score["ref_course_id"] + ""));

            if (dataRow_Course.Count() > 0)
                return dataRow_Course.ElementAt(0)["class_name"] + "";
            else
                return string.Empty;
        }

        private void SalvageOperation_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            DataSet dataSet = e.Result as DataSet;

            if (dataSet == null || dataSet.Tables.Count == 0 || !dataSet.Tables.Contains("課程學期成績") || dataSet.Tables["課程學期成績"].Rows.Count == 0)
            {
                FISCA.Presentation.Controls.MsgBox.Show("無資料可匯出。", "欄位空白", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                this.btnExport.Enabled = true;
                this.circularProgress.Visible = false;
                this.circularProgress.IsRunning = false;
                return;
            }

            //  加入 dbf 資料			
            //系碼	年級	學號	課程識別碼	班次	學分	課程中文名稱	教師中文姓名	學生中文姓名	課號(前6碼)	課號(後4碼)	成績	成績代碼 'S' 為停修	學期	學年度	等第成績	中位數成績	等第績分	百分制成績
            Dictionary<string, bool> dicCancels = new Dictionary<string, bool>();
            foreach (DataRow dataRow in dataSet.Tables["停修學生"].Rows)
            {
                if (!dicCancels.ContainsKey(dataRow["ref_course_id"] + "-" + dataRow["ref_student_id"]))
                    dicCancels.Add(dataRow["ref_course_id"] + "-" + dataRow["ref_student_id"], true);
            }
            DataTable dataTable = (new QueryHelper()).Select(this.QuerySQL);
            foreach (DataRow dataRow in dataSet.Tables["課程學期成績"].Rows)
            {
                DataRow row = dataTable.NewRow();

                string grade = dataRow["score"] + "";
                KeyValuePair<string, string> kv = new KeyValuePair<string, string>();
                if (dicGradeScoreMappings.ContainsKey(grade))
                    kv = dicGradeScoreMappings[grade];

                row["課程學期成績系統編號"] = dataRow["uid"] + "";
                row["系碼"] = GetDeptCode(dataRow, dataSet);
                row["年級"] = GetGradeYear(dataRow, dataSet);
                row["學號"] = GetStudentNumber(dataRow, dataSet);
                row["課程識別碼"] = dataRow["subject_code"] + "";
                row["班次"] = GetClassName(dataRow, dataSet);
                row["學分"] = dataRow["credit"] + "";
                row["課程中文名稱"] = dataRow["subject_name"] + "";
                row["教師中文姓名"] = GetInstructorName(dataRow, dataSet);
                row["學生中文姓名"] = GetStudentName(dataRow, dataSet);
                row["課號"] = dataRow["new_subject_code"] + "";
                row["成績"] = kv.Value;
                row["學期"] = dataRow["semester"] + "";
                row["學年度"] = dataRow["school_year"] + "";
                row["等第成績"] = grade;
                row["中位數成績"] = kv.Value;
                row["等第績分"] = kv.Key;
                row["百分制成績"] =kv.Value;

                if (dicCancels.ContainsKey(dataRow["ref_course_id"] + "-" + dataRow["ref_student_id"]))
                    row["停修"] = "S";
                else
                    row["停修"] = string.Empty;

                dataTable.Rows.Add(row);
            }

            string fileName = "成績檔";
            string filePath = string.Empty;
            string message = string.Empty;

            System.Windows.Forms.FolderBrowserDialog folder = new FolderBrowserDialog();
            do
            {
                DialogResult dr = folder.ShowDialog();
                if (dr == DialogResult.OK)
                    filePath = folder.SelectedPath;
                if (dr == DialogResult.Cancel)
                    return;
            } while (!System.IO.Directory.Exists(filePath));

            DBF_Table dbf_Table = new DBF_Table();

            //  加入 dbf 欄位  
            dbf_Table.Fields.Add(new DBF_Field("blank", FieldType.Char, 250, 0, false));
            dbf_Table.Fields.Add(new DBF_Field("dpt_code", FieldType.Char, 250, 0, false));
            dbf_Table.Fields.Add(new DBF_Field("year", FieldType.Char, 250, 0, false));
            dbf_Table.Fields.Add(new DBF_Field("reg_no", FieldType.Char, 250, 0, false));
            dbf_Table.Fields.Add(new DBF_Field("cou_code", FieldType.Char, 250, 0, false));
            dbf_Table.Fields.Add(new DBF_Field("class", FieldType.Char, 250, 0, false));
            dbf_Table.Fields.Add(new DBF_Field("credit", FieldType.Char, 250, 0, false));
            dbf_Table.Fields.Add(new DBF_Field("cou_cname", FieldType.Char, 250, 0, false));
            dbf_Table.Fields.Add(new DBF_Field("tea_cname", FieldType.Char, 250, 0, false));
            dbf_Table.Fields.Add(new DBF_Field("stu_cname", FieldType.Char, 250, 0, false));
            dbf_Table.Fields.Add(new DBF_Field("dpt_abbr", FieldType.Char, 250, 0, false));
            dbf_Table.Fields.Add(new DBF_Field("cou_teacno", FieldType.Char, 250, 0, false));
            dbf_Table.Fields.Add(new DBF_Field("score", FieldType.Char, 250, 0, false));
            dbf_Table.Fields.Add(new DBF_Field("score_a", FieldType.Char, 250, 0, false));
            dbf_Table.Fields.Add(new DBF_Field("s_term", FieldType.Char, 250, 0, false));
            dbf_Table.Fields.Add(new DBF_Field("s_year", FieldType.Char, 250, 0, false));
            dbf_Table.Fields.Add(new DBF_Field("grade", FieldType.Char, 250, 0, false));
            dbf_Table.Fields.Add(new DBF_Field("score_mid", FieldType.Char, 250, 0, false));
            dbf_Table.Fields.Add(new DBF_Field("score_gp", FieldType.Char, 250, 0, false));
            dbf_Table.Fields.Add(new DBF_Field("score_ref", FieldType.Char, 250, 0, false));

            foreach (DataRow row in dataTable.Rows)
            {
                if (string.IsNullOrEmpty(row["課程學期成績系統編號"] + ""))
                    continue;

                DBF_Row dbf_row = dbf_Table.NewRow();

                dbf_row["blank"] = ("");
                dbf_row["dpt_code"] = (row["系碼"] + "");
                dbf_row["year"] = (row["年級"] + "");
                dbf_row["reg_no"] = (row["學號"] + "");
                dbf_row["cou_code"] = (row["課程識別碼"] + "");
                dbf_row["class"] = (row["班次"] + "");
                dbf_row["credit"] = (row["學分"] + "");
                dbf_row["cou_cname"] = (row["課程中文名稱"] + "");
                dbf_row["tea_cname"] = (row["教師中文姓名"] + "");
                dbf_row["stu_cname"] = (row["學生中文姓名"] + "");

                //  解析課號(6+4)
                string s6 = string.Empty;
                string s4 = string.Empty;
                string s = (row["課號"] + "");
                if (s.Length >= 4)
                    s4 = s.Substring(s.Length - 4, 4);
                else
                    s4 = s;

                s6 = s.Substring(0, s.Length - s4.Length);

                dbf_row["dpt_abbr"] = s6;
                dbf_row["cou_teacno"] = s4;
                dbf_row["score"] = row["成績"] + "";
                dbf_row["score_a"] = (row["停修"] + "");
                dbf_row["s_term"] = (row["學期"] + "");
                dbf_row["s_year"] = (row["學年度"] + "");
                dbf_row["grade"] = (row["等第成績"] + "");
                dbf_row["score_mid"] = row["中位數成績"] + "";
                dbf_row["score_gp"] = row["等第績分"] + "";
                dbf_row["score_ref"] = row["百分制成績"] + "";

                dbf_Table.Rows.Add(dbf_row);
            }
            int i = 0;
            do
            {
                if (!System.IO.File.Exists(System.IO.Path.Combine(filePath, fileName + ".dbf")))
                    break;

                fileName = fileName.Replace(i.ToString(), "") + (++i).ToString();
            } while (true);

            bool result = dbf_Table.ToDBF(System.IO.Path.Combine(filePath, fileName), false);
            if (result)
            {
                message = "已產生檔案：" + fileName + ".dbf\n";

                System.Diagnostics.Process.Start(filePath);
                MessageBox.Show(message, "匯出成績檔(DBF 格式)", MessageBoxButtons.OK, MessageBoxIcon.Information, MessageBoxDefaultButton.Button1, MessageBoxOptions.DefaultDesktopOnly);
            }
            this.Close();
        }

        private void nudSchoolYear_ValueChanged(object sender, EventArgs e)
        {
            this.QuerySQL = this.SetQueryString();
        }

        private void cboSemester_SelectedIndexChanged(object sender, EventArgs e)
        {
            this.QuerySQL = this.SetQueryString();
        }

        //  使用資料比對的方式較查詢精準，此查詢僅提供欄位名稱之顯示，資料比對的程式在「btnExport.Click」
        private string SetQueryString()
        {
            string querySQL = @"select '' as 課程學期成績系統編號, '' as 系碼, '' as 年級, '' as 學號, '' as 課程識別碼, '' as 班次, '' as 學分, '' as 課程中文名稱, '' as 教師中文姓名, '' as 學生中文姓名, '' as 課號, '' as 成績, '' as 學期, '' as 學年度, '' as 等第成績, '' as 中位數成績, '' as 等第績分, '' as 百分制成績, '' as 停修";

            return querySQL;
        }

        private void RefreshScoreDegreeMappingTable()
        {

        }

        private void lnkScoreDegreeMapping_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            ScoreDegreeMapping frm = new ScoreDegreeMapping();
            if (frm.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                this.RefreshScoreDegreeMappingTable();
        }
    }
}
