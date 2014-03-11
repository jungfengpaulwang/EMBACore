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
using DevComponents.DotNetBar;
using FISCA.UDT;
using EMBACore.DataItems;

namespace EMBACore.Forms
{
    public partial class SubjectScore : BaseForm
    {
        private Dictionary<string, UDT.SubjectSemesterScore> dicScores = new Dictionary<string, UDT.SubjectSemesterScore>();
        private Dictionary<string, string> dicStudents = new Dictionary<string, string>();
        private bool isDirty = false;
        private int currentCourseID ;
        private string currentSubjectName = "";
        private string currentSubjectCode = "";
        private string currentCourseName = "";
        private int currentSubjectID ;

        private int currentCredit;
        private bool currentIsRequired;

        private bool isLocked;  //成績輸入功能是否鎖定？
        private bool isInitializing = false;

        private Dictionary<string, ScoreInputProgress> dicScoreInputProgress = new Dictionary<string, ScoreInputProgress>();

        public SubjectScore()
        {
            InitializeComponent();
        }

        private void SubjectScore_Load(object sender, EventArgs e)
        {
            this.isInitializing = true;
            Util.InitSchoolYearNumberUpDown(this.nudSchoolYear);
            this.isInitializing = false;

            Util.InitSemesterCombobox(this.cboSemester);
           
        }

        private void cboSemester_SelectedIndexChanged(object sender, EventArgs e)
        {
            getLockedStatus();
            refreshCourses();
        }

        private void getLockedStatus()
        {
            if (this.isInitializing)
                return;

            this.isLocked = false;  //Default value 
            try
            {
                AccessHelper hp = new AccessHelper();
                string condition = string.Format("school_year={0} and semester={1}", this.nudSchoolYear.Value.ToString(), ((SemesterItem)this.cboSemester.SelectedItem).Value);
                List<UDT.SubjectScoreLock> scoreLocks = hp.Select<UDT.SubjectScoreLock>(condition);
                if (scoreLocks.Count > 0)
                {
                    this.isLocked = scoreLocks[0].IsLocked;
                }
                this.pictureBox1.Visible = this.isLocked;
               
                this.dataGridViewX1.ReadOnly = this.isLocked;
                
            }
            catch (Exception ex)
            {
                Util.ShowMsg("取得成績輸入鎖定狀態時發生錯誤：" + ex.Message,"取得鎖定成績輸入狀態");
            }
        }

        private void refreshCourses()
        {
            if (this.isInitializing)
                return;

            try
            {
                QueryHelper q = new QueryHelper();
                string strSQL = string.Format("SELECT c.*, s.name subject_name, s.subject_code, c_ext.class_name, c_ext.ref_subject_id, c_ext.is_required, c_ext.score_confirmed FROM course c left outer join $ischool.emba.course_ext c_ext on c_ext.ref_course_id = c.id left outer join $ischool.emba.subject s on c_ext.ref_subject_id = s.uid WHERE c.school_year={0} and c.semester={1} ORDER BY c.course_name", this.nudSchoolYear.Value.ToString(), ((SemesterItem)this.cboSemester.SelectedItem).Value);

                DataTable dt = q.Select(strSQL);
                this.itemPanel1.Items.Clear();
                foreach (DataRow dr in dt.Rows)
                {
                    if (!this.dicScoreInputProgress.ContainsKey(dr["id"] + ""))
                        this.dicScoreInputProgress.Add(dr["id"] + "", new ScoreInputProgress());

                    bool score_confirmed = false;
                    bool.TryParse(dr["score_confirmed"] + "", out score_confirmed);

                    this.dicScoreInputProgress[dr["id"] + ""].CourseID = dr["id"] + "";
                    this.dicScoreInputProgress[dr["id"] + ""].Progress = score_confirmed ? "已確認並上傳" : "未輸入";

                    ButtonItem bi = new ButtonItem();
                    bi.Text = string.Format("{0}", dr["course_name"].ToString()  );
                    bi.Tag = dr;
                    bi.Click += new EventHandler(bi_click);                    
                    if (dr["credit"].ToString() == "")
                    {
                        bi.Enabled = false;
                        bi.Text = string.Format("{0} (須指定學分數)", dr["course_name"].ToString());
                        //this.toolTip1.SetToolTip(bi, "請先指定這門課程的學分數！");
                        //bi.MouseEnter += delegate {
                        //    //this.balloonTip1.SetBalloonCaption(this.textBoxX2, "Attention!");
                        //    this.balloonTip1.SetBalloonText(bi, "Please enter both first name and surname.");
                            
                        //    this.balloonTip1.ShowBalloon(bi); 
                        //};
                    }
                    this.itemPanel1.Items.Add(bi);
                }
                this.itemPanel1.Refresh();
            }
            catch (Exception ex)
            {
                Util.ShowMsg("讀取課程清單時發生錯誤！" + ex.Message, "管理科目成績");
            }
        }

