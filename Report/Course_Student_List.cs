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

namespace EMBA.Print
{
    class Course_Student_List
    {
        private List<string> CourseIDs;
        private BackgroundWorker BGW;
        private QueryHelper Query;

        public Course_Student_List(List<string> CourseIDs)
        {
            this.CourseIDs = CourseIDs;
            this.Query = new QueryHelper();
            this.BGW = new BackgroundWorker();
            this.BGW.DoWork += new DoWorkEventHandler(BGW_DoWork);
            this.BGW.RunWorkerCompleted += new RunWorkerCompletedEventHandler(BGW_RunWorkerCompleted);
        }

        public void Execute()
        {
            if (this.CourseIDs.Count == 0)
            {
                MessageBox.Show("請先選取課程。", "錯誤", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            List<string> Fields = new List<string>() { "序號", "系所", "分組", "學號", "姓名", "照片", "公司電話", "行動電話1", "行動電話2", "任職公司", "職稱", "畢業學校", "電子郵件" };
            Agent.Viewer.SelectField SelectFieldForm = Agent.Export.GetSelectFieldForm();
            SelectFieldForm.SetFields(Fields).SetTitleText("列印選課名單").SetSelectAllState(true).SetConfirmButtonText("列印").SetExitButtonText("離開");
            if (SelectFieldForm.ShowDialog() != DialogResult.OK)
                return;
            List<string> SelectedFields = SelectFieldForm.GetSelectedFields();
            System.Windows.Forms.Form form = Agent.Export.GetProgressForm();
            if (SelectedFields.Count > 0)
            {
                this.BGW.RunWorkerAsync(new object[] { SelectedFields, form, CourseIDs });
                form.ShowDialog();
            }
            else
            {
                MessageBox.Show("未勾選欄位。", "錯誤", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
        }

        private string GetQueryString(string CourseID)
        {
            string querySQL = string.Format(@"select '' as 序號, table_a.course_name as 開課名稱, table_a.school_year as 學年度, table_a.semester as 學期, table_d.系所, table_a.report_group as 分組, table_d.學號, table_d.姓名, table_d.照片, table_b.company_name as 任職公司, table_b.position as 職稱, table_c.school_name as 畢業學校, table_d.電子郵件, table_d.公司電話, table_d.行動電話1, table_d.行動電話2 from 
(select student.id as student_id, se.report_group, course.course_name, course.school_year, course.semester from course
join $ischool.emba.scattend_ext se on se.ref_course_id=course.id
join student on student.id=se.ref_student_id where course.id in ({0})
) as table_a
left join 
(select id as student_id, dg.name as 系所, student.student_number as 學號, student.name as 姓名,  
student.freshman_photo as 照片, sb2.email_list as 電子郵件, (xpath_string(student.other_phones,'PhoneNumber[1]')) as 公司電話, student.sms_phone as 行動電話1, (xpath_string(student.other_phones,'PhoneNumber[2]')) as 行動電話2
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
(select eb.ref_student_id as student_id, case trim(both ' ' from degree) when '學士' then trim(both ' ' from school_name) || trim(both ' ' from department) when '' then trim(both ' ' from school_name) || trim(both ' ' from department) else trim(both ' ' from school_name) || trim(both ' ' from department) || '(' || trim(both ' ' from degree) || ')' end as school_name from $ischool.emba.education_background as eb join student on student.id=eb.ref_student_id where is_top=true
) as table_c 
on table_d.student_id=table_c.student_id
order by table_d.學號", "'" + CourseID + "'");

            return querySQL;
        }

        private void BGW_DoWork(object sender, DoWorkEventArgs e)
        {
            List<string> SelectedFields = (e.Argument as object[]).ElementAt(0) as List<string>;
            System.Windows.Forms.Form form = (e.Argument as object[]).ElementAt(1) as System.Windows.Forms.Form;
            List<string> CourseIDs = (e.Argument as object[]).ElementAt(2) as List<string>;

            List<DataTable> dataTables = new List<DataTable>();
            Parallel.ForEach<string>(CourseIDs, x =>
            {
                try
                {
                    DataTable dataTable = Query.Select(this.GetQueryString(x));

                    if (dataTable.Rows.Count > 0)
                    {
                        dataTable.TableName = dataTable.Rows[0]["開課名稱"] + "";
                        lock (dataTables)
                        {
                            dataTables.Add(dataTable);
                        }
                    }
                }
                catch (Exception ex)
                {
                    throw new Exception(ex.Message);
                }
            });
            Dictionary<string, List<DataTable>> dicDataTables = new Dictionary<string, List<DataTable>>();
            Dictionary<string, List<DataSet>> dicDataSets = new Dictionary<string,List<DataSet>>();
            dataTables.ForEach((x) =>
            {
                if (!dicDataTables.ContainsKey(x.TableName))
                    dicDataTables.Add(x.TableName, new List<DataTable>());

                dicDataTables[x.TableName].Add(x);
            });
            string schoolYear = string.Empty;
            string semester = string.Empty;
            foreach(string key in dicDataTables.Keys)
            {
                List<DataTable> tables = dicDataTables[key];
                int i = 0;
                if (tables.Count > 1)
                    i = 1;
                foreach (DataTable dataTable in tables)
                {
                    string kkk = (i == 0 ? key : key + "-" + i);
                    dicDataSets.Add(kkk, new List<DataSet>());

                    DataSet dataSet_PageHeader = new DataSet("PageHeader");
                    schoolYear = dataTable.Rows[0]["學年度"] + "";
                    semester = dataTable.Rows[0]["學期"] + "";
                    string reportTitle = schoolYear + "學年度" + SemesterItem.GetSemesterByCode(semester).Name + "  " + kkk;
                    dataSet_PageHeader.Tables.Add(reportTitle.ToDataTable("CourseName", "CourseName"));
                    dicDataSets[kkk].Add(dataSet_PageHeader);

                    Dictionary<string, dynamic> dicStudents = new Dictionary<string, dynamic>();
                    foreach (DataRow row in dataTable.Rows)
                    {
                        if (!dicStudents.ContainsKey((row["學號"] + "").Trim().ToLower()))
                            dicStudents.Add((row["學號"] + "").Trim().ToLower(), new ExpandoObject());

                        dynamic o = dicStudents[(row["學號"] + "").Trim().ToLower()];

                        o.系所 = row["系所"] + "";
                        o.分組 = row["分組"] + "";
                        o.學號 = row["學號"] + "";
                        o.姓名 = row["姓名"] + "";
                        o.照片 = row["照片"] + "";
                        o.公司電話 = row["公司電話"] + "";
                        o.行動電話1 = row["行動電話1"] + "";
                        o.行動電話2 = row["行動電話2"] + "";
                        o.電子郵件 = row["電子郵件"] + "";

                        if (!((IDictionary<String, object>)o).ContainsKey("任職公司"))
                            o.任職公司 = new List<string>();
                        if (!((IDictionary<String, object>)o).ContainsKey("職稱"))
                            o.職稱 = new List<string>();
                        if (!((IDictionary<String, object>)o).ContainsKey("畢業學校"))
                            o.畢業學校 = new List<string>();

                        o.任職公司.Add(row["任職公司"] + "");
                        o.職稱.Add(row["職稱"] + "");
                        o.畢業學校.Add(row["畢業學校"] + "");
                    }

                    int j = 0;
                    foreach (string key_o in dicStudents.Keys)
                    {
                        j++;

                        //  序號、系所、分組、學號、姓名、照片、電話、任職公司、職稱、畢業學校、電子郵件
                        dynamic o = dicStudents[key_o];

                        DataSet dataSet_DataSection = new DataSet("DataSection");
                        if (SelectedFields.Contains("序號"))
                            dataSet_DataSection.Tables.Add(j.ToDataTable("序號", "序號"));
                        else
                            dataSet_DataSection.Tables.Add("".ToDataTable("序號", "序號"));

                        if (SelectedFields.Contains("系所"))
                            dataSet_DataSection.Tables.Add((o.系所 as string).ToDataTable("系所", "系所"));
                        else
                            dataSet_DataSection.Tables.Add("".ToDataTable("系所", "系所"));

                        if (SelectedFields.Contains("分組"))
                            dataSet_DataSection.Tables.Add((o.分組 as string).ToDataTable("分組", "分組"));
                        else
                            dataSet_DataSection.Tables.Add("".ToDataTable("分組", "分組"));

                        if (SelectedFields.Contains("學號"))
                            dataSet_DataSection.Tables.Add((o.學號 as string).ToDataTable("學號", "學號"));
                        else
                            dataSet_DataSection.Tables.Add("".ToDataTable("學號", "學號"));

                        if (SelectedFields.Contains("姓名"))
                            dataSet_DataSection.Tables.Add((o.姓名 as string).ToDataTable("姓名", "姓名"));
                        else
                            dataSet_DataSection.Tables.Add("".ToDataTable("姓名", "姓名"));

                        if (SelectedFields.Contains("照片"))
                        {
                            if (string.IsNullOrEmpty((o.照片 + "")))
                                dataSet_DataSection.Tables.Add("".ToDataTable("照片", "照片"));
                            else
                            {
                                byte[] bs = Convert.FromBase64String(o.照片 + "");
                                dataSet_DataSection.Tables.Add(bs.ToDataTable("照片", "照片", bs.GetType()));
                            }
                        }

                        if (SelectedFields.Contains("公司電話"))
                            dataSet_DataSection.Tables.Add((o.公司電話 as string).ToDataTable("公司電話", "公司電話"));
                        else
                            dataSet_DataSection.Tables.Add("".ToDataTable("公司電話", "公司電話"));

                        List<string> Tels = new List<string>();
                        if (SelectedFields.Contains("行動電話1"))
                            Tels.Add(o.行動電話1 + "");
                        if (SelectedFields.Contains("行動電話2"))
                            Tels.Add(o.行動電話2 + "");

                        //行動電話1、行動電話2
                        string tel = string.Join(",", Tels.Where(x => !string.IsNullOrWhiteSpace(x)));
                        dataSet_DataSection.Tables.Add(tel.ToDataTable("手機", "手機"));

                        if (SelectedFields.Contains("任職公司"))
                            dataSet_DataSection.Tables.Add(string.Join(";", (o.任職公司 as List<string>).Distinct()).ToDataTable("任職公司", "任職公司"));
                        else
                            dataSet_DataSection.Tables.Add("".ToDataTable("任職公司", "任職公司"));

                        if (SelectedFields.Contains("職稱"))
                            dataSet_DataSection.Tables.Add(string.Join(";", (o.職稱 as List<string>).Distinct()).ToDataTable("職稱", "職稱"));
                        else
                            dataSet_DataSection.Tables.Add("".ToDataTable("職稱", "職稱"));

                        if (SelectedFields.Contains("畢業學校"))
                            dataSet_DataSection.Tables.Add(string.Join(";", (o.畢業學校 as List<string>).Distinct()).ToDataTable("畢業學校", "畢業學校"));
                        else
                            dataSet_DataSection.Tables.Add("".ToDataTable("畢業學校", "畢業學校"));

                        string email = string.Empty;
                        if (SelectedFields.Contains("電子郵件"))
                        {
                            StringBuilder sb = new StringBuilder();
                            IEnumerable<XElement> emails = XElement.Parse("<root>" + (o.電子郵件 + "") + "</root>").Descendants();
                            List<string> Emails = new List<string>();
                            foreach (XElement elm in emails)
                            {
                                Emails.Add(elm.Value);
                            }
                            email = string.Join(";", Emails.Where(x => !string.IsNullOrWhiteSpace(x)));
                        }
                        dataSet_DataSection.Tables.Add(email.ToDataTable("電子郵件", "電子郵件"));
                        dicDataSets[kkk].Add(dataSet_DataSection);
                    }
                    i++;
                }
            }

            Workbook wb = ReportHelper.Report.Produce(dicDataSets, new MemoryStream(EMBACore.Properties.Resources.選課名單樣版), false);
            wb.Worksheets.Cast<Worksheet>().ToList().ForEach(x => x.PageSetup.PrintTitleRows = "$1:$2");
            //wb.Worksheets.Cast<Worksheet>().ToList().ForEach(y => y.AutoFitRows());
            e.Result = new object[] { wb, form, schoolYear, semester };
        }

        private void BGW_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            Workbook wb = (e.Result as object[]).ElementAt(0) as Workbook;
            System.Windows.Forms.Form form = (e.Result as object[]).ElementAt(1) as System.Windows.Forms.Form;
            string schoolYear = (e.Result as object[]).ElementAt(2) + "";
            string semester = (e.Result as object[]).ElementAt(3) + "";
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
                sd.FileName = schoolYear + "-" + semester + " 選課名單.xls";
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