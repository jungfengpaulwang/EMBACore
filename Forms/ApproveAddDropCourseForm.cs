using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using FISCA.Presentation.Controls;
using FISCA.Data;
using FISCA.UDT;
using K12.Data;
using System.Xml;
using Campus.Configuration;

namespace EMBACore.Forms
{
    public partial class ApproveAddDropCourseForm : BaseForm
    {
        private Dictionary<int, string> dicCourses = new Dictionary<int, string>();    //紀錄取得的課程, courseID, course name.
        private Dictionary<string, string> dicStudent = new Dictionary<string, string>();   //紀錄取得的學生資料 , studentID, student name
        private bool canEdit = false;
        private int SchoolYear;
        private int Semester;

        public ApproveAddDropCourseForm()
        {
            InitializeComponent();
            Agent.Event.CSOpeningInfo.AfterUpdate += (x, y) => this.InitSemesterInfo();
        }

        private void AddDropCourseForm_Load(object sender, EventArgs e)
        {
            this.InitSemesterInfo();
            this.loadData();
        }

        private void loadData()
        {
            //string schoolYear = this.nudSchoolYear.Value.ToString();
            //string semester = ((DataItems.SemesterItem)this.cboSemester.SelectedItem).Value;
            
            List<UDT.AddDropCourse> adRecords = (new AccessHelper()).Select<UDT.AddDropCourse>("confirm_date is null");
            Dictionary<string, UDT.AddDropCourse> dicAdRecords = new Dictionary<string, UDT.AddDropCourse>();
            foreach (UDT.AddDropCourse ad in adRecords)
            {
                dicAdRecords.Add(ad.UID, ad);
            }

            //this.Text = string.Format("核准加退選 ( {0}學年度 {1} )", SchoolYear, DataItems.SemesterItem.GetSemesterByCode(Semester.ToString()).Name);
            QueryHelper q = new QueryHelper();
            string strSQL = @"select ad.uid ad_id, stud.id stud_id, stud.name, c.id c_id, c.course_name, ad.add_or_drop 
                                            from $ischool.emba.course_add_drop ad inner join student stud on stud.id = ad.ref_student_id 
                                            inner join course c on ad.ref_course_id = c.id 
                                            where c.school_year={0} and c.semester={1} and ad.confirm_date is null 
                                            order by stud.name, c.course_name";

            DataTable dt = q.Select(string.Format(strSQL, SchoolYear, Semester));
            this.dataGridViewX1.Rows.Clear();
            foreach (DataRow dr in dt.Rows)
            {
                UDT.AddDropCourse ad = dicAdRecords[dr["ad_id"].ToString()];
                object[] rawData = new object[] { dr["name"].ToString(), dr["course_name"].ToString(), dr["add_or_drop"].ToString() };
                int rowIndex = this.dataGridViewX1.Rows.Add(rawData);
                this.dataGridViewX1.Rows[rowIndex].Tag = ad;

                //紀錄有讀取的課程清單
                int courseID = int.Parse(dr["c_id"].ToString());
                if (!this.dicCourses.ContainsKey(courseID))
                    this.dicCourses.Add(courseID, dr["course_name"].ToString());
                //紀錄有讀取的學生資料
                string studID = dr["stud_id"].ToString();
                if (!this.dicStudent.ContainsKey(studID))
                    this.dicStudent.Add(studID, dr["name"].ToString());
            }
            this.progressBarX1.Visible = false;
        }

