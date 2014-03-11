using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using FISCA.Presentation.Controls;
using FISCA.Data;


public partial class Teacher_Add : BaseForm
    {
        public Teacher_Add()
        {
            InitializeComponent();
        }

        private void btnExit_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            if (txtName.Text.Trim() == "")
                return;

            QueryHelper queryHelper = new QueryHelper();
            string strQuery = String.Format(@"select id, teacher_name, nickname from teacher
where teacher_name='{0}'", txtName.Text.Trim());

            DataTable dataTable = queryHelper.Select(strQuery);
            IEnumerable<DataRow> teacherDatas = dataTable.Rows.Cast<DataRow>();

            string TeacherID = string.Empty;
            K12.Data.TeacherRecord teacherRec = new K12.Data.TeacherRecord();
            teacherRec.Name = txtName.Text.Trim();
            if (dataTable == null || dataTable.Rows.Count == 0)
                teacherRec.Nickname = txtNickName.Text.Trim();
            else
                teacherRec.Nickname = GetUniqueNickName(teacherDatas.Where(x=>x["teacher_name"].ToString() == txtName.Text.Trim()));
            
            TeacherID = K12.Data.Teacher.Insert(teacherRec);
            
            //  同步資料
            //  Teacher.Instance.SyncDataBackground(TeacherID);
            
            if (chkInputData.Checked)
            {
                if (!String.IsNullOrEmpty(TeacherID))
                {
                    K12.Presentation.NLDPanels.Teacher.PopupDetailPane(TeacherID);
                    //  同步資料
                    //  Teacher.Instance.SyncDataBackground(TeacherID);
                }
            }
            //  log
            //PermRecLogProcess prlp = new PermRecLogProcess();
            //prlp.SaveLog("學籍.教師", "新增教師", "新增教師,姓名:" + txtName.Text + ",暱稱:" + txtNickName.Text);

            this.Close();            
        }

        private string GetUniqueNickName(IEnumerable<DataRow> teacherDatas)
        {
            string nick_name = "New";
            string i = string.Empty;

            while (teacherDatas.Where(x => x["nickname"].ToString() == (nick_name + i)).Count() > 0)
            {
                i = ((i+0) + 1).ToString();
            }
            return nick_name + i;
        }
    }

