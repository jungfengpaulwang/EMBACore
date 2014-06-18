using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
////using Westwind.Windows.Dialogs;
using System.IO;
using FISCA.UDT;
using EMBACore.UDT;
using DevComponents.Editors;
using System.Dynamic;
using FISCA.Data;

namespace EMBACore
{
    public partial class SetCSIdentity : FISCA.Presentation.Controls.BaseForm
    {
        //    記錄所有 UDT 資料，便於比對待刪除資料
        //    private Dictionary<string, TIContainRecord> _dicUTDs;
        private AccessHelper Access;
        private QueryHelper queryHelper;
        private Dictionary<string, dynamic> dicSubjects;
        private Dictionary<string, dynamic> dicGroupSubjects;
        private List<Subject> subjects;
        private List<DataGridViewRow> assignedGraduationRequirementStudent;
        bool notDelete;
        public SetCSIdentity()
        {
            InitializeComponent();

            this.dgvMotherData.DataError += new DataGridViewDataErrorEventHandler(dgvMotherData_DataError);
            this.dgvMotherData.CurrentCellDirtyStateChanged += new EventHandler(dgvMotherData_CurrentCellDirtyStateChanged);
            this.dgvMotherData.CellEnter += new DataGridViewCellEventHandler(dgvMotherData_CellEnter);
            this.dgvMotherData.ColumnHeaderMouseClick += new System.Windows.Forms.DataGridViewCellMouseEventHandler(this.dgvMotherData_ColumnHeaderMouseClick);
            this.dgvMotherData.RowHeaderMouseClick += new System.Windows.Forms.DataGridViewCellMouseEventHandler(this.dgvMotherData_RowHeaderMouseClick);
            this.dgvMotherData.MouseClick += new System.Windows.Forms.MouseEventHandler(this.dgvMotherData_MouseClick);
            this.dgvMotherData.EditingControlShowing += new System.Windows.Forms.DataGridViewEditingControlShowingEventHandler(this.dgvMotherData_EditingControlShowing);

            this.dgvChildData.DataError += new DataGridViewDataErrorEventHandler(dgvMotherData_DataError);
            this.dgvChildData.CurrentCellDirtyStateChanged += new EventHandler(dgvChildData_CurrentCellDirtyStateChanged);
            this.dgvChildData.CellEnter += new DataGridViewCellEventHandler(dgvChildData_CellEnter);
            this.dgvChildData.ColumnHeaderMouseClick += new System.Windows.Forms.DataGridViewCellMouseEventHandler(this.dgvChildData_ColumnHeaderMouseClick);
            this.dgvChildData.RowHeaderMouseClick += new System.Windows.Forms.DataGridViewCellMouseEventHandler(this.dgvChildData_RowHeaderMouseClick);
            this.dgvChildData.MouseClick += new System.Windows.Forms.MouseEventHandler(this.dgvChildData_MouseClick);
            this.cboDepartmentGroup.SelectedIndexChanged += new System.EventHandler(this.cboDepartmentGroup_SelectedIndexChanged);

            this.Load += new System.EventHandler(this.Form_Load);
        }

        private void Form_Load(object sender, EventArgs e)
        {
            Access = new AccessHelper();
            subjects = Access.Select<Subject>();

            if (subjects.Count > 0)
                subjects = subjects.OrderBy(x => x.SubjectCode).ToList();

            BindDepartmentGroup();

            this.lstGraduationRequirementRule.DisplayMember = "Name";
            this.lstGraduationRequirementRule.ValueMember = "UID";

            notDelete = true;
        }

        private void BindDepartmentGroup()
        {
            List<DepartmentGroup> departmentGroups = Access.Select<DepartmentGroup>();

            ComboItem comboItem1 = new ComboItem("");
            comboItem1.Tag = null;

            this.cboDepartmentGroup.Items.Add(comboItem1);
            foreach (DepartmentGroup var in departmentGroups)
            {
                string departmentGroup = var.Name;
                ComboItem item = new ComboItem(departmentGroup);
                item.Tag = var;
                cboDepartmentGroup.Items.Add(item);
            }
            cboDepartmentGroup.SelectedItem = comboItem1;
        }

        private void Add_Click(object sender, EventArgs e)
        {
            ComboItem item = (ComboItem)this.cboDepartmentGroup.SelectedItem;

            if (item.Tag == null)
            {
                System.Windows.Forms.MessageBox.Show("請先選擇系所組別");
                return;
            }
            else
            {
                DepartmentGroup departmentGroup = (item.Tag as DepartmentGroup);

                if ((new GraduationRequirementRuleCreator("畢業條件", Convert.ToInt32(departmentGroup.UID))).ShowDialog() == System.Windows.Forms.DialogResult.OK)
                {
                    BindGraduationRequirementRule(departmentGroup.UID);
                }
            }
        }

        private void cboDepartmentGroup_SelectedIndexChanged(object sender, EventArgs e)
        {
            notDelete = true;
            ComboItem item = ((sender as DevComponents.DotNetBar.Controls.ComboBoxEx).SelectedItem as ComboItem);

            if (item.Tag == null)
                BindGraduationRequirementRule(string.Empty);
            else
            {
                DepartmentGroup departmentGroup = (item.Tag as DepartmentGroup);
                BindGraduationRequirementRule(departmentGroup.UID);
            }
        }

