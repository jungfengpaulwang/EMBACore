using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Campus.Windows;
using System.Xml;
using Campus.Configuration;
using EMBACore.DataItems;
using FISCA.Permission;

namespace EMBACore.DetailItems
{
    [AccessControl("ischool.EMBA.Student_TakeCourse", "修課紀錄", "學生>資料項目")]
    public partial class Student_TakeCourse : DetailContentImproved
    {

        public Student_TakeCourse()
        {
            InitializeComponent();
            this.Group = "修課紀錄";
        }

        private void Student_TakeCourse_Load(object sender, EventArgs e)
        {
            //1. Fill SchoolYear, Semester
            this.cboSemester.Items.Clear();
            foreach (SemesterItem semester in SemesterItem.GetSemesterList())
                this.cboSemester.Items.Add(semester);

            XmlDocument xmlSystemConfig = new XmlDocument();
            xmlSystemConfig.LoadXml(Config.App["系統設定"].PreviousData.OuterXml);
            XmlElement elmSchoolYear = (XmlElement)xmlSystemConfig.DocumentElement.SelectSingleNode("DefaultSchoolYear");
            XmlElement elmSemester = (XmlElement)xmlSystemConfig.DocumentElement.SelectSingleNode("DefaultSemester");

            this.nudSchoolYear.Value = int.Parse(elmSchoolYear.InnerText);
            this.cboSemester.SelectedItem = SemesterItem.GetSemesterByCode(elmSemester.InnerText);
            //EMBACore.Forms.SemesterItem

            //2. Get Courses  this student takes in this semester, and fill to DataGrid.
            List<K12.Data.SCAttendRecord> records = K12.Data.SCAttend.SelectByStudentID(this.PrimaryKey);
            foreach (K12.Data.SCAttendRecord att in records)
            {
                object[] rawData = new object[] { att.Course.Name };
                int rowData = this.dataGridViewX1.Rows.Add(rawData);
                DataGridViewRow dr = this.dataGridViewX1.Rows[rowData];
            }
        }

        private void btnAddNew_Click(object sender, EventArgs e)
        {
            //顯示 一個有指定學年度學期，且可選擇課程的表單
            Student_TakeCourse_Form frm = new Student_TakeCourse_Form();
            frm.AfterSaved += new EventHandler(frm_AfterSaved);
            frm.ShowDialog();
        }

        void frm_AfterSaved(object sender, EventArgs e)
        {
            //Refresh Grid
            Util.ShowMsg("Data Saved, Refresh Data Here","Attention !");

        }

        protected override void OnInitializeAsync()
        {
  
        }

        protected override void OnInitializeComplete(Exception error)
        {            
            //Bind School Year, Semester
        }

        protected override void OnPrimaryKeyChangedAsync()
        {
            //Record = Student.SelectByID(PrimaryKey);
            // Get SCAttend Record
        }

        protected override void OnDirtyStatusChanged(ChangeEventArgs e)
        {
            if (UserAcl.Current[this.GetType()].Editable)
                SaveButtonVisible = e.Status == ValueStatus.Dirty;
            else
                this.SaveButtonVisible = false;

            CancelButtonVisible = e.Status == ValueStatus.Dirty;
        }

        protected override void OnPrimaryKeyChangedComplete(Exception error)
        {            
            
            //Bind SCAttend Records To Grid !

            //ResetDirtyStatus();
        }
    }
}
