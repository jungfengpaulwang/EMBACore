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
    public partial class Subject_Export_Excel
    {
        private ExportProxyForm exportProxyForm;
        public Subject_Export_Excel()
        {
        }

        public void Execute()
        {
            string querySQL = string.Format(@"select uid as 課程系統編號, name as 課程中文名稱, eng_name as 課程英文名稱, subject_code as 課程識別碼, dept_name as 開課系所, dept_code as 系所代碼, credit as 學分數, description as 內容簡介, web_url as 網頁連結, is_required as 必選修, remark as 備註, new_subject_code as 課號 from $ischool.emba.subject");

            Dictionary<string, List<KeyValuePair<string, string>>> dicReplaceFields = new Dictionary<string, List<KeyValuePair<string, string>>>();
            List<KeyValuePair<string, string>> replaceFields = new List<KeyValuePair<string, string>>();
            replaceFields.Add(new KeyValuePair<string, string>("false", "選修"));
            replaceFields.Add(new KeyValuePair<string, string>("true", "必修"));
            dicReplaceFields.Add("必選修", replaceFields);

            exportProxyForm = new ExportProxyForm(querySQL, true, true, "課程系統編號", null, dicReplaceFields);
            //exportProxyForm.HideSemesterControls();
            exportProxyForm.Text = "匯出課程總檔";
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