        private void BindGraduationRequirementRule(string departmentGroupID)
        {
            this.lstGraduationRequirementRule.Items.Clear();

            if (departmentGroupID == string.Empty)
                return;

            List<GraduationRequirement> graduationRequirements = Access.Select<GraduationRequirement>(string.Format("ref_department_group_id = {0}", departmentGroupID));
            foreach (GraduationRequirement graduationRequirement in graduationRequirements)
            {
                dynamic item = new ExpandoObject();

                //item.UID = graduationRequirement.UID;
                //item.Name = graduationRequirement.Name;
                //item.Tag = graduationRequirement;
                //item.DepartmentCredit = graduationRequirement.DepartmentCredit;
                //item.ElectiveCredit = graduationRequirement.ElectiveCredit;
                //item.RequiredCredit = graduationRequirement.RequiredCredit;

                this.lstGraduationRequirementRule.Items.Add(new { UID = graduationRequirement.UID, Name = graduationRequirement.Name, DepartmentCredit = graduationRequirement.DepartmentCredit, ElectiveCredit = graduationRequirement.ElectiveCredit, RequiredCredit = graduationRequirement.RequiredCredit, GraduationRequirement = graduationRequirement });
                //this.lstGraduationRequirementRule.Items.Add(item);
            }
        }

        private void lstGraduationRequirementRule_SelectedIndexChanged(object sender, EventArgs e)
        {
            notDelete = true;
            System.Windows.Forms.ListBox lstBox = (sender as System.Windows.Forms.ListBox);
            if (lstBox.SelectedIndex == -1)
                return;

            this.GraduationRequirementRuleName.Text = ((dynamic)lstBox.SelectedItem).Name;
            //  畢業應修總學分
            this.RequiredCredit.Text = ((dynamic)lstBox.SelectedItem).RequiredCredit.ToString();
            //  畢業應選學分數
            this.ElectiveCredit.Text = ((dynamic)lstBox.SelectedItem).ElectiveCredit.ToString();
            //  系訂必修學分數
            this.DepartmentCredit.Text = ((dynamic)lstBox.SelectedItem).DepartmentCredit.ToString();
            this.GraduationRequirementRuleName.Tag = ((dynamic)lstBox.SelectedItem).GraduationRequirement;
            BindRightBigBlock(((dynamic)lstBox.SelectedItem).UID);
        }

