using System;
using System.ComponentModel;
using FISCA.Presentation;
using K12.Data;
using FCode = FISCA.Permission.FeatureCodeAttribute;
using Campus.Windows;
using FISCA.Permission;
using FISCA.UDT;
using System.Collections.Generic;

namespace EMBACore.DetailItems
{
    [FCode("ischool.EMBA.Student.Detail0082", "�q�ܸ��")]
    public partial class Student_Phone : FISCA.Presentation.DetailContent
    {
        //private SkypeControl.SkypeProxy skypeProxy;
        private string _PermanentPhone = string.Empty;  //��a�q��
        private string _ContactPhone = string.Empty;    //�p���q��
        private string _SMS = string.Empty;     //��ʹq��1
        private string _OtherPhone1 = string.Empty;     //���q�q��
        private string _OtherPhone2 = string.Empty;     //��ʹq��2
        private string _OtherPhone3 = string.Empty;     //�䥦�q��

        private UDT.StudentRemark _remark;       //�Ƶ�

        private bool _IsBgBusy = false;
        private BackgroundWorker _bwWork;

        private ChangeListen DataListener { get; set; }

        // �q�ܸ�T
        private PhoneRecord _PhoneRecord;

        private Log.LogAgent logAgent = new Log.LogAgent();


        public Student_Phone()
            : base()
        {
            InitializeComponent();
            DataListener = new ChangeListen();
            DataListener.Add(new TextBoxSource(txtEverPhone));
            DataListener.Add(new TextBoxSource(txtContactPhone));
            DataListener.Add(new TextBoxSource(this.txtMobile1));
            DataListener.Add(new TextBoxSource(this.txtMobile2));
            DataListener.Add(new TextBoxSource(this.txtCompanyPhone));
            DataListener.Add(new TextBoxSource(this.txtOtherPhone));
            DataListener.Add(new TextBoxSource(this.txtContactRemark));
            DataListener.StatusChanged += new EventHandler<ChangeEventArgs>(DataListener_StatusChanged);


            Group = "�q�T���";

            //skypeProxy = new SkypeControl.SkypeProxy();
            //skypeProxy.CountryInfo = new SkypeControl.CountryInfo("886", "�x�W");
            //skypeProxy.OnSkypeStatusChange += new SkypeControl.SkypeStatusChangeHandler(skypeProxy_OnSkypeStatusChange);
            //skypeProxy_OnSkypeStatusChange(null, null);

            _bwWork = new BackgroundWorker();
            _bwWork.DoWork += new DoWorkEventHandler(_bwWork_DoWork);
            _bwWork.RunWorkerCompleted += new RunWorkerCompletedEventHandler(_bwWork_RunWorkerCompleted);

            Phone.AfterUpdate += new EventHandler<K12.Data.DataChangedEventArgs>(JHPhone_AfterUpdate);
            Disposed += new EventHandler(PhonePalmerwormItem_Disposed);
        }

        void PhonePalmerwormItem_Disposed(object sender, EventArgs e)
        {
            Phone.AfterUpdate -= new EventHandler<K12.Data.DataChangedEventArgs>(JHPhone_AfterUpdate);
        }

        void JHPhone_AfterUpdate(object sender, K12.Data.DataChangedEventArgs e)
        {
            if (InvokeRequired)
            {
                BeginInvoke(new Action<object, K12.Data.DataChangedEventArgs>(JHPhone_AfterUpdate), sender, e);
                Invoke(new Action<object, K12.Data.DataChangedEventArgs>(JHPhone_AfterUpdate), sender, e);
            }
            else
            {
                if (PrimaryKey != "")
                {
                    if (!_bwWork.IsBusy)
                        _bwWork.RunWorkerAsync();
                }
            }

        }

        void DataListener_StatusChanged(object sender, ChangeEventArgs e)
        {
            if (UserAcl.Current[GetType()].Editable)
                SaveButtonVisible = (e.Status == ValueStatus.Dirty);
            else
                SaveButtonVisible = false;

            CancelButtonVisible = (e.Status == ValueStatus.Dirty);
        }

        void _bwWork_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if (_IsBgBusy)
            {
                _IsBgBusy = false;
                _bwWork.RunWorkerAsync();
                return;
            }
            BindDataToForm();
        }

        void _bwWork_DoWork(object sender, DoWorkEventArgs e)
        {
            //load phone information
            _PhoneRecord = Phone.SelectByStudentID(PrimaryKey);

            //load student remark
            List<UDT.StudentRemark> remarks = (new AccessHelper()).Select<UDT.StudentRemark>("ref_student_id=" + this.PrimaryKey);
            if (remarks.Count > 0)
                _remark = remarks[0];
        }

