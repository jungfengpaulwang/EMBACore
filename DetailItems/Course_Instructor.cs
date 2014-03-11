using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Campus.Windows;
using FISCA.Data;
using EMBACore.DataItems;
using K12.Data;
using SHSchool.Data;
using FISCA.UDT;
using FISCA.Permission;
using FISCA.Presentation;

namespace EMBACore.DetailItems
{
    [AccessControl("ischool.EMBA.Course_Instructor", "教授 / 助教 / 助理", "課程>資料項目")]
    public partial class Course_Instructor : DetailContentImproved
    {
        private Dictionary<string, TeacherItem> dicTeachers;  //<teacherID, TeacherItem>
        private List<TagItem> tags;    //<tagID, tagName>
        private Dictionary<string, TagItem> dicTags;
        private Dictionary<string, List<string>> teacher_tags;   //<tagID, List<teacherID>>

        private List<UDT.CourseInstructor> records = new List<UDT.CourseInstructor>();
        /*   Log  */
        private Log.LogAgent logAgent = new Log.LogAgent();
        public Course_Instructor()
        {
            InitializeComponent();
            this.Group = "教師 / 助教 / 助理";            
        }

        private void Course_Instructor_Load(object sender, EventArgs e)
        {
            this.WatchChange(new DataGridViewSource(this.dataGridViewX1));

            TeacherTag.AfterInsert += new EventHandler<K12.Data.DataChangedEventArgs>(TeacherTag_AfterChange);
            TeacherTag.AfterUpdate += new EventHandler<K12.Data.DataChangedEventArgs>(TeacherTag_AfterChange);
            TeacherTag.AfterDelete += new EventHandler<K12.Data.DataChangedEventArgs>(TeacherTag_AfterChange);
            Disposed += new EventHandler(TeacherTag_Disposed);
            this.dataGridViewX1.DataError += new DataGridViewDataErrorEventHandler(dataGridViewX1_DataError);
            //this.toolTip1.SetToolTip(this.label1, "重新載入教師標籤！");
        }

        private void dataGridViewX1_DataError(object sender, DataGridViewDataErrorEventArgs e)
        {
            e.Cancel = true;
        }

        private void TeacherTag_AfterChange(object sender, EventArgs e)
        {
            this.LoadTags();
            this.LoadTeacherTags();
            this.LoadInstructorList();

            this.colTeachers.Items.Clear();
            this.colTeachers.DisplayMember = "Name";
            this.colTeachers.ValueMember = "ID";
            foreach (TeacherItem ti in this.dicTeachers.Values)
                this.colTeachers.Items.Add(ti);
        }

        void TeacherTag_Disposed(object sender, EventArgs e)
        {
            TeacherTag.AfterInsert -= new EventHandler<K12.Data.DataChangedEventArgs>(TeacherTag_AfterChange);
            TeacherTag.AfterUpdate -= new EventHandler<K12.Data.DataChangedEventArgs>(TeacherTag_AfterChange);
            TeacherTag.AfterDelete -= new EventHandler<K12.Data.DataChangedEventArgs>(TeacherTag_AfterChange);
        }

        //載入所有教師清單
        private void LoadInstructorList()
        {
            QueryHelper query = new QueryHelper();
            string cmd = @"select t.id , t.teacher_name, t.status
                                        from teacher t order by t.status , t.teacher_name asc";

            this.dicTeachers = new Dictionary<string, TeacherItem>();
            DataTable dt = query.Select(cmd);
            foreach (DataRow row in dt.Rows)
            {
                TeacherItem item = new TeacherItem(row["id"].ToString(), row["teacher_name"].ToString(), row["status"].ToString());
                this.dicTeachers.Add(item.ID , item);
            }
        }

