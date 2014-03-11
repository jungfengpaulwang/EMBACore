using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using Aspose.Cells;
using FISCA.Data;
using FISCA.Presentation.Controls;

namespace EMBA.Print
{
    public partial class Course_RollCallReport : BaseForm
    {
        private BackgroundWorker _BGWLoadData;
        private QueryHelper queryHelper;

        public Course_RollCallReport()
        {
            InitializeComponent();

            this.Load += new System.EventHandler(this.Course_RollCallReport_Load);
        }

        private void Course_RollCallReport_Load(object sender, System.EventArgs e)
        {
            if (K12.Presentation.NLDPanels.Course.SelectedSource.Count == 0)
            {
                MessageBox.Show("請先選取課程！");
                this.Close();
                return;
            }
            this.progress.Visible = true;
            this.progress.IsRunning = true;

            queryHelper = new QueryHelper();
            _BGWLoadData = new BackgroundWorker();
            _BGWLoadData.DoWork += new DoWorkEventHandler(_BGWLoadData_DoWork);
            _BGWLoadData.RunWorkerCompleted += new RunWorkerCompletedEventHandler(_BGWLoadData_RunWorkerCompleted);

            GetSCAttendExtAndCourseTimeTableData();
        }

        private void GetSCAttendExtAndCourseTimeTableData()
        {
            _BGWLoadData.RunWorkerAsync();
        }

        private void _BGWLoadData_DoWork(object sender, DoWorkEventArgs e)
        {
            string strQrySCAttend = string.Format(@"select course.school_year, course.semester, course.id as course_id, course_name, student.student_number, student.name as student_name from course join $ischool.emba.scattend_ext as se on se.ref_course_id=course.id join student on student.id=se.ref_student_id where course.id in ({0}) order by course.id,  student.student_number", string.Join(",", K12.Presentation.NLDPanels.Course.SelectedSource));
            DataTable dataTableSCAttend = queryHelper.Select(strQrySCAttend);

            string strQryCourseTimeTable = string.Format(@"select course.id as course_id, section.starttime, section.endtime from course join $ischool.course_calendar.section as section on CAST(section.refcourseid AS integer)=course.id where course.id in ({0}) and removed=false order by course.id, section.starttime", string.Join(",", K12.Presentation.NLDPanels.Course.SelectedSource));
            DataTable dataTableCourseTimeTable = queryHelper.Select(strQryCourseTimeTable);
            
            try
            {
                e.Result = MakeReport(dataTableSCAttend, dataTableCourseTimeTable);
            }
            catch (Exception ex)
            {
                e.Result = new Exception(ex.Message);
            }
        }

        private void _BGWLoadData_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            this.progress.IsRunning = false;
            this.progress.Visible = false;

            try
            {
                if (e.Result != null && e.Result.GetType().Equals(Type.GetType("System.Exception")))
                    throw new Exception(((System.Exception)e.Result).Message);

                Workbook workbook = e.Result as Workbook;
                if (workbook == null)
                {
                    MsgBox.Show("沒有資料！");
                    this.Close();
                    return;
                }
                SaveFileDialog sfd = new SaveFileDialog();
                sfd.Title = "另存新檔";
                sfd.FileName = "點名單.xls";
                sfd.Filter = "Excel 2003 相容檔案 (*.xls)|*.xls|所有檔案 (*.*)|*.*";
                DialogResult dr = sfd.ShowDialog();
                if (dr == System.Windows.Forms.DialogResult.OK)
                {
                    workbook.Save(sfd.FileName);
                    if (System.IO.File.Exists(sfd.FileName))
                        System.Diagnostics.Process.Start(sfd.FileName);
                }
            }
            catch (Exception ex)
            {
                MsgBox.Show(ex.Message);
            }
            finally
            {
                this.Close();
            }
        }

