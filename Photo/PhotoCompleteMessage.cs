using System;
using System.Collections.Generic;
using System.Windows.Forms;
using Aspose.Cells;

namespace EMBA.Photo
{
    public partial class PhotoCompleteMessage : FISCA.Presentation.Controls.BaseForm
    {
        private List<StudPhotoEntity> _StudPhotoEntityList;

        public PhotoCompleteMessage()
        {
            InitializeComponent();
            _StudPhotoEntityList = new List<StudPhotoEntity>();
        }


        public void SetStudPhotoEntity(List<StudPhotoEntity> StudPhotoEntityList1)
        {
            _StudPhotoEntityList = StudPhotoEntityList1;
        }

        private void btnExit_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        public void SetFormText(string formText)
        {
            this.Text = formText;
        }

        public void SetMessageText(string msg)
        {
            lblMsg.Text = msg;
        }

        public void SetErrorMsgBottonEnable(bool Display)
        {
            btnShowError.Enabled = Display;
        }

        private void btnShowError_Click(object sender, EventArgs e)
        {
            if (_StudPhotoEntityList.Count < 1)
                return;

            Workbook wb = new Workbook();
            Worksheet wst = wb.Worksheets[0];

            wst.Cells[0, 0].PutValue("學號");
            wst.Cells[0, 1].PutValue("班級");
            wst.Cells[0, 2].PutValue("姓名");
            wst.Cells[0, 3].PutValue("錯誤訊息");

            int row = 1;
            foreach (StudPhotoEntity spe in _StudPhotoEntityList)
            {
                wst.Cells[row, 0].PutValue(spe.StudentNumber);
                wst.Cells[row, 1].PutValue(spe.ClassName);
                wst.Cells[row, 2].PutValue(spe.StudentName);
                wst.Cells[row, 3].PutValue(spe.ErrorMessage);
                row++;
            }


            try
            {
                wb.Save(Application.StartupPath + "\\Reports\\照片處理錯誤檢視.xls", FileFormatType.Excel2003);
                System.Diagnostics.Process.Start(Application.StartupPath + "\\Reports\\照片處理錯誤檢視.xls");

            }
            catch
            {
                System.Windows.Forms.SaveFileDialog sd1 = new SaveFileDialog();
                sd1.Title = "另存新檔";
                sd1.FileName = "照片處理錯誤檢視.xls";
                sd1.Filter = "Excel檔案 (*.xls)|*.xls|所有檔案 (*.*)|*.*";
                if (sd1.ShowDialog() == DialogResult.OK)
                {
                    try
                    {
                        wb.Save(sd1.FileName, FileFormatType.Excel2003);
                        System.Diagnostics.Process.Start(sd1.FileName);
                    }
                    catch
                    {
                        System.Windows.Forms.MessageBox.Show("指定路徑無法存取。", "建立檔案失敗", System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Error);
                        return;
                    }
                }
            }
        }
    }
}
