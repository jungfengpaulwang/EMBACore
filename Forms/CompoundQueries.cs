using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using FISCA.Presentation.Controls;
using FISCA.Data;

namespace EMBACore.Forms
{
    public partial class CompoundQueries : BaseForm
    {
        public CompoundQueries()
        {
            InitializeComponent();
        }

        private void Query_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(this.Student_Name.Text) && string.IsNullOrWhiteSpace(this.Student_EngName.Text) && string.IsNullOrWhiteSpace(this.EMail_List.Text) && string.IsNullOrWhiteSpace(this.Enroll_Year.Text) && string.IsNullOrWhiteSpace(this.Company_Name.Text) && string.IsNullOrWhiteSpace(this.Department_Group.Text))
                return;

            this.progress.Visible = true;
            this.progress.IsRunning = true;

            QueryHelper helper = new QueryHelper();
            DataTable dataTable = new DataTable();
            IEnumerable<string> studentIDs_Intersection = new List<string>();

            //  所有學生
            dataTable = helper.Select(string.Format("select id as student_id from student"));
            if (dataTable.Rows.Count > 0)
                studentIDs_Intersection = dataTable.Rows.Cast<DataRow>().Select(x => (x[0] + ""));

            //  以姓名查詢
            if (!string.IsNullOrWhiteSpace(this.Student_Name.Text))
            {
                dataTable = helper.Select(string.Format("select id as student_id from student where name like '%{0}%'", this.Student_Name.Text.Trim()));

                studentIDs_Intersection = studentIDs_Intersection.Intersect(dataTable.Rows.Cast<DataRow>().Select(x => (x[0] + "")));
            }

            //  以英文姓名查詢
            if (!string.IsNullOrWhiteSpace(this.Student_EngName.Text))
            {
                dataTable = helper.Select(string.Format("select id as student_id from student where english_name like '%{0}%'", this.Student_EngName.Text.Trim()));

                studentIDs_Intersection = studentIDs_Intersection.Intersect(dataTable.Rows.Cast<DataRow>().Select(x => (x[0] + "")));
            }

            //  以系所組別查詢
            if (!string.IsNullOrWhiteSpace(this.Department_Group.Text))
            {
                dataTable = helper.Select(string.Format("select sb.ref_student_id as student_id from student join $ischool.emba.student_brief2 as sb on student.id=sb.ref_student_id join $ischool.emba.department_group as dg on dg.uid=sb.ref_department_group_id where dg.name like '%{0}%'", this.Department_Group.Text.Trim()));

                studentIDs_Intersection = studentIDs_Intersection.Intersect(dataTable.Rows.Cast<DataRow>().Select(x => (x[0] + "")));
            }

            //  以入學年度查詢
            if (!string.IsNullOrWhiteSpace(this.Enroll_Year.Text))
            {
                dataTable = helper.Select(string.Format("select sb.ref_student_id as student_id from student join $ischool.emba.student_brief2 as sb on student.id=sb.ref_student_id where sb.enroll_year ='{0}'", this.Enroll_Year.Text.Trim()));

                studentIDs_Intersection = studentIDs_Intersection.Intersect(dataTable.Rows.Cast<DataRow>().Select(x => (x[0] + "")));
            }

            //  以任職公司查詢
            if (!string.IsNullOrWhiteSpace(this.Company_Name.Text))
            {
                dataTable = helper.Select(string.Format("select student.id as student_id from $ischool.emba.experience as ep join student on student.id=ep.ref_student_id where ep.company_name like '%{0}%'", this.Company_Name.Text.Trim()));
                
                studentIDs_Intersection = studentIDs_Intersection.Intersect(dataTable.Rows.Cast<DataRow>().Select(x => (x[0] + "")));
            }

            //  以電子郵件查詢
            if (!string.IsNullOrWhiteSpace(this.EMail_List.Text))
            {
                dataTable = helper.Select(string.Format(@"select sb.ref_student_id as student_id, sb.email_list from student join $ischool.emba.student_brief2 as sb on student.id=sb.ref_student_id where (xpath_string('<root>' || sb.email_list || '</root>','email1')) like '%{0}%' or (xpath_string('<root>' || sb.email_list || '</root>','email2')) like '%{0}%' or (xpath_string('<root>' || sb.email_list || '</root>','email3')) like '%{0}%' or 
(xpath_string('<root>' || sb.email_list || '</root>','email4')) like '%{0}%' or (xpath_string('<root>' || sb.email_list || '</root>','email5')) like '%{0}%'", this.EMail_List.Text.Trim()));

                studentIDs_Intersection = studentIDs_Intersection.Intersect(dataTable.Rows.Cast<DataRow>().Select(x => (x[0] + "")));
            }

            if (studentIDs_Intersection.Count() == 0)
            {
                MsgBox.Show("查無資料！");
                this.progress.IsRunning = false;
                this.progress.Visible = false;
                return;
            }

            //  清空待處理學生
            (K12.Presentation.NLDPanels.Student as FISCA.Presentation.NLDPanel).RemoveFromTemp(K12.Presentation.NLDPanels.Student.TempSource);
            //  加入查詢結果至待處理
            K12.Presentation.NLDPanels.Student.AddToTemp(studentIDs_Intersection.ToList());

            this.progress.IsRunning = false;
            this.progress.Visible = false;
            //MsgBox.Show("已清空待處理中原有資料，並將查詢結果放入待處理清單中。請切換至「待處理」！");
            this.Close();
        }
    }
}
