using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Windows.Forms;
using FISCA.Presentation;
using FISCA.Permission;
using Campus;
using K12.Data;
using Campus.Windows;
using FISCA.Data;
using System.Data;
using System.Diagnostics;

namespace EMBACore.DetailItems
{
    [FeatureCode("ischool.EMBA.Student.Detail0000", "基本資料")]
    internal partial class Student_Brief : FISCA.Presentation.DetailContent
    {
        private bool _isInitialized = false;
        private ErrorTip _errors = new ErrorTip();
        private bool _isBGBusy = false;
        private BackgroundWorker _BGWorker;
        private StudentRecord _StudRec;
        //private string _defaultLoginID = string.Empty;
        //private string _defaultIDNumber = string.Empty;

        // 入學照片
        private string _FreshmanPhotoStr = string.Empty;

        // 畢業照片
        private string _GraduatePhotoStr = string.Empty;

        private ChangeListen _DataListener { get; set; }

        /*   Log  */
        private Log.LogAgent logAgent = new Log.LogAgent();

        public Student_Brief()
        {
            InitializeComponent();

            Group = "基本資料";
            _DataListener = new ChangeListen();
            _DataListener.Add(new TextBoxSource(txtName));
            _DataListener.Add(new TextBoxSource(txtIDNumber));
            _DataListener.Add(new TextBoxSource(txtBirthDate));
            _DataListener.Add(new TextBoxSource(txtBirthPlace));
            _DataListener.Add(new TextBoxSource(txtEngName));
            _DataListener.Add(new TextBoxSource(txtLoginID));
            _DataListener.Add(new TextBoxSource(txtLoginPwd));
            _DataListener.Add(new ComboBoxSource(cboGender, ComboBoxSource.ListenAttribute.Text));
            _DataListener.Add(new ComboBoxSource(cboNationality, ComboBoxSource.ListenAttribute.Text));
            _DataListener.Add(new ComboBoxSource(cboAccountType, ComboBoxSource.ListenAttribute.Text));
            _DataListener.StatusChanged += new EventHandler<ChangeEventArgs>(_DataListener_StatusChanged);

            _BGWorker = new BackgroundWorker();
            _BGWorker.DoWork += new DoWorkEventHandler(_BGWorker_DoWork);
            _BGWorker.RunWorkerCompleted += new RunWorkerCompletedEventHandler(_BGWorker_RunWorkerCompleted);
            Initialize();
            Student.AfterChange += new EventHandler<K12.Data.DataChangedEventArgs>(JHStudent_AfterChange);
            Disposed += new EventHandler(StudentBrief_Disposed);
        }

        private void StudentBrief_Disposed(object sender, EventArgs e)
        {
            Student.AfterChange -= new EventHandler<K12.Data.DataChangedEventArgs>(JHStudent_AfterChange);
        }

        private void JHStudent_AfterChange(object sender, K12.Data.DataChangedEventArgs e)
        {
            if (InvokeRequired)
            {
                BeginInvoke(new Action<object, K12.Data.DataChangedEventArgs>(JHStudent_AfterChange), sender, e);
            }
            else
            {
                if (PrimaryKey != "")
                {
                    if (!_BGWorker.IsBusy)
                        _BGWorker.RunWorkerAsync();
                }
            }
        }

        void _DataListener_StatusChanged(object sender, ChangeEventArgs e)
        {
            if (UserAcl.Current[GetType()].Editable)
                SaveButtonVisible = (e.Status == ValueStatus.Dirty);
            else
                SaveButtonVisible = false;
            CancelButtonVisible = (e.Status == ValueStatus.Dirty);
        }

        void _BGWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if (_isBGBusy)
            {
                _isBGBusy = false;
                _BGWorker.RunWorkerAsync();
                return;
            }
            BindDataToForm();
        }

        private string t_Status(string status)
        {
            string t_Status = string.Empty;
            switch (status)
            {
                case "一般":
                    t_Status = "1";
                    break;
                case "休學":
                    t_Status = "4";
                    break;
                case "退學":
                    t_Status = "64";
                    break;
                case "畢業或離校":
                    t_Status = "16";
                    break;
                case "刪除":
                    t_Status = "256";
                    break;
                default:
                    t_Status = "";
                    break;
            }
            return t_Status;
        }

