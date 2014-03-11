using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using EMBA.Import;
using K12.Data;
using EMBACore.UDT;
using FISCA.UDT;
using EMBA.DocumentValidator;
using EMBA.Validator;
using System.Data;
using FISCA.Data;
using System.Xml.Linq;
using EMBACore.Extension.UDT;

namespace EMBACore.Import
{
    class SCAttendExt_Import : ImportWizard
    {
        ImportOption mOption;
        IEnumerable<DataRow> students;
        IEnumerable<DataRow> courses;
        DataTable dataTableStudent;
        DataTable dataTableCourse;
        Dictionary<string, string> studentsMapping;
        Dictionary<string, string> coursesMapping;

        public SCAttendExt_Import()
        {
            this.IsSplit = false;
            this.ShowAdvancedForm = false;
            this.ValidateRuleFormater = XDocument.Parse(EMBACore.Properties.Resources.format);
            //this.CustomValidate = null;
            //this.SplitThreadCount = 5;
            //this.SplitSize = 3000;

            this.CustomValidate = (Rows, Messages) =>
            {
                CustomValidator(Rows, Messages);
            };
        }

        public void CustomValidator(List<IRowStream> Rows, RowMessages Messages)
        {
            #  region 驗證流程

            QueryHelper queryHelper = new QueryHelper();
            dataTableStudent = new DataTable();
            dataTableCourse = new DataTable();

            List<KeyValuePair<int, string>> student_Number = new List<KeyValuePair<int, string>>();
            List<KeyValuePair<string, string>> schoolYearSemester = new List<KeyValuePair<string, string>>();

            string strSQL = string.Empty;

            //  抽出匯入資料之「學號」與「學年度、學期」，便於取得「學生」與「開課」資料。
            Rows.ForEach((x) =>
            {
                student_Number.Add(new KeyValuePair<int, string>(x.Position, x.GetValue("學號").Trim().ToUpper()));

                string schoolYear = x.GetValue("學年度").Trim();
                string semester = x.GetValue("學期").Trim();

                if (schoolYearSemester.Where(y => (y.Key == schoolYear && y.Value == semester)).Count() == 0)
                    schoolYearSemester.Add(new KeyValuePair<string, string>(schoolYear, semester));
            });

            //  學生資料
            strSQL = string.Format(@"select student.id 學生系統編號, student.student_number 學號, student.status 學生狀態 from student where student.student_number in ({0})", String.Join(",", student_Number.Select(x => ("'" + x.Value + "'"))));
            dataTableStudent = queryHelper.Select(strSQL);

            //  開課資料
            strSQL = string.Format(@"select c.id 課程系統編號, c.course_name 開課, c.school_year 學年度, c.semester 學期 from course c where c.school_year in ({0}) and c.semester in ({1})", String.Join(",", schoolYearSemester.Select(x => ("'" + x.Key.Trim() + "'"))), String.Join(",", schoolYearSemester.Select(x => ("'" + x.Value.Trim() + "'"))));
            dataTableCourse = queryHelper.Select(strSQL);

            students = dataTableStudent.Rows.Cast<DataRow>();
            courses = dataTableCourse.Rows.Cast<DataRow>();

            studentsMapping = new Dictionary<string, string>();
            //List<string> selectedKeyFields = this.SelectedKeyFields;
            //  1、學號必須存在於系統
            foreach (KeyValuePair<int, string> kv in student_Number)
            {
                if (students.Where(x => (x["學號"] + "").Trim().ToUpper() == kv.Value.Trim().ToUpper()).Count() == 0)
                    Messages[kv.Key].MessageItems.Add(new MessageItem(EMBA.Validator.ErrorType.Error, EMBA.Validator.ValidatorType.Row, "系統無此學號。"));
            }

            //  2、學生狀態必須為一般生(若狀態不為「一般生」，則驗證為「Error」)
            //foreach (KeyValuePair<int, string> kv in student_Number)
            //{
            //    foreach (DataRow row in students.Where(x => (x["學生狀態"] + "") != "1").Where(x => (x["學號"] + "").Trim() == kv.Value.Trim()).Select(x => x))
            //    {
            //        Messages[kv.Key].MessageItems.Add(new MessageItem(EMBA.Validator.ErrorType.Error, EMBA.Validator.ValidatorType.Row, "學生狀態非「在學」。"));
            //    }
            //}

            //  3、「開課」必須存在於系統
            foreach (IRowStream row in Rows)
            {
                if (courses.Where(x => ((x["開課"] + "").Trim().ToUpper() == row.GetValue("開課").Trim().ToUpper() && ((x["學年度"] + "").Trim() == row.GetValue("學年度").Trim()) && ((x["學期"] + "").Trim() == row.GetValue("學期").Trim()))).Count() == 0)
                    Messages[row.Position].MessageItems.Add(new MessageItem(EMBA.Validator.ErrorType.Error, EMBA.Validator.ValidatorType.Row, "系統無此開課。"));
            }

            #endregion
        }

