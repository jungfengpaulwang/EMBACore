using System.Collections.Generic;
using System.Data;
using System.Linq;
using EMBA.DocumentValidator;
using EMBA.Import;
using EMBA.Validator;
using EMBACore.UDT;
using FISCA.Data;
using FISCA.UDT;
using K12.Data;
using System;
using System.Xml.Linq;

namespace EMBACore.Import
{
    class SubjectSemesterScore_Import : ImportWizard
    {
        ImportOption mOption;
        string keyField;
        AccessHelper Access;
        QueryHelper queryHelper;
        List<DataRow> lstSubjects;
        DataSet dataSet;
        Dictionary<string, string> dicStudent_Numbers;
        Dictionary<string, string> dicSubjectIDs;
        Dictionary<string, string> dicClass_Names;
        public SubjectSemesterScore_Import()
        {
            this.IsSplit = false;
            this.ShowAdvancedForm = false;
            this.ValidateRuleFormater = XDocument.Parse(EMBACore.Properties.Resources.format);
            //this.CustomValidate = null;
            //this.SplitThreadCount = 5;
            //this.SplitSize = 3000;

            Access = new AccessHelper();
            queryHelper = new QueryHelper();
            keyField = string.Empty;
            lstSubjects = new List<DataRow>();
            dataSet = new DataSet();
            dicStudent_Numbers = new Dictionary<string, string>();
            dicSubjectIDs = new Dictionary<string, string>();
            dicClass_Names = new Dictionary<string, string>();
            this.CustomValidate = (Rows, Messages) =>
            {
                CustomValidator(Rows, Messages);
            };
        }

