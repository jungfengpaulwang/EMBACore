using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows.Forms;
using System.Linq;
using System.Text;
using FISCA.LogAgent;
using System.Xml.Serialization;
using System.Xml;
using System.IO;
using Campus.Windows;
using FCode = FISCA.Permission.FeatureCodeAttribute;
using FISCA.UDT;
using FISCA.Data;
using EMBACore.UDT;
using EMBACore.DataItems;
using FISCA.Permission;

namespace EMBACore
{
    [FCode("ischool.EMBA.Student_PaymentHistory", "繳費紀錄")]
    public partial class Student_PaymentHistory : FISCA.Presentation.DetailContent
    {
        //  背景載入 UDT 資料物件
        private BackgroundWorker _BGWLoadData;
        private BackgroundWorker _BGWSaveData;

        //  監控 UI 資料變更
        private ChangeListen _Listener;

        //  正在下載的資料之主鍵，用於檢查是否下載他人資料，若 _RunningKey != PrimaryKey 就再下載乙次
        private string _RunningKey;

        //  _BGWLoadData_DoWork 之  e.Result，包含所有被下載的資料物件。
        private object _Result;

        private AccessHelper Access;
        private QueryHelper Helper;

        private Dictionary<string, PaymentHistory> _dicUTDs;

        public Student_PaymentHistory()
        {
            InitializeComponent();
            this.Group = "繳費紀錄";

            this.Load += new System.EventHandler(this.Student_PaymentHistory_Load);
        }

        private void Student_PaymentHistory_Load(object sender, EventArgs e)
        {            
            _Listener = new ChangeListen();
            _Listener.Add(new DataGridViewSource(this.dgvData));
            _Listener.StatusChanged += new EventHandler<ChangeEventArgs>(Listener_StatusChanged);

            _BGWLoadData = new BackgroundWorker();
            _BGWLoadData.DoWork += new DoWorkEventHandler(_BGWLoadData_DoWork);
            _BGWLoadData.RunWorkerCompleted += new RunWorkerCompletedEventHandler(_BGWLoadData_RunWorkerCompleted);

            _BGWSaveData = new BackgroundWorker();
            _BGWSaveData.DoWork += new DoWorkEventHandler(_BGWSaveData_DoWork);
            _BGWSaveData.RunWorkerCompleted += new RunWorkerCompletedEventHandler(_BGWSaveData_RunWorkerCompleted);

            _RunningKey = "";

            dgvData.CurrentCellDirtyStateChanged += new EventHandler(dgvData_CurrentCellDirtyStateChanged);
            dgvData.CellEnter += new DataGridViewCellEventHandler(dgvData_CellEnter);
            dgvData.RowsRemoved += new DataGridViewRowsRemovedEventHandler(dgvData_RowsRemoved);
            this.dgvData.ColumnHeaderMouseClick += new System.Windows.Forms.DataGridViewCellMouseEventHandler(this.dgvData_ColumnHeaderMouseClick);
            this.dgvData.RowHeaderMouseClick += new System.Windows.Forms.DataGridViewCellMouseEventHandler(this.dgvData_RowHeaderMouseClick);
            this.dgvData.MouseClick += new System.Windows.Forms.MouseEventHandler(this.dgvData_MouseClick);

            this.dgvData.DataError += new DataGridViewDataErrorEventHandler(dgvData_DataError);

            Access = new AccessHelper();
            Helper = new QueryHelper();

            _dicUTDs = new Dictionary<string, PaymentHistory>();
            //  耀明建議不要自動更新資料項目的內容，若要自動更新則取消註解下列程式碼即可。
            //UDT.PaymentHistory.AfterUpdate += new EventHandler<UDT.ParameterEventArgs>(PaymentHistory_AfterUpdate);
        }

        private void PaymentHistory_AfterUpdate(object sender, UDT.ParameterEventArgs e)
        {
            if (InvokeRequired)
            {
                BeginInvoke(new Action<object, UDT.ParameterEventArgs>(PaymentHistory_AfterUpdate), sender, e);
            }
            else
            {
                if (PrimaryKey != "")
                {
                    if (!_BGWLoadData.IsBusy)
                        _BGWLoadData.RunWorkerAsync();
                }
            }
        }

        private void SumPaied()
        {
            int sumPaied = 0;
            foreach (DataGridViewRow rows in this.dgvData.Rows)
            {
                foreach (DataGridViewCell cell in rows.Cells)
                {
                    if (cell.Value == null || cell.ColumnIndex != 2)
                        continue;

                    sumPaied += ((bool)cell.Value ? 1 : 0); 
                }
            }
            this.PaiedTimes.Text = sumPaied.ToString();
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
            List<PaymentHistory> paymentHistory = Access.Select<PaymentHistory>(string.Format("ref_student_id={0}", PrimaryKey));

            e.Result = new object[] { paymentHistory };
        }

