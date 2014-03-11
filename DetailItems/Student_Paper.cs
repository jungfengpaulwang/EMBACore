using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using FISCA;
using FISCA.Presentation;
using FISCA.Permission;
using UDTDetailContentBase;
using Campus.Windows;
using EMBACore.DataItems;
using FISCA.Data;
using FISCA.UDT;
using EMBACore.UDT;
using System.Xml.Linq;
using FISCA.LogAgent;
using EMBACore.Extension.UDT;

namespace EMBACore.DetailItems
{
    [FeatureCode("ischool.EMBA.Student.Student_Paper", "指導教授及論文")]
    public partial class Student_Paper  : FISCA.Presentation.DetailContent
    {
        //  驗證資料物件
        private ErrorProvider _Errors;

        //  背景載入 UDT 資料物件
        private BackgroundWorker _BGWLoadData;
        private BackgroundWorker _BGWSaveData;

        //  監控 UI 資料變更
        private ChangeListen _Listener;

        //  正在下載的資料之主鍵，用於檢查是否下載他人資料，若 _RunningKey != PrimaryKey 就再下載乙次
        private string _RunningKey;

        //  記錄所有 UDT 資料，便於比對待刪除資料
        private Dictionary<string, UDT.Paper> _dicUTDs;

        //  _BGWLoadData_DoWork 之  e.Result，包含所有被下載的資料物件。
        private object _Result;
        private AccessHelper Access;
        private QueryHelper queryHelper;

        public Student_Paper()
        {
            InitializeComponent();
            this.Group = "指導教授及論文";

            this.Load += new EventHandler(Student_Paper_Load);
        }

        private void Student_Paper_Load(object sender, EventArgs e)
        {
            _Errors = new ErrorProvider();

            _Listener = new ChangeListen();
            _Listener.Add(new TextBoxSource(this.txtPaperName));
            _Listener.Add(new TextBoxSource(this.txtDescription));
            _Listener.Add(new TextBoxSource(this.txtAdvisor1));
            _Listener.Add(new TextBoxSource(this.txtAdvisor2));
            _Listener.Add(new TextBoxSource(this.txtAdvisor3));
            _Listener.Add(new TextBoxSource(this.nudSchoolYear));
            _Listener.Add(new ComboBoxSource(this.cboSemester, ComboBoxSource.ListenAttribute.Text));
            this.IsPublic.ValueChanged += new EventHandler(IsPublic_ValueChanged);
            //_Listener.Add(new CheckBoxSource(this.IsPublic));
            _Listener.Add(new TextBoxSource(this.txtPublishedDate));
            _Listener.StatusChanged += new EventHandler<ChangeEventArgs>(Listener_StatusChanged);

            _BGWLoadData = new BackgroundWorker();
            _BGWLoadData.DoWork += new DoWorkEventHandler(_BGWLoadData_DoWork);
            _BGWLoadData.RunWorkerCompleted += new RunWorkerCompletedEventHandler(_BGWLoadData_RunWorkerCompleted);

            _BGWSaveData = new BackgroundWorker();
            _BGWSaveData.DoWork += new DoWorkEventHandler(_BGWSaveData_DoWork);
            _BGWSaveData.RunWorkerCompleted += new RunWorkerCompletedEventHandler(_BGWSaveData_RunWorkerCompleted);

            this.nudSchoolYear.TextChanged += new EventHandler(SchoolYear_TextChanged);

            _RunningKey = "";
            _dicUTDs = new Dictionary<string, UDT.Paper>();

            Access = new AccessHelper();
            queryHelper = new QueryHelper();

            BindSemester(string.Empty);
        }

        void IsPublic_ValueChanged(object sender, EventArgs e)
        {
            if (UserAcl.Current[typeof(Student_Paper)].Editable)
                this.SaveButtonVisible = true;
            else
                this.SaveButtonVisible = false;

            this.CancelButtonVisible = true;
        }

        private void BindSemester(string semester)
        {
            this.cboSemester.DataSource = SemesterItem.GetSemesterList();
            this.cboSemester.ValueMember = "Value";
            this.cboSemester.DisplayMember = "Name";

            this.cboSemester.SelectedValue = semester;
        }

        private void _BGWLoadData_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if (e.Error != null)
            {
                ResetOverrideButton();
                this._Result = null;
                this.ClearUI();
                this.Loading = false;
                return;
            }

