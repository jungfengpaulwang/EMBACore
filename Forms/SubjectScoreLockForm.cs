using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using FISCA.Presentation.Controls;
using EMBACore.DataItems;
using FISCA.Data;
using FISCA.UDT;

namespace EMBACore.Forms
{
    public partial class SubjectScoreLockForm : BaseForm
    {
        private bool is_data_loading = false;  //正在載入資料中
        private UDT.SubjectScoreLock targetScoreLock;

        public SubjectScoreLockForm()
        {
            InitializeComponent();
        }

        private void SubjectScoreLock_Load(object sender, EventArgs e)
        {
            Util.InitSchoolYearNumberUpDown(this.nudSchoolYear);
            Util.InitSemesterCombobox(this.cboSemester);
            this.GetLockedStatus();
        }

        private void GetLockedStatus()
        {
            this.is_data_loading = true;
            this.targetScoreLock = null;

            bool is_locked = false;
            try
            {
                AccessHelper hp = new AccessHelper();
                string condition = string.Format("school_year={0} and semester={1}", this.nudSchoolYear.Value.ToString(), ((SemesterItem)this.cboSemester.SelectedItem).Value);
                List<UDT.SubjectScoreLock> scoreLocks = hp.Select<UDT.SubjectScoreLock>(condition);
                if (scoreLocks.Count > 0)
                {
                    this.targetScoreLock = scoreLocks[0];
                    is_locked = this.targetScoreLock.IsLocked;
                }
                this.chkIsLocked.Checked = is_locked;
            }
            catch (Exception ex)
            {

            }

            this.is_data_loading = false;
        }

        private void chkIsLocked_CheckedChanged(object sender, EventArgs e)
        {
            

        }

        private void nudSchoolYear_ValueChanged(object sender, EventArgs e)
        {
            this.GetLockedStatus();
        }

        private void cboSemester_SelectedIndexChanged(object sender, EventArgs e)
        {
            this.GetLockedStatus();
        }

        private void btnSet_Click(object sender, EventArgs e)
        {
            //1. 重設密碼登入，待實作。


            //2. 更新資料
            try
            {
                // update status 
                if (this.targetScoreLock == null)
                {
                    this.targetScoreLock = new UDT.SubjectScoreLock();
                    this.targetScoreLock.SchoolYear = (int)this.nudSchoolYear.Value;
                    this.targetScoreLock.Semester = int.Parse(((SemesterItem)this.cboSemester.SelectedItem).Value);
                }

                this.targetScoreLock.IsLocked = this.chkIsLocked.Checked;
                List<ActiveRecord> recs = new List<ActiveRecord>();
                recs.Add(this.targetScoreLock);
                recs.SaveAll();
                string msg = string.Format("已 {0} {1} {2} 成績輸入設定。",
                    (this.chkIsLocked.Checked ? "鎖定" : "開放"),
                    this.nudSchoolYear.Value,
                    ((SemesterItem)this.cboSemester.SelectedItem).Name);

                //log
                FISCA.LogAgent.ApplicationLog.Log("成績輸入設定", "修改", "", "", msg);

                Util.ShowMsg(msg, "開放/鎖定成績輸入！");

            }
            catch (Exception ex)
            {
                string msg = string.Format("{0} {1} {2} 成績輸入設定時發生錯誤！",
                    (this.chkIsLocked.Checked ? "鎖定" : "開放"),
                    this.nudSchoolYear.Value,
                    ((SemesterItem)this.cboSemester.SelectedItem).Name);
                Util.ShowMsg("", "開放/鎖定成績輸入！");
            }

            //reload data ...
            this.GetLockedStatus();
        }
    }
}
