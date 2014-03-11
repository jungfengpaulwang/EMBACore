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
    [FeatureCode("ischool.EMBA.Student.Detail0000", "�򥻸��")]
    internal partial class Student_Brief : FISCA.Presentation.DetailContent
    {
        private bool _isInitialized = false;
        private ErrorTip _errors = new ErrorTip();
        private bool _isBGBusy = false;
        private BackgroundWorker _BGWorker;
        private StudentRecord _StudRec;
        //private string _defaultLoginID = string.Empty;
        //private string _defaultIDNumber = string.Empty;

        // �J�ǷӤ�
        private string _FreshmanPhotoStr = string.Empty;

        // ���~�Ӥ�
        private string _GraduatePhotoStr = string.Empty;

        private ChangeListen _DataListener { get; set; }

        /*   Log  */
        private Log.LogAgent logAgent = new Log.LogAgent();

        public Student_Brief()
        {
            InitializeComponent();

            Group = "�򥻸��";
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
                case "�@��":
                    t_Status = "1";
                    break;
                case "���":
                    t_Status = "4";
                    break;
                case "�h��":
                    t_Status = "64";
                    break;
                case "���~������":
                    t_Status = "16";
                    break;
                case "�R��":
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
            // �ˬd�����Ҹ�
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
                    msg += string.Format("�оǤ��Z�G{0}�A�m�W�G{1}�A�Ǹ��G{2}�A�w���Φ������Ҹ��C", className, studentName, studentNumber) + "\n";
                }
                if (!string.IsNullOrEmpty(msg))
                {
                    _errors.SetError(this.txtIDNumber, msg);
                    return;
                }
            }
            // �ˬd�ʧO
            List<string> checkGender = new List<string>();
            checkGender.Add("�k");
            checkGender.Add("");
            checkGender.Add("�k");

            if (!checkGender.Contains(cboGender.Text))
            {
                _errors.SetError(cboGender, "�ʧO���~�A�нT�{��ơC");
                return;
            }

            DateTime dt;
            if (!string.IsNullOrEmpty(txtBirthDate.Text))
            {
                if (!DateTime.TryParse(txtBirthDate.Text, out dt))
                {
                    _errors.SetError(txtBirthDate, "������~�A�нT�{��ơC");
                    return;
                }
            }
            else
                _StudRec.Birthday = null;

            List<string> checkID = new List<string>();
            List<string> checkSSN = new List<string>();

            if (!string.IsNullOrWhiteSpace(txtLoginID.Text) && IsLoginExists(PrimaryKey, txtLoginID.Text))
            {
                _errors.SetError(txtLoginID, "�ǥ͵n�J�b�����СA�нT�{��ơC");
                return;
            }

            Student.Update(_StudRec);
            _errors.Clear();

            this.AddLog(this._StudRec);
            this.logAgent.ActionType = Log.LogActionType.Update;
            this.logAgent.Save("�ǥͰ򥻸��", "�ק�G", "", Log.LogTargetCategory.Student, this._StudRec.ID);

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

        //�N�e���M��
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
            // �ѪR
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
            //���J��a�C��
            this.cboNationality.Items.Add("���إ���");
            this.cboNationality.Items.Add("���ؤH���@�X��");
            this.cboNationality.Items.Add("�s�[��");
            this.cboNationality.Items.Add("�q�l");
            this.cboNationality.Items.Add("�L��");
            this.cboNationality.Items.Add("�饻");
            this.cboNationality.Items.Add("����");
            this.cboNationality.Items.Add("���Ӧ��");
            this.cboNationality.Items.Add("��߻�");
            this.cboNationality.Items.Add("�s�[�Y");
            this.cboNationality.Items.Add("����");
            this.cboNationality.Items.Add("�V�n");
            this.cboNationality.Items.Add("�Z��");
            this.cboNationality.Items.Add("�D�j�Q��");
            this.cboNationality.Items.Add("�æ���");
            this.cboNationality.Items.Add("�J��");
            this.cboNationality.Items.Add("�n�D");
            this.cboNationality.Items.Add("�k��");
            this.cboNationality.Items.Add("�q�j�Q");
            this.cboNationality.Items.Add("���");
            this.cboNationality.Items.Add("�^��");
            this.cboNationality.Items.Add("�w��");
            this.cboNationality.Items.Add("�[���j");
            this.cboNationality.Items.Add("�����j���[");
            this.cboNationality.Items.Add("�ʦa����");
            this.cboNationality.Items.Add("����");
            this.cboNationality.Items.Add("���ڧ�");
            this.cboNationality.Items.Add("�ڦ�");
            this.cboNationality.Items.Add("���ۤ��");
            this.cboNationality.Items.Add("�کԦc");
            this.cboNationality.Items.Add("�Q�Ԧc");
            this.cboNationality.Items.Add("��L");

            cboGender.Items.AddRange(new string[] { "�k", "�k" });

            _isInitialized = true;
        }


        private void buttonItem1_Click(object sender, EventArgs e)
        {
            OpenFileDialog od = new OpenFileDialog();
            od.Filter = "�Ҧ��v��(*.jpg,*.jpeg,*.gif,*.png)|*.jpg;*.jpeg;*.gif;*.png;";
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
            od.Filter = "�Ҧ��v��(*.jpg,*.jpeg,*.gif,*.png)|*.jpg;*.jpeg;*.gif;*.png;";
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

        //�t�s�Ӥ�
        private void buttonItem2_Click(object sender, EventArgs e)
        {
            SavePicture(_FreshmanPhotoStr);
        }

        //�t�s�Ӥ�
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
                MsgBox.Show("�Х��]�w�Ǹ��C");
                return;
            }
            if (imageString == string.Empty)
                return;

            SaveFileDialog sd = new SaveFileDialog();
            sd.Filter = "PNG �v��|*.png;";
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
                _errors.SetError(txtBirthDate, "�п�J yyyy/mm/dd �ŦX����榡��r");
        }

        // �ˬd
        private void ValidateIDNumber()
        {
            _errors.SetError(txtIDNumber, string.Empty);

            if (string.IsNullOrEmpty(txtIDNumber.Text))
            {
                _errors.SetError(txtIDNumber, string.Empty);
                return;
            }

            if (IsIDNumberExists(PrimaryKey, txtIDNumber.Text))
                _errors.SetError(txtIDNumber, "�����Ҹ����СA�нT�{��ơC");
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
                _errors.SetError(txtLoginID, "�b�����СA�Э��s��ܡC");
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

        #region �M���Ӥ�
        //�M���s�ͷӤ�
        private void buttonItem5_Click(object sender, EventArgs e)
        {
            if (FISCA.Presentation.Controls.MsgBox.Show("�z�T�w�n�M�����ǥͪ��Ӥ��ܡH", "", MessageBoxButtons.YesNo) == DialogResult.No) return;

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

        //�M�����~�Ӥ�
        private void buttonItem6_Click(object sender, EventArgs e)
        {
            if (FISCA.Presentation.Controls.MsgBox.Show("�z�T�w�n�M�����ǥͪ��Ӥ��ܡH", "", MessageBoxButtons.YesNo) == DialogResult.No) return;

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
            this.logAgent.SetLogValue("�m�W", obj.Name);
            this.logAgent.SetLogValue("�����Ҹ�", obj.IDNumber);
            this.logAgent.SetLogValue("�ͤ�", (obj.Birthday == null ? "" : obj.Birthday.ToString()));
            this.logAgent.SetLogValue("�ʧO", obj.Gender);
            this.logAgent.SetLogValue("���y", obj.Nationality);
            this.logAgent.SetLogValue("�X�ͦa", obj.BirthPlace);
            this.logAgent.SetLogValue("�^��m�W", obj.EnglishName);
            this.logAgent.SetLogValue("�n�J�b��", obj.SALoginName);
        }

        private void Student_Brief_Load(object sender, EventArgs e)
        {
            this.cboAccountType.SelectedIndex = 1;
        }
    }
}
