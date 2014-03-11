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
using DevComponents.Editors;
using FISCA.Data;
using System.Threading.Tasks;
using System.Dynamic;
using Aspose.Cells;
using System.Xml.Linq;
using Mandrill;

namespace EMBACore.Forms
{
    public partial class frmQueryCourseAttendance_EmailNotification : BaseForm
    {
        private AccessHelper Access;
        private QueryHelper Query;

        private Dictionary<string, Absence> dicAbsences;
        private Dictionary<string, List<UDT.MailLog>> dicMailLogs;

        private bool data_loaded;

        public frmQueryCourseAttendance_EmailNotification()
        {
            InitializeComponent();

            this.circularProgress.Visible = true;
            this.circularProgress.IsRunning = true;
            this.txtFilterSnum.Enabled = false;
            this.cboCourse.Enabled = false;

            Access = new AccessHelper();
            Query = new QueryHelper();

            dicAbsences = new Dictionary<string, Absence>();
            dicMailLogs = new Dictionary<string, List<UDT.MailLog>>();

            List<Action> Tasks = new List<Action>() { this.InitDataSource };
            Task task = Task.Factory.StartNew(() =>
            {
                Parallel.ForEach(Tasks, x => x.Invoke());
            }).ContinueWith(x => 
            { 
                this.InitDate();
                this.InitCourseDataSource();
                this.DataLoadComplete();
                this.DGV_DataBinding(true);

                this.circularProgress.Visible = false;
                this.circularProgress.IsRunning = false;
                this.txtFilterSnum.Enabled = true;
                this.cboCourse.Enabled = true;
                this.dtBeginDate.Enabled = true;
                this.dtEndDate.Enabled = true;
            }, TaskScheduler.FromCurrentSynchronizationContext());

            this.Load += new EventHandler(Form_Load);
        }

        private void dgvData_MouseClick(object sender, MouseEventArgs e)
        {
            DataGridView dgv = (DataGridView)sender;
            DataGridView.HitTestInfo hit = dgv.HitTest(e.X, e.Y);

            if (hit.Type == DataGridViewHitTestType.TopLeftHeader)
            {
                dgv.SelectAll();
            }
        }

        private void Form_Load(object sender, EventArgs e)
        {

        }

        //  改用 Mandrill Webhook 
        private void UpdateSendMailStatus()
        {
            string MandrillAPIKey = string.Empty;
            List<UDT.MandrillAPIKey> MandrillAPIKeys = Access.Select<UDT.MandrillAPIKey>();
            if (MandrillAPIKeys.Count > 0)
                MandrillAPIKey = MandrillAPIKeys.ElementAt(0).APIKey;
            else
                return;

            MandrillApi mandrill = new MandrillApi(MandrillAPIKey, false);

            string pong = string.Empty;

            try
            {
                pong = mandrill.Ping();
            }
            catch (Exception ex)
            {
                return;
            }
            if (!pong.ToUpper().Contains("PONG!"))
            {
                return;
            }

            string SQL = "select * from $ischool.emba.mail_log where (status ilike 'Sent' or status ilike 'Queued') and ((time_stamp + '7'<update_status_time) or update_status_time is null)";

            DataTable dataTable = Query.Select(SQL);
            foreach (DataRow x in dataTable.Rows)
            {
                string Result = x["result"] + "";
                if (Result.StartsWith("![CDATA["))
                    Result = Result.Replace("![CDATA[", "");
                if (Result.EndsWith("]]"))
                    Result = Result.Replace("]]", "");

                XElement xElement = XElement.Parse(Result, LoadOptions.None);
                if (xElement.Descendants("ResultID") != null)
                {
                    string ResultID = xElement.Descendants("ResultID").ElementAt(0).Value;
                    Info info = new Info();
                    info.id = ResultID;
                    dynamic result = mandrill.SendInfoMessageSync(info);

                    List<UDT.MailLog> MailLogs = Access.Select<UDT.MailLog>(new List<string>() { x["uid"] + "" });
                    MailLogs.ElementAt(0).UpdateStatusTime = DateTime.Now;
                    if ((x["status"] + "").ToLower() != ((string)result.state).ToLower())
                    {
                        MailLogs.ElementAt(0).Status = result.state + "";
                    }
                    MailLogs.SaveAll();
                }
            }
        }

        private void InitDate()
        {
            DateTime minDate = this.dicAbsences.Values.OrderBy(x => x.BeginTime).ElementAt(0).BeginTime;
            DateTime maxDate = this.dicAbsences.Values.OrderByDescending(x => x.EndTime).ElementAt(0).EndTime;
            this.dtBeginDate.MinDate = minDate;
            this.dtBeginDate.MaxDate = maxDate;
            this.dtEndDate.MinDate = minDate;
            this.dtEndDate.MaxDate = maxDate;
            this.dtBeginDate.Value = minDate;
            this.dtEndDate.Value = maxDate;
        }

        private void DataLoadComplete()
        {
            this.data_loaded = true;
        }

        private void InitCourseDataSource()
        {
            string oCourseID = string.Empty;
            if (this.cboCourse.SelectedItem != null)
                oCourseID = (this.cboCourse.SelectedItem as ComboItem).Tag + "";
            DateTime minDate = new DateTime[] { this.dtBeginDate.Value, this.dtEndDate.Value }.Min();
            DateTime maxDate = new DateTime[] { this.dtBeginDate.Value, this.dtEndDate.Value }.Max();

            this.cboCourse.Items.Clear();
            this.cboCourse.Items.Add(new ComboItem());
            List<Absence> Absences = this.dicAbsences.Values.OrderByDescending(x => x.SchoolYear).ThenByDescending(x => x.Semester).ToList();
            Dictionary<string, string> dicCourseIDs = new Dictionary<string, string>();
            foreach (Absence absence in Absences)
            {
                if (absence.BeginTime<minDate || absence.EndTime > maxDate)
                    continue;

                if (dicCourseIDs.ContainsKey(absence.CourseID))
                    continue;
                else
                    dicCourseIDs.Add(absence.CourseID, absence.CourseID);

                int idx = this.cboCourse.Items.Add(new ComboItem(absence.CourseName));
                (this.cboCourse.Items[idx] as ComboItem).Tag = absence.CourseID;

                if (absence.CourseID == oCourseID)
                    this.cboCourse.SelectedItem = this.cboCourse.Items[idx];
            }

            this.cboCourse.AutoCompleteMode = AutoCompleteMode.Suggest;
            this.cboCourse.AutoCompleteSource = AutoCompleteSource.ListItems;
        }

        private void DGV_DataBinding(bool DoEventRequired = false)
        {
            if (!this.data_loaded)
                return;

            this.dgvData.Rows.Clear();

            DateTime minDate = new DateTime[] { this.dtBeginDate.Value, this.dtEndDate.Value }.Min();
            DateTime maxDate = new DateTime[] { this.dtBeginDate.Value, this.dtEndDate.Value }.Max();

            string strFilter_SNum = this.txtFilterSnum.Text.Trim();
            string strFilter_Course_ID = string.Empty;

            if (this.cboCourse.SelectedItem != null)
                strFilter_Course_ID = (this.cboCourse.SelectedItem as ComboItem).Tag + "";

            List<Absence> Absences = this.dicAbsences.Values.OrderByDescending(x => x.SchoolYear).ThenByDescending(x => x.Semester).OrderBy(x => x.StudentNumber).OrderBy(x => x.CourseID).OrderByDescending(x => x.BeginTime).OrderByDescending(x => x.EndTime).ToList();

            foreach (Absence absence in Absences)
            {
                if (!string.IsNullOrEmpty(strFilter_SNum))
                {
                    if (!absence.StudentNumber.Contains(strFilter_SNum) && !absence.Name.Contains(strFilter_SNum))
                        continue;
                }
                if (!string.IsNullOrEmpty(strFilter_Course_ID))
                {
                    if (absence.CourseID != strFilter_Course_ID)
                        continue;
                }
                if (absence.BeginTime < minDate || absence.EndTime > maxDate)
                    continue;

                List<object> sources = new List<object>();

                sources.Add(absence.StudentNumber);
                sources.Add(absence.Name);
                sources.Add(absence.SchoolYear);
                sources.Add(absence.Semester);
                sources.Add(absence.NewSubjectCode);
                sources.Add(absence.CourseName);
                sources.Add(absence.BeginTime.ToString("yyyy/MM/dd"));
                sources.Add(absence.BeginTime.ToString("HH:mm") + "~" + absence.EndTime.ToString("HH:mm"));
                sources.Add(absence.MakeUpDescription);

                //if (absence.StudentID != "10668")
                //    continue;
                //if (absence.CourseID != "540")
                //    continue;

                string key = absence.CourseID + "-" + absence.StudentID + "-" + absence.SectionID;
                if (this.dicMailLogs.ContainsKey(key))
                {
                    List<UDT.MailLog> MailLogs = this.dicMailLogs[key];
                    sources.Add(MailLogs.Max(x=>x.TimeStamp).ToString("yyyy/MM/dd HH:mm:ss"));
                    sources.Add(MailLogs.OrderByDescending(x=>x.TimeStamp).ElementAt(0).EmailCategory);
                    sources.Add(string.Join("、", MailLogs.Select(x=>x.RecipientEmailAddress).Distinct()));
                }
                else
                {
                    sources.Add(string.Empty);
                    sources.Add(string.Empty);
                    sources.Add(string.Empty);
                }

                int idx = this.dgvData.Rows.Add(sources.ToArray());
                this.dgvData.Rows[idx].Tag = absence;

                if (DoEventRequired)
                    Application.DoEvents();
            }
        }

