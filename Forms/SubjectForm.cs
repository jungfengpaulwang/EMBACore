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
    public partial class SubjectForm : FISCA.Presentation.Controls.BaseForm
    {
        private Subject_SingleForm frm;
        private GenerateCourse mForm;

        public List<UDT.Subject> rawSubjects ;

        public SubjectForm()
        {
            InitializeComponent();

            this.cmdAdd.Visible = true;
            this.cmdUpdate.Visible = true;
            this.cmdDelete.Visible = true;
        }

        public SubjectForm(GenerateCourse form)
        {
            InitializeComponent();

            if (form != null)
            {
                this.mForm = form;

                this.cmdAdd.Visible = false;
                this.cmdUpdate.Visible = false;
                this.cmdDelete.Visible = false;
            }
        }

        private void SubjectForm_Load(object sender, EventArgs e)
        {
            this.RefreshData();
            this.makeForm();
        }

        void frm_AfterSaved(object sender, string[] uids)
        {
            UDT.Subject.RaiseAfterUpdateEvent();    
            this.RefreshData();
        }

        private void RefreshData(string search_string = "")
        {
            AccessHelper ah = new AccessHelper();
            try
            {
                List<UDT.Subject> subjects = ah.Select<UDT.Subject>();
                subjects.Sort(new Comparison<UDT.Subject>(delegate(UDT.Subject x, UDT.Subject y)
                {
                    return x.SubjectCode.CompareTo(y.SubjectCode);

                }));
                this.rawSubjects = subjects;

                this.dataGridView1.Rows.Clear();
                foreach (UDT.Subject subj in subjects)
                {
                    if (!string.IsNullOrWhiteSpace(search_string))
                    {
                        if (!subj.Name.Trim().ToLower().Contains(search_string.Trim().ToLower()))
                            continue;
                    }
                    object[] rawData = new object[] { "開課", subj.DeptName, subj.DeptCode, subj.Name, subj.EnglishName, subj.Credit, subj.SubjectCode, subj.NewSubjectCode, subj.IsRequired, subj.WebUrl, subj.Description, subj.Remark };
                    int rowIndex = this.dataGridView1.Rows.Add(rawData);
                    DataGridViewRow row = this.dataGridView1.Rows[rowIndex];
                    row.Tag = subj;
                }
            }
            catch (Exception ex)
            {
                Util.ShowMsg("讀取科目資料時發生錯誤！", "注意！");
                return;
            }
            finally
            {
                if (this.mForm == null)
                    this.dataGridView1.Columns[0].Visible = false;
                else
                    this.dataGridView1.Columns[0].Visible = true;
            }
        }

        private void makeForm()
        {
            this.frm = new Subject_SingleForm();
            this.frm.AfterSaved += new UDTDetailContentBase.UDTSingleForm.UDTSingleFormEventHandler(frm_AfterSaved);
        }

        private void button2_Click(object sender, EventArgs e)
        {
            //this.makeForm();
            this.frm.SetUDT(new UDT.Subject());
            frm.SetSubjects(this.rawSubjects);
            this.frm.ShowDialog();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            if (this.dataGridView1.SelectedRows.Count < 1) {
                Util.ShowMsg("請先選擇要修改的科目","注意！");
                return ;
            }
            //this.makeForm();
            UDT.Subject subj = (UDT.Subject)this.dataGridView1.SelectedRows[0].Tag ;            
            this.frm.SetUDT(subj);
            frm.SetSubjects(this.rawSubjects);
            this.frm.ShowDialog();            
        }

        private void button4_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("您確定要刪除這個科目？", "注意！", MessageBoxButtons.YesNo, MessageBoxIcon.Information) != System.Windows.Forms.DialogResult.Yes)
                return;

            StringBuilder sb = new StringBuilder("刪除科目如下：");
            List<ActiveRecord> recs = new List<ActiveRecord>();
            foreach(DataGridViewRow row in this.dataGridView1.SelectedRows)
            {
                UDT.Subject subj = (UDT.Subject)row.Tag;
                sb.Append( makeSubjMsg(subj));
                recs.Add(subj);
            }

            AccessHelper ah = new AccessHelper();
            try
            {
                ah.DeletedValues(recs);
                FISCA.LogAgent.ApplicationLog.Log("科目管理", "刪除", "", "", sb.ToString());
                this.RefreshData();
            }
            catch (Exception ex)
            {
                Util.ShowMsg("刪除科目資料時發生錯誤！", "注意！");
            }
           
        }

        private string makeSubjMsg(UDT.Subject subj)
        {
            return string.Format("科目名稱 : {0} , 課號：{1},  識別碼: {2} , 英文名稱 :{3} ",
                subj.Name, subj.NewSubjectCode, subj.SubjectCode, subj.EnglishName );
        }

        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.ColumnIndex == 0 && this.mForm != null)
                this.mForm.AddSubject(this.dataGridView1.Rows[e.RowIndex].Tag as UDT.Subject);
        }

        private void txtCourse_TextChanged(object sender, EventArgs e)
        {
            this.RefreshData(this.txtCourse.Text);
        }

        private void btnExit_Click(object sender, EventArgs e)
        {
            this.Close();
        }     
    }
}