        private void InitSemesterInfo()
        {
            DataTable dataTable_Server_Time = (new QueryHelper()).Select("select now()");
            DateTime server_time = DateTime.Parse(dataTable_Server_Time.Rows[0][0] + "");

            List<UDT.CSOpeningInfo> opening_infos = (new AccessHelper()).Select<UDT.CSOpeningInfo>();
            if (opening_infos.Count > 0)
            {
                IEnumerable<UDT.CSOpeningInfo> AddDropCoursePeriodTimes = opening_infos.Where(x => x.Item == 0);
                if (AddDropCoursePeriodTimes.Count() == 0)
                {
                    this.lblSemesterInfo.Text = "尚未設定加退選期間";
                    this.lblSemesterInfo.ForeColor = System.Drawing.Color.Red;
                    this.canEdit = false;
                    goto CheckPermission;
                }
                this.SchoolYear = AddDropCoursePeriodTimes.ElementAt(0).SchoolYear;
                this.Semester = AddDropCoursePeriodTimes.ElementAt(0).Semester;

                DateTime begin_time = AddDropCoursePeriodTimes.ElementAt(0).BeginTime;
                DateTime end_time = AddDropCoursePeriodTimes.ElementAt(0).EndTime;

                if (server_time < begin_time || server_time > end_time.AddDays(7))
                {
                    this.lblSemesterInfo.Text = string.Format("目前時間不在加退選期間：{0}~{1}(+7日)\n不可加退選", begin_time.ToString("yyyy/MM/dd HH:mm"), end_time.ToString("yyyy/MM/dd HH:mm"));
                    this.lblSemesterInfo.ForeColor = System.Drawing.Color.Red;
                    this.canEdit = false;
                }
                else
                {
                    this.lblSemesterInfo.Text = this.SchoolYear + "學年度" + DataItems.SemesterItem.GetSemesterByCode(this.Semester.ToString()).Name;
                    this.lblSemesterInfo.ForeColor = System.Drawing.Color.Blue;
                    this.canEdit = true;
                }
            }
            else
            {
                this.lblSemesterInfo.Text = "尚未設定加退選期間";
                this.lblSemesterInfo.ForeColor = System.Drawing.Color.Red;
                this.canEdit = false;
            }
        CheckPermission:
            this.btnApproved.Enabled = this.canEdit;
        }