        //取得 tag 清單
        private void LoadTags()
        {
            QueryHelper query = new QueryHelper();
            string cmd = @"SELECT  id, prefix, name from tag where category='Teacher' and (prefix='教師' or prefix='助教' or prefix='助理') order by prefix , name desc";
            this.tags = new List<TagItem>();
            this.dicTags = new Dictionary<string, TagItem>();
            
            DataTable dt = query.Select(cmd);
            foreach (DataRow row in dt.Rows)
            {
                TagItem ti = new TagItem(row["id"].ToString(),row["prefix"].ToString(), (row.IsNull("name") ? "" : row["name"].ToString()));
                this.tags.Add(ti);
                this.dicTags.Add(ti.ID, ti);
            }
        }

        //取得教師與 tag 的關聯
        private void LoadTeacherTags()
        {
            QueryHelper query = new QueryHelper();
            string cmd = @"SELECT ref_teacher_id, ref_tag_id from tag_teacher ";
            this.teacher_tags = new Dictionary<string, List<string>>();

            DataTable dt = query.Select(cmd);
            foreach (DataRow row in dt.Rows)
            {
                string tagID = row["ref_tag_id"].ToString();
                if (!this.teacher_tags.ContainsKey(tagID)) 
                    this.teacher_tags.Add(tagID, new List<string>());

                string teacherID = row["ref_teacher_id"].ToString();
                this.teacher_tags[tagID].Add(teacherID);
            }
        }

        //取得具有該TagID 的所有教師
        private List<TeacherItem> GetTeachersByTagID(string tagID)
        {
            List<TeacherItem> result = new List<TeacherItem>();
            if (this.teacher_tags.ContainsKey(tagID))
            {
                foreach (string teacherID in this.teacher_tags[tagID])
                {
                    if (this.dicTeachers.ContainsKey(teacherID))
                        result.Add(this.dicTeachers[teacherID]);
                }
            }
            //sort by name
            result.Sort(delegate(TeacherItem p1, TeacherItem p2)
            {
                return p1.Name.CompareTo(p2.Name);
            });

            return result;
        }

        //載入課程教師/助教
        private void LoadCourseInstructors()
        {
            AccessHelper ah = new AccessHelper();
            this.records = ah.Select<UDT.CourseInstructor>(string.Format("ref_course_id='{0}'", this.PrimaryKey));
        }

        protected override void OnInitializeAsync()
        {
            this.LoadTags();
            this.LoadTeacherTags();
            this.LoadInstructorList();
        }

        protected override void OnInitializeComplete(Exception error)
        {
            if (error != null) //有錯就直接丟出去吧。
                throw error;

            this.colRole.DataSource = this.tags;
            this.colRole.DisplayMember = "TagFullName";
            this.colRole.ValueMember = "ID";


            this.colTeachers.Items.Clear();            
            this.colTeachers.DisplayMember = "Name";
            this.colTeachers.ValueMember = "ID";
            foreach (TeacherItem ti in this.dicTeachers.Values)
                this.colTeachers.Items.Add(ti);
           
        }

        protected override void OnPrimaryKeyChangedAsync()
        {
            LoadCourseInstructors();
        }

        protected override void OnPrimaryKeyChangedComplete(Exception error)
        {
            if (error != null) //有錯就直接丟出去吧。
                throw error;

            ErrorTip.Clear();

            this.dataGridViewX1.Rows.Clear();

            foreach (UDT.CourseInstructor tea in this.records)
            {
                object[] rawdata = new object[] { tea.TagID.ToString(), null, tea.IsScored };
                int rowIndex = this.dataGridViewX1.Rows.Add(rawdata);   //加入一筆 Record
                this.dataGridViewX1.Rows[rowIndex].Tag = tea;
                if (this.dicTags.ContainsKey(tea.TagID.ToString()))
                    this.dataGridViewX1.Rows[rowIndex].Cells[0].Tag = this.dicTags[tea.TagID.ToString()].Prefix;
                else
                    this.dataGridViewX1.Rows[rowIndex].Cells[0].Tag = string.Empty;

                //建立 Combobox Items :
                DataGridViewComboBoxCell cboCell = (this.dataGridViewX1.Rows[rowIndex].Cells[1] as DataGridViewComboBoxCell);
                if (this.teacher_tags.ContainsKey(tea.TagID.ToString() ))
                {
                    bool isTeacherInItems = false;  //tea.TeacherID 是否在清單中
                    foreach (TeacherItem ti in this.GetTeachersByTagID(tea.TagID.ToString()))
                    {
                        cboCell.Items.Add(ti);

                        if (ti.ID == tea.TeacherID.ToString())
                            isTeacherInItems = true;
                    }
                    if (isTeacherInItems)
                        cboCell.Value = tea.TeacherID.ToString();
                }
            }

            this.dataGridViewX1.CurrentCell = null;
            this.logAgent.Clear();
            ResetDirtyStatus();
        }

