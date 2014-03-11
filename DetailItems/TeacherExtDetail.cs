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
using K12.Data;
using EMBACore.UDT;
using FISCA.UDT;
using FISCA.LogAgent;

namespace EMBACore.DetailItems
{
    [FeatureCode("ischool.EMBA.Teacher.TeacherExtDetail", "教師基本資料(延伸)")]
    public partial class TeacherExtDetail : DetailContent
    {
        private Log.LogAgent logAgent;
        private ChangeListen DataListener { get; set; }
        private BackgroundWorker _BGWork;
        private bool _isBGWorkBusy;
        private AccessHelper Access;
        private ErrorProvider ErrorProvider1;

        //  記錄所有 UDT 資料，便於比對待刪除資料
        private Dictionary<int, TeacherExtVO> _dicUTDs;

        public TeacherExtDetail()
            : base()
        {
            InitializeComponent();
            ErrorProvider1 = new ErrorProvider();
            this.Group = "教師基本資料";
        }

        private void TeacherExtDetail_Load(object sender, EventArgs e)
        {
            this.logAgent = new Log.LogAgent();

            DataListener = new ChangeListen();
            DataListener.Add(new TextBoxSource(txtBirthday));
            DataListener.Add(new TextBoxSource(txtAddress));
            DataListener.Add(new TextBoxSource(txtMobil));
            DataListener.Add(new TextBoxSource(txtOtherPhone));
            DataListener.Add(new TextBoxSource(txtPhone));
            DataListener.Add(new TextBoxSource(txtResearch));
            DataListener.Add(new TextBoxSource(txtMajorWorkPlace));
            DataListener.Add(new TextBoxSource(txtWebSiteUrl));
            DataListener.Add(new TextBoxSource(txtMemo));
            DataListener.Add(new TextBoxSource(txtEmployeeNo));
            DataListener.Add(new TextBoxSource(txtNtuSystemNo));
            DataListener.Add(new TextBoxSource(txtEnglishName));
            DataListener.StatusChanged += new EventHandler<ChangeEventArgs>(DataListener_StatusChanged);

            _BGWork = new BackgroundWorker();
            _BGWork.DoWork += new DoWorkEventHandler(_BGWork_DoWork);
            _BGWork.RunWorkerCompleted += new RunWorkerCompletedEventHandler(_BGWork_RunWorkerCompleted);
            _isBGWorkBusy = false;

            _dicUTDs = new Dictionary<int, TeacherExtVO>();
            Access = new AccessHelper();
        }

        void _BGWork_DoWork(object sender, DoWorkEventArgs e)
        {
            // 讀取教師資料
            e.Result = Access.Select<TeacherExtVO>(string.Format("ref_teacher_id={0}", PrimaryKey));
        }

        void _BGWork_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if (_isBGWorkBusy)
            {
                _isBGWorkBusy = false;
                _BGWork.RunWorkerAsync();
                return;
            }

            DataBindToForm(e.Result);
        }

        private void DataBindToForm(object result)
        {
            DataListener.SuspendListen();

            _dicUTDs.Clear();

            List<TeacherExtVO> teacherExtVORecords = result as List<TeacherExtVO>;

            if (teacherExtVORecords != null && teacherExtVORecords.Count > 0)
            {
                if (!_dicUTDs.ContainsKey(teacherExtVORecords[0].TeacherID))
                    _dicUTDs.Add(teacherExtVORecords[0].TeacherID, teacherExtVORecords[0]);
            }

            int teacherID = int.Parse(PrimaryKey);
            if (_dicUTDs.ContainsKey(teacherID))
            {
                this.txtWebSiteUrl.Text = _dicUTDs[teacherID].WebSiteUrl;
                this.txtBirthday.Text = (_dicUTDs[teacherID].Birthday);
                this.txtAddress.Text = (_dicUTDs[teacherID].Address);
                this.txtMobil.Text = (_dicUTDs[teacherID].Mobil);
                this.txtOtherPhone.Text = (_dicUTDs[teacherID].OtherPhone);
                this.txtPhone.Text = (_dicUTDs[teacherID].Phone);
                this.txtMajorWorkPlace.Text = (_dicUTDs[teacherID].MajorWorkPlace);
                this.txtMemo.Text = (_dicUTDs[teacherID].Memo);
                this.txtEmployeeNo.Text = (_dicUTDs[teacherID].EmployeeNo);
                this.txtNtuSystemNo.Text = (_dicUTDs[teacherID].NtuSystemNo);
                this.txtEnglishName.Text = (_dicUTDs[teacherID].EnglishName);
                this.txtResearch.Text = (_dicUTDs[teacherID].Research);
            }

            this.Loading = false;
            SaveButtonVisible = false;
            CancelButtonVisible = false;
            DataListener.Reset();
            DataListener.ResumeListen();
        }

