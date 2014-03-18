using System;
using System.Collections.Generic;
using System.Windows.Forms;
using Campus.Windows;
using EMBACore.DataItems;
using FISCA.Permission;
using FISCA.UDT;
using SHSchool.Data;
using FISCA.Presentation;

namespace EMBACore.DetailItems
{
    [AccessControl("ischool.EMBA.Course_BasicInfo", "課程資訊", "課程>資料項目")]
    public partial class Course_BasicInfo : DetailContentImproved
    {
        private SHCourseRecord Record = null;
        private UDT.CourseExt Course2 = null;

        private List<ClassItem> ClassRowSource = new List<ClassItem>();
        private List<UDT.Subject> subjects = new List<UDT.Subject>();
        private Dictionary<string, UDT.Subject> dicSubjects = new Dictionary<string, UDT.Subject>();
        private Dictionary<string, UDT.Subject> dicSubjectsByNewSubjectCode = new Dictionary<string, UDT.Subject>();    //  by 課號
        private Dictionary<string, UDT.Subject> dicSubjectsBySubjectIdentifier = new Dictionary<string, UDT.Subject>();    //  by 課號


        /*   Log  */
        private Log.LogAgent logAgent = new Log.LogAgent();

        private bool isTriggerByCboSubject = false;     //由 cboSubject 所觸發
        private bool isTriggerByCboNewSubjectCode = false;   //由 cboNewSubjectCode 所觸發

        public Course_BasicInfo()
        {
            InitializeComponent();
            this.Group = "課程基本資料";
        }

        private void Course_BasicInfo_Load(object sender, EventArgs e)
        {            
            WatchChange(new ComboBoxSource(this.cboCategory, ComboBoxSource.ListenAttribute.Text));
            WatchChange(new ComboBoxSource(this.cboClass, ComboBoxSource.ListenAttribute.Text));
            WatchChange(new ComboBoxSource(this.cboSemester, ComboBoxSource.ListenAttribute.Text));
            WatchChange(new ComboBoxSource(this.cboSubject, ComboBoxSource.ListenAttribute.Text));
            WatchChange(new ComboBoxSource(this.cboNewSubjectCode, ComboBoxSource.ListenAttribute.Text));

            WatchChange(new TextBoxSource(this.txtCourseName));
            WatchChange(new TextBoxSource(this.txtSubjectCode));

            WatchChange(new TextBoxSource(this.nudCredit));
            WatchChange(new TextBoxSource(this.nudSerialNo));
            WatchChange(new TextBoxSource(this.nudCountLimit));
            WatchChange(new TextBoxSource(this.txtClassroom));
            WatchChange(new TextBoxSource(this.txtCourseTimeInfo));
            WatchChange(new TextBoxSource(this.txtOutline));
            WatchChange(new TextBoxSource(this.txtMemo));
            WatchChange(new NumericUpDownSource(this.nudSchoolYear));

            SHClass.AfterChange += (x, y) => ReInitialize();
            UDT.Subject.AfterUpdate += (x, y) => ReInitialize();
            //K12.Data.Course.AfterChange += (x, y) => ReInitialize();
            //UDT.CourseExt.AfterUpdate += new EventHandler<UDT.ParameterEventArgs>(CourseExt_AfterUpdate);

            this.swb.ValueChanged += new EventHandler(swb_ValueChanged);

        }

        private void CourseExt_AfterUpdate(object sender, UDT.ParameterEventArgs e)
        {
            if (InvokeRequired)
            {
                BeginInvoke(new Action<object, UDT.ParameterEventArgs>(CourseExt_AfterUpdate), sender, e);
            }
            else
            {
                if (PrimaryKey != "")
                {
                    //OnPrimaryKeyChanged();
                    //要實做以下的東東，但OnPrimaryKeyChangedAsync變為無效

                    //  檢視不同資料項目即呼叫此方法，PrimaryKey 為資料項目的 Key 值。
                    //protected override void OnPrimaryKeyChanged(EventArgs e)
                    //{
                    //    if (!this._BGWLoadData.IsBusy)
                    //    {
                    //        this.Loading = true;
                    //        this._RunningKey = PrimaryKey;
                    //        this._BGWLoadData.RunWorkerAsync();
                    //    }
                    //}
                }
            }
        }


