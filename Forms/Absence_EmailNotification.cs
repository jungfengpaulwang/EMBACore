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

namespace EMBACore.Forms
{
    public partial class Absence_EmailNotification : BaseForm
    {
        private AccessHelper Access;
        private QueryHelper Query;

        public Absence_EmailNotification()
        {
            InitializeComponent();

            Access = new AccessHelper();
            Query = new QueryHelper();
        }

        private DataGridViewColumn MakeColumn(int documentIndex)
        {
            DataGridViewColumn col = new DataGridViewLinkColumn();
            col.HeaderText = "文件名稱" + documentIndex;
            col.Name = "document" + documentIndex;
            col.Width = 60;
            col.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;
            col.AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            return col;
        }

        //建立 DataGrid 欄位
        private void SyncColumns(int DocumentCount)
        {
            int columnCount = this.dgvData.Columns.Count;
            for (int i = 3; i < columnCount; i++)
            {
                this.RemoveColumn(this.dgvData.Columns.Count - 1);
            }

            for (int i = 0; i < DocumentCount; i++)
            {
                int columnIndex = this.dgvData.Columns.Add(this.MakeColumn(i + 1));
                this.dgvData.Columns[columnIndex].SortMode = DataGridViewColumnSortMode.Automatic;
            }
        }

        private void RemoveColumn(int columnIndex)
        {
            this.dgvData.Columns.RemoveAt(columnIndex);
        }

        private void btnQuery_Click(object sender, EventArgs e)
        {
            this.refreshData();
        }

        private void refreshData()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append(string.Format("is_make_up=false AND starttime > '{0}' AND endtime < '{1}'",
                                                        this.dtpFromDate.Value.ToString("yyyy/MM/dd 00:00:00"),
                                                        this.dtpToDate.Value.ToString("yyyy/MM/dd 23:59:59")));
            if (this.checkBoxX1.Checked)
                sb.Append(" AND email_time is null");
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
            this.dgvData.Rows.Clear();
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
                int rowIndex = this.dgvData.Rows.Add(rawData);
                this.dgvData.Rows[rowIndex].Tag = absID;
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
    }
}
