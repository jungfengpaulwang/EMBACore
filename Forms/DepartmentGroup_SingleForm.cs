using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using DevComponents.DotNetBar.Controls;

namespace EMBACore.Forms
{
    public partial class DepartmentGroup_SingleForm : UDTDetailContentBase.UDTSingleForm
    {
        private Log.LogAgent logAgent = new Log.LogAgent();
        public DepartmentGroup_SingleForm()
        {
            InitializeComponent();
            this.AfterSaved += new UDTSingleFormEventHandler(Subject_SingleForm_AfterSaved);
        }

        void Subject_SingleForm_AfterSaved(object sender, string[] uids)
        {
        }

        protected override void FillData()
        {
            UDT.DepartmentGroup dept = (UDT.DepartmentGroup)this._target;
            this.txtSubjectName.Text = dept.Name;
            this.txtEnglishName.Text = dept.EnglishName;
            this.txtDeptCode.Text = dept.Code;
            this.txtOrder.Text = dept.Order;
        }

        protected override void GatherData()
        {
            UDT.DepartmentGroup subj = (UDT.DepartmentGroup)this._target;
            subj.Name =this.txtSubjectName.Text;
            subj.EnglishName=this.txtEnglishName.Text;
            subj.Code=this.txtDeptCode.Text ;
            subj.Order = this.txtOrder.Text;
            this._target = subj;
        }

        protected override bool ValidateData()
        {
            if (!this.checkValue(this.txtSubjectName)) return false;

            return true;
        }

        private bool checkValue(TextBoxX txt) {
            bool result = true;
            if (string.IsNullOrWhiteSpace(txt.Text))
            {
                errorProvider1.SetError(txt, "必填");
                result = false;
            }
            else
                errorProvider1.SetError(txt, "");

            return result;
        }

        private void Subject_SingleForm_Load(object sender, EventArgs e)
        {

        }
    }
}
