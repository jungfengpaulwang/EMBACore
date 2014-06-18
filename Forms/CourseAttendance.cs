using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Security;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml;
using System.Xml.Linq;
using Aspose.Cells;
using DevComponents.DotNetBar;
using DevComponents.Editors;
using FISCA.Data;
using FISCA.Presentation.Controls;
using FISCA.UDT;
using Mandrill;

namespace EMBACore.Forms
{
    public delegate void CheckBoxClickedHandler(bool check);
    public partial class CourseAttendance : BaseForm
    {
        //  每個學生的電子郵件
        private Dictionary<string, List<dynamic>> dicStudentEmails = new Dictionary<string, List<dynamic>>();
        private Dictionary<string, string> dicCourses = new Dictionary<string, string>();

        private DataTable dtCourseSections;
        private Dictionary<string, DataRow> dicCourseSections;
        private Dictionary<string, string> dicStudents; //修課學生資料
        private List<int> SectionIDs;
        private DataTable dtSCAtts;

        private Dictionary<string, UDT.Absence> updatedRecs = new Dictionary<string, UDT.Absence>();
        private Dictionary<int, Dictionary<int, UDT.Absence>> dicAbsences;     // <studentid, <section_id, UDT.Absence>>

        private int selectedIndex = -1; //default selected index for combobox        
        private bool isInResetIndexProcess = false;     //是否正在重設 combobox index ;

        XmlDocument xmlSystemConfig;
        XmlElement elmSchoolYear;
        XmlElement elmSemester;

        private string SenderPassword = "";
        private string CC = "";
        private List<string> validated_cc;
        private string MandrillAPIKey = "";
        private string from_email = "";
        private string from_name = "";
        private string guid = string.Empty;
        private string user_account = FISCA.Authentication.DSAServices.UserAccount;
        private string email_category = "";
        private int SchoolYear;
        private int Semester;
        private DateTime time_stamp = DateTime.Now;
        private K12.Data.Configuration.ConfigData config;

        private AccessHelper Access;
        private QueryHelper Query;

        private UDT.CSConfiguration conf;
        private UDT.CSConfiguration conf_subject;
        private UDT.CSConfiguration conf_2;
        private UDT.CSConfiguration conf_subject_2;
        private UDT.CSConfiguration conf_3;
        private UDT.CSConfiguration conf_subject_3;
        private string TemplateName_Subfix = "course-attend-reminder";

        private bool form_loaded;

        private Dictionary<string, bool> dicChecks = new Dictionary<string, bool>();
        private Dictionary<string, List<UDT.MailLog>> dicMailLogs = new Dictionary<string, List<UDT.MailLog>>();

        private bool ColumnCellCheckBoxChecked;

        public CourseAttendance()
        {
            InitializeComponent();

            Access = new AccessHelper();
            Query = new QueryHelper();

            this.form_loaded = false;
            this.InitSchoolYearAndSemester();

            this.buttonX1.Popup(-1000, -1000);
            this.Load += new System.EventHandler(this.CourseAttendance_Load);
            this.FormClosing += new FormClosingEventHandler(Form_Closing);
            this.validated_cc = new List<string>();

            Task task = Task.Factory.StartNew(() =>
            {
                List<UDT.MandrillAPIKey> MandrillAPIKeys = Access.Select<UDT.MandrillAPIKey>();
                if (MandrillAPIKeys.Count > 0)
                    this.MandrillAPIKey = MandrillAPIKeys.ElementAt(0).APIKey;
                else
                    this.MandrillAPIKey = "";

                DataTable dataTable = Query.Select(string.Format(@"Select student.id as 學生系統編號, student.student_number as 學號, student.name as 學生姓名, student.sa_login_name as 登入帳號, (xpath_string('<root>' || sb2.email_list || '</root>','email1')) as 電子郵件一, (xpath_string('<root>' || sb2.email_list || '</root>','email2')) as 電子郵件二, (xpath_string('<root>' || sb2.email_list || '</root>','email3')) as 電子郵件三, (xpath_string('<root>' || sb2.email_list || '</root>','email4')) as 電子郵件四, (xpath_string('<root>' || sb2.email_list || '</root>','email5')) as 電子郵件五 From student Left join $ischool.emba.student_brief2 as sb2 on sb2.ref_student_id = student.id"));

                foreach (DataRow row in dataTable.Rows)
                {
                    if (!dicStudentEmails.ContainsKey(row["學生系統編號"] + ""))
                        dicStudentEmails.Add(row["學生系統編號"] + "", new List<dynamic>());

                    dicStudentEmails[row["學生系統編號"] + ""].Add(new               
                    { 
                        學號 = row["學號"] + "", 學生姓名 = row["學生姓名"] + "", 
                        登入帳號 = row["登入帳號"] + "", 
                        電子郵件一 = row["電子郵件一"] + "",
                        電子郵件二 = row["電子郵件二"] + "",
                        電子郵件三 = row["電子郵件三"] + "",
                        電子郵件四 = row["電子郵件四"] + "",
                        電子郵件五 = row["電子郵件五"] + ""
                    });                    
                }
                List<UDT.MailLog> MailLogs = Access.Select<UDT.MailLog>();
                foreach(UDT.MailLog MailLog in MailLogs)
                {
                    if (string.IsNullOrEmpty(MailLog.Extension))
                        continue;

                    if (MailLog.Status.ToLower() != "sent")
                        continue;

                    if (MailLog.IsCC)
                        continue;

                    string extension = (MailLog.Extension);
                    if (extension.StartsWith("![CDATA["))
                        extension = extension.Replace("![CDATA[", "");
                    if (extension.EndsWith("]]"))
                        extension = extension.Replace("]]", "");

                    XElement xElement = XElement.Parse(extension, LoadOptions.None);

                    string StudentID = string.Empty;
                    if (xElement.Element("Student") != null)
                        StudentID = xElement.Element("Student").Attribute("ID").Value;
                    string CourseID = string.Empty;
                    if (xElement.Element("Course") != null)
                        CourseID = xElement.Element("Course").Attribute("ID").Value;

                    if (string.IsNullOrEmpty(StudentID))
                        continue;
                    if (string.IsNullOrEmpty(CourseID))
                        continue;

                    string key = CourseID + "-" + StudentID;
                    if (!this.dicMailLogs.ContainsKey(key))
                        this.dicMailLogs.Add(key, new List<UDT.MailLog>());

                    this.dicMailLogs[key].Add(MailLog);
                }
            });
            task.ContinueWith((x) =>
            {
                this.btnSend.Enabled = true;
            }, TaskScheduler.FromCurrentSynchronizationContext());
        }

        private void InitSchoolYearAndSemester()
        {
            this.cboSemester.DataSource = EMBACore.DataItems.SemesterItem.GetSemesterList();
            this.cboSemester.ValueMember = "Value";
            this.cboSemester.DisplayMember = "Name";

            int DefaultSchoolYear;
            if (int.TryParse(K12.Data.School.DefaultSchoolYear, out DefaultSchoolYear))
            {
                this.nudSchoolYear.Value = decimal.Parse(DefaultSchoolYear.ToString());
            }
            else
            {
                this.nudSchoolYear.Value = decimal.Parse((DateTime.Today.Year - 1911).ToString());
            }

            this.cboSemester.SelectedValue = K12.Data.School.DefaultSemester;
        }

        private void Form_Closing(object sender, EventArgs e)
        {
            this.form_loaded = false;
        }

        private void CourseAttendance_Load(object sender, EventArgs e)
        {
            form_loaded = false;

            config = K12.Data.School.Configuration["台大EMBA缺課通知寄件人資訊"];
            if (config != null)
            {
                this.txtSenderEMail.Text = config["SenderEMail"];
                this.txtSenderName.Text = config["SenderName"];
                this.txtCC.Text = config["CC"];

                this.from_email = config["SenderEMail"];
                this.from_name = config["SenderName"];
                string[] ccs = config["CC"].Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
                foreach (string cc in ccs)
                {
                    if (cc.Trim().Length > 0)
                        this.validated_cc.Add(cc);
                }
            }

            this.SetConf();

            this.InitSemesterCourses();
        }

