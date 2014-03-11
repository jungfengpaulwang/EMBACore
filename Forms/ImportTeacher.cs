using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using FISCA.UDT;

namespace EMBACore.Forms
{
    public partial class ImportTeacher : Form
    {
        public ImportTeacher()
        {
            InitializeComponent();
        }

        private void textBoxX1_Click(object sender, EventArgs e)
        {
            if (this.openFileDialog1.ShowDialog() != System.Windows.Forms.DialogResult.OK)
                return;

            this.textBoxX1.Text = this.openFileDialog1.FileName;
        }

        private void btnImport_Click(object sender, EventArgs e)
        {
            Aspose.Cells.Workbook wb = new Aspose.Cells.Workbook();
            try
            {
                wb.Open(this.textBoxX1.Text);

                Aspose.Cells.Worksheet ws = wb.Worksheets[0];

                int rowIndex = 1;
                int totalCount = 0;

                while (ws.Cells[rowIndex, 0].Value != null && !string.IsNullOrWhiteSpace(ws.Cells[rowIndex, 0].Value.ToString()))
                {
                    rowIndex += 1;
                    totalCount += 1;
                }


                this.progressBarX1.Maximum = totalCount;

                rowIndex = 1;
                AccessHelper ah = new AccessHelper();
                while (ws.Cells[rowIndex, 0].Value != null && !string.IsNullOrWhiteSpace(ws.Cells[rowIndex, 0].Value.ToString()))
                {
                    string ntu_sys_no = GetCellValue(ws.Cells[rowIndex, 0]);
                    string tea_name = GetCellValue(ws.Cells[rowIndex, 1]);
                    string tea_eng_name = GetCellValue(ws.Cells[rowIndex, 2]);
                    string tea_account = GetCellValue(ws.Cells[rowIndex, 7]);
                    string tea_email = GetCellValue(ws.Cells[rowIndex, 8]);
                    string tea_office_telno = GetCellValue(ws.Cells[rowIndex, 12]);
                    string unit = GetCellValue(ws.Cells[rowIndex, 15]);

                    //string emp_no = ws.Cells[rowIndex, 5].Value.ToString();


                    K12.Data.TeacherRecord tea = new K12.Data.TeacherRecord();
                    tea.Name = tea_name;
                    tea.TALoginName = tea_account;
                    tea.Email = tea_email;

                    string newTID = K12.Data.Teacher.Insert(tea);

                    //K12.Data.Teacher.Update(tea);
                        

                    UDT.TeacherExtVO udtTe = new UDT.TeacherExtVO();
                    udtTe.EnglishName = tea_eng_name;
                    udtTe.TeacherID = int.Parse(newTID);
                    udtTe.NtuSystemNo = ntu_sys_no;
                    //udtTe.EmployeeNo = emp_no;
                    udtTe.OtherPhone = tea_office_telno;
                    udtTe.MajorWorkPlace = unit;

                    List<ActiveRecord> rec = new List<ActiveRecord>();
                    rec.Add(udtTe);
                    ah.SaveAll(rec);

                    rowIndex += 1;

                    this.labelX1.Text = string.Format("{0} / {1} ", rowIndex.ToString(), totalCount.ToString());

                    this.progressBarX1.Value = rowIndex;
                }

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                return;
            }
        }

        private string GetCellValue(Aspose.Cells.Cell cell)
        {
            string result = "";
            if (cell.Value != null)
                result = cell.Value.ToString();

            return result;
        }
    }
}
