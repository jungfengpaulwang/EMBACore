using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Xml.Linq;
using EMBA.DocumentValidator;
using EMBA.Import;
using EMBA.Validator;
using EMBACore.DataItems;
using EMBACore.UDT;
using FISCA.Data;
using FISCA.UDT;
using K12.Data;
using System;

namespace EMBACore.Import
{
    class Course_Import : ImportWizard
    {
        ImportOption mOption;
        string keyField;
        AccessHelper Access;
        QueryHelper queryHelper;
        public Course_Import()
        {
            this.IsSplit = false;
            this.ShowAdvancedForm = false;
            this.ValidateRuleFormater = XDocument.Parse(EMBACore.Properties.Resources.format);
            //this.CustomValidate = null;
            //this.SplitThreadCount = 5;
            //this.SplitSize = 3000;

            keyField = string.Empty;
            Access = new AccessHelper();
            queryHelper = new QueryHelper();
            
            this.CustomValidate = (Rows, Messages) =>
            {
                CustomValidator(Rows, Messages);
            };
        }

        public void CustomValidator(List<IRowStream> Rows, RowMessages Messages)
        {
            #  region 驗證流程
            List<UDT.CourseTypeDataSource> CourseTypeDataSources = Access.Select<UDT.CourseTypeDataSource>();
            List<string> CourseTypes = new List<string>();
            foreach (UDT.CourseTypeDataSource CourseTypeDataSource in CourseTypeDataSources)
            {
                CourseTypes.Add(CourseTypeDataSource.CourseType);
            }
            CourseTypes = CourseTypes.Distinct().ToList();
            if (this.SelectedKeyFields.Contains("開課系統編號"))
                keyField = "開課系統編號";
            else
                keyField = "學年度+學期+開課";

            //  若「課程識別碼」不為空白，則必須存在於資料庫中。
            //  若「課號」不為空白，則必須存在於資料庫中。
            //DataTable dataTableSubjects = queryHelper.Select("select subject_code, new_subject_code, name from $ischool.emba.subject");
            DataTable dataTableCourses = queryHelper.Select("select id from course");
            //IEnumerable<DataRow> subjects = dataTableSubjects.Rows.Cast<DataRow>();
            IEnumerable<DataRow> courses = dataTableCourses.Rows.Cast<DataRow>();
            List<string> Courses = new List<string>();
            if (courses.Count() > 0)
                courses.ToList().ForEach(x => Courses.Add(x["id"] + ""));

            Rows.ForEach((x) =>
            {
                string subject_code = x.GetValue("課程識別碼").Trim().ToUpper();
                string new_subject_code = x.GetValue("課號").Trim().ToUpper();
                string course_id = x.GetValue("開課系統編號").Trim();
                string class_name = x.GetValue("開課班次").Trim();
                //string subject_name = x.GetValue("開課課程").Trim().ToUpper();
                //IEnumerable<DataRow> filterSubjects = new List<DataRow>();

                //filterSubjects = subjects.Where(y => (y["subject_code"] + "").ToUpper() == x.GetValue("課程識別碼").Trim().ToUpper());
                //if (filterSubjects.Count() == 0)
                //    filterSubjects = subjects.Where(y => (y["new_subject_code"] + "").ToUpper() == x.GetValue("課號").Trim().ToUpper());

                //if (filterSubjects.Count() > 0)
                //    if (subject_name != (filterSubjects.ElementAt(0)["name"] + "").Trim())
                //        Messages[x.Position].MessageItems.Add(new MessageItem(EMBA.Validator.ErrorType.Error, EMBA.Validator.ValidatorType.Row, "由「課程識別碼」或「課號」反查「開課課程」錯誤。"));

                //  「課程類別」必須存在
                if (this.SelectedFields.Contains("類別") && !string.IsNullOrWhiteSpace(x.GetValue("類別")))
                {
                    if (!CourseTypes.Contains(x.GetValue("類別").Trim()))
                        Messages[x.Position].MessageItems.Add(new MessageItem(EMBA.Validator.ErrorType.Error, EMBA.Validator.ValidatorType.Row, "「類別」不存在，請先至「課程類別管理」建立「課程類別」內容。"));
                }

                if (string.IsNullOrEmpty(subject_code) && string.IsNullOrEmpty(new_subject_code))
                    Messages[x.Position].MessageItems.Add(new MessageItem(EMBA.Validator.ErrorType.Error, EMBA.Validator.ValidatorType.Row, "「課程識別碼」與「課號」至少有一個不得為空白。"));

                //if (!string.IsNullOrEmpty(subject_code))
                //{
                //    if (subjects.Where(y=>(y["subject_code"] + "").Trim() == subject_code).Count() == 0)
                //        Messages[x.Position].MessageItems.Add(new MessageItem(EMBA.Validator.ErrorType.Error, EMBA.Validator.ValidatorType.Row, "系統無此課程識別碼。"));
                //}

                //if (!string.IsNullOrEmpty(new_subject_code))
                //{
                //    if (subjects.Where(y => (y["new_subject_code"] + "").Trim() == new_subject_code).Count() == 0)
                //        Messages[x.Position].MessageItems.Add(new MessageItem(EMBA.Validator.ErrorType.Error, EMBA.Validator.ValidatorType.Row, "系統無此課號。"));
                //}
                //「開課班次」只允許：空白、01、02、03。
                if (SelectedFields.Contains("開課班次"))
                {
                    int intClass_name = 0;
                    if (!string.IsNullOrEmpty(class_name))
                    {
                        if (!int.TryParse(class_name, out intClass_name))
                            Messages[x.Position].MessageItems.Add(new MessageItem(EMBA.Validator.ErrorType.Error, EMBA.Validator.ValidatorType.Row, "「開課班次」只允許：空白、01、02、03。"));
                        else if (!(intClass_name == 1 || intClass_name == 2 || intClass_name == 3))
                            Messages[x.Position].MessageItems.Add(new MessageItem(EMBA.Validator.ErrorType.Error, EMBA.Validator.ValidatorType.Row, "「開課班次」只允許：空白、01、02、03。"));
                    }
                }
                //  若鍵值為「開課系統編號」，則必須存在於系統中
                if (keyField == "開課系統編號")
                {
                    if (!string.IsNullOrEmpty(course_id))
                    {
                        if (!Courses.Contains(course_id))
                            Messages[x.Position].MessageItems.Add(new MessageItem(EMBA.Validator.ErrorType.Error, EMBA.Validator.ValidatorType.Row, "系統無此開課系統編號。"));
                    }
                }
            });

            #endregion
        }