        private void InitSemesterCourses()
        {
            circularProgress.Visible = true;
            circularProgress.IsRunning = true;
            this.nudSchoolYear.Enabled = false;
            this.cboSemester.Enabled = false;
            this.cboCourse.Enabled = false;
            decimal school_year = this.nudSchoolYear.Value;
            string semester = this.cboSemester.SelectedValue.ToString();
            Task<DataTable> task = Task<DataTable>.Factory.StartNew(() =>
            {
                string strSQL = "select c.id, c.course_name, ce.subject_code from course c left outer join $ischool.emba.course_ext ce on c.id = ce.ref_course_id where c.school_year={0} and c.semester = {1} order by ce.subject_code, c.course_name";
                strSQL = string.Format(strSQL, school_year, semester);
                DataTable dt = this.Query.Select(strSQL);

                return dt;
            });
            task.ContinueWith((x) =>
            {
                this.circularProgress.Visible = false;
                this.circularProgress.IsRunning = false;
                this.nudSchoolYear.Enabled = true;
                this.cboSemester.Enabled = true;
                this.cboCourse.Enabled = true;
                form_loaded = true;
                if (x.Exception != null)
                {
                    MessageBox.Show(x.Exception.InnerException.Message);
                    return;
                }
                this.cboCourse.Items.Clear();
                this.dicCourses.Clear();
                foreach (DataRow dr in x.Result.Rows)
                {
                    string key = (dr["subject_code"] == null ? "" : dr["subject_code"].ToString()) + "  " + dr["course_name"].ToString();
                    ComboItem item = new ComboItem(key);
                    item.Tag = new { CourseName = dr["course_name"] + "", SchoolYear = school_year, Semester = semester, CourseID = dr["id"] + "" };
                    this.cboCourse.Items.Add(item);

                    this.dicCourses.Add(key, dr["id"].ToString());
                }
                if (this.cboCourse.Items.Count > 0)
                    this.cboCourse.SelectedIndex = 0;
                else
                    this.dg.Rows.Clear();

            }, System.Threading.CancellationToken.None, TaskContinuationOptions.None, TaskScheduler.FromCurrentSynchronizationContext());
        }

        private void SendMailButtionClick(ButtonItem button, int template_no)
        {
            if (this.cboCourse.SelectedIndex < 0)
            {
                MessageBox.Show("請先選擇課程。");
                return;
            }
            if (this.updatedRecs.Count > 0)
            {
                MessageBox.Show("請先儲存。");
                return;
            }
            this.btnSend.Enabled = false;
            this.circularProgress.Visible = true;
            this.circularProgress.IsRunning = true;

            try
            {
                object[] args = button.Tag as object[];
                MailLogBase MailLogBase = this.InitMailLog(args[1] as UDT.CSConfiguration, args[0] as UDT.CSConfiguration, args[2] as string);

                this.SendEMail(MailLogBase, template_no);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);

                this.btnSend.Enabled = true;
                this.circularProgress.Visible = false;
                this.circularProgress.IsRunning = false;
            }
        }
        

        private void SetConf()
        {
            conf = UDT.CSConfiguration.GetTemplate(this.TemplateName_Subfix + "-4");
            conf_subject = UDT.CSConfiguration.GetTemplate(this.TemplateName_Subfix + "-4_subject");

            conf_2 = UDT.CSConfiguration.GetTemplate(this.TemplateName_Subfix + "-5");
            conf_subject_2 = UDT.CSConfiguration.GetTemplate(this.TemplateName_Subfix + "-5_subject");

            conf_3 = UDT.CSConfiguration.GetTemplate(this.TemplateName_Subfix + "-3");
            conf_subject_3 = UDT.CSConfiguration.GetTemplate(this.TemplateName_Subfix + "-3_subject");

            this.btnSend.SubItems.Clear();

            ButtonItem button3 = new ButtonItem();
            button3.Text = "缺課3次通知";
            button3.Tag = new object[] { conf_3, conf_subject_3, button3.Text };
            button3.Click += (x, y) =>
            {
                this.SendMailButtionClick(button3, 3);
            };
            this.btnSend.SubItems.Add(button3);

            ButtonItem button = new ButtonItem();
            button.Text = "缺課4次通知";
            button.Tag = new object[] { conf, conf_subject, button.Text };
            button.Click += (x, y) =>
            {
                this.SendMailButtionClick(button, 4);
            };
            this.btnSend.SubItems.Add(button);

            ButtonItem button2 = new ButtonItem();
            button2.Text = "缺課5次通知";
            button2.Tag = new object[] { conf_2, conf_subject_2, button2.Text };
            button2.Click += (x, y) =>
            {
                this.SendMailButtionClick(button2, 5);
            };
            this.btnSend.SubItems.Add(button2);
        }

        private MailLogBase InitMailLog(UDT.CSConfiguration Conf_Subject, UDT.CSConfiguration Conf_Content, string EmailCategory)
        {            
            dynamic course = (this.cboCourse.Items[this.cboCourse.SelectedIndex] as ComboItem).Tag;
            int SchoolYear = int.Parse(course.SchoolYear + "");
            int Semester = int.Parse(course.Semester + "");
            string user_account = FISCA.Authentication.DSAServices.UserAccount;
            string Subject = Conf_Subject.Content;
            string Content = Conf_Content.Content;
            MailLogBase mailLogBase = new MailLogBase(SchoolYear, Semester, user_account, this.from_email, this.from_name, Subject, Content, EmailCategory, Guid.NewGuid().ToString());
            return mailLogBase;
        }

        private void SendEMail(MailLogBase MailLogBase, int AttendNo)
        {
            dynamic course = (this.cboCourse.Items[this.cboCourse.SelectedIndex] as ComboItem).Tag;

            List<string> StudentIDs = new List<string>();
            Dictionary<string, Dictionary<int, string>> dicSections = new Dictionary<string, Dictionary<int, string>>();
            Dictionary<string, string> dicAttendNos = new Dictionary<string, string>();
            List<string> ErrorMessages = new List<string>();
            foreach (DataGridViewRow row in this.dg.Rows)
            {
                if (row.IsNewRow)
                    continue;

                bool is_cancel = false;
                if (bool.TryParse(row.Cells[3].Value + "", out is_cancel) && is_cancel)
                    continue;

                string StudentID = row.Tag + "";

                bool bChecked = false;
                bool.TryParse(row.Cells[4].Value + "", out bChecked);

                int attend_no = 0;
                int.TryParse(row.Cells[2].Value + "", out attend_no);
                if ((attend_no < AttendNo) && bChecked)
                    ErrorMessages.Add(string.Format("學號「{0}」，姓名「{1}」，缺課次數「{2}」", row.Cells[0].Value + "", row.Cells[1].Value + "", row.Cells[2].Value + ""));

                if (!dicAttendNos.ContainsKey(StudentID))
                    dicAttendNos.Add(StudentID, attend_no.ToString());

                if (bChecked && this.dg.Columns.Count > 6)
                {
                    StudentIDs.Add(StudentID);

                    if (!dicSections.ContainsKey(StudentID))
                        dicSections.Add(StudentID, new Dictionary<int, string>());
                    
                    for (int j = 7; j < this.dg.Columns.Count; j++)
                    {
                        if (row.Cells[j].Value + "" != "缺")
                            continue;

                        object[] sections = this.dg.Columns[j].Tag as object[];
                        DateTime begin_time = DateTime.Parse(sections[0] + "");
                        DateTime end_time = DateTime.Parse(sections[1] + "");
                        string sDate = begin_time.Year.ToString().Length == 4 ? begin_time.ToString("yyyy/MM/dd") : (begin_time.Year + 1911) + "/" + begin_time.Month.ToString("00") + "/" + begin_time.Day.ToString("00");
                        string bTime =begin_time.Hour.ToString("00") + ":" + begin_time.Minute.ToString("00");
                        string eTime = end_time.Hour.ToString("00") + ":" + end_time.Minute.ToString("00");
                        string section_time = sDate + " " + bTime + "~" + eTime;

                        dicSections[StudentID].Add(j, section_time);
                    }
                }
            }
            if (ErrorMessages.Count > 0)
                throw new Exception(string.Format("待寄出之缺課通知為缺課{0}次通知，但下列學生缺課未達寄發之缺課次數：\n\n" + string.Join("\n", ErrorMessages), AttendNo));

            MandrillApi mandrill = new MandrillApi(this.MandrillAPIKey, false);

            string pong = string.Empty;

            try
            {
                pong = mandrill.Ping();
            }
            catch (Exception ex)
            {
                MessageBox.Show("未正確設定「MandrillAPIKey」, 錯誤訊息：" + ex.Message);
                this.btnSend.Enabled = true;
                this.circularProgress.Visible = false;
                this.circularProgress.IsRunning = false;
                return;
            }
            if (!pong.ToUpper().Contains("PONG!"))
            {
                MessageBox.Show("未正確設定「MandrillAPIKey」。");
                this.btnSend.Enabled = true;
                this.circularProgress.Visible = false;
                this.circularProgress.IsRunning = false;
                return;
            }

            Task<List<UDT.MailLog>> task = Task < List<UDT.MailLog>>.Factory.StartNew(() =>
            {
                List<UDT.MailLog> MandrillSendLogs = new List<UDT.MailLog>();
                this.SendEmails(StudentIDs, course, dicSections, dicAttendNos, MailLogBase, mandrill, MandrillSendLogs);
                return MandrillSendLogs;
            });
            task.ContinueWith((x) =>
            {
                this.btnSend.Enabled = true;
                this.circularProgress.Visible = false;
                this.circularProgress.IsRunning = false;

                if (x.Exception != null)
                    MessageBox.Show(x.Exception.InnerException.Message);
                else
                {
                    x.Result.SaveAll();
                    this.UpdateLogShow(x.Result);
                    MessageBox.Show("已寄出缺課通知。");
                }

            }, TaskScheduler.FromCurrentSynchronizationContext());
        }

