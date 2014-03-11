using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using FISCA.Presentation.Controls;

namespace EMBACore.DetailItems
{
    public partial class Student_TakeCourse_Form : BaseForm
    {
        public event EventHandler AfterSaved;

        public Student_TakeCourse_Form()
        {
            InitializeComponent();
            
        }

        private void buttonX1_Click(object sender, EventArgs e)
        {
            if (this.AfterSaved != null)
                this.AfterSaved(this, EventArgs.Empty);
        }
    }
}