        void swb_ValueChanged(object sender, EventArgs e)
        {
            if (this.IsInitialized)
            {
                if (UserAcl.Current[GetType()].Editable)
                    this.SaveButtonVisible = true;
                else
                    SaveButtonVisible = false;

                this.CancelButtonVisible = true;
            }
        }

        private void LoadClassRowSource()
        {
            ClassRowSource.Clear();
            ClassRowSource.Add(new ClassItem("", "", ""));
            ClassRowSource.Add(new ClassItem("01", "01",""));
            ClassRowSource.Add(new ClassItem("02", "02",""));
            ClassRowSource.Add(new ClassItem("03", "03",""));

            //Get All Subjects
            this.dicSubjects.Clear();
            AccessHelper ah = new AccessHelper();
            this.subjects = ah.Select<UDT.Subject>();
            foreach (UDT.Subject subj in this.subjects)
            {
                this.dicSubjects.Add(subj.UID, subj);

                if (!string.IsNullOrWhiteSpace(subj.NewSubjectCode))
                    this.dicSubjectsByNewSubjectCode.Add(subj.NewSubjectCode, subj);

                if (!string.IsNullOrWhiteSpace(subj.SubjectCode))
                    this.dicSubjectsBySubjectIdentifier.Add(subj.SubjectCode, subj);
            }
        }

        private void BindClassRowSource()
        {
            //Assign Class List
            cboClass.DataSource = ClassRowSource;
            cboClass.ValueMember = "ID";
            cboClass.DisplayMember = "Name";

            //Assign Subject List
            this.cboSubject.DataSource = this.subjects;
            this.cboSubject.ValueMember = "UID";
            //this.cboSubject.DisplayMember = "Name";
            this.cboSubject.DisplayMember = "SubjectCode";
            this.cboSubject.AutoCompleteMode = AutoCompleteMode.Suggest;
            this.cboSubject.AutoCompleteSource = AutoCompleteSource.ListItems;
            this.cboSubject.SelectedIndexChanged += new EventHandler(cboSubject_SelectedIndexChanged);

            //Assign Semesters
            this.cboSemester.DataSource = SemesterItem.GetSemesterList();
            this.cboSemester.ValueMember = "Value";
            this.cboSemester.DisplayMember = "Name";

            //Assign Course Category
            this.cboCategory.Items.Clear();
            this.cboCategory.Items.Add("核心必修");
            this.cboCategory.Items.Add("核心選修");
            this.cboCategory.Items.Add("分組必修");
            this.cboCategory.Items.Add("選修");

            //Assign NewSubjectCode 
            List<string> subjCodes = new List<string>(this.dicSubjectsByNewSubjectCode.Keys);
            subjCodes.Sort();
            foreach (string code in subjCodes)
            {
                this.cboNewSubjectCode.Items.Add(code);
            }
            this.cboNewSubjectCode.AutoCompleteMode = AutoCompleteMode.Suggest;
            this.cboNewSubjectCode.AutoCompleteSource = AutoCompleteSource.ListItems;
            this.cboNewSubjectCode.SelectedIndexChanged += new EventHandler(cboNewSubjectCode_SelectedIndexChanged);
        }

        void cboSubject_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (IsInitialized)
            {
                if (string.IsNullOrWhiteSpace(this.cboSubject.Text )) return;
                if (this.isTriggerByCboNewSubjectCode) return; //避免無窮迴圈

                this.isTriggerByCboSubject = true;  //確保更新 cboNewSubjectCode 時不會造成無窮迴圈。

                string subjectCode = this.cboSubject.Text;
                UDT.Subject subj = (this.dicSubjectsBySubjectIdentifier.ContainsKey(subjectCode) ? this.dicSubjectsBySubjectIdentifier[subjectCode] : null);
               
                if (subj != null)
                {
                    this.nudCredit.Text = subj.Credit.ToString();
                    this.swb.Value = subj.IsRequired;
                    this.txtSubjectCode.Text = subj.Name;
                    this.cboNewSubjectCode.Text = subj.NewSubjectCode;
                }

                this.isTriggerByCboSubject = false;
            }
        }