        private void UpdateLogShow(List<UDT.MailLog> MandrillSendLogs)
        {
            if (MandrillSendLogs.Count == 0)
                return;

            dynamic course = (this.cboCourse.Items[this.cboCourse.SelectedIndex] as ComboItem).Tag;
            foreach(UDT.MailLog x in MandrillSendLogs)
            {
                if (x.IsCC)
                    continue;

                if (x.Status.ToLower() != "sent")
                    continue;

                if (!string.IsNullOrEmpty(x.Extension))
                {
                    string extension = x.Extension;
                    if (extension.StartsWith("![CDATA["))
                        extension = extension.Replace("![CDATA[", "");
                    if (extension.EndsWith("]]"))
                        extension = extension.Replace("]]", "");

                    XElement xElement = XElement.Parse(extension, LoadOptions.None);
                    string StudentID = string.Empty;
                    if (xElement.Element("Student") != null)
                        StudentID = xElement.Element("Student").Attribute("ID").Value;
                    string CourseID = string.Empty;
                    if (xElement.Element("Course") != null)
                        CourseID = xElement.Element("Course").Attribute("ID").Value;

                    if (!this.dicMailLogs.ContainsKey(CourseID + "-" + StudentID))
                        this.dicMailLogs.Add(CourseID + "-" + StudentID, new List<UDT.MailLog>());
                    
                    this.dicMailLogs[CourseID + "-" + StudentID].Add(x);
                }
            }
            foreach (DataGridViewRow row in this.dg.Rows)
            {
                string StudentID = row.Tag + "";
                string CourseID = course.CourseID + "";
                if (this.dicMailLogs.ContainsKey(CourseID + "-" + StudentID))
                {
                    List<UDT.MailLog> MailLogs = this.dicMailLogs[CourseID + "-" + StudentID];
                    MailLogs = MailLogs.OrderByDescending(x => x.TimeStamp).ToList();

                    row.Cells[5].Value = MailLogs[0].TimeStamp.ToString("yyyy/MM/dd hh:mm:ss");
                    row.Cells[6].Value = MailLogs[0].EmailCategory;
                }
            }
        }
        /// <summary>
        /// 驗證學生電子郵件是否合法
        /// </summary>
        /// <param name="StudentIDs"></param>
        /// <returns></returns>      
        private List<string> GetErrorEmails(List<string> StudentIDs)
        {
            List<string> ErrorEmails = new List<string>();

            StudentIDs.ForEach((x) =>
            {
                if (dicStudentEmails.ContainsKey(x))
                {
                    List<dynamic> emails = dicStudentEmails[x];
                    foreach (dynamic o in emails)
                    {
                        string student_number = o.學號 + "";
                        string student_name = o.學生姓名 + "";
                        string e0 = o.登入帳號 + "";
                        string e1 = o.電子郵件一 + "";
                        string e2 = o.電子郵件二 + "";
                        string e3 = o.電子郵件三 + "";
                        string e4 = o.電子郵件四 + "";
                        string e5 = o.電子郵件五 + "";

                        if (!string.IsNullOrEmpty(e0))
                        {
                            if (!this.isValidEmail(e0))
                                ErrorEmails.Add(string.Format("學號「{0}」，姓名「{1}」，登入帳號「{2}」。", student_number, student_name, e0));
                        }
                        if (!string.IsNullOrEmpty(e1))
                        {
                            if (!this.isValidEmail(e1))
                                ErrorEmails.Add(string.Format("學號「{0}」，姓名「{1}」，電子郵件一「{2}」。", student_number, student_name, e1));
                        }
                        if (!string.IsNullOrEmpty(e2))
                        {
                            if (!this.isValidEmail(e2))
                                ErrorEmails.Add(string.Format("學號「{0}」，姓名「{1}」，電子郵件二「{2}」。", student_number, student_name, e2));
                        }
                        if (!string.IsNullOrEmpty(e3))
                        {
                            if (!this.isValidEmail(e3))
                                ErrorEmails.Add(string.Format("學號「{0}」，姓名「{1}」，電子郵件三「{2}」。", student_number, student_name, e3));
                        }
                        if (!string.IsNullOrEmpty(e4))
                        {
                            if (!this.isValidEmail(e4))
                                ErrorEmails.Add(string.Format("學號「{0}」，姓名「{1}」，電子郵件四「{2}」。", student_number, student_name, e4));
                        }
                        if (!string.IsNullOrEmpty(e5))
                        {
                            if (!this.isValidEmail(e5))
                                ErrorEmails.Add(string.Format("學號「{0}」，姓名「{1}」，電子郵件五「{2}」。", student_number, student_name, e5));
                        }
                    }
                }
            });

            return ErrorEmails;
        }

        /// <summary>
        /// 批次寄送缺課通知
        /// </summary>
        /// <param name="StudentIDs"></param>
        /// <param name="Course"></param>
        /// <param name="dicSections"></param>
        /// <param name="dicAttendNos"></param>
        /// <param name="Subject"></param>
        /// <param name="Content"></param>
        /// <param name="template_name"></param>
        /// <param name="mandrill"></param>
        private void SendEmails(List<string> StudentIDs, dynamic Course, Dictionary<string, Dictionary<int, string>> dicSections, Dictionary<string, string> dicAttendNos, MailLogBase MailLogBase, MandrillApi mandrill, List<UDT.MailLog> MandrillSendLogs)
        {
            if (StudentIDs.Count == 0)
                throw new Exception("請先勾選學生。");

            List<string> ErrorEmails = this.GetErrorEmails(StudentIDs);
            if (ErrorEmails.Count > 0)
            {
                string error_email_alert_message = "下列學生及其電子郵件格式有誤，請先修正：\n\n";
                ErrorEmails.ForEach((x) => error_email_alert_message += (x + "\n"));

                throw new Exception(error_email_alert_message);
            }

            MailLog mail_log = new MailLog(false, MailLogBase);
            foreach (string StudentID in StudentIDs)
            {
                if (!this.dicStudentEmails.ContainsKey(StudentID))
                    continue;

                List<dynamic> emails = dicStudentEmails[StudentID];
                List<string> Emails = new List<string>();
                string StudentName = string.Empty;
                foreach (dynamic o in emails)
                {
                    string student_number = o.學號 + "";
                    StudentName = o.學生姓名 + "";
                    string e0 = o.登入帳號 + "";
                    string e1 = o.電子郵件一 + "";
                    string e2 = o.電子郵件二 + "";
                    string e3 = o.電子郵件三 + "";
                    string e4 = o.電子郵件四 + "";
                    string e5 = o.電子郵件五 + "";

                    if (!string.IsNullOrEmpty(e0))
                        Emails.Add(e0);
                    if (!string.IsNullOrEmpty(e1))
                        Emails.Add(e1);
                    if (!string.IsNullOrEmpty(e2))
                        Emails.Add(e2);
                    if (!string.IsNullOrEmpty(e3))
                        Emails.Add(e3);
                    if (!string.IsNullOrEmpty(e4))
                        Emails.Add(e4);
                    if (!string.IsNullOrEmpty(e5))
                        Emails.Add(e5);
                }
                mail_log = new MailLog(false, MailLogBase);
                mail_log.StudentID = StudentID;
                mail_log.CourseID = Course.CourseID + "";
                List<int> ColumnIndexs = dicSections[StudentID].Keys.ToList();
                ColumnIndexs.ForEach((x) => mail_log.AddSectionID(this.SectionIDs[x-7].ToString()));
                this.Emailing(mail_log, Emails.Distinct().ToList(), StudentName, Course.CourseName + "", dicAttendNos[StudentID], string.Join("、", dicSections[StudentID].Values), mandrill, MandrillSendLogs);

                if (this.validated_cc.Count() > 0)
                {
                    mail_log = new MailLog(true, MailLogBase);
                    mail_log.StudentID = StudentID;
                    mail_log.CourseID = Course.CourseID + "";
                    ColumnIndexs.ForEach((x) => mail_log.AddSectionID(this.SectionIDs[x - 7].ToString()));
                    this.Emailing(mail_log, this.validated_cc.Distinct().ToList(), StudentName, Course.CourseName + "", dicAttendNos[StudentID], string.Join("、", dicSections[StudentID].Values), mandrill, MandrillSendLogs);
                }
            }
        }