        protected override void OnSaveButtonClick(EventArgs e)
        {
            StudentRecord student = Student.SelectByID(PrimaryKey);
            SetFormDataToDALRec();
            // 檢查身分證號
            if (string.IsNullOrWhiteSpace(this.txtIDNumber.Text))
            {
                _errors.SetError(this.txtIDNumber, string.Empty);
                this.txtIDNumber.Text = string.Empty;
            }
            else
            {
                QueryHelper queryHelper = new QueryHelper();
                DataTable dataTable = queryHelper.Select(string.Format("select class_name, student.name as name, student_number from student left join class on class.id=student.ref_class_id where id_number ilike ('{0}') and student.status={1} and student.id not in ('{2}')", this.txtIDNumber.Text.Trim(), t_Status(student.StatusStr), PrimaryKey));

                string msg = string.Empty;
                foreach (DataRow row in dataTable.Rows)
                {
                    string className = row["class_name"] + "";
                    string studentName = row["name"] + "";
                    string studentNumber = row["student_number"] + "";
                    msg += string.Format("教學分班：{0}，姓名：{1}，學號：{2}，已佔用此身分證號。", className, studentName, studentNumber) + "\n";
                }
                if (!string.IsNullOrEmpty(msg))
                {
                    _errors.SetError(this.txtIDNumber, msg);
                    return;
                }
            }
            // 檢查性別
            List<string> checkGender = new List<string>();
            checkGender.Add("男");
            checkGender.Add("");
            checkGender.Add("女");

            if (!checkGender.Contains(cboGender.Text))
            {
                _errors.SetError(cboGender, "性別錯誤，請確認資料。");
                return;
            }

            DateTime dt;
            if (!string.IsNullOrEmpty(txtBirthDate.Text))
            {
                if (!DateTime.TryParse(txtBirthDate.Text, out dt))
                {
                    _errors.SetError(txtBirthDate, "日期錯誤，請確認資料。");
                    return;
                }
            }
            else
                _StudRec.Birthday = null;

            List<string> checkID = new List<string>();
            List<string> checkSSN = new List<string>();

            if (!string.IsNullOrWhiteSpace(txtLoginID.Text) && IsLoginExists(PrimaryKey, txtLoginID.Text))
            {
                _errors.SetError(txtLoginID, "學生登入帳號重覆，請確認資料。");
                return;
            }

            Student.Update(_StudRec);
            _errors.Clear();

            this.AddLog(this._StudRec);
            this.logAgent.ActionType = Log.LogActionType.Update;
            this.logAgent.Save("學生基本資料", "修改：", "", Log.LogTargetCategory.Student, this._StudRec.ID);

            //BindDataToForm();
        }

        private void SetFormDataToDALRec()
        {
            _StudRec.AccountType = cboAccountType.Text;

            DateTime dt;
            if (DateTime.TryParse(txtBirthDate.Text, out dt))
                _StudRec.Birthday = dt;

            _StudRec.BirthPlace = txtBirthPlace.Text;
            _StudRec.EnglishName = txtEngName.Text;
            _StudRec.Gender = cboGender.Text;
            _StudRec.IDNumber = txtIDNumber.Text;
            _StudRec.Name = txtName.Text;
            _StudRec.Nationality = cboNationality.Text;
            _StudRec.SALoginName = txtLoginID.Text;
            _StudRec.SAPassword = txtLoginPwd.Text; 
        }

        protected override void OnCancelButtonClick(EventArgs e)
        {
            _DataListener.SuspendListen();
            _errors.Clear();
            ClearFormValue();
            //LoadDALDataToForm();
            _BGWorker.RunWorkerAsync();
            _DataListener.Reset();
            _DataListener.ResumeListen();
            SaveButtonVisible = false;
            CancelButtonVisible = false;
        }

        void _BGWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            // Get Photo
            _FreshmanPhotoStr = _GraduatePhotoStr = string.Empty;
            _FreshmanPhotoStr = K12.Data.Photo.SelectFreshmanPhoto(PrimaryKey);
            _GraduatePhotoStr = K12.Data.Photo.SelectGraduatePhoto(PrimaryKey);