        void bi_click(object sender, EventArgs e)
        {
            ButtonItem bi = (ButtonItem)sender;
            string courseID = ((DataRow)bi.Tag)["id"].ToString();
            
            string schoolYear = ((DataRow)bi.Tag)["school_year"].ToString();
            string semester = ((DataRow)bi.Tag)["semester"].ToString();
            this.currentCourseID = int.Parse(courseID);
            this.currentCourseName = ((DataRow)bi.Tag)["course_name"].ToString();
            this.currentSubjectID = int.Parse(((DataRow)bi.Tag)["ref_subject_id"].ToString());
            this.currentSubjectCode = ((DataRow)bi.Tag)["subject_code"].ToString();
            this.currentSubjectName = ((DataRow)bi.Tag)["subject_name"].ToString();
            this.currentCredit = int.Parse(((DataRow)bi.Tag)["credit"].ToString());
            this.currentIsRequired = bool.Parse(((DataRow)bi.Tag)["is_required"].ToString());

            this.lblCourseName.Text = string.Format(" {0} ( {1} 學年度  {2} )", bi.Text, schoolYear, SemesterItem.GetSemesterByCode(semester));
            this.reloadStudents(courseID, schoolYear, semester);
            
        }

        void reloadStudents(string courseID, string schoolYear, string semester)
        {
            if (this.isDirty)
            {
                if (MessageBox.Show("您有資料尚未儲存，是否先儲存後才切換課程？", "", MessageBoxButtons.YesNo, MessageBoxIcon.Information) == System.Windows.Forms.DialogResult.Yes)
                    if (!SaveData())
                        return;
            }

            try
            {             
                //Get Semester Score Records
                AccessHelper ah = new AccessHelper();
                List<UDT.SubjectSemesterScore> scores = ah.Select<UDT.SubjectSemesterScore>("ref_course_id=" + courseID);
                this.dicScores.Clear();
                foreach (UDT.SubjectSemesterScore score in scores)
                    dicScores.Add(score.StudentID.ToString(), score);

                // Get SC_Attend Records
                QueryHelper q = new QueryHelper();
                string strSQL = @"select cls.class_name, stu.id, stu.name, stu.student_number , att.ref_course_id, att.is_cancel
                                            from student stu inner join $ischool.emba.scattend_ext att on att.ref_student_id = stu.id 
                                                left outer join class cls on stu.ref_class_id = cls.id 
                                            where att.ref_course_id={0} 
                                            order by cls.class_name, stu.student_number";
                strSQL = string.Format(strSQL, courseID);
                DataTable dt = q.Select(strSQL);
                this.dataGridViewX1.Rows.Clear();
                this.dicStudents = new Dictionary<string, string>();
                if (this.dicScoreInputProgress.ContainsKey(courseID))
                {
                    if (this.dicScoreInputProgress[courseID].Progress != "已確認並上傳")
                        this.dicScoreInputProgress[courseID].Progress = "未輸入";
                }
                foreach (DataRow dr in dt.Rows)
                {
                    SubjectScoreWrapper ssw = new SubjectScoreWrapper(dr, this.dicScores, schoolYear, semester);
                    object[] rawData = new object[] { ssw.ClassName, ssw.StudentNumber, ssw.StudentName + (ssw.IsCanceled ? "(停)" : "") , (ssw.IsCanceled ? "X" : ssw.Score) , (ssw.IsCanceled ? false : ssw.IsPass)};
                    int rowIndex = this.dataGridViewX1.Rows.Add(rawData);
                    this.dataGridViewX1.Rows[rowIndex].Tag = ssw;
                    string studID = dr["id"].ToString();
                    if (!this.dicStudents.ContainsKey(studID))
                        this.dicStudents.Add(studID, dr["name"].ToString());

                    if (this.dicScoreInputProgress.ContainsKey(dr["ref_course_id"] + ""))
                    {
                        if (this.dicScoreInputProgress[dr["ref_course_id"] + ""].Progress == "已確認並上傳")
                            continue;

                        if (!ssw.IsCanceled && !string.IsNullOrEmpty(ssw.Score))
                            this.dicScoreInputProgress[dr["ref_course_id"] + ""].Progress = "已暫存";
                    }
                }
                this.lblScoreProgress.Text = this.dicScoreInputProgress.ContainsKey(courseID) ? this.dicScoreInputProgress[courseID].Progress : "";
 
                this.enableSaveButton();
            }
            catch (Exception ex)
            {
                Util.ShowMsg("讀取學生成績時發生錯誤！" + ex.Message, "管理科目成績");
            }
        }