            if (_RunningKey != PrimaryKey)
            {
                this.Loading = true;
                this._RunningKey = PrimaryKey;
                this._BGWLoadData.RunWorkerAsync();
            }
            else
            {
                this._Result = e.Result;

                this.RefreshUI();
            }
        }

        private void _BGWLoadData_DoWork(object sender, DoWorkEventArgs e)
        {
            List<UDT.Paper> records = Access.Select<UDT.Paper>(string.Format("ref_student_id = {0}", PrimaryKey));

            e.Result = new object[] { records };
        }

        private void _BGWSaveData_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            this._BGWLoadData.RunWorkerAsync();
        }

        private void _BGWSaveData_DoWork(object sender, DoWorkEventArgs e)
        {
            this.Loading = true;
            SaveUDT(e.Argument.ToString());
        }

        private void Listener_StatusChanged(object sender, ChangeEventArgs e)
        {
            if (UserAcl.Current[typeof(Student_Paper)].Editable)
                SaveButtonVisible = e.Status == ValueStatus.Dirty;
            else
                this.SaveButtonVisible = false;

            CancelButtonVisible = e.Status == ValueStatus.Dirty;
        }

        //  檢視不同資料項目即呼叫此方法，PrimaryKey 為資料項目的 Key 值。
        protected override void OnPrimaryKeyChanged(EventArgs e)
        {
            if (!this._BGWLoadData.IsBusy)
            {
                this.Loading = true;
                this._RunningKey = PrimaryKey;
                this._BGWLoadData.RunWorkerAsync();
            }
        }

        //  清除畫面上的所有資料
        private void ClearUI()
        {
            this.txtPaperName.Text = string.Empty;
            this.txtDescription.Text = string.Empty;
            this.txtAdvisor1.Text = string.Empty;
            this.txtAdvisor2.Text = string.Empty;
            this.txtAdvisor3.Text = string.Empty;
            this.nudSchoolYear.Text = string.Empty;
            this.cboSemester.Text = string.Empty; 
            this.cboSemester.SelectedValue = string.Empty;
            this.IsPublic.Value = false;
            this.txtPublishedDate.Text = string.Empty;
        }

        //  更新資料項目內 UI 的資料
        private void RefreshUI()
        {
            _Listener.SuspendListen();

            this.ClearUI();
            _dicUTDs.Clear();

            List<UDT.Paper> records = ((object[])this._Result)[0] as List<UDT.Paper>;

            if (records != null && records.Count > 0)
            {
                this.txtPaperName.Text = records[0].PaperName;
                this.txtDescription.Text = records[0].Description;
                this.txtAdvisor1.Text = ReverseAdvisor(records[0].AdvisorList).ElementAt(0);
                this.txtAdvisor2.Text = ReverseAdvisor(records[0].AdvisorList).ElementAt(1);
                this.txtAdvisor3.Text = ReverseAdvisor(records[0].AdvisorList).ElementAt(2);
                this.nudSchoolYear.Text = records[0].SchoolYear;
                BindSemester(records[0].Semester);
                this.IsPublic.Value = records[0].IsPublic;
                this.txtPublishedDate.Text = records[0].PublishedDate;

                _dicUTDs.Add(records[0].StudentID.ToString(), records[0]);
            }

            this.Loading = false;
            ResetOverrideButton();
        }
        
        private string ParseAdvisor()
        {
            StringBuilder sb = new StringBuilder();

            sb.Append("<Advisor TeacherID=''><Name>" + txtAdvisor1.Text.Trim() + "</Name></Advisor>");
            sb.Append("<Advisor TeacherID=''><Name>" + txtAdvisor2.Text.Trim() + "</Name></Advisor>");
            sb.Append("<Advisor TeacherID=''><Name>" + txtAdvisor3.Text.Trim() + "</Name></Advisor>");

            return sb.ToString();
        }

        private List<string> ReverseAdvisor(string advisorList)
        {
            XDocument xDocument = XDocument.Parse("<root>" + advisorList + "</root>");
            List<string> advisors = new List<string>();
            foreach (XElement xElement in xDocument.Descendants("Advisor"))
                advisors.Add(xElement.Element("Name").Value);

            return advisors;
        }

        private void ResetOverrideButton()
        {
            SaveButtonVisible = false;
            CancelButtonVisible = false;

            _Listener.Reset();
            _Listener.ResumeListen();
        }

        protected override void OnCancelButtonClick(EventArgs e)
        {
            if (!_BGWLoadData.IsBusy)
                this._BGWLoadData.RunWorkerAsync();
        }

        protected override void OnSaveButtonClick(EventArgs e)
        {
            if (!AreYouReady())
            {
                MessageBox.Show("請修正錯誤資料再儲存。");
                return;
            }

            this.Loading = true;

            string semester = string.Empty;

            semester = (this.cboSemester.SelectedValue == null ? string.Empty : this.cboSemester.SelectedValue.ToString());

            this._BGWSaveData.RunWorkerAsync(semester);
        }

        private void SaveUDT(string semester)
        {
            K12.Data.StudentRecord studentRecord = K12.Data.Student.SelectByID(PrimaryKey);
            //  待更新資料
            Dictionary<string, UDT.Paper> updateRecords = new Dictionary<string, UDT.Paper>();
            //  待新增資料
            List<UDT.Paper> insertRecords = new List<UDT.Paper>();

            UDT.Paper record;

            if (_dicUTDs.ContainsKey(PrimaryKey))
            {
                record = _dicUTDs[PrimaryKey];
                _dicUTDs[PrimaryKey] = record.Clone();
            }
            else
                record = new UDT.Paper();

            record.StudentID = int.Parse(this.PrimaryKey);
            record.AdvisorList = ParseAdvisor();
            record.Description = this.txtDescription.Text.Trim();
            record.IsPublic = this.IsPublic.Value;
            record.PaperName = this.txtPaperName.Text.Trim();
            record.PublishedDate = this.txtPublishedDate.Text.Trim();
            record.SchoolYear = this.nudSchoolYear.Text.Trim();
            record.Semester = semester;
            
            if (record.RecordStatus == FISCA.UDT.RecordStatus.Update)
                updateRecords.Add(PrimaryKey, record);
            else if (record.RecordStatus == FISCA.UDT.RecordStatus.Insert)
                insertRecords.Add(record);

            List<string> insertedRecordUIDs = insertRecords.SaveAllWithLog("學生.資料項目.指導教授及論文", "", "", Log.LogTargetCategory.Student, "學生系統編號");   //.SaveAll();                   //  寫入「新增」資料 
            //if (insertedRecordUIDs != null && insertedRecordUIDs.Count > 0)
            //{
            //    //  寫入「新增」的 Log
            //    List<UDT.Paper> insertedRecords = Access.Select<UDT.Paper>(insertedRecordUIDs);
            //    Dictionary<string, UDT.Paper> dicInsertedRecords = insertedRecords.ToDictionary(x => x.UID);
            //    foreach (string iRecords in dicInsertedRecords.Keys)
            //    {
            //        UDT.Paper insertedClubFeeRecord = dicInsertedRecords[iRecords];

            //        StringBuilder sb = new StringBuilder();
            //        List<string> advisors = ReverseAdvisor(insertedClubFeeRecord.AdvisorList);
            //        sb.Append("學生「" + studentRecord.Name + "」，學號「" + studentRecord.StudentNumber + "」");
            //        sb.AppendLine("被新增一筆「指導教授及論文」記錄。");
            //        sb.AppendLine("詳細資料：");
            //        sb.Append("論文題目「" + insertedClubFeeRecord.PaperName + "」\n");
            //        sb.Append("書籍狀況「" + insertedClubFeeRecord.Description + "」\n");
            //        sb.Append("指導教授1「" + advisors[0] + "」\n");
            //        sb.Append("指導教授2「" + advisors[1] + "」\n");
            //        sb.Append("指導教授3「" + advisors[2] + "」\n");
            //        sb.Append("學年度「" + insertedClubFeeRecord.SchoolYear + "」\n");
            //        sb.Append("學期「" + GetSemesterName(insertedClubFeeRecord.Semester) + "」\n");
            //        sb.Append("是否公開紙本論文「" + (insertedClubFeeRecord.IsPublic ? "是" : "否") + "」\n");
            //        sb.Append("延後公開期限「" + insertedClubFeeRecord.PublishedDate + "」\n");
            //        ApplicationLog.Log("指導教授及論文.學生", "新增", "student", insertedClubFeeRecord.StudentID.ToString(), sb.ToString());
            //    }
            //}

            List<string> updatedRecords = updateRecords.Values.SaveAllWithLog("學生.資料項目.指導教授及論文", "", "", Log.LogTargetCategory.Student, "學生系統編號");   //.SaveAll();   //  寫入「更新」資料             
            
            //if (updatedRecords != null && updatedRecords.Count > 0)
            //{
            //    //  寫入「修改」的 Log
            //    List<UDT.Paper> updatedRecordss = Access.Select<UDT.Paper>(updatedRecords);
            //    foreach (UDT.Paper uRecords in updatedRecordss)
            //    {
            //        UDT.Paper newRecord = uRecords;
            //        UDT.Paper oldRecord = _dicUTDs[uRecords.StudentID.ToString()];

            //        StringBuilder sb = new StringBuilder();
            //        List<string> advisors_OLD = ReverseAdvisor(oldRecord.AdvisorList);
            //        List<string> advisors_NEW = ReverseAdvisor(newRecord.AdvisorList);

            //        sb.Append("學生「" + studentRecord.Name + "」，學號「" + studentRecord.StudentNumber + "」");
            //        sb.AppendLine("被修改一筆「指導教授及論文」記錄。");
            //        sb.AppendLine("詳細資料：");
            //        if (!oldRecord.PaperName.Equals(newRecord.PaperName))
            //            sb.Append("論文題目由「" + oldRecord.PaperName + "」改為「" + newRecord.PaperName + "」\n");
            //        if (!oldRecord.Description.Equals(newRecord.Description))
            //            sb.Append("書籍狀況由「" + oldRecord.Description + "」改為「" + newRecord.Description + "」\n");
            //        if (!advisors_OLD[0].Equals(advisors_NEW[0]))
            //            sb.Append("指導教授1由「" + advisors_OLD[0] + "」改為「" + advisors_NEW[0] + "」\n");
            //        if (!advisors_OLD[1].Equals(advisors_NEW[1]))
            //            sb.Append("指導教授2由「" + advisors_OLD[1] + "」改為「" + advisors_NEW[1] + "」\n");
            //        if (!advisors_OLD[2].Equals(advisors_NEW[2]))
            //            sb.Append("指導教授3由「" + advisors_OLD[2] + "」改為「" + advisors_NEW[2] + "」\n");
            //        if (!oldRecord.SchoolYear.Equals(newRecord.SchoolYear))
            //            sb.Append("學年度由「" + oldRecord.SchoolYear + "」改為「" + newRecord.SchoolYear + "」\n");
            //        if (!oldRecord.Semester.Equals(newRecord.Semester))
            //            sb.Append("學期由「" + GetSemesterName(oldRecord.Semester) + "」改為「" + GetSemesterName(newRecord.Semester) + "」\n");
            //        if (!oldRecord.IsPublic.Equals(newRecord.IsPublic))
            //            sb.Append("是否公開紙本論文由「" + oldRecord.IsPublic + "」改為「" + newRecord.IsPublic + "」\n");
            //        if (!oldRecord.PublishedDate.Equals(newRecord.PublishedDate))
            //            sb.Append("延後公開期限由「" + oldRecord.PublishedDate + "」改為「" + newRecord.PublishedDate + "」\n");

            //        ApplicationLog.Log("指導教授及論文.學生", "修改", "student", uRecords.StudentID.ToString(), sb.ToString());
            //    }
            //}
        }

        private string GetSemesterName(string code)
        {
            if (string.IsNullOrEmpty(code))
                return string.Empty;
            else
                return SemesterItem.GetSemesterByCode(code).Name;
        }

        //  檢查輸入畫面是否仍有錯誤訊息
        private bool AreYouReady()
        {
            foreach (Control ctl in Controls)
            {
                if (_Errors.GetError(ctl) != string.Empty)
                    return false;
            }
            return true;
        }

        private void SchoolYear_TextChanged(object sender, EventArgs e)
        {
            uint d;
            if (!string.IsNullOrEmpty(this.nudSchoolYear.Text) && !uint.TryParse(nudSchoolYear.Text, out d))
            {
                _Errors.SetError(this.nudSchoolYear, "僅允許正整數。");
                return;
            }
            else
                _Errors.SetError(this.nudSchoolYear, string.Empty);
        }
    }
}