        void cboNewSubjectCode_SelectedIndexChanged(object sender, EventArgs e)
        {
            //找出目前選擇 subject code 的課程，並更新
            if (IsInitialized)
            {
                if (string.IsNullOrWhiteSpace(this.cboNewSubjectCode.Text )) return;
                if (this.isTriggerByCboSubject)  return;  //避免無窮迴圈

                this.isTriggerByCboNewSubjectCode = true;  //確保更新 cboSubject 時不會造成無窮迴圈。

                string subjectCode = this.cboNewSubjectCode.Text;
                UDT.Subject subj = (this.dicSubjectsByNewSubjectCode.ContainsKey(subjectCode) ? this.dicSubjectsByNewSubjectCode[subjectCode] : null);

                if (subj != null)
                {
                    this.nudCredit.Text = subj.Credit.ToString();
                    this.swb.Value = subj.IsRequired;
                    this.txtSubjectCode.Text = subj.Name;
                    this.cboSubject.SelectedValue = subj.UID;
                }

                this.isTriggerByCboNewSubjectCode = false;  
            }

        }

        protected override void OnInitializeAsync()
        {
            LoadClassRowSource();
        }

        protected override void OnInitializeComplete(Exception error)
        {
            if (error != null) //有錯就直接丟出去吧。
                throw error;

            BindClassRowSource();
        }

        protected override void OnPrimaryKeyChangedAsync()
        {
            //Get the Course Record
            Record = SHCourse.SelectByID(PrimaryKey);

            //Get the Course_Ext Record
            AccessHelper ah = new AccessHelper();
            List<UDT.CourseExt> moreInfo = ah.Select<UDT.CourseExt>("ref_course_id='" + this.PrimaryKey + "'");
            this.Course2 = (moreInfo.Count > 0) ? moreInfo[0] : null;
        }

        protected override void OnPrimaryKeyChangedComplete(Exception error)
        {
            if (Record == null)
                throw new Exception("資料可能已經被其他人刪除。");

            if (error != null) //有錯就直接丟出去吧。
                throw error;

            ErrorTip.Clear();


            this.txtCourseName.Text = this.Record.Name ;
            this.nudSchoolYear.Value = (decimal)this.Record.SchoolYear ;

            this.cboSemester.SelectedValue = this.Record.Semester.ToString();
            this.cboClass.SelectedValue = (this.Course2 == null) ? "" : this.Course2.ClassName;

            string targetSubjectID =  (this.Course2 == null) ? "" : this.Course2.SubjectID.ToString() ;

            if (  (this.cboSubject.SelectedValue != null) && (this.cboSubject.SelectedValue.ToString() != targetSubjectID))
            {
                this.cboSubject.SelectedValue =targetSubjectID;
                //this.txtSubjectCode.Text = "";  //clear & init , value will be filled after cboSubject.selectedIndexChanged event.
            }

            this.cboNewSubjectCode.Text = (this.Course2 == null) ? "" : this.Course2.NewSubjectCode.ToString();
            
            this.nudCredit.Text = (this.Record.Credit == null ? "" : this.Record.Credit.ToString() );
            //this.swb.Value = this.Record.Required;

            //this.txtNewSubjectCode.Text = (this.Course2 == null) ? "" : this.Course2.NewSubjectCode;
            this.cboCategory.SelectedItem = (this.Course2 == null) ? "" : this.Course2.CourseType;
            this.swb.Value = (this.Course2 == null) ? false : this.Course2.IsRequired ;
            this.nudCountLimit.Text = (this.Course2 == null) ? "" : this.Course2.Capacity.ToString() ;
            this.nudSerialNo.Text = (this.Course2 == null) ? "" : this.Course2.SerialNo.ToString() ;

            this.txtClassroom.Text = (this.Course2 == null) ? "" : this.Course2.Classroom;
            this.txtCourseTimeInfo.Text = (this.Course2 == null) ? "" : this.Course2.CourseTimeInfo;
            this.txtOutline.Text = (this.Course2 == null) ? "" : this.Course2.Syllabus;
            this.txtMemo.Text = (this.Course2 == null) ? "" : this.Course2.Memo;

            ResetDirtyStatus();

            /* Log */
            this.logAgent.Clear();
            this.AddLog(this.Record, this.Course2);
        }

