using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using EMBA.Export;
using EMBACore.DataItems;
using System.Xml;
using Campus.Configuration;
using FISCA.Data;

namespace EMBACore.Export
{
    public partial class SCAttend_Export_DBF : ExportProxyForm
    {
        private string exportType;
        public SCAttend_Export_DBF(string pExportType)
        {
            InitializeComponent();

            this.exportType = pExportType;

            if (exportType.ToUpper() == "COURSE")
                this.HideSemesterControls();

            this.KeyField = "學生修課系統編號";
            this.AutoSaveLog = true;
            this.AutoSaveFile = false;
            this.Text = "匯出選課檔(DBF 格式)";
            this.Tag = this.Text;
            Dictionary<string, List<KeyValuePair<string, string>>> dicReplaceFields = new Dictionary<string, List<KeyValuePair<string, string>>>();
            List<KeyValuePair<string, string>> replaceFields = new List<KeyValuePair<string, string>>();
            replaceFields.Add(new KeyValuePair<string, string>("true", "S"));
            replaceFields.Add(new KeyValuePair<string, string>("false", ""));
            dicReplaceFields.Add("停修", replaceFields);
            this.ReplaceFields = dicReplaceFields;
            this.InvisibleFields = new List<string>() { "學生修課系統編號" };

            XmlDocument xmlSystemConfig;
            XmlElement elmSchoolYear;
            XmlElement elmSemester;

            foreach (SemesterItem semester in SemesterItem.GetSemesterList())
                this.cboSemester.Items.Add(semester);

            xmlSystemConfig = new XmlDocument();
            xmlSystemConfig.LoadXml(Config.App["系統設定"].PreviousData.OuterXml);
            elmSchoolYear = (XmlElement)xmlSystemConfig.DocumentElement.SelectSingleNode("DefaultSchoolYear");
            elmSemester = (XmlElement)xmlSystemConfig.DocumentElement.SelectSingleNode("DefaultSemester");

            this.nudSchoolYear.Value = int.Parse(elmSchoolYear.InnerText);
            this.cboSemester.SelectedItem = SemesterItem.GetSemesterByCode(elmSemester.InnerText);
            this.nudSchoolYear.ValueChanged += new EventHandler(nudSchoolYear_ValueChanged);
            this.cboSemester.SelectedIndexChanged += new EventHandler(cboSemester_SelectedIndexChanged);
            this.QuerySQL = SetQueryString();

            this.chkSelectAll.Visible = false;
            this.FieldContainer.Enabled = false;
            this.lblExplanation.Text = "以下欄位將被寫入DBF檔";
        }

        private void HideSemesterControls()
        {
            this.lblSchoolYear.Visible = false;
            this.nudSchoolYear.Visible = false;
            this.lblSemester.Visible = false;
            this.cboSemester.Visible = false;
        }

        protected override void OnExportButtonClick(object sender, EventArgs e)
        {
            this.SalvageOperation.RunWorkerCompleted += new RunWorkerCompletedEventHandler(_BGWLoadData_RunWorkerCompleted);
            this.SalvageOperation.RunWorkerAsync();
            this.Close();
        }

        private void _BGWLoadData_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            DataTable dataTable = e.Result as DataTable;

