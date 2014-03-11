using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using FISCA;
using FISCA.Presentation.Controls;
using System;
using System.Linq;
using EMBA.Export;
using FISCA.Data;
using System.Data;
using System.Text;
using System.Xml;
using System.Xml.Linq;
using Aspose.Cells;
using System.IO;
using System.ComponentModel;
using EMBACore.DataItems;
using System.Threading.Tasks;

namespace EMBA.Print
{
    public partial class Student_CourseList : BaseForm
    {
        private Dictionary<string, ListViewItem> export_Fields;
        private string querySQL;
        private Workbook workbook;
        private BackgroundWorker backgroundWorker;

        public Student_CourseList()
        {
            InitializeComponent();

            backgroundWorker = new BackgroundWorker();
            backgroundWorker.DoWork += new System.ComponentModel.DoWorkEventHandler(backgroundWorker_DoWork);
            backgroundWorker.RunWorkerCompleted += new System.ComponentModel.RunWorkerCompletedEventHandler(backgroundWorker_RunWorkerCompleted);

            this.Load += new System.EventHandler(this.Student_CourseList_Load);
            this.btnPrint.Click += new System.EventHandler(this.btnPrint_Click);
        }

        private void backgroundWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            try
            {
                workbook = new Workbook();
                workbook.Open(new MemoryStream(EMBACore.Properties.Resources.選課名單樣版));
                Worksheet templateSheet = workbook.Worksheets[0];
                List<K12.Data.CourseRecord> courses = K12.Data.Course.SelectByIDs(K12.Presentation.NLDPanels.Course.SelectedSource);

                List<DataTable> t = new List<DataTable>();
                Parallel.ForEach<string>(new string[] { "1", "2" }, x =>
                {

                    lock (t)
                    {
                        t.Add(new DataTable());

                    }
                });

                foreach (K12.Data.CourseRecord course in courses)
                {
                    int rowIndex = 0;
                    int no = 0;
                    querySQL = this.SetQueryString(course.ID);
                    QueryHelper helper = new QueryHelper();
                    DataTable dataTable = helper.Select(querySQL);
                    if (dataTable.Rows.Count == 0) continue;

                    //  每個課程要做的事
                    //  1、複製樣版
                    int instanceSheetIndex = workbook.Worksheets.AddCopy("選課名單樣版");
                    Worksheet instanceSheet = workbook.Worksheets[instanceSheetIndex];
                    instanceSheet.Name = course.Name;
                    string schoolYear = (course.SchoolYear.HasValue ? course.SchoolYear.Value.ToString() : "");
                    string semester = (course.Semester.HasValue ? course.Semester.Value.ToString() : "");
                    string reportTitle = schoolYear + "學年度" + SemesterItem.GetSemesterByCode(semester).Name + "  " + course.Name;
                    //  2、設定標題
                    this.SetTitle(instanceSheet, 0, reportTitle);
                    //  3、列標加2
                    rowIndex += 2;
                    foreach (DataRow x in dataTable.Rows)
                    {
                        no += 1;
                        //  每10筆資料要做的事
                        //  1、複制樣版
                        //  2、設定標題
                        //  3、插入換行符號
                        //  4、列標加2
                        if (no > 10 && ((no-1) % 10 == 0))
                        {
                            //  1、複制樣版
                            Range instanceRange = instanceSheet.Cells.CreateRange(rowIndex, templateSheet.Cells.MinColumn, templateSheet.Cells.MaxRow + 1, templateSheet.Cells.MaxColumn + 1);
                            Range templateRange = templateSheet.Cells.CreateRange(templateSheet.Cells.MinRow, templateSheet.Cells.MinColumn, templateSheet.Cells.MaxRow + 1, templateSheet.Cells.MaxColumn + 1);

                            instanceRange.Copy(templateRange);
                            instanceRange.CopyStyle(templateRange);

                            // 若不手動設定列高，則列高為預設值
                            int index = rowIndex;
                            for (int i = templateSheet.Cells.MinRow; i <= templateSheet.Cells.MaxRow; i++)
                            {
                                if (templateSheet.Cells.Rows[i] == null)
                                    continue;

                                instanceSheet.Cells.SetRowHeight(index, templateSheet.Cells.GetRowHeight(i));
                                index++;
                            }

                            //  2、設定標題
                            SetTitle(instanceSheet, rowIndex, reportTitle);

                            //  3、插入換行符號
                            instanceSheet.HPageBreaks.Add(rowIndex, instanceSheet.Cells.MaxColumn);

                            //  3、列標加2
                            rowIndex += 2;                            
                        }

                        //  每筆資料要做的事
                        //  1、寫入資料
                        //  2、列標加1

                        //  1、寫入資料
                        if (export_Fields.ContainsKey("序號")) instanceSheet.Cells[rowIndex, 0].PutValue(no);
                        if (export_Fields.ContainsKey("系所")) instanceSheet.Cells[rowIndex, 1].PutValue(x["系所"] + "");
                        if (export_Fields.ContainsKey("分組")) instanceSheet.Cells[rowIndex, 2].PutValue(x["分組"] + "");
                        if (export_Fields.ContainsKey("學號")) instanceSheet.Cells[rowIndex, 3].PutValue(x["學號"] + "");
                        if (export_Fields.ContainsKey("姓名")) instanceSheet.Cells[rowIndex, 4].PutValue(x["姓名"] + "");
                        /* 照片 **/
                        if (export_Fields.ContainsKey("照片"))
                        {
                            if (string.IsNullOrEmpty((x["照片"] + "")))
                                instanceSheet.Cells[rowIndex, 5].PutValue(string.Empty);
                            else
                            {
                                byte[] bs = Convert.FromBase64String(x["照片"] + "");
                                MemoryStream ms = new MemoryStream(bs);
                                Cell cell = instanceSheet.Cells[rowIndex, 5];
                                instanceSheet.Pictures.Add(cell.Row, cell.Column, cell.Row + 1, cell.Column + 1, ms);
                            }
                        }
                        /********/
                        if (export_Fields.ContainsKey("電話")) instanceSheet.Cells[rowIndex , 6].PutValue(x["電話"] + "");
                        if (export_Fields.ContainsKey("任職公司")) instanceSheet.Cells[rowIndex, 7].PutValue(x["任職公司"] + "");
                        if (export_Fields.ContainsKey("職稱")) instanceSheet.Cells[rowIndex, 8].PutValue(x["職稱"] + "");
                        if (export_Fields.ContainsKey("畢業學校")) instanceSheet.Cells[rowIndex, 9].PutValue(x["畢業學校"] + "");
                        if (export_Fields.ContainsKey("電子郵件"))
                        {
                            StringBuilder sb = new StringBuilder();
                            IEnumerable<XElement> emails = XElement.Parse("<root>" + (x["電子郵件"] + "") + "</root>").Descendants();
                            foreach (XElement elm in emails)
                            {
                                if (!string.IsNullOrWhiteSpace(elm.Value))
                                {
                                    sb.Append(elm.Value);
                                    sb.Append(";");
                                }
                            }
                            instanceSheet.Cells[rowIndex, 10].PutValue(sb.ToString());
                        }
                        //  2、列標加1
                        rowIndex++;
                    }
                    //List<string> columns = dataTable.Columns.Cast<DataColumn>().Select(x => x.ColumnName).ToList();
                    //foreach (string columnName in columns)
                    //{
                    //    if (!export_Fields[columnName].Checked)
                    //        dataTable.Columns.Remove(columnName);
                    //}
                }
                e.Result = workbook;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                return;
            }
        }