        protected override void OnValidateData(Dictionary<Control, string> errors)
        {
            return;
            //確定使用者修改的值都更新到控制項裡了(預防點選checkbox 後直接點選儲存，這時抓到的值仍是前一個值)。
            this.dataGridViewX1.EndEdit();

            foreach (DataGridViewRow row in this.dataGridViewX1.Rows)
            {
                if (!row.IsNewRow)
                {
                    DataGridViewCell cell = row.Cells[0];
                    if (cell.Value == null)
                    {
                        cell.ErrorText = "請選擇角色 ！";
                        this.dataGridViewX1.UpdateCellErrorText(cell.ColumnIndex, cell.RowIndex);
                        errors.Add(this.dataGridViewX1, "請選擇角色！");                        
                    }
                    else
                    {
                        cell.ErrorText = "";
                        this.dataGridViewX1.UpdateCellErrorText(cell.ColumnIndex, cell.RowIndex);
                    }

                    cell = row.Cells[1];
                    if (cell.Value == null)
                    {
                        cell.ErrorText = "請選擇授課教師 ！";
                        this.dataGridViewX1.UpdateCellErrorText(cell.ColumnIndex, cell.RowIndex);
                        errors.Add(this.dataGridViewX1, "請選擇授課教師！");
                    }
                    else
                    {
                        cell.ErrorText = "";
                        this.dataGridViewX1.UpdateCellErrorText(cell.ColumnIndex, cell.RowIndex);
                    }
                }
            }
        }

        private void dataGridViewX1_UserDeletingRow(object sender, DataGridViewRowCancelEventArgs e)
        {
            if (e.Row.IsNewRow) return;

            if (MessageBox.Show("您確定要移除這位老師？", "注意", MessageBoxButtons.YesNo, MessageBoxIcon.Information) != DialogResult.Yes)
            {
                e.Cancel = true;
                return;
            }
        }

        protected override void OnDirtyStatusChanged(ChangeEventArgs e)
        {
            if (UserAcl.Current[this.GetType()].Editable)
                SaveButtonVisible = e.Status == ValueStatus.Dirty;
            else
                this.SaveButtonVisible = false;

            CancelButtonVisible = e.Status == ValueStatus.Dirty;
        }

        protected override void OnCancelButtonClick(EventArgs e)
        {
            this.OnPrimaryKeyChanged(EventArgs.Empty);
        }

        private bool is_validated()
        {
            bool is_valid = true;

            foreach (DataGridViewRow row in this.dataGridViewX1.Rows)
            {
                if (row.IsNewRow)
                    continue;

                row.Cells[0].ErrorText = string.Empty;
                row.Cells[1].ErrorText = string.Empty;

                if (string.IsNullOrEmpty(row.Cells[0].Value + ""))
                {
                    is_valid = false;
                    row.Cells[0].ErrorText = "必填。";
                }

                if (string.IsNullOrEmpty(row.Cells[1].Value + ""))
                {
                    is_valid = false;
                    row.Cells[1].ErrorText = "必填。";
                }

                if (!is_valid)
                    continue;

                this.dataGridViewX1.Rows.Cast<DataGridViewRow>().ToList().ForEach((x) =>
                {
                    if (!string.IsNullOrEmpty(x.Cells[0].Value + "") && !string.IsNullOrEmpty(x.Cells[1].Value + ""))
                    {
                        if (x.Index != row.Index && (x.Cells[0].Value + "" + x.Cells[1].Value) == (row.Cells[0].Value + "" + row.Cells[1].Value))
                        {
                            row.Cells[0].ErrorText = "「角色」與「姓名」的組合重複。";
                            row.Cells[1].ErrorText = "「角色」與「姓名」的組合重複。";
                            x.Cells[0].ErrorText = "「角色」與「姓名」的組合重複。";
                            x.Cells[1].ErrorText = "「角色」與「姓名」的組合重複。";

                            is_valid = false;
                        }
                    }
                });
            }

            return is_valid;
        }

