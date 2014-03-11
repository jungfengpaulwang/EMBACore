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
    public partial class Subject_SingleForm : UDTDetailContentBase.UDTSingleForm
    {
        private Log.LogAgent logAgent = new Log.LogAgent();
        private List<UDT.Subject> subjects ;
        private Dictionary<string, UDT.Subject> dicNewSubjCodes = new Dictionary<string,UDT.Subject>();
        private Dictionary<string, UDT.Subject> dicSubjCodes = new Dictionary<string, UDT.Subject>();

        public Subject_SingleForm()
        {
            InitializeComponent();
            this.AfterSaved += new UDTSingleFormEventHandler(Subject_SingleForm_AfterSaved);
        }

        //用來檢查 識別碼及 課號是否重複
        public void SetSubjects(List<UDT.Subject> subjects)
        {
            this.subjects = subjects;
            this.dicNewSubjCodes.Clear();
            this.dicSubjCodes.Clear();
            foreach (UDT.Subject subj in this.subjects)
            {
                if (!string.IsNullOrWhiteSpace(subj.NewSubjectCode)) {
                    if (!this.dicNewSubjCodes.ContainsKey(subj.NewSubjectCode))
                        this.dicNewSubjCodes.Add(subj.NewSubjectCode, subj);
                }

                if (!string.IsNullOrWhiteSpace(subj.SubjectCode))
                {
                    if (!this.dicSubjCodes.ContainsKey(subj.SubjectCode))
                        this.dicSubjCodes.Add(subj.SubjectCode, subj);
                }
            }
        }

        void Subject_SingleForm_AfterSaved(object sender, string[] uids)
        {
            this.logAgent.Save("科目管理", "", "", Log.LogTargetCategory.Course, "");
        }

        protected override void FillData()
        {
            UDT.Subject subj = (UDT.Subject)this._target;
            this.txtSubjectName.Text = subj.Name;
            this.txtEnglishName.Text = subj.EnglishName;
            this.txtDept.Text = subj.DeptName;
            this.txtDeptCode.Text = subj.DeptCode;
            this.txtSubjectCode1.Text = subj.SubjectCode;
            this.txtNewSubjectCode.Text = subj.NewSubjectCode;
            this.txtWebUrl.Text = subj.WebUrl;
            this.txtDescription.Text = subj.Description;
            this.txtRemark.Text = subj.Remark;
            this.chkIsRequired.Checked = subj.IsRequired;
            this.nudCredit.Value = subj.Credit;

            this.addLog();
        }

        protected override void GatherData()
        {
            UDT.Subject subj = (UDT.Subject)this._target;
             subj.Name =this.txtSubjectName.Text;
             subj.EnglishName=this.txtEnglishName.Text;
             subj.DeptName=this.txtDept.Text;
            subj.DeptCode=this.txtDeptCode.Text ;
             subj.SubjectCode = string.Format("{0}",this.txtSubjectCode1.Text );
             subj.NewSubjectCode = this.txtNewSubjectCode.Text;
             subj.WebUrl =this.txtWebUrl.Text;
             subj.Description =this.txtDescription.Text;
             subj.Remark =this.txtRemark.Text;
             subj.IsRequired =this.chkIsRequired.Checked;
             subj.Credit = (int)this.nudCredit.Value;

             this.addLog();
             this.logAgent.ActionType = (string.IsNullOrWhiteSpace(subj.UID)) ? Log.LogActionType.AddNew : Log.LogActionType.Update;
        }

        private void addLog()
        {
            this.logAgent.SetLogValue("科目名稱", this.txtSubjectName.Text);
            this.logAgent.SetLogValue("英文名稱", this.txtEnglishName.Text);
            this.logAgent.SetLogValue("系所名稱", this.txtDept.Text);
            this.logAgent.SetLogValue("系所代碼", this.txtDeptCode.Text);
            this.logAgent.SetLogValue("識別碼", this.txtSubjectCode1.Text);
            this.logAgent.SetLogValue("課號", this.txtNewSubjectCode.Text);
            this.logAgent.SetLogValue("網址", this.txtWebUrl.Text);
            this.logAgent.SetLogValue("描述", this.txtDescription.Text);
            this.logAgent.SetLogValue("備註", this.txtRemark.Text);
            this.logAgent.SetLogValue("必修", this.chkIsRequired.Checked.ToString());
            this.logAgent.SetLogValue("學分", this.nudCredit.Value.ToString());
        }

        protected override bool ValidateData()
        {
            bool result = true;
            if (!this.checkValue(this.txtSubjectName)) return false;
            if (!this.checkValue(this.txtDept))  return false ;
            if (!this.checkValue(this.txtSubjectCode1))  return false;

            //檢查識別碼及課號是否重複
            UDT.Subject subj = (UDT.Subject)this._target;

            //如果是新增，直接檢查就可以了。
            if (string.IsNullOrWhiteSpace(subj.UID))
            {               
                if (!string.IsNullOrWhiteSpace(this.txtNewSubjectCode.Text))
                {
                    if (this.dicNewSubjCodes.ContainsKey(this.txtNewSubjectCode.Text))
                    {
                        UDT.Subject subjTarget = this.dicNewSubjCodes[this.txtNewSubjectCode.Text];
                        Util.ShowMsg(string.Format("課號：『{0}』與課程：『{1}』的課號重複，請修正！",
                                this.txtNewSubjectCode.Text, subjTarget.Name ) ,"新增課程");
                        result = false;
                    }
                }

                if (!string.IsNullOrWhiteSpace(this.txtSubjectCode1.Text))
                {
                    if (this.dicSubjCodes.ContainsKey(this.txtSubjectCode1.Text))
                    {
                        UDT.Subject subjTarget = this.dicSubjCodes[this.txtSubjectCode1.Text];
                        Util.ShowMsg(string.Format("識別碼：『{0}』與課程：『{1}』的識別碼重複，請修正！",
                                this.txtSubjectCode1.Text, subjTarget.Name), "新增課程");
                        result = false;
                    }
                }
            }
            else  //如果是修改，則要檢查是不是同一筆記錄 
            {
                if (!string.IsNullOrWhiteSpace(this.txtNewSubjectCode.Text))
                {
                    if (this.dicNewSubjCodes.ContainsKey(this.txtNewSubjectCode.Text))
                    {
                        UDT.Subject subjTarget = this.dicNewSubjCodes[this.txtNewSubjectCode.Text];
                        if (subjTarget.UID != subj.UID)
                        {
                            Util.ShowMsg(string.Format("課號：『{0}』與課程：『{1}』的課號重複，請修正！",
                                    this.txtNewSubjectCode.Text, subjTarget.Name), "修改課程");
                            result = false;
                        }
                    }
                }

                if (!string.IsNullOrWhiteSpace(this.txtSubjectCode1.Text))
                {
                    if (this.dicSubjCodes.ContainsKey(this.txtSubjectCode1.Text))
                    {
                        UDT.Subject subjTarget = this.dicSubjCodes[this.txtSubjectCode1.Text];
                        if (subjTarget.UID != subj.UID)
                        {
                            Util.ShowMsg(string.Format("識別碼：『{0}』與課程：『{1}』的識別碼重複，請修正！",
                                    this.txtSubjectCode1.Text, subjTarget.Name), "修改課程");
                            result = false;
                        }
                    }
                }
            }

            return result ;
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

        private void btnSave_Click(object sender, EventArgs e)
        {

        }
    }
}