        // �x�s���
        protected override void OnSaveButtonClick(EventArgs e)
        {
            //Update Phone Information
            _PermanentPhone = txtEverPhone.Text;
            _ContactPhone = txtContactPhone.Text;
            _SMS = txtMobile1.Text;            
            _OtherPhone1 = txtCompanyPhone.Text;
            _OtherPhone2 = txtMobile2.Text;
            _OtherPhone3 = txtOtherPhone.Text;

            _PhoneRecord.Permanent = _PermanentPhone ;
            _PhoneRecord.Contact = _ContactPhone ;
            _PhoneRecord.Cell = _SMS ;           
            _PhoneRecord.Phone1 = _OtherPhone1;
            _PhoneRecord.Phone2 = _OtherPhone2;
            _PhoneRecord.Phone3 = _OtherPhone3;

            Phone.Update(_PhoneRecord);

            this.addLog();

            //Update Remark Information
            if (this._remark == null) {
                this._remark = new UDT.StudentRemark();
                this._remark.StudentID = int.Parse(PrimaryKey);
            }
            this._remark.Remark = this.txtContactRemark.Text;
            List<ActiveRecord> recs = new List<ActiveRecord>();
            recs.Add(this._remark);
            (new AccessHelper()).SaveAll(recs);

            this.logAgent.Save("�q�T���.�ǥ�", "", "", Log.LogTargetCategory.Student, this.PrimaryKey);

            BindDataToForm();
        }

        // �������
        protected override void OnCancelButtonClick(EventArgs e)
        {
            txtEverPhone.Text = _PermanentPhone;
            txtContactPhone.Text = _ContactPhone;
            txtMobile1.Text = _SMS;
            txtCompanyPhone.Text = _OtherPhone1;
            txtMobile2.Text = _OtherPhone2;
            txtOtherPhone.Text = _OtherPhone3;
            txtContactRemark.Text = (this._remark == null ? "" : this._remark.Remark);

            SaveButtonVisible = false;
            CancelButtonVisible = false;
        }

        // ��󴫿�ܾǥ�
        protected override void OnPrimaryKeyChanged(EventArgs e)
        {
            this.Loading = true;
            if (_bwWork.IsBusy)
                _IsBgBusy = true;
            else
                _bwWork.RunWorkerAsync();

        }

        // ���J���
        private void BindDataToForm()
        {
            SaveButtonVisible = false;
            CancelButtonVisible = false;
            DataListener.SuspendListen();

            _PermanentPhone = _ContactPhone = _SMS = _OtherPhone1 = _OtherPhone2 = _OtherPhone3 = "";
            txtEverPhone.Text = "";
            txtContactPhone.Text = "";
            txtOtherPhone.Text = "";
            txtMobile2.Text = "";
            txtMobile1.Text = "";
            txtCompanyPhone.Text = "";

            if (!string.IsNullOrEmpty(_PhoneRecord.Permanent))
                txtEverPhone.Text = _PermanentPhone = _PhoneRecord.Permanent;
            if (!string.IsNullOrEmpty(_PhoneRecord.Contact))
                txtContactPhone.Text = _ContactPhone = _PhoneRecord.Contact;
            if (!string.IsNullOrEmpty(_PhoneRecord.Cell))
                txtMobile1.Text = _SMS = _PhoneRecord.Cell;
            if (!string.IsNullOrEmpty(_PhoneRecord.Phone1))
                txtCompanyPhone.Text =  _OtherPhone1 = _PhoneRecord.Phone1;
            if (!string.IsNullOrEmpty(_PhoneRecord.Phone2))
                txtMobile2.Text =  _OtherPhone2 = _PhoneRecord.Phone2;
            if (!string.IsNullOrEmpty(_PhoneRecord.Phone3))
                txtOtherPhone.Text = _OtherPhone3 = _PhoneRecord.Phone3;

            txtContactRemark.Text = (this._remark == null ? "" : this._remark.Remark);

            DataListener.Reset();
            DataListener.ResumeListen();
            this.Loading = false;
            this.logAgent.Clear();
            this.addLog();
            this.logAgent.ActionType = Log.LogActionType.Update;
        }

        public DetailContent GetContent()
        {
            return new Student_Phone();
        }

        private void Student_Phone_Load(object sender, EventArgs e)
        {

        }

        private void addLog()
        {
            this.logAgent.SetLogValue("��ʹq��1", this.txtMobile1.Text);
            this.logAgent.SetLogValue("��ʹq��2", this.txtMobile2.Text);
            this.logAgent.SetLogValue("���q�q��", this.txtCompanyPhone.Text);
            this.logAgent.SetLogValue("��a�q��", this.txtEverPhone.Text);
            this.logAgent.SetLogValue("�䥦�q��", this.txtOtherPhone.Text);
            this.logAgent.SetLogValue("�p���q��", this.txtContactPhone.Text);
            this.logAgent.SetLogValue("�Ƶ�", this.txtContactRemark.Text);

        }


    }
}
