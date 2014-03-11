using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using FISCA.Presentation.Controls;
using FISCA.UDT;
using FISCA.Data;
using System.Xml;
using System.Net.Mail;

namespace EMBACore.Forms
{
    public partial class CourseAttendance_EmailNotification : BaseForm
    {
        private Dictionary<string, UDT.Absence> dicAbsences = new Dictionary<string, UDT.Absence>();

        private string SenderEmail = "";
        private string SenderPassword = "";
        private string CC = "";

        public CourseAttendance_EmailNotification()
        {
            InitializeComponent();
        }

        private void btnQuery_Click(object sender, EventArgs e)
        {
            refreshData();
        }

        private void refreshData()
        {
            
            StringBuilder sb = new StringBuilder();
            sb.Append(string.Format("is_make_up=false AND starttime > '{0}' AND endtime < '{1}'", 
                                                        this.dtpFromDate.Value.ToString("yyyy/MM/dd 00:00:00"),
                                                        this.dtpToDate.Value.ToString("yyyy/MM/dd 23:59:59") ));
            if (this.checkBoxX1.Checked)
                sb.Append(" AND email_time is null") ;
            string whereCond = sb.ToString();

            /*
            //取得尚未送出通知的缺曠紀錄
            List<UDT.Absence> abs = (new AccessHelper()).Select<UDT.Absence>("email_time is null" );
            this.dicAbsences.Clear();
            foreach (UDT.Absence ab in abs)
                this.dicAbsences.Add(ab.UID, ab);
            */
            //取得尚未送出通知的缺曠紀錄及學生課程詳細資訊
            string strSQL = "select ab.uid, c.course_name, cext.subject_code, stud.student_number, stud.name , sec.starttime, ab.email_time, stu2.email_list, ab.target_email from $ischool.emba.absence ab inner join $ischool.course_calendar.section sec on sec.uid = ab.ref_section_id inner join student stud on stud.id = ab.ref_student_id inner join course c on c.id = ab.ref_course_id left outer join $ischool.emba.course_ext cext on ab.ref_course_id = cext.ref_course_id left outer join $ischool.emba.student_brief2 stu2 on stud.id = stu2.ref_student_id where " + whereCond;
            DataTable dt = (new QueryHelper()).Select(strSQL);
            this.dataGridViewX1.Rows.Clear();
            foreach (DataRow dr in dt.Rows)
            {
                string absID = dr["uid"].ToString();
                string email_list = dr["email_list"].ToString();
                DateTime dtStart = DateTime.Parse(dr["starttime"].ToString());
                object[] rawData = new object[] { dr["subject_code"].ToString(), dr["course_name"].ToString(),
                                                                dr["student_number"].ToString(), dr["name"].ToString(), parseEmail(email_list),
                                                                dtStart.ToString("yyyy/MM/dd"), dtStart.ToString("HH:mm") ,
                                                                (dr["email_time"] == null ? "" : dr["email_time"].ToString()),
                                                                (dr["target_email"] == null ? "" : dr["target_email"].ToString())};
                int rowIndex = this.dataGridViewX1.Rows.Add(rawData);
                this.dataGridViewX1.Rows[rowIndex].Tag = absID;
            }
        }

        private string parseEmail(string email_list)
        {
            string result = "";

            if (!string.IsNullOrWhiteSpace(email_list))
            {
                XmlDocument xmlDoc = new XmlDocument();
                try
                {
                    xmlDoc.LoadXml(string.Format("<emails>{0}</emails>", email_list));
                    XmlNode nd = xmlDoc.DocumentElement.SelectSingleNode("email1");
                    if (nd != null)
                        result = nd.InnerText;
                }
                catch (Exception ex)
                {
                }
            }
            return result;
        }

