using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using FISCA.UDT;

namespace EMBACore.DetailItems
{
    public partial class Student_Experience_SingleForm_Ext : UDTDetailContentBase.UDTSingleForm
    {
        private Log.LogAgent logAgent = new Log.LogAgent();
        private AccessHelper Access;

        private string _PostLevel;
        private string _Industry;
        private string _DepartmentCategory;
        private string _WorkPlace;
        private string _WorkStatus;

        public Student_Experience_SingleForm_Ext()
        {
            InitializeComponent();
            Access = new AccessHelper();
            this.AfterSaved += new UDTSingleFormEventHandler(Student_Experience_SingleForm_AfterSaved);
            UDT.ExperienceDataSource.AfterUpdate += new EventHandler(ExperienceDataSource_AfterUpdate);
            this.Load += new EventHandler(Student_Experience_SingleForm_Load);
        }

        private void Student_Experience_SingleForm_Load(object sender, EventArgs e)
        {
            this.InitExperienceDataSource();
            UDT.Experience exp = (UDT.Experience)this._target;
            this.cboIndustry.Text = exp.Industry;
            this.cboWorkStatus.Text = exp.WorkStatus;
            this.cboWorkPlace.Text = exp.WorkPlace;
            this.cboPostLevel.Text = exp.PostLevel;
            this.cboDepartmentCategory.Text = exp.DepartmentCategory;
        }

        private void ExperienceDataSource_AfterUpdate(object sender, EventArgs e)
        {
            this.InitExperienceDataSource();
            this.cboIndustry.Text = this._Industry;
            this.cboWorkStatus.Text = this._WorkStatus;
            this.cboWorkPlace.Text = this._WorkPlace;
            this.cboPostLevel.Text = this._PostLevel;
            this.cboDepartmentCategory.Text = this._DepartmentCategory;
        }

        private void InitExperienceDataSource()
        {
            UDT.Experience exp = (UDT.Experience)this._target;
            List<UDT.ExperienceDataSource> ExperienceDataSources = Access.Select<UDT.ExperienceDataSource>();
            List<string> dataSource;

            //  資料繫結「產業別」
            dataSource = new List<string>();
            dataSource.Add(string.Empty);
            ExperienceDataSources.Where(x => x.ItemCategory == "產業別").Where(x=>!x.NotDisplay).Select(x => x.Item).ToList().ForEach(x => dataSource.Add(x));
            dataSource.Add(exp.Industry);
            dataSource = dataSource.Distinct().ToList();
            this.cboIndustry.DataSource = dataSource;

            //  資料繫結「部門類別」
            dataSource = new List<string>();
            dataSource.Add(string.Empty);
            ExperienceDataSources.Where(x => x.ItemCategory == "部門類別").Where(x => !x.NotDisplay).Select(x => x.Item).ToList().ForEach(x => dataSource.Add(x));
            dataSource.Add(exp.DepartmentCategory);
            dataSource = dataSource.Distinct().ToList();
            this.cboDepartmentCategory.DataSource = dataSource;

            //  資料繫結「層級別」
            dataSource = new List<string>();
            dataSource.Add(string.Empty);
            ExperienceDataSources.Where(x => x.ItemCategory == "層級別").Where(x => !x.NotDisplay).Select(x => x.Item).ToList().ForEach(x => dataSource.Add(x));
            dataSource.Add(exp.PostLevel);
            dataSource = dataSource.Distinct().ToList();
            this.cboPostLevel.DataSource = dataSource;

            //  資料繫結「工作地點」
            dataSource = new List<string>();
            dataSource.Add(string.Empty);
            ExperienceDataSources.Where(x => x.ItemCategory == "工作地點").Where(x => !x.NotDisplay).Select(x => x.Item).ToList().ForEach(x => dataSource.Add(x));
            dataSource.Add(exp.WorkPlace);
            dataSource = dataSource.Distinct().ToList();
            this.cboWorkPlace.DataSource = dataSource;

            //  資料繫結「工作狀態」
            dataSource = new List<string>();
            dataSource.Add(string.Empty);
            ExperienceDataSources.Where(x => x.ItemCategory == "工作狀態").Where(x => !x.NotDisplay).Select(x => x.Item).ToList().ForEach(x => dataSource.Add(x));
            dataSource.Add(exp.WorkStatus);
            dataSource = dataSource.Distinct().ToList();
            this.cboWorkStatus.DataSource = dataSource;
        }

