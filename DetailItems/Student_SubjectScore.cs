using Campus.Windows;
using EMBACore.DataItems;
using FISCA.Data;
using FISCA.Permission;
using FISCA.Presentation;
using FISCA.UDT;
using K12.Data;
using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace EMBACore.DetailItems
{
    [AccessControl("ischool.EMBA.Student_SubjectScore", "課程成績", "學生>資料項目")]
    public partial class Student_SubjectScore : DetailContentImproved   //FISCA.Presentation.DetailContent    //   //FISCA.Presentation.DetailContent//DetailContentImproved   //FISCA.Presentation.DetailContent      
    {
        private List<UDT.Subject> subjects;
        private Dictionary<string, UDT.Subject> dicSubjects;
        private Dictionary<string, UDT.Subject> dicSubjectIDs;
        private Dictionary<string, int?> dicSemester;
        private List<UDT.SubjectSemesterScore> myScores;
        private List<UDT.SubjectSemesterScore> deletedScores;
        private Dictionary<string, UDT.SubjectSemesterScore> updatedScores;
        private Dictionary<string, bool> dicCancelCourseMappings;
        private Dictionary<int, UDT.CourseExt> dicCourseExts;
        
        public Student_SubjectScore()
        {
            InitializeComponent();
            this.Group = "課程成績";

            Task task = Task.Factory.StartNew(() =>
            {
                this.dicCourseExts = new Dictionary<int, UDT.CourseExt>();
                List<UDT.CourseExt> CourseExts = (new AccessHelper()).Select<UDT.CourseExt>();
                CourseExts.ForEach((x) =>
                {
                    if (!this.dicCourseExts.ContainsKey(x.CourseID))
                        this.dicCourseExts.Add(x.CourseID, x);
                });
                this.dicSubjectIDs = new Dictionary<string, UDT.Subject>();
                List<UDT.Subject> SubjectIDs = (new AccessHelper()).Select<UDT.Subject>();
                SubjectIDs.ForEach((x) =>
                {
                    this.dicSubjectIDs.Add(x.UID, x);
                });
            });
        }

        private void Student_SubjectScore_Load(object sender, EventArgs e)
        {
            //Util.InitSchoolYearNumberUpDown(this.nudSchoolYear);
            //Util.InitSemesterCombobox(this.cboSemester);           
            this.deletedScores = new List<UDT.SubjectSemesterScore>();
            this.dicCancelCourseMappings = new Dictionary<string, bool>();
            this.dataGridViewX1.BackgroundColor = Color.White;
            
            this.WatchChange(new DataGridViewSource(this.dataGridViewX1));
            Student.AfterChange += (x, y) => ReInitialize();
            //UDT.SubjectSemesterScore.AfterUpdate += new EventHandler<UDT.ParameterEventArgs>(SubjectSemesterScore_AfterUpdate);
            this.dataGridViewX1.DataError += new DataGridViewDataErrorEventHandler(dataGridViewX1_DataError);
            this.dataGridViewX1.CellEnter += new DataGridViewCellEventHandler(dataGridViewX1_CellEnter);
        }

        private void dataGridViewX1_CellEnter(object sender, DataGridViewCellEventArgs e)
        {
            this.dataGridViewX1.CommitEdit(DataGridViewDataErrorContexts.Commit);
        }

        private void dataGridViewX1_DataError(object sender, DataGridViewDataErrorEventArgs e)
        {
            e.Cancel = true;
        }

        public void SubjectSemesterScore_AfterUpdate(object sender, UDT.ParameterEventArgs e)
        {
            LoadSubjectSemesterScores(null);
            DataGridView_DataBinding();
        }
        protected override void OnCancelButtonClick(EventArgs e)
        {
            foreach(DataGridViewRow row in this.dataGridViewX1.Rows)
            {
                if (row.IsNewRow)
                    continue;

                foreach (DataGridViewCell cell in row.Cells)
                    cell.ErrorText = "";
            }
            LoadSubjectSemesterScores(null);
            DataGridView_DataBinding();
        }

        protected override void OnInitializeAsync()
        {
            this.subjects = (new AccessHelper()).Select<UDT.Subject>();
            this.dicSubjects = new Dictionary<string, UDT.Subject>();            
        }

        protected override void OnInitializeComplete(Exception error)
        {            
            if (error != null) //有錯就直接丟出去吧。
                throw error;

            //填入 Semester
            this.dicSemester = new Dictionary<string, int?>();
            this.dicSemester.Add(string.Empty, null);
            this.dicSemester.Add("夏季學期", 0);
            this.dicSemester.Add("第1學期", 1);
            this.dicSemester.Add("第2學期", 2);
            this.colSemester.Items.Add(string.Empty);
            this.colSemester.Items.Add("夏季學期");
            this.colSemester.Items.Add("第1學期");
            this.colSemester.Items.Add("第2學期");


            //foreach (SemesterItem si in SemesterItem.GetSemesterList())
            //{
            //    this.colSemester.Items.Add(si.Name);
            //    this.dicSemester.Add(si.Name, int.Parse(si.Value));
            //}

            //填入 Subjects
            this.colCourse.Items.Clear();
            this.subjects.Sort(delegate(UDT.Subject subj1, UDT.Subject subj2)
            {
                return subj1.NewSubjectCode.CompareTo(subj2.NewSubjectCode);
            });

            foreach (UDT.Subject subj in this.subjects)
            {
                this.colCourse.Items.Add(subj.NewSubjectCode);
                this.dicSubjects.Add(subj.NewSubjectCode, subj);
            }
            
        }

        protected override void OnPrimaryKeyChangedAsync()
        {
            LoadSubjectSemesterScores(null);
        }

        protected override void OnPrimaryKeyChangedComplete(Exception error)
        {
            if (error != null) //有錯就直接丟出去吧。
                throw error;

            this.updatedScores = new Dictionary<string, UDT.SubjectSemesterScore>();

            DataGridView_DataBinding();
        }

        private void DataGridView_DataBinding()
        {
            ErrorTip.Clear();

            this.deletedScores.Clear();

            this.dataGridViewX1.Rows.Clear();

            this.myScores.Sort(
                delegate(UDT.SubjectSemesterScore x, UDT.SubjectSemesterScore y)
                {
                    string order_x_key = "0";
                    string order_y_key = "0";
                    
                    //  抵免課程
                    if (string.IsNullOrEmpty(x.OffsetCourse))
                    {
                        order_x_key = "1";
                    }
                    if (string.IsNullOrEmpty(y.OffsetCourse))
                    {
                        order_y_key = "1";
                    }
                    //  學年
                    order_x_key += (x.SchoolYear.HasValue ? x.SchoolYear.Value.ToString("000") : "000");
                    order_y_key += (y.SchoolYear.HasValue ? y.SchoolYear.Value.ToString("000") : "000");
                    //  學期
                    order_x_key += (x.Semester.HasValue ? x.Semester.Value.ToString("0") : "0");
                    order_y_key += (y.Semester.HasValue ? y.Semester.Value.ToString("0") : "0");

                    //  流水號<100學年後為6碼，以前是5碼>
                    if (dicCourseExts.ContainsKey(x.CourseID))
                        order_x_key += dicCourseExts[x.CourseID].SerialNo.ToString("000000");
                    else
                        order_x_key += "000000";
                    if (dicCourseExts.ContainsKey(y.CourseID))
                        order_y_key += dicCourseExts[y.CourseID].SerialNo.ToString("000000");
                    else
                        order_y_key += "000000";

                    return order_x_key.CompareTo(order_y_key);
                });

            foreach (UDT.SubjectSemesterScore scr in this.myScores)
            {
                string strScoreConfirmed = string.Empty;
                
                if (this.dicCourseExts.ContainsKey(scr.CourseID))
                {
                    if (this.dicCourseExts[scr.CourseID].ScoreConfirmed)
                        strScoreConfirmed = "已確認並上傳";
                    else if (string.IsNullOrEmpty(scr.Score))
                        strScoreConfirmed = "未輸入";
                    else
                        strScoreConfirmed = "已暫存";
                }
                string new_subject_code = string.IsNullOrEmpty(scr.NewSubjectCode) ? (this.dicSubjectIDs.ContainsKey(scr.SubjectID.ToString()) ? this.dicSubjectIDs[scr.SubjectID.ToString()].NewSubjectCode : "") : scr.NewSubjectCode;
                object[] rawdata = new object[] { (scr.SchoolYear == null ? null : scr.SchoolYear.ToString()), (scr.Semester == null ? null : SemesterItem.GetSemesterByCode(scr.Semester.ToString()).Name), new_subject_code, scr.SubjectName, (this.dicCourseExts.ContainsKey(scr.CourseID) ? this.dicCourseExts[scr.CourseID].ClassName : ""), scr.IsRequired, scr.Credit, strScoreConfirmed, scr.Score, scr.IsPass, scr.OffsetCourse, scr.Remark, (this.dicCourseExts.ContainsKey(scr.CourseID) ? this.dicCourseExts[scr.CourseID].SerialNo.ToString() : "") };
                int rowIndex = this.dataGridViewX1.Rows.Add(rawdata);
                //int credit = 0;
                //int.TryParse(this.dataGridViewX1.Rows[rowIndex].Cells["colCredit"].Value + "", out credit);
                //if (credit == 0)
                //    this.dataGridViewX1.Rows[rowIndex].ReadOnly = true;
                this.dataGridViewX1.Rows[rowIndex].Tag = scr;
                this.dataGridViewX1.Rows[rowIndex].Cells[8].Tag = (this.dicCancelCourseMappings.ContainsKey(scr.CourseID.ToString()) ? true : false);
                if (this.dicCancelCourseMappings.ContainsKey(scr.CourseID.ToString()))
                {
                    this.dataGridViewX1.Rows[rowIndex].Cells[8].Value = "X";
                    this.dataGridViewX1.Rows[rowIndex].Cells[9].Value = false;
                }
            }

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

        private bool IsValidate()
        {
            //確定使用者修改的值都更新到控制項裡了(預防點選checkbox 後直接點選儲存，這時抓到的值仍是前一個值)。
            this.dataGridViewX1.EndEdit();
            Dictionary<string, List<DataGridViewRow>> dicRows = new Dictionary<string, List<DataGridViewRow>>();
            bool is_valid = true;
            foreach (DataGridViewRow row in this.dataGridViewX1.Rows)
            {
                //0：學年度
                //1：學期
                //2：課號
                //3：課程名稱
                //4：班次
                //5：必修
                //6：學分數
                //7：成績輸入進度
                //8：成績
                //9：取得學分
                //10：抵免課程
                //11：備註
                //12：流水號
                if (!row.IsNewRow)
                {
                    string school_year = row.Cells[0].Value + "";
                    string semester = row.Cells[1].Value + "";
                    string subject_code = row.Cells[2].Value + "";
                    string subject_name = row.Cells[3].Value + "";
                    string class_name = row.Cells[4].Value + "";
                    string is_required = row.Cells[5].Value + "";
                    string credit = row.Cells[6].Value + "";
                    string scored_progress = row.Cells[7].Value + ""; 
                    string score = row.Cells[8].Value + "";
                    string is_pass = row.Cells[9].Value + "";
                    string offset_course = row.Cells[10].Value + "";
                    string memo = row.Cells[11].Value + "";
                    string serial_no = row.Cells[12].Value + "";

                    bool changed = false;
                    bool bool_value = false;

                    UDT.SubjectSemesterScore sss = new UDT.SubjectSemesterScore();
                    if (row.Tag != null)
                    {
                        sss = row.Tag as UDT.SubjectSemesterScore;

                        if ((sss.SchoolYear + "") != school_year.Trim())
                            changed = true;
                        if (this.dicSemester[semester] != sss.Semester)
                            changed = true;
                        if (subject_code != sss.NewSubjectCode)
                            changed = true;
                        if (subject_name != sss.SubjectName)
                            changed = true;
                        if (bool.TryParse(is_required, out bool_value) && bool_value && !sss.IsRequired)
                            changed = true;
                        if ((!bool.TryParse(is_required, out bool_value) && sss.IsRequired) || (bool.TryParse(is_required, out bool_value) && !bool_value && sss.IsRequired))
                            changed = true;
                        if (credit != sss.Credit.ToString())
                            changed = true;
                        if (score != sss.Score)
                            changed = true;
                        if (bool.TryParse(is_pass, out bool_value) && bool_value && !sss.IsPass)
                            changed = true;
                        if ((!bool.TryParse(is_pass, out bool_value) && sss.IsPass) || (bool.TryParse(is_pass, out bool_value) && !bool_value && sss.IsPass))
                            changed = true;

                        if (changed && string.IsNullOrWhiteSpace(offset_course))
                        {
                            row.Cells[0].ErrorText = "非抵免課程僅可查詢，不可修改！";
                            is_valid = false;
                            continue;
                        }
                    }
                    else
                    {
                        if (string.IsNullOrWhiteSpace(offset_course))
                        {
                            row.Cells[0].ErrorText = "非抵免課程僅可查詢，不可新增！";
                            is_valid = false;
                            continue;
                        }
                    }

                    if (!dicRows.ContainsKey(school_year + "-" + subject_code))
                        dicRows.Add(school_year + "-" + subject_code, new List<DataGridViewRow>());

                    dicRows[school_year + "-" + subject_code].Add(row);
                    foreach(DataGridViewColumn column in this.dataGridViewX1.Columns)
                        row.Cells[column.Index].ErrorText = string.Empty;

                    //1. 如果學年度或學期為空，則視為抵免課程，必須輸入 "抵免課程" 欄位
                    //if (string.IsNullOrWhiteSpace(school_year) || string.IsNullOrWhiteSpace(semester))
                    //{
                    //    //必須是必修課程才能抵免
                    //    if (row.Cells[5].Value != null && bool.Parse(row.Cells[5].Value.ToString()))
                    //    {
                    //        DataGridViewCell cell = row.Cells[10];
                    //        if (cell.Value == null || string.IsNullOrWhiteSpace(cell.Value.ToString()))
                    //        {
                    //            cell.ErrorText = "請填入抵免課程資訊！";
                    //            is_valid = false;
                    //        }
                    //    }
                    //    else  //如果是選修，則依定不會是抵免課程，所以依定要有學年度學期
                    //    {
                    //        is_valid = this.cellCanNotBeNull(row.Cells[0], "請選擇學年度！");
                    //        is_valid = this.cellCanNotBeNull(row.Cells[1], "請選擇學期！");
                    //    }
                    //}
                    if (!string.IsNullOrWhiteSpace(offset_course))
                    {
                        //必須是必修課程才能抵免
                        bool required_course = false;
                        if (!(bool.TryParse(row.Cells[5].Value + "", out required_course) && required_course))
                        {
                            DataGridViewCell cell = row.Cells[5];
                            cell.ErrorText = "必修課程才能抵免！";
                            is_valid = false;
                        }
                        if (!string.IsNullOrWhiteSpace(school_year))
                        {
                            DataGridViewCell cell = row.Cells[0];
                            cell.ErrorText = "抵免課程之學年度應為空值！";
                            is_valid = false;
                        }
                        if (!string.IsNullOrWhiteSpace(semester))
                        {
                            DataGridViewCell cell = row.Cells[1];
                            cell.ErrorText = "抵免課程之學期應為空值！";
                            is_valid = false;
                        }
                        if (!string.IsNullOrWhiteSpace(score))
                        {
                            DataGridViewCell cell = row.Cells[8];
                            cell.ErrorText = "抵免課程之成績應為空值！";
                            is_valid = false;
                        }
                    }
                    //else    //如果學年度不為空值
                    //{
                    //    //檢查必填欄位
                    //    is_valid = this.cellCanNotBeNull(row.Cells[1], "請選擇學期！");
                    //}
                    if (!this.cellCanNotBeNull(row.Cells[2], "請選擇課程！"))
                        is_valid = false;
                    if (!this.cellCanNotBeNull(row.Cells[3], "請填入課程名稱！"))
                        is_valid = false;
                    if (!this.cellCanNotBeNull(row.Cells[6], "請填入學分數！"))
                        is_valid = false;
                    //this.cellCanNotBeNull(row.Cells[4], "請填入必選修！", errors);
                    
                    //this.cellCanNotBeNull(row.Cells[6], "請填入成績！", errors);

                    //學年度和學分數必須為整數
                    if (!this.cellMustBeInteger(row.Cells[0]))
                        is_valid = false;
                    if (!this.cellMustBeInteger(row.Cells[6]))
                        is_valid = false;

                    //成績必須是有效的成績
                    //if (string.IsNullOrWhiteSpace(offset_course))
                    //    is_valid = this.cellMustBeValidScore(row.Cells[8]);

                    //如果不是必修，則不能輸入抵免課程資訊
                    //if (! bool.Parse(row.Cells[5].Value.ToString()))
                    //{
                    //    DataGridViewCell cell = row.Cells[9];
                    //    if (cell.Value != null && !string.IsNullOrWhiteSpace(cell.Value.ToString()))
                    //    {
                    //        cell.ErrorText = "非必修課程不能抵免";
                    //        is_valid = false;
                    //    }
                    //}
                }
            }
            foreach (string key in dicRows.Keys)
            {
                if (dicRows[key].Count < 2)
                    continue;

                foreach (DataGridViewRow row in dicRows[key])
                {
                    row.Cells[0].ErrorText = "「學年度」與「課號」的組合重複。";
                    row.Cells[2].ErrorText = "「學年度」與「課號」的組合重複。";
                    is_valid = false;
                }
            }
            return is_valid;
        }

        private string makeScoreMsg(UDT.SubjectSemesterScore scr)
        {
            string msg = string.Format("{0}, {1}, 課號：{2}, 課程：{3}, 班次：{8}, 學分數:{4}, 必選修: {5}, 成績：{6}, 是否取得學分：{7}, 抵免課程：{9} ",
                    (scr.SchoolYear == null ? "" : scr.SchoolYear.ToString()),
                    (scr.Semester == null ? "" : DataItems.SemesterItem.GetSemesterByCode(scr.Semester.ToString()).Name),
                    scr.NewSubjectCode, scr.SubjectName, scr.Credit.ToString(),
                    scr.IsRequired.ToString(), scr.Score, scr.IsPass.ToString(), (this.dicCourseExts.ContainsKey(scr.CourseID) ? this.dicCourseExts[scr.CourseID].ClassName : ""), scr.OffsetCourse);
            return msg;
        }

        protected override void OnSaveData()
        {
            if (!IsValidate())
            {
                MessageBox.Show("請先修正錯誤再儲存。", "錯誤");
                return;
            }
            K12.Data.StudentRecord stud = K12.Data.Student.SelectByID(this.PrimaryKey);
            StringBuilder sb = new StringBuilder();
            sb.Append(string.Format("學生『{0}』的課程成績變動如下：\n", stud.Name));
           
            //Deleted Records
            List<ActiveRecord> recs = new List<ActiveRecord>();
            foreach (UDT.SubjectSemesterScore scr in this.deletedScores)
            {
                sb.Append(string.Format("刪除：{0}  \n",this.makeScoreMsg(scr)));
                scr.Deleted = true;
                recs.Add(scr);
            }

            //Inserted Or Updated Records
            foreach (DataGridViewRow row in this.dataGridViewX1.Rows)
            {
                if (!row.IsNewRow)
                {
                    UDT.SubjectSemesterScore scr = (UDT.SubjectSemesterScore)row.Tag;
                    if (row.Tag == null)
                    {
                        scr = new UDT.SubjectSemesterScore();
                        scr.StudentID = int.Parse(this.PrimaryKey);
                    }
                    else
                    {  
                        //如果不存在 updatedScores ，則跳下一筆
                        if (!this.updatedScores.ContainsKey(scr.UID))
                            continue;
                    }

                    if (row.Cells[0].Value == null)
                        scr.SchoolYear = null;
                    else
                        scr.SchoolYear =  int.Parse(row.Cells[0].Value.ToString());

                    if (row.Cells[1].Value == null)
                        scr.Semester = null;
                    else
                        scr.Semester =  this.dicSemester[row.Cells[1].Value.ToString()];

                    scr.NewSubjectCode = row.Cells[2].Value.ToString();
                    scr.SubjectID = int.Parse(this.dicSubjects[scr.NewSubjectCode].UID);
                    scr.SubjectName = row.Cells[3].Value.ToString();
                    scr.IsRequired = bool.Parse(row.Cells[5].Value.ToString());
                    scr.Credit = int.Parse(row.Cells[6].Value.ToString());
                    scr.Score = (row.Cells[8].Value == null ? "" : row.Cells[8].Value.ToString());
                    scr.IsPass = (row.Cells[9].Value==null) ? false : bool.Parse(row.Cells[9].Value.ToString());
                    scr.OffsetCourse = (row.Cells[10].Value == null ? "" : row.Cells[10].Value.ToString());
                    scr.Remark = (row.Cells[11].Value == null ? "" : row.Cells[11].Value.ToString());
                    
                    if (string.IsNullOrWhiteSpace(scr.UID))
                        sb.Append(string.Format("新增：{0}  \n", this.makeScoreMsg(scr)));
                    else
                        sb.Append(string.Format("修改：{0}  \n", this.makeScoreMsg(scr)));

                    recs.Add(scr);                    
                }
            }

            AccessHelper ah = new AccessHelper();
            //delete
            //if (this.deletedList.Count > 0)
            ah.SaveAll(recs);

            FISCA.LogAgent.ApplicationLog.Log("課程學期成績.學生", "修改", "student", this.PrimaryKey, sb.ToString());

            this.OnPrimaryKeyChanged(EventArgs.Empty);

            ResetDirtyStatus();
        }

        //private bool cellMustBeValidScore(DataGridViewCell cell)(本資料項目僅能抵免，不需輸入成績)
        //{
        //    if (cell == null )
        //        return false;

        //    if (cell.Value == null)
        //        return false;

        //    if (Util.IsValidScore(cell.Value.ToString()))
        //    {
        //        cell.ErrorText = string.Empty;
        //        return true;
        //    }
        //    else
        //    {
        //        string errMsg = "不是有效的分數";
        //        cell.ErrorText = errMsg;
        //        return false;
        //    }
        //}
        private bool cellCanNotBeNull(DataGridViewCell cell, string errMsg)
        {
            if (cell == null)
                return false;

            if (string.IsNullOrEmpty(cell.Value + ""))
            {
                cell.ErrorText = errMsg;
                return false;
            }
            else
                return true;
        }

        private bool cellMustBeInteger(DataGridViewCell cell)
        {
            if (cell.Value != null)
            {
                int intValue = 0;
                if (!int.TryParse(cell.Value.ToString(), out intValue))
                {
                    cell.ErrorText = "必須是整數數字";

                    return false;
                }
                else
                    return true;
            }
            else
                return true;
        }
        
        private void dataGridViewX1_CurrentCellDirtyStateChanged(object sender, EventArgs e)
        {
            this.dataGridViewX1.CommitEdit(DataGridViewDataErrorContexts.Commit);
            DataGridViewCell cell = this.dataGridViewX1.CurrentCell;
            DataGridViewRow row = this.dataGridViewX1.Rows[cell.RowIndex];
            //紀錄被更改的成績物件
            UDT.SubjectSemesterScore src = (UDT.SubjectSemesterScore)row.Tag;
            if (src != null)
            {
                if (!this.updatedScores.ContainsKey(src.UID))
                    this.updatedScores.Add(src.UID, src);
            }

            //如果選課程，要填入課程名稱、學分數、必選修等資訊
            if (cell.ColumnIndex == 2)
            {
                if (cell.Value == null)
                {
                    row.Cells[3].Value = null;
                    row.Cells[5].Value = false;
                    row.Cells[6].Value = null;                    
                }
                else
                {
                    UDT.Subject subj = this.dicSubjects[cell.Value.ToString()];
                    row.Cells[3].Value = subj.Name;
                    row.Cells[5].Value = subj.IsRequired;
                    row.Cells[6].Value = subj.Credit;                    
                }                
            }
            //如果填入分數，則判斷是否取得學分 (本資料項目僅能抵免，不需輸入成績)
            //if (cell.ColumnIndex == 8)
            //{
            //    if (cell.Value == null)
            //        row.Cells[9].Value = false;
            //    else
            //    {
            //        string score = cell.Value.ToString().ToUpper();  //變成大寫
            //        if (Util.IsValidScore(score))
            //        {
            //            cell.Value = score;
            //            cell.ErrorText = string.Empty;
            //        }
            //        else
            //            cell.ErrorText = "不是有效的分數";

            //        row.Cells[9].Value = Util.IsPass(cell.Value.ToString());
            //    }
            //}
        }

        //載入此學生的所有科目學期成績
        private void LoadSubjectSemesterScores(IEnumerable<string> uids)
        {
            if (uids == null || uids.Count() == 0)
                this.myScores = (new AccessHelper()).Select<UDT.SubjectSemesterScore>("ref_student_id=" + this.PrimaryKey);
            else
                this.myScores = (new AccessHelper()).Select<UDT.SubjectSemesterScore>(string.Format(@"ref_student_id={0} and uid in ({1})", this.PrimaryKey, string.Join(",", uids)));

            QueryHelper q = new QueryHelper();
            string strSQL = string.Format(@"select att.ref_course_id from student stu inner join $ischool.emba.scattend_ext att on att.ref_student_id = stu.id where att.is_cancel=true and stu.id={0}", this.PrimaryKey);
            DataTable dt = q.Select(strSQL);
            this.dicCancelCourseMappings.Clear();
            foreach (DataRow dr in dt.Rows)
            {
                if (!dicCancelCourseMappings.ContainsKey(dr["ref_course_id"] + ""))
                    dicCancelCourseMappings.Add(dr["ref_course_id"] + "", true);
            }               
        }

        private void dataGridViewX1_UserDeletingRow(object sender, DataGridViewRowCancelEventArgs e)
        {
            if (e.Row.IsNewRow) return;

            string offset_course = e.Row.Cells[10].Value + "";

            if (string.IsNullOrWhiteSpace(offset_course))
            {
                MessageBox.Show("非抵免課程僅可查詢，不可刪除！");
                return;
            }

            if (MessageBox.Show("您確定要移除這筆成績紀錄？", "注意", MessageBoxButtons.YesNo, MessageBoxIcon.Information) != DialogResult.Yes)
            {
                e.Cancel = true;
                return;
            }
        }

        private void dataGridViewX1_UserDeletedRow(object sender, DataGridViewRowEventArgs e)
        {
            if (e.Row.IsNewRow) return;
            if (e.Row.Tag != null)
                this.deletedScores.Add((UDT.SubjectSemesterScore)e.Row.Tag);
        }

        private void dataGridViewX1_CellEndEdit(object sender, DataGridViewCellEventArgs e)
        {
            if (e.ColumnIndex == 8)
            {
                bool is_cancel = false;
                bool.TryParse(this.dataGridViewX1.Rows[e.RowIndex].Cells[8].Tag + "", out is_cancel);
                if (is_cancel)
                {
                    this.dataGridViewX1.Rows[e.RowIndex].Cells[8].Value = "X";
                    this.dataGridViewX1.Rows[e.RowIndex].Cells[9].Value = false;
                }
            }
        }

        private void dataGridViewX1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            //if (e.ColumnIndex == 7)
            //{
            //    if ((bool)this.dataGridViewX1.Rows[e.RowIndex].Cells[6].Tag)
            //    {
            //        this.dataGridViewX1.Rows[e.RowIndex].Cells[6].Value = "X";
            //        (this.dataGridViewX1.Rows[e.RowIndex].Cells[7] as DataGridViewCheckBoxCell).Value = false;
            //        this.dataGridViewX1.CurrentCell = null;
            //    }
            //    else
            //    {
            //        if ((this.dataGridViewX1.Rows[e.RowIndex].Cells[6].Value + "").ToUpper() == "X")
            //        {
            //            (this.dataGridViewX1.Rows[e.RowIndex].Cells[7] as DataGridViewCheckBoxCell).Value = false;
            //            this.dataGridViewX1.CurrentCell = null;
            //        }
            //    }
            //}
        }

        private void dataGridViewX1_CellContentDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
        //    if (e.ColumnIndex == 7)
        //    {
        //        if ((bool)this.dataGridViewX1.Rows[e.RowIndex].Cells[6].Tag)
        //        {
        //            this.dataGridViewX1.Rows[e.RowIndex].Cells[6].Value = "X";
        //            (this.dataGridViewX1.Rows[e.RowIndex].Cells[7] as DataGridViewCheckBoxCell).Value = false;
        //            this.dataGridViewX1.CurrentCell = null;
        //        }
        //        else
        //        {
        //            if ((this.dataGridViewX1.Rows[e.RowIndex].Cells[6].Value + "").ToUpper() == "X")
        //            {
        //                (this.dataGridViewX1.Rows[e.RowIndex].Cells[7] as DataGridViewCheckBoxCell).Value = false;
        //                this.dataGridViewX1.CurrentCell = null;
        //            }
        //        }
        //    }
        }
    }
}