        protected override void OnSaveData()
        {
            if (!this.is_validated())
            {
                MessageBox.Show("請先修正錯誤再儲存。");
                return;
            }
            //Delete All Records
            List<ActiveRecord> deleteRecs = new List<ActiveRecord>();
            foreach (UDT.CourseInstructor ci in this.records)
            {                
                deleteRecs.Add(ci);
            }
            
            //Save new Record
            List<ActiveRecord> insertRecs = new List<ActiveRecord>();
                        
            foreach (DataGridViewRow row in this.dataGridViewX1.Rows)
            {
                if (!row.IsNewRow)
                {
                    UDT.CourseInstructor ins = new UDT.CourseInstructor();
                    //SHSchool.Data.SHTCInstructRecord tcr = new SHTCInstructRecord();
                    ins.CourseID = int.Parse(this.PrimaryKey);
                    ins.TeacherID = int.Parse(row.Cells[1].Value.ToString());
                    ins.TagID = int.Parse(row.Cells[0].Value.ToString());
                    //ins.Role = row.Cells[1].Value.ToString(); //此欄位已不再使用。2012/3/16, kevin.
                    ins.IsScored = (row.Cells["IsScored"].Value != null && (bool)row.Cells["IsScored"].Value);
                    /*
                    if (row.Cells["IsScored"].Value != null && (bool)row.Cells["IsScored"].Value)
                    {
                        // item is checked
                        ins.IsScored = true;
                    }
                    else 
                    {
                        // item is null
                        ins.IsScored = false;
                    }
                    */
                    insertRecs.Add(ins);
                }
            }

            AccessHelper ah = new AccessHelper();
            //delete
            //if (this.deletedList.Count > 0)
            ah.DeletedValues(deleteRecs);             
            
            //insert
            if (insertRecs.Count > 0)
                ah.SaveAll(insertRecs);

            this.OnPrimaryKeyChanged(EventArgs.Empty);

            ResetDirtyStatus();

            /* ====  Log  for deleted records =====*/
            /*
            foreach (UDT.CourseInstructor ci in this.records)
            {
                Log.LogAgent agt = new Log.LogAgent();
                agt.ActionType = Log.LogActionType.Delete;
                this.AddLog(ci, agt);
               agt.Save("授課教師.課程", "刪除","", Log.LogTargetCategory.Course, ci.CourseID.ToString());
            }
             * */
            /* ====  Log  for inserted records =====*/
            K12.Data.CourseRecord course = K12.Data.Course.SelectByID(this.PrimaryKey);
            StringBuilder sb = new StringBuilder(string.Format("課程「{0}」，學年度「{1}」，學期「{2}」 \n", course.Name, course.SchoolYear, EMBACore.DataItems.SemesterItem.GetSemesterByCode(course.Semester + "").Name));
            sb.Append("授課教師更改為： \n");
            foreach (UDT.CourseInstructor ci in insertRecs)
            {
                string teacherName = this.dicTeachers[ci.TeacherID.ToString()].Name;
                string role = this.dicTags[ci.TagID.ToString()].TagFullName;
                sb.Append(string.Format("教師：{0} , 角色：{1} , 成績管理：{2} \n ", teacherName, role, ci.IsScored ? "是" : "否"));

                //Log.LogAgent agt = new Log.LogAgent();
                //agt.ActionType = Log.LogActionType.AddNew;
                //this.AddLog(ci, agt);
                //agt.Save("授課教師.課程", "新增", "", Log.LogTargetCategory.Course, ci.CourseID.ToString());
            }

            FISCA.LogAgent.ApplicationLog.Log("課程.資料項目.教授 / 助教 / 助理", "修改", "course", this.PrimaryKey, sb.ToString());
        }
      