        protected override void OnDirtyStatusChanged(ChangeEventArgs e)
        {
            if (UserAcl.Current[this.GetType()].Editable)
                SaveButtonVisible = e.Status == ValueStatus.Dirty;
            else
                this.SaveButtonVisible = false;

            CancelButtonVisible = e.Status == ValueStatus.Dirty;
        }

        protected override void OnValidateData(Dictionary<Control, string> errors)
        {
            if (string.IsNullOrWhiteSpace(this.txtCourseName.Text))
                errors.Add(cboClass, "請輸入課程名稱。");

            if (cboClass.SelectedIndex < 0)
                errors.Add(cboClass, "請選擇清單中的班級。");

            if (!this.dicSubjectsBySubjectIdentifier.ContainsKey(this.cboSubject.Text))
               errors.Add(cboSubject, "請選擇清單中的課程識別。");

           if (this.cboCategory.SelectedIndex < 0)
               errors.Add(cboCategory, "請選擇清單中的類別。");

            //if (string.IsNullOrWhiteSpace(this.txtSubjectCode.Text))
            //    errors.Add(txtSubjectCode, "請輸入科目代號。");
           if (!this.dicSubjectsByNewSubjectCode.ContainsKey(this.cboNewSubjectCode.Text))
           {
               errors.Add(cboNewSubjectCode, "請選擇清單中的課號");
           }
            //流水號必須是數字
           int serial = 0;
           bool isInt = int.TryParse(this.nudSerialNo.Text, out serial);
           if (!isInt)
           {
               errors.Add(this.nudSerialNo, "流水號必須為數字！");
           }
           //人數上限必須是數字
           int capacity = 0;
           if (!int.TryParse(this.nudCountLimit.Text, out capacity))
           {
               errors.Add(this.nudCountLimit, "人數上限必須為數字！");
           }
        }

        protected override void OnSaveData()
        {
            //Save To UDT
            UDT.CourseExt c = this.Course2;
            if (this.Course2 == null)
                c = new UDT.CourseExt() ;
            c.CourseID = int.Parse(this.PrimaryKey);
            string subjCode = this.cboSubject.Text ;
            UDT.Subject subj = this.dicSubjectsBySubjectIdentifier[subjCode];

            c.SubjectID = int.Parse(subj.UID);
            c.SubjectCode = subjCode;
            //c.NewSubjectCode = this.txtNewSubjectCode.Text;
            c.NewSubjectCode = this.cboNewSubjectCode.Text;
            c.CourseType = this.cboCategory.SelectedItem.ToString();
            c.IsRequired = this.swb.Value;

            int capacity = 0;
            int.TryParse(this.nudCountLimit.Text, out capacity);

            int serial = 0;
            int.TryParse(this.nudSerialNo.Text, out serial);

            c.Capacity = capacity;
            c.SerialNo = serial;
            c.ClassName = this.cboClass.SelectedValue.ToString();

            c.Classroom = this.txtClassroom.Text.Trim();
            c.CourseTimeInfo = this.txtCourseTimeInfo.Text.Trim();
            c.Syllabus = this.txtOutline.Text.Trim();
            c.Memo = this.txtMemo.Text.Trim();

            List<ActiveRecord> recs = new List<ActiveRecord>();
            recs.Add(c);
            AccessHelper ah = new AccessHelper();
            ah.SaveAll(recs);

            //Save to DB
            this.Record.Name = this.txtCourseName.Text;
            this.Record.SchoolYear = (int)this.nudSchoolYear.Value;
            this.Record.Semester = int.Parse(this.cboSemester.SelectedValue.ToString());
            //this.Record.RefClassID = this.cboClass.SelectedValue.ToString();
            this.Record.Subject = this.cboSubject.Text;

            decimal credit = 0;
            decimal.TryParse(this.nudCredit.Text, out credit);
            this.Record.Credit = credit;

            //this.Record.Required = this.swb.Value;
            SHCourse.Update(this.Record);

            /* Log */
            this.AddLog(this.Record, c);
            this.logAgent.ActionType = Log.LogActionType.Update;
            this.logAgent.Save("課程.資料項目.基本資料", "", "", Log.LogTargetCategory.Course, this.Record.ID);

            /* */
            //K12.Presentation.NLDPanels.Course.RefillListPane();

            ResetDirtyStatus();
        }

