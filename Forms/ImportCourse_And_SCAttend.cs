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
using SHSchool.Data;
using Aspose.Cells;

namespace EMBACore.Forms
{
    public partial class ImportCourse_And_SCAttend : BaseForm
    {
        private Dictionary<string, UDT.Subject> dicSubjects;
        private Dictionary<string, SHSchool.Data.SHCourseRecord> dicCourses;
        private Dictionary<string, UDT.CourseExt> dicCourseExts;    //<courseID,

        public ImportCourse_And_SCAttend()
        {
            InitializeComponent();
        }

        private void ImportCourse_And_SCAttend_Load(object sender, EventArgs e)
        {

        }

        private void textBoxX1_Click(object sender, EventArgs e)
        {
            if (this.openFileDialog1.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                this.textBoxX1.Text = this.openFileDialog1.FileName;
            }
        }

        private void buttonX1_Click(object sender, EventArgs e)
        {
            this.textBoxX2.Text = "";

            this.addMsg(" ==== 開始匯入課程 ===");

            if (string.IsNullOrWhiteSpace(this.textBoxX1.Text))
                return;


            //1. 讀取所有科目 (供比對出科目代碼)
            this.GetAllSubjects();

            //2. 讀取某學年度學期的所有課程 (決定要新增或修改)
            this.GetAllCourses();

            //3. 開始讀取Excel 上的課程資料，
            Workbook wb = new Aspose.Cells.Workbook();
            wb.Open(this.textBoxX1.Text);
            Worksheet ws = wb.Worksheets[1];   //課程
            int rowIndex = 1;
            while (ws.Cells[rowIndex, 0].Value != null)
            {
                string courseCode = GetCellValue(ws.Cells[rowIndex, 1].Value);
                string classCode = GetCellValue(ws.Cells[rowIndex, 2].Value);
                if (classCode.Length > 2)
                    classCode = classCode.Substring(1, 2);
                string credit = GetCellValue(ws.Cells[rowIndex, 3].Value);
                string courseName = GetCellValue(ws.Cells[rowIndex, 4].Value);
                string semester = GetCellValue(ws.Cells[rowIndex, 5].Value);
                string schoolyear = GetCellValue(ws.Cells[rowIndex, 6].Value);

                string subjID = "0";
                if (!this.dicSubjects.ContainsKey(courseCode))
                    this.addMsg( string.Format("匯入課程時找不到 subject ID, code : {0}, class name :{1} , course name : {2}, schoolyear: {3}, semester :{4}  ",
                                                                                                                    courseCode, classCode, courseName, schoolyear, semester));
                else
                {
                    subjID = this.dicSubjects[courseCode].UID;

                    string key = string.Format("{0}_{1}", courseCode, classCode);
                    //檢查資料庫中是否已有存在？如果存再就修改，否則新增。
                    bool isCourseExisted = false;
                    if (this.dicCourseExts.ContainsKey(key))
                    {
                        UDT.CourseExt cExt = this.dicCourseExts[key];
                        if (this.dicCourses.ContainsKey(cExt.CourseID.ToString()))
                            isCourseExisted = true;
                    }

                    if (isCourseExisted)
                    {
                        UDT.CourseExt cExt = this.dicCourseExts[key];
                        cExt.ClassName = classCode;
                        cExt.SubjectCode = courseCode;
                        cExt.SubjectID = int.Parse(subjID);
                        List<ActiveRecord> recs = new List<ActiveRecord>();
                        recs.Add(cExt);
                        (new AccessHelper()).UpdateValues(recs);

                        SHCourseRecord course = this.dicCourses[cExt.CourseID.ToString()];
                        course.Credit = decimal.Parse(credit);
                        course.Name = string.Format("{0} {1}", courseName, classCode);
                        course.SchoolYear = int.Parse(schoolyear);
                        course.Semester = int.Parse(semester);
                        SHCourse.Update(course);
                    }
                    else
                    {
                        SHCourseRecord course = new SHCourseRecord();
                        course.Credit = decimal.Parse(credit);
                        course.Name = string.Format("{0} {1}", courseName, classCode);
                        course.SchoolYear = int.Parse(schoolyear);
                        course.Semester = int.Parse(semester);
                        string newID = SHCourse.Insert(course);

                        UDT.CourseExt cExt = new UDT.CourseExt();
                        cExt.CourseID = int.Parse(newID);
                        cExt.ClassName = classCode;
                        cExt.SubjectCode = courseCode;
                        cExt.SubjectID = int.Parse(subjID);
                        List<ActiveRecord> recs = new List<ActiveRecord>();
                        recs.Add(cExt);
                        (new AccessHelper()).InsertValues(recs);

                    }
                } //if find subject id
                rowIndex += 1;
                this.lblStatus.Text = rowIndex.ToString() ;
                Application.DoEvents();

            } // while loop


            //匯入修課學生 
            this.ImportSCAttendRecords();

        }