        public void CustomValidator(List<IRowStream> Rows, RowMessages Messages)
        {
            keyField = string.Empty;
            lstSubjects = new List<DataRow>();
            dataSet = new DataSet();
            dicStudent_Numbers = new Dictionary<string, string>();
            dicSubjectIDs = new Dictionary<string, string>();
            dicClass_Names = new Dictionary<string, string>();

            string strSQL = string.Empty;
            QueryHelper queryHelper = new QueryHelper();

            DataTable dataTable;

            //  科目(課程)
            strSQL = string.Format(@"select uid, name, subject_code, dept_name, dept_code, credit, is_required, new_subject_code from $ischool.emba.subject;");

            dataTable = queryHelper.Select(strSQL);
            dataTable.TableName = "科目";
            dataSet.Tables.Add(dataTable);

            //  科目(課程)學期成績
            strSQL = string.Format(@"select uid, ref_student_id, ref_course_id, school_year, semester, score, ref_subject_id, subject_name, subject_code, new_subject_code, credit, is_required, is_pass, offset_course, remark from $ischool.emba.subject_semester_score;");

            dataTable = queryHelper.Select(strSQL);
            dataTable.TableName = "科目學期成績";
            dataSet.Tables.Add(dataTable);

            //  開課
            strSQL = string.Format(@"select id, course_name, school_year, semester, credit from course;");

            dataTable = queryHelper.Select(strSQL);
            dataTable.TableName = "開課";
            dataSet.Tables.Add(dataTable);

            //  開課延伸資料
            strSQL = string.Format(@"select uid, ref_course_id, subject_code, new_subject_code, course_type, is_required, ref_subject_id, serial_no, class_name, score_confirmed from $ischool.emba.course_ext;");

            dataTable = queryHelper.Select(strSQL);
            dataTable.TableName = "開課延伸資料";
            dataSet.Tables.Add(dataTable);

            //  學生
            strSQL = string.Format(@"select id, name, id_number, ref_class_id, student_number, seat_no, status  from student;");

            dataTable = queryHelper.Select(strSQL);
            dataTable.TableName = "學生";
            dataSet.Tables.Add(dataTable);

            //  學生修課
            strSQL = string.Format(@"select ref_student_id, ref_course_id from $ischool.emba.scattend_ext;");

            dataTable = queryHelper.Select(strSQL);
            dataTable.TableName = "學生修課";
            dataSet.Tables.Add(dataTable);

            #region 1、無現存科目資料，則中斷驗證、中斷匯入。

            if (dataSet.Tables["科目"].Rows.Count == 0)
                throw new Exception("無課程總檔資料，無法匯入課程學期成績資料！");

            #endregion

            #region 2、無現存學生資料，則中斷驗證、中斷匯入。

            if (dataSet.Tables["學生"].Rows.Count == 0)
                throw new Exception("無學生資料，無法匯入課程學期成績資料！");

            #endregion

            #region 3、判讀鍵值為「課號」或「課程識別碼」

            if (this.SelectedKeyFields.Contains("課號")) keyField = "課號";
            if (this.SelectedKeyFields.Contains("課程識別碼")) keyField = "課程識別碼";

            #endregion

            #region 4、「學號」反查「學生系統編號」索引

            dataSet.Tables["學生"].Rows.Cast<DataRow>().ToList().ForEach((x) =>
            {
                if (!dicStudent_Numbers.ContainsKey((x["student_number"] + "").Trim().ToUpper()))
                    dicStudent_Numbers.Add((x["student_number"] + "").Trim().ToUpper(), x["id"] + "");
            });

            #endregion

            #region 5、「課號」或「課程識別碼」反查「科目(課程)系統編號」索引
            lstSubjects = dataSet.Tables["科目"].Rows.Cast<DataRow>().ToList();
            lstSubjects.ForEach((x) =>
            {
                string key = string.Empty;

                if (keyField == "課號")
                    key = (x["new_subject_code"] + "").Trim().ToUpper();

                if (keyField == "課程識別碼")
                    key = (x["subject_code"] + "").Trim().ToUpper();

                if (!dicSubjectIDs.ContainsKey(key))
                    dicSubjectIDs.Add(key, x["uid"] + "");
            });

            #endregion

            #region 6、鍵值必須存在、學號必須存在、以「學號」及「課號 或 課程識別碼」反查之開課系統編號必須為唯一

            Rows.ForEach((x) =>
            {
                string subject_code = x.GetValue("課程識別碼").Trim();
                string new_subject_code = x.GetValue("課號").Trim();
                string subject_name = x.GetValue("課程名稱").Trim();
                string school_year = x.GetValue("學年度").Trim();
                string semester = x.GetValue("學期").Trim();
                string student_number = x.GetValue("學號").Trim();
                string class_name = x.GetValue("班次").Trim();

                if (keyField == "課程識別碼")
                {
                    //  若鍵值為「課程識別碼」，則必須存在於系統中
                    IEnumerable<DataRow> filterDataRows = lstSubjects.Where(y => (y["subject_code"] + "").Trim().ToUpper() == subject_code.ToUpper());
                    if (filterDataRows.Count() == 0)
                        Messages[x.Position].MessageItems.Add(new MessageItem(EMBA.Validator.ErrorType.Error, EMBA.Validator.ValidatorType.Row, "系統不存在此課程識別碼。"));

                    if (!IsValidCourseID(subject_code, dicStudent_Numbers[student_number.ToUpper()]))
                        Messages[x.Position].MessageItems.Add(new MessageItem(EMBA.Validator.ErrorType.Error, EMBA.Validator.ValidatorType.Row, "以「學號」及「課程識別碼」反查出多筆「班次」資料，請查明是否學生重覆修習相同課程。"));
                }

                if (keyField == "課號")
                {
                    //  若鍵值為「課號」，則必須存在於系統中
                    IEnumerable<DataRow> filterDataRows = lstSubjects.Where(y => (y["new_subject_code"] + "").Trim().ToUpper() == new_subject_code.ToUpper());
                    if (filterDataRows.Count() == 0)
                        Messages[x.Position].MessageItems.Add(new MessageItem(EMBA.Validator.ErrorType.Error, EMBA.Validator.ValidatorType.Row, "系統不存在此課號。"));

                    if (!IsValidCourseID(new_subject_code, dicStudent_Numbers[student_number.ToUpper()]))
                        Messages[x.Position].MessageItems.Add(new MessageItem(EMBA.Validator.ErrorType.Error, EMBA.Validator.ValidatorType.Row, "以「學號」及「課號」反查出多筆「班次」資料，請查明是否學生重覆修習相同課程。"));
                }

                //  學號必須存在於系統中
                if (!dicStudent_Numbers.ContainsKey(student_number.ToUpper()))
                    Messages[x.Position].MessageItems.Add(new MessageItem(EMBA.Validator.ErrorType.Error, EMBA.Validator.ValidatorType.Row, "系統無此學號。"));
            });

            #endregion

        }

