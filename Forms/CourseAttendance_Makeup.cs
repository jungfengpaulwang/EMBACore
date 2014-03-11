using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using FISCA.Presentation.Controls;

namespace EMBACore.Forms
{
    public partial class CourseAttendance_Makeup : BaseForm
    {
        private UDT.Absence abs;
        private int rowIndex;
        private int colIndex;

        public delegate void AfterMakeUpEventHandler(object sender, MakeUpEventArgs arg) ;
        public event AfterMakeUpEventHandler AfterMakeUp;

        public CourseAttendance_Makeup()
        {
            InitializeComponent();
        }

        public void SetAbsence(UDT.Absence abs, int rowIndex, int colIndex)
        {
            if (abs == null)  this.Close();

            this.abs = abs;
            this.rowIndex = rowIndex;
            this.colIndex = colIndex;
        }

        private void chkIsMakeup_CheckedChanged(object sender, EventArgs e)
        {
            this.txtMakeup.Enabled = this.chkIsMakeup.Enabled;
            this.labelX1.Enabled = this.chkIsMakeup.Enabled;
        }

        private void btnOk_Click(object sender, EventArgs e)
        {
            this.abs.IsMakeUp = this.chkIsMakeup.Checked;
            this.abs.MakeUpDescription = (this.chkIsMakeup.Checked) ? this.txtMakeup.Text : "";
            this.Close();
            if (this.AfterMakeUp != null)
            {
                MakeUpEventArgs arg = new MakeUpEventArgs();
                arg.AbsenceRecord = this.abs;
                arg.RowIndex = this.rowIndex;
                arg.ColumnIndex = this.colIndex;
                this.AfterMakeUp(this, arg);
            }
        }

        private void CourseAttendance_Makeup_Load(object sender, EventArgs e)
        {
            this.chkIsMakeup.Checked = this.abs.IsMakeUp;
            this.txtMakeup.Text = this.abs.MakeUpDescription;
        }

    }

    public class MakeUpEventArgs : EventArgs
    {
        public MakeUpEventArgs()
            : base()
        {
        }

        public int RowIndex { get; set; }
        public int ColumnIndex { get; set; }
        public UDT.Absence AbsenceRecord { get; set; }
    }
}
