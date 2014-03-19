using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Windows.Forms;
using System.Xml;
using Campus;
using Campus.Windows;
using FISCA.Data;
using FISCA.Permission;
using FISCA.Presentation;
using FISCA.UDT;
using SHSchool.Data;

namespace EMBACore.DetailItems
{
    [AccessControl("ischool.EMBA.Course_AddDrop", "加退選", "學生>資料項目")]
    public partial class Student_AddDropCourse : DetailContentImproved
    {
        private List<SHCourseRecord> courses = null;
        private List<string> attRecords = null;  //courseID List
        private List<UDT.AddDropCourse> udtCourses = null;
        private Dictionary<string, UDT.AddDropCourse> dicAddDropList;  //courseid , type
        private Dictionary<string, SHCourseRecord> dicCourses;
        //private Dictionary<string, string> dicCourse_NewSubjectCode;
        private Dictionary<string, string> dicCourse_SerialNo;  //courseID, serial_no 顯示課程順序用的
        private string addCourse = "加";
        private string dropCourse = "退";

        private int SchoolYear;
        private int Semester;

        private bool canEdit = false;

        public Student_AddDropCourse()
        {
            InitializeComponent();
            this.Group = "加退選";
        }

        private void Course_AddDrop_Load(object sender, EventArgs e)
        {
            this.courses = new List<SHCourseRecord>();

            /* Check Permission */
            this.canEdit = Permission.Editable;

            this.InitSemesterInfo();   
        }

        //private void getDefaultSchoolYearSemester()
        //{
        //    //0, 取得目前學年度學期
        //    xmlSystemConfig = new XmlDocument();
        //    xmlSystemConfig.LoadXml(Config.App["系統設定"].PreviousData.OuterXml);
        //    elmSchoolYear = (XmlElement)xmlSystemConfig.DocumentElement.SelectSingleNode("DefaultSchoolYear");
        //    elmSemester = (XmlElement)xmlSystemConfig.DocumentElement.SelectSingleNode("DefaultSemester");
        //    this.defaultSchoolYear = elmSchoolYear.InnerText;
        //    this.defaultSemester = elmSemester.InnerText;
        //}

        private void LoadCourses()
        {
            this.courses = SHCourse.SelectBySchoolYearAndSemester(this.SchoolYear, this.Semester);
            this.dicCourses = new Dictionary<string, SHCourseRecord>();
            foreach (SHCourseRecord rec in this.courses)
            {
                this.dicCourses.Add(rec.ID, rec);
            }

            //用來將課程按照流水號排序。
            this.dicCourse_SerialNo = new Dictionary<string, string>();
            QueryHelper qh = new QueryHelper();
            string strSQL = string.Format("select c.id, ext.serial_no from course c inner join $ischool.emba.course_ext ext on c.id = ext.ref_course_id where c.school_year={0} and c.semester={1} ;", this.SchoolYear, this.Semester);
            DataTable dt = qh.Select(strSQL);
            foreach (DataRow dr in dt.Rows)
            {
                string courseid = dr[0].ToString();
                if (!this.dicCourse_SerialNo.ContainsKey(courseid))
                    this.dicCourse_SerialNo.Add(dr[0].ToString(), dr[1].ToString());
            }
        }