        private void _BGWSaveData_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            this._BGWLoadData.RunWorkerAsync();
        }

        private void _BGWSaveData_DoWork(object sender, DoWorkEventArgs e)
        {
            this.SaveUDT();
        }

        private void Listener_StatusChanged(object sender, ChangeEventArgs e)
        {
            if (UserAcl.Current[typeof(Student_PaymentHistory)].Editable)
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
            this.PaiedTimes.Text = string.Empty;
            this.dgvData.Rows.Clear();
        }

        //  更新資料項目內 UI 的資料
        private void RefreshUI()
        {
            _Listener.SuspendListen();

            this.ClearUI();
            _dicUTDs.Clear();

            List<PaymentHistory> paymentHistory = ((object[])this._Result)[0] as List<PaymentHistory>;
            Semester.DataSource = new BindingList<string>(new List<string>() { "", "夏季學期", "第1學期", "第2學期" });

            int paiedTimes = 0;
            if (paymentHistory != null && paymentHistory.Count > 0)
            {
                for (int i = 0; i < paymentHistory.Count; i++)
                {
                    List<object> list = new List<object>();

                    list.Add(paymentHistory[i].SchoolYear);
                    list.Add(SemesterItem.GetSemesterByCode(paymentHistory[i].Semester.ToString()).Name);
                    list.Add((paymentHistory[i].IsPaied == 0) ? false : true);
                    list.Add(paymentHistory[i].LastModifiedDate.HasValue ? paymentHistory[i].LastModifiedDate.Value.ToString("yyyy/MM/dd hh:mm:ss") : null);
                    list.Add(paymentHistory[i].UID);

                    int idx = this.dgvData.Rows.Add(list.ToArray());
                    this.dgvData.Rows[idx].Tag = paymentHistory[i];
                    _dicUTDs.Add(paymentHistory[i].UID, paymentHistory[i].Clone());

                    paiedTimes += paymentHistory[i].IsPaied;
                }
            }
            this.dgvData.AutoResizeColumns();
            this.PaiedTimes.Text = paiedTimes.ToString();

            this.Loading = false;
            ResetOverrideButton();
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
            this.dgvData.CurrentCell = null;

            if (!AreYouReady())
            {
                MessageBox.Show("請修正錯誤資料再儲存。");
                return;
            }

            this.Loading = true;
            this._BGWSaveData.RunWorkerAsync();
        }

