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

namespace EMBACore.Forms
{
    public partial class QueryAdvisor : BaseForm
    {

        public QueryAdvisor()
        {
            InitializeComponent();
            
            this.Load += new System.EventHandler(this.QueryAdvisor_Load);
        }

        private void QueryAdvisor_Load(object sender, EventArgs e)
        {
            this.progress.Visible = false;
            this.progress.IsRunning = false;
            this.dgvData.Columns["PaperName"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;
        }

        private void btnQuery_Click(object sender, EventArgs e)
        {
            this.progress.Visible = true;
            this.progress.IsRunning = true;

            Application.DoEvents();

            string strQuery = string.Format(@"select student.name as name, paper.paper_name as paper_name, student.id as id from $ischool.emba.paper as paper join student on student.id=paper.ref_student_id
where (xpath_string('<root>' || paper.advisor_list || '</root>','Advisor[1]/Name')) like '%{0}%' or (xpath_string('<root>' || paper.advisor_list || '</root>','Advisor[2]/Name')) like '%{0}%' or (xpath_string('<root>' || paper.advisor_list || '</root>','Advisor[3]/Name')) like '%{0}%'
", Advisor_Name.Text.Trim());

            DataTable dataTable = (new QueryHelper()).Select(strQuery);

            this.dgvData.Rows.Clear();

            if (dataTable != null && dataTable.Rows.Count > 0)
            {
                dataTable.Rows.Cast<DataRow>().ToList().ForEach((x) =>
                {
                    List<object> datas = new List<object>();

                    datas.Add(x["name"] + "");
                    datas.Add(x["paper_name"] + "");
                    datas.Add(x["id"] + "");

                    int rowIndex = this.dgvData.Rows.Add(datas.ToArray());
                    this.dgvData.Rows[rowIndex].Tag = x;
                });
            }

            this.dgvData.AutoResizeColumns();
            this.progress.Visible = false;
            this.progress.IsRunning = false;
        }

        private void btnExit_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void btnAddToTempSource_Click(object sender, EventArgs e)
        {
            try
            {
                this.dgvData.SelectedRows.Cast<DataGridViewRow>().ToList().ForEach((x) =>
                {
                    if (!x.IsNewRow)
                    {
                        if (!K12.Presentation.NLDPanels.Student.TempSource.Contains(x.Cells["StudentID"].Value.ToString()))
                            K12.Presentation.NLDPanels.Student.AddToTemp(new List<string>(){ x.Cells["StudentID"].Value.ToString() });
                    }
                });

                MessageBox.Show("已加入選取學生至待處理！");
            }
            catch (Exception ex)
            {
                MsgBox.Show(ex.Message);
            }
        }
    }
}
//  Timer Sample
//namespace WindowsFormsApplication8
//{
//    public partial class Form1 : Form
//    {
//        public Form1()
//        {
//            InitializeComponent();
//        }

//        private void Form1_Load(object sender, EventArgs e)
//        {
//            StatusChecker statusChecker = new StatusChecker(3);
//            System.Threading.Timer stateTimer = new System.Threading.Timer(new TimerCallback(statusChecker.CheckStatus));
//            stateTimer.Change(0, 1000);
//        }
//    }

//    class StatusChecker
//    {
//        private int invokeCount;
//        private int maxCount;

//        public StatusChecker(int count)
//        {
//            invokeCount = 0;
//            maxCount = count;
//        }

//        public void CheckStatus(Object stateInfo)
//        {
//            Console.WriteLine("public void CheckStatus(Object stateInfo)");
//        }
//    }
//}