        private void refreshUI()
        {
            if (this.attRecords == null)
                return;

            Dictionary<string, string> dicResult = new Dictionary<string, string>();    //紀錄已經填入的課程。
            this.dataGridViewX1.Rows.Clear();
            this.dataGridViewX2.Rows.Clear();
            //1. 填入已修課課程，要檢查是否有退選課程 (Grid 1)
            foreach (string courseID in this.attRecords)
            {
                if (dicCourses.ContainsKey(courseID))
                {
                    SHCourseRecord course = this.dicCourses[courseID];
                    //檢查是否有退選
                    string isDrop = "";
                    if (this.dicAddDropList.ContainsKey(courseID))
                    {
                        isDrop = (this.dicAddDropList[courseID].AddOrDrop.ToUpper() == this.dropCourse) ? this.dicAddDropList[courseID].AddOrDrop.ToUpper() : ""; 
                    }
                    object[] rawData = new object[] { isDrop, course.Name };
                    int rowIndex = this.dataGridViewX1.Rows.Add(rawData);
                    this.dataGridViewX1.Rows[rowIndex].Tag = course;

                    //紀錄哪些課程已經填入，除退選外
                    if (isDrop != this.dropCourse)
                    {
                        if (!dicResult.ContainsKey(courseID))
                            dicResult.Add(courseID, "");
                    }
                }
            }

            //2. 填入加選課程(Grid 1)
            foreach (string courseID in this.dicAddDropList.Keys)
            {
                if (this.dicAddDropList[courseID].AddOrDrop.ToUpper() == this.addCourse)
                {
                    if (dicCourses.ContainsKey(courseID))
                    {
                        SHCourseRecord course = this.dicCourses[courseID];
                        //檢查是否有退選
                        string isDrop = this.dicAddDropList[courseID].AddOrDrop.ToUpper(); ;
                        object[] rawData = new object[] { isDrop, course.Name };
                        int rowIndex = this.dataGridViewX1.Rows.Add(rawData);
                        this.dataGridViewX1.Rows[rowIndex].Tag = course;

                        //紀錄加選課程
                        if (!dicResult.ContainsKey(courseID))
                            dicResult.Add(courseID, "");
                    }
                }
            }

            //3. 填入尚未修課課程 (Grid 2)
            List<SHCourseRecord> recs = new List<SHCourseRecord>();
            foreach (SHCourseRecord course in this.courses)
            {
                if (!dicResult.ContainsKey(course.ID))
                {
                    recs.Add(course);
                }
            }
            //按照 course_ext.serial_no 排序
            recs.Sort(
                delegate(SHCourseRecord s1, SHCourseRecord s2) 
                {
                    string serial_no1 = (this.dicCourse_SerialNo.ContainsKey(s1.ID) ? this.dicCourse_SerialNo[s1.ID] : "");
                    string serial_no2 = (this.dicCourse_SerialNo.ContainsKey(s2.ID) ? this.dicCourse_SerialNo[s2.ID] : "");
                    return serial_no1.CompareTo(serial_no2); 
                }
            );
            foreach (SHCourseRecord course in recs)
            {
                object[] rawData = new object[] { course.Name };
                int rowIndex = this.dataGridViewX2.Rows.Add(rawData);
                this.dataGridViewX2.Rows[rowIndex].Tag = course;
            }

        }

        protected override void OnInitializeAsync()
        {
            LoadCourses();
        }

        protected override void OnInitializeComplete(Exception error)
        {
            if (error != null) //有錯就直接丟出去吧。
                throw error;
            else
            {
                Agent.Event.CSOpeningInfo.AfterUpdate += (x, y) => this.InitSemesterInfo();
                UDT.SCAttendExt.AfterUpdate += (x, y) => base.OnPrimaryKeyChanged(EventArgs.Empty);
            }
        }

        protected override void OnPrimaryKeyChangedAsync()
        {
            string studentID = this.PrimaryKey;
            //1.取得修課紀錄
            QueryHelper q = new QueryHelper();
            string sql = @"select ref_course_id 
                                    from $ischool.emba.scattend_ext att inner join course c on c.id = att.ref_course_id  
                                    where att.ref_student_id={0} and c.school_year={1} and c.semester={2}";
            //string schoolyear = K12.Data.School.DefaultSchoolYear;
            //string semester = K12.Data.School.DefaultSemester;
            DataTable dt = q.Select(string.Format(sql, studentID, this.SchoolYear, this.Semester));
            this.attRecords = new List<string>();
            foreach (DataRow dr in dt.Rows)
            {
                this.attRecords.Add(dr[0].ToString());
            }
            this.attRecords = this.attRecords.Distinct().ToList();
            //2. 取得加退選紀錄
            AccessHelper ah = new AccessHelper();
            this.udtCourses = ah.Select<UDT.AddDropCourse>("ref_student_id='" + this.PrimaryKey + "' and confirm_date is null ");
            this.dicAddDropList = new Dictionary<string, UDT.AddDropCourse>();
            foreach (UDT.AddDropCourse addDrop in this.udtCourses)
            {
                if (!this.dicAddDropList.ContainsKey(addDrop.CourseID.ToString()))
                    this.dicAddDropList.Add(addDrop.CourseID.ToString(), addDrop);
            }
        }

        protected override void OnPrimaryKeyChangedComplete(Exception error)
        {
            if (error != null) //有錯就直接丟出去吧。
                throw error;

            ErrorTip.Clear();

            refreshUI();

            ResetDirtyStatus();

            this.btnAdd.Enabled = this.canEdit ;
            this.btnRemove.Enabled = this.canEdit;
        }

        protected override void OnValidateData(Dictionary<Control, string> errors)
        {

        }

        protected override void OnSaveData()
        {
            

            ResetDirtyStatus();
        }

        protected override void OnDirtyStatusChanged(ChangeEventArgs e)
        {
            if (UserAcl.Current[this.GetType()].Editable)
                SaveButtonVisible = e.Status == ValueStatus.Dirty;
            else
                this.SaveButtonVisible = false;

            CancelButtonVisible = e.Status == ValueStatus.Dirty;
        }

        private void dataGridViewX2_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }
                
