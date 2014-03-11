using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Campus.Windows;
using FISCA.Permission;

namespace EMBACore.DetailItems
{
    [AccessControl("ischool.EMBA.Student.Detail0005", "經歷", "學生>資料項目")]
    public partial class Student_Experience : DetailContentImproved
    {
        private UDTDetailContentBase.MultipleUDCDecorator<UDT.Experience, Student_Experience_SingleForm> dec;
        private bool canEdit;
        private Log.LogAgent logAgent = new Log.LogAgent();

        public Student_Experience()
        {
            InitializeComponent();
            this.Group = "經歷";
        }

        private void Student_Experience_Load(object sender, EventArgs e)
        {
            this.dec = new UDTDetailContentBase.MultipleUDCDecorator<UDT.Experience, Student_Experience_SingleForm>(this, this.dg,  "StudentID", true);

            /*  Assign Event Handler */
            this.dec.AfterDataLoaded += new EventHandler(dec_AfterDataLoaded);
            this.dec.AfterDeleted += new UDTDetailContentBase.MultipleUDCDecorator<UDT.Experience, Student_Experience_SingleForm>.UDTDetailContentEventHandler(dec_AfterDeleted);
            this.dec.AfterSaved += new UDTDetailContentBase.MultipleUDCDecorator<UDT.Experience, Student_Experience_SingleForm>.UDTDetailContentEventHandler(dec_AfterSaved);
            this.dec.OnReadDataError += new UDTDetailContentBase.MultipleUDCDecorator<UDT.Experience, Student_Experience_SingleForm>.UDTDetailContentErrorEventHandler(dec_OnReadDataError);
            this.dec.OnDeleteError += new UDTDetailContentBase.MultipleUDCDecorator<UDT.Experience, Student_Experience_SingleForm>.UDTDetailContentErrorEventHandler(dec_OnDeleteError);

            /* Check Permission */
            this.canEdit = Permission.Editable;
            this.btnAddNew.Enabled = this.canEdit;
            this.btnDelete.Enabled = this.canEdit;
            this.btnUpdate.Enabled = this.canEdit;
        }

        void dec_OnDeleteError(object sender, UDTDetailContentBase.UDTDetailContentErrorEventArgs<UDT.Experience> e)
        {
            Util.ShowMsg("刪除資料時發生錯誤！", this.Group);
        }

        void dec_OnReadDataError(object sender, UDTDetailContentBase.UDTDetailContentErrorEventArgs<UDT.Experience> e)
        {
            Util.ShowMsg("讀取資料時發生錯誤！", this.Group);
        }

        //新增或修改成功後會觸發此事件，可以在這裡儲存 Log
        void dec_AfterSaved(object sender, UDTDetailContentBase.UDTDetailContentEventArgs<UDT.Experience> e)
        {
            this.dec.ReloadData();
        }

        //刪除資料成功後會觸發此事件，可以在這裡紀錄並儲存 Log
        void dec_AfterDeleted(object sender, UDTDetailContentBase.UDTDetailContentEventArgs<UDT.Experience> e)
        {
            this.logAgent.Clear();
            this.addLog(e.CurrentTargets[0]);
            this.logAgent.Save("學生.資料項目.經歷", "", "", Log.LogTargetCategory.Student, this.PrimaryKey);
            this.dec.ReloadData();
        }

        private void addLog(UDT.Experience rec)
        {
            this.logAgent.ActionType = Log.LogActionType.Delete;
            this.logAgent.SetLogValue("公司名稱", rec.Company);
            this.logAgent.SetLogValue("職稱", rec.Position);
            //this.logAgent.SetLogValue("是否現職", rec.IsCurrent.ToString());
            this.logAgent.SetLogValue("產業別", rec.Industry);
            this.logAgent.SetLogValue("部門類別", rec.DepartmentCategory);
            this.logAgent.SetLogValue("層級別", rec.PostLevel);
            this.logAgent.SetLogValue("工作地點", rec.WorkPlace);
            this.logAgent.SetLogValue("工作狀態", rec.WorkStatus); 
            DateTime work_begin_date;
            if (DateTime.TryParse(rec.WorkBeginDate + "", out work_begin_date))
                this.logAgent.SetLogValue("工作起日", work_begin_date.ToString("yyyy/MM/dd"));
            else
                this.logAgent.SetLogValue("工作起日", rec.WorkBeginDate + "");

            DateTime work_end_date;
            if (DateTime.TryParse(rec.WorkEndDate + "", out work_end_date))
                this.logAgent.SetLogValue("工作迄日", work_end_date.ToString("yyyy/MM/dd"));
            else
                this.logAgent.SetLogValue("工作迄日", rec.WorkEndDate + "");
        }

        //資料讀取成功後會觸發此事件，可以在這裡讀取資料並填入畫面控制項上
        //預設會自行匹配 DataGrid 上的 Column Name 及 ActiveRecord 的 Property Name，然後把資料填到Grid 上。
        //如果要自行填值，就在此事件中覆寫。
        void dec_AfterDataLoaded(object sender, EventArgs e)
        {
            if (this.canEdit)
            {
                //List<UDT.Experience> exps = this.dec.GetRecords();
                this.btnUpdate.Enabled = (this.dg.Rows.Count > 0);
                this.btnDelete.Enabled = (this.dg.Rows.Count > 0);
            }
        }

        //當按下 "新增" 按鈕時
        private void btnAddNew_Click(object sender, EventArgs e)
        {
            this.dec.OpenAddNewForm(); //開啟編輯表單，並把 ActiveRecord 物件傳過去。
        }

        //當按下 "修改" 按鈕時
        private void btnUpdate_Click(object sender, EventArgs e)
        {
            this.dec.OpenUpdateForm();
        }

        //毛毛蟲一定要複寫這兩個 Method !
        protected override void OnPrimaryKeyChangedAsync()
        {
        }
        protected override void OnPrimaryKeyChangedComplete(Exception error)
        {
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

        private void btnDelete_Click(object sender, EventArgs e)
        { 
            this.dec.Delete();
        }


    }
}
