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
    public partial class Subject_Score_Export_Excel : EMBA.Export.ExportProxyForm
    {
        public Subject_Score_Export_Excel()
        {
            InitializeComponent();
            InitializeData();

            this.Load += new EventHandler(Subject_Score_Export_Excel_Load);
        }

        private void Subject_Score_Export_Excel_Load(object sender, EventArgs e)
        {

        }

        private void InitializeData()
        {
            InitSemester();

            this.AutoSaveFile = true;
            this.AutoSaveLog = true;
            this.KeyField = "課程學期成績系統編號";
            this.InvisibleFields = new List<string>() { "課程學期成績系統編號" };

            Dictionary<string, List<KeyValuePair<string, string>>> dicReplaceFields = new Dictionary<string, List<KeyValuePair<string, string>>>();
            List<KeyValuePair<string, string>> replaceFields = new List<KeyValuePair<string, string>>();
            replaceFields.Add(new KeyValuePair<string, string>("false", "選修"));
            replaceFields.Add(new KeyValuePair<string, string>("true", "必修"));
            dicReplaceFields.Add("必選修", replaceFields);

            replaceFields = new List<KeyValuePair<string, string>>();
            replaceFields.Add(new KeyValuePair<string, string>("false", "否"));
            replaceFields.Add(new KeyValuePair<string, string>("true", "是"));
            dicReplaceFields.Add("取得學分", replaceFields);

            this.ReplaceFields = dicReplaceFields;
            this.Text = "匯出學期成績";
            this.Tag = this.Text;
            this.QuerySQL = SetQueryString();
        }

        private void InitSemester()
        {
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
            string school_year = this.nudSchoolYear.Value.ToString();
            string semester = (this.cboSemester.SelectedItem == null ? "0" : (this.cboSemester.SelectedItem as SemesterItem).Value);
            string querySQL = string.Empty;
            
            if (this.chkAllSemester.Checked)
                querySQL = string.Format(@"select sss.uid as 課程學期成績系統編號, student.id as 學生系統編號, class.class_name as 教學分班, student.student_number as 學號, student.name as 姓名, sss.school_year as 學年度, sss.semester as 學期, 
sss.ref_subject_id as 課程系統編號, ce.class_name as 班次, subject.subject_code as 課程識別碼, subject.new_subject_code as 課號, sss.subject_name as 課程名稱, sss.score as 等第成績, sss.credit as 學分數, 
sss.is_required as 必選修, case sss.score when 'X' then false else sss.is_pass end as 取得學分, sss.remark as 備註, sss.offset_course as 抵免課程 from student 
join $ischool.emba.subject_semester_score as sss on student.id=sss.ref_student_id 
left join class on class.id=student.ref_class_id 
left join course on course.id = sss.ref_course_id left join $ischool.emba.course_ext as ce on ce.ref_course_id=course.id 
left join $ischool.emba.subject as subject on subject.uid = sss.ref_subject_id 
where sss.uid in 
(select sss.uid from $ischool.emba.subject_semester_score as sss join student on student.id=sss.ref_student_id where not (offset_course is null or offset_course='') and student.id in ({0})
union select sss.uid from $ischool.emba.subject_semester_score as sss join student on student.id=sss.ref_student_id join course on course.id = sss.ref_course_id 
left join $ischool.emba.course_ext as ce on ce.ref_course_id=course.id where (offset_course is null or offset_course='') and ce.score_confirmed=true and student.id in ({0})) 
and student.id in ({0}) order by class.class_name, student_number, sss.school_year, sss.semester", String.Join(",", K12.Presentation.NLDPanels.Student.SelectedSource));
            else
                querySQL = string.Format(@"select sss.uid as 課程學期成績系統編號, student.id as 學生系統編號, class.class_name as 教學分班, student.student_number as 學號, student.name as 姓名, sss.school_year as 學年度, sss.semester as 學期, 
sss.ref_subject_id as 課程系統編號, ce.class_name as 班次, subject.subject_code as 課程識別碼, subject.new_subject_code as 課號, sss.subject_name as 課程名稱, sss.score as 等第成績, sss.credit as 學分數, 
sss.is_required as 必選修, case sss.score when 'X' then false else sss.is_pass end as 取得學分, sss.remark as 備註, sss.offset_course as 抵免課程 from student 
join $ischool.emba.subject_semester_score as sss on student.id=sss.ref_student_id 
left join class on class.id=student.ref_class_id 
left join course on course.id = sss.ref_course_id left join $ischool.emba.course_ext as ce on ce.ref_course_id=course.id 
left join $ischool.emba.subject as subject on subject.uid = sss.ref_subject_id 
where sss.uid in 
(select sss.uid from $ischool.emba.subject_semester_score as sss join student on student.id=sss.ref_student_id where not (offset_course is null or offset_course='') and student.id in ({2})
union select sss.uid from $ischool.emba.subject_semester_score as sss join student on student.id=sss.ref_student_id join course on course.id = sss.ref_course_id 
left join $ischool.emba.course_ext as ce on ce.ref_course_id=course.id where (offset_course is null or offset_course='') and ce.score_confirmed=true and student.id in ({2})) 
and ((sss.school_year='{0}' and sss.semester='{1}') or (sss.school_year is null or sss.semester is null)) and student.id in ({2})
 order by class.class_name, student_number, sss.school_year, sss.semester", school_year, semester, String.Join(",", K12.Presentation.NLDPanels.Student.SelectedSource));

            return querySQL;
        }

        private void chkAllSemester_CheckedChanged(object sender, EventArgs e)
        {
            this.QuerySQL = SetQueryString();
        }
    }
}