        private void btnAdd_Click(object sender, EventArgs e)
        {            
            if (this.dataGridViewX2.SelectedRows.Count < 1)
            {
                return;
            }

            List<ActiveRecord> addRecs = new List<ActiveRecord>();
            List<ActiveRecord> delRecs = new List<ActiveRecord>();
            this.btnAdd.Enabled = false;
            
            foreach (DataGridViewRow row in this.dataGridViewX2.SelectedRows)
            {
                SHCourseRecord course = (SHCourseRecord)row.Tag;
                //如果原本是被標註退選的，應該要把該退選紀錄移除
                if (this.dicAddDropList.ContainsKey(course.ID))
                {
                    if (this.dicAddDropList[course.ID].AddOrDrop.ToUpper() == this.dropCourse)
                    {
                        delRecs.Add(this.dicAddDropList[course.ID]);
                    }
                }
                else
                {
                    UDT.AddDropCourse ad = new UDT.AddDropCourse();
                    ad.StudentID = int.Parse(this.PrimaryKey);
                    ad.CourseID = int.Parse(course.ID);
                    ad.AddOrDrop = this.addCourse;
                    addRecs.Add(ad);
                }
            }
            try
            {
                AccessHelper ah = new AccessHelper();
                if (delRecs.Count > 0)  ah.DeletedValues(delRecs);
                if (addRecs.Count > 0) ah.InsertValues(addRecs);
                this.OnPrimaryKeyChanged(EventArgs.Empty);
                this.saveLogs(delRecs, addRecs);
            }
            catch (Exception ex)
            {
                Util.ShowMsg(ex.Message, "加選課程");
                this.btnAdd.Enabled = true;
            }

            //this.btnAdd.Enabled = true ;    //應該要等資料讀取完後才 enabled,  2012/5/2, kevin. 
        }

        //原本被標為退選的，按下退選後沒有反應。
        //原本被標為加選的，按下退選後刪除該加退選紀錄。
        //原本沒有標示的，按下退選後會加入一筆退選紀錄。
        private void btnRemove_Click(object sender, EventArgs e)
        {          
            if (this.dataGridViewX1.SelectedRows.Count < 1)
            {
                return;
            }

            List<ActiveRecord> addRecs = new List<ActiveRecord>();
            List<ActiveRecord> delRecs = new List<ActiveRecord>();
            this.btnRemove.Enabled = false;

            foreach (DataGridViewRow row in this.dataGridViewX1.SelectedRows)
            {
                SHCourseRecord course = (SHCourseRecord)row.Tag;
                if (row.Cells[0].Value.ToString() == "")
                {
                    UDT.AddDropCourse ad = new UDT.AddDropCourse();
                    ad.CourseID = int.Parse(course.ID);
                    ad.StudentID = int.Parse(this.PrimaryKey);
                    ad.AddOrDrop = this.dropCourse;
                    addRecs.Add(ad);
                }

                if (row.Cells[0].Value.ToString() == this.addCourse)
                {
                    UDT.AddDropCourse ad = this.dicAddDropList[course.ID];
                    delRecs.Add(ad);
                }                
            }

            try
            {
                AccessHelper ah = new AccessHelper();
                if (delRecs.Count > 0) ah.DeletedValues(delRecs);
                if (addRecs.Count > 0) ah.InsertValues(addRecs);
                this.OnPrimaryKeyChanged(EventArgs.Empty);
                this.saveLogs(delRecs, addRecs);
            }
            catch (Exception ex)
            {
                Util.ShowMsg(ex.Message, "退選課程");
                this.btnRemove.Enabled = true;
            }

            //this.btnRemove.Enabled = true;           
        }

        private void AddLog(UDT.AddDropCourse obj, Log.LogAgent agt)
        {
            if (agt != null)
            {
                agt.SetLogValue("課程编號", obj.CourseID.ToString());
                string courseName = "";
                if (this.dicCourses.ContainsKey(obj.CourseID.ToString()))
                    courseName = this.dicCourses[obj.CourseID.ToString()].Name;
                agt.SetLogValue("課程名稱", courseName);
                agt.SetLogValue("加/退選", obj.AddOrDrop);                
            }
        }

        private void saveLogs(List<ActiveRecord> delRecs, List<ActiveRecord> addRecs)
        {
            /* ====  Log  for deleted records =====*/
            foreach (UDT.AddDropCourse ci in delRecs)
            {
                Log.LogAgent agt = new Log.LogAgent();
                agt.ActionType = Log.LogActionType.Delete;
                this.AddLog(ci, agt);
                agt.Save("加退選.學生", "刪除", "", Log.LogTargetCategory.Student, ci.StudentID.ToString());
            }
            /* ====  Log  for inserted records =====*/
            foreach (UDT.AddDropCourse ci in addRecs)
            {
                Log.LogAgent agt = new Log.LogAgent();
                agt.ActionType = Log.LogActionType.AddNew;
                this.AddLog(ci, agt);
                agt.Save("加退選.學生", "新增", "", Log.LogTargetCategory.Student, ci.StudentID.ToString());
            }
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
            this.btnAdd.Enabled = this.canEdit;
            this.btnRemove.Enabled = this.canEdit;     
        }
    }
}
