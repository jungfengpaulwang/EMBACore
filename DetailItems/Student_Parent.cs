using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows.Forms;
using FISCA.Presentation;
using K12.Data;
using FCode = FISCA.Permission.FeatureCodeAttribute;
using Campus.Windows;
using FISCA.Permission;

namespace EMBACore
{
    [FCode("ischool.EMBA.Student.Detail0083", "緊急聯絡人資料")]
    internal partial class Student_Parent : FISCA.Presentation.DetailContent
    {
        private bool _isInitialized = false;
        private ParentRecord _StudParentRec;
        private ChangeListen _DataListener_Guardian;
        private bool isBGBusy = false;
        private BackgroundWorker BGWorker;
        
        private ParentType _ParentType;

        private Log.LogAgent logAgent = new Log.LogAgent();

        private enum ParentType
        {
            Father,
            Mother,
            Guardian
        }

        public Student_Parent()
        {
            InitializeComponent();
            Group = "緊急聯絡人資料";
            _StudParentRec = new ParentRecord();
            _ParentType = ParentType.Guardian;

        
            _DataListener_Guardian = new ChangeListen();        
            _DataListener_Guardian.StatusChanged += new EventHandler<ChangeEventArgs>(_DataListener_Guardian_StatusChanged);
        
            
            // 加入緊急聯絡人 Listener
            _DataListener_Guardian.Add(new TextBoxSource(txtParentName));            
            _DataListener_Guardian.Add(new TextBoxSource(txtParentPhone));
            
            //_DataListener_Guardian.Add(new ComboBoxSource(cboRelationship, ComboBoxSource.ListenAttribute.Text));

            K12.Data.Parent.AfterUpdate += new EventHandler<K12.Data.DataChangedEventArgs>(JHParent_AfterUpdate);

            BGWorker = new BackgroundWorker();
            BGWorker.DoWork += new DoWorkEventHandler(BGWorker_DoWork);
            BGWorker.RunWorkerCompleted += new RunWorkerCompletedEventHandler(BGWorker_RunWorkerCompleted);
            
            Disposed += new EventHandler(ParentInfoPalmerwormItem_Disposed);
        }

        void ParentInfoPalmerwormItem_Disposed(object sender, EventArgs e)
        {
            K12.Data.Parent.AfterUpdate -= new EventHandler<K12.Data.DataChangedEventArgs>(JHParent_AfterUpdate);
        }

        void JHParent_AfterUpdate(object sender, K12.Data.DataChangedEventArgs e)
        {
            if (InvokeRequired)
            {
                BeginInvoke(new Action<object, K12.Data.DataChangedEventArgs>(JHParent_AfterUpdate), sender, e);
            }
            else
            {
                if (PrimaryKey != "")
                {
                    if (!BGWorker.IsBusy)
                        BGWorker.RunWorkerAsync();
                }
            }
        }



        void BGWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if (isBGBusy)
            {
                isBGBusy = false;
                BGWorker.RunWorkerAsync();
                return;
            }
            Initialize();
            BindDataToForm();
        }