        public override XDocument GetValidateRule()
        {
            return XDocument.Parse(EMBACore.Properties.Resources.Course_Import);
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
            List<CourseRecord> ExistingCourseRecords = Course.SelectAll();
            Dictionary<string, CourseRecord> dicExistingCourseRecords = new Dictionary<string, CourseRecord>();
            if (ExistingCourseRecords.Count > 0)
            {
                if (keyField == "開課系統編號")
                    ExistingCourseRecords.ForEach(x => dicExistingCourseRecords.Add(x.ID, x));
                else
                {
                    foreach(CourseRecord course in ExistingCourseRecords)
                    {
                        string school_year = course.SchoolYear.HasValue ? course.SchoolYear.Value.ToString() : "";
                        string semester = course.Semester.HasValue ? course.Semester.Value.ToString() : "";
                        string name = course.Name.Trim().ToUpper();

                        dicExistingCourseRecords.Add(name + "_" + school_year + "_" + semester, course);
                    }
                }
            }

            DataTable dataTableSubjects = queryHelper.Select("select uid, subject_code, new_subject_code, name from $ischool.emba.subject");
            IEnumerable<DataRow> subjects = dataTableSubjects.Rows.Cast<DataRow>();
            Dictionary<string, KeyValuePair<string, string>> dicExistingSubjectCodes = new Dictionary<string, KeyValuePair<string, string>>();
            Dictionary<string, KeyValuePair<string, string>> dicExistingNewSubjectCodes = new Dictionary<string, KeyValuePair<string, string>>();
            if (subjects.Count() > 0)
            {
                subjects.ToList().ForEach((x) => 
                {
                    if (!dicExistingSubjectCodes.ContainsKey((x["subject_code"] + "").Trim().ToUpper()))
                        dicExistingSubjectCodes.Add((x["subject_code"] + "").Trim().ToUpper(), new KeyValuePair<string, string>((x["uid"] + ""), (x["name"] + "")));
                    if (!dicExistingNewSubjectCodes.ContainsKey((x["new_subject_code"] + "").Trim().ToUpper()))
                        dicExistingNewSubjectCodes.Add((x["new_subject_code"] + "").Trim().ToUpper(), new KeyValuePair<string, string>((x["uid"] + ""), (x["name"] + "")));
                });
            }
            
            //  要新增的 CourseRecord 
            List<CourseRecord> insertCourseRecords = new List<CourseRecord>();
            //  要更新的 CourseRecord
            List<CourseRecord> updateCourseRecords = new List<CourseRecord>();
            Dictionary<string, Log.LogAgent> dicLogAgents = new Dictionary<string, Log.LogAgent>();
            foreach (IRowStream row in Rows)
            {
                Log.LogAgent logAgent = new Log.LogAgent();
                
                CourseRecord courseRecord = new CourseRecord();
                IEnumerable<CourseRecord> filterCourseRecords = new List<CourseRecord>();
                //  若鍵值不為「開課系統編號」，則查出哪些課程是要新增的
                string key = string.Empty;
                if (keyField != "開課系統編號")
                {
                    string school_year = string.IsNullOrEmpty(row.GetValue("學年度").Trim()) ? "" : int.Parse(row.GetValue("學年度").Trim()).ToString();
                    string semester = string.IsNullOrEmpty(row.GetValue("學期").Trim()) ? "" : int.Parse(row.GetValue("學期").Trim()).ToString();
                    string name = row.GetValue("開課").Trim().ToUpper();

                    key = name + "_" + school_year + "_" + semester;
                }
                else
                    key = row.GetValue("開課系統編號").Trim();

                if (dicExistingCourseRecords.ContainsKey(key))
                    courseRecord = dicExistingCourseRecords[key];

                //  K12.Data.CourseRecord 要寫入：課程名稱，學年度，學期，學分數，科目編號      
                if (mOption.SelectedFields.Contains("開課"))
                {
                    if (!string.IsNullOrEmpty(courseRecord.ID))
                        logAgent.SetLogValue("開課", courseRecord.Name);

                    logAgent.SetLogValue("開課", row.GetValue("開課").Trim());
                    
                    courseRecord.Name = row.GetValue("開課").Trim();
                }
                if (mOption.SelectedFields.Contains("學年度"))
                {
                    int school_year1 = 0;
                    if (int.TryParse(row.GetValue("學年度").Trim(), out school_year1))
                    {
                        if (!string.IsNullOrEmpty(courseRecord.ID))
                            logAgent.SetLogValue("學年度", courseRecord.SchoolYear + "");

                        logAgent.SetLogValue("學年度", school_year1.ToString());
                        
                        courseRecord.SchoolYear = school_year1;
                    }
                }
                if (mOption.SelectedFields.Contains("學期"))
                {
                    int semester1 = 0;
                    if (int.TryParse(row.GetValue("學期").Trim(), out semester1))
                    {
                        if (!string.IsNullOrEmpty(courseRecord.ID))
                            logAgent.SetLogValue("學期", SemesterItem.GetSemesterByCode(courseRecord.Semester + "").Name);

                        logAgent.SetLogValue("學期", SemesterItem.GetSemesterByCode(semester1.ToString()).Name);
                        
                        courseRecord.Semester = semester1;
                    }
                }
                if (mOption.SelectedFields.Contains("學分數"))
                {
                    decimal credit = 0;
                    if (decimal.TryParse(row.GetValue("學分數").Trim(), out credit))
                    {
                        if (!string.IsNullOrEmpty(courseRecord.ID))
                            logAgent.SetLogValue("學分數", courseRecord.Credit + "");

                        logAgent.SetLogValue("學分數", credit.ToString());
                        
                        courseRecord.Credit = credit;
                    }
                }

                //  「課程識別碼」與「課號」至少有一個不為空白
                string subject_code = row.GetValue("課程識別碼").Trim().ToUpper();
                string new_subject_code = row.GetValue("課號").Trim().ToUpper();
                //  優先以「課程識別碼」比對出科目名稱
                string subject = string.Empty;

                if (dicExistingSubjectCodes.ContainsKey(subject_code))
                    subject = dicExistingSubjectCodes[subject_code].Value;
                else if (dicExistingNewSubjectCodes.ContainsKey(new_subject_code))
                    subject = dicExistingNewSubjectCodes[new_subject_code].Value;

                courseRecord.Subject = subject;

                if (string.IsNullOrEmpty(courseRecord.ID))
                {
                    logAgent.ActionType = Log.LogActionType.AddNew;
                    logAgent.TargetDetail = "課程「" + courseRecord.Name + "」，學年度「" + (courseRecord.SchoolYear + "") + "」，學期「" + EMBACore.DataItems.SemesterItem.GetSemesterByCode(courseRecord.Semester + "").Name + "」\n";
                    logAgent.Set("課程.匯入.基本資料", "", "", Log.LogTargetCategory.Course);
                    insertCourseRecords.Add(courseRecord);
                }
                else
                {
                    logAgent.ActionType = Log.LogActionType.Update;
                    logAgent.Set("課程.匯入.基本資料", "", "", Log.LogTargetCategory.Course, courseRecord.ID);
                    updateCourseRecords.Add(courseRecord);
                }
                dicLogAgents.Add(courseRecord.Name.Trim().ToUpper() + "_" + (courseRecord.SchoolYear + "") + "_" + (courseRecord.Semester + ""), logAgent);
            }
            //  新增開課
            List<string> insertedCourseIDs = Course.Insert(insertCourseRecords);
            //  更新開課
            Course.Update(updateCourseRecords);
            //  所有新增或更新的 CourseID
            IEnumerable<string> allAffectedCourseIDs = insertedCourseIDs.Union(updateCourseRecords.Select(x => x.ID));
            IEnumerable<CourseRecord> allAffectedCourseRecords = K12.Data.Course.SelectByIDs(allAffectedCourseIDs);
            Dictionary<string, CourseRecord> dicCourseRecords = new Dictionary<string, CourseRecord>();

            if (allAffectedCourseRecords.Count() > 0)
            {
                allAffectedCourseRecords.ToList().ForEach((x) =>
                {
                    if (keyField == "開課系統編號")
                        dicCourseRecords.Add(x.ID, x);
                    else
                        dicCourseRecords.Add(x.Name.Trim().ToUpper() + "_" + (x.SchoolYear + "") + "_" + (x.Semester + ""), x);
                });
            }
            //  課程延伸資料
            List<UDT.CourseExt> CourseExtRecords = Access.Select<UDT.CourseExt>(string.Format("ref_course_id in ({0})", string.Join(",", allAffectedCourseIDs)));
            Dictionary<string, UDT.CourseExt> dicCourseExtRecords = new Dictionary<string, UDT.CourseExt>();
            if (CourseExtRecords.Count > 0)
            {
                CourseExtRecords.ForEach((x) =>
                {
                    if (!dicCourseExtRecords.ContainsKey(x.CourseID.ToString()))
                        dicCourseExtRecords.Add(x.CourseID.ToString(), x);
                });
            }

            foreach (IRowStream row in Rows)
            {
                string school_year = int.Parse(row.GetValue("學年度")) + "";
                string semester = int.Parse(row.GetValue("學期")) + "";
                string name = row.GetValue("開課").Trim().ToUpper();

                CourseRecord courseRecord;
                if (keyField != "開課系統編號")
                    courseRecord = dicCourseRecords[name + "_" + school_year + "_" + semester];
                else
                    courseRecord = dicCourseRecords[row.GetValue("開課系統編號")];

                CourseExt courseExt = new CourseExt();
                if (dicCourseExtRecords.ContainsKey(courseRecord.ID))
                    courseExt = dicCourseExtRecords[courseRecord.ID];

                courseExt.CourseID = int.Parse(courseRecord.ID);
                Log.LogAgent logAgent = dicLogAgents[courseRecord.Name.Trim().ToUpper() + "_" + (courseRecord.SchoolYear + "") + "_" + (courseRecord.Semester + "")];

                if (mOption.SelectedFields.Contains("開課班次"))
                {
                    if (!string.IsNullOrEmpty(courseExt.UID))
                        logAgent.SetLogValue("開課班次", courseExt.ClassName);

                    int intClass_name = 0;
                    string strClass_name = string.Empty;
                    if (int.TryParse(row.GetValue("開課班次").Trim(), out intClass_name))
                    {
                        logAgent.SetLogValue("開課班次", string.Format("{0:00}", intClass_name));
                        courseExt.ClassName = string.Format("{0:00}", intClass_name);
                    }
                    else
                    {
                        logAgent.SetLogValue("開課班次", row.GetValue("開課班次").Trim());
                        courseExt.ClassName = row.GetValue("開課班次").Trim();
                    }
                }               
                if (mOption.SelectedFields.Contains("類別") && !string.IsNullOrWhiteSpace("類別"))
                {
                    if (!string.IsNullOrEmpty(courseExt.UID))
                        logAgent.SetLogValue("類別", courseExt.CourseType);

                    logAgent.SetLogValue("類別", row.GetValue("類別").Trim());
                    courseExt.CourseType = row.GetValue("類別").Trim();
                }
                if (mOption.SelectedFields.Contains("課程識別碼") && !string.IsNullOrWhiteSpace("課程識別碼"))
                {
                    if (!string.IsNullOrEmpty(courseExt.UID))
                        logAgent.SetLogValue("課程識別碼", courseExt.SubjectCode);

                    logAgent.SetLogValue("課程識別碼", row.GetValue("課程識別碼").Trim());
                    courseExt.SubjectCode = row.GetValue("課程識別碼").Trim();
                }
                if (mOption.SelectedFields.Contains("課號") && !string.IsNullOrWhiteSpace("課號"))
                {
                    if (!string.IsNullOrEmpty(courseExt.UID))
                        logAgent.SetLogValue("課號", courseExt.NewSubjectCode);

                    logAgent.SetLogValue("課號", row.GetValue("課號").Trim());
                    courseExt.NewSubjectCode = row.GetValue("課號").Trim();
                }
                if (mOption.SelectedFields.Contains("必選修") && !string.IsNullOrWhiteSpace("必選修"))
                {
                    if (!string.IsNullOrEmpty(courseExt.UID))
                        logAgent.SetLogValue("必選修", courseExt.IsRequired ? "必修" : "選修");

                    logAgent.SetLogValue("必選修", row.GetValue("必選修").Trim());
                    courseExt.IsRequired = (row.GetValue("必選修").Trim() == "必修" ? true : false);
                }
                if (mOption.SelectedFields.Contains("流水號") && !string.IsNullOrWhiteSpace("流水號"))
                {
                    int serialNo = 0;
                    if (int.TryParse(row.GetValue("流水號").Trim(), out serialNo))
                    {
                        if (!string.IsNullOrEmpty(courseExt.UID))
                            logAgent.SetLogValue("流水號", courseExt.SerialNo + "");

                        logAgent.SetLogValue("流水號", serialNo + "");
                        
                        courseExt.SerialNo = serialNo;
                    }
                }
                if (mOption.SelectedFields.Contains("人數上限") && !string.IsNullOrWhiteSpace("人數上限"))
                {
                    int capacity = 0;
                    if (int.TryParse(row.GetValue("人數上限").Trim(), out capacity))
                    {
                        if (!string.IsNullOrEmpty(courseExt.UID))
                            logAgent.SetLogValue("人數上限", courseExt.Capacity + "");

                        logAgent.SetLogValue("人數上限", capacity + "");

                        courseExt.Capacity = capacity;
                    }
                }
                //  優先以「課程識別碼」比對出科目系統編號
                string subject_code = row.GetValue("課程識別碼").Trim().ToUpper();
                string new_subject_code = row.GetValue("課號").Trim().ToUpper();
                if (dicExistingSubjectCodes.ContainsKey(subject_code))
                    courseExt.SubjectID = int.Parse(dicExistingSubjectCodes[subject_code].Key);
                else if (dicExistingNewSubjectCodes.ContainsKey(new_subject_code))
                    courseExt.SubjectID = int.Parse(dicExistingNewSubjectCodes[new_subject_code].Key);

                if (string.IsNullOrEmpty(courseExt.UID))
                    CourseExtRecords.Add(courseExt);
            }
            //  更新 CourseExt
            List<string> updatedCourseExtIDs = new List<string>();
            
            try
            {
                updatedCourseExtIDs = CourseExtRecords.SaveAll();
            }
            catch (Exception e)
            {
                System.Windows.Forms.MessageBox.Show(e.Message);
                return string.Empty;
            }
            //  批次寫入 Log
            Log.BatchLogAgent BatchLogAgent = new Log.BatchLogAgent();
            BatchLogAgent.AddLogAgents(dicLogAgents.Values);

            try
            {
                BatchLogAgent.Save();
            }
            catch (Exception e)
            {
                System.Windows.Forms.MessageBox.Show(e.Message);
                return e.Message;
            }
            //  RaiseEvent
            if (updatedCourseExtIDs.Count > 0)
            {
                UDT.CourseExt.RaiseAfterUpdateEvent(this, new ParameterEventArgs(updatedCourseExtIDs));
            }
            return string.Empty;
        }
    }
}