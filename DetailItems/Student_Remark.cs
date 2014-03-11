using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using FISCA.Presentation;
using UDTDetailContentBase;
using Campus.Windows;
using FISCA.Data;
using FISCA.Permission;

namespace EMBACore.DetailItems
{
    [AccessControl("ischool.EMBA.Student.Detail0006", "備註", "學生>資料項目")]
    public partial class Student_Remark :  DetailContentImproved
    {
        private SingleUDCDecorator<UDT.StudentRemark> decDetailBase;
        private Log.LogAgent logAgent = new Log.LogAgent();

        public Student_Remark()
        {
            InitializeComponent();
            this.Group = "備註";
        }

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

        private void Student_Remark_Load(object sender, EventArgs e)
        {
            this.decDetailBase = new UDTDetailContentBase.SingleUDCDecorator<UDT.StudentRemark>(this, "StudentID");
            this.decDetailBase.AfterDataLoaded += new SingleUDCDecorator<UDT.StudentRemark>.UDTDetailContentEventHandler(decDetailBase_AfterDataLoaded);
            this.decDetailBase.OnValidatingData += new SingleUDCDecorator<UDT.StudentRemark>.UDTDetailContentEventHandler(decDetailBase_OnValidatingData);
            this.decDetailBase.OnReadDataError += new SingleUDCDecorator<UDT.StudentRemark>.UDTDetailContentErrorEventHandler(decDetailBase_OnReadDataError);
            this.decDetailBase.OnSaveDataError += new SingleUDCDecorator<UDT.StudentRemark>.UDTDetailContentErrorEventHandler(decDetailBase_OnSaveDataError);
            this.decDetailBase.OnSaveActionCanceled += new EventHandler(decDetailBase_OnSaveActionCanceled);
            this.decDetailBase.AfterDataSaved += new EventHandler(decDetailBase_AfterDataSaved);

            WatchChange(new TextBoxSource(txtRemark));

        }

        void decDetailBase_AfterDataSaved(object sender, EventArgs e)
        {
            this.logAgent.SetLogValue("備註", this.txtRemark.Text);
            this.logAgent.Save("備註.學生", "", "", Log.LogTargetCategory.Student, this.PrimaryKey);
            this.decDetailBase.ReloadData();
        }

        void decDetailBase_OnSaveActionCanceled(object sender, EventArgs e)
        {
            
        }

        void decDetailBase_OnSaveDataError(object sender, UDTDetailContentErrorEventArgs<UDT.StudentRemark> e)
        {
            ShowMsg("儲存資料時發生錯誤！", this.Group);
        }

        void decDetailBase_OnReadDataError(object sender, UDTDetailContentErrorEventArgs<UDT.StudentRemark> e)
        {
            ShowMsg("讀取資料時發生錯誤！", this.Group);
        }

        void decDetailBase_OnValidatingData(object sender, UDTDetailContentEventArgs<UDT.StudentRemark> e)
        {
            e.CurrentTargets[0].Remark = this.txtRemark.Text;
        }

        void decDetailBase_AfterDataLoaded(object sender, UDTDetailContentEventArgs<UDT.StudentRemark> e)
        {
            this.txtRemark.Text = e.CurrentTargets[0].Remark;
            this.logAgent.Clear();
            this.logAgent.ActionType = (string.IsNullOrWhiteSpace(e.CurrentTargets[0].UID) ? Log.LogActionType.AddNew : Log.LogActionType.Update);
            this.logAgent.SetLogValue("備註", this.txtRemark.Text);
        }

        private void ShowMsg(string msg, string source)
        {
            MessageBox.Show(msg, source, MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

    }
}
