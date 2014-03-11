using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using FISCA.Presentation.Controls;
using SHSchool.Data;

namespace EMBACore.Forms
{
    public partial class LearningGroup : BaseForm
    {
        List<SHDepartmentRecord> depts;

        List<SHDepartmentRecord> deletedDepts;

        public LearningGroup()
        {
            InitializeComponent();
        }

        private void LearningGroup_Load(object sender, EventArgs e)
        {
            depts = SHSchool.Data.SHDepartment.SelectAll() ;
            this.deletedDepts = new List<SHDepartmentRecord>(); //要刪除的組別
            FillData();
        }

        private void FillData()
        {
            this.dataGridViewX1.Rows.Clear();            

            foreach (SHDepartmentRecord dept in this.depts)
            {
                object[] rawData = new object[] { dept.Name, dept.FullName, dept.Code };
                int rowIndex = this.dataGridViewX1.Rows.Add(rawData);
                this.dataGridViewX1.Rows[rowIndex].Tag = dept;
            }
            this.deletedDepts.Clear();
        }

        private void dataGridViewX1_UserDeletingRow(object sender, DataGridViewRowCancelEventArgs e)
        {
            if (MsgBox.Show("確定要刪除這筆資料？", "注意！", MessageBoxButtons.YesNo, MessageBoxIcon.Information) != System.Windows.Forms.DialogResult.Yes)
                return;

            SHDepartmentRecord dept = (SHDepartmentRecord)e.Row.Tag ;
            if (dept != null)
                this.deletedDepts.Add (dept);
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            bool isValid = this.ValidateData();
            if (!isValid)
                Util.ShowMsg("請確定所有資訊都已經輸入完成！", "儲存系所分組");

            List<SHDepartment> insertItems = new List<SHDepartment>();
            List<SHDepartment> updateItems = new List<SHDepartment>();
            foreach (DataGridViewRow row in this.dataGridViewX1.Rows)
            {
                if (!row.IsNewRow)
                {
                    if (row.Tag == null)  //addnew
                    {
                        
                        //SHDepartmentRecord dept = new SHDepartmentRecord(
                    }
                    else  //update
                    {
                        SHDepartmentRecord dept = (SHDepartmentRecord)row.Tag;
                        //dept.Name = row.Cells[0].Value.ToString();
                        //dept.Name = row.Cells[0].Value.ToString();
                        //dept.Name = row.Cells[0].Value.ToString();

                    }
                }
            } // end for
            

            

        }

        private bool ValidateData()
        {
            bool result = true;
            foreach (DataGridViewRow row in this.dataGridViewX1.Rows)
            {
                if (!row.IsNewRow)
                {                   
                    for (int i = 0; i < 3; i++)
                    {
                        if (row.Cells[0].Value == null)
                        {
                            row.Cells[0].Style.BackColor = Color.Pink;
                            result = false;
                        }
                    }                    
                }
            } // end for

            return result;
        }
    }
}