        private void Emailing(MailLog MailLog, List<string> Emails, string StudentName, string CourseName, string AttendNo, string AttendPeriod, MandrillApi mandrill, List<UDT.MailLog> MandrillSendLogs)
        {
            DateTime time_stamp = DateTime.Now;
            string email_subject =MailLog.MailLogBase.MailSubject;
            string email_body = MailLog.MailLogBase.MailContent;
            
            try
            {
                //  學年度、學期、開課、學生姓名、缺課次數、缺課時間                
                email_subject = email_subject.Replace("[[學年度]]", MailLog.MailLogBase.SchoolYear.ToString()).Replace("[[學期]]", DataItems.SemesterItem.GetSemesterByCode(MailLog.MailLogBase.Semester + "").Name).Replace("[[開課]]", CourseName).Replace("[[學生姓名]]", StudentName).Replace("[[缺課次數]]", AttendNo).Replace("[[缺課時間]]", AttendPeriod).Replace("[[上課總堂數]]", this.SectionIDs.Count.ToString());
                email_body = email_body.Replace("[[學年度]]", MailLog.MailLogBase.SchoolYear.ToString()).Replace("[[學期]]", DataItems.SemesterItem.GetSemesterByCode(MailLog.MailLogBase.Semester + "").Name).Replace("[[開課]]", CourseName).Replace("[[學生姓名]]", StudentName).Replace("[[缺課次數]]", AttendNo).Replace("[[缺課時間]]", AttendPeriod).Replace("[[上課總堂數]]", this.SectionIDs.Count.ToString());
                if (MailLog.IsCC)
                {
                    email_subject += "【副本】";
                }
                MailLog.MailSubject = email_subject;
                MailLog.MailContent = email_body;
                this.SendSingleMail(MailLog, Emails, mandrill, MandrillSendLogs);
            }
            catch (Exception ex)
            {
                //throw new Exception(ex.Message);
            }
            finally
            {
                //Access.InsertValues(MandrillSendLogs);
                //MandrillSendLogs.SaveAll();
            }
        }

        private void SendSingleMail(MailLog MailLog, IEnumerable<string> email_list, MandrillApi mandrill, List<UDT.MailLog> MandrillSendLogs)
        {
            if (email_list.Count() == 0)
                return;

            try
            {
                EmailMessage message = new EmailMessage();
                message.auto_text = true;
                message.from_email = this.from_email;
                message.from_name = this.from_name;

                List<EmailAddress> EmailAddresss = new List<EmailAddress>();
                foreach (string mail_to in email_list)
                {
                    EmailAddress mt = new EmailAddress();

                    mt.email = mail_to;
                    mt.name = string.Empty;

                    EmailAddresss.Add(mt);
                }
                message.to = EmailAddresss;

                message.track_clicks = true;
                message.track_opens = true;
                message.html = MailLog.MailContent;
                message.important = true;
                message.merge = false;
                message.preserve_recipients = true;
                message.subject = MailLog.MailSubject;

                MailLog.TimeStamp = DateTime.Now;

                List<EmailResult> results = mandrill.SendMessageSync(message);

                //  Log Email Result
                foreach (EmailResult result in results)
                {
                    MailLog.Result = "<Result>" +
                                                       "<NoneException>" +
                                                           "<Status>" + result.Status.ToString() + "</Status>" +
                                                           "<ResultID>" + result.Id + "</ResultID>" +
                                                           "<Email>" + result.Email + "</Email>" +
                                                           "<RejectReason>" + result.RejectReason + "</RejectReason>" +
                                                        "</NoneException>" +
                                                    "</Result>";
                    MailLog.RecipientEmailAddress = result.Email;
                    MailLog.Status = result.Status.ToString();
                    this.Log(MailLog, MandrillSendLogs);
                }
            }
            //  Log Email Error
            catch (Exception ex)
            {
                MandrillException error = (MandrillException)ex;

                MailLog.Result = "<Result>" +
                                                    "<Exception>" +
                                                        "<Status>" + error.Error.status + "</Status>" +
                                                        "<Code>" + error.Error.code + "</Code>" +
                                                        "<Name>" + error.Error.name + "</Name>" +
                                                        "<Message>" + error.Message + "</Message>" +
                                                        "<Source>" + error.Source + "</Source>" +
                                                        "<Email></Email>" +
                                                    "</Exception>" +
                                                "</Result>";
                MailLog.RecipientEmailAddress = string.Empty;
                MailLog.Status = error.Error.status;
                this.Log(MailLog, MandrillSendLogs);
            }
        }

        private void Log(MailLog MailLog, List<UDT.MailLog> MandrillSendLogs)
        {
            UDT.MailLog MandrillSendLog = new UDT.MailLog();

            MandrillSendLog.EmailCategory = MailLog.MailLogBase.EmailCategory;
            MandrillSendLog.GUID = MailLog.MailLogBase.GUID;
            MandrillSendLog.IsCC = MailLog.IsCC;
            MandrillSendLog.SchoolYear = MailLog.MailLogBase.SchoolYear;
            MandrillSendLog.Semester = MailLog.MailLogBase.Semester;
            MandrillSendLog.SenderEmail = MailLog.MailLogBase.SenderEmail;
            MandrillSendLog.SenderName = MailLog.MailLogBase.SenderName;
            MandrillSendLog.TimeStamp = MailLog.TimeStamp;
            MandrillSendLog.UserAccount = MailLog.MailLogBase.UserAccount;
            MandrillSendLog.MailContent = SecurityElement.Escape(MailLog.MailContent);
            MandrillSendLog.MailSubject = SecurityElement.Escape(MailLog.MailSubject);
            MandrillSendLog.RecipientEmailAddress = MailLog.RecipientEmailAddress;
            MandrillSendLog.Status = MailLog.Status;
            MandrillSendLog.Result = MailLog.Result;

            MandrillSendLog.Extension = string.Format("<Extension>" + (string.IsNullOrEmpty(MailLog.StudentID) ? "" :
                                                                                                ("<Student ID=\"" + MailLog.StudentID + "\"></Student>")) +
                                                                                                "<Course ID=\"" + MailLog.CourseID + "\"></Course>" +
                                                                                                "<Section ID=\"" + string.Join(",", MailLog.SectionIDs) + "\"></Section>" +
                                                                                            "</Extension>");

            MandrillSendLogs.Add(MandrillSendLog);
        }

        private void comboBoxEx1_SelectedIndexChanged(object sender, EventArgs e)
        {
            bool keep_refresh = true ;
            this.circularProgress.Visible = true;
            this.circularProgress.IsRunning = true;
            Application.DoEvents();
            if (this.isInResetIndexProcess)
            {
                this.isInResetIndexProcess = false;
                return;
            }

            if (this.updatedRecs.Count != 0)
            {
                if (MessageBox.Show("您有尚未儲存的資料，是否先儲存後再轉換到下一個課程？", "注意", MessageBoxButtons.YesNo, MessageBoxIcon.Information) == System.Windows.Forms.DialogResult.Yes)
                {
                    //先儲存
                    btnSave_Click(null, EventArgs.Empty);
                    this.refreshData();
                }
                else
                {
                    keep_refresh = false;
                    this.isInResetIndexProcess = true;
                    this.cboCourse.SelectedIndex = this.selectedIndex;
                }
            }
            else
                this.refreshData();

            this.circularProgress.Visible = false;
            this.circularProgress.IsRunning = false;
        }