        //1. 分類出加選和退選的紀錄，
        //2. 加選的加入 SCAttend，退選的從 SCAttend 刪除
        //3. 更新 ConfirmDate 為今天。
        private void btnApproved_Click(object sender, EventArgs e)
        {                       //student_id,       add_or_drop,         recs
            Dictionary<string, Dictionary<string, List<UDT.AddDropCourse>>> recPerStud = new Dictionary<string,Dictionary<string,List<UDT.AddDropCourse>>>();
            //foreach (DataGridViewRow row in this.dataGridViewX1.Rows)
            foreach (DataGridViewRow row in this.dataGridViewX1.SelectedRows)  //2012.5.8 改成選取的紀錄才核准
            {
                UDT.AddDropCourse ad = (UDT.AddDropCourse)row.Tag;

                if (!recPerStud.ContainsKey(ad.StudentID.ToString()))
                     recPerStud.Add(ad.StudentID.ToString(), new Dictionary<string,List<UDT.AddDropCourse>>());

                string add_or_drop = row.Cells["colAddOrDrop"].Value.ToString();

                if (!recPerStud[ad.StudentID.ToString()].ContainsKey(add_or_drop))
                     recPerStud[ad.StudentID.ToString()].Add(add_or_drop, new List<UDT.AddDropCourse>());

                recPerStud[ad.StudentID.ToString()][add_or_drop].Add(ad);
            }

            if (recPerStud.Keys.Count > 0)
            {
                this.progressBarX1.Maximum = recPerStud.Keys.Count;
                this.progressBarX1.Value = 0;
                this.progressBarX1.Visible = true ;
            }
            else {
                this.progressBarX1.Visible = false ;
                return ;
            }

            int index = 0;
            /* for each student  */
            foreach (string studID in recPerStud.Keys)
            {
                index += 1;
                this.progressBarX1.Value = index;
                Application.DoEvents();

                string studName = this.dicStudent[studID];
                StringBuilder sb = new StringBuilder(string.Format("核准 {0} 的加退選紀錄如下：", studName));
                //取得該生的修課紀錄
                //List<SCAttendRecord> recs = SCAttend.SelectByStudentID(studID);
                List<UDT.SCAttendExt> recs = (new AccessHelper()).Select<UDT.SCAttendExt>("ref_student_id=" + studID);
                Dictionary<string, UDT.SCAttendExt> dicRecs = new Dictionary<string, UDT.SCAttendExt>();
                foreach (UDT.SCAttendExt rec in recs)
                {
                    if (!dicRecs.ContainsKey(rec.StudentID.ToString() + "_" + rec.CourseID.ToString()))
                        dicRecs.Add(rec.StudentID.ToString() + "_" + rec.CourseID.ToString(), rec);
                }
                
                // * 1. 處理加選，先濾掉已經存在修課紀錄的加選紀錄 * /
                if (recPerStud[studID].ContainsKey("加"))
                {
                    sb.Append(" 加選： ( ");
                    List<ActiveRecord> addRecs = new List<ActiveRecord>();
                    foreach (UDT.AddDropCourse ad in recPerStud[studID]["加"])
                    {
                        if (!dicRecs.ContainsKey(ad.StudentID.ToString() + "_" + ad.CourseID.ToString()))
                        {
                            UDT.SCAttendExt scatt = new UDT.SCAttendExt();
                            scatt.StudentID = ad.StudentID;
                            scatt.CourseID = ad.CourseID;

                            addRecs.Add(scatt);
                            //
                            string courseName = this.dicCourses[ad.CourseID];
                            sb.Append(courseName);
                            sb.Append(",");
                        }
                    }
                    sb.Append(" )  ,\n ");

                    if (addRecs.Count > 0)
                        (new AccessHelper()).InsertValues(addRecs);   //加選

                }

                // * 2. 處理退選 * /
                if (recPerStud[studID].ContainsKey("退"))
                {
                    sb.Append(" 退選 : ( ");
                    List<ActiveRecord> delRecs = new List<ActiveRecord>();
                    foreach (UDT.AddDropCourse ad in recPerStud[studID]["退"])
                    {
                        if (dicRecs.ContainsKey(ad.StudentID.ToString() + "_" + ad.CourseID.ToString()))
                        {
                            delRecs.Add(dicRecs[ad.StudentID.ToString() + "_" + ad.CourseID.ToString()]);

                            //add log
                            string courseName = this.dicCourses[ad.CourseID];
                            sb.Append(courseName);
                            sb.Append(", ");
                        }
                    }

                    if (delRecs.Count > 0)
                        (new AccessHelper()).DeletedValues(delRecs);
                        //SCAttend.Delete(delRecs);   //退選

                    sb.Append(" )");
                }               

                FISCA.LogAgent.ApplicationLog.Log("核准加退選.學生", "新增", "student", studID, sb.ToString());
       
            }// END FOR

            // * 3. 更新加退選紀錄的 confirm_date * /
            List<ActiveRecord> updRecs = new List<ActiveRecord>();
            foreach (DataGridViewRow row in this.dataGridViewX1.SelectedRows)
            {
                UDT.AddDropCourse ad = (UDT.AddDropCourse)row.Tag;
                ad.ConfirmDate = DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss");
                updRecs.Add(ad);
            }
            (new AccessHelper()).SaveAll(updRecs);


            Util.ShowMsg("核准完成！", "核准加退選");
            UDT.SCAttendExt.RaiseAfterUpdateEvent(this, null);
            this.loadData();
        }

        private void nudSchoolYear_ValueChanged(object sender, EventArgs e)
        {
            this.loadData();
        }

        private void cboSemester_SelectedIndexChanged(object sender, EventArgs e)
        {
            this.loadData();
        }

        private void buttonX1_Click(object sender, EventArgs e)
        {
            this.Close();
        }    
  
    }

}