        private void BindRightBigBlock(string graduationRequirementRuleID)
        {
            this.dgvMotherData.Rows.Clear();
            this.dgvChildData.Rows.Clear();

            dicSubjects = new Dictionary<string, dynamic>();
            if (subjects.Count > 0)
            {
                foreach (Subject subject in subjects)
                {
                    dynamic oo = new ExpandoObject();

                    oo.UID = subject.UID;
                    oo.SubjectCode = subject.SubjectCode;
                    oo.Credit = subject.Credit;
                    oo.EnglishName = subject.EnglishName;
                    oo.Name = subject.Name;
                    oo.SubjectGroup = "";
                    oo.IsDeptRequired = false;
                    oo.Prerequisites = "";

                    dicSubjects.Add(subject.SubjectCode, oo);
                }
            }

            queryHelper = new QueryHelper();

            string strSQL = string.Format("select $ischool.emba.graduation_subject_list.uid subjectlistid, $ischool.emba.subject.uid as subjectid, $ischool.emba.subject.subject_code, $ischool.emba.subject.name, $ischool.emba.subject.eng_name, $ischool.emba.subject.credit, $ischool.emba.graduation_subject_list.subject_group, $ischool.emba.graduation_subject_list.prerequisites, $ischool.emba.graduation_subject_list.is_dept_required from $ischool.emba.subject join $ischool.emba.graduation_subject_list on $ischool.emba.graduation_subject_list.ref_subject_id=$ischool.emba.subject.uid where $ischool.emba.graduation_subject_list.ref_graduation_requirement_id={0} order by $ischool.emba.subject.subject_code;", graduationRequirementRuleID);

            DataTable dataTable = queryHelper.Select(strSQL);
            SubjectCode.DataSource = new BindingList<string>(new List<string>(subjects.Select(x=>x.SubjectCode)));
            foreach (DataRow row in dataTable.Rows)
            {
                dynamic oo = dicSubjects[row["subject_code"].ToString()];
                oo.SubjectGroup = row["subject_group"].ToString();
                oo.Prerequisites = row["prerequisites"].ToString();

                bool IsDeptRequired = false;
                bool.TryParse(row["is_dept_required"] + "", out IsDeptRequired);
                oo.IsDeptRequired = IsDeptRequired;

                List<object> list = new List<object>();

                list.Add(row["subject_code"].ToString());
                list.Add(row["name"].ToString());
                list.Add(row["eng_name"].ToString());
                list.Add(row["credit"].ToString());
                list.Add(row["subject_group"].ToString());
                list.Add((bool)oo.IsDeptRequired);
                list.Add(row["prerequisites"].ToString());


                int idx = this.dgvMotherData.Rows.Add(list.ToArray());
            }
            this.dgvMotherData.CurrentCell = null;

            List<GraduationSubjectGroupRequirement> graduationSubjectGroupRequirements = Access.Select<GraduationSubjectGroupRequirement>(string.Format("ref_graduation_requirement_id={0}", graduationRequirementRuleID));

            dicGroupSubjects = new Dictionary<string, dynamic>();
            foreach (GraduationSubjectGroupRequirement graduationSubjectGroupRequirement in graduationSubjectGroupRequirements)
            {
                dynamic oo = new ExpandoObject();

                oo.SubjectGroup = graduationSubjectGroupRequirement.SubjectGroup;
                oo.LowestSubjectCount = graduationSubjectGroupRequirement.LowestSubjectCount;
                oo.LowestCredit = graduationSubjectGroupRequirement.LowestCredit;
                oo.SubjectChineseNames = "";

                dicGroupSubjects.Add(oo.SubjectGroup, oo);
            }
            //  已指定學生
            this.dgvData.Rows.Clear();
            assignedGraduationRequirementStudent = new List<DataGridViewRow>();
            strSQL = string.Format(@"select student.id id, student.name studentname, student.student_number studentnumber, case student.gender when '0' then '女' else '男' end gender, class.class_name classname from $ischool.emba.student_brief2 join $ischool.emba.graduation_requirement on $ischool.emba.student_brief2.ref_graduation_requirement_id=$ischool.emba.graduation_requirement.uid join student on student.id=$ischool.emba.student_brief2.ref_student_id left join class on class.id=student.ref_class_id where $ischool.emba.graduation_requirement.uid={0}
", graduationRequirementRuleID);

            dataTable = queryHelper.Select(strSQL);
            foreach (DataRow row in dataTable.Rows)
            {
                List<object> list = new List<object>();

                list.Add(row["id"] + "");
                list.Add(row["classname"] + "");
                list.Add(row["studentnumber"] + "");
                list.Add(row["studentname"] + "");
                list.Add(row["gender"] + "");

                int idx = this.dgvData.Rows.Add(list.ToArray());
                assignedGraduationRequirementStudent.Add(this.dgvData.Rows[idx]);
            }
            this.dgvData.CurrentCell = null;
            notDelete = false;
            this.BindChildData();
        }

        private void dgvChildData_CurrentCellDirtyStateChanged(object sender, EventArgs e)
        {
            dgvMotherData.CommitEdit(DataGridViewDataErrorContexts.Commit);
            ValidateData();
        }

        private void ValidateData()
        {
            foreach (DataGridViewRow rows in this.dgvChildData.Rows)
            {
                if (rows.IsNewRow)
                    continue;

                foreach (DataGridViewCell cell in rows.Cells)
                {
                    if (!(cell.ColumnIndex == 1 || cell.ColumnIndex == 2))
                        continue;

                    uint d;
                    cell.ErrorText = string.Empty;

                    if ((cell.Value != null) && !uint.TryParse(cell.Value.ToString(), out d))
                        cell.ErrorText = "僅允許正整數。";
                }
            }
        }

        private void dgvMotherData_CurrentCellDirtyStateChanged(object sender, EventArgs e)
        {
            int rowIndex = dgvMotherData.CurrentCell.RowIndex;
            int colIndex = dgvMotherData.CurrentCell.ColumnIndex;

            if (dgvMotherData.Rows[rowIndex].Cells[0].Value == null) return;

            dgvMotherData.CommitEdit(DataGridViewDataErrorContexts.Commit);

            if (colIndex == 4 || colIndex == 5)
            {
                dynamic oo = dicSubjects[dgvMotherData.Rows[rowIndex].Cells[0].Value.ToString()];

                //  科目群組
                if (colIndex == 4)
                {
                    oo.SubjectGroup = (dgvMotherData.CurrentCell.Value == null ? "" : dgvMotherData.CurrentCell.Value.ToString());
                    if (!dicGroupSubjects.ContainsKey(oo.SubjectGroup))
                    {
                        dynamic op = new ExpandoObject();

                        op.SubjectGroup = oo.SubjectGroup;
                        op.LowestSubjectCount = "";
                        op.LowestCredit = "";
                        op.SubjectChineseNames = "";

                        dicGroupSubjects.Add(oo.SubjectGroup, op);
                    }

                    BindChildData();
                }
                //  必修認可範圍
                if (colIndex == 5)
                    oo.Prerequisites = dgvMotherData.CurrentCell.Value.ToString();
            }
        }

        private void BindChildData()
        {
            IEnumerable<DataGridViewRow> rowsMother = this.dgvMotherData.Rows.Cast<DataGridViewRow>();
            IEnumerable<DataGridViewRow> rowsChild = this.dgvChildData.Rows.Cast<DataGridViewRow>();

            Dictionary<string, List<DataGridViewRow>> dicMotherRows = new Dictionary<string, List<DataGridViewRow>>();

            foreach (DataGridViewRow dataGridViewRow in rowsMother.Where(x=>x.Cells["SubjectGroup"].Value != null).Where(x=>!string.IsNullOrWhiteSpace(x.Cells["SubjectGroup"].Value.ToString())))
            {
                string subjectGroup = dataGridViewRow.Cells["SubjectGroup"].Value.ToString().Trim();
                if (!dicMotherRows.ContainsKey(subjectGroup))
                    dicMotherRows.Add(subjectGroup, new List<DataGridViewRow>());

                dicMotherRows[subjectGroup].Add(dataGridViewRow);
            }
            foreach (string subjectGroup in dicMotherRows.Keys)
            {
                string subjectChineseName = String.Join("、", dicMotherRows[subjectGroup].Select(x => x.Cells["SubjectChineseName"].Value.ToString()));

                if (rowsChild.Where(x => x.Cells["SubjectGroup_Enactment"].Value.ToString() == subjectGroup).Count() > 0)
                {
                    DataGridViewRow dataGridViewRow = rowsChild.Where(x => x.Cells["SubjectGroup_Enactment"].Value.ToString() == subjectGroup).ElementAt(0);
                    dataGridViewRow.Cells["SubjectChineseName_Enactment"].Value = subjectChineseName;
                    dataGridViewRow.Cells["Subject_Count"].Value = ((dynamic)dicGroupSubjects[subjectGroup]).LowestSubjectCount;
                    dataGridViewRow.Cells["Credits"].Value = ((dynamic)dicGroupSubjects[subjectGroup]).LowestCredit;
                }
                else
                {
                    int rowIndex = this.dgvChildData.Rows.Add();

                    this.dgvChildData.Rows[rowIndex].Cells["SubjectGroup_Enactment"].Value = subjectGroup;
                    this.dgvChildData.Rows[rowIndex].Cells["SubjectChineseName_Enactment"].Value = subjectChineseName;

                    if (dicGroupSubjects.ContainsKey(subjectGroup))
                    {
                        this.dgvChildData.Rows[rowIndex].Cells["Subject_Count"].Value = ((dynamic)dicGroupSubjects[subjectGroup]).LowestSubjectCount;
                        this.dgvChildData.Rows[rowIndex].Cells["Credits"].Value = ((dynamic)dicGroupSubjects[subjectGroup]).LowestCredit;
                    }
                }
            }
            foreach (DataGridViewRow dataGridViewRow in this.dgvChildData.Rows)
            {
                if (!dicMotherRows.ContainsKey(dataGridViewRow.Cells["SubjectGroup_Enactment"].Value.ToString()))
                    this.dgvChildData.Rows.Remove(dataGridViewRow);
            }
        }

        private void dgvChildData_CellEnter(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0 && e.ColumnIndex >= 0 && dgvChildData.SelectedCells.Count == 1)
            {
                dgvChildData.BeginEdit(true);
            }
        }

        private void dgvMotherData_CellEnter(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0 && e.ColumnIndex >= 0 && dgvMotherData.SelectedCells.Count == 1)
            {
                dgvMotherData.BeginEdit(true);
                if (dgvMotherData.CurrentCell != null && dgvMotherData.CurrentCell.GetType().ToString() == "System.Windows.Forms.DataGridViewComboBoxCell")
                    (dgvMotherData.EditingControl as ComboBox).DroppedDown = true;
            }
        }

        private void dgvChildData_RowHeaderMouseClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            dgvChildData.CurrentCell = null;
            dgvChildData.Rows[e.RowIndex].Selected = true;
        }

        private void dgvChildData_ColumnHeaderMouseClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            dgvChildData.CurrentCell = null;
            dgvChildData.Columns[e.ColumnIndex].Selected = true;
        }

        private void dgvMotherData_RowHeaderMouseClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            dgvMotherData.CurrentCell = null;
            dgvMotherData.Rows[e.RowIndex].Selected = true;
        }

        private void dgvMotherData_ColumnHeaderMouseClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            dgvMotherData.CurrentCell = null;
            dgvMotherData.Columns[e.ColumnIndex].Selected = true;
        }

        private void dgvChildData_MouseClick(object sender, MouseEventArgs e)
        {
            MouseEventArgs args = (MouseEventArgs)e;
            DataGridView dgv = (DataGridView)sender;
            DataGridView.HitTestInfo hit = dgv.HitTest(args.X, args.Y);

            if (hit.Type == DataGridViewHitTestType.TopLeftHeader)
            {
                dgvChildData.CurrentCell = null;
                dgvChildData.SelectAll();
            }
        }

        private void dgvMotherData_MouseClick(object sender, MouseEventArgs e)
        {
            MouseEventArgs args = (MouseEventArgs)e;
            DataGridView dgv = (DataGridView)sender;
            DataGridView.HitTestInfo hit = dgv.HitTest(args.X, args.Y);

            if (hit.Type == DataGridViewHitTestType.TopLeftHeader)
            {
                dgvMotherData.CurrentCell = null;
                dgvMotherData.SelectAll();
            }
        }

        private void dgvMotherData_EditingControlShowing(object sender, DataGridViewEditingControlShowingEventArgs e)
        {
            if (this.dgvMotherData.CurrentCell.ColumnIndex == 0)
            {
                ComboBox comboBox = e.Control as ComboBox;

                comboBox.SelectedIndexChanged += new EventHandler(comboBox_SelectedIndexChanged);
            }
        }

        private void dgvMotherData_DataError(object sender, DataGridViewDataErrorEventArgs e)
        {
            e.Cancel = true;
        }

        private void comboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            string subjectCode = (sender as ComboBox).Text;

            if (!dicSubjects.ContainsKey(subjectCode))
                return;

            dynamic oo = dicSubjects[subjectCode];
            this.dgvMotherData.CurrentRow.Cells["SubjectChineseName"].Value = oo.Name;
            this.dgvMotherData.CurrentRow.Cells["SubjectEnglishName"].Value = oo.EnglishName;
            this.dgvMotherData.CurrentRow.Cells["Credit"].Value = oo.Credit;
            this.dgvMotherData.CurrentRow.Cells["SubjectGroup"].Value = oo.SubjectGroup;
            this.dgvMotherData.CurrentRow.Cells["Prerequisites"].Value = oo.Prerequisites;

            //((ComboBox)sender).SelectedIndexChanged -= new EventHandler(comboBox_SelectedIndexChanged);
        }

        private void Save_Click(object sender, EventArgs e)
        {
            foreach (DataGridViewRow rows in this.dgvChildData.Rows)
            {
                if (rows.IsNewRow)
                    continue;

                foreach (DataGridViewCell cell in rows.Cells)
                {
                    if (cell.ErrorText != string.Empty)
                    {
                        MessageBox.Show("請先修正錯誤再儲存！");
                        return;
                    }
                }
            }        

            GraduationRequirement graduationRequirement = (GraduationRequirement)this.GraduationRequirementRuleName.Tag;
            
            if (graduationRequirement == null)
                return;

            graduationRequirement = SaveGraduationRequirement(graduationRequirement);
            this.GraduationRequirementRuleName.Tag = graduationRequirement;

            BindGraduationRequirementRule(graduationRequirement.DepartmentGroupID.ToString());

            SaveGraduationSubjectList(graduationRequirement);
            SaveGraduationSubjectGroupRequirement(graduationRequirement);

            //EMBACore.Initialization.StudentInit.RedrawGraduationRequirementMenuButton();
        }

        private GraduationRequirement SaveGraduationRequirement(GraduationRequirement graduationRequirement)
        {
            graduationRequirement.Name = this.GraduationRequirementRuleName.Text.Trim();
            graduationRequirement.DepartmentCredit = (String.IsNullOrWhiteSpace(this.DepartmentCredit.Text) ? 0 : Convert.ToInt32(this.DepartmentCredit.Text.Trim()));
            graduationRequirement.ElectiveCredit = (String.IsNullOrWhiteSpace(this.ElectiveCredit.Text) ? 0 : Convert.ToInt32(this.ElectiveCredit.Text.Trim()));
            graduationRequirement.RequiredCredit = (String.IsNullOrWhiteSpace(this.RequiredCredit.Text) ? 0 : Convert.ToInt32(this.RequiredCredit.Text.Trim()));

            graduationRequirement.Save();

            return graduationRequirement;
        }

        private void SaveGraduationSubjectList(GraduationRequirement graduationRequirement)
        {
            List<GraduationSubjectList> graduationSubjectLists = Access.Select<GraduationSubjectList>(string.Format("ref_graduation_requirement_id={0}", graduationRequirement.UID));

            graduationSubjectLists.ForEach(x => x.Deleted = true);
            graduationSubjectLists.SaveAll();

            List<GraduationSubjectList> newGraduationSubjectLists = new List<GraduationSubjectList>();

            foreach (DataGridViewRow dataGridViewRow in this.dgvMotherData.Rows)
            {
                if (dataGridViewRow.IsNewRow)
                    continue;

                GraduationSubjectList graduationSubjectList = new GraduationSubjectList();

                graduationSubjectList.GraduationRequirementID = int.Parse(graduationRequirement.UID);
                graduationSubjectList.SubjectID = int.Parse(dicSubjects[dataGridViewRow.Cells["SubjectCode"].Value.ToString()].UID);
                graduationSubjectList.SubjectGroup = (dataGridViewRow.Cells["SubjectGroup"].Value == null ? "" : dataGridViewRow.Cells["SubjectGroup"].Value.ToString());
                bool IsDeptRequired = false;
                bool.TryParse(dataGridViewRow.Cells["IsDeptRequired"].Value + "", out IsDeptRequired);
                graduationSubjectList.IsDeptRequired = IsDeptRequired;
                graduationSubjectList.Prerequisites = (dataGridViewRow.Cells["Prerequisites"].Value == null ? "" : dataGridViewRow.Cells["Prerequisites"].Value.ToString());

                newGraduationSubjectLists.Add(graduationSubjectList);
            }
            try
            {
                newGraduationSubjectLists.SaveAll();
                MessageBox.Show("儲存成功。");
            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void SaveGraduationSubjectGroupRequirement(GraduationRequirement graduationRequirement)
        {
            List<GraduationSubjectGroupRequirement> graduationSubjectGroupRequirements = Access.Select<GraduationSubjectGroupRequirement>(string.Format("ref_graduation_requirement_id={0}", graduationRequirement.UID));

            graduationSubjectGroupRequirements.ForEach(x => x.Deleted = true);
            graduationSubjectGroupRequirements.SaveAll();

            List<GraduationSubjectGroupRequirement> newGraduationSubjectGroupRequirements = new List<GraduationSubjectGroupRequirement>();

            foreach (DataGridViewRow dataGridViewRow in this.dgvChildData.Rows)
            {
                GraduationSubjectGroupRequirement graduationSubjectGroupRequirement = new GraduationSubjectGroupRequirement();

                graduationSubjectGroupRequirement.GraduationRequirementID = int.Parse(graduationRequirement.UID);

                int intLowestSubjectCount = 0;
                int.TryParse(dataGridViewRow.Cells["Subject_Count"].Value.ToString(), out intLowestSubjectCount);
                graduationSubjectGroupRequirement.LowestSubjectCount = intLowestSubjectCount;

                int intLowestCredit = 0;
                int.TryParse(dataGridViewRow.Cells["Credits"].Value.ToString(), out intLowestCredit);
                graduationSubjectGroupRequirement.LowestCredit = intLowestCredit;

                graduationSubjectGroupRequirement.SubjectGroup = dataGridViewRow.Cells["SubjectGroup_Enactment"].Value.ToString();

                newGraduationSubjectGroupRequirements.Add(graduationSubjectGroupRequirement);
            }
            newGraduationSubjectGroupRequirements.SaveAll();
        }

        private void dgvMotherData_RowsRemoved(object sender, DataGridViewRowsRemovedEventArgs e)
        {
            this.BindChildData();
        }

        private void dgvChildData_RowsRemoved(object sender, DataGridViewRowsRemovedEventArgs e)
        {
            IEnumerable<DataGridViewRow> rowsMother = this.dgvMotherData.Rows.Cast<DataGridViewRow>();
            IEnumerable<DataGridViewRow> rowsChild = this.dgvChildData.Rows.Cast<DataGridViewRow>();

            foreach (DataGridViewRow dataGridViewRow in rowsMother)
            {
                if (dataGridViewRow.Cells["SubjectGroup"].Value != null && !string.IsNullOrEmpty(dataGridViewRow.Cells["SubjectGroup"].Value.ToString()) && rowsChild.Where(x => x.Cells["SubjectGroup_Enactment"].Value.ToString() == dataGridViewRow.Cells["SubjectGroup"].Value.ToString()).Count() == 0)
                    dataGridViewRow.Cells["SubjectGroup"].Value = string.Empty;
            }
        }

        private void Delete_Click(object sender, EventArgs e)
        {
            System.Windows.Forms.ListBox lstBox = this.lstGraduationRequirementRule;
            if (lstBox.SelectedIndex == -1)
                return;

            string graduationRequirementRuleID = ((dynamic)lstBox.SelectedItem).UID;

            //  若已指定學生畢業條件，則提醒使用者是否刪除？
            List<StudentBrief2> student_Brief2s = Access.Select<StudentBrief2>(string.Format("ref_graduation_requirement_id = {0}", graduationRequirementRuleID));

            if (student_Brief2s.Count > 0)
            {
                if (MessageBox.Show("待刪除之畢業條件已指定學生，請先移除指定學生！", "提醒", MessageBoxButtons.OKCancel) == System.Windows.Forms.DialogResult.Cancel)
                    return;
                else
                {
                    this.tabControl1.SelectedTab = this.tabItem2;
                    return;
                }
            }

            //  刪除「畢業條件」
            GraduationRequirement graduationRequirement = ((dynamic)lstBox.SelectedItem).GraduationRequirement;
            graduationRequirement.Deleted = true;
            graduationRequirement.Save();

            //  刪除「畢業應修科目清單」
            List<GraduationSubjectList> graduationSubjectLists = Access.Select<GraduationSubjectList>(string.Format("ref_graduation_requirement_id={0}", graduationRequirementRuleID));

            if (graduationSubjectLists.Count > 0)
            {
                graduationSubjectLists.ForEach(x => x.Deleted = true);
                graduationSubjectLists.SaveAll();
            }

            //  刪除「畢業應修科目群組」
            List<GraduationSubjectGroupRequirement> graduationSubjectGroupRequirement = Access.Select<GraduationSubjectGroupRequirement>(string.Format("ref_graduation_requirement_id={0}", graduationRequirementRuleID));

            if (graduationSubjectGroupRequirement.Count > 0)
            {
                graduationSubjectGroupRequirement.ForEach(x => x.Deleted = true);
                graduationSubjectGroupRequirement.SaveAll();
            }

            //  重整「畢業條件」清單
            ComboItem item = (ComboItem)this.cboDepartmentGroup.SelectedItem;

            if (item.Tag == null)
                BindGraduationRequirementRule(string.Empty);
            else
            {
                DepartmentGroup departmentGroup = (item.Tag as DepartmentGroup);
                BindGraduationRequirementRule(departmentGroup.UID);
            }

            //  清空右側大區塊
            ClearRightBigBlock();
            //  重繪「指定畢業條件」選單
            //EMBACore.Initialization.StudentInit.RedrawGraduationRequirementMenuButton();
        }

        private void ClearRightBigBlock()
        {
            this.GraduationRequirementRuleName.Text = string.Empty;
            this.DepartmentCredit.Text = string.Empty;
            this.ElectiveCredit.Text = string.Empty;
            this.RequiredCredit.Text = string.Empty;
            this.dgvMotherData.Rows.Clear();
            this.dgvChildData.Rows.Clear();
        }

        private void dgvData_RowsRemoved(object sender, DataGridViewRowsRemovedEventArgs e)
        {
            if (notDelete)
                return;

            if (assignedGraduationRequirementStudent.Count == 0)
                return;

            List<string> studentIDs = new List<string>();

            foreach (DataGridViewRow dataGridViewRow in assignedGraduationRequirementStudent)
            {
                if (!this.dgvData.Rows.Cast<DataGridViewRow>().Contains(dataGridViewRow))
                    studentIDs.Add(dataGridViewRow.Cells[0].Value.ToString());
            }

            if (studentIDs.Count() == 0)
                return;

            try
            {
                IEnumerable<StudentBrief2> delete_StudentBrief2 = Access.Select<StudentBrief2>(string.Format("ref_student_id in {0}", "(" + string.Join(",", studentIDs)) + ")");

                delete_StudentBrief2.ToList().ForEach(x => x.GraduationRequirementID = 0);
                delete_StudentBrief2.SaveAll();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            finally
            {
                //notDelete = false;
            }
        }
        //    private void CalcSalaryTemplate_SelectedIndexChanged(object sender, EventArgs e)
        //    {
        //        ListBox lsBox = sender as ListBox;

        //        if (lsBox.SelectedIndex == -1)
        //            return;

        //        this.TemplateName.Text = ((dynamic)lsBox.SelectedItem).Item;
        //        this.TemplateName.Tag = lsBox.Items[lsBox.SelectedIndex];

        //        RefreshTIContain();
        //        RefreshTSContain(((dynamic)lsBox.SelectedItem).UID);
        //    }

        //    private void RefreshTSContain(string salaryCalculationTemplateID)
        //    {
        //        this.lstView.Items.Clear();

        //        List<TSContainRecord> tSContainRecords = UDTProvider.GetTSContainRecordsByTemplateID(salaryCalculationTemplateID);

        //        if (tSContainRecords == null || tSContainRecords.Count == 0)
        //            return;

        //        List<K12.Data.TeacherRecord> teacherRecords = K12.Data.Teacher.SelectByIDs(tSContainRecords.Select(x => x.RefTeacherID));
        //        ImageList images = new ImageList();
        //        //this.lstView.View = View.Tile;
        //        this.lstView.LargeImageList = images;

        //        foreach (K12.Data.TeacherRecord teacherRecord in teacherRecords)
        //        {
        //            MemoryStream stream = new MemoryStream(Convert.FromBase64String(teacherRecord.Photo));

        //            ListViewItem item = new ListViewItem();

        //            item.Text = teacherRecord.Name;
        //            //if (teacherRecord.Photo.Length > 0)
        //            //{
        //                images.Images.Add(teacherRecord.ID, Properties.Resources.林志玲);
        //                item.ImageKey = teacherRecord.ID;
        //            //}
        //            this.lstView.Items.Add(item);
        //        }
        //    }

        //    private void RefreshTIContain()
        //    {
        //        this.dgvData.Rows.Clear();
        //        _dicUTDs.Clear();
        //        string salaryCalculationTemplateID = ((dynamic)this.TemplateName.Tag).UID;

        //        List<TIContainRecord> ticrs = UDTProvider.GetTIContainRecords(salaryCalculationTemplateID); 
        //        List<SalaryItemRecord> udtRecords = UDTProvider.GetSalaryItemRecords();

        //        foreach (TIContainRecord ticr in ticrs)
        //        {
        //            List<object> list = new List<object>();

        //            list.Add(ticr.ItemType);
        //            list.Add(null);
        //            list.Add(ticr.IsCalculated);
        //            list.Add(ticr.IsIncomeTax);
        //            list.Add(ticr.TaxRate);
        //            list.Add(ticr.Memo);

        //            int idx = this.dgvData.Rows.Add(list.ToArray());
        //            this.dgvData.Rows[idx].Tag = ticr;

        //            _dicUTDs.Add(ticr.UID, ticr.Clone());

        //            DataGridViewComboBoxCell dataGridViewComboBoxCell = (this.dgvData.Rows[idx].Cells[1] as DataGridViewComboBoxCell);

        //            if (udtRecords.Count > 0 && udtRecords.Where(x => x.ItemType == ticr.ItemType).Count() > 0)
        //            {
        //                IEnumerable<string> items = udtRecords.Where(x => x.ItemType == ticr.ItemType).Select(x => x.Item);
        //                for (int i = 0; i < items.Count(); i++)
        //                {
        //                    dataGridViewComboBoxCell.Items.Add(items.ElementAt(i));
        //                }
        //                dataGridViewComboBoxCell.Value = ticr.Item;
        //            }
        //        }
        //        this.dgvData.AutoResizeColumns();
        //        this.dgvData.CurrentCell = null;
        //    }

        //    private void Save_Click(object sender, EventArgs e)
        //    {
        //        string oName = this.TemplateName.Text.Trim();   // ((dynamic)this.TemplateName.Tag).Item;
        //        string salaryCalculationTemplateID = ((dynamic)this.TemplateName.Tag).UID;

        //        List<SalaryCalculationTemplateRecord> sctr = UDTProvider.GetSalaryCalculationTemplateRecords(salaryCalculationTemplateID);

        //        //  儲存薪資計算樣版名稱
        //        sctr[0].Item = oName;
        //        sctr.SaveAll();

        //        this.RefreshCalcSalaryTemplate();

        //        //  儲存薪資計算樣版所包含之薪資項目及其計稅規則
        //        List<TIContainRecord> ticrs = UDTProvider.GetTIContainRecords(salaryCalculationTemplateID);

        //        //  待刪除資料
        //        Dictionary<string, TIContainRecord> deleteRecords = new Dictionary<string, TIContainRecord>();
        //        //  待更新資料
        //        Dictionary<string, TIContainRecord> updateRecords = new Dictionary<string, TIContainRecord>();
        //        //  待新增資料
        //        List<TIContainRecord> insertRecords = new List<TIContainRecord>();

        //        foreach (DataGridViewRow dataGridRow in this.dgvData.Rows)
        //        {
        //            if (dataGridRow.IsNewRow)
        //                continue;

        //            TIContainRecord gridRecord = (TIContainRecord)dataGridRow.Tag;

        //            if (gridRecord == null)
        //                gridRecord = new TIContainRecord();

        //            gridRecord.SalaryCalculationTemplateID = salaryCalculationTemplateID;
        //            gridRecord.ItemType = (dataGridRow.Cells["ItemType"].Value == null ? "" : dataGridRow.Cells["ItemType"].Value.ToString());
        //            gridRecord.Item = (dataGridRow.Cells["Item"].Value == null ? "" : dataGridRow.Cells["Item"].Value.ToString());
        //            gridRecord.IsCalculated = (dataGridRow.Cells["IsCalculated"].Value == null ? false : (bool)dataGridRow.Cells["IsCalculated"].Value);
        //            gridRecord.IsIncomeTax = (dataGridRow.Cells["IsIncomeTax"].Value == null ? false : (bool)dataGridRow.Cells["IsIncomeTax"].Value);
        //            gridRecord.TaxRate = (dataGridRow.Cells["TaxRate"].Value == null ? "" : dataGridRow.Cells["TaxRate"].Value.ToString());
        //            gridRecord.Memo = (dataGridRow.Cells["Memo"].Value == null ? "" : dataGridRow.Cells["Memo"].Value.ToString());

        //            if (gridRecord.RecordStatus == FISCA.UDT.RecordStatus.Insert)
        //                insertRecords.Add(gridRecord);
        //            else if (gridRecord.RecordStatus == FISCA.UDT.RecordStatus.Update)
        //                updateRecords.Add(gridRecord.UID, gridRecord);
        //        }

        //        IEnumerable<DataGridViewRow> dataGridViewRows = this.dgvData.Rows.Cast<DataGridViewRow>();
        //        foreach (string key in _dicUTDs.Keys)
        //        {
        //            if (dataGridViewRows.Where(x => x.Tag != null).Where(x => ((x.Tag as TIContainRecord).UID == key)).Count() == 0)
        //            {
        //                TIContainRecord gridRecord = _dicUTDs[key];
        //                gridRecord.Deleted = true;
        //                deleteRecords.Add(key, gridRecord);
        //            }
        //        }

        //        List<string> insertedRecordUIDs = insertRecords.SaveAll();                    //  匯入「新增」資料 
        //        //List<BonusFeeRecord> insertedRecords = UDTProvider.GetBonusFeeRecords(insertedRecordUIDs);
        //        //if (insertedRecordUIDs != null && insertedRecordUIDs.Count > 0)
        //        //{
        //        //    //  寫入「新增」的 Log
        //        //    Dictionary<string, BonusFeeRecord> dicInsertedRecords = insertedRecords.ToDictionary(x => x.UID);
        //        //    foreach (string iRecords in dicInsertedRecords.Keys)
        //        //    {
        //        //        BonusFeeRecord insertedBonusFeeRecord = dicInsertedRecords[iRecords];

        //        //        StringBuilder sb = new StringBuilder();

        //        //        sb.Append("教師「" + teacherRecord.Name + "」，暱稱「" + teacherRecord.Nickname + "」");
        //        //        sb.AppendLine("被新增一筆「獎金、補助、代收」資料。");
        //        //        sb.AppendLine("詳細資料：");
        //        //        sb.Append("項目名稱「" + insertedBonusFeeRecord.Item + "」\n");
        //        //        sb.Append("金額「" + insertedBonusFeeRecord.Money + "」\n");
        //        //        sb.Append("備註「" + insertedBonusFeeRecord.Memo + "」\n");

        //        //        ApplicationLog.Log("教師.資料項目.獎金、補助、代收.新增", "教師.資料項目.獎金、補助、代收.新增", "UDT.BonusFeeRecord", iRecords, sb.ToString());
        //        //    }
        //        //}

        //        List<string> updatedRecords = updateRecords.Values.SaveAll();   //  匯入「更新」資料 
        //        //List<BonusFeeRecord> updatedRecordss = UDTProvider.GetBonusFeeRecords(updatedRecords);

        //        //if (updatedRecordss != null && updatedRecordss.Count > 0)
        //        //{
        //        //    //  批次寫入「修改」的 Log
        //        //    foreach (BonusFeeRecord uRecords in updatedRecordss)
        //        //    {
        //        //        BonusFeeRecord newBonusFeeRecord = uRecords;
        //        //        BonusFeeRecord oldBonusFeeRecord = _dicUTDs[uRecords.UID];

        //        //        StringBuilder sb = new StringBuilder();

        //        //        sb.Append("教師「" + teacherRecord.Name + "」，暱稱「" + teacherRecord.Nickname + "」");
        //        //        sb.AppendLine("被修改一筆「獎金、補助、代收」資料。");
        //        //        sb.AppendLine("詳細資料：");

        //        //        if (!oldBonusFeeRecord.Item.Equals(newBonusFeeRecord.Item))
        //        //            sb.Append("項目名稱由「" + oldBonusFeeRecord.Item + "」改為「" + newBonusFeeRecord.Item + "」\n");

        //        //        if (!oldBonusFeeRecord.Money.Equals(newBonusFeeRecord.Money))
        //        //            sb.Append("金額由「" + oldBonusFeeRecord.Money + "」改為「" + newBonusFeeRecord.Money + "」\n");

        //        //        if (!oldBonusFeeRecord.Memo.Equals(newBonusFeeRecord.Memo))
        //        //            sb.Append("備註由「" + oldBonusFeeRecord.Memo + "」改為「" + newBonusFeeRecord.Memo + "」\n");

        //        //        ApplicationLog.Log("教師.資料項目.獎金、補助、代收.修改", "教師.資料項目.獎金、補助、代收.修改", "UDT.BonusFeeRecord", uRecords.UID, sb.ToString());
        //        //    }


        //        //  刪除資料
        //        List<string> deletedRecords = deleteRecords.Values.SaveAll();
        //        //if (deletedRecords != null && deletedRecords.Count > 0)
        //        //{
        //        //    //  寫入「刪除」的 Log
        //        //    foreach (string key in deletedRecords)
        //        //    {
        //        //        BonusFeeRecord newBonusFeeRecord = _dicUTDs[key];

        //        //        StringBuilder sb = new StringBuilder();

        //        //        sb.Append("教師「" + teacherRecord.Name + "」，暱稱「" + teacherRecord.Nickname + "」");
        //        //        sb.AppendLine("被刪除一筆「獎金、補助、代收」資料。");
        //        //        sb.AppendLine("詳細資料：");
        //        //        sb.Append("項目名稱「" + newBonusFeeRecord.Item + "」\n");
        //        //        sb.Append("金額「" + newBonusFeeRecord.Money + "」\n");
        //        //        sb.Append("備註「" + newBonusFeeRecord.Memo + "」\n");

        //        //        ApplicationLog.Log("教師.資料項目.獎金、補助、代收.刪除", "教師.資料項目.獎金、補助、代收.刪除", "UDT.BonusFeeRecord", key, sb.ToString());
        //        //    }
        //        //}
        //        this.RefreshTIContain();
        //    }

        //    private void Add_Click(object sender, EventArgs e)
        //    {
        //        string result = InputForm.InputBox("請輸入薪資計算樣版名稱：", "薪資計算樣版管理", "") as String;
        //        List<SalaryCalculationTemplateRecord> sctrs = UDTProvider.GetSalaryCalculationTemplateRecords();

        //        if (sctrs.Count > 0 && sctrs.Where(x => x.Item == result.Trim()).Count() > 1)
        //        {
        //            MessageBox.Show("同名樣版已存在！");
        //            return;
        //        }

        //        SalaryCalculationTemplateRecord sctr = new SalaryCalculationTemplateRecord();
        //        sctr.Item = result.Trim();
        //        sctr.Save();

        //        RefreshCalcSalaryTemplate();
        //    }

        //    private void RefreshCalcSalaryTemplate()
        //    {
        //        this.CalcSalaryTemplate.Items.Clear();

        //        List<SalaryCalculationTemplateRecord> sctrs = UDTProvider.GetSalaryCalculationTemplateRecords();
        //        foreach (SalaryCalculationTemplateRecord sctr in sctrs)
        //        {
        //            this.CalcSalaryTemplate.Items.Add(new { sctr.UID, sctr.Item });
        //        }
        //    }

        //    private void Delete_Click(object sender, EventArgs e)
        //    {
        //        this.CalcSalaryTemplate.Items.RemoveAt(this.CalcSalaryTemplate.SelectedIndex);
        //    }
    }
}