        void Student_Experience_SingleForm_AfterSaved(object sender, string[] uids)
        {
            this.logAgent.Save("學生.資料項目.經歷", "", "", Log.LogTargetCategory.Student, ((UDT.Experience)this._target).StudentID.ToString());
        }

        protected override void FillData()
        {
            UDT.Experience exp = (UDT.Experience)this._target;
            this.txtCompany.Text = exp.Company;
            this.txtPosition.Text = exp.Position;
            this.cboIndustry.Text = exp.Industry;
            //this.chkIscurrent.Checked = exp.IsCurrent;
            this.cboDepartmentCategory.Text = exp.DepartmentCategory;
            this.cboPostLevel.Text = exp.PostLevel;
            this.cboWorkPlace.Text = exp.WorkPlace;
            this.cboWorkStatus.Text = exp.WorkStatus;
            
            DateTime work_begin_date;
            if (DateTime.TryParse(exp.WorkBeginDate + "", out work_begin_date))
                this.txtWorkBeginDate.Text = work_begin_date.ToString("yyyy/MM/dd");
            else
                this.txtWorkBeginDate.Text = exp.WorkBeginDate + "";

            DateTime work_end_date;
            if (DateTime.TryParse(exp.WorkEndDate + "", out work_end_date))
                this.txtWorkEndDate.Text = work_end_date.ToString("yyyy/MM/dd");
            else
                this.txtWorkEndDate.Text = exp.WorkEndDate + "";

            this.txtPublicist.Text = exp.Publicist;
            this.txtPublicRelationsOfficeTelephone.Text = exp.PublicRelationsOfficeTelephone;
            this.txtPublicRelationsOfficeFax.Text = exp.PublicRelationsOfficeFax;
            this.txtPublicistEmail.Text = exp.PublicistEmail;
            this.txtCompanyWebSite.Text = exp.CompanyWebsite;

            this.logAgent.Clear();
            if (!string.IsNullOrWhiteSpace(exp.UID))
                this.addLog(exp);
        }

        private void addLog(UDT.Experience rec)
        {
            this.logAgent.ActionType = (string.IsNullOrWhiteSpace(rec.UID) ? Log.LogActionType.AddNew : Log.LogActionType.Update);
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

            this.logAgent.SetLogValue("公關連絡人", rec.Publicist);
            this.logAgent.SetLogValue("公關室電話", rec.PublicRelationsOfficeTelephone);
            this.logAgent.SetLogValue("公關室傳真", rec.PublicRelationsOfficeFax);
            this.logAgent.SetLogValue("公關EMAIL", rec.PublicistEmail);
            this.logAgent.SetLogValue("公司網址", rec.CompanyWebsite);
            this.logAgent.SetLogValue("最後更新日期", rec.TimeStamp.HasValue ? rec.TimeStamp.Value.ToString("yyyy/MM/dd") : "");
        }

        protected override void GatherData()
        {
            UDT.Experience exp = (UDT.Experience)this._target;
            exp.Company = this.txtCompany.Text.Trim();
            exp.Position =this.txtPosition.Text.Trim() ;
            //exp.IsCurrent =this.chkIscurrent.Checked;
            exp.Industry = this.cboIndustry.Text;
            exp.DepartmentCategory = this.cboDepartmentCategory.Text;
            exp.PostLevel = this.cboPostLevel.Text;
            exp.WorkPlace = this.cboWorkPlace.Text;
            exp.WorkStatus = this.cboWorkStatus.Text;
            exp.Publicist = this.txtPublicist.Text.Trim();
            exp.PublicRelationsOfficeTelephone = this.txtPublicRelationsOfficeTelephone.Text.Trim();
            exp.PublicRelationsOfficeFax = this.txtPublicRelationsOfficeFax.Text.Trim();
            exp.PublicistEmail = this.txtPublicistEmail.Text.Trim();
            exp.CompanyWebsite = this.txtCompanyWebSite.Text.Trim();
            exp.TimeStamp = DateTime.Now;

            DateTime work_begin_date;
            if (DateTime.TryParse(this.txtWorkBeginDate.Text, out work_begin_date))
                exp.WorkBeginDate = work_begin_date;
            else
                exp.WorkBeginDate = null;

            DateTime work_end_date;
            if (DateTime.TryParse(this.txtWorkEndDate.Text, out work_end_date))
                exp.WorkEndDate = work_end_date;
            else
                exp.WorkEndDate = null;

            this.addLog(exp);
        }

