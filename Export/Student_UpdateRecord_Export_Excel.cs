using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.ComponentModel;
using Aspose.Cells;
using System.Data;
using System.Threading.Tasks;
using FISCA.Data;
using System.IO;
using CourseCalendar.DataItems;
using ReportHelper;
using System.Xml.Linq;
using System.Dynamic;

namespace EMBACore.Export
{
    class Student_UpdateRecord_Export_Excel
    {
        private IEnumerable<string> StudentIDs;
        private BackgroundWorker BGW;
        private QueryHelper Query;

        public Student_UpdateRecord_Export_Excel(IEnumerable<string> StudentIDs)
        {
            this.StudentIDs = StudentIDs;
            this.Query = new QueryHelper();
            this.BGW = new BackgroundWorker();
            this.BGW.DoWork += new DoWorkEventHandler(BGW_DoWork);
            this.BGW.RunWorkerCompleted += new RunWorkerCompletedEventHandler(BGW_RunWorkerCompleted);
        }

        public void Execute()
        {
            if (this.StudentIDs.Count() == 0)
            {
                MessageBox.Show("請先選取學生。", "錯誤", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            List<string> Fields = new List<string>() { "學生系統編號", "年級", "入學年度", "系所組別", "教學分班", "學號", "姓名", "是否在校", "異動類別", "最後異動學年度學期", "畢業學年度", "畢業學期", "轉系學年度學期", "轉系前原系所", "入學前畢業學校", "國籍", "退學原因代碼", "身分別代碼", "是否延畢", "休學一", "休學二", "休學三", "休學四", "休學五", "休學六", "休學七", "休學八", "學生狀態" };
            Agent.Viewer.SelectField SelectFieldForm = Agent.Export.GetSelectFieldForm();
            SelectFieldForm.SetFields(Fields).SetTitleText("匯出異動記錄").SetSelectAllState(true).SetConfirmButtonText("匯出").SetExitButtonText("離開");
            if (SelectFieldForm.ShowDialog() != DialogResult.OK)
                return;
            List<string> SelectedFields = SelectFieldForm.GetSelectedFields();
            System.Windows.Forms.Form form = Agent.Export.GetProgressForm();
            if (SelectedFields.Count > 0)
            {
                this.BGW.RunWorkerAsync(new object[] { SelectedFields, form, StudentIDs });
                form.ShowDialog();
            }
            else
            {
                MessageBox.Show("未勾選欄位。", "錯誤", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
        }

        private string GetQueryString(IEnumerable<string> StudentIDs)
        {
            string querySQL = string.Format(@"select ref_student_id as 學生系統編號, sb.grade_year as 年級, sb.enroll_year as 入學年度, dg.name as 系所組別, class_name as 教學分班, student_number as 學號, student.name as 姓名, 
case is_in_school when true then '是' when false then '否' end as 是否在校, case upper(update_code) when 'G' then '畢業' when 'O' then '退學' when 'B' then '休學'  when 'R' then '復學' else '' end as 異動類別, 
sb.update_schoolyear_semester as 最後異動學年度學期, sb.graduate_year as 畢業學年度, sb.graduate_semester as 畢業學期, sb.transfer_dept_schoolyear_semester as 轉系學年度學期, sb.transfer_previous_dept as 轉系前原系所,
sb.previous_school as 入學前畢業學校, sb.nationality as 國籍, sb.drop_out_code as 退學原因代碼, sb.id_code as 身分別代碼, case sb.is_delay when true then '是' when false then '否' end as 是否延畢, 
suspension1 as 休學一, suspension2 as 休學二, suspension3 as 休學三, suspension4 as 休學四, suspension5 as 休學五, suspension6 as 休學六, suspension7 as 休學七, suspension8 as 休學八, case student.status when 1 then '在學' when 4 then '休學' when 16 then '畢業' when 64 then '退學' end as 學生狀態
from student 
left join $ischool.emba.student_brief2 as sb on sb.ref_student_id=student.id  
left join $ischool.emba.department_group as dg on dg.uid=sb.ref_department_group_id 
left join class on class.id=student.ref_class_id
where student.status in (1, 4, 16, 64) and student.id in ({0}) 
order by sb.grade_year, dg.name, class_name, student_number", string.Join(",", StudentIDs));

            return querySQL;
        }

        private void BGW_DoWork(object sender, DoWorkEventArgs e)
        {
            List<string> SelectedFields = (e.Argument as object[]).ElementAt(0) as List<string>;
            System.Windows.Forms.Form form = (e.Argument as object[]).ElementAt(1) as System.Windows.Forms.Form;
            List<string> StudentIDs = (e.Argument as object[]).ElementAt(2) as List<string>;

            DataTable dataTable = Query.Select(this.GetQueryString(StudentIDs));
            foreach(DataColumn column in dataTable.Columns.Cast<DataColumn>().ToList())
            {
                if (SelectedFields.Contains(column.ColumnName))
                    continue;

                dataTable.Columns.Remove(column);
            }

            Workbook wb = new Workbook();
            wb.Worksheets.Cast<Worksheet>().ToList().ForEach(x => wb.Worksheets.RemoveAt(x.Name));
            wb.Worksheets.Add();
            wb.Worksheets[0].Name = "異動記錄";
            wb.Worksheets[0].Cells.ImportDataTable(dataTable, true, "A1");
            //int i=0;
            //foreach (DataColumn column in dataTable.Columns)
            //{
            //    wb.Worksheets[0].Cells[0, i].PutValue(column.ColumnName);
            //    i++;
            //}
            wb.Worksheets[0].AutoFitColumns();
            e.Result = new object[] { wb, form };
        }

        private void BGW_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            Workbook wb = (e.Result as object[]).ElementAt(0) as Workbook;
            System.Windows.Forms.Form form = (e.Result as object[]).ElementAt(1) as System.Windows.Forms.Form;
            form.Close();

            if (e.Error != null)
            {
                MessageBox.Show(e.Error.Message);
                return;
            }
            else
            {
                SaveFileDialog sd = new SaveFileDialog();
                sd.Title = "另存新檔";
                sd.FileName = "匯出異動記錄.xls";
                sd.Filter = "Excel 2003 相容檔案 (*.xls)|*.xls|所有檔案 (*.*)|*.*";
                if (sd.ShowDialog() == DialogResult.OK)
                {
                    try
                    {
                        wb.Save(sd.FileName, FileFormatType.Excel2003);
                        System.Diagnostics.Process.Start(sd.FileName);
                    }
                    catch
                    {
                        MessageBox.Show("指定路徑無法存取。", "建立檔案失敗", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }
                }
            }
        }
    }
}