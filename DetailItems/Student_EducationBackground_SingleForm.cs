using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using DevComponents.Editors;

namespace EMBACore.DetailItems
{
    public partial class Student_EducationBackground_SingleForm : UDTDetailContentBase.UDTSingleForm
    {
        private Log.LogAgent logAgent= new Log.LogAgent();

        public Student_EducationBackground_SingleForm()
        {
            InitializeComponent();
            this.AfterSaved += new UDTSingleFormEventHandler(Student_EducationBackground_SingleForm_AfterSaved);
        }

        void Student_EducationBackground_SingleForm_AfterSaved(object sender, string[] uids)
        {
            this.logAgent.Save("學歷.學生", "", "", Log.LogTargetCategory.Student, ((UDT.EducationBackground)this._target).StudentID.ToString());
        }

        protected override void FillData()
        {
            UDT.EducationBackground exp = (UDT.EducationBackground)this._target;
            this.SchoolName.Text = exp.SchoolName;
            this.Department.Text = exp.Department;            
            this.chkIsTop.Checked = exp.IsTop;
            this.Degree.SelectedItem = null;
            foreach (ComboItem citem in this.Degree.Items)
            {
                if (citem.Text == exp.Degree)
                {
                    this.Degree.SelectedItem = citem;
                    break;
                }
            }
            this.logAgent.Clear();
            this.addLog(exp);

        }
        private void addLog(UDT.EducationBackground rec)
        {
            this.logAgent.ActionType = (string.IsNullOrWhiteSpace(rec.UID) ? Log.LogActionType.AddNew : Log.LogActionType.Update);
            this.logAgent.SetLogValue("學校名稱", rec.SchoolName );
            this.logAgent.SetLogValue("系所", rec.Department );
            this.logAgent.SetLogValue("學位", rec.Degree);
            this.logAgent.SetLogValue("是否最高學歷", rec.IsTop.ToString());
        }

        protected override void GatherData()
        {
            UDT.EducationBackground exp = (UDT.EducationBackground)this._target;
            exp.SchoolName = this.SchoolName.Text;
            exp.Department = this.Department.Text;
            exp.Degree = this.Degree.SelectedItem.ToString(); ;
            exp.IsTop = this.chkIsTop.Checked;
            this.addLog(exp);
        }

        protected override bool ValidateData()
        {
            bool result = true;

            if (string.IsNullOrEmpty(this.SchoolName.Text))
            {
                errorProvider1.SetError(this.SchoolName, "必填");
                result = false;
            }
            else
                errorProvider1.SetError(this.SchoolName, "");

            if (string.IsNullOrEmpty(this.Department.Text))
            {
                errorProvider1.SetError(this.Department, "必填");
                result = false;
            }
            else
                errorProvider1.SetError(this.Department, "");

            if (this.Degree.SelectedItem == null)
            {
                errorProvider1.SetError(this.Degree, "必選");
                result = false;
            }
            else
                errorProvider1.SetError(this.Degree, "");

            return result;
        }

        private void btnSave_Click(object sender, EventArgs e)
        {

        }
    }
}
