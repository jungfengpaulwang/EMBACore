using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using EMBA.Export;

namespace EMBACore.Export
{
    public partial class Student_Experience_Export_Excel
    {
        private ExportProxyForm exportProxyForm;
        public Student_Experience_Export_Excel()
        {
        }

        public void Execute()
        {
            string querySQL = string.Format(@"select student.id as 學生系統編號, class.class_name as 教學分班, student.student_number as 學號, student.name as 姓名, ex.company_name as 公司名稱, ex.position as 職稱, ex.industry as 產業別, ex.department_category as 部門類別, ex.post_level as 層級別, ex.work_place as 工作地點, ex.work_status as 工作狀態, date_part('year', ex.work_begin_date) || '/' || date_part('month', ex.work_begin_date) || '/' || date_part('day', ex.work_begin_date) as 工作起日, date_part('year', ex.work_end_date) || '/' || date_part('month', ex.work_end_date) || '/' || date_part('day', ex.work_end_date) as 工作迄日, ex.publicist as 公關連絡人, ex.public_relations_office_telephone as 公關室電話, ex.public_relations_office_fax as 公關室傳真, 
ex.publicist_email as 公關EMAIL, ex.company_website as 公司網址, sb2.remark as 備註, case when lower(ex.action_by)='student' then '是' else ex.action_by end as 學生修改, ex.time_stamp as 最後更新日期
from student left join $ischool.emba.experience as ex on ex.ref_student_id=student.id
left join class on class.id=student.ref_class_id left join $ischool.emba.student_brief2 as sb2 on sb2.ref_student_id=student.id where student.id in ({0})", String.Join(",", K12.Presentation.NLDPanels.Student.SelectedSource));

            exportProxyForm = new ExportProxyForm(querySQL, true, true, "學生系統編號", null, null);
            //exportProxyForm.HideSemesterControls();
            exportProxyForm.Text = "匯出學生經歷資料";
            exportProxyForm.Tag = exportProxyForm.Text;

            exportProxyForm.ShowDialog();
            this.Dispose();
        }

        private void Dispose()
        {
            exportProxyForm.Dispose();
        }
    }
}
