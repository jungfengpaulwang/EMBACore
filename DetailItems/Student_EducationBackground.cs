using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Campus.Windows;
using UDTDetailContentBase;
using FISCA.Permission;

namespace EMBACore.DetailItems
{
    [AccessControl("ischool.EMBA.Student.Detail0004", "學歷", "學生>資料項目")]
    public partial class Student_EducationBackground : DetailContentImproved
    {
        private UDTDetailContentBase.MultipleUDCDecorator<UDT.EducationBackground, Student_EducationBackground_SingleForm> dec;
        private bool canEdit = false;

        private string Log_deletedMessage = "";
        
        public Student_EducationBackground()
        {
            InitializeComponent();
            this.Group = "學歷";
        }

        private void Student_EducationBackground_Load(object sender, EventArgs e)
        {
            this.dec = new MultipleUDCDecorator<UDT.EducationBackground, Student_EducationBackground_SingleForm>(this, this.dg, "StudentID");

            /*  Assign Event Handler */
            this.dec.AfterDataLoaded += new EventHandler(dec_AfterDataLoaded);
            this.dec.AfterDeleted += new UDTDetailContentBase.MultipleUDCDecorator<UDT.EducationBackground, Student_EducationBackground_SingleForm>.UDTDetailContentEventHandler(dec_AfterDeleted);
            this.dec.AfterSaved += new UDTDetailContentBase.MultipleUDCDecorator<UDT.EducationBackground, Student_EducationBackground_SingleForm>.UDTDetailContentEventHandler(dec_AfterSaved);
            this.dec.OnReadDataError += new UDTDetailContentBase.MultipleUDCDecorator<UDT.EducationBackground, Student_EducationBackground_SingleForm>.UDTDetailContentErrorEventHandler(dec_OnReadDataError);
            this.dec.OnDeleteError += new UDTDetailContentBase.MultipleUDCDecorator<UDT.EducationBackground, Student_EducationBackground_SingleForm>.UDTDetailContentErrorEventHandler(dec_OnDeleteError);


            /* Check Permission */
            this.canEdit = Permission.Editable;// FISCA.Permission.UserAcl.Current[this.GetType()].Editable; //User.Acl[strSetUserDefineDataAcl].Executable;
            this.btnAddNew.Enabled = this.canEdit;
            this.btnDelete.Enabled = this.canEdit;
            this.btnUpdate.Enabled = this.canEdit;
        }

        void dec_OnDeleteError(object sender, UDTDetailContentBase.UDTDetailContentErrorEventArgs<UDT.EducationBackground> e)
        {
            Util.ShowMsg("刪除資料時發生錯誤！", this.Group);
        }

        void dec_OnReadDataError(object sender, UDTDetailContentBase.UDTDetailContentErrorEventArgs<UDT.EducationBackground> e)
        {
            Util.ShowMsg("讀取資料時發生錯誤！", this.Group);
        }

        //新增或修改成功後會觸發此事件，可以在這裡儲存 Log
        void dec_AfterSaved(object sender, UDTDetailContentBase.UDTDetailContentEventArgs<UDT.EducationBackground> e)
        {
            this.dec.ReloadData();
        }

        //刪除資料成功後會觸發此事件，可以在這裡紀錄並儲存 Log
        void dec_AfterDeleted(object sender, UDTDetailContentBase.UDTDetailContentEventArgs<UDT.EducationBackground> e)
        {
            FISCA.LogAgent.ApplicationLog.Log("學歷.學生", "刪除", "student", this.PrimaryKey, this.Log_deletedMessage);
            this.dec.ReloadData();
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
            K12.Data.StudentRecord stud = K12.Data.Student.SelectByID(this.PrimaryKey);
            StringBuilder sb = new StringBuilder("學生：『"+ stud.Name+ "』刪除以下學歷資料：");
            
            foreach (DataGridViewRow dr in this.dg.SelectedRows)
            {
                sb.Append(string.Format("( 學校:{0}, 系所:{1}, 學位:{2}, 學歷:{3} ) ,",
                    (dr.Cells[0].Value == null) ? "" : dr.Cells[0].Value.ToString(),
                     (dr.Cells[1].Value == null) ? "" : dr.Cells[1].Value.ToString(),
                      (dr.Cells[2].Value == null) ? "" : dr.Cells[2].Value.ToString(),
                       (dr.Cells[3].Value == null) ? "" : dr.Cells[3].Value.ToString()));
            }
            this.Log_deletedMessage = sb.ToString();    //先把訊息紀錄下來，等待 afterdeleted 事件再儲存。
            this.dec.Delete();
            
        }

    }
}