        private bool IsValidCourseID(string code, string student_id)
        {
            DataTable dataTableSCAttend = dataSet.Tables["學生修課"];
            DataTable dataTableCourseExt = dataSet.Tables["開課延伸資料"];

            List<DataRow> filterSCAttends = dataTableSCAttend.Rows.Cast<DataRow>().ToList();
            List<DataRow> filterCourseExt = dataTableCourseExt.Rows.Cast<DataRow>().ToList();

            filterSCAttends = filterSCAttends.Where(x => (x["ref_student_id"] + "") == student_id).ToList();

            if (filterSCAttends.Count == 0)
                return true;

            foreach (DataRow xRow in filterCourseExt)
            {
                //  ref_course_id, subject_code, new_subject_code
                if ((keyField == "課號") && ((xRow["new_subject_code"] + "") != code))
                    continue;

                if ((keyField == "課程識別碼") && ((xRow["subject_code"] + "") != code))
                    continue;

                foreach (DataRow yRow in filterSCAttends)
                {
                    if ((yRow["ref_course_id"] + "") == (xRow["ref_course_id"] + ""))
                    {
                        if (dicClass_Names.ContainsKey(student_id + "_" + code))
                            return false;
                        else
                            dicClass_Names.Add(student_id + "_" + code, xRow["ref_course_id"] + "");
                    }
                }
            }

            return true;
        }