        private void btnSendEmail_Click(object sender, EventArgs e)
        {
            if (this.dataGridViewX1.SelectedRows.Count < 1)
            {
                Util.ShowMsg("請選擇要發送 Email 的缺曠紀錄！", "確定");
                return;
            }

            //彈出畫面指定寄件者
            if (string.IsNullOrWhiteSpace(this.SenderEmail))
            {
                Forms.Email_Credential frmCred = new Email_Credential();
                frmCred.AfterInputCredential += new Email_Credential.InputCredentialHandler(frmCred_AfterInputCredential);
                frmCred.ShowDialog();
            }

            //如果還是沒有指定寄件者，則離開
            if (string.IsNullOrWhiteSpace(this.SenderEmail))
                return;

            int rowCount = 0;
                        
            Dictionary<string, List<DataGridViewRow>> dicRows = new Dictionary<string,List<DataGridViewRow>>();
            List<string> uids = new List<string>();

            foreach (DataGridViewRow row in this.dataGridViewX1.SelectedRows)
            {
                string email = (row.Cells[4].Value == null) ? "" : row.Cells[4].Value.ToString();
                if (!string.IsNullOrWhiteSpace(email))
                {
                    string studNumber = row.Cells[2].Value.ToString();
                    string studName = row.Cells[3].Value.ToString();
                    string key = studNumber + "_" + studName;

                    if (!dicRows.ContainsKey(key))
                        dicRows.Add(key, new List<DataGridViewRow>());

                    dicRows[key].Add(row);
                    uids.Add(row.Tag.ToString());
                }
            }

            int totalCount = dicRows.Keys.Count;
            this.progressBarX1.Maximum = totalCount;
            this.progressBarX1.Visible = true;

            //Load UDT
            List<UDT.Absence> abs = (new AccessHelper()).Select<UDT.Absence>(uids);
            this.dicAbsences.Clear();
            foreach (UDT.Absence ab in abs)
                this.dicAbsences.Add(ab.UID, ab);

            //string template = UDT.AbsenceConfiguration.GetEmailContentTemplate().Content;
            string template = "[[Name]] 同學 ( [[StudentNumber]] ) 您好： <br/>";
            template += "<div  style='margin-left:30px;margin-top:30px;'>您有下列缺課紀錄：</div>";
            template += "<div style='margin-left:30px;' >[[Content]]</div>";
            template += UDT.AbsenceConfiguration.GetEmailContentTemplate().Content;


            //對於毎個學生
            foreach (string stuKey in dicRows.Keys)
            {
                DataGridViewRow row = dicRows[stuKey][0];
                string email = (row.Cells[4].Value == null) ? "" : row.Cells[4].Value.ToString();
                string studNumber = row.Cells[2].Value.ToString();
                string studName = row.Cells[3].Value.ToString();

                List<ActiveRecord> updatedRecs = new List<ActiveRecord>();

                StringBuilder sb = new StringBuilder();
                sb.Append("<table cellpadding='10' cellspacing='0' style='border : 1px solid blue;'><tr><td>課程名稱</td><td>課程識別碼</td><td>上課日期</td>上課時間<td></td></tr>");
                foreach (DataGridViewRow row2 in dicRows[stuKey])
                {                    
                    string subjectCode = row2.Cells[0].Value.ToString();
                    string courseName = row2.Cells[1].Value.ToString();
                    string occurDate = row2.Cells[5].Value.ToString();
                    string occurTime = row2.Cells[6].Value.ToString();
                    sb.Append("<tr>");
                    sb.Append(string.Format("<td>{0}</td>", courseName));
                    sb.Append(string.Format("<td>{0}</td>", subjectCode));
                    sb.Append(string.Format("<td>{0}</td>", occurDate));
                    sb.Append(string.Format("<td>{0}</td>", occurTime));
                    sb.Append("</tr>");

                    string absenceID = row2.Tag.ToString();
                    if (this.dicAbsences.ContainsKey(absenceID)) {
                        this.dicAbsences[absenceID].EmailTime = DateTime.Now.ToString("yyyy/MM/dd hh:mm:ss");
                        this.dicAbsences[absenceID].TargetEmail = email;
                        updatedRecs.Add(this.dicAbsences[absenceID]);
                    }
                }
                sb.Append("</table>");

                string emailBody = template.Replace("[[Name]]", studName).Replace("[[StudentNumber]]", studNumber).Replace("[[Content]]", sb.ToString());

                try
                {
                    rowCount += 1;
                    this.progressBarX1.Value = rowCount;

                    System.Net.Mail.MailMessage mail = new System.Net.Mail.MailMessage();

                    System.Net.NetworkCredential cred = new System.Net.NetworkCredential(this.SenderEmail, this.SenderPassword);

                    mail.To.Add(email);
                    mail.Subject = "台大 EMBA 缺課通知 (" + DateTime.Now.ToString("yyyy/MM/dd")  + ")"; ;
                    mail.From = new System.Net.Mail.MailAddress(this.SenderEmail);

                    //處理寄件備份
                    string[] ccs = this.CC.Split(new char[] { ',' });
                    foreach (string cc in ccs)
                    {
                        if (cc.Trim().Length > 0)
                        {
                            try
                            {
                                MailAddress copy = new MailAddress(cc);
                                mail.CC.Add(copy);
                            }
                            catch (Exception ex)
                            {
                                string msg = string.Format("『{0}』是不正確的電子郵件格式，是否仍繼續寄出信件？", cc);
                                if (MessageBox.Show(msg, "注意", MessageBoxButtons.YesNo, MessageBoxIcon.Information) == System.Windows.Forms.DialogResult.No)
                                {
                                    return; //停止作業
                                }
                            }
                        }
                    }                    
                    
                    mail.IsBodyHtml = true;
                    mail.Body = emailBody ;

                    System.Net.Mail.SmtpClient smtp = new System.Net.Mail.SmtpClient("smtp.gmail.com");
                    smtp.UseDefaultCredentials = false;
                    smtp.EnableSsl = true;
                    smtp.Credentials = cred;
                    smtp.Port = 587;
                    smtp.Send(mail);

                    //更新 email 時間
                    if (updatedRecs.Count > 0)
                    {
                        (new AccessHelper()).SaveAll(updatedRecs);
                    }
                }
                catch (Exception ex)
                {
                    Util.ShowMsg("寄送 Email 發生錯誤 \n" + ex.Message, "注意！");
                    break;
                }
                
            }
            this.progressBarX1.Visible = false;
            Util.ShowMsg("完成發送郵件通知", "");
            this.refreshData();
        }

        void frmCred_AfterInputCredential(object sender, EmailCredentialEventArgs args)
        {
            this.SenderEmail = args.UserID;
            this.SenderPassword = args.Password;
            this.CC = args.CC;
        }

        private void CourseAttendance_EmailNotification_Load(object sender, EventArgs e)
        {
            DateTime dt = DateTime.Now;
            this.dtpToDate.Value = dt;
            this.dtpFromDate.Value = dt.AddDays(-7);
        }
    }
}