        private void dataGridViewX1_CurrentCellDirtyStateChanged(object sender, EventArgs e)
        {            
            this.dataGridViewX1.CommitEdit(DataGridViewDataErrorContexts.Commit);            

            //確保只有一筆 Record 的值為 True
            DataGridViewCell cell = this.dataGridViewX1.CurrentCell;
            if (cell.ColumnIndex == 0)
            {
                if (this.dicTags.ContainsKey(cell.Value + ""))
                    this.dataGridViewX1.Rows[cell.RowIndex].Cells[0].Tag = this.dicTags[cell.Value + ""].Prefix;
            }
            string role = this.dataGridViewX1.Rows[cell.RowIndex].Cells[0].Tag + "";
            if (cell.ColumnIndex == 2)
            {
                if ((bool)cell.Value)
                {
                    foreach (DataGridViewRow row in this.dataGridViewX1.Rows)
                    {
                        if (!row.IsNewRow && row.Index != cell.RowIndex && role == (row.Cells[0].Tag + ""))
                        {
                            row.Cells[2].Value = false;
                        }
                    }
                }
            }
        }

        private void AddLog(UDT.CourseInstructor obj, Log.LogAgent agt)
        {
            if (agt != null)
            {
                agt.SetLogValue("課程编號", obj.CourseID.ToString());
                agt.SetLogValue("教師編號", obj.TeacherID.ToString());
                agt.SetLogValue("角色", obj.Role.ToString());
            }
        }

        //When the cell got focus ...
        private void dataGridViewX1_CellEnter(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0 && e.ColumnIndex >= 0 && dataGridViewX1.SelectedCells.Count == 1)
            {
                dataGridViewX1.BeginEdit(true);  //Raise EditingControlShowing Event !
                if (dataGridViewX1.CurrentCell != null && dataGridViewX1.CurrentCell.GetType().ToString() == "System.Windows.Forms.DataGridViewComboBoxCell")
                    (dataGridViewX1.EditingControl as ComboBox).DroppedDown = true;  //自動拉下清單
            }
        }

        private void dataGridViewX1_EditingControlShowing(object sender, DataGridViewEditingControlShowingEventArgs e)
        {
            //MessageBox.Show("Editing Control Showing!");
            if (this.dataGridViewX1.CurrentCell.ColumnIndex == 0)
            {
                ComboBox comboBox = e.Control as ComboBox;
                comboBox.SelectedIndexChanged += new EventHandler(comboBox_SelectedIndexChanged);
            }
        }

        private void comboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            //MessageBox.Show("comboBox_SelectedIndexChanged!");

            dataGridViewX1.CurrentRow.Cells[1].Value = null;
            DataGridViewComboBoxCell comboBoxTarget = dataGridViewX1.CurrentRow.Cells[1] as DataGridViewComboBoxCell;
            comboBoxTarget.Items.Clear();

            TagItem tg = ((ComboBox)sender).SelectedItem as TagItem;

            if (tg != null)
            {
                List<TeacherItem> udtRecords = null;
                if (this.teacher_tags.ContainsKey(tg.ID))
                    udtRecords = this.GetTeachersByTagID(tg.ID);
                else
                    udtRecords = new List<TeacherItem>();


                if (udtRecords == null || udtRecords.Count == 0)
                    return;

                foreach (TeacherItem teaItem in udtRecords)
                {
                    comboBoxTarget.Items.Add(teaItem);
                }
            }

            ((ComboBox)sender).SelectedIndexChanged -= new EventHandler(comboBox_SelectedIndexChanged);
        }

        private void label1_MouseEnter(object sender, EventArgs e)
        {
            this.label1.BackColor = Color.LightGreen;
            this.Cursor = Cursors.Hand;
        }

        private void label1_MouseLeave(object sender, EventArgs e)
        {
            this.label1.BackColor = Color.Transparent;
            this.Cursor = Cursors.Default;
        }

        private void label1_Click(object sender, EventArgs e)
        {            
            LoadTeacherTags();  //重新載入教師和標籤
        }
    }

}