        public override XDocument GetValidateRule()
        {
            return XDocument.Parse(EMBACore.Properties.Resources.SubjectSemesterScore_Import);
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
            IEnumerable<SubjectSemesterScore> allSubjectSemesterScoreRecords = Access.Select<SubjectSemesterScore>();
            //  要新增的 SubjectSemesterScoreRecord 
            List<SubjectSemesterScore> insertRecords = new List<SubjectSemesterScore>();
            //  要更新的 SubjectSemesterScoreRecord
            List<SubjectSemesterScore> updateRecords = new List<SubjectSemesterScore>();
            foreach (IRowStream row in Rows)
            {
                SubjectSemesterScore record = new SubjectSemesterScore();

                string subject_code = row.GetValue("課程識別碼").Trim();
                string new_subject_code = row.GetValue("課號").Trim();
                string subject_name = row.GetValue("課程名稱").Trim();
                string school_year = row.GetValue("學年度").Trim();
                string semester = row.GetValue("學期").Trim();
                string student_number = row.GetValue("學號").Trim();
                string class_name = string.Empty;
                string course_id = string.Empty;

                string studentID = dicStudent_Numbers[student_number.ToUpper()];
                IEnumerable<SubjectSemesterScore> filterRecords = new List<SubjectSemesterScore>();
                if (keyField == "課程識別碼")
                {
                    //  鍵值為：學年度+學期+學號+課程識別碼
                    filterRecords = allSubjectSemesterScoreRecords.Where(x => (x.SchoolYear.HasValue ? x.SchoolYear.Value.ToString() : "").Trim() == (string.IsNullOrEmpty(school_year) ? "" : int.Parse(school_year).ToString())).Where(x => (x.Semester.HasValue ? x.Semester.ToString() : "").Trim() == (string.IsNullOrEmpty(semester) ? "" : int.Parse(semester).ToString())).Where(x => x.SubjectCode.Trim().ToUpper() == subject_code.ToUpper()).Where(x => x.StudentID.ToString() == dicStudent_Numbers[student_number.ToUpper()]);
                }
                else
                {
                    //  鍵值為：學年度+學期+學號+課號
                    filterRecords = allSubjectSemesterScoreRecords.Where(x => (x.SchoolYear.HasValue ? x.SchoolYear.Value.ToString() : "").Trim() == (string.IsNullOrEmpty(school_year) ? "" : int.Parse(school_year).ToString())).Where(x => (x.Semester.HasValue ? x.Semester.ToString() : "").Trim() == (string.IsNullOrEmpty(semester) ? "" : int.Parse(semester).ToString())).Where(x => x.NewSubjectCode.Trim().ToUpper() == new_subject_code.ToUpper()).Where(x => x.StudentID.ToString() == dicStudent_Numbers[student_number.ToUpper()]);
                }
                if (filterRecords.Count() > 0)
                    record = filterRecords.ElementAt(0);

                if (mOption.SelectedFields.Contains("等第成績") && !string.IsNullOrWhiteSpace(row.GetValue("等第成績")))
                    record.Score = row.GetValue("等第成績").Trim();

                int intSchoolyear = 0;
                int.TryParse(school_year, out intSchoolyear);
                if (!string.IsNullOrEmpty(school_year))
                    record.SchoolYear = intSchoolyear;
                int intSemester = 0;
                int.TryParse(semester, out intSemester);
                if (!string.IsNullOrEmpty(semester))
                    record.Semester = intSemester;

                //  「開課」系統編號暫不寫入
                //record.CourseID = int.Parse(filterDataRows.ElementAt(0)["course_id"] + "");

                if (mOption.SelectedFields.Contains("學分數") && !string.IsNullOrWhiteSpace(row.GetValue("學分數")))
                    record.Credit = int.Parse(row.GetValue("學分數").Trim());

                if (mOption.SelectedFields.Contains("取得學分") && !string.IsNullOrWhiteSpace(row.GetValue("取得學分")))
                    record.IsPass = (row.GetValue("取得學分").Trim() == "是" ? true : false);

                if (mOption.SelectedFields.Contains("必選修") && !string.IsNullOrWhiteSpace(row.GetValue("必選修")))
                    record.IsRequired = (row.GetValue("必選修") == "必修" ? true : false);

                if (keyField == "課號")
                {
                    record.NewSubjectCode = new_subject_code;

                    if (mOption.SelectedFields.Contains("課程識別碼") && !string.IsNullOrEmpty(subject_code))
                        record.SubjectCode = subject_code;
                }
                else
                {
                    record.SubjectCode = subject_code;

                    if (mOption.SelectedFields.Contains("課號") && !string.IsNullOrEmpty(new_subject_code))
                        record.NewSubjectCode = new_subject_code;
                }

                if (mOption.SelectedFields.Contains("備註") && !string.IsNullOrWhiteSpace(row.GetValue("備註")))
                    record.Remark = row.GetValue("備註").Trim();

                if (mOption.SelectedFields.Contains("抵免課程") && !string.IsNullOrWhiteSpace(row.GetValue("抵免課程")))
                    record.OffsetCourse = row.GetValue("抵免課程").Trim();

                if (!string.IsNullOrEmpty(course_id))
                    record.CourseID = int.Parse(course_id);

                record.StudentID = int.Parse(studentID);
                record.SubjectID = int.Parse(dicSubjectIDs[(keyField == "課程識別碼" ? subject_code : new_subject_code)]);

                if (mOption.SelectedFields.Contains("課程名稱") && !string.IsNullOrEmpty(subject_name))
                    record.SubjectName = subject_name;

                if (dicClass_Names.ContainsKey(studentID + "_" + (keyField == "課程識別碼" ? subject_code : new_subject_code)))
                    record.CourseID = int.Parse(dicClass_Names[studentID + "_" + (keyField == "課程識別碼" ? subject_code : new_subject_code)]);

                if (record.RecordStatus == RecordStatus.Insert)
                    insertRecords.Add(record);
                if (record.RecordStatus == RecordStatus.Update)
                    updateRecords.Add(record);
            }
            List<string> insertedRecordIDs = new List<string>();
            try
            {
                insertedRecordIDs = insertRecords.SaveAll();
            }
            catch (System.Exception e)
            {
                System.Windows.Forms.MessageBox.Show(e.Message);
                return e.Message;
            }
            List<string> updatedRecordIDs = new List<string>();
            try
            {
                updatedRecordIDs = updateRecords.SaveAll();
            }
            catch (System.Exception e)
            {
                System.Windows.Forms.MessageBox.Show(e.Message);
                return e.Message;
            }    
            //  RaiseEvent
            if (insertedRecordIDs.Count > 0 || updateRecords.Count > 0)
            {
                IEnumerable<string> uids = insertedRecordIDs.Union(updatedRecordIDs);
                UDT.SubjectSemesterScore.RaiseAfterUpdateEvent(this, new ParameterEventArgs(uids));
            }
            return string.Empty;
        }
    }
}