        private Workbook MakeReport(DataTable dataTableSCAttend, DataTable dataTableCourseTimeTable)
        {
            if (dataTableSCAttend.Rows.Count == 0)
                throw new Exception("您所選的課程沒有修課學生，無法產生點名單！");

            IEnumerable<DataRow> lstSCAttends = dataTableSCAttend.Rows.Cast<DataRow>();
            IEnumerable<DataRow> lstCourseTimeTables = dataTableCourseTimeTable.Rows.Cast<DataRow>();
            Workbook workbook = new Workbook();
            try
            {
                //  讀取樣版檔
                workbook.Open(new MemoryStream(EMBACore.Properties.Resources.點名單樣版));
                //  讀取樣版工作表
                Worksheet templateSheet = workbook.Worksheets[0];
                Dictionary<string, List<DataRow>> dicSCAttends = new Dictionary<string,List<DataRow>>();
                lstSCAttends.ToList().ForEach((x) =>
                {
                    if (!dicSCAttends.ContainsKey(x["course_id"] + ""))
                        dicSCAttends.Add(x["course_id"] + "", new List<DataRow>());

                    dicSCAttends[x["course_id"] + ""].Add(x);
                });
                foreach (string key in dicSCAttends.Keys)
                {
                    //  Copy樣版工作表
                    int instanceSheetIndex = workbook.Worksheets.AddCopy("點名單樣版");
                    Worksheet instanceSheet = workbook.Worksheets[instanceSheetIndex];
                    IEnumerable<DataRow> filterSCAttends = dicSCAttends[key];
                    string course_name = (filterSCAttends.ElementAt(0)["course_name"] + "");
                    instanceSheet.Name = course_name;
                    string schoolYear = (filterSCAttends.ElementAt(0)["school_year"] + "");
                    string semester = (EMBACore.DataItems.SemesterItem.GetSemesterByCode(filterSCAttends.ElementAt(0)["semester"] + "") as EMBACore.DataItems.SemesterItem).Name;
                    string reportTitle = schoolYear + "學年度" + semester + "【" + course_name + "】點名單";
                    instanceSheet.Cells[0, 0].PutValue(reportTitle);
                    //  修課學生超過60人或上課時段超過10段，Copy樣版
                    IEnumerable<DataRow> filterCourseTimeTables = lstCourseTimeTables.Where(x => (x["course_id"] + "") == key);
                    int copyTimes = (filterSCAttends.Count() / 61 + 1) * (filterCourseTimeTables.Count() / 11 + 1);
                    for (int j = 1; j < copyTimes; j++)
                    {
                        Range templateRange = templateSheet.Cells.CreateRange(templateSheet.Cells.MinRow, templateSheet.Cells.MinColumn, templateSheet.Cells.MaxRow + 1, templateSheet.Cells.MaxColumn + 1);
                        Range instanceRange = instanceSheet.Cells.CreateRange(62 * j, templateSheet.Cells.MinColumn, templateSheet.Cells.MaxRow + 1, templateSheet.Cells.MaxColumn + 1);

                        instanceSheet.HPageBreaks.Add(instanceSheet.Cells.MaxRow + 1, instanceSheet.Cells.MaxColumn);

                        instanceRange.Copy(templateRange);
                        instanceRange.CopyStyle(templateRange);
                        instanceSheet.Cells[62 * j, 0].PutValue(reportTitle);

                        // 若不手動設定列高，則列高為預設值
                        int index = 62 * j;
                        for (int i = templateSheet.Cells.MinRow; i <= templateSheet.Cells.MaxRow; i++)
                        {
                            if (templateSheet.Cells.Rows[i] == null)
                                continue;

                            instanceSheet.Cells.SetRowHeight(index, templateSheet.Cells.GetRowHeight(i));
                            index++;
                        }
                    }
                    //  填入修課學生資料
                    FillSCAttend(instanceSheet, filterSCAttends, filterCourseTimeTables);
                }
                //  匯出資料
                workbook.Worksheets.RemoveAt("點名單樣版");
                return workbook;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        private void FillSCAttend(Worksheet instanceSheet, IEnumerable<DataRow> filterSCAttends, IEnumerable<DataRow> filterCourseTimeTables)
        {            
            int repeatTimes_CourseTimeTable = 1;
            if (filterCourseTimeTables.Count() > 0)
                repeatTimes_CourseTimeTable = (filterCourseTimeTables.Count() / 11 + 1);
            int repeatTimes_SCAttend = (filterSCAttends.Count() / 61 + 1);
            int serial_No = 0;
            int rowIndex = 1;
            foreach (DataRow row in filterSCAttends)
            {
                serial_No += 1;
                if (serial_No > 1 && serial_No % 60 == 1)
                    rowIndex += 3;
                else
                    rowIndex += 1;

                for (int q = 0; q < repeatTimes_CourseTimeTable; q++)
                {
                    instanceSheet.Cells[q * 62 * repeatTimes_SCAttend + rowIndex, 0].PutValue(serial_No);
                    instanceSheet.Cells[q * 62 * repeatTimes_SCAttend + rowIndex, 1].PutValue(row["student_number"] + "");
                    instanceSheet.Cells[q * 62 * repeatTimes_SCAttend + rowIndex, 2].PutValue(row["student_name"] + "");
                }
            }

            //  填入上課時間
            int page = 0;
            for (int k = 0; k < filterCourseTimeTables.Count(); k++)
            {
                if (k > 0 && (k % 10 == 0))
                {
                    page = page + repeatTimes_SCAttend;
                }

                DateTime startTime = DateTime.Now;
                DateTime.TryParse(filterCourseTimeTables.ElementAt(k)["starttime"] + "", out startTime);
                DateTime endTime = DateTime.Now;
                DateTime.TryParse(filterCourseTimeTables.ElementAt(k)["endtime"] + "", out endTime);
                string strDate = startTime.ToString("yyyy/MM/dd");
                string strStartTime = startTime.ToString("HH:mm");
                string strEndTime = endTime.ToString("HH:mm");
                for (int z = 0; z < repeatTimes_SCAttend; z++)
                {
                    instanceSheet.Cells[(page+z) * 62 + 1, k % 10 + 3].PutValue(strDate + "\n" + strStartTime + "~" + strEndTime);
                }
            }     
        }
    }
}