        private void InitDataSource()
        {
            string SQL = "select stud.student_number, stud.name, cext.new_subject_code, c.course_name, sec.starttime, sec.endtime, sec.uid, c.id, c.school_year, c.semester, stud.id as student_id, ab.make_up_description from $ischool.emba.absence ab " +
                                    "inner join $ischool.course_calendar.section sec on sec.uid = ab.ref_section_id " +
                                    "inner join student stud on stud.id = ab.ref_student_id " +
                                    "inner join course c on c.id = ab.ref_course_id " +
                                    "left outer join $ischool.emba.course_ext cext on ab.ref_course_id = cext.ref_course_id";

            DataTable dataTable = Query.Select(SQL);

            foreach (DataRow row in dataTable.Rows)
            {
                Absence absence = new Absence();

                absence.StudentNumber = row["student_number"] + "";
                absence.Name = row["name"] + "";
                absence.NewSubjectCode = row["new_subject_code"] + "";
                absence.CourseName = row["course_name"] + "";
                absence.BeginTime = DateTime.Parse(row["starttime"] + "");
                absence.EndTime = DateTime.Parse(row["endtime"] + "");
                absence.SectionID = row["uid"] + "";
                absence.CourseID = row["id"] + "";
                absence.SchoolYear = int.Parse(row["school_year"] + "");
                absence.Semester = int.Parse(row["semester"] + "");
                absence.StudentID = row["student_id"] + "";
                absence.MakeUpDescription = row["make_up_description"] + "";

                if (!this.dicAbsences.ContainsKey(absence.StudentNumber + "-" + absence.CourseID + "-" + absence.SectionID))
                    this.dicAbsences.Add(absence.StudentNumber + "-" + absence.CourseID + "-" + absence.SectionID, absence);
            }
            List<UDT.MailLog> MailLogs = Access.Select<UDT.MailLog>();
            foreach (UDT.MailLog x in MailLogs)
            {
                if (x.Status.ToLower() != "sent")
                    continue;

                if (x.IsCC)
                    continue;

                if (string.IsNullOrEmpty(x.Extension))
                    continue;

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
                List<string> SectionIDs = new List<string>();
                if (xElement.Element("Section") != null)
                {
                    if (!string.IsNullOrEmpty(xElement.Element("Section").Attribute("ID").Value))
                        SectionIDs = xElement.Element("Section").Attribute("ID").Value.Split(',').ToList();
                }

                foreach (string SectionID in SectionIDs)
                {
                    string key = CourseID + "-" + StudentID + "-" + SectionID;
                    if (!this.dicMailLogs.ContainsKey(key))
                        this.dicMailLogs.Add(key, new List<UDT.MailLog>());

                    this.dicMailLogs[key].Add(x);
                }                    
            }
        }