        private void SaveUDT()
        {
            //  待刪除資料
            Dictionary<string, PaymentHistory> deleteRecords = new Dictionary<string, PaymentHistory>();
            //  待更新資料
            Dictionary<string, PaymentHistory> updateRecords = new Dictionary<string, PaymentHistory>();
            //  待新增資料
            List<PaymentHistory> insertRecords = new List<PaymentHistory>();
            //  學生資料
            K12.Data.StudentRecord studentRecord = K12.Data.Student.SelectByID(PrimaryKey);

            foreach (DataGridViewRow dataGridRow in this.dgvData.Rows)
            {
                if (dataGridRow.IsNewRow)
                    continue;

                PaymentHistory paymentHistoryRecord = (PaymentHistory)dataGridRow.Tag;

                if (paymentHistoryRecord == null)
                    paymentHistoryRecord = new PaymentHistory();

                //  學年度
                paymentHistoryRecord.SchoolYear = int.Parse(dataGridRow.Cells["SchoolYear"].Value.ToString());
                //  學期
                paymentHistoryRecord.Semester = int.Parse(SemesterItem.GetSemesterByName(dataGridRow.Cells["Semester"].Value.ToString()).Value);
                //  是否繳費
                bool isPaied = false;
                bool.TryParse(dataGridRow.Cells["IsPaied"].Value == null ? "" : dataGridRow.Cells["IsPaied"].Value.ToString(), out isPaied);
                paymentHistoryRecord.IsPaied = (isPaied ? 1 : 0);
                //  學生系統編號
                paymentHistoryRecord.StudentID = int.Parse(PrimaryKey);

                if (paymentHistoryRecord.RecordStatus == FISCA.UDT.RecordStatus.Insert)
                    insertRecords.Add(paymentHistoryRecord);
                else if (paymentHistoryRecord.RecordStatus == FISCA.UDT.RecordStatus.Update)
                    updateRecords.Add(paymentHistoryRecord.UID, paymentHistoryRecord);
            }

            IEnumerable<DataGridViewRow> dataGridViewRows = this.dgvData.Rows.Cast<DataGridViewRow>();
            foreach (string key in _dicUTDs.Keys)
            {
                if (dataGridViewRows.Where(x => x.Cells["UID"].Value != null).Where(x => (x.Cells["UID"].Value.ToString() == key)).Count() == 0)
                {
                    PaymentHistory paymentHistoryRecord = _dicUTDs[key];
                    paymentHistoryRecord.Deleted = true;
                    deleteRecords.Add(key, paymentHistoryRecord);
                }
            }

            //  更新日期
            if (insertRecords.Count>0)
                insertRecords.ForEach(x=>x.LastModifiedDate = System.DateTime.Now);

            List<string> insertedRecordUIDs = insertRecords.SaveAll();                    //  匯入「新增」資料 
            List<PaymentHistory> insertedRecords = new List<PaymentHistory>();
            if (insertedRecordUIDs != null && insertedRecordUIDs.Count>0)
                insertedRecords = this.Access.Select<PaymentHistory>(insertedRecordUIDs);
            if (insertedRecords != null && insertedRecords.Count > 0)
            {
                //  寫入「新增」的 Log
                Dictionary<string, PaymentHistory> dicInsertedRecords = insertedRecords.ToDictionary(x => x.UID);
                foreach (string iRecords in dicInsertedRecords.Keys)
                {
                    PaymentHistory record = dicInsertedRecords[iRecords];

                    StringBuilder sb = new StringBuilder();

                    sb.Append("學生「" + studentRecord.Name + "」，學號「" + studentRecord.StudentNumber + "」");
                    sb.AppendLine("被新增一筆「繳費紀錄」。");
                    sb.AppendLine("詳細資料：");
                    sb.Append("學年度「" + record.SchoolYear + "」\n");
                    sb.Append("學期「" + record.Semester + "」\n");
                    sb.Append("繳費「" + IsPaidToString(record.IsPaied) + "」\n");
                    
                    FISCA.LogAgent.ApplicationLog.Log("繳費記錄.學生", "新增", Log.LogTargetCategory.Student.ToString().ToLower(), record.StudentID.ToString(), sb.ToString());
                }
            }


            //  更新日期
            if (updateRecords.Count > 0)
                updateRecords.Values.ToList().ForEach(x => x.LastModifiedDate = System.DateTime.Now);

            List<string> updatedRecords = updateRecords.Values.SaveAll();   //  匯入「更新」資料 
            List<PaymentHistory> updatedRecordss = new List<PaymentHistory>();
            if (updatedRecords != null && updatedRecords.Count>0)
                updatedRecordss = Access.Select<PaymentHistory>(updatedRecords);

            if (updatedRecordss != null && updatedRecordss.Count > 0)
            {
                //  批次寫入「修改」的 Log
                foreach (PaymentHistory uRecords in updatedRecordss)
                {
                    PaymentHistory record = uRecords;
                    PaymentHistory oldRecord = _dicUTDs[uRecords.UID];

                    StringBuilder sb = new StringBuilder();

                    sb.Append("學生「" + studentRecord.Name + "」，學號「" + studentRecord.StudentNumber + "」");
                    sb.AppendLine("被修改一筆「繳費記錄」。");
                    sb.AppendLine("詳細資料：");

                    if (!oldRecord.SchoolYear.Equals(record.SchoolYear))
                        sb.Append("學年度由「" + oldRecord.SchoolYear + "」改為「" + record.SchoolYear + "」\n");

                    if (!oldRecord.Semester.Equals(record.Semester))
                        sb.Append("學期由「" + oldRecord.Semester + "」改為「" + record.Semester + "」\n");

                    if (!oldRecord.IsPaied.Equals(record.IsPaied))
                        sb.Append("繳費由「" + IsPaidToString(oldRecord.IsPaied) + "」改為「" + IsPaidToString(record.IsPaied) + "」\n");

                    FISCA.LogAgent.ApplicationLog.Log("繳費記錄.學生", "修改", Log.LogTargetCategory.Student.ToString().ToLower(), record.StudentID.ToString(), sb.ToString());
                }
            }

            //  刪除資料
            List<string> deletedRecords = deleteRecords.Values.SaveAll();
            if (deleteRecords != null && deleteRecords.Count > 0)
            {
                //  寫入「刪除」的 Log
                foreach (string dRecords in deletedRecords)
                {
                    PaymentHistory newRecord = deleteRecords[dRecords];

                    StringBuilder sb = new StringBuilder();

                    sb.Append("學生「" + studentRecord.Name + "」，學號「" + studentRecord.StudentNumber + "」");
                    sb.AppendLine("被刪除一筆「繳費記錄」。");
                    sb.AppendLine("詳細資料：");
                    sb.Append("學年度「" + newRecord.SchoolYear + "」\n");
                    sb.Append("學期「" + newRecord.Semester + "」\n");
                    sb.Append("繳費「" + IsPaidToString(newRecord.IsPaied) + "」\n");

                    FISCA.LogAgent.ApplicationLog.Log("繳費記錄.學生", "刪除", Log.LogTargetCategory.Student.ToString().ToLower(), newRecord.StudentID.ToString(), sb.ToString());
                }
            }
        }