        void AddLog(SHCourseRecord objCourse, UDT.CourseExt udtCourse)
        {
            this.logAgent.SetLogValue("開課", objCourse.Name);
            this.logAgent.SetLogValue("學年度", objCourse.SchoolYear.ToString());
            this.logAgent.SetLogValue("學期", SemesterItem.GetSemesterByCode(objCourse.Semester.ToString()).Name);
            this.logAgent.SetLogValue("開課班次", (udtCourse == null) ? "" : udtCourse.ClassName);
            this.logAgent.SetLogValue("類別", (udtCourse == null) ? "" : udtCourse.CourseType);
            this.logAgent.SetLogValue("課程識別", (udtCourse == null) ? "" : udtCourse.SubjectCode);
            //this.logAgent.SetLogValue("開課課程", udtCourse.);
            this.logAgent.SetLogValue("學分數", objCourse.Credit.ToString());
            this.logAgent.SetLogValue("必選修", (udtCourse == null) ? "" : udtCourse.IsRequired.ToString());
            this.logAgent.SetLogValue("流水號", (udtCourse == null) ? "" : udtCourse.SerialNo.ToString());
            this.logAgent.SetLogValue("人數上限", (udtCourse == null) ? "" : udtCourse.Capacity.ToString());

            this.logAgent.SetLogValue("教室", (udtCourse == null) ? "" : udtCourse.Classroom);
            this.logAgent.SetLogValue("上課時間", (udtCourse == null) ? "" : udtCourse.CourseTimeInfo);
            this.logAgent.SetLogValue("課程大綱", (udtCourse == null) ? "" : udtCourse.Syllabus);
            this.logAgent.SetLogValue("備註", (udtCourse == null) ? "" : udtCourse.Memo);
        }

        private void cboSubject_TextChanged(object sender, EventArgs e)
        {
            string text = this.cboSubject.Text;
            if (this.dicSubjectsBySubjectIdentifier.ContainsKey(text))
            {
                this.errorProvider1.SetError(this.cboSubject, "");
                //更新其它兩個控制項
                this.cboSubject_SelectedIndexChanged(null, EventArgs.Empty);
            }
            else
            {
                this.errorProvider1.SetError(this.cboSubject, "請選擇清單中的識別碼");
                //this.cboSubject.Focus();
            }
        }

        private void cboNewSubjectCode_TextChanged(object sender, EventArgs e)
        {
            string text = this.cboNewSubjectCode.Text;
            if (this.dicSubjectsByNewSubjectCode.ContainsKey(text))
            {
                this.errorProvider1.SetError(this.cboNewSubjectCode, "");
                this.cboNewSubjectCode_SelectedIndexChanged(null, EventArgs.Empty);
            }
            else
            {
                this.errorProvider1.SetError(this.cboNewSubjectCode, "請選擇清單中的課號");
                //this.cboNewSubjectCode.Focus();
            }
        }

        private void cboSubject_Leave(object sender, EventArgs e)
        {
            this.cboSubject_TextChanged(null, null);
        }

        private void cboNewSubjectCode_Leave(object sender, EventArgs e)
        {
            this.cboNewSubjectCode_TextChanged(null, null);
        }

        private void linkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            if (!string.IsNullOrWhiteSpace(this.txtOutline.Text))
                System.Diagnostics.Process.Start(this.txtOutline.Text);
        }
    }
}