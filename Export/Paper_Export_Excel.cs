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
using System.Xml.Linq;

namespace EMBACore.Export
{
    public partial class Paper_Export_Excel
    {
        private ExportProxyForm exportProxyForm;
        public Paper_Export_Excel()
        {
        }

        public void Execute()
        {
            exportProxyForm = new ExportProxyForm();

            exportProxyForm.AutoSaveFile = false;
            exportProxyForm.AutoSaveLog = true;
            exportProxyForm.KeyField = "論文及指導教授系統編號";
            exportProxyForm.InvisibleFields = new List<string>() { "論文及指導教授系統編號" };
            exportProxyForm.ReplaceFields = null;
            exportProxyForm.QuerySQL = this.SetQueryString();
            exportProxyForm.Text = "匯出論文及指導教授";
            //exportProxyForm.HideSemesterControls();

            DialogResult dr = exportProxyForm.ShowDialog();

            if (dr == DialogResult.Cancel)
                return;

            if (dr == DialogResult.OK)
            {
                BackgroundWorker _BGWLoadData = exportProxyForm.SalvageOperation;
                _BGWLoadData.RunWorkerCompleted += new RunWorkerCompletedEventHandler(_BGWLoadData_RunWorkerCompleted);
            }
        }

        private void _BGWLoadData_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            try
            {
                DataTable dataTable = e.Result as DataTable;

                if (dataTable == null || dataTable.Rows.Count == 0)
                {
                    FISCA.Presentation.Controls.MsgBox.Show("無資料可匯出。", "欄位空白", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    return;
                }
                if (this.exportProxyForm.SelectedFields.Contains("指導教授"))
                {
                    dataTable.Columns.Add("指導教授1");
                    dataTable.Columns.Add("指導教授2");
                    dataTable.Columns.Add("指導教授3");
                    this.exportProxyForm.SelectedFields.Add("指導教授1");
                    this.exportProxyForm.SelectedFields.Add("指導教授2");
                    this.exportProxyForm.SelectedFields.Add("指導教授3");
                    foreach (DataRow row in dataTable.Rows)
                    {
                        XElement xElement = XElement.Parse("<root>" + row["指導教授"] + "</root>");

                        string teacherNames = string.Empty;

                        int i = 0;
                        if (xElement.Descendants("Advisor") != null)
                        {
                            xElement.Descendants("Advisor").ToList().ForEach((x) =>
                            {
                                //<Advisor TeacherID=''>
                                //    <Name>黃崇興</Name>
                                //</Advisor>
                                row["指導教授" + (++i).ToString()] = x.Descendants("Name").ElementAt(0).Value;
                            });
                        }
                    }
                    dataTable.Columns.Remove("指導教授");
                }
                if (this.exportProxyForm.SelectedFields.Contains("是否公開紙本論文"))
                    dataTable.Rows.Cast<DataRow>().ToList().ForEach((x) => x["是否公開紙本論文"] = (((x["是否公開紙本論文"] + "").ToUpper() == "FALSE") ? "否" : "是"));
                //  匯出資料
                dataTable.ToWorkbook(true, this.exportProxyForm.SelectedFields).Save(true, "匯出論文及指導教授");
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                return;
            }
            this.Dispose();
        }

        private void Dispose()
        {
            exportProxyForm.Dispose();
        }

        private string SetQueryString()
        {
            string querySQL = string.Empty;

            querySQL = string.Format(@"select p.uid as 論文及指導教授系統編號, s.id as 學生系統編號, c.class_name as 教學分班, s.student_number as 學號, s.name as 姓名, dg.name as 系所組別, p.paper_name as 論文題目, p.school_year as 畢業學年度, p.semester as 畢業學期, p.is_public as 是否公開紙本論文, p.published_date as 延後公開期限, p.description as 書籍狀況, p.advisor_list as 指導教授 from $ischool.emba.paper as p join student as s on s.id=p.ref_student_id left join class c on c.id=s.ref_class_id left join $ischool.emba.student_brief2 as sb2 on sb2.ref_student_id=s.id left join $ischool.emba.department_group as dg on dg.uid=sb2.ref_department_group_id where s.id in ({0})", String.Join(",", K12.Presentation.NLDPanels.Student.SelectedSource));

            return querySQL;
        }
    }
}
