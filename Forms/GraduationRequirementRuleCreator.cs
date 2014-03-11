using System;
using System.Xml;
using FISCA.UDT;
using System.Collections.Generic;
using EMBACore.UDT;
using DevComponents.Editors;
using System.Linq;
using System.Windows.Forms;

namespace EMBACore
{
    public partial class GraduationRequirementRuleCreator : FISCA.Presentation.Controls.BaseForm
    {
        private string _Catalog;
        private int _DepartmentGroupID;
        private AccessHelper Access;

        public GraduationRequirementRuleCreator(string catalog, int departmentGroupID)
        {
            InitializeComponent();

            this.Load += new System.EventHandler(this.Form_Load);

            this._Catalog = catalog;
            this._DepartmentGroupID = departmentGroupID;
        }

        private void Form_Load(object sender, EventArgs e)
        {
            this.Text = "新增" + _Catalog;

            Access = new AccessHelper();
            List<GraduationRequirement> graduationRequirements = Access.Select<GraduationRequirement>();
            List<DepartmentGroup> departmentGroups = Access.Select<DepartmentGroup>();

            ComboItem comboItem1 = new ComboItem("不進行複製");
            comboItem1.Tag = null;

            this.cboGraduationRequirementRule.Items.Add(comboItem1);
            foreach (GraduationRequirement var in graduationRequirements)
            {
                IEnumerable<DepartmentGroup> filterRecords = departmentGroups.Where(x => x.UID == var.DepartmentGroupID.ToString());
                if (filterRecords.Count() == 0)
                    continue;

                string departmentGroup = filterRecords.Select(x => x.Name).ElementAt(0);
                ComboItem item = new ComboItem(departmentGroup + "-" + var.Name);
                item.Tag = var;
                cboGraduationRequirementRule.Items.Add(item);
            }

            cboGraduationRequirementRule.SelectedItem = comboItem1;
            txtName.Focus();
        }

        private void buttonX2_Click(object sender, EventArgs e)
        {
            this.Close();
        }
        
        private void Cancel_Click(object sender, EventArgs e)
        {
            this.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.Close();
        }

        private void Save_Click(object sender, EventArgs e)
        {
            string graduationRequirementName = this.txtName.Text.Trim();
            Access = new AccessHelper();

            //  所有的「畢業條件」
            List<GraduationRequirement> graduationRequirements = Access.Select<GraduationRequirement>();

            //  若新增的「畢業條件」名稱已存在，則發出警告並跳出程式
            if (graduationRequirements.Where(x => (x.Name == graduationRequirementName && x.DepartmentGroupID == _DepartmentGroupID)).Count() > 0)
            {
                MessageBox.Show("同名之畢業條件已存在！");
                return;
            }

            //  待新增的「畢業條件」
            GraduationRequirement graduationRequirement = new GraduationRequirement();

            //  檢查是否複製
            ComboItem item = (ComboItem)this.cboGraduationRequirementRule.SelectedItem;
            if (item == null || item.Tag == null)
            {
                //  不複製
                graduationRequirement.Name = graduationRequirementName;
                graduationRequirement.DepartmentGroupID = _DepartmentGroupID;
                graduationRequirement.RequiredCredit = 0;
                graduationRequirement.DepartmentCredit = 0;
                graduationRequirement.ElectiveCredit = 0;

                graduationRequirement.Save();
            }
            else
            {
                //  1、複製「畢業條件」
                GraduationRequirement oGraduationRequirement = (GraduationRequirement)item.Tag;

                graduationRequirement.Name = graduationRequirementName;
                graduationRequirement.DepartmentGroupID = _DepartmentGroupID;
                graduationRequirement.RequiredCredit = oGraduationRequirement.RequiredCredit;
                graduationRequirement.DepartmentCredit = oGraduationRequirement.DepartmentCredit;
                graduationRequirement.ElectiveCredit = oGraduationRequirement.ElectiveCredit;

                graduationRequirements = new List<GraduationRequirement>();
                graduationRequirements.Add(graduationRequirement);
                List<string> graduationRequirementIDs = graduationRequirements.SaveAll();           
                //  2、複製「畢業應修科目清單」
                List<GraduationSubjectList> graduationSubjects = Access.Select<GraduationSubjectList>(string.Format("ref_graduation_requirement_id = {0}", oGraduationRequirement.UID));
                List<GraduationSubjectList> newGraduationSubjects = new List<GraduationSubjectList>();
                foreach(GraduationSubjectList graduationSubject in graduationSubjects)
                {
                    GraduationSubjectList newGraduationSubjectList = new GraduationSubjectList();

                    newGraduationSubjectList.GraduationRequirementID = int.Parse(graduationRequirementIDs[0]);
                    newGraduationSubjectList.SubjectID = graduationSubject.SubjectID;
                    newGraduationSubjectList.Prerequisites = graduationSubject.Prerequisites;

                    newGraduationSubjects.Add(newGraduationSubjectList);
                }
                newGraduationSubjects.SaveAll();
                //  3、複製「畢業應修科目群組應修科目數及應修學分數」--待討論，建議不做

                //  4、重繪「指定畢業條件」選單
                //EMBACore.Initialization.StudentInit.RedrawGraduationRequirementMenuButton();
            }

            this.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.Close();
        }
    }
}