using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Windows.Forms;
using FISCA.Presentation;
using K12.Data;
using FCode = FISCA.Permission.FeatureCodeAttribute;
using Campus.Windows;
using FISCA.Permission;
using Campus;
using System.Text.RegularExpressions;

namespace EMBACore.DetailItems
{
    [FCode("ischool.EMBA.Teacher.Teacher_Brief", "�Юv�򥻸��")]
    internal partial class Teacher_Brief : FISCA.Presentation.DetailContent
    {
        ErrorProvider epName = new ErrorProvider();
        ErrorProvider epNick = new ErrorProvider();
        ErrorProvider epGender = new ErrorProvider();
        ErrorProvider epLoginName = new ErrorProvider();
        private bool _isBGWorkBusy = false;
        private BackgroundWorker _BGWork;
        private TeacherRecord _TeacherRec;
        private Dictionary<string, string> _AllTeacherNameDic;
        private Dictionary<string, string> _AllLogIDDic;
        //PermRecLogProcess prlp;
        private ChangeListen _DataListener;

        public Teacher_Brief()
        {
            InitializeComponent();
            Group = "�Юv�򥻸��";
            _BGWork = new BackgroundWorker();
            _BGWork.DoWork += new DoWorkEventHandler(_BGWork_DoWork);
            _BGWork.RunWorkerCompleted += new RunWorkerCompletedEventHandler(_BGWork_RunWorkerCompleted);
            _AllTeacherNameDic = new Dictionary<string, string>();
            _AllLogIDDic = new Dictionary<string, string>();
            //prlp = new PermRecLogProcess();
            _DataListener = new ChangeListen();
            _DataListener.Add(new TextBoxSource(txtName));
            _DataListener.Add(new TextBoxSource(txtIDNumber));
            _DataListener.Add(new TextBoxSource(txtNickname));
            _DataListener.Add(new TextBoxSource(txtPhone));
            _DataListener.Add(new TextBoxSource(txtEmail));
            _DataListener.Add(new TextBoxSource(txtCategory));
            _DataListener.Add(new TextBoxSource(txtSTLoginAccount));
            _DataListener.Add(new TextBoxSource(txtSTLoginPwd));
            _DataListener.Add(new ComboBoxSource(cboAccountType, ComboBoxSource.ListenAttribute.Text));
            _DataListener.Add(new ComboBoxSource(cboGender, ComboBoxSource.ListenAttribute.Text));
            _DataListener.StatusChanged += new EventHandler<ChangeEventArgs>(_DataListener_StatusChanged);
            cboGender.DropDownStyle = ComboBoxStyle.DropDownList;
            Teacher.AfterChange += new EventHandler<K12.Data.DataChangedEventArgs>(Teacher_AfterChange);
            Disposed += new EventHandler(BaseInfoItem_Disposed);
        }

        void BaseInfoItem_Disposed(object sender, EventArgs e)
        {
            Teacher.AfterChange -= new EventHandler<K12.Data.DataChangedEventArgs>(Teacher_AfterChange);
        }