        private void SetTitle(Worksheet instanceSheet, int rowIndex, string reportTitle)
        {
            instanceSheet.Cells[rowIndex, 0].PutValue(reportTitle);
        }

        private void backgroundWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            Workbook workbook = new Workbook();

            if (e.Result == null)
                return;

            try
            {
                workbook = e.Result as Workbook;

                //  匯出資料
                workbook.Worksheets.RemoveAt("選課名單樣版");
                if (workbook.Worksheets.Count == 0)
                    throw new Exception("您所選的課程沒有修課學生，無法產生選課名單！");
                workbook.Save(true, "選課名單", true);
            }
            catch (Exception ex)
            {
                MsgBox.Show(ex.Message);
            }
        }
        
        private void btnPrint_Click(object sender, System.EventArgs e)
        {
            backgroundWorker.RunWorkerAsync();
            this.Close();        
        }

        private void Student_CourseList_Load(object sender, System.EventArgs e)
        {
            querySQL = this.SetQueryString(string.Empty);

            //  解析查詢語法，抽出欄位名稱
            List<string> resolve_Fields = querySQL.ResolveField();
            export_Fields = new Dictionary<string, ListViewItem>();
            listView.Items.Clear();
            for (int i = 0; i < resolve_Fields.Count; i++)
            {
                ListViewItem item = listView.Items.Add(resolve_Fields[i]);
                item.Checked = true;
                if (!export_Fields.ContainsKey(resolve_Fields[i]))
                    export_Fields.Add(resolve_Fields[i], item);
            }
        }

        private string SetQueryString(string course_id)
        {
            string querySQL = string.Empty;
            if (string.IsNullOrEmpty(course_id)) course_id = "0";
            querySQL = string.Format(@"select '' as 序號,  table_d.系所, table_a.report_group as 分組, table_d.學號, table_d.姓名, table_d.照片, table_d.電話, table_b.company_name as 任職公司, table_b.position as 職稱, table_c.school_name as 畢業學校, table_d.電子郵件, '' as 電子郵件一, '' as 電子郵件二, '' as 電子郵件三, '' as 電子郵件四, '' as 電子郵件五, '' as 住家電話, '' as 聯絡電話, '' as 行動電話1, '' as 公司電話, '' as 行動電話2,'' as 秘書電話 from 
(select student.id as student_id, se.report_group from course
join $ischool.emba.scattend_ext se on se.ref_course_id=course.id
join student on student.id=se.ref_student_id where course.id in ('{0}')
) as table_a
left join 
(select id as student_id, dg.name as 系所, student.student_number as 學號, student.name as 姓名,  
student.freshman_photo as 照片, (xpath_string(student.other_phones,'PhoneNumber[1]')) as 電話, sb2.email_list as 電子郵件
from student
left join $ischool.emba.student_brief2 as sb2 on sb2.ref_student_id=student.id
left join $ischool.emba.department_group dg on sb2.ref_department_group_id = dg.uid
) as table_d
on table_a.student_id=table_d.student_id
left join 
(select ex.ref_student_id as student_id, ex.company_name, ex.position, ex.industry from $ischool.emba.experience as ex join student on ex.ref_student_id=student.id where ex.work_status='現職'
) as table_b
on table_d.student_id=table_b.student_id
left join 
(select eb.ref_student_id as student_id, eb.school_name, eb.department, eb.degree from $ischool.emba.education_background as eb join student on student.id=eb.ref_student_id where is_top=true
) as table_c 
on table_d.student_id=table_c.student_id
order by table_d.學號", course_id);

            return querySQL;
        }

        private void chkSelect_CheckedChanged(object sender, EventArgs e)
        {
            foreach (ListViewItem item in listView.Items)
            {
                item.Checked = chkSelect.Checked;
            }
        }

        private void btnPrint_Click_1(object sender, EventArgs e)
        {

        }
    }
}