        void DataListener_StatusChanged(object sender, ChangeEventArgs e)
        {
            if (UserAcl.Current[GetType()].Editable)
                SaveButtonVisible = (e.Status == ValueStatus.Dirty);
            else
                SaveButtonVisible = false;

            CancelButtonVisible = (e.Status == ValueStatus.Dirty);
        }

        protected override void OnPrimaryKeyChanged(EventArgs e)
        {
            this.Loading = true;
            if (_BGWork.IsBusy)
                _isBGWorkBusy = true;
            else
                _BGWork.RunWorkerAsync();
        }

        protected override void OnSaveButtonClick(EventArgs e)
        {
            //  1、先 Clone 原始資料，便於記錄 Log
            TeacherExtVO originalTeacherExtVO;

            int teacherID = int.Parse(PrimaryKey);
            if (_dicUTDs.ContainsKey(teacherID))
            {
                originalTeacherExtVO = _dicUTDs[teacherID];
                _dicUTDs[teacherID] = originalTeacherExtVO.Clone();
            }
            else
                originalTeacherExtVO = new TeacherExtVO();

            //  2、回填修改資料
            originalTeacherExtVO.WebSiteUrl = this.txtWebSiteUrl.Text;

            ErrorProvider1.Clear();
            DateTime birthday = DateTime.Now;

            if (!string.IsNullOrWhiteSpace(this.txtBirthday.Text))
            {
                if (!DateTime.TryParse(this.txtBirthday.Text.Trim(), out birthday))
                {
                    ErrorProvider1.SetError(this.txtBirthday, "格式錯誤，範例：1970/8/24");
                    return;
                }
                else
                    ErrorProvider1.SetError(this.txtBirthday, "");
            }

            originalTeacherExtVO.Birthday = string.IsNullOrWhiteSpace(this.txtBirthday.Text) ? string.Empty : birthday.ToShortDateString();
            originalTeacherExtVO.Address = this.txtAddress.Text;
            originalTeacherExtVO.Mobil = this.txtMobil.Text;
            originalTeacherExtVO.OtherPhone = this.txtOtherPhone.Text;
            originalTeacherExtVO.Phone = this.txtPhone.Text;
            originalTeacherExtVO.MajorWorkPlace = this.txtMajorWorkPlace.Text;
            originalTeacherExtVO.Memo = this.txtMemo.Text;
            originalTeacherExtVO.EmployeeNo = this.txtEmployeeNo.Text;
            originalTeacherExtVO.NtuSystemNo = this.txtNtuSystemNo.Text;
            originalTeacherExtVO.EnglishName = this.txtEnglishName.Text;
            originalTeacherExtVO.Research = this.txtResearch.Text;
            originalTeacherExtVO.TeacherID = int.Parse(PrimaryKey);

            //  3、存檔
            List<TeacherExtVO> updateRecords = new List<TeacherExtVO>();
            updateRecords.Add(originalTeacherExtVO);
            List<string> updatedRecordIDs = updateRecords.SaveAll();

            //  4、記錄 Log
            List<TeacherExtVO> updatedRecords = Access.Select<TeacherExtVO>(string.Format("ref_teacher_id={0}", PrimaryKey));
            if (updatedRecords != null && updatedRecords.Count > 0)
            {
                if (_dicUTDs.ContainsKey(teacherID))
                    WriteUpdateLog(_dicUTDs[teacherID], originalTeacherExtVO);
                else
                    WriteAddLog(originalTeacherExtVO);
            }

            DataBindToForm(updatedRecords);
            SaveButtonVisible = false;
            CancelButtonVisible = false;
        }

        protected override void OnCancelButtonClick(EventArgs e)
        {
            SaveButtonVisible = false;
            CancelButtonVisible = false;
            _BGWork.RunWorkerAsync();
        }

        /// <summary>
        /// 重設資料修改狀態。
        /// </summary>
        protected void ResetDirtyStatus()
        {
            DataListener.Reset();
            DataListener.ResumeListen();
        }