        private void refreshData()
        {
            if (this.cboCourse.SelectedIndex < 0)
                return;

            string course_ID = this.dicCourses[this.cboCourse.SelectedItem.ToString()];
            this.selectedIndex = this.cboCourse.SelectedIndex;            

            QueryHelper qh = new QueryHelper();
            //1. 取得該課程的上課時間表
            string strSQL = string.Format("select * from $ischool.course_calendar.section where refcourseid = '{0}' and removed=false order by starttime asc ", course_ID);
            this.dtCourseSections = qh.Select(strSQL);
            //2. 取得修課學生清單
            strSQL = string.Format("select stud.student_number, stud.id, stud.name, is_cancel, ref_course_id from student stud inner join $ischool.emba.scattend_ext att on att.ref_student_id = stud.id where att.ref_course_id = {0} order by stud.student_number asc", course_ID);
            DataTable dtAtts = qh.Select(strSQL);
           
            
            //3. 以及學生曠課紀錄。
            List<UDT.Absence> atts = (new FISCA.UDT.AccessHelper()).Select<UDT.Absence>("ref_course_id=" + course_ID);
            this.dicAbsences = new Dictionary<int, Dictionary<int, UDT.Absence>>();
            foreach (UDT.Absence abs in atts)
            {
                if (!this.dicAbsences.ContainsKey(abs.StudentID))
                    this.dicAbsences.Add(abs.StudentID, new Dictionary<int, UDT.Absence>());

                if (!this.dicAbsences[abs.StudentID].ContainsKey(abs.SectionID))
                    this.dicAbsences[abs.StudentID].Add(abs.SectionID, abs);
                else
                    this.dicAbsences[abs.StudentID][abs.SectionID] = abs;
            }
            //4. 繪製UI
            //4.1 Create Columns            
            CreateColumns();
            //4.2 Fill Student and Absence Record.
            this.dicStudents = new Dictionary<string, string>();
            foreach (DataRow dr in dtAtts.Rows)
            {
                int studentID = int.Parse(dr["id"].ToString());
                string last_update_time = string.Empty;
                string mail_category = string.Empty;

                string key = course_ID + "-" + studentID;
                if (this.dicMailLogs.ContainsKey(key))
                {
                    List<UDT.MailLog> MailLogs = this.dicMailLogs[key];
                    MailLogs = MailLogs.OrderByDescending(x => x.TimeStamp).ToList();

                    last_update_time = MailLogs[0].TimeStamp.ToString("yyyy/MM/dd hh:mm:ss");
                    mail_category = MailLogs[0].EmailCategory;
                }

                List<object> rawData = new List<object>();                
                rawData.Add(dr["student_number"].ToString());
                rawData.Add(dr["name"].ToString());
                rawData.Add(string.Empty);
                rawData.Add(bool.Parse(dr["is_cancel"] + ""));
                bool bChecked = false;
                if (this.dicChecks.ContainsKey(course_ID + "-" + studentID.ToString()))
                    bChecked = this.dicChecks[course_ID + "-" + studentID.ToString()];
                rawData.Add(bChecked);
                rawData.Add(last_update_time);
                rawData.Add(mail_category);
                Dictionary<int, UDT.Absence> myAbs = null ;
                if (this.dicAbsences.ContainsKey(studentID))
                    myAbs = this.dicAbsences[studentID];
                if (!this.dicStudents.ContainsKey(studentID.ToString()))
                    this.dicStudents.Add(studentID.ToString(), dr["name"].ToString());

                foreach (int sectionID in this.SectionIDs)
                {
                    string content = "";
                    if (myAbs != null && myAbs.ContainsKey(sectionID))
                    {
                        UDT.Absence abs = myAbs[sectionID];
                        content = this.makeCellContent(abs);

                    }
                    rawData.Add(content);
                }

                int rowIndex = this.dg.Rows.Add(rawData.ToArray());
                this.dg.Rows[rowIndex].Tag = studentID;
            }
            CountAllRowPeriod();
            //4.3 initialized collections
            this.updatedRecs.Clear();
            this.enableButtons();

        }
        //  統計所有缺課節數，若4節以上底色為粉紅色
        private void CountAllRowPeriod()
        {
            foreach (DataGridViewRow row in this.dg.Rows)
                this.CountSingleRowPeriod(row);
        }
        //  統計單筆缺課節數，若4節以上底色為粉紅色
        private void CountSingleRowPeriod(DataGridViewRow row)
        {
            int count = 0;
            foreach (DataGridViewColumn column in this.dg.Columns)
            {
                if (column.Index < 7)
                    continue;

                if ((row.Cells[column.Index].Value + "").Trim() == "缺")
                    count += 1;
            }
            row.Cells[2].Value = (count == 0 ? string.Empty : count.ToString());
            if (count >= 4)
                row.DefaultCellStyle.BackColor = System.Drawing.Color.Pink;
            else
                row.DefaultCellStyle.BackColor = System.Drawing.Color.White;
        }
        //建立 DataGrid 欄位
        private void CreateColumns()
        {
            this.dg.Rows.Clear();
            this.dg.Columns.Clear();
            DataGridViewColumn colStudNumber = this.makeColumn("學號");
            colStudNumber.Frozen = true;
            colStudNumber.Width = 120;
            colStudNumber.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            this.dg.Columns.Add(colStudNumber);

            DataGridViewColumn colName = this.makeColumn("姓名");
            colName.Frozen = true;
            this.dg.Columns.Add(colName);
                        
            DataGridViewColumn colName_No = this.makeColumn("缺課次數");
            colName_No.Frozen = true;
            colName_No.ReadOnly = true;
            colName_No.Width = 40;
            this.dg.Columns.Add(colName_No);

            DataGridViewColumn colName_Cancel = new DataGridViewCheckBoxColumn();
            colName_Cancel.HeaderText = "停修";
            colName_Cancel.Width = 40;
            colName_Cancel.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            colName_Cancel.Frozen = true;
            colName_Cancel.ReadOnly = true;
            colName_Cancel.SortMode = DataGridViewColumnSortMode.Automatic;
            this.dg.Columns.Add(colName_Cancel);

            //寄發缺課通知(勾選)
            DataGridViewColumn colSendMail_Checker = new DevComponents.DotNetBar.Controls.DataGridViewCheckBoxXColumn();
            colSendMail_Checker.HeaderText = "寄發缺課通知";
            colSendMail_Checker.Width = 60;
            colSendMail_Checker.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            colSendMail_Checker.Frozen = true;
            colSendMail_Checker.ReadOnly = false;
            colSendMail_Checker.SortMode = DataGridViewColumnSortMode.Automatic;
            this.dg.Columns.Add(colSendMail_Checker);

            //最後寄發通知時間
            DataGridViewColumn colSendMail_Time = new DataGridViewTextBoxColumn();
            colSendMail_Time.HeaderText = "最後寄發\n通知時間";
            colSendMail_Time.Width = 150;
            colSendMail_Time.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            colSendMail_Time.Frozen = true;
            colSendMail_Time.ReadOnly = true;
            colSendMail_Time.SortMode = DataGridViewColumnSortMode.Automatic;
            this.dg.Columns.Add(colSendMail_Time);

            //最後寄發通知樣版
            DataGridViewColumn colSendMail_Template = new DataGridViewTextBoxColumn();
            colSendMail_Template.HeaderText = "最後寄發\n通知樣版";
            colSendMail_Template.Width = 100;
            colSendMail_Template.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            colSendMail_Template.Frozen = true;
            colSendMail_Template.ReadOnly = true;
            colSendMail_Template.SortMode = DataGridViewColumnSortMode.Automatic;
            this.dg.Columns.Add(colSendMail_Template);


            this.SectionIDs = new List<int>();
            this.dicCourseSections = new Dictionary<string, DataRow>();
            foreach(DataRow dr in this.dtCourseSections.Rows)
            {
                string start_date = DateTime.Parse(dr["starttime"].ToString()).ToString("MM/dd");
                string start_time = DateTime.Parse(dr["starttime"].ToString()).ToString("HH:mm");

                string text = start_date + "\r\n" + start_time;
                int column_index = this.dg.Columns.Add(this.makeColumn(text));

                DatagridViewCheckBoxHeaderCell cbHeader = new DatagridViewCheckBoxHeaderCell(this.Font); 
                cbHeader.OnCheckBoxClicked += new CheckBoxClickedHandler(cbHeader_OnCheckBoxClicked);
                //this.dg.Columns[column_index].Width = 120;
                this.dg.Columns[column_index].HeaderCell = cbHeader;
                //this.dg.Columns[column_index].AutoSizeMode = DataGridViewAutoSizeColumnMode.ColumnHeader;
                this.dg.Columns[column_index].HeaderText = text;
                this.dg.Columns[column_index].HeaderCell.Style.Alignment = DataGridViewContentAlignment.TopCenter;
                this.dg.Columns[column_index].SortMode = DataGridViewColumnSortMode.Programmatic;

                this.dg.Columns[column_index].Tag = new object[] { DateTime.Parse(dr["starttime"] + ""), DateTime.Parse(dr["endtime"] + "") };
                this.SectionIDs.Add(int.Parse(dr["uid"].ToString()));
                this.dicCourseSections.Add(dr["uid"].ToString(), dr);
            }
        }

        private void cbHeader_OnCheckBoxClicked(bool check)
        {
            this.ColumnCellCheckBoxChecked = true;
        }