        private void dataGridViewX1_CellEndEdit(object sender, DataGridViewCellEventArgs e)
        {            
            if (e.ColumnIndex == 3)
            {
                DataGridViewRow row = this.dataGridViewX1.Rows[e.RowIndex];
                DataGridViewCell cell = row.Cells[e.ColumnIndex];
                SubjectScoreWrapper ssw = (SubjectScoreWrapper)row.Tag;

                if (ssw.IsCanceled)
                {  
                    cell.Value = "X";
                    row.Cells[4].Value = false;
                    return;
                }

                string newValue = (row.Cells[e.ColumnIndex].Value == null ? "" : row.Cells[e.ColumnIndex].Value.ToString()).ToUpper();
                if (Util.IsValidScore(newValue))
                {
                    row.Cells[e.ColumnIndex].Value = newValue;  //重新指定值一次，確保輸入的是大寫
                    ssw.NewScore = newValue;
                    row.Cells[e.ColumnIndex].Style.BackColor = ssw.IsDirty ? Color.Pink : Color.White;
                    row.Cells[e.ColumnIndex + 1].Value = Util.IsPass(newValue);
                    cell.ErrorText= "";
                }
                else
                {
                    cell.ErrorText = "不是有效的分數";
                    this.dataGridViewX1.CurrentCell = cell;
                }
            }
            this.enableSaveButton();
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            this.SaveData();
        }

        private bool SaveData()
        {
            //先確定所有分數格式都正確
            bool scoreAllRight = true;
            foreach (DataGridViewRow row in this.dataGridViewX1.Rows)
            {
                if (row.IsNewRow) continue;                
                SubjectScoreWrapper ssw = (SubjectScoreWrapper)row.Tag;
                if (ssw.IsDirty)
                {
                    string Score = (row.Cells["colScore"].Value == null) ? "" : row.Cells["colScore"].Value.ToString();
                    if (!string.IsNullOrWhiteSpace(Score) )  //有輸入分數
                    {
                        if (!Util.IsValidScore(Score))  //但不是有效分數
                        {
                            scoreAllRight = false;
                            break;
                        }
                    }
                }
            }
            if (!scoreAllRight)
            {
                Util.ShowMsg("有些分數的格式不正確，請修正。", "");
                return false ;
            }
            
            //prepare log message 
            StringBuilder sb = new StringBuilder(string.Format("更改 {0} 學年度 {1} 『{2}』的學生成績如下：\n",  this.nudSchoolYear.Value.ToString(), this.cboSemester.Text , this.currentCourseName));
            
            //找出所有需要儲存的成績紀錄
            bool result = true;
            List<ActiveRecord> recs = new List<ActiveRecord>();
            foreach (DataGridViewRow row in this.dataGridViewX1.Rows)
            {
                if (row.IsNewRow) continue;                
                SubjectScoreWrapper ssw = (SubjectScoreWrapper)row.Tag;
                if (ssw.IsDirty)
                {
                    UDT.SubjectSemesterScore score = ssw.GetScoreObject();
                    score.Score = (row.Cells["colScore"].Value == null) ? "" : row.Cells["colScore"].Value.ToString();
                    score.SubjectID = this.currentSubjectID;
                    score.SubjectCode = this.currentSubjectCode;
                    score.SubjectName = this.currentSubjectName;
                    score.IsPass = (row.Cells["colIsPass"].Value == null) ? false : bool.Parse(row.Cells["colIsPass"].Value.ToString());
                    score.Credit = this.currentCredit;
                    score.IsRequired = this.currentIsRequired;

                    string studName = this.dicStudents[score.StudentID.ToString()];

                    if (string.IsNullOrWhiteSpace(score.Score))
                    {
                        if (!string.IsNullOrWhiteSpace(score.UID))
                        {
                            score.Deleted = true;
                            recs.Add(score);
                            sb.Append(string.Format("刪除成績 ->  學生：{0} , 分數 : {1} \n ", studName, score.Score));
                        }
                    }
                    else
                    {
                        recs.Add(score);
                        if (string.IsNullOrWhiteSpace(score.UID))
                            sb.Append(string.Format("新增成績 ->  學生：{0} , 分數 : {1} \n ", studName, score.Score));
                        else
                            sb.Append(string.Format("修改成績 ->  學生：{0} , 分數由 {1} 改為 {2}  \n ", studName, ssw.OldScore, score.Score));
                    }
                }                
            }

           

            try
            {
                if (recs.Count > 0)
                {
                    AccessHelper ah = new AccessHelper();
                    ah.SaveAll(recs);                    
                    
                    //Refresh
                    SubjectScoreWrapper ssw = (SubjectScoreWrapper)this.dataGridViewX1.Rows[0].Tag;
                    this.isDirty = false;
                    this.reloadStudents(ssw.CourseID, ssw.SchoolYear, ssw.Semester);

                    FISCA.LogAgent.ApplicationLog.Log("學期科目成績", "修改", "course", this.currentCourseID.ToString(), sb.ToString());

                    Util.ShowMsg("儲存成功", "管理科目成績");
                }
            }
            catch (Exception ex)
            {
                Util.ShowMsg("儲存成績時發生錯誤！", "管理科目成績");
                result = false;
            }
            this.enableSaveButton();
            return result;
        }