        void BGWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            _StudParentRec = K12.Data.Parent.SelectByStudentID(PrimaryKey);
        }

        protected override void OnPrimaryKeyChanged(EventArgs e)
        {
            this.Loading = true;
            if (BGWorker.IsBusy)
                isBGBusy = true;
            else
                BGWorker.RunWorkerAsync();
        }

        private void BindDataToForm()
        {
            this.Loading = false;
            SaveButtonVisible = false;
            CancelButtonVisible = false;
            LoadDALDefaultData();


        }

        // 載入 DAL 預設取到值
        private void LoadDALDefaultData()
        {
            if (_ParentType == ParentType.Guardian)
            {
                LoadGuardian();
            }
        }

        private void DataListenerPause()
        {            
            _DataListener_Guardian.SuspendListen();
        }

        private void btnGuardian_Click(object sender, EventArgs e)
        {
            LoadGuardian();
            _ParentType = ParentType.Guardian;
        }

        protected override void OnCancelButtonClick(EventArgs e)
        {
            SaveButtonVisible = false;
            CancelButtonVisible = false;
            LoadDALDefaultData();
        }

        protected override void OnSaveButtonClick(EventArgs e)
        {
            // 回存資料
            if (_ParentType == ParentType.Guardian)
            { 
                _StudParentRec.Custodian.Name = txtParentName.Text;               
                _StudParentRec.Custodian.Phone = txtParentPhone.Text;
            }


            K12.Data.Parent.Update(_StudParentRec);
            this.addLog();
            this.logAgent.Save("緊急聯絡人資料.學生", "", "", Log.LogTargetCategory.Student, this.PrimaryKey);

            StudentRecord studRec = Student.SelectByID(PrimaryKey);
            //Student.Instance.SyncDataBackground(PrimaryKey);
            //Program.CustodianField.Reload();
            BindDataToForm();

        }
        
        void _DataListener_Guardian_StatusChanged(object sender, ChangeEventArgs e)
        {
            if (UserAcl.Current[GetType()].Editable)
                SaveButtonVisible = (e.Status == ValueStatus.Dirty);
            else
                SaveButtonVisible = false;

            CancelButtonVisible = (e.Status == ValueStatus.Dirty);
        }
        
        public DetailContent GetContent()
        {
            return new Student_Parent();
        }
        
        private void Initialize()
        {
            if (_isInitialized) return;

            _isInitialized = true;
        }

        // 載入緊急聯絡人資料
        private void LoadGuardian()
        {
            DataListenerPause();
            this.txtParentName.Text = this._StudParentRec.CustodianName;
            this.txtParentPhone.Text = this._StudParentRec.CustodianPhone;
        
            _DataListener_Guardian.Reset();
            _DataListener_Guardian.ResumeListen();

            this.logAgent.Clear();
            this.addLog();

        }

        private void addLog()
        {
            this.logAgent.ActionType = Log.LogActionType.Update;
            this.logAgent.SetLogValue("聯絡人姓名", this.txtParentName.Text );
            this.logAgent.SetLogValue("聯絡人電話", this.txtParentPhone.Text);
        }



        class Utility
        {
            /// <summary>
            /// 稱謂預設資料
            /// </summary>
            /// <returns></returns>
            public static List<string> GetRelationshipList()
            {
                List<string> retVal = new List<string>();
                string item = "父女,父子,母女,母子,伯叔姑姪甥,姊妹,祖孫";
                //            string item ="父,母,祖父,祖母,外公,外婆,伯,叔,舅,姑,姨,伯母,嬸,舅媽,姑丈,姨丈,兄,姊,弟,妹,堂兄,堂姊,堂弟,堂妹,表兄,表姊,表弟,表妹,養父,養母,院長";
                retVal.AddRange(item.Split(','));
                return retVal;
            }

            /// <summary>
            /// 職業預設資料
            /// </summary>
            /// <returns></returns>
            public static List<string> GetJobList()
            {
                List<string> retVal = new List<string>();
                string item = "工,商,軍,公,教,警,醫,農,漁,林,服務,金融,自由,家管,退休,無";
                retVal.AddRange(item.Split(','));
                return retVal;
            }

            /// <summary>
            /// 國籍預設資料
            /// </summary>
            /// <returns></returns>
            public static List<string> GetNationalityList()
            {
                List<string> retVal = new List<string>();
                string item = "中華民國,中華人民共合國,孟加拉,緬甸,印尼,日本,韓國,馬來西亞,菲律賓,新加坡,泰國,越南,汶萊,澳大利亞,紐西蘭,埃及,南非,法國,義大利,瑞典,英國,德國,加拿大,哥斯大黎加,瓜地馬拉,美國,阿根廷,巴西,哥倫比亞,巴拉圭,烏拉圭,其他";
                retVal.AddRange(item.Split(','));
                return retVal;
            }

            /// <summary>
            /// 最高學歷預設資料
            /// </summary>
            /// <returns></returns>
            public static List<string> GetEducationDegreeList()
            {
                List<string> retVal = new List<string>();
                string item = "無,國小,國中,高中,專科,大學,碩士,博士,其它";
                retVal.AddRange(item.Split(','));
                return retVal;
            }
        }
    }
}
