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
    public partial class Student_Education_Export_Excel
    {
        private ExportProxyForm exportProxyForm;
        public Student_Education_Export_Excel()
        {
        }

        public void Execute()
        {
            string querySQL = string.Format(@"select student.id as 學生系統編號, class.class_name as 教學分班, student.student_number as 學號, student.name as 姓名, eb.school_name as 學校名稱, eb.department as 系所, eb.degree as 學位
from student left join $ischool.emba.education_background as eb on eb.ref_student_id=student.id
left join class on class.id=student.ref_class_id where student.id in ({0})", String.Join(",", K12.Presentation.NLDPanels.Student.SelectedSource));

            exportProxyForm = new ExportProxyForm(querySQL, true, true, "學生系統編號", null, null);
            //exportProxyForm.HideSemesterControls();
            exportProxyForm.Text = "匯出學生學歷資料";
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