        private DataGridViewColumn makeColumn(string text)
        {
            //DataGridViewCheckBoxColumn col = new DataGridViewCheckBoxColumn();
            //DatagridViewCheckBoxHeaderCell cbHeader = new DatagridViewCheckBoxHeaderCell();
            //col.HeaderCell = cbHeader;
            //col.HeaderText = text;
            //col.Width = 60;
            //col.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            //col.ReadOnly = true;
            //col.SortMode = DataGridViewColumnSortMode.Automatic;
            //return col;



            DataGridViewColumn col = new DataGridViewTextBoxColumn();
            col.HeaderText = text;
            //col.Name = colName;
            col.Width = 60;
            col.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            col.ReadOnly = true;
            col.SortMode = DataGridViewColumnSortMode.Automatic;
            return col;

        }

        private void dg_KeyUp(object sender, KeyEventArgs e)
        {
             if ((this.dg.CurrentCell == null) || (this.dg.CurrentCell.RowIndex < 0) || (this.dg.CurrentCell.ColumnIndex < 7))
                return;

             if  ((this.dg.CurrentCell.Value + "" != "") && (e.KeyCode == Keys.Delete))
                 removeAbsRec();
             else if ((this.dg.CurrentCell.Value + "" == "") && (e.KeyCode == Keys.A))
                 addAbsRec();

             this.CountSingleRowPeriod(this.dg.CurrentRow);
        }
        //刪除該筆缺曠紀錄
        private void removeAbsRec()
        {
            int studentID = (int)dg.CurrentCell.OwningRow.Tag;
            int sectionID = this.SectionIDs[this.dg.CurrentCell.ColumnIndex - 7];
            string studName = dg.CurrentCell.OwningRow.Cells[1].Value + "";
            string sectionName = dg.CurrentCell.OwningColumn.HeaderText .Replace("\n", " ");

            if (MessageBox.Show(string.Format("您確定要刪除『{0}』在『{1}』的缺課紀錄嗎？", studName, sectionName), "注意！", MessageBoxButtons.YesNo, MessageBoxIcon.Information) != System.Windows.Forms.DialogResult.Yes)
                return;

            //判斷這筆紀錄在 DB 中是否存在
            string key = studentID + "_" + sectionID;
            UDT.Absence absRec = this.GetAbsenceRecord(studentID, sectionID);
            //檢查記憶體中是否有這筆Record 
            if (this.updatedRecs.ContainsKey(key))
            {
                if (absRec == null) //如果 DB 中沒有，但記憶體中有，則直接從記憶體移除。                
                    this.updatedRecs.Remove(key);
                else  //如果 DB 有，記憶體中也有，則標示為刪除。
                    this.updatedRecs[key].Deleted = true;
            }
            else
            {
                if (absRec != null) //如果 DB 中有，但記憶體中沒有，則加入記憶體中並標註為 deleted。                
                {
                    absRec.Deleted = true;
                    this.updatedRecs.Add(key, absRec);
                }
                else  //如果 DB 沒有，記憶體中也沒有，則當作沒事。
                    this.updatedRecs[key].Deleted = true;
            }

            this.dg.CurrentCell.Value = "";
            this.dg.CurrentCell.Style.ForeColor = this.dg.CurrentCell.OwningColumn.DefaultCellStyle.ForeColor ;
            this.enableButtons();
        }

        private void addAbsRec()
        {
            int studentID = (int)dg.CurrentCell.OwningRow.Tag;
            int sectionID = this.SectionIDs[this.dg.CurrentCell.ColumnIndex - 7];
            int course_ID = int.Parse(this.dicCourses[this.cboCourse.SelectedItem.ToString()]);
            //判斷這筆紀錄在 DB 中是否存在
            string key = studentID + "_" + sectionID;
            UDT.Absence absRec = this.GetAbsenceRecord(studentID, sectionID);

            if (!this.updatedRecs.ContainsKey(key)) //判斷記憶體中是否存在。理論上應該不會存在才對，如果已存在，就忽略不理。
            {
                //如果 DB 有，而記憶體中沒有 (雖然不可能)，就加入
                if (absRec != null)
                {
                    absRec.Deleted = false;
                    this.updatedRecs.Add(key, absRec);
                }
                else   //如果 DB 有，而記憶體中沒有 (雖然不可能)，就加入
                {
                    UDT.Absence abs = new UDT.Absence();
                    abs.SectionID = sectionID;
                    abs.StudentID = studentID;
                    abs.CourseID = course_ID;
                    this.updatedRecs.Add(key, abs);
                }
            }
            else  //如果存在記憶體中，只有一種狀況 => 已存在 DB，然後畫面上被刪除，且尚未儲存，這時候該物件的 Deleted = false ,讓它變成修改
            {
                if (absRec != null)
                {
                    this.updatedRecs[key].Deleted = false;
                }
            }
            this.dg.CurrentCell.Value = makeCellContent(this.updatedRecs[key]);
            this.dg.CurrentCell.Style.ForeColor = Color.Red;
            this.enableButtons();
        }

        private void enableButtons()
        {
            this.btnSave.Enabled = (this.updatedRecs.Count > 0);
            this.btnCancel.Enabled = (this.updatedRecs.Count > 0); 
        }

        private UDT.Absence GetAbsenceRecord(int studentID, int sectionID)
        {
            UDT.Absence result = null;
            if (this.dicAbsences.ContainsKey(studentID))
                if (this.dicAbsences[studentID].ContainsKey(sectionID))
                    result = this.dicAbsences[studentID][sectionID];

            return result;
        }