        private void enableSaveButton()
        {
            if (this.isLocked)  //如果成績輸入被鎖定了，就一定 Disabled
            {
                this.btnSave.Enabled = false;
                return;
            }
            else
                this.btnSave.Enabled = true;

            this.isDirty = false;
            foreach (DataGridViewRow row in this.dataGridViewX1.Rows)
            {
                if (row.IsNewRow) continue;
                SubjectScoreWrapper ssw = (SubjectScoreWrapper)row.Tag;
                if (ssw.IsDirty)
                {
                    this.isDirty = true;
                    break;
                }
            }
            this.btnSave.Enabled = (this.isDirty);
            if (this.isDirty)
            {
                //if (this.lblCourseName.Text.Substring(this.lblCourseName.Text.Length - 1, 1) != "*")
                //    this.lblCourseName.Text += "*";
                this.lblScoreInputStatus.Text = "成績修改，尚未儲存";
            }
            else
            {
                //if (this.lblCourseName.Text.Substring(this.lblCourseName.Text.Length - 1, 1) == "*")
                //    this.lblCourseName.Text = this.lblCourseName.Text.Substring(0, this.lblCourseName.Text.Length - 2);
                this.lblScoreInputStatus.Text = "";
            }

        }


        private class SubjectScoreWrapper
        {
            private DataRow dr;
            private Dictionary<string, UDT.SubjectSemesterScore> dicScores;
            public SubjectScoreWrapper(DataRow drSCAttendRecord, Dictionary<string, UDT.SubjectSemesterScore> dicScores, string schoolYear, string semester)
            {
                this.dicScores = dicScores;
                this.dr = drSCAttendRecord;
                this.ClassName = (dr["class_name"] == null) ? "" : dr["class_name"].ToString();
                this.StudentID = dr["id"].ToString();
                this.StudentName = dr["name"].ToString();
                this.StudentNumber = dr["student_number"].ToString();
                this.CourseID = dr["ref_course_id"].ToString();
                this.SchoolYear = schoolYear;
                this.Semester = semester;
                this.IsCanceled = (dr["is_cancel"].ToString().Trim() == "true");
                this.oldScore = (this.dicScores.ContainsKey(this.StudentID) ? this.dicScores[this.StudentID].Score  : "");
            }
            private string oldScore = "";
            public string OldScore { get { return this.oldScore; } }
            public string ClassName { get; set; }
            public string StudentID { get; set; }
            public string StudentName { get; set; }
            public string StudentNumber { get; set; }
            public string CourseID { get; set; }
            public string SubjectID { get; set; }
            public string SchoolYear { get; set; }
            public string Semester { get; set; }
            public bool IsCanceled { get; set; }    //是否停修
            public string Score
            {
                get
                {
                    string result = "";

                    if (this.dicScores.ContainsKey(this.StudentID))
                        result = this.dicScores[this.StudentID].Score;

                    return result;
                }
            }

            public bool IsPass
            {
                get
                {
                    bool result = false;

                    if (this.dicScores.ContainsKey(this.StudentID))
                        result = this.dicScores[this.StudentID].IsPass;

                    return result;
                }
            }

            private string newValue = "";
            public string NewScore {
                get { return this.newValue; }
                set
                {
                    this.newValue = value;
                    this.IsDirty = (this.newValue != this.Score);
                }
            }
            public bool IsDirty { get; set; }

            public bool HasScore
            {
                get { return this.dicScores.ContainsKey(this.StudentID); }
            }

            public UDT.SubjectSemesterScore GetScoreObject()
            {
                UDT.SubjectSemesterScore result = null;
                if (this.dicScores.ContainsKey(this.StudentID))
                    result = this.dicScores[this.StudentID];
                else
                {
                    result = new UDT.SubjectSemesterScore();
                    result.StudentID = int.Parse(this.StudentID);
                    result.CourseID = int.Parse(this.CourseID);
                    result.SchoolYear = int.Parse(this.SchoolYear);
                    result.Semester = int.Parse(this.Semester);
                }

                return result;
            }
        }

        private void nudSchoolYear_ValueChanged(object sender, EventArgs e)
        {
            getLockedStatus();
            refreshCourses();
        }
    }
    public class ScoreInputProgress
    {
        //  課號、開課、班次、成績管理者、成績輸入進度
        public string CourseID { get; set; }
        public string Progress { get; set; }
    }
}
