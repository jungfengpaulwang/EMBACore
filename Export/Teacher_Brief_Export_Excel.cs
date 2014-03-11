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
    public partial class Teacher_Brief_Export_Excel
    {
        private ExportProxyForm exportProxyForm;
        public Teacher_Brief_Export_Excel()
        {
        }

        public void Execute()
        {
            string querySQL = string.Format(@"select t.id 教師系統編號, te.employee_no 人事編號, te.ntu_system_no 教師編號, t.teacher_name 姓名, te.english_name 英文姓名, t.nickname 暱稱, t.id_number 身分證號, t.gender 性別, te.birthday 生日, t.st_login_name 登入帳號, t.email 電子信箱, t.contact_phone 聯絡電話, te.mobil 手機, te.phone 電話, te.other_phone 研究室電話, te.research 研究室, te.address 戶籍地址, te.major_work_place 所屬單位, te.website_url 個人網址, te.memo 備註, t.status 教師狀態 from teacher t left join $ischool.emba.teacher_ext te on t.id=te.ref_teacher_id
 where t.id in ({0})", String.Join(",", K12.Presentation.NLDPanels.Teacher.SelectedSource));

            Dictionary<string, List<KeyValuePair<string, string>>> dicReplaceFields = new Dictionary<string, List<KeyValuePair<string, string>>>();
            List<KeyValuePair<string, string>> replaceFields = new List<KeyValuePair<string, string>>();
            replaceFields.Add(new KeyValuePair<string, string>("1", "一般"));
            replaceFields.Add(new KeyValuePair<string, string>("256", "刪除"));
            dicReplaceFields.Add("教師狀態", replaceFields);

            exportProxyForm = new ExportProxyForm(querySQL, true, true, "教師系統編號", null, dicReplaceFields);
            //exportProxyForm.HideSemesterControls();
            exportProxyForm.Text = "匯出教師基本資料";
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
