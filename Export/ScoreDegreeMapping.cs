using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using FISCA.Presentation.Controls;
using FISCA.UDT;
using System.Threading.Tasks;

namespace EMBACore.Export
{
    public partial class ScoreDegreeMapping : BaseForm
    {
        private AccessHelper __Access;

        public ScoreDegreeMapping()
        {
            InitializeComponent();

            __Access = new AccessHelper();

            this.Load += new EventHandler(ScoreDegreeMapping_Load);
        }

        private void ScoreDegreeMapping_Load(object sender, EventArgs e)
        {
            this.circularProgress.Visible = true;
            this.circularProgress.IsRunning = true;

            List<UDT.GradeScoreMappingTable> GradeScoreMappings = new List<UDT.GradeScoreMappingTable>();
            Task task = Task.Factory.StartNew(() =>
            {
                GradeScoreMappings = __Access.Select<UDT.GradeScoreMappingTable>();
            }); 
            task.ContinueWith((x) =>
            {
                if (x.Exception != null)
                {
                    MessageBox.Show(x.Exception.InnerException.Message);
                }
                else
                {
                    GradeScoreMappings.ForEach((y) =>
                    {
                        List<object> source = new List<object>();

                        source.Add(y.Grade);
                        source.Add(y.GP);
                        source.Add(y.Score);

                        int idx = this.dgvData.Rows.Add(source.ToArray());
                    });
                }
                this.circularProgress.IsRunning = false;
                this.circularProgress.Visible = false;
            }, System.Threading.CancellationToken.None, TaskContinuationOptions.None, TaskScheduler.FromCurrentSynchronizationContext());
        }

        private void Cancel_Click(object sender, EventArgs e)
        {
            this.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.Close();
        }

        private void Save_Click(object sender, EventArgs e)
        {
            try
            {
                if (!this.ValidateData())
                {
                    MessageBox.Show("請先修正錯誤再儲存。");
                    this.DialogResult = System.Windows.Forms.DialogResult.None;
                    return;
                }
                List<UDT.GradeScoreMappingTable> GradeScoreMappings = __Access.Select<UDT.GradeScoreMappingTable>();
                GradeScoreMappings.ForEach(x => x.Deleted = true);
                GradeScoreMappings.SaveAll();
                GradeScoreMappings.Clear();
                foreach (DataGridViewRow row in this.dgvData.Rows)
                {
                    if (row.IsNewRow)
                        continue;

                    UDT.GradeScoreMappingTable GradeScoreMappingTable = new UDT.GradeScoreMappingTable();

                    GradeScoreMappingTable.Grade = (row.Cells[0].Value + "").Trim();
                    GradeScoreMappingTable.GP = decimal.Parse(row.Cells[1].Value + "");
                    GradeScoreMappingTable.Score = decimal.Parse(row.Cells[2].Value + "");

                    GradeScoreMappings.Add(GradeScoreMappingTable);
                }
                GradeScoreMappings.SaveAll();
                MessageBox.Show("儲存成功。");
                this.DialogResult = System.Windows.Forms.DialogResult.OK;
                this.Close();                
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                this.DialogResult = System.Windows.Forms.DialogResult.None;
            }
        }

        private bool ValidateData()
        {
            bool is_valid = true;
            //確定使用者修改的值都更新到控制項裡了(預防點選checkbox 後直接點選儲存，這時抓到的值仍是前一個值)。
            this.dgvData.EndEdit();
            this.dgvData.CurrentCell = null;
            Dictionary<string, List<DataGridViewRow>> dicDuplicatedRows = new Dictionary<string,List<DataGridViewRow>>();
            foreach (DataGridViewRow row in this.dgvData.Rows)
            {
                if (row.IsNewRow)
                    continue;

                //1. 「等第」為必填
                if (string.IsNullOrEmpty(row.Cells[0].Value + ""))
                {
                    row.Cells[0].ErrorText = "必填。";
                    is_valid = false;
                }
                else
                    row.Cells[0].ErrorText = string.Empty;

                //2. 「積分」為必填
                if (string.IsNullOrEmpty(row.Cells[1].Value + ""))
                {
                    row.Cells[1].ErrorText = "必填。";
                    is_valid = false;
                }
                else
                {
                    decimal gp = 0;
                    if (!decimal.TryParse(row.Cells[1].Value + "", out gp))
                    {
                        row.Cells[1].ErrorText = "請填數字。";
                        is_valid = false;
                    }
                    else
                        row.Cells[1].ErrorText = string.Empty;
                }

                //3. 「分數」為必填
                if (string.IsNullOrEmpty(row.Cells[2].Value + ""))
                {
                    row.Cells[2].ErrorText = "必填。";
                    is_valid = false;
                }
                else
                {
                    decimal score = 0;
                    if (!decimal.TryParse(row.Cells[2].Value + "", out score))
                    {
                        row.Cells[2].ErrorText = "請填數字。";
                        is_valid = false;
                    }
                    else
                        row.Cells[2].ErrorText = string.Empty;
                }

                if (!dicDuplicatedRows.ContainsKey((row.Cells[0].Value + "").Trim().ToLower()))
                    dicDuplicatedRows.Add((row.Cells[0].Value + "").Trim().ToLower(), new List<DataGridViewRow>());

                dicDuplicatedRows[(row.Cells[0].Value + "").Trim().ToLower()].Add(row);
            }
            foreach (string key in dicDuplicatedRows.Keys)
            {
                if (dicDuplicatedRows[key].Count > 1)
                {
                    dicDuplicatedRows[key].ForEach((x) => x.Cells[0].ErrorText = "重覆。");
                    is_valid = false;
                }
            }
            return is_valid;
        }
    }
}