        private void linkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            if (!string.IsNullOrEmpty(this.txtWebSiteUrl.Text))
                System.Diagnostics.Process.Start(this.txtWebSiteUrl.Text);
        }

        void WriteAddLog(TeacherExtVO nRecord)
        {
            StringBuilder sb = new StringBuilder();
            TeacherRecord teacher = Teacher.SelectByID(PrimaryKey);
            sb.Append("教師「" + teacher.Name + "」，暱稱「" + teacher.Nickname + "」");
            sb.AppendLine("被新增一筆「教師基本資料」。");
            sb.AppendLine("詳細資料：");
            sb.Append("個人網址「" + nRecord.WebSiteUrl + "」\n");
            sb.Append("生日「" + nRecord.Birthday + "」\n");
            sb.Append("戶籍地籍「" + nRecord.Address + "」\n");
            sb.Append("手機「" + nRecord.Mobil + "」\n");
            sb.Append("研究室電話「" + nRecord.OtherPhone + "」\n");
            sb.Append("電話「" + nRecord.Phone + "」\n");
            sb.Append("所屬單位「" + nRecord.MajorWorkPlace + "」\n");
            sb.Append("備註「" + nRecord.Memo + "」\n");
            sb.Append("教師編號「" + nRecord.NtuSystemNo + "」\n");
            sb.Append("人事編號「" + nRecord.EmployeeNo + "」\n");
            sb.Append("英文姓名「" + nRecord.EnglishName + "」\n");
            sb.Append("研究室「" + nRecord.Research + "」\n");

            ApplicationLog.Log("基本資料.新增", "新增", "teacher", PrimaryKey, sb.ToString());
        }

        void WriteUpdateLog(TeacherExtVO oRecord, TeacherExtVO nRecord)
        {
            StringBuilder sb = new StringBuilder();
            TeacherRecord teacher = Teacher.SelectByID(PrimaryKey);
            sb.Append("教師「" + teacher.Name + "」，暱稱「" + teacher.Nickname + "」");
            sb.AppendLine("被修改一筆「教師基本資料」。");
            sb.AppendLine("詳細資料：");
            if (!oRecord.WebSiteUrl.Equals(nRecord.WebSiteUrl))
                sb.Append("個人網址由「" + oRecord.WebSiteUrl + "」改為「" + nRecord.WebSiteUrl + "」\n");
            if (!oRecord.Birthday.Equals(nRecord.Birthday))
                sb.Append("生日由「" + oRecord.Birthday + "」改為「" + nRecord.Birthday + "」\n");
            if (!oRecord.Address.Equals(nRecord.Address))
                sb.Append("戶籍地籍由「" + oRecord.Address + "」改為「" + nRecord.Address + "」\n");
            if (!oRecord.Mobil.Equals(nRecord.Mobil))
                sb.Append("手機由「" + oRecord.Mobil + "」改為「" + nRecord.Mobil + "」\n");
            if (!oRecord.OtherPhone.Equals(nRecord.OtherPhone))
                sb.Append("研究室電話由「" + oRecord.OtherPhone + "」改為「" + nRecord.OtherPhone + "」\n");
            if (!oRecord.Phone.Equals(nRecord.Phone))
                sb.Append("電話由「" + oRecord.Phone + "」改為「" + nRecord.Phone + "」\n");
            if (!oRecord.MajorWorkPlace.Equals(nRecord.MajorWorkPlace))
                sb.Append("所屬單位由「" + oRecord.MajorWorkPlace + "」改為「" + nRecord.MajorWorkPlace + "」\n");
            if (!oRecord.Memo.Equals(nRecord.Memo))
                sb.Append("備註由「" + oRecord.Memo + "」改為「" + nRecord.Memo + "」\n");
            if (!oRecord.NtuSystemNo.Equals(nRecord.NtuSystemNo))
                sb.Append("教師編號由「" + oRecord.NtuSystemNo + "」改為「" + nRecord.NtuSystemNo + "」\n");
            if (!oRecord.EmployeeNo.Equals(nRecord.EmployeeNo))
                sb.Append("人事編號由「" + oRecord.EmployeeNo + "」改為「" + nRecord.EmployeeNo + "」\n");
            if (!oRecord.EnglishName.Equals(nRecord.EnglishName))
                sb.Append("英文姓名由「" + oRecord.EnglishName + "」改為「" + nRecord.EnglishName + "」\n");
            if (!oRecord.Research.Equals(nRecord.Research))
                sb.Append("研究室由「" + oRecord.Research + "」改為「" + nRecord.Research + "」\n");

            ApplicationLog.Log("基本資料.修改", "修改", "teacher", PrimaryKey, sb.ToString());
        }
    }
}