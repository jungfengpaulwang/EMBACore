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
using System.Dynamic;

namespace EMBACore.Export
{
    public partial class Payment_History_Export_Excel
    {
        private ExportProxyForm exportProxyForm;
        public Payment_History_Export_Excel()
        {
        }

        public void Execute()
        {
            exportProxyForm = new ExportProxyForm();

            exportProxyForm.AutoSaveFile = false;
            exportProxyForm.AutoSaveLog = true;
            exportProxyForm.KeyField = "學生繳費記錄系統編號";
            exportProxyForm.InvisibleFields = new List<string>() { "學生繳費記錄系統編號" };
            exportProxyForm.ReplaceFields = null;
            exportProxyForm.QuerySQL = this.SetQueryString();
            exportProxyForm.Text = "匯出學生繳費記錄";
            //exportProxyForm.HideSemesterControls();
            exportProxyForm.FieldContainer.Enabled = false;
            exportProxyForm.chkSelectAll.Visible = false;

            DialogResult dr = exportProxyForm.ShowDialog();

            if (dr == DialogResult.Cancel)
                return;

            if (dr == DialogResult.OK)
            {
                BackgroundWorker _BGWLoadData = exportProxyForm.SalvageOperation;
                _BGWLoadData.RunWorkerCompleted += new RunWorkerCompletedEventHandler(_BGWLoadData_RunWorkerCompleted);
            }
        }

        private string t_Status(string status)
        {
            string t_Status = string.Empty;
            switch (status)
            {
                case "1":
                    t_Status = "在學";
                    break;
                case "4":
                    t_Status = "休學";
                    break;
                case "64":
                    t_Status = "退學";
                    break;
                case "16":
                    t_Status = "畢業";
                    break;
                case "256":
                    t_Status = "刪除";
                    break;
                default:
                    t_Status = "";
                    break;
            }
            return t_Status;
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
                Dictionary<string, List<dynamic>> dicPaymentHistories = new Dictionary<string, List<dynamic>>();
                Dictionary<string, KeyValuePair<string, string>> dicAllSchoolYearSemesters = new Dictionary<string, KeyValuePair<string, string>>();
                Dictionary<string, int> dicColumnMappingTable = new Dictionary<string, int>();
                foreach (DataRow row in dataTable.Rows)
                {
                    dynamic paymentHistory = new ExpandoObject();

                    paymentHistory.StudentID = (row["學生系統編號"] + "");
                    paymentHistory.ClassName = (row["教學分班"] + "");
                    paymentHistory.StudentNumber = (row["學號"] + "");
                    paymentHistory.Name = (row["姓名"] + "");
                    paymentHistory.Status = t_Status(row["學生狀態"] + "");
                    paymentHistory.SchoolYear = (row["學年度"] + "");
                    paymentHistory.Semester = (row["學期"] + "");
                    paymentHistory.IsPaied = (row["繳費標記"] + "");

                    if (!dicPaymentHistories.ContainsKey(paymentHistory.StudentID))
                        dicPaymentHistories.Add(paymentHistory.StudentID, new List<dynamic>());

                    dicPaymentHistories[paymentHistory.StudentID].Add(paymentHistory);

                    KeyValuePair<string, string> schoolYearSemester = new KeyValuePair<string, string>(paymentHistory.SchoolYear, paymentHistory.Semester);
                    string key = paymentHistory.SchoolYear + "_" + paymentHistory.Semester;
                    if (!dicAllSchoolYearSemesters.ContainsKey(key))
                        dicAllSchoolYearSemesters.Add(key, schoolYearSemester);
                }

                DataTable newTable = new DataTable("匯出學生繳費記錄");
                newTable.Columns.Add("學生系統編號");
                newTable.Columns.Add("教學分班");
                newTable.Columns.Add("學號");
                newTable.Columns.Add("姓名");
                newTable.Columns.Add("學生狀態");
                newTable.Columns.Add("繳費次數");
                if (dicAllSchoolYearSemesters.Count > 0)
                    foreach (KeyValuePair<string, string> kv in dicAllSchoolYearSemesters.Values.OrderBy(x => x.Key).ThenBy(x => x.Value))
                        newTable.Columns.Add("學年期:" + kv.Key + kv.Value);

                //  轉換 DataTable
                foreach (string key in dicPaymentHistories.Keys)
                {
                    DataRow row = newTable.NewRow();

                    row["學生系統編號"] = dicPaymentHistories[key][0].StudentID;
                    row["教學分班"] = dicPaymentHistories[key][0].ClassName;
                    row["學號"] = dicPaymentHistories[key][0].StudentNumber;
                    row["姓名"] = dicPaymentHistories[key][0].Name;
                    row["學生狀態"] = dicPaymentHistories[key][0].Status;

                    List<dynamic> singleStudentPaymentHistoried = dicPaymentHistories[key];
                    int sum = 0;
                    for (int i = 0; i < singleStudentPaymentHistoried.Count; i++)
                    {
                        dynamic student = singleStudentPaymentHistoried[i];
                        int count = 0;
                        int.TryParse(student.IsPaied.ToString(), out count);
                        sum += count;

                        //if (count == 0)
                        //    row["學年期:" + student.SchoolYear + student.Semester] = "";
                        //else if (count == 1)
                        //    row["學年期:" + student.SchoolYear + student.Semester] = "已繳";
                        //else
                            row["學年期:" + student.SchoolYear + student.Semester] = student.IsPaied;
                    }
                    row["繳費次數"] = sum;

                    newTable.Rows.Add(row);
                }

                //  匯出資料
                newTable.ToWorkbook(true).Save(true, "匯出學生繳費記錄");
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

            querySQL = string.Format(@"select p.uid as 學生繳費記錄系統編號, s.id as 學生系統編號, c.class_name as 教學分班, s.student_number as 學號, s.name as 姓名, p.school_year as 學年度, p.semester as 學期, p.is_paied as 繳費標記, s.status as 學生狀態 from $ischool.emba.payment_history as p join student as s on p.ref_student_id=s.id join class as c on c.id=s.ref_class_id where s.id in ({0}) order by c.class_name, s.student_number, p.school_year, p.semester", String.Join(",", K12.Presentation.NLDPanels.Student.SelectedSource));

            return querySQL;
        }
    }
}