        void Teacher_AfterChange(object sender, K12.Data.DataChangedEventArgs e)
        {
            if (InvokeRequired)
            {
                BeginInvoke(new Action<object, K12.Data.DataChangedEventArgs>(Teacher_AfterChange), sender, e);
            }
            else
            {
                if (PrimaryKey != "")
                {
                    if (!_BGWork.IsBusy)
                        _BGWork.RunWorkerAsync();
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

        void _BGWork_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if (_isBGWorkBusy)
            {
                _isBGWorkBusy = false;
                _BGWork.RunWorkerAsync();
                return;
            }

            DataBindToForm();
        }

        // ���J��ƨ�e��
        private void DataBindToForm()
        {
            _DataListener.SuspendListen();
            txtName.Text = _TeacherRec.Name;
            txtIDNumber.Text = _TeacherRec.IDNumber;
            cboGender.Text = _TeacherRec.Gender;
            txtNickname.Text = _TeacherRec.Nickname;
            txtPhone.Text = _TeacherRec.ContactPhone;
            txtEmail.Text = _TeacherRec.Email;
            txtCategory.Text = _TeacherRec.Category;
            txtSTLoginAccount.Text = _TeacherRec.TALoginName;
            txtSTLoginPwd.Text = _TeacherRec.TAPassword;
            cboAccountType.Text = _TeacherRec.AccountType;

            try
            {

                pic1.Image = PhotoUtil.ConvertFromBase64Encoding(_TeacherRec.Photo, pic1.Width, pic1.Height);
            }
            catch (Exception)
            {
                pic1.Image = pic1.InitialImage;
            }



            // Log
            //prlp.SetBeforeSaveText("�m�W", txtName.Text);
            //prlp.SetBeforeSaveText("�����Ҹ�", txtIDNumber.Text);
            //prlp.SetBeforeSaveText("�ʧO", cboGender.Text);
            //prlp.SetBeforeSaveText("�ʺ�", txtNickname.Text);
            //prlp.SetBeforeSaveText("�p���q��", txtPhone.Text);
            //prlp.SetBeforeSaveText("�q�l�H�c", txtEmail.Text);
            //prlp.SetBeforeSaveText("�Юv���O", txtCategory.Text);
            //prlp.SetBeforeSaveText("�n�J�b��", txtSTLoginAccount.Text);
            //prlp.SetBeforeSaveText("�n�J�K�X", txtSTLoginPwd.Text);
            //prlp.SetBeforeSaveText("�b������", cboAccountType.Text);
            this.Loading = false;
            SaveButtonVisible = false;
            CancelButtonVisible = false;
            _DataListener.Reset();
            _DataListener.ResumeListen();
        }

        void _BGWork_DoWork(object sender, DoWorkEventArgs e)
        {
            _AllTeacherNameDic.Clear();
            _AllLogIDDic.Clear();

            foreach (TeacherRecord TR in Teacher.SelectAll())
            {
                _AllTeacherNameDic.Add(TR.Name + TR.Nickname, TR.ID);

                if (!string.IsNullOrEmpty(TR.TALoginName))
                    _AllLogIDDic.Add(TR.TALoginName.Trim().ToUpper(), TR.ID);
            }

            // Ū���Юv���
            _TeacherRec = Teacher.SelectByID(PrimaryKey);
        }

        protected override void OnPrimaryKeyChanged(EventArgs e)
        {
            this.Loading = true;
            if (_BGWork.IsBusy)
                _isBGWorkBusy = true;
            else
                _BGWork.RunWorkerAsync();

            ClearErrorMessage();
        }

        private bool isValidEmail(string email)
        {
            string patternStrict = @"^(([^<>()[\]\\.,;:\s@\""]+"
                       + @"(\.[^<>()[\]\\.,;:\s@\""]+)*)|(\"".+\""))@"
                       + @"((\[[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}"
                       + @"\.[0-9]{1,3}\])|(([a-zA-Z\-0-9]+\.)+"
                       + @"[a-zA-Z]{2,}))$";
            Regex reStrict = new Regex(patternStrict);

            bool isStrictMatch = reStrict.IsMatch(email);
            return isStrictMatch;
        }

        protected override void OnSaveButtonClick(EventArgs e)
        {
            bool isValid = true;
            // ���Ҹ��
            if (string.IsNullOrEmpty(txtName.Text.Trim()))
            {
                epName.SetError(txtName, "�m�W���i�ťաC");
                isValid = false;
            }

            //  ���ҵn�J�b��
            if (!string.IsNullOrWhiteSpace(this.txtSTLoginAccount.Text))
            {
                if (!this.isValidEmail(this.txtSTLoginAccount.Text.Trim()))
                {
                    (new ErrorProvider()).SetError(this.txtSTLoginAccount, "�����T���榡�C");
                    isValid = false;
                }
            }

            //  ���ҹq�l�H�c
            if (!string.IsNullOrWhiteSpace(this.txtEmail.Text))
            {
                if (!this.isValidEmail(this.txtEmail.Text.Trim()))
                {
                    (new ErrorProvider()).SetError(this.txtEmail, "�����T���榡�C");
                    isValid = false;
                }
            }

            // �ˬd�b���O�_����
            if (_AllLogIDDic.ContainsKey(txtSTLoginAccount.Text.Trim().ToUpper()))
            {
                if (_AllLogIDDic[txtSTLoginAccount.Text.Trim().ToUpper()] != _TeacherRec.ID)
                {
                    epLoginName.SetError(txtSTLoginAccount, "�n�J�b�����СC");
                    isValid = false;
                }
            }

            // �ˬd�m�W+�ʺ٬O�_����
            string checkName = txtName.Text + txtNickname.Text;

            if (_AllTeacherNameDic.ContainsKey(checkName))
            {
                if (_AllTeacherNameDic[checkName] != _TeacherRec.ID)
                {
                    epName.SetError(txtName, "�m�W+�ʺ٭��СA���ˬd�C");
                    epLoginName.SetError(txtNickname, "�m�W+�ʺ٭��СA���ˬd�C");
                    isValid = false;
                }
            }
            if (!isValid)
                return;
            // �^��� DAL
            _TeacherRec.AccountType = cboAccountType.Text;
            _TeacherRec.Category = txtCategory.Text;
            _TeacherRec.ContactPhone = txtPhone.Text;
            _TeacherRec.Email = txtEmail.Text;
            _TeacherRec.Gender = cboGender.Text;
            _TeacherRec.IDNumber = txtIDNumber.Text.Trim().ToUpper();
            _TeacherRec.TALoginName = txtSTLoginAccount.Text.Trim().ToUpper();
            _TeacherRec.Name = txtName.Text;
            _TeacherRec.Nickname = txtNickname.Text;
            _TeacherRec.TAPassword = txtSTLoginPwd.Text;

            // �s��
            Teacher.Update(_TeacherRec);

            // Save Log
            //prlp.SetAfterSaveText("�m�W", txtName.Text);
            //prlp.SetAfterSaveText("�����Ҹ�", txtIDNumber.Text);
            //prlp.SetAfterSaveText("�ʧO", cboGender.Text);
            //prlp.SetAfterSaveText("�ʺ�", txtNickname.Text);
            //prlp.SetAfterSaveText("�p���q��", txtPhone.Text);
            //prlp.SetAfterSaveText("�q�l�H�c", txtEmail.Text);
            //prlp.SetAfterSaveText("�Юv���O", txtCategory.Text);
            //prlp.SetAfterSaveText("�n�J�b��", txtSTLoginAccount.Text);
            //prlp.SetAfterSaveText("�n�J�K�X", txtSTLoginPwd.Text);
            //prlp.SetAfterSaveText("�b������", cboAccountType.Text);


            //prlp.SetDescTitle("�Юv�m�W�G" + txtName.Text + ",");
            //prlp.SetActionBy("���y", "�Юv�򥻸��");
            //prlp.SetAction("�ק�Юv�򥻸��");
            //prlp.SaveLog("", "", "teacher", PrimaryKey);
            DataBindToForm();
            SaveButtonVisible = false;
            CancelButtonVisible = false;
            //Teacher.Instance.SyncDataBackground(PrimaryKey);
            //Class.Instance.SyncAllBackground();
            ClearErrorMessage();
        }

        private void ClearErrorMessage()
        {
            epGender.Clear();
            epLoginName.Clear();
            epName.Clear();
            epNick.Clear();
            (new ErrorProvider()).SetError(this.txtSTLoginAccount, "");
            (new ErrorProvider()).SetError(this.txtEmail, "");
        }

        protected override void OnCancelButtonClick(EventArgs e)
        {
            SaveButtonVisible = false;
            CancelButtonVisible = false;
            DataBindToForm();
            ClearErrorMessage();
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

                    _TeacherRec.Photo = ToBase64String(PhotoUtil.Resize(new Bitmap(orgBmp)));
                    Teacher.Update(_TeacherRec);
                    //prlp.SaveLog("���y�t��-�Юv�򥻸��", "�ܧ�Юv�Ӥ�", "teacher", PrimaryKey, "�ܧ�Юv:" + _TeacherRec.Name + "���Ӥ�");
                    DataBindToForm();

                }
                catch (Exception ex)
                {
                    FISCA.Presentation.Controls.MsgBox.Show(ex.Message);
                }
            }
        }

        public DetailContent GetContent()
        {
            return new Teacher_Brief();
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

        private void buttonItem2_Click(object sender, EventArgs e)
        {
            SavePicture(_TeacherRec.Photo);
        }

        private void buttonItem5_Click(object sender, EventArgs e)
        {
            if (FISCA.Presentation.Controls.MsgBox.Show("�z�T�w�n�M�����ǥͪ��Ӥ��ܡH", "", MessageBoxButtons.YesNo) == DialogResult.No) return;

            try
            {
                _TeacherRec.Photo = string.Empty;
                pic1.Image = pic1.InitialImage;
                Teacher.Update(_TeacherRec);
                //prlp.SaveLog("���y�t��-�Юv�򥻸��", "�ܧ�Юv�Ӥ�", "teacher", PrimaryKey, "�ܧ�Юv:" + _TeacherRec.Name + "���Ӥ�");
                DataBindToForm();
            }
            catch (Exception ex)
            {
                FISCA.Presentation.Controls.MsgBox.Show(ex.Message);
            }
        }

        private void SavePicture(string imageString)
        {
            if (imageString == string.Empty)
                return;

            SaveFileDialog sd = new SaveFileDialog();
            sd.Filter = "PNG �v��|*.png;";
            sd.FileName = txtIDNumber.Text + ".png";

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
    }
}

