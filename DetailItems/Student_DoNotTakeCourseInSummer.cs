using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Campus.Windows;
using FISCA.Permission;
using FISCA.UDT;

namespace EMBACore.DetailItems
{
     [AccessControl("ischool.EMBA.Student_DotNotTakeCourseInSummer", "夏季學期修課紀錄", "學生>資料項目")]
    public partial class Student_DoNotTakeCourseInSummer : DetailContentImproved
    {
         private int recordCount = 6;   //max record count per student.
         private Dictionary<int, UDT.DoNotTakeCourseInSummer> dicRecords;

        public Student_DoNotTakeCourseInSummer()
        {
            InitializeComponent();
            this.Group = "夏季學期修課紀錄";            
        }

        private void Student_DoNotTakeCourseInSummer_Load(object sender, EventArgs e)
        {
            this.dicRecords = new Dictionary<int, UDT.DoNotTakeCourseInSummer>();
            this.WatchChange(new DataGridViewSource(this.dataGridViewX1));
        }


        protected override void OnPrimaryKeyChangedAsync()
        {
            List<UDT.DoNotTakeCourseInSummer> recs = (new AccessHelper()).Select<UDT.DoNotTakeCourseInSummer>("ref_student_id=" + this.PrimaryKey);
            this.dicRecords.Clear();
            foreach (UDT.DoNotTakeCourseInSummer sum in recs)
            {
                if (!this.dicRecords.ContainsKey(sum.RecNo))
                    this.dicRecords.Add(sum.RecNo, sum);
            }
        }

        protected override void OnPrimaryKeyChangedComplete(Exception error)
        {
            if (error != null) //有錯就直接丟出去吧。
                throw error;

            ErrorTip.Clear();

            this.dataGridViewX1.Rows.Clear();
            for (int i = 0; i < this.recordCount; i++)
            {
                int recNo = (i+1) ;
                UDT.DoNotTakeCourseInSummer rec = null;
                if (this.dicRecords.ContainsKey(recNo))
                    rec = this.dicRecords[recNo];
                string rec_name = string.Format("紀錄 {0}", recNo.ToString());
                string school_year = (rec == null) ? "" : rec.SchoolYear;
                string remark = (rec == null) ? "" : rec.Remark;

                object[] rawdata = new object[] { "紀錄 " + recNo.ToString(),  school_year, remark };
                int rowIndex = this.dataGridViewX1.Rows.Add(rawdata);
                this.dataGridViewX1.Rows[rowIndex].Tag = rec;
            }

            ResetDirtyStatus();
        }

        protected override void OnDirtyStatusChanged(ChangeEventArgs e)
        {
            if (UserAcl.Current[this.GetType()].Editable)
                SaveButtonVisible = e.Status == ValueStatus.Dirty;
            else
                this.SaveButtonVisible = false;

            CancelButtonVisible = e.Status == ValueStatus.Dirty;
        }

        protected override void OnValidateData(Dictionary<Control, string> errors)
        {
            //確定使用者修改的值都更新到控制項裡了(預防點選checkbox 後直接點選儲存，這時抓到的值仍是前一個值)。
            this.dataGridViewX1.EndEdit();
            foreach (DataGridViewRow row in this.dataGridViewX1.Rows)
            {
                if (!row.IsNewRow)
                {
                    if (row.Cells["colSchoolYear"].Value == null || row.Cells["colSchoolYear"].Value == "")
                    {
                        if (row.Cells["colRemark"].Value != null && row.Cells["colRemark"].Value.ToString() != "")
                        {
                            row.Cells["colSchoolYear"].ErrorText = "請填入學年度";
                            errors.Add(this.dataGridViewX1, "請填入學年度");
                        }
                        else
                            row.Cells["colSchoolYear"].ErrorText = "";
                    }
                    else  // if schoolyear is not empty
                    {
                        int result = 0;
                        if (!int.TryParse(row.Cells["colSchoolYear"].Value.ToString(), out result))
                        {
                            row.Cells["colSchoolYear"].ErrorText = "學年度請輸入數字";
                            errors.Add(this.dataGridViewX1, "學年度請輸入數字");
                        }
                        else
                            row.Cells["colSchoolYear"].ErrorText = "";
                    }
                }
            }// end foreach
        }

        protected override void OnSaveData()
        {
            List<ActiveRecord> Recs = new List<ActiveRecord>();

            for (int i = 0; i < this.dataGridViewX1.Rows.Count; i++)
            {
                int recNo = i + 1;
                DataGridViewRow row = this.dataGridViewX1.Rows[i];
                UDT.DoNotTakeCourseInSummer udt = (UDT.DoNotTakeCourseInSummer)row.Tag;
                //
                if (row.Cells["colSchoolYear"].Value == null || string.IsNullOrWhiteSpace(row.Cells["colSchoolYear"].Value.ToString()))
                {
                    if (udt != null)
                    {
                        udt.Deleted = true;
                        Recs.Add(udt);
                    }
                }
                else
                {
                    if (udt != null)
                    {
                        udt.SchoolYear = row.Cells["colSchoolYear"].Value.ToString();
                        udt.RecNo = recNo;
                        udt.Remark = (row.Cells["colRemark"].Value == null ) ? "" : row.Cells["colRemark"].Value.ToString();
                        Recs.Add(udt);
                    }
                    else {
                        udt = new UDT.DoNotTakeCourseInSummer();
                        udt.StudentID = int.Parse(this.PrimaryKey);
                        udt.SchoolYear = row.Cells["colSchoolYear"].Value.ToString();
                        udt.RecNo = recNo;
                        udt.Remark = (row.Cells["colRemark"].Value == null) ? "" : row.Cells["colRemark"].Value.ToString();
                        Recs.Add(udt);
                    }
                }
            }

            (new AccessHelper()).SaveAll(Recs);
            
            this.OnPrimaryKeyChanged(EventArgs.Empty);

            ResetDirtyStatus();
        }

        private void dataGridViewX1_CellEndEdit(object sender, DataGridViewCellEventArgs e)
        {
            if (e.ColumnIndex == 1)  //schoolyear
            {
                if (!string.IsNullOrWhiteSpace(this.dataGridViewX1.Rows[e.RowIndex].Cells[e.ColumnIndex].Value.ToString()))
                {
                    int result = 0;

                    if (!int.TryParse(this.dataGridViewX1.Rows[e.RowIndex].Cells[e.ColumnIndex].Value.ToString(), out result))
                    {
                        this.dataGridViewX1.Rows[e.RowIndex].Cells[e.ColumnIndex].ErrorText = "請學年度請輸入數字";
                    }
                    else
                        this.dataGridViewX1.Rows[e.RowIndex].Cells[e.ColumnIndex].ErrorText = "";
                }
                else
                    this.dataGridViewX1.Rows[e.RowIndex].Cells[e.ColumnIndex].ErrorText = "";
            }
        }



    }
}