        private string makeCellContent(UDT.Absence abs)
        {
            StringBuilder sb = new StringBuilder();
            if (abs != null)
            {
                if (abs.IsMakeUp) //有補課
                {

                    sb.Append("補(") ;
                    if (!string.IsNullOrWhiteSpace(abs.MakeUpDescription))
                    {
                        if (abs.MakeUpDescription.Length > 6)
                        {
                            sb.Append(abs.MakeUpDescription.Substring(0, 4));
                            sb.Append("...");
                        }
                        else
                            sb.Append(abs.MakeUpDescription);
                    }
                    sb.Append( ")");
                }
                else
                {
                    sb.Append("缺");
                }
            }

            return sb.ToString();
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            int course_ID = int.Parse(this.dicCourses[this.cboCourse.SelectedItem.ToString()]);
            foreach (DataGridViewRow row in this.dg.Rows)
            {
                if (row.IsNewRow)
                    continue;

                string StudentID = row.Tag + "";
                bool bChecked = false;
                bool.TryParse(row.Cells[4].Value + "", out bChecked);
                if (!this.dicChecks.ContainsKey(course_ID + "-" + StudentID))
                    this.dicChecks.Add(course_ID + "-" + StudentID, bChecked);
                else
                    this.dicChecks[course_ID + "-" + StudentID] = bChecked;
            }
            if (this.updatedRecs.Count > 0)
            {
                AccessHelper ah = new AccessHelper();
                List<ActiveRecord> recs = new List<ActiveRecord>();
                foreach (UDT.Absence abs in this.updatedRecs.Values)
                {  
                    recs.Add(abs);
                }

                bool isSaveOK = false;
                try
                {
                    ah.SaveAll(recs);
                    isSaveOK = true;


                    //add log
                    foreach (UDT.Absence abs in this.updatedRecs.Values)
                    {
                        string studName = this.dicStudents[abs.StudentID.ToString()];
                        string courseName = this.cboCourse.Text;
                        DataRow section = this.dicCourseSections[abs.SectionID.ToString()];
                        string start_time = section["starttime"].ToString();
                        if (abs.Deleted)
                        {
                            if (!string.IsNullOrWhiteSpace(abs.UID))
                            {
                                //紀錄刪除此筆缺曠紀錄
                                string msg = string.Format("刪除缺課紀錄: \n 學生：{0} \n 課程：{1}  \n 上課時間 : {2} \n   補課描述：\"{3}\" ", studName, courseName, start_time, abs.MakeUpDescription);
                                FISCA.LogAgent.ApplicationLog.Log("缺課紀錄.課程", "刪除", "course", abs.CourseID.ToString(), msg);
                            }
                        }
                        else
                        {
                            if (!string.IsNullOrWhiteSpace(abs.UID))
                            {
                                //紀錄修改此筆缺曠紀錄
                                string msg = string.Format("修改缺課紀錄: \n 學生：{0} \n 課程：{1}   \n 上課時間  : {2} \n   補課描述：\"{3}\" ", studName, courseName, start_time, abs.MakeUpDescription);
                                FISCA.LogAgent.ApplicationLog.Log("缺課紀錄.課程", "修改", "course", abs.CourseID.ToString(), msg);
                            }
                            else
                            {
                                //紀錄新增此筆缺曠紀錄
                                string msg = string.Format("新增缺課紀錄: \n 學生：{0} \n 課程：{1}   \n 上課時間  : {2} \n   補課描述：\"{3}\" ", studName, courseName, start_time, abs.MakeUpDescription);
                                FISCA.LogAgent.ApplicationLog.Log("缺課紀錄.課程", "新增", "course", abs.CourseID.ToString(), msg);
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    Util.ShowMsg("儲存缺課紀錄時發生錯誤", "注意");
                }

                if (isSaveOK)
                    this.refreshData();
            }
        }

        private void dg_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            if ((e.RowIndex < 0) || (e.ColumnIndex < 7))
                return;

            if (string.IsNullOrWhiteSpace(this.dg.Rows[e.RowIndex].Cells[e.ColumnIndex].Value + ""))
                return;

            int studentID = (int)dg.CurrentCell.OwningRow.Tag;
            int sectionID = this.SectionIDs[this.dg.CurrentCell.ColumnIndex - 7];
             string key = studentID + "_" + sectionID;

            UDT.Absence absRec = this.GetAbsenceRecord(studentID, sectionID);

            if (absRec == null)
            {
                if (this.updatedRecs.ContainsKey(key))
                    absRec = this.updatedRecs[key];
            }

            if (absRec != null)
            {
                CourseAttendance_Makeup frm = new CourseAttendance_Makeup();
                frm.SetAbsence(absRec , e.RowIndex, e.ColumnIndex);
                frm.AfterMakeUp += new CourseAttendance_Makeup.AfterMakeUpEventHandler(frm_AfterMakeUp);
                frm.ShowDialog();                
            }

        }

        void frm_AfterMakeUp(object sender, MakeUpEventArgs e)
        {
            if ((e.RowIndex < 0) || (e.ColumnIndex < 7))
                return;

            this.dg.Rows[e.RowIndex].Cells[e.ColumnIndex].Value = this.makeCellContent(e.AbsenceRecord);
            string key = e.AbsenceRecord.StudentID + "_" + e.AbsenceRecord.SectionID;
            if (!this.updatedRecs.ContainsKey(key))
                this.updatedRecs.Add(key, e.AbsenceRecord);
            else
                this.updatedRecs[key] = e.AbsenceRecord;

            this.dg.Rows[e.RowIndex].Cells[e.ColumnIndex].Style.ForeColor = Color.Red;

            this.enableButtons();
        }

        private void dg_CellMouseEnter(object sender, DataGridViewCellEventArgs e)
        {
            if ((e.RowIndex < 0) || (e.ColumnIndex < 7))
                return;

            bool hasContent =  (!string.IsNullOrWhiteSpace(this.dg.Rows[e.RowIndex].Cells[e.ColumnIndex].Value + ""));
            this.Cursor = (hasContent) ? Cursors.Hand : Cursors.Default;
        }

        private void dg_CellMouseLeave(object sender, DataGridViewCellEventArgs e)
        {
            if ((e.RowIndex < 0) || (e.ColumnIndex < 7))
                return;
            this.Cursor = Cursors.Default;
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            bool go_refresh = true  ;
            if (this.updatedRecs.Count > 0)
            {
                if (MessageBox.Show("是否要放棄已修改的內容？", "注意", MessageBoxButtons.YesNo, MessageBoxIcon.Information) != System.Windows.Forms.DialogResult.Yes) {
                    go_refresh = false ;
                }
            }
            if (go_refresh)
                this.refreshData();
        }

        private void btnExport_Click(object sender, EventArgs e)
        {
            if (this.updatedRecs.Count > 0)
            {
                MessageBox.Show("請先儲存。");
                return;
            }
            if (this.cboCourse.SelectedIndex < 0)
            {
                MessageBox.Show("請先選擇課程。");
                return;
            }
            dynamic course = (this.cboCourse.Items[this.cboCourse.SelectedIndex] as ComboItem).Tag;

            DataTable dataTable = new DataTable();
            this.dg.Columns.Cast<DataGridViewColumn>().ToList().ForEach(x => dataTable.Columns.Add(x.HeaderText));
            this.dg.Rows.Cast<DataGridViewRow>().ToList().ForEach(x =>
            {
                DataRow row = dataTable.NewRow();
                foreach (DataGridViewColumn Column in this.dg.Columns)
                {
                    string content = (x.Cells[Column.Index].Value + "").Trim();
                    if (content.ToLower() == "true")
                        content = "X";
                    else if (content.ToLower() == "false")
                        content = string.Empty;
                    row[Column.HeaderText] = content;
                }

                dataTable.Rows.Add(row);
            });

            Workbook wb = new Workbook();
            foreach (Worksheet sheet in wb.Worksheets.Cast<Worksheet>().ToList())
                wb.Worksheets.RemoveAt(sheet.Name);

            int sheet_index = wb.Worksheets.Add();
            wb.Worksheets[sheet_index].Cells.ImportDataTable(dataTable, true, "A1");

            wb.Worksheets.Cast<Worksheet>().ToList().ForEach(y => y.AutoFitColumns());
            SaveFileDialog sd = new SaveFileDialog();
            sd.Title = "另存新檔";
            sd.FileName = course.SchoolYear + "-" + course.Semester + "-" + course.CourseName.Trim() + "-缺課明細及節數統計.xls";
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

        private void buttonItem2_Click(object sender, EventArgs e)
        {
            this.txtSenderEMail.Text = this.from_email;
            this.txtSenderName.Text = this.from_name;
            this.txtCC.Text = string.Join(",", this.validated_cc);
            this.buttonX1.Expanded = false;
        }

        private bool isValidEmail(string email)
        {
            string patternStrict = @"^(([^<>()[\]\\.,;:\s@\""]+"
                       + @"(\.[^<>()[\]\\.,;:\s@\""]+)*)|(\"".+\""))@"
                       + @"((\[[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}"
                       + @"\.[0-9]{1,3}\])|(([a-zA-Z\-0-9]+\.)+"
                       + @"[a-zA-Z]{2,}))$";
            Regex reStrict = new Regex(patternStrict);

            return reStrict.IsMatch(email);
        }

        private void buttonItem1_Click(object sender, EventArgs e)
        {
            bool isOk = true;

            //Validate SenderEMail
            if (string.IsNullOrWhiteSpace(this.txtSenderEMail.Text))
            {
                this.errorProvider1.SetError(this.txtSenderEMail.TextBox, "必填。");
                isOk = false;
            }
            else
            {
                if (!this.isValidEmail(this.txtSenderEMail.Text.Trim()))
                {
                    this.errorProvider1.SetError(this.txtSenderEMail.TextBox, string.Format("不正確的電子郵件格式。"));
                    isOk = false;
                }
                else
                    this.errorProvider1.SetError(this.txtSenderEMail.TextBox, "");
            }

            //Validate SenderName
            if (string.IsNullOrWhiteSpace(this.txtSenderName.Text))
            {
                this.errorProvider1.SetError(this.txtSenderName.TextBox, "必填。");
                isOk = false;
            }
            else
            {
                this.errorProvider1.SetError(this.txtSenderName.TextBox, "");
            }

            //  驗證副本是否符合電子郵件格式
            string[] ccs = this.txtCC.Text.Trim().Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
            List<string> err_emails = new List<string>();
            foreach (string cc in ccs)
            {
                if (cc.Trim().Length > 0)
                {
                    if (!this.isValidEmail(cc))
                        err_emails.Add(cc);
                }
            }
            if (err_emails.Count > 0)
            {
                this.errorProvider1.SetError(this.txtCC.TextBox, string.Format("副本『{0}』是不正確的電子郵件格式。", string.Join(",", err_emails)));
                isOk = false;
            }
            else
                this.errorProvider1.SetError(this.txtCC.TextBox, "");

            if (isOk)
            {
                this.from_email = this.txtSenderEMail.Text.Trim();
                this.from_name = this.txtSenderName.Text.Trim();
                this.validated_cc = ccs.ToList();

                if (config != null)
                {
                    config["SenderEMail"] = this.txtSenderEMail.Text.Trim();
                    config["SenderName"] = this.txtSenderName.Text.Trim();
                    config["CC"] = this.txtCC.Text.Trim();

                    config.Save();
                }
                this.lblMessage.Text = "儲存成功";
                Timer timer = new Timer();
                timer.Enabled = true;
                timer.Interval = 3000;
                timer.Tick += (x, y) =>
                {
                    this.lblMessage.Text = string.Empty;
                    timer.Enabled = false;
                };
                buttonX1.Expanded = false;
            }
        }

        private void btnExit_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void nudSchoolYear_ValueChanged(object sender, EventArgs e)
        {
            if (this.form_loaded)
                this.InitSemesterCourses();
        }

        private void cboSemester_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (this.form_loaded)
                this.InitSemesterCourses();
        }

        private void dg_ColumnHeaderMouseClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            if (this.dg.Columns[e.ColumnIndex].HeaderCell.GetType().Name != "DatagridViewCheckBoxHeaderCell")
                return;

            if (!this.ColumnCellCheckBoxChecked)
            {
                string direction = this.dg.Columns[e.ColumnIndex].HeaderCell.Tag + "";
                if (string.IsNullOrEmpty(direction))
                {
                    this.dg.Sort(this.dg.Columns[e.ColumnIndex], System.ComponentModel.ListSortDirection.Descending);
                    this.dg.Columns[e.ColumnIndex].HeaderCell.Tag = System.ComponentModel.ListSortDirection.Descending;
                }
                else
                {
                    if (direction.ToLower() == "ascending")
                    {
                        this.dg.Sort(this.dg.Columns[e.ColumnIndex], System.ComponentModel.ListSortDirection.Descending);
                        this.dg.Columns[e.ColumnIndex].HeaderCell.Tag = "descending";
                    }
                    else
                    {
                        this.dg.Sort(this.dg.Columns[e.ColumnIndex], System.ComponentModel.ListSortDirection.Ascending);
                        this.dg.Columns[e.ColumnIndex].HeaderCell.Tag = "ascending";
                    }
                }
            }
            else
            {
                this.ColumnCellCheckBoxChecked = false;
            }
        }
    }

    public class MailLogBase
    {
        public MailLogBase(int SchoolYear, int Semester, string UserAccount, string SenderEmail, string SenderName, string MailSubject, string MailContent, string EmailCategory, string GUID)
        {
            this.SchoolYear = SchoolYear;
            this.Semester = Semester;
            this.UserAccount = UserAccount;
            this.SenderEmail = SenderEmail;
            this.SenderName = SenderName;
            this.MailSubject = MailSubject;
            this.MailContent = MailContent;
            this.EmailCategory = EmailCategory;
            this.GUID = GUID;
        }

        /// <summary>
        /// 學年度
        /// </summary>
        public int SchoolYear { get; set; }

        /// <summary>
        /// 學期
        /// </summary>
        public int Semester { get; set; }

        /// <summary>
        /// 寄信帳號
        /// </summary>
        public string UserAccount { get; set; }

        /// <summary>
        /// 寄件人電子郵件
        /// </summary>
        public string SenderEmail { get; set; }

        /// <summary>
        /// 寄件人名稱
        /// </summary>
        public string SenderName { get; set; }

        /// <summary>
        /// 信件主旨
        /// </summary>
        public string MailSubject { get; set; }

        /// <summary>
        /// 信件內容
        /// </summary>
        public string MailContent { get; set; }

        /// <summary>
        /// 信件類別：發送 Email 提醒通知、發送 Email 再次提醒通知、缺課4次通知、缺課5次通知
        /// </summary>
        public string EmailCategory { get; set; }

        /// <summary>
        /// 作業代碼
        /// </summary>
        public string GUID { get; set; }
    }

    public class MailLog
    {
        public MailLog(bool IsCC, MailLogBase MailLogBase)
        {
            this.IsCC = IsCC;
            this.MailLogBase = MailLogBase;

            this.SectionIDs = new List<string>();
        }

        /// <summary>
        /// 通用資料
        /// </summary>
        public MailLogBase MailLogBase { get; set; }

        /// <summary>
        /// 信件主旨
        /// </summary>
        public string MailSubject { get; set; }

        /// <summary>
        /// 信件內容
        /// </summary>
        public string MailContent { get; set; }

        /// <summary>
        /// 收件人電子郵件
        /// </summary>
        public string RecipientEmailAddress { get; set; }

        /// <summary>
        /// 是否為副本
        /// </summary>
        public bool IsCC { get; set; }

        /// <summary>
        /// 寄信時間
        /// </summary>
        public DateTime TimeStamp { get; set; }

        /// <summary>
        /// 寄信結果代碼
        /// </summary>
        public string Status { get; set; }
 
        /// <remarks>
        ///<Result>
        ///     <NoneException>
        ///         <Status></Status>
        ///         <ResultID></ResultID>
        ///         <Email></Email>
        ///     </NoneException>
        ///     <Exception>
        ///         <Status></Status>
        ///         <Code></Code>
        ///         <Name></Name>
        ///         <Message></Message>
        ///         <Source></Source>
        ///         <RejectReason></RejectReason>
        ///         <Email></Email>
        ///     </Exception>
        ///</Result>
        /// </remarks>
        /// <summary>
        /// 寄信結果
        /// </summary>
        public string Result { get; set; }

        public string StudentID { get; set; }

        public string CourseID { get; set; }

        public List<string> SectionIDs { get; private set; }

        public void AddSectionID(string SectionID)
        {
            this.SectionIDs.Add(SectionID);
        }
    }
    public class DataGridViewCheckBoxHeaderCellEventArgs : EventArgs
    {
        bool _bChecked;
        public DataGridViewCheckBoxHeaderCellEventArgs(bool bChecked)
        {
            _bChecked = bChecked;
        }
        public bool Checked
        {
            get { return _bChecked; }
        }
    }
    public class DatagridViewCheckBoxHeaderCell : DataGridViewColumnHeaderCell
    {
        Point checkBoxLocation;
        Size checkBoxSize;
        bool _checked = false;
        Point _cellLocation = new Point();
        System.Windows.Forms.VisualStyles.CheckBoxState _cbState =
            System.Windows.Forms.VisualStyles.CheckBoxState.UncheckedNormal;
        public event CheckBoxClickedHandler OnCheckBoxClicked;
        private System.Drawing.Font CellFont;

        public DatagridViewCheckBoxHeaderCell(System.Drawing.Font CellFont)
        {
            this.CellFont = CellFont;
        }

        protected override void Paint(System.Drawing.Graphics graphics,
            System.Drawing.Rectangle clipBounds,
            System.Drawing.Rectangle cellBounds,
            int rowIndex,
            DataGridViewElementStates dataGridViewElementState,
            object value,
            object formattedValue,
            string errorText,
            DataGridViewCellStyle cellStyle,
            DataGridViewAdvancedBorderStyle advancedBorderStyle,
            DataGridViewPaintParts paintParts)
        {
            base.Paint(graphics, clipBounds, cellBounds, rowIndex,
                dataGridViewElementState, value,
                formattedValue, errorText, cellStyle,
                advancedBorderStyle, paintParts);

            Point p = new Point();
            Size s = CheckBoxRenderer.GetGlyphSize(graphics,
            System.Windows.Forms.VisualStyles.CheckBoxState.UncheckedNormal);
            p.X = cellBounds.Location.X +
                (cellBounds.Width / 2) - (s.Width / 2);
            p.Y = cellBounds.Location.Y +
                (cellBounds.Height / 2) - (s.Height / 2) + 20;
            _cellLocation = cellBounds.Location;
            checkBoxLocation = p;
            checkBoxSize = new Size(s.Width, s.Height);
            if (_checked)
                _cbState = System.Windows.Forms.VisualStyles.
                    CheckBoxState.CheckedNormal;
            else
                _cbState = System.Windows.Forms.VisualStyles.
                    CheckBoxState.UncheckedNormal;
            System.Drawing.Rectangle CellTextBounds = new Rectangle();
            CellTextBounds.X = p.X;
            CellTextBounds.Y = p.Y;
            CellTextBounds.Width = int.MaxValue;
            CellTextBounds.Height = int.MaxValue;

            CheckBoxRenderer.DrawCheckBox
            (graphics, checkBoxLocation, CellTextBounds, string.Empty, this.CellFont, false, _cbState);
        }

        protected override void OnMouseClick(DataGridViewCellMouseEventArgs e)
        {
            Point p = new Point(e.X + _cellLocation.X, e.Y + _cellLocation.Y);
            //Point p = new Point(e.X, e.Y);
            if (p.X >= checkBoxLocation.X && p.X <=
                checkBoxLocation.X + checkBoxSize.Width
            && p.Y >= checkBoxLocation.Y && p.Y <=
                checkBoxLocation.Y + checkBoxSize.Height)
            {
                _checked = !_checked;
                if (OnCheckBoxClicked != null)
                {
                    OnCheckBoxClicked(_checked);
                    this.DataGridView.InvalidateCell(this);
                }
                //base.OnMouseClick(e);
                //MessageBox.Show("CheckBoxMouseClick");
            }
        }
    }
}