        public override XDocument GetValidateRule()
        {
            return XDocument.Parse(EMBACore.Properties.Resources.SCAttendExt_Import);
        }

        public override ImportAction GetSupportActions()
        {
            return ImportAction.InsertOrUpdate;
        }

        public override void Prepare(ImportOption Option)
        {
            mOption = Option;
        }

        public override string Import(List<EMBA.DocumentValidator.IRowStream> Rows)
        {
            Dictionary<string, int> dicImportInfos = new Dictionary<string, int>();
            List<SCAttendExt> records = new List<SCAttendExt>();
            List<SCAttendExt> insertRecords = new List<SCAttendExt>();
            List<SCAttendExt> updateRecords = new List<SCAttendExt>();
            string message = string.Empty;

            //  為學生加入學號索引
            studentsMapping = new Dictionary<string, string>();
            foreach (DataRow row in students)
                if (!studentsMapping.ContainsKey((row["學號"] + "").Trim().ToUpper()))
                    studentsMapping.Add((row["學號"] + "").Trim().ToUpper(), (row["學生系統編號"] + "").Trim());

            //  為課程加入「開課」索引
            coursesMapping = new Dictionary<string, string>();
            foreach (DataRow row in courses)
            {
                string schoolYear = (row["學年度"] + "").Trim();
                string semester = (row["學期"] + "").Trim();
                string course = (row["開課"] + "").Trim().ToUpper();
                string key = course + "_" + schoolYear + "_" + semester;
                if (!coursesMapping.ContainsKey(key))
                    coursesMapping.Add(key, row["課程系統編號"] + "");
            }

            AccessHelper Access = new AccessHelper();
            string studentIDs = String.Join(",", students.Select(x => ("'" + studentsMapping[(x["學號"] + "").Trim().ToUpper()] + "'")));
            string courseIDs = string.Empty;

            courseIDs = String.Join(",", courses.Select(x => ("'" + coursesMapping[((x["開課"] + "").Trim().ToUpper() + "_" + (x["學年度"] + "").Trim() + "_" + (x["學期"] + "").Trim()).Trim()] + "'")));

            records = Access.Select<SCAttendExt>(string.Format("ref_student_id in ({0}) and ref_course_id in ({1})", studentIDs, courseIDs));
            Dictionary<string, SCAttendExt> dicSCAttendExts = new Dictionary<string, SCAttendExt>();
            if (records.Count > 0)
                dicSCAttendExts = records.ToDictionary(x => x.StudentID + "-" + x.CourseID);
            foreach (IRowStream row in Rows)
            {
                SCAttendExt record = new SCAttendExt();

                string schoolYear = row.GetValue("學年度").Trim();
                string semester = row.GetValue("學期").Trim();
                string course = row.GetValue("開課").Trim().ToUpper();
                string key = course + "_" + schoolYear + "_" + semester;
                string studentID = studentsMapping[row.GetValue("學號").Trim().ToUpper()];
                string courseID = coursesMapping[key];
                string originalReportGroup = string.Empty;

                if (dicSCAttendExts.ContainsKey(studentID + "-" + courseID))
                    record = dicSCAttendExts[studentID + "-" + courseID];

                record.CourseID = int.Parse(courseID);
                record.StudentID = int.Parse(studentID);

                if (mOption.SelectedFields.Contains("報告小組") && !string.IsNullOrEmpty(row.GetValue("報告小組").Trim()))
                {
                    record.Group = row.GetValue("報告小組").Trim();
                }
                if (mOption.SelectedFields.Contains("停修"))
                    record.IsCancel = (row.GetValue("停修").Trim() == "是" ? true : false);
                if (!dicSCAttendExts.ContainsKey(studentID + "-" + courseID))
                    records.Add(record);
            }
            
            //  匯入+Log 
            List<string> updatedCourseExtIDs = new List<string>();

            try
            {
                updatedCourseExtIDs = records.SaveAllWithLog("課程.匯入.修課學生", "", "", Log.LogTargetCategory.Course, "開課系統編號");
            }
            catch (System.Exception e)
            {
                System.Windows.Forms.MessageBox.Show(e.Message);
                return e.Message;
            }
            ////  新增 
            //List<string> insertedCourseExtIDs = updateRecords.SaveAll();

            //  RaiseEvent
            if (updatedCourseExtIDs.Count > 0)
            {
                SCAttendExt.RaiseAfterUpdateEvent(this, new ParameterEventArgs(updatedCourseExtIDs));
            }

            return message;
        }
    }
}