using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using EMBACore.DataItems;
using System.Xml;
using Campus.Configuration;

namespace EMBACore.Export
{
    public partial class SCAttend_Export_Excel : EMBA.Export.ExportProxyForm
    {
        private string exportType;
        public SCAttend_Export_Excel(string pExportType)
        {
            InitializeComponent();

            this.exportType = pExportType;

            if (exportType.ToUpper() == "COURSE")
                this.HideSemesterControls();

            this.KeyField = "學生修課系統編號";
            this.AutoSaveLog = true;
            this.AutoSaveFile = true;
            this.Text = "匯出修課記錄";
            this.Tag = this.Text;
            Dictionary<string, List<KeyValuePair<string, string>>> dicReplaceFields = new Dictionary<string, List<KeyValuePair<string, string>>>();
            List<KeyValuePair<string, string>> replaceFields = new List<KeyValuePair<string, string>>();
            replaceFields = new List<KeyValuePair<string, string>>();
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
        }

        public void HideSemesterControls()
        {
            this.lblSchoolYear.Visible = false;
            this.nudSchoolYear.Visible = false;
            this.lblSemester.Visible = false;
            this.cboSemester.Visible = false;
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
                querySQL = string.Format(@"select sc.uid as 學生修課系統編號, student.id as 學生系統編號, class.class_name as 教學分班, student.student_number as 學號, student.name as 姓名, course.id as 課程系統編號, course.course_name as 開課, course.school_year as 學年度, course.semester as 學期, ce.class_name as 班次, ce.subject_code as 課程識別碼, ce.new_subject_code as 課號, sc.report_group as 報告小組, sc.is_cancel as 停修  from student 
left join $ischool.emba.scattend_ext as sc on sc.ref_student_id=student.id 
left join course on course.id=sc.ref_course_id 
left join $ischool.emba.course_ext as ce on ce.ref_course_id=course.id 
left join class on class.id=student.ref_class_id where course.school_year='{0}' and course.semester='{1}' and student.id in ({2}) order by student.student_number", school_year, semester, String.Join(",", K12.Presentation.NLDPanels.Student.SelectedSource));
                return querySQL;
            }
            if (exportType.ToUpper() == "COURSE")
            {
                querySQL = string.Format(@"select sc.uid as 學生修課系統編號, student.id as 學生系統編號, class.class_name as 教學分班, student.student_number as 學號, student.name as 姓名, course.id as 課程系統編號, course.course_name as 開課, course.school_year as 學年度, course.semester as 學期, ce.class_name as 班次, ce.subject_code as 課程識別碼, ce.new_subject_code as 課號, sc.report_group as 報告小組, sc.is_cancel as 停修  from course
left join $ischool.emba.scattend_ext as sc on sc.ref_course_id=course.id 
left join student on student.id=sc.ref_student_id 
left join $ischool.emba.course_ext as ce on ce.ref_course_id=course.id 
left join class on class.id=student.ref_class_id where course.id in ({0}) order by course.school_year, course.semester, student.student_number", String.Join(",", K12.Presentation.NLDPanels.Course.SelectedSource));
                return querySQL;
            }
            return querySQL;
        }
    }
}
