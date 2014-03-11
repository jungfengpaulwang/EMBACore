using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Xml;
using FISCA.UDT;

namespace EMBACore.Forms
{
    public partial class DepartmentGroupForm : FISCA.Presentation.Controls.BaseForm
    {
        private DepartmentGroup_SingleForm frm;

        public List<UDT.DepartmentGroup> DepartmentGroups { set; get; }
        public DepartmentGroupForm()
        {
            InitializeComponent();

            this.Load += new System.EventHandler(this.DepartmentGroupForm_Load);
        }

        private void DepartmentGroupForm_Load(object sender, EventArgs e)
        {
            this.cmdAdd.Click += new System.EventHandler(this.button2_Click);
            this.cmdSelect.Click += new System.EventHandler(this.cmdSelect_Click);
            this.cmdDelete.Click += new System.EventHandler(this.button4_Click);
            this.cmdUpdate.Click += new System.EventHandler(this.button3_Click);

            this.RefreshData();
            this.makeForm();
        }

        void frm_AfterSaved(object sender, string[] uids)
        {
            this.RefreshData();
        }

        private void RefreshData()
        {
            AccessHelper ah = new AccessHelper();
            try
            {
                List<UDT.DepartmentGroup> departmentGroups = ah.Select<UDT.DepartmentGroup>();
                departmentGroups.Sort(new Comparison<UDT.DepartmentGroup>(delegate(UDT.DepartmentGroup x, UDT.DepartmentGroup y)
                {
                    return x.Order.CompareTo(y.Order);

                }));

                this.dataGridView1.Rows.Clear();
                foreach (UDT.DepartmentGroup dept in departmentGroups)
                {
                    object[] rawData = new object[] { dept.Name, dept.EnglishName, dept.Code, dept.Order };
                    int rowIndex = this.dataGridView1.Rows.Add(rawData);
                    DataGridViewRow row = this.dataGridView1.Rows[rowIndex];
                    row.Tag = dept;
                }
            }
            catch (Exception ex)
            {
                Util.ShowMsg("讀取系所組別資料時發生錯誤！", "注意！");
                return;
            }
        }

        private void makeForm()
        {
            this.frm = new DepartmentGroup_SingleForm();
            this.frm.AfterSaved += new UDTDetailContentBase.UDTSingleForm.UDTSingleFormEventHandler(frm_AfterSaved);
        }

        private void button2_Click(object sender, EventArgs e)
        {
            //this.makeForm();
            this.frm.SetUDT(new UDT.DepartmentGroup());
            this.frm.ShowDialog();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            if (this.dataGridView1.SelectedRows.Count < 1) {
                Util.ShowMsg("請先選擇要修改的系所組別","注意！");
                return ;
            }
            //this.makeForm();
            UDT.DepartmentGroup dept = (UDT.DepartmentGroup)this.dataGridView1.SelectedRows[0].Tag;            
            this.frm.SetUDT(dept);
            this.frm.ShowDialog();            
        }

        private void button4_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("您確定要刪除這個系所組別？", "注意！", MessageBoxButtons.YesNo, MessageBoxIcon.Information) != System.Windows.Forms.DialogResult.Yes)
                return;

            List<ActiveRecord> recs = new List<ActiveRecord>();
            foreach(DataGridViewRow row in this.dataGridView1.SelectedRows)
            {
                recs.Add((UDT.DepartmentGroup)row.Tag);
            }
            AccessHelper ah = new AccessHelper();
            try
            {
                ah.DeletedValues(recs);
                this.RefreshData();
            }
            catch (Exception ex)
            {
                Util.ShowMsg("刪除科目資料時發生錯誤！", "注意！");
            }
           
        }

        private void cmdSelect_Click(object sender, EventArgs e)
        {
            this.DepartmentGroups = new List<UDT.DepartmentGroup>();
            foreach (DataGridViewRow dgvr in this.dataGridView1.SelectedRows)
                this.DepartmentGroups.Add(dgvr.Tag as UDT.DepartmentGroup);

            this.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.Close();
        }
    }
}