        protected override bool ValidateData()
        {
            bool result = true;
            UDT.Experience exp = (UDT.Experience)this._target;
            List<UDT.Experience> Exps = Access.Select<UDT.Experience>("ref_student_id=" + exp.StudentID);

            if (string.IsNullOrEmpty(this.txtCompany.Text))
            {
                errorProvider1.SetError(this.txtCompany, "必填");
                result = false;
            }
            else
            {                
                if (exp.RecordStatus == RecordStatus.Insert)
                {
                    if (Exps.Where(x => x.Company.Trim().ToLower() == this.txtCompany.Text.Trim().ToLower()).Count() > 0)
                    {
                        errorProvider1.SetError(this.txtCompany, "公司名稱已存在，請直接覆寫職稱或其它資訊。");
                        result = false;
                    }
                    else
                        errorProvider1.SetError(this.txtCompany, "");
                }
                else 
                {
                    if (Exps.Where(x => (x.Company.Trim().ToLower() == this.txtCompany.Text.Trim().ToLower() && (x.UID != exp.UID))).Count() > 0)
                    {
                        errorProvider1.SetError(this.txtCompany, "公司名稱已存在，請直接覆寫職稱或其它資訊。");
                        result = false;
                    }
                    else
                        errorProvider1.SetError(this.txtCompany, "");
                }
            }

            if (string.IsNullOrEmpty(this.txtPosition.Text))
            {
                errorProvider1.SetError(this.txtPosition, "必填");
                result = false;
            }
            else
                errorProvider1.SetError(this.txtPosition, "");

            if (string.IsNullOrEmpty(this.txtWorkBeginDate.Text))
            {
                errorProvider1.SetError(this.txtWorkBeginDate, "");
            }
            else
            {
                DateTime work_begin_date;
                if (!DateTime.TryParse(this.txtWorkBeginDate.Text, out work_begin_date))
                {
                    errorProvider1.SetError(this.txtWorkBeginDate, "請依照範例格式輸入。");
                    result = false;
                }
                else
                    errorProvider1.SetError(this.txtWorkBeginDate, "");
            }

            if (string.IsNullOrEmpty(this.txtWorkEndDate.Text))
            {
                errorProvider1.SetError(this.txtWorkEndDate, "");
            }
            else
            {
                DateTime work_end_date;
                if (!DateTime.TryParse(this.txtWorkEndDate.Text, out work_end_date))
                {
                    errorProvider1.SetError(this.txtWorkEndDate, "請依照範例格式輸入。");
                    result = false;
                }
                else
                    errorProvider1.SetError(this.txtWorkEndDate, "");
            }

            //if (string.IsNullOrEmpty(this.txtIndustry.Text))
            //{
            //    errorProvider1.SetError(this.txtIndustry, "必填");
            //    result = false;
            //}
            //else
            //    errorProvider1.SetError(this.txtIndustry, "");
              
            return result;
        }

        private void btnPostLevel_Click(object sender, EventArgs e)
        {
            PreserveComboBoxText();
            (new EMBACore.Forms.ExperienceDataSourceManagement("層級別")).ShowDialog();
        }

        private void btnIndustry_Click(object sender, EventArgs e)
        {
            PreserveComboBoxText();
            (new EMBACore.Forms.ExperienceDataSourceManagement("產業別")).ShowDialog();
        }

        private void btnDepartmentCategory_Click(object sender, EventArgs e)
        {
            PreserveComboBoxText();
            (new EMBACore.Forms.ExperienceDataSourceManagement("部門類別")).ShowDialog();
        }

        private void btnWorkPlace_Click(object sender, EventArgs e)
        {
            PreserveComboBoxText();
            (new EMBACore.Forms.ExperienceDataSourceManagement("工作地點")).ShowDialog();
        }

        private void btnWorkStatus_Click(object sender, EventArgs e)
        {
            PreserveComboBoxText();
            (new EMBACore.Forms.ExperienceDataSourceManagement("工作狀態")).ShowDialog();
        }

        private void PreserveComboBoxText()
        {
            this._PostLevel = this.cboPostLevel.Text;
            this._Industry = this.cboIndustry.Text;
            this._DepartmentCategory = this.cboDepartmentCategory.Text;
            this._WorkPlace = this.cboWorkPlace.Text;
            this._WorkStatus = this.cboWorkStatus.Text;
        }

        private void btnBrowse_Click(object sender, EventArgs e)
        {
            if (!string.IsNullOrWhiteSpace(this.txtCompanyWebSite.Text))
                System.Diagnostics.Process.Start(this.txtCompanyWebSite.Text.Trim());
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            return;
        }
    }
}