        private void Exit_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void Confirm_Click(object sender, EventArgs e)
        {          
            this.circularProgress.Visible = true;
            this.circularProgress.IsRunning = true;
            //this.Confirm.Enabled = false;

            string oCourseName = this.cboCourse.Text.Trim();
            string sBeginDate = this.dtBeginDate.Value.ToString("yyyy-MM-dd");
            string sEndDate = this.dtEndDate.Value.ToString("yyyy-MM-dd");

            List<DataGridViewRow> DataGridViewRows = this.dgvData.Rows.Cast<DataGridViewRow>().ToList();
            
            Task<Workbook> task = Task<Workbook>.Factory.StartNew(() =>
            {
                Workbook wb = new Workbook();   
                wb.Worksheets.Cast<Worksheet>().ToList().ForEach(x => wb.Worksheets.RemoveAt(x.Index));

                wb.Worksheets.Add();
                wb.Worksheets[0].Name = sBeginDate + "~" + sEndDate + " " + oCourseName + " 缺課紀錄";

                if (DataGridViewRows.Count > 0)
                {
                    for (int i = 0; i < DataGridViewRows.ElementAt(0).Cells.Count; i++)
                    {
                        wb.Worksheets[0].Cells[0, i].PutValue(DataGridViewRows.ElementAt(0).Cells[i].OwningColumn.HeaderText);
                    }
                }

                for (int i = 0; i < DataGridViewRows.Count; i++)
                {
                    for (int j = 0; j < DataGridViewRows.ElementAt(0).Cells.Count; j++)
                    {
                        wb.Worksheets[0].Cells[i + 1, j].PutValue(DataGridViewRows.ElementAt(i).Cells[j].Value + "");
                    }
                }

                wb.Worksheets[0].AutoFitColumns();

                return wb;
            });
            task.ContinueWith((x) =>
            {
                if (x.Exception != null)
                {
                    MessageBox.Show(x.Exception.InnerException.Message);
                    goto TheEnd;
                }
                SaveFileDialog sd = new SaveFileDialog();
                sd.Filter = "Excel 檔案|*.xls;";
                sd.FileName = x.Result.Worksheets[0].Name;
                sd.AddExtension = true;
                if (sd.ShowDialog() == DialogResult.OK)
                {
                    try
                    {
                        x.Result.Save(sd.FileName);
                        System.Diagnostics.Process.Start(sd.FileName);
                    }
                    catch (Exception ex)
                    {
                        FISCA.Presentation.Controls.MsgBox.Show(ex.Message);
                    }
                }
            TheEnd:
                this.circularProgress.IsRunning = false;
                this.circularProgress.Visible = false;
                //this.Confirm.Enabled = true;
                    
            }, System.Threading.CancellationToken.None, TaskContinuationOptions.None, TaskScheduler.FromCurrentSynchronizationContext());
        }

        private void txtSNum_TextChanged(object sender, EventArgs e)
        {
            if (!this.data_loaded)
                return;

            this.DGV_DataBinding();
        }

        private void cboCourse_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (!this.data_loaded)
                return;

            this.DGV_DataBinding();
        }

        private void dtBeginDate_ValueChanged(object sender, EventArgs e)
        {
            if (!this.data_loaded)
                return;

            this.InitCourseDataSource();
            this.DGV_DataBinding();
        }

        private void dtEndDate_ValueChanged(object sender, EventArgs e)
        {
            if (!this.data_loaded)
                return;

            this.InitCourseDataSource();
            this.DGV_DataBinding();
        }
    }

    public class Absence
    {
        public string StudentNumber { get; set; }
        public string Name { get; set; }
        public string NewSubjectCode { get; set; }
        public string CourseName { get; set; }
        public DateTime BeginTime { get; set; }
        public DateTime EndTime { get; set; }
        public string SectionID { get; set; }
        public string CourseID { get; set; }
        public string StudentID { get; set; }
        public int SchoolYear { get; set; }
        public int Semester { get; set; }
        public string MakeUpDescription { get; set; }
        public List<UDT.MailLog> MailLogs  { get; set; }
        public Absence()
        {

        }
    }
}