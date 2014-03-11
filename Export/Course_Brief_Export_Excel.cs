using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using EMBA.Export;
using FISCA.LogAgent;

namespace EMBACore.Export
{
    public partial class Course_Brief_Export_Excel
    {
        private ExportProxyForm exportProxyForm;
        public Course_Brief_Export_Excel()
        {
        }

        public void Execute()
        {
            string querySQL = string.Format(@"select course.id as 課程系統編號, course.course_name as 開課, course.school_year as 學年度, course.semester as 學期, ce.class_name as 開課班次, ce.course_type as 類別, ce.subject_code as 課程識別碼, ce.new_subject_code as 課號, course.credit as 學分數, ce.is_required as 必選修, ce.serial_no as 流水號, ce.capacity as 人數上限 from course
left join $ischool.emba.course_ext as ce on ce.ref_course_id=course.id where course.id in ({0})", String.Join(",", K12.Presentation.NLDPanels.Course.SelectedSource));

            Dictionary<string, List<KeyValuePair<string, string>>> dicReplaceFields = new Dictionary<string,List<KeyValuePair<string,string>>>();
            List<KeyValuePair<string,string>> replaceFields = new List<KeyValuePair<string,string>>();
            replaceFields.Add(new KeyValuePair<string,string>("FALSE", "選修"));
            replaceFields.Add(new KeyValuePair<string,string>("TRUE", "必修"));
            dicReplaceFields.Add("必選修", replaceFields);
            exportProxyForm = new ExportProxyForm(querySQL, true, true, "課程系統編號", null, dicReplaceFields);
            //exportProxyForm.HideSemesterControls();
            exportProxyForm.Text = "匯出課程基本資料";
            exportProxyForm.Tag = exportProxyForm.Text;

            exportProxyForm.ShowDialog();
        }

        private void Dispose()
        {
            exportProxyForm.Dispose();
        }
    }
}
