using System;
using System.Collections.Generic;
using FISCA.Presentation.Controls;
using FISCA.UDT;
using System.Windows.Forms;
using System.Linq;

namespace EMBACore.Forms
{
    public partial class CourseTypeDataSourceManagement : BaseForm
    {
        private AccessHelper Access;
        private bool _ShowDialog;

        //private List<UDT.ExperienceDataSource> oExperienceDataSources;

        public CourseTypeDataSourceManagement(bool showDialog = false)
        {
            InitializeComponent();
            Access = new AccessHelper();
            this._ShowDialog = showDialog;
            //oExperienceDataSources = new List<UDT.ExperienceDataSource>();

            this.Load += new EventHandler(Form_Load);

            //  DataGridView 事件
            this.dgvData.DataError += new DataGridViewDataErrorEventHandler(dgvData_DataError);
            this.dgvData.CurrentCellDirtyStateChanged += new EventHandler(dgvData_CurrentCellDirtyStateChanged);
            this.dgvData.CellEnter += new DataGridViewCellEventHandler(dgvData_CellEnter);
            this.dgvData.ColumnHeaderMouseClick += new System.Windows.Forms.DataGridViewCellMouseEventHandler(this.dgvData_ColumnHeaderMouseClick);
            this.dgvData.RowHeaderMouseClick += new System.Windows.Forms.DataGridViewCellMouseEventHandler(this.dgvData_RowHeaderMouseClick);
            this.dgvData.MouseClick += new System.Windows.Forms.MouseEventHandler(this.dgvData_MouseClick);
        }

        private void Form_Load(object sender, EventArgs e)
        {
            this.Text = "課程類別管理";
            this.InitExperienceDataSouce();
        }

        private void dgvData_DataError(object sender, DataGridViewDataErrorEventArgs e)
        {
            e.Cancel = true;
        }

        private void dgvData_CurrentCellDirtyStateChanged(object sender, EventArgs e)
        {
            dgvData.CommitEdit(DataGridViewDataErrorContexts.Commit);
        }

        private void dgvData_CellEnter(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0 && e.ColumnIndex >= 0 && dgvData.SelectedCells.Count == 1)
            {
                dgvData.BeginEdit(true);
                //if (dgvData.CurrentCell != null && dgvData.CurrentCell.GetType().ToString() == "System.Windows.Forms.DataGridViewComboBoxCell")
                //    (dgvData.EditingControl as ComboBox).DroppedDown = true;  //自動拉下清單
            }
        }

        private void dgvData_RowHeaderMouseClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            dgvData.CurrentCell = null;
            dgvData.Rows[e.RowIndex].Selected = true;
        }

        private void dgvData_ColumnHeaderMouseClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            dgvData.CurrentCell = null;
        }

        private void dgvData_MouseClick(object sender, MouseEventArgs e)
        {
            DataGridView dgv = (DataGridView)sender;
            DataGridView.HitTestInfo hit = dgv.HitTest(e.X, e.Y);

            if (hit.Type == DataGridViewHitTestType.TopLeftHeader)
            {
                dgvData.CurrentCell = null;
                dgvData.SelectAll();
            }
        }

        private void InitExperienceDataSouce()
        {
            List<UDT.CourseTypeDataSource> CourseTypeDataSources = Access.Select<UDT.CourseTypeDataSource>();

            this.dgvData.Rows.Clear();
            //oExperienceDataSources.Clear();
            foreach (UDT.CourseTypeDataSource record in CourseTypeDataSources)
            {
                List<object> source = new List<object>();

                source.Add(record.CourseType);
                source.Add(record.NotDisplay);

                int idx = this.dgvData.Rows.Add(source.ToArray());
                this.dgvData.Rows[idx].Tag = record;
            }
        }

        private bool AllPass()
        {
            bool allPass = true;
            IEnumerable<DataGridViewRow> Rows = this.dgvData.Rows.Cast<DataGridViewRow>().Where(x=>!x.IsNewRow);
            if (Rows.Count() == 0)
                return allPass;
            Rows.ToList().ForEach(x => x.Cells[0].ErrorText = string.Empty);
            foreach(DataGridViewRow row in this.dgvData.Rows)
            {
                if (row.IsNewRow)
                    continue;

                row.Cells[0].ErrorText = string.Empty;
                IEnumerable<DataGridViewRow> DuplicateRows = Rows.Where(x => (x.Cells[0].Value + "").Trim() == (row.Cells[0].Value + "").Trim());
                if (string.IsNullOrWhiteSpace(row.Cells[0].Value + ""))
                {
                    row.Cells[0].ErrorText = "必填。";
                    allPass = false;
                    continue;
                }
                if (DuplicateRows.Count() > 1)
                {
                    foreach(DataGridViewRow dRow in DuplicateRows)
                    {
                        if (string.IsNullOrEmpty(dRow.Cells[0].ErrorText))
                        {
                            dRow.Cells[0].ErrorText = "課程類別重複。";
                            allPass = false;
                        }
                    }
                }
            }

            return allPass;
        }

        private void Save_Click(object sender, EventArgs e)
        {
            this.dgvData.CurrentCell = null;
            this.Save.Enabled = false;
            if (!AllPass())
            {
                MessageBox.Show("請先修正錯誤再儲存。");
                this.Save.Enabled = true;
                return;
            }
            List<UDT.CourseTypeDataSource> nRecords = this.Access.Select<UDT.CourseTypeDataSource>();
            nRecords.ForEach(x => x.Deleted = true);
            //List<UDT.ExperienceDataSource> dRecords = new List<UDT.ExperienceDataSource>(); 
            foreach (DataGridViewRow dataGridViewRow in this.dgvData.Rows)
            {
                if (dataGridViewRow.IsNewRow)
                    continue;

                UDT.CourseTypeDataSource record = new UDT.CourseTypeDataSource();
                record.CourseType = dataGridViewRow.Cells[0].Value + "";
                record.NotDisplay = (dataGridViewRow.Cells[1].Value == null ? false : bool.Parse(dataGridViewRow.Cells[1].Value + ""));
                nRecords.Add(record);
            }
            nRecords.SaveAll();
            if (this._ShowDialog)
            {
                UDT.CourseTypeDataSource.RaiseAfterUpdateEvent();
                this.Close();
            }
            else
            {
                this.Save.Enabled = true;
                this.InitExperienceDataSouce();
                UDT.CourseTypeDataSource.RaiseAfterUpdateEvent();
                MessageBox.Show("儲存成功。");
            }
        }

        private void Exit_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}