        private void ImportSCAttendRecords()
        {
            this.addMsg(" ==== 開始匯入修課學生 ===");

            /*  取得目前所有的修課紀錄 */
            List<UDT.SCAttendExt> attRecs = (new AccessHelper()).Select<UDT.SCAttendExt>();
            Dictionary<string, Dictionary<string, UDT.SCAttendExt>> dicAttRecs = new Dictionary<string, Dictionary<string, UDT.SCAttendExt>>();
            foreach (UDT.SCAttendExt att in attRecs)
            {
                if (!dicAttRecs.ContainsKey(att.CourseID.ToString()))
                    dicAttRecs.Add(att.CourseID.ToString(), new Dictionary<string, UDT.SCAttendExt>());

                dicAttRecs[att.CourseID.ToString()].Add(att.StudentID.ToString(), att);
            }

            /* 取得所有課程 ，以便從課程代碼 及班及名稱，找出 課程系統編號 */
            this.GetAllCourses();

            List<UDT.CourseExt> allCourses = (new AccessHelper()).Select<UDT.CourseExt>();
            Dictionary<string, UDT.CourseExt> dicAllCourses = new Dictionary<string, UDT.CourseExt>();
            foreach (UDT.CourseExt course in allCourses)
            {
                if (this.dicCourses.ContainsKey(course.CourseID.ToString()))
                {
                    string key = string.Format("{0}_{1}", course.SubjectCode, course.ClassName);
                    dicAllCourses.Add(key, course);
                }
            }

            /* 取得所有學生資料，以便從學號找出學生編號  */
            List<K12.Data.StudentRecord> allStudents = K12.Data.Student.SelectAll();
            Dictionary<string, K12.Data.StudentRecord> dicAllStudents = new Dictionary<string, K12.Data.StudentRecord>();
            foreach (K12.Data.StudentRecord stud in allStudents)
            {
                if (!string.IsNullOrWhiteSpace(stud.StudentNumber))
                    dicAllStudents.Add(stud.StudentNumber, stud);
            }

            /* 讀取 Excel 資料  */
             Workbook wb = new Aspose.Cells.Workbook();
            wb.Open(this.textBoxX1.Text);
            Worksheet ws = wb.Worksheets[0];   //修課紀錄
            int rowIndex = 1;
            while (ws.Cells[rowIndex, 3].Value != null)
            {
                string studNo = GetCellValue(ws.Cells[rowIndex, 3].Value);
                if (!dicAllStudents.ContainsKey(studNo))
                {
                    this.addMsg(string.Format("找不到學生，學號：{0}, rowNo: {1}  ", studNo, rowIndex.ToString()));
                }
                else
                {
                    string studID = dicAllStudents[studNo].ID ;
                    string courseCode = GetCellValue(ws.Cells[rowIndex, 7].Value);
                    string classCode = GetCellValue(ws.Cells[rowIndex, 8].Value);
                    if (classCode.Length > 2)
                        classCode = classCode.Substring(1, 2);
                    string key = string.Format("{0}_{1}", courseCode, classCode);
                    if (!dicAllCourses.ContainsKey(key))
                    {
                        this.addMsg(string.Format("找不到課程，課號：{0}, 班及：{1},  rowNo: {2}  ", courseCode, classCode,  rowIndex.ToString()));
                    }
                    else
                    {
                        string courseID = dicAllCourses[key].CourseID.ToString(); 
                        //判斷該生是否已經修課，若是，則 skip ，否則新增 !
                        if (dicAttRecs.ContainsKey(courseID) && dicAttRecs[courseID].ContainsKey(studID))
                        {
                            //do nothing
                            string msg = string.Format("學號：{0}  已修課程： 課號 = {1}, 班號 = {2},  rowindex ={3}，故忽略不匯入！", studNo, courseCode, classCode, rowIndex.ToString());
                            this.addMsg(msg);
                        }
                        else
                        {
                            // 新增修課紀錄
                            
                            UDT.SCAttendExt attRec = new UDT.SCAttendExt();
                            attRec.StudentID = int.Parse(studID);
                            attRec.CourseID = int.Parse(courseID);
                            List<ActiveRecord> recs = new List<ActiveRecord>();
                            recs.Add(attRec);
                            (new AccessHelper()).InsertValues( recs);                            
                        }
                    }
                }
                rowIndex += 1;
                this.lblStatus.Text = rowIndex.ToString();
                Application.DoEvents();
            }


        }

        private void addMsg(string msg)
        {
            this.textBoxX2.Text += string.Format("{0}  \n", msg);
            Application.DoEvents();
        }

        private string GetCellValue(object cellValue)
        {
            return (cellValue == null) ? "" : cellValue.ToString().Trim();
        }
        private void GetAllSubjects()
        {
            List<UDT.Subject> subjects = (new AccessHelper()).Select<UDT.Subject>();
            this.dicSubjects = new Dictionary<string, UDT.Subject>();
            foreach (UDT.Subject subj in subjects)
            {
                this.dicSubjects.Add(subj.SubjectCode, subj);
            }
        }

        private void GetAllCourses()
        {
            this.dicCourses = new Dictionary<string, SHCourseRecord>();
            List<SHSchool.Data.SHCourseRecord> courses = SHSchool.Data.SHCourse.SelectBySchoolYearAndSemester((int)nudSchoolYear.Value, (int)nudSemester.Value);
            foreach (SHCourseRecord c in courses)
                this.dicCourses.Add(c.ID, c);


            List<UDT.CourseExt> courseExts = (new AccessHelper()).Select<UDT.CourseExt>();
            //Dictionary<string, UDT.CourseExt> dicCourseExt = new Dictionary<string, UDT.CourseExt>();
            this.dicCourseExts = new Dictionary<string,UDT.CourseExt>();
            foreach (UDT.CourseExt cExt in courseExts)
            {
                if (this.dicCourses.ContainsKey(cExt.CourseID.ToString()))
                    dicCourseExts.Add(string.Format("{0}_{1}", cExt.SubjectCode.Trim(), cExt.ClassName.Trim()), cExt);
            }
        }


    }
}