            if (dataTable == null || dataTable.Rows.Count == 0)
            {
                FISCA.Presentation.Controls.MsgBox.Show("無資料可匯出。", "欄位空白", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                return;
            }

            //  授課教師
            Dictionary<string, List<string>> dicCourseTeachers = new Dictionary<string, List<string>>();
            DataTable dataTable_Teacher = (new QueryHelper()).Select(string.Format(@"select course.id as ref_course_id, teacher.id as ref_teacher_id, course.course_name, teacher.teacher_name from course join $ischool.emba.course_instructor as ci on course.id=ci.ref_course_id join teacher on teacher.id=ci.ref_teacher_id where teacher.id in (SELECT  distinct teacher.id FROM course LEFT JOIN $ischool.emba.course_instructor ON $ischool.emba.course_instructor.ref_course_id = course.id LEFT JOIN teacher ON teacher.id = $ischool.emba.course_instructor.ref_teacher_id LEFT JOIN tag_teacher ON tag_teacher.ref_teacher_id = teacher.id LEFT JOIN tag ON tag.id = tag_teacher.ref_tag_id WHERE tag.category = 'Teacher' AND tag.prefix = '教師') order by course.course_name, teacher.teacher_name"));
            foreach (DataRow row in dataTable_Teacher.Rows)
            {
                if (!dicCourseTeachers.ContainsKey(row["ref_course_id"] + ""))
                    dicCourseTeachers.Add(row["ref_course_id"] + "", new List<string>());

                dicCourseTeachers[row["ref_course_id"] + ""].Add(row["ref_teacher_id"] + "");
            }

            DataTable newDataTable = dataTable.Clone();
            DataRow dRow = dataTable.NewRow();
            foreach (DataRow dataRow in dataTable.Rows)
            {
                if (!dicCourseTeachers.ContainsKey(dataRow["課程系統編號"] + ""))
                    continue;
                else if (!dicCourseTeachers[dataRow["課程系統編號"] + ""].Contains(dataRow["教師系統編號"] + ""))
                    continue;

                if (newDataTable.Rows.Count == 0)
                    newDataTable.ImportRow(dataRow);
                else
                {
                    if (dataRow["學生修課系統編號"].ToString() == dRow["學生修課系統編號"].ToString())
                    {
                        if (!string.IsNullOrWhiteSpace(dataRow["教師中文姓名"].ToString()))
                        {
                            if (!string.IsNullOrWhiteSpace(newDataTable.Rows[newDataTable.Rows.Count - 1]["教師中文姓名"].ToString()))
                                newDataTable.Rows[newDataTable.Rows.Count - 1]["教師中文姓名"] += "；" + dataRow["教師中文姓名"].ToString().Trim();
                            else
                                newDataTable.Rows[newDataTable.Rows.Count - 1]["教師中文姓名"] = dataRow["教師中文姓名"].ToString().Trim();
                        }

                        if (!string.IsNullOrWhiteSpace(dataRow["教師英文姓名"].ToString()))
                        {
                            if (!string.IsNullOrWhiteSpace(newDataTable.Rows[newDataTable.Rows.Count - 1]["教師英文姓名"].ToString()))
                                newDataTable.Rows[newDataTable.Rows.Count - 1]["教師英文姓名"] += "；" + dataRow["教師英文姓名"].ToString().Trim();
                            else
                                newDataTable.Rows[newDataTable.Rows.Count - 1]["教師英文姓名"] = dataRow["教師英文姓名"].ToString().Trim();
                        }
                    }
                    else
                    {
                        newDataTable.ImportRow(dataRow);
                    }
                }
                dRow = dataRow;
            }

            string fileName = "選課檔";
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
            dbf_Table.Fields.Add(new DBF_Field("serno", FieldType.Char, 250, 0, false));
            dbf_Table.Fields.Add(new DBF_Field("cou_code", FieldType.Char, 250, 0, false));
            dbf_Table.Fields.Add(new DBF_Field("dpt_abbr", FieldType.Char, 250, 0, false));
            dbf_Table.Fields.Add(new DBF_Field("cou_teacno", FieldType.Char, 250, 0, false));
            dbf_Table.Fields.Add(new DBF_Field("class", FieldType.Char, 250, 0, false));
            dbf_Table.Fields.Add(new DBF_Field("credit", FieldType.Char, 250, 0, false));
            dbf_Table.Fields.Add(new DBF_Field("cou_cname", FieldType.Char, 250, 0, false));
            dbf_Table.Fields.Add(new DBF_Field("cou_ename", FieldType.Char, 250, 0, false));
            dbf_Table.Fields.Add(new DBF_Field("score", FieldType.Char, 250, 0, false));
            dbf_Table.Fields.Add(new DBF_Field("score_a", FieldType.Char, 250, 0, false));
            dbf_Table.Fields.Add(new DBF_Field("tea_code", FieldType.Char, 250, 0, false));
            dbf_Table.Fields.Add(new DBF_Field("tea_cname", FieldType.Char, 250, 0, false));
            dbf_Table.Fields.Add(new DBF_Field("tea_ename", FieldType.Char, 250, 0, false));
            dbf_Table.Fields.Add(new DBF_Field("stu_cname", FieldType.Char, 250, 0, false));
            dbf_Table.Fields.Add(new DBF_Field("s_year", FieldType.Char, 250, 0, false));
            dbf_Table.Fields.Add(new DBF_Field("s_term", FieldType.Char, 250, 0, false));
            dbf_Table.Fields.Add(new DBF_Field("cls_time", FieldType.Char, 250, 0, false));
            dbf_Table.Fields.Add(new DBF_Field("seq_code", FieldType.Char, 250, 0, false));
            dbf_Table.Fields.Add(new DBF_Field("kind", FieldType.Char, 250, 0, false));
            dbf_Table.Fields.Add(new DBF_Field("cou_kind", FieldType.Char, 250, 0, false));
            dbf_Table.Fields.Add(new DBF_Field("grp", FieldType.Char, 250, 0, false));
            dbf_Table.Fields.Add(new DBF_Field("sflag", FieldType.Char, 250, 0, false));
            dbf_Table.Fields.Add(new DBF_Field("co_gmark", FieldType.Char, 250, 0, false));

            //  加入 dbf 資料
            foreach (DataRow row in newDataTable.Rows)
            {
                DBF_Row dbf_row = dbf_Table.NewRow();

                dbf_row["blank"] = ("");
                dbf_row["dpt_code"] = (row["系碼"] + "");
                dbf_row["year"] = (row["年級"] + "");
                dbf_row["reg_no"] = (row["學號"] + "");
                dbf_row["serno"] = (row["課程流水號"] + "");
                dbf_row["cou_code"] = (row["課程識別碼"] + "");

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
                dbf_row["class"] = (row["班次"] + "");
                dbf_row["credit"] = (row["學分"] + "");
                dbf_row["cou_cname"] = (row["課程中文名稱"] + "");
                dbf_row["cou_ename"] = (row["課程英文名稱"] + "");
                dbf_row["score"] = "";
                dbf_row["score_a"] = "";
                dbf_row["tea_code"] = (row["教師代碼"] + "");
                dbf_row["tea_cname"] = (row["教師中文姓名"] + "");
                dbf_row["tea_ename"] = (row["教師英文姓名"] + "");
                dbf_row["stu_cname"] = (row["學生中文姓名"] + "");
                dbf_row["s_year"] = (row["學年度"] + "");
                dbf_row["s_term"] = (row["學期"] + "");
                dbf_row["cls_time"] = (row["上課時間"] + "");
                dbf_row["seq_code"] = "";
                dbf_row["kind"] = "";
                dbf_row["cou_kind"] = "";
                dbf_row["grp"] = "";
                dbf_row["sflag"] = "";
                dbf_row["co_gmark"] = "";

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
                MessageBox.Show(message, "匯出修課記錄", MessageBoxButtons.OK, MessageBoxIcon.Information, MessageBoxDefaultButton.Button1, MessageBoxOptions.DefaultDesktopOnly);
            }
        }

        private void nudSchoolYear_ValueChanged(object sender, EventArgs e)
        {
            this.QuerySQL = SetQueryString();
        }

        private void cboSemester_SelectedIndexChanged(object sender, EventArgs e)
        {
            this.QuerySQL = SetQueryString();
        }

        private string SetQueryString()
        {
            string querySQL = string.Empty;

            if (exportType.ToUpper() == "STUDENT")
            {
                string school_year = this.nudSchoolYear.Value.ToString();
                string semester = (this.cboSemester.SelectedItem == null ? "0" : (this.cboSemester.SelectedItem as SemesterItem).Value);
                querySQL = string.Format(@"select sc.uid as 學生修課系統編號, dg.code 系碼, sb.grade_year 年級, student.student_number 學號, ce.serial_no 課程流水號, su.subject_code 課程識別碼, su.new_subject_code 課號, ce.class_name 班次 , course.credit 學分, su.name 課程中文名稱, su.eng_name 課程英文名稱, te.ntu_system_no 教師代碼, teacher.teacher_name 教師中文姓名, te.english_name 教師英文姓名, student.name 學生中文姓名, course.school_year 學年度, course.semester 學期, '' 上課時間, course.id as 課程系統編號, teacher.id as 教師系統編號 from student join $ischool.emba.scattend_ext sc on sc.ref_student_id=student.id join course on course.id=sc.ref_course_id left join $ischool.emba.student_brief2 sb on sb.ref_student_id=student.id left join $ischool.emba.department_group dg on dg.uid = sb.ref_department_group_id left join $ischool.emba.course_ext ce on ce.ref_course_id=course.id left join $ischool.emba.subject su on su.uid=ce.ref_subject_id left join $ischool.emba.course_instructor ci on ci.ref_course_id=course.id left join teacher on teacher.id=ci.ref_teacher_id
left join $ischool.emba.teacher_ext te on te.ref_teacher_id=teacher.id where course.school_year='{0}' and course.semester='{1}' and student.id in ({2}) order by sc.uid, student.student_number, su.subject_code, su.new_subject_code", this.nudSchoolYear.Value.ToString(), (this.cboSemester.SelectedItem as SemesterItem).Value, String.Join(",", K12.Presentation.NLDPanels.Student.SelectedSource));
                return querySQL;
            }
            if (exportType.ToUpper() == "COURSE")
            {
                querySQL = string.Format(@"select sc.uid as 學生修課系統編號, dg.code 系碼, sb.grade_year 年級, student.student_number 學號, ce.serial_no 課程流水號, su.subject_code 課程識別碼, su.new_subject_code 課號, ce.class_name 班次 , course.credit 學分, su.name 課程中文名稱, su.eng_name 課程英文名稱, te.ntu_system_no 教師代碼, teacher.teacher_name 教師中文姓名, te.english_name 教師英文姓名, student.name 學生中文姓名, course.school_year 學年度, course.semester 學期, '' 上課時間, course.id as 課程系統編號, teacher.id as 教師系統編號 from course join $ischool.emba.scattend_ext sc on sc.ref_course_id=course.id join student on student.id=sc.ref_student_id left join $ischool.emba.student_brief2 sb on sb.ref_student_id=student.id left join $ischool.emba.department_group dg on dg.uid = sb.ref_department_group_id left join $ischool.emba.course_ext ce on ce.ref_course_id=course.id left join $ischool.emba.subject su on su.uid=ce.ref_subject_id left join $ischool.emba.course_instructor ci on ci.ref_course_id=course.id left join teacher on teacher.id=ci.ref_teacher_id left join $ischool.emba.teacher_ext te on te.ref_teacher_id=teacher.id where course.id in ({0}) order by sc.uid, student.student_number, su.subject_code, su.new_subject_code", String.Join(",", K12.Presentation.NLDPanels.Course.SelectedSource));
                return querySQL;
            }
            return querySQL;
        }
    }
}
