using System;
using System.Data;
using System.Windows.Forms;
using FISCA.Data;
using FISCA.Presentation.Controls;

public partial class Class_Add : BaseForm
    {
        public Class_Add()
        {
            InitializeComponent();
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            string classname = txtName.Text.Trim();

            if (classname == String.Empty)
                return;

            QueryHelper queryHelper = new QueryHelper();
            string strQuery = String.Format(@"select class_name from class where class_name='{0}'", classname);

            DataTable dataTable = queryHelper.Select(strQuery);

            if (dataTable == null || dataTable.Rows.Count == 0)
            {
                K12.Data.ClassRecord addRecord = new K12.Data.ClassRecord();

                addRecord.Name = classname;

                string addRecord_ID = K12.Data.Class.Insert(addRecord);
                //Class.Instance.SyncDataBackground(ClassID);   同步處理
                //  log 待處理
                //PermRecLogProcess prlp = new PermRecLogProcess();
                //prlp.SaveLog("學籍.班級", "新增班級", "新增班級,名稱:" + txtName.Text);
                if (chkInputData.Checked)
                    K12.Presentation.NLDPanels.Class.PopupDetailPane(addRecord_ID);
                //Class.Instance.SyncDataBackground(ClassID);   同步處理
            }
            else
            {
                MessageBox.Show("班級名稱重複");
                return;
            }
            this.Close();
        }

        private void btnExit_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }

