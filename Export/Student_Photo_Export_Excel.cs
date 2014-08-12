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
    partial class Student_Photo_Export_Excel : Agent.Viewer.SelectField
    {
        private IEnumerable<string> StudentIDs;
        private BackgroundWorker BGW;
        private QueryHelper Query;

        public Student_Photo_Export_Excel(IEnumerable<string> StudentIDs)
        {
            this.InitializeComponent();

            this.StudentIDs = StudentIDs;
            this.Query = new QueryHelper();
            this.BGW = new BackgroundWorker();
            this.BGW.DoWork += new DoWorkEventHandler(BGW_DoWork);
            this.BGW.RunWorkerCompleted += new RunWorkerCompletedEventHandler(BGW_RunWorkerCompleted);
            this.BGW.WorkerSupportsCancellation = true;

            List<string> Fields = new List<string>() { "學生系統編號", "身分證號", "教學分班", "學號", "姓名", "英文姓名", "性別", "生日", "登入帳號", "國籍", "電子郵件一", "電子郵件二", "電子郵件三", "電子郵件四", "電子郵件五", "住家電話", "聯絡電話", "行動電話1", "公司電話", "行動電話2", "秘書電話", "住家地址", "住家地址:郵遞區號", "住家地址:縣市", "住家地址:鄉鎮", "住家地址:村里", "住家地址:鄰", "住家地址:其它", "聯絡地址", "聯絡地址:郵遞區號", "聯絡地址:縣市", "聯絡地址:鄉鎮", "聯絡地址:村里", "聯絡地址:鄰", "聯絡地址:其它", "公司地址", "公司地址:郵遞區號", "公司地址:縣市", "公司地址:鄉鎮", "公司地址:村里", "公司地址:鄰", "公司地址:其它", "入學年度", "畢業學年度", "畢業學期", "系所組別", "公司名稱", "職稱", "產業別", "部門類別", "層級別", "工作地點", "工作狀態", "工作起日", "工作迄日", "公關連絡人", "公關室電話", "公關室傳真", "公關 Email", "公司網址", "學生修改經歷資料", "經歷最後更新日期", "畢業學校", "畢業系所", "學位", "學生狀態" };
            this.SetFields(Fields).SetTitleText("匯出學生基本資料").SetSelectAllState(true).SetConfirmButtonText("匯出").SetExitButtonText("離開");
            this.btnConfirm.DialogResult = System.Windows.Forms.DialogResult.None;

            this.FormClosed += new FormClosedEventHandler(Form_Closed);
        }

        private void Form_Closed(object sender, FormClosedEventArgs e)
        {
            this.BGW.CancelAsync();
        }

        public void Execute()
        {
            List<string> SelectedFields = this.GetSelectedFields();
            System.Windows.Forms.Form form = Agent.Export.GetProgressForm();
            if (SelectedFields.Count > 0)
            {
                this.BGW.RunWorkerAsync(new object[] { SelectedFields, form, StudentIDs, this.chkPhoto.Checked });
                this.Visible = false;
                form.StartPosition = FormStartPosition.CenterParent;
                form.ShowDialog(this);
            }
            else
            {
                MessageBox.Show("未勾選欄位。", "錯誤", MessageBoxButtons.OK, MessageBoxIcon.Error);
                this.Visible = true;
                return;
            }
        }

        private string GetQueryString()
        {
            string querySQL = string.Empty;

			querySQL = string.Format(@"select table_b.ex_uid, table_a.freshman_photo as 照片, table_a.學生系統編號, table_a.身分證號, table_a.教學分班, table_a.學號, table_a.姓名, table_a.英文姓名, table_a.性別, table_a.生日, table_a.sa_login_name as 登入帳號, table_a.國籍, table_a.電子郵件一, table_a.電子郵件二, table_a.電子郵件三, table_a.電子郵件四, table_a.電子郵件五, table_a.住家電話, table_a.聯絡電話, table_a.行動電話1, table_a.公司電話, table_a.行動電話2, table_a.秘書電話, table_a.住家地址, table_a.""住家地址:郵遞區號"", table_a.""住家地址:縣市"", table_a.""住家地址:鄉鎮"", table_a.""住家地址:村里"", table_a.""住家地址:鄰"", table_a.""住家地址:其它"", table_a.聯絡地址, table_a.""聯絡地址:郵遞區號"", table_a.""聯絡地址:縣市"", table_a.""聯絡地址:鄉鎮"", table_a.""聯絡地址:村里"", table_a.""聯絡地址:鄰"", table_a.""聯絡地址:其它"", table_a.公司地址, table_a.""公司地址:郵遞區號"", table_a.""公司地址:縣市"", table_a.""公司地址:鄉鎮"", table_a.""公司地址:村里"", table_a.""公司地址:鄰"", table_a.""公司地址:其它"", table_a.入學年度, table_a.畢業學年度, table_a.畢業學期, table_a.畢業學年期, table_a.異動代碼, table_a.系所組別, table_b.company_name as ""公司名稱"", table_b.position as ""職稱"", table_b.industry as ""產業別"", table_b.department_category as ""部門類別"", table_b.post_level as ""層級別"", table_b.work_place as ""工作地點"", table_b.work_status as ""工作狀態"", table_b.work_begin_date as ""工作起日"", table_b.work_end_date as ""工作迄日"", table_b.publicist as ""公關連絡人"", table_b.public_relations_office_telephone as ""公關室電話"", table_b.public_relations_office_fax as ""公關室傳真"", table_b.publicist_email as ""公關 Email"", table_b.company_website as ""公司網址"", table_b.action_by as ""學生修改經歷資料"", table_b.time_stamp as ""經歷最後更新日期"", table_c.school_name as ""畢業學校"", table_c.department as ""畢業系所"", table_c.degree as ""學位"", table_a.學生狀態 from 
(select freshman_photo, student.id as student_id, student.status as 學生狀態, student.id as 學生系統編號, student.id_number as 身分證號, class.class_name as 教學分班, student.student_number as 學號, student.name as 姓名, student.sa_login_name,
student.english_name as 英文姓名, student.gender as 性別, student.birthdate as 生日, sb2.nationality as 國籍, student.birth_place as 出生地, 
(xpath_string('<root>' || sb2.email_list || '</root>','email1')) as 電子郵件一, 
(xpath_string('<root>' || sb2.email_list || '</root>','email2')) as 電子郵件二, 
(xpath_string('<root>' || sb2.email_list || '</root>','email3')) as 電子郵件三, 
(xpath_string('<root>' || sb2.email_list || '</root>','email4')) as 電子郵件四, 
(xpath_string('<root>' || sb2.email_list || '</root>','email5')) as 電子郵件五,
student.permanent_phone as 住家電話, student.contact_phone as 聯絡電話, student.sms_phone as 行動電話1,
(xpath_string(student.other_phones,'PhoneNumber[1]')) as 公司電話,
(xpath_string(student.other_phones,'PhoneNumber[2]')) as 行動電話2,
(xpath_string(student.other_phones,'PhoneNumber[3]')) as 秘書電話,
'' as 住家地址,
(xpath_string(student.permanent_address,'Address/ZipCode')) as ""住家地址:郵遞區號"",
(xpath_string(student.permanent_address,'Address/County')) as ""住家地址:縣市"",
(xpath_string(student.permanent_address,'Address/Town')) as ""住家地址:鄉鎮"",
(xpath_string(student.permanent_address,'Address/District')) as ""住家地址:村里"",
(xpath_string(student.permanent_address,'Address/Area')) as ""住家地址:鄰"",
(xpath_string(student.permanent_address,'Address/DetailAddress')) as ""住家地址:其它"",
'' as 聯絡地址,
(xpath_string(student.mailing_address,'Address/ZipCode')) as ""聯絡地址:郵遞區號"",
(xpath_string(student.mailing_address,'Address/County')) as ""聯絡地址:縣市"",
(xpath_string(student.mailing_address,'Address/Town')) as ""聯絡地址:鄉鎮"",
(xpath_string(student.mailing_address,'Address/District')) as ""聯絡地址:村里"",
(xpath_string(student.mailing_address,'Address/Area')) as ""聯絡地址:鄰"",
(xpath_string(student.mailing_address,'Address/DetailAddress')) as ""聯絡地址:其它"",
'' as 公司地址,
(xpath_string(student.other_addresses,'Address[1]/ZipCode')) as ""公司地址:郵遞區號"",
(xpath_string(student.other_addresses,'Address[1]/County')) as ""公司地址:縣市"",
(xpath_string(student.other_addresses,'Address[1]/Town')) as ""公司地址:鄉鎮"",
(xpath_string(student.other_addresses,'Address[1]/District')) as ""公司地址:村里"",
(xpath_string(student.other_addresses,'Address[1]/Area')) as ""公司地址:鄰"",
(xpath_string(student.other_addresses,'Address[1]/DetailAddress')) as ""公司地址:其它"",
sb2.enroll_year as 入學年度, sb2.update_code as 異動代碼, sb2.update_schoolyear_semester as 畢業學年期, '' as 畢業學年度, '' as 畢業學期, dg.name as 系所組別
from student left join class on class.id=student.ref_class_id
left join $ischool.emba.student_brief2 as sb2 on sb2.ref_student_id=student.id
left join $ischool.emba.department_group dg on sb2.ref_department_group_id = dg.uid
) as table_a
left join 
(select ex.uid as ex_uid, ex.ref_student_id as student_id, ex.company_name, ex.position, ex.industry, ex.department_category, ex.post_level, ex.work_place, ex.is_current, ex.work_status, ex.work_begin_date, 
ex.work_end_date, ex.publicist, ex.public_relations_office_telephone, ex.public_relations_office_fax, ex.publicist_email, ex.company_website, case when lower(ex.action_by)='student' then '是' else ex.action_by end as action_by, ex.time_stamp from $ischool.emba.experience as ex 
join student on ex.ref_student_id=student.id where ex.work_status in ('現職', '退休')
order by ex.ref_student_id, ex.work_status, ex.work_begin_date DESC, ex.work_end_date DESC) as table_b
on table_a.student_id=table_b.student_id
left join 
(select eb.ref_student_id as student_id, eb.school_name, eb.department, eb.degree from $ischool.emba.education_background as eb join student on student.id=eb.ref_student_id where is_top=true) as table_c
on table_a.student_id=table_c.student_id where table_a.學生系統編號 in ({0})", String.Join(",", this.StudentIDs));

            return querySQL;
        }

        private void BGW_DoWork(object sender, DoWorkEventArgs e)
        {
            List<string> SelectedFields = (e.Argument as object[]).ElementAt(0) as List<string>;
            System.Windows.Forms.Form form = (e.Argument as object[]).ElementAt(1) as System.Windows.Forms.Form;
            List<string> StudentIDs = (e.Argument as object[]).ElementAt(2) as List<string>;
            bool photo_checked = (bool)((e.Argument as object[]).ElementAt(3)); 
            Dictionary<string, string> dicStudents = new Dictionary<string, string>();

            DataTable dataTable = Query.Select(this.GetQueryString());
            if (dataTable.Rows.Count == 0)
                throw new Exception("沒有資料。");

            foreach (DataRow x in dataTable.Rows.Cast<DataRow>().ToList())
            {
                if (dicStudents.ContainsKey(x["學生系統編號"] + ""))
                {
                    dataTable.Rows.Remove(x);
                    continue;
                }
                else
                    dicStudents.Add(x["學生系統編號"] + "", x["學生系統編號"] + "");

                x["住家地址"] = x["住家地址:郵遞區號"] + " " + x["住家地址:縣市"] + x["住家地址:鄉鎮"] + x["住家地址:村里"] + x["住家地址:鄰"] + x["住家地址:其它"];
                x["聯絡地址"] = x["聯絡地址:郵遞區號"] + " " + x["聯絡地址:縣市"] + x["聯絡地址:鄉鎮"] + x["聯絡地址:村里"] + x["聯絡地址:鄰"] + x["聯絡地址:其它"];
                x["公司地址"] = x["公司地址:郵遞區號"] + " " + x["公司地址:縣市"] + x["公司地址:鄉鎮"] + x["公司地址:村里"] + x["公司地址:鄰"] + x["公司地址:其它"];

                //x["工作狀態"] = ((x["工作狀態"] + "").ToUpper() == "TRUE" ? "是" : "否");

                string school_year = string.Empty;
                string semester = string.Empty;

                if ((x["畢業學年期"] + "").Length > 0 && (x["異動代碼"] + "") == "G")
                {
                    school_year = int.Parse((x["畢業學年期"] + "").Substring(0, 3)).ToString();
                    semester = (x["畢業學年期"] + "").Substring(3, 1);
                }

                x["畢業學年度"] = (school_year == "0" ? string.Empty : school_year);
                x["畢業學期"] = semester;

                DateTime date;
                if (DateTime.TryParse(x["生日"] + "", out date))
                    x["生日"] = date.ToString("yyyy/MM/dd");

                if (DateTime.TryParse(x["工作起日"] + "", out date))
                    x["工作起日"] = date.ToString("yyyy/MM/dd");

                if (DateTime.TryParse(x["工作迄日"] + "", out date))
                    x["工作迄日"] = date.ToString("yyyy/MM/dd");

                if ((x["性別"] + "") == "1")
                    x["性別"] = "男";

                if ((x["性別"] + "") == "0")
                    x["性別"] = "女";

                switch (x["學生狀態"] + "")
                {
                    case "1":
                        x["學生狀態"] = "在學";
                        break;
                    case "4":
                        x["學生狀態"] = "休學";
                        break;
                    case "64":
                        x["學生狀態"] = "退學";
                        break;
                    case "16":
                        x["學生狀態"] = "畢業";
                        break;
                    case "256":
                        x["學生狀態"] = "刪除";
                        break;
                    default:
                        x["學生狀態"] = "";
                        break;
                }
            }

            foreach (DataColumn column in dataTable.Columns.Cast<DataColumn>().ToList())
            {
                if (SelectedFields.Contains(column.ColumnName))
                    continue;

                if (photo_checked && column.ColumnName == "照片")
                    continue;

                dataTable.Columns.Remove(column);
            }

            Workbook wb = new Workbook();
            wb.Worksheets.Cast<Worksheet>().ToList().ForEach(x => wb.Worksheets.RemoveAt(x.Name));
            wb.Worksheets.Add();
            wb.Worksheets[0].Name = "學生基本資料";

            wb.Worksheets[0].Cells.ImportDataTable(dataTable, true, "A1");
            wb.Worksheets[0].AutoFitColumns();
            if (photo_checked)
            {            
                //  填入照片
                wb.Worksheets[0].Cells.SetColumnWidth(0, 10.38);
                for (int i = 0; i < dataTable.Rows.Count; i++)
                {
                    wb.Worksheets[0].Cells.SetRowHeight(i+1, 74);
                    if (string.IsNullOrWhiteSpace(dataTable.Rows[i][0] + ""))
                        continue;

                    byte[] imageData = Convert.FromBase64String(dataTable.Rows[i][0] + "");
                    MemoryStream ms = new MemoryStream(imageData);
                    wb.Worksheets[0].Pictures.Add(i+1, 0, i+2, 1, ms);
                    wb.Worksheets[0].Cells[i + 1, 0].PutValue(string.Empty);
                }
            }
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
                this.Visible = true;
                return;
            }
            else
            {
                SaveFileDialog sd = new SaveFileDialog();
                sd.Title = "另存新檔";
                sd.FileName = "匯出學生基本資料.xls";
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
                        this.Visible = true;
                        return;
                    }
                }
            }
            this.Close();
        }

        private void OnConfirmButtonClick(object sender, EventArgs e)
        {
            this.DialogResult = System.Windows.Forms.DialogResult.None;
            this.Execute();
        }
    }
}