            // studentRec
            _StudRec = Student.SelectByID(PrimaryKey);
        }

        protected override void OnPrimaryKeyChanged(EventArgs e)
        {
            _errors.Clear();
            this.Loading = true;
            if (_BGWorker.IsBusy)
                _isBGBusy = true;
            else
                _BGWorker.RunWorkerAsync();
        }

        //將畫面清空
        private void ClearFormValue()
        {
            txtBirthDate.Text = txtBirthPlace.Text = txtEngName.Text = txtLoginID.Text = txtName.Text = txtIDNumber.Text = cboAccountType.Text = cboGender.Text = cboNationality.Text = string.Empty;
        }

        private void BindDataToForm()
        {
            _DataListener.SuspendListen();
            ClearFormValue();
            LoadDALDataToForm();

            this.logAgent.Clear();
            this.AddLog(this._StudRec);

            // get checkDefault
            //_defaultIDNumber = _StudRec.IDNumber;
            //_defaultLoginID = _StudRec.SALoginName;
            this.Loading = false;
            SaveButtonVisible = false;
            CancelButtonVisible = false;
            _DataListener.Reset();
            _DataListener.ResumeListen();
        }

        private void LoadDALDataToForm()
        {
            if (_StudRec.Birthday.HasValue)
                txtBirthDate.Text = _StudRec.Birthday.Value.ToShortDateString();
            txtBirthPlace.Text = _StudRec.BirthPlace;
            txtEngName.Text = _StudRec.EnglishName;
            txtLoginID.Text = _StudRec.SALoginName;
            txtLoginPwd.Text = _StudRec.SAPassword;
            txtName.Text = _StudRec.Name;
            txtIDNumber.Text = _StudRec.IDNumber;
            cboAccountType.Text = _StudRec.AccountType;
            cboGender.Text = _StudRec.Gender;
            cboNationality.Text = _StudRec.Nationality;
            // 解析
            try
            {

                pic1.Image = PhotoUtil.ConvertFromBase64Encoding(_FreshmanPhotoStr, pic1.Width, pic1.Height);
            }
            catch (Exception)
            {
                pic1.Image = pic1.InitialImage;
            }

            try
            {
                pic2.Image = PhotoUtil.ConvertFromBase64Encoding(_GraduatePhotoStr, pic2.Width, pic2.Height);
            }
            catch (Exception)
            {
                pic2.Image = pic2.InitialImage;
            }

        }

        public DetailContent GetContent()
        {
            return new Student_Brief();
        }

        private void Initialize()
        {
            if (_isInitialized) return;
            //載入國家列表
            this.cboNationality.Items.Add("中華民國");
            this.cboNationality.Items.Add("中華人民共合國");
            this.cboNationality.Items.Add("孟加拉");
            this.cboNationality.Items.Add("緬甸");
            this.cboNationality.Items.Add("印尼");
            this.cboNationality.Items.Add("日本");
            this.cboNationality.Items.Add("韓國");
            this.cboNationality.Items.Add("馬來西亞");
            this.cboNationality.Items.Add("菲律賓");
            this.cboNationality.Items.Add("新加坡");
            this.cboNationality.Items.Add("泰國");
            this.cboNationality.Items.Add("越南");
            this.cboNationality.Items.Add("汶萊");
            this.cboNationality.Items.Add("澳大利亞");
            this.cboNationality.Items.Add("紐西蘭");
            this.cboNationality.Items.Add("埃及");
            this.cboNationality.Items.Add("南非");
            this.cboNationality.Items.Add("法國");
            this.cboNationality.Items.Add("義大利");
            this.cboNationality.Items.Add("瑞典");
            this.cboNationality.Items.Add("英國");
            this.cboNationality.Items.Add("德國");
            this.cboNationality.Items.Add("加拿大");
            this.cboNationality.Items.Add("哥斯大黎加");
            this.cboNationality.Items.Add("瓜地馬拉");
            this.cboNationality.Items.Add("美國");
            this.cboNationality.Items.Add("阿根廷");
            this.cboNationality.Items.Add("巴西");
            this.cboNationality.Items.Add("哥倫比亞");
            this.cboNationality.Items.Add("巴拉圭");
            this.cboNationality.Items.Add("烏拉圭");
            this.cboNationality.Items.Add("其他");

            cboGender.Items.AddRange(new string[] { "男", "女" });

            _isInitialized = true;
        }


        private void buttonItem1_Click(object sender, EventArgs e)
        {
            OpenFileDialog od = new OpenFileDialog();
            od.Filter = "所有影像(*.jpg,*.jpeg,*.gif,*.png)|*.jpg;*.jpeg;*.gif;*.png;";
            if (od.ShowDialog() == DialogResult.OK)
            {
                FileStream fs = null;
                try
                {
                    fs = new FileStream(od.FileName, FileMode.Open);
                    Bitmap orgBmp = new Bitmap(fs);
                    fs.Close();

                    Bitmap newBmp = new Bitmap(orgBmp, pic1.Size);
                    pic1.Image = newBmp;

                    _FreshmanPhotoStr = ToBase64String(PhotoUtil.Resize(new Bitmap(orgBmp)));
                    K12.Data.Photo.UpdateFreshmanPhoto(_FreshmanPhotoStr, PrimaryKey);
                }
                catch (Exception ex)
                {
                    FISCA.Presentation.Controls.MsgBox.Show(ex.Message);
                }
            }
        }

        private void buttonItem3_Click(object sender, EventArgs e)
        {
            OpenFileDialog od = new OpenFileDialog();
            od.Filter = "所有影像(*.jpg,*.jpeg,*.gif,*.png)|*.jpg;*.jpeg;*.gif;*.png;";
            if (od.ShowDialog() == DialogResult.OK)
            {
                FileStream fs = null;
                try
                {
                    fs = new FileStream(od.FileName, FileMode.Open);
                    Bitmap orgBmp = new Bitmap(fs);
                    fs.Close();

                    Bitmap newBmp = new Bitmap(orgBmp, pic2.Size);
                    pic2.Image = newBmp;

                    _GraduatePhotoStr = ToBase64String(PhotoUtil.Resize(new Bitmap(orgBmp)));

                    K12.Data.Photo.UpdateGraduatePhoto(_GraduatePhotoStr, PrimaryKey);
                }
                catch (Exception ex)
                {
                    FISCA.Presentation.Controls.MsgBox.Show(ex.Message);
                }
            }
        }

        private static string ToBase64String(Bitmap newBmp)
        {
            MemoryStream ms = new MemoryStream();
            newBmp.Save(ms, ImageFormat.Jpeg);
            ms.Seek(0, SeekOrigin.Begin);
            byte[] bytes = new byte[ms.Length];
            ms.Read(bytes, 0, (int)ms.Length);
            ms.Close();

            return Convert.ToBase64String(bytes);
        }

        //另存照片
        private void buttonItem2_Click(object sender, EventArgs e)
        {
            SavePicture(_FreshmanPhotoStr);
        }

        //另存照片
        private void buttonItem4_Click(object sender, EventArgs e)
        {
            SavePicture(_GraduatePhotoStr);
        }

        private void SavePicture(string imageString)
        {
            if (_StudRec == null)
                return;

            if (string.IsNullOrEmpty(_StudRec.StudentNumber))
            {
                MsgBox.Show("請先設定學號。");
                return;
            }
            if (imageString == string.Empty)
                return;

            SaveFileDialog sd = new SaveFileDialog();
            sd.Filter = "PNG 影像|*.png;";
            sd.FileName = _StudRec.StudentNumber + ".png";

            if (sd.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    FileStream fs = new FileStream(sd.FileName, FileMode.Create);
                    byte[] imageData = Convert.FromBase64String(imageString);
                    fs.Write(imageData, 0, imageData.Length);
                    fs.Close();
                }
                catch (Exception ex)
                {
                    FISCA.Presentation.Controls.MsgBox.Show(ex.Message);
                }
            }
        }

        private void txtBirthDate_Validated(object sender, EventArgs e)
        {
            _errors.SetError(txtBirthDate, string.Empty);

            if (!txtBirthDate.IsValid)
                _errors.SetError(txtBirthDate, "請輸入 yyyy/mm/dd 符合日期格式文字");
        }

        // 檢查
        private void ValidateIDNumber()
        {
            _errors.SetError(txtIDNumber, string.Empty);

            if (string.IsNullOrEmpty(txtIDNumber.Text))
            {
                _errors.SetError(txtIDNumber, string.Empty);
                return;
            }

            if (IsIDNumberExists(PrimaryKey, txtIDNumber.Text))
                _errors.SetError(txtIDNumber, "身分證號重覆，請確認資料。");
        }

        private void ValidateLoginID()
        {
            _errors.SetError(txtLoginID, string.Empty);

            if (string.IsNullOrEmpty(txtLoginID.Text))
            {
                _errors.SetError(txtLoginID, string.Empty);
                return;
            }

            if (IsLoginExists(PrimaryKey, txtLoginID.Text))
                _errors.SetError(txtLoginID, "帳號重覆，請重新選擇。");
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="selfPrimaryKey"></param>
        /// <param name="idNumber"></param>
        /// <returns></returns>
        private bool IsIDNumberExists(string selfPrimaryKey, string idNumber)
        {
            string cmd = string.Format("select id from student where id_number='{0}' and id not in('{1}') ", idNumber, selfPrimaryKey);
            QueryHelper query = new QueryHelper();
            DataTable dt = query.Select(cmd);
            return dt.Rows.Count > 0;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="selfPrimaryKey"></param>
        /// <param name="loginId"></param>
        /// <returns></returns>
        private bool IsLoginExists(string selfPrimaryKey, string loginId)
        {
            string cmd = string.Format("select id from student where sa_login_name='{0}' and id not in('{1}') ", loginId, selfPrimaryKey);
            QueryHelper query = new QueryHelper();
            DataTable dt = query.Select(cmd);
            return dt.Rows.Count > 0;
        }

        #region 清除照片
        //清除新生照片
        private void buttonItem5_Click(object sender, EventArgs e)
        {
            if (FISCA.Presentation.Controls.MsgBox.Show("您確定要清除此學生的照片嗎？", "", MessageBoxButtons.YesNo) == DialogResult.No) return;

            try
            {
                _FreshmanPhotoStr = string.Empty;
                pic1.Image = pic1.InitialImage;
                K12.Data.Photo.UpdateFreshmanPhoto("", PrimaryKey);
            }
            catch (Exception ex)
            {
                FISCA.Presentation.Controls.MsgBox.Show(ex.Message);
            }
        }

        //清除畢業照片
        private void buttonItem6_Click(object sender, EventArgs e)
        {
            if (FISCA.Presentation.Controls.MsgBox.Show("您確定要清除此學生的照片嗎？", "", MessageBoxButtons.YesNo) == DialogResult.No) return;

            try
            {
                _GraduatePhotoStr = string.Empty;
                pic2.Image = pic2.InitialImage;
                K12.Data.Photo.UpdateGraduatePhoto("", PrimaryKey);
            }
            catch (Exception ex)
            {
                FISCA.Presentation.Controls.MsgBox.Show(ex.Message);
            }
        }
        #endregion

        private void txtBirthDate_TextChanged(object sender, EventArgs e)
        {
            _errors.SetError(txtBirthDate, string.Empty);
        }

        private void cboGender_TextChanged(object sender, EventArgs e)
        {
            _errors.SetError(cboGender, string.Empty);
        }

        void AddLog(StudentRecord obj)
        {
            this.logAgent.SetLogValue("姓名", obj.Name);
            this.logAgent.SetLogValue("身分證號", obj.IDNumber);
            this.logAgent.SetLogValue("生日", (obj.Birthday == null ? "" : obj.Birthday.ToString()));
            this.logAgent.SetLogValue("性別", obj.Gender);
            this.logAgent.SetLogValue("國籍", obj.Nationality);
            this.logAgent.SetLogValue("出生地", obj.BirthPlace);
            this.logAgent.SetLogValue("英文姓名", obj.EnglishName);
            this.logAgent.SetLogValue("登入帳號", obj.SALoginName);
        }

        private void Student_Brief_Load(object sender, EventArgs e)
        {
            this.cboAccountType.SelectedIndex = 1;
        }
    }
}
