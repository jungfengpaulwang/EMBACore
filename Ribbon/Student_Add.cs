using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using FISCA.Presentation.Controls;
using K12.Data;


public partial class Student_Add : BaseForm
    {
        public Student_Add()
        {
            InitializeComponent();
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            if (txtName.Text.Trim() == "")
                return;

            StudentRecord studRec = new StudentRecord();
            studRec.Name = txtName.Text;
            string StudentID = Student.Insert(studRec);
            //  新的 log 機制待寫
            //  這是舊的：PermRecLogProcess prlp = new PermRecLogProcess();
            //prlp.SaveLog("學籍.學生", "新增學生", "新增學生姓名:" + txtName.Text);
            if (chkInputData.Checked == true)
            {
                if (StudentID != "")
                {
                    K12.Presentation.NLDPanels.Student.PopupDetailPane(StudentID);
                }
            }
            this.Close();
        }

        private void btnExit_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }

