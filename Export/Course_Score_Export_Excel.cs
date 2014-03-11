using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace EMBACore.Export
{
    public partial class Course_Score_Export_Excel : EMBA.Export.ExportProxyForm
    {
        public Course_Score_Export_Excel()
        {
            InitializeComponent();
            InitializeData();
            this.Load += new EventHandler(Course_SCAttend_Export_Excel_Load);
        }

        private void Course_SCAttend_Export_Excel_Load(object sender, EventArgs e)
        {
        }

        private void InitializeData()
        {
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

            this.QuerySQL = this.SetQueryString();
        }

        private string SetQueryString()
        {
            string querySQL = string.Format(@"select table_s.subject_semester_score_uid as 課程學期成績系統編號, table_c.student_id as 學生系統編號, table_c.class_name as 教學分班, table_c.student_number as 學號, table_c.name as 姓名, table_c.school_year as 學年度, table_c.semester as 學期, table_c.subject_id as 課程系統編號, table_c.ce_class_name as 班次, table_c.subject_code as 課程識別碼, table_c.new_subject_code as 課號, table_c.course_name as 開課, table_s.score as 等第成績, table_s.credit as 學分數, table_s.is_pass as 取得學分, table_s.is_required as 必選修, table_s.remark as 備註, table_s.offset_course as 抵免課程 from 
(
select school_year, semester, student.id as student_id, class.class_name, student.student_number, student.name, ce.ref_subject_id as subject_id, course.course_name, ce.class_name as ce_class_name, ce.subject_code, ce.new_subject_code from course join $ischool.emba.course_ext as ce on course.id=ce.ref_course_id
join $ischool.emba.scattend_ext as se on se.ref_course_id=course.id
join student on student.id=se.ref_student_id
left join class on class.id=student.ref_class_id
where course.id in ({0}) and ce.score_confirmed=true
) as table_c
left join 
(
select student.id as student_id, sss.ref_subject_id as subject_id, sss.uid as subject_semester_score_uid, sss.school_year, sss.semester, subject.subject_code, subject.new_subject_code, sss.score, sss.remark, sss.credit, sss.is_pass, sss.is_required, sss.offset_course from $ischool.emba.subject_semester_score as sss 
join student on student.id=sss.ref_student_id 
join $ischool.emba.subject as subject on subject.uid=sss.ref_subject_id
) as table_s on table_s.subject_id=table_c.subject_id and table_s.student_id=table_c.student_id and table_s.school_year=table_c.school_year and table_s.semester=table_c.semester
order by table_c.course_name, table_c.student_number, table_s.school_year, table_s.semester, table_s.subject_id", String.Join(",", K12.Presentation.NLDPanels.Course.SelectedSource));
            return querySQL;
        }
    }
}