        private string IsPaidToString(int isPaid)
        {
            if (isPaid == 0)
                return "未繳";
            else if (isPaid == 1)
                return "已繳";
            else
                return string.Empty;
        }

        //  檢查輸入畫面是否仍有錯誤訊息
        private bool AreYouReady()
        {
            ValidateData();
            foreach (DataGridViewRow rows in this.dgvData.Rows)
            {
                if (rows.IsNewRow)
                    continue;

                foreach (DataGridViewCell cell in rows.Cells)
                {
                    if (cell.ErrorText != string.Empty)
                        return false;
                }
            }

            return true;
        }

        private void ValidateData()
        {
            List<KeyValuePair<string, List<DataGridViewCell>>> uniqueData = new List<KeyValuePair<string, List<DataGridViewCell>>>();
            foreach (DataGridViewRow rows in this.dgvData.Rows)
            {
                if (rows.IsNewRow)
                    continue;

                foreach (DataGridViewCell cell in rows.Cells)
                {
                    uint d;
                    cell.ErrorText = string.Empty;

                    if (cell.ColumnIndex <= 1 && cell.Value == null)
                    {
                        cell.ErrorText = "必填。";
                        continue;
                    }

                    if (cell.ColumnIndex == 0)
                    {
                        if ((cell.Value != null) && !uint.TryParse(cell.Value.ToString(), out d))
                            cell.ErrorText = "僅允許正整數。";
                    }

                    if (cell.ColumnIndex == 1)
                    {
                        List<DataGridViewCell> cells = new List<DataGridViewCell>();
                        cells.Add(cell.OwningRow.Cells[0]);
                        cells.Add(cell);
                        uniqueData.Add(new KeyValuePair<string, List<DataGridViewCell>>((cell.OwningRow.Cells[0].Value == null ? "" : cell.OwningRow.Cells[0].Value.ToString()) + "_" + cell.Value.ToString(), cells));
                    }
                }
            }
            foreach (KeyValuePair<string, List<DataGridViewCell>> kv in uniqueData)
            {
                IEnumerable<KeyValuePair<string, List<DataGridViewCell>>> duplicateData = uniqueData.Where(x => x.Key == kv.Key);
                if (duplicateData.Count() > 1)
                {
                    foreach (KeyValuePair<string, List<DataGridViewCell>> vk in duplicateData)
                    {
                        foreach(DataGridViewCell cell in vk.Value)
                            cell.ErrorText = "資料重覆。";
                    }
                }
            }
        }

        private void dgvData_DataError(object sender, DataGridViewDataErrorEventArgs e)
        {
            e.Cancel = true;
        }

        void dgvData_CurrentCellDirtyStateChanged(object sender, EventArgs e)
        {
            dgvData.CommitEdit(DataGridViewDataErrorContexts.Commit);
            ValidateData();
            SumPaied();
        }

        private void dgvData_CellEnter(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0 && e.ColumnIndex >= 0 && dgvData.SelectedCells.Count == 1)
            {
                dgvData.BeginEdit(true);
                if (dgvData.CurrentCell != null && dgvData.CurrentCell.GetType().ToString() == "System.Windows.Forms.DataGridViewComboBoxCell")
                    (dgvData.EditingControl as ComboBox).DroppedDown = true;
            }
        }

        private void dgvData_RowsRemoved(object sender, DataGridViewRowsRemovedEventArgs e)
        {
            SumPaied();
        }

        private void dgvData_RowHeaderMouseClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            dgvData.CurrentCell = null;
            dgvData.Rows[e.RowIndex].Selected = true;
        }

        private void dgvData_ColumnHeaderMouseClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            dgvData.CurrentCell = null;
            dgvData.Columns[e.ColumnIndex].Selected = true;
        }

        private void dgvData_MouseClick(object sender, MouseEventArgs e)
        {
            MouseEventArgs args = (MouseEventArgs)e;
            DataGridView dgv = (DataGridView)sender;
            DataGridView.HitTestInfo hit = dgv.HitTest(args.X, args.Y);

            if (hit.Type == DataGridViewHitTestType.TopLeftHeader)
            {
                dgvData.CurrentCell = null;
                dgvData.SelectAll();
            }
        }
    }
}
