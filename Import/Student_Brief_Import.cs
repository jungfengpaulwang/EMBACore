using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Xml.Linq;
using EMBA.DocumentValidator;
using EMBA.Import;
using EMBA.Validator;
using EMBACore.UDT;
using FISCA.UDT;
using K12.Data;
using FISCA.Data;
using System.Data;
using System.IO;
using System.Text.RegularExpressions;

namespace EMBACore.Import
{
    class Student_Brief_Import : ImportWizard
    {
        ImportOption mOption;
        readonly string keyField = "學號";
        readonly string importType = "更新";
        AccessHelper Access;
        QueryHelper queryHelper;
        DataTable dataTable_ImportTarget;
        Dictionary<string, ClassRecord> dicAllClass_Name;
        Dictionary<string, DepartmentGroup> dicAllDepart_Name;
        Dictionary<string, ClassRecord> dicAllClass_ID;
        Dictionary<string, DepartmentGroup> dicAllDepart_ID;
        Dictionary<string, Log.LogAgent> dicLogAgents;
        public Student_Brief_Import()
        {
            this.IsSplit = false;
            this.ShowAdvancedForm = false;
            this.ValidateRuleFormater = XDocument.Parse(EMBACore.Properties.Resources.format);
            //this.CustomValidate = null;
            //this.SplitThreadCount = 5;
            //this.SplitSize = 3000;

            importType = string.Empty;
            Access = new AccessHelper();
            queryHelper = new QueryHelper();
            dataTable_ImportTarget = new DataTable();
            dicLogAgents = new Dictionary<string, Log.LogAgent>();
            this.CustomValidate = (Rows, Messages) =>
            {
                CustomValidator(Rows, Messages);
            };
        }

        public void CustomValidator(List<IRowStream> Rows, RowMessages Messages)
        {
            #  region 驗證流程
            List<ClassRecord> allClass = Class.SelectAll();
            List<DepartmentGroup> allDepart = Access.Select<DepartmentGroup>();
            dicAllClass_Name = new Dictionary<string, ClassRecord>();
            dicAllDepart_Name = new Dictionary<string, DepartmentGroup>();
            dicAllClass_ID = new Dictionary<string, ClassRecord>();
            dicAllDepart_ID = new Dictionary<string, DepartmentGroup>();
            if (allClass.Count > 0)
                allClass.ForEach((x) =>
                {
                    if (!dicAllClass_Name.ContainsKey(x.Name))
                        dicAllClass_Name.Add(x.Name.Trim().ToUpper(), x);

                    dicAllClass_ID.Add(x.ID, x);
                });
            if (allDepart.Count > 0)
                allDepart.ForEach((x) =>
                {
                    if (!dicAllDepart_Name.ContainsKey(x.Name))
                        dicAllDepart_Name.Add(x.Name.Trim().ToUpper(), x);

                    dicAllDepart_ID.Add(x.UID, x);
                });

            List<EMBACore.UDT.ExperienceDataSource> ExperienceDataSources = Access.Select<EMBACore.UDT.ExperienceDataSource>();
            Dictionary<string, List<string>> dicExperiences = new Dictionary<string, List<string>>();
            foreach (EMBACore.UDT.ExperienceDataSource experienceDataSource in ExperienceDataSources)
            {
                if (!dicExperiences.ContainsKey(experienceDataSource.ItemCategory))
                    dicExperiences.Add(experienceDataSource.ItemCategory, new List<string>());

                dicExperiences[experienceDataSource.ItemCategory].Add(experienceDataSource.Item);
            }

            //  「身分證號+學生狀態」在資料庫的設定為唯一。
            //  「學號」在台大EMBA的設定為唯一。
            //  「登入帳號」在資料庫的設定為唯一。
            //  「教學分班」若有填則必須存在
            //  學號同步自台大「校務系統」，無需以「學生系統編號」修改「學號」，故取消「學生系統編號」為鍵值。
            dataTable_ImportTarget = queryHelper.Select("select student.name as name, student.id as id, id_number, sa_login_name, student_number, student.status,  class.id as class_id, class_name from student left join class on class.id=student.ref_class_id");
            IEnumerable<DataRow> ImportTargets = dataTable_ImportTarget.Rows.Cast<DataRow>();

            foreach (IRowStream x in Rows)
            {
                string loginName = x.GetValue("登入帳號").Trim();
                string idNumber = x.GetValue("身分證號").Trim();
                string studentNumber = x.GetValue("學號").Trim();
                //string status = x.GetValue("學生狀態").Trim();    匯入已移除
                string className = x.GetValue("教學分班").Trim().ToUpper();
                string depart = x.GetValue("系所組別").Trim().ToUpper();

                //  「登入帳號」不可重覆
                if (!string.IsNullOrEmpty(loginName))
                {
                    string msg = string.Empty;
                    if (this.SelectedFields.Contains("登入帳號"))
                    {
                        //  排除本人資料後若已存在則為錯誤
                        IEnumerable<DataRow> students = ImportTargets.Where(y => y["sa_login_name"] != null).Where(y => (y["sa_login_name"] + "").Trim().ToUpper() == loginName.ToUpper());
                        bool error = false;
                        if (students.Count() > 0)
                        {
                            //if (keyField == "學生系統編號" && (students.ElementAt(0)["id"] + "") != studentID)
                            //    error = true;
                            if ((students.ElementAt(0)["student_number"] + "").ToUpper() != studentNumber.ToUpper())
                                error = true;
                            //if (keyField == "身分證號" && ((students.ElementAt(0)["id_number"] + "_" + t_Status(students.ElementAt(0)["status"] + "")) != (idNumber + "_" + status)))
                            //    error = true;

                            if (error)
                            {
                                msg = "資料庫已存在登入帳號「" + loginName + "」，擁有人資訊為「姓名：" + (students.ElementAt(0)["name"]) + "；學號：" + students.ElementAt(0)["student_number"];
                                Messages[x.Position].MessageItems.Add(new MessageItem(EMBA.Validator.ErrorType.Error, EMBA.Validator.ValidatorType.Row, msg));
                            }
                        }
                    }
                }

                //  「身分證號+學生狀態」不可重覆
                //  若「身分證號」為空白則不匯入故亦不驗證
                if (!string.IsNullOrEmpty(idNumber))
                {
                    string msg = string.Empty;
                    if (this.SelectedFields.Contains("身分證號"))
                    {
                        //  排除本人資料後若已存在則為錯誤
                        IEnumerable<DataRow> students = ImportTargets.Where(y => y["id_number"] != null).Where(y => ((y["id_number"] + "").Trim().ToUpper() == idNumber.ToUpper()));
                        bool error = false;
                        if (students.Count() > 0)
                        {
                            //if (keyField == "學生系統編號" && (students.ElementAt(0)["id"] + "") != studentID)
                            //    error = true;
                            if ((students.ElementAt(0)["student_number"] + "").ToUpper() != studentNumber.ToUpper())
                                error = true;

                            if (error)
                            {
                                msg = "資料庫已存在身分證號「" + idNumber + "」，擁有人資訊為「姓名：" + (students.ElementAt(0)["name"]) + "；學號：" + students.ElementAt(0)["student_number"];
                                Messages[x.Position].MessageItems.Add(new MessageItem(EMBA.Validator.ErrorType.Error, EMBA.Validator.ValidatorType.Row, msg));
                            }
                        }
                    }
                }

                //  學號必須存在
                if (ImportTargets.Where(y => ((y["student_number"] + "").Trim().ToUpper() == studentNumber.ToUpper())).Count() == 0)
                {
                    Messages[x.Position].MessageItems.Add(new MessageItem(EMBA.Validator.ErrorType.Error, EMBA.Validator.ValidatorType.Row, "學號不存在。"));
                }

                //  教學分班必須存在
                if (this.SelectedFields.Contains("教學分班") && !dicAllClass_Name.ContainsKey(className))
                    Messages[x.Position].MessageItems.Add(new MessageItem(EMBA.Validator.ErrorType.Error, EMBA.Validator.ValidatorType.Row, "教學分班不存在。"));

                //  系所組別必須存在
                if (this.SelectedFields.Contains("系所組別") && !dicAllDepart_Name.ContainsKey(depart))
                    Messages[x.Position].MessageItems.Add(new MessageItem(EMBA.Validator.ErrorType.Error, EMBA.Validator.ValidatorType.Row, "系所組別不存在。"));

                //  「產業別」必須存在
                if (this.SelectedFields.Contains("產業別") && !string.IsNullOrWhiteSpace(x.GetValue("產業別")))
                {
                    if (!dicExperiences.ContainsKey("產業別"))
                        Messages[x.Position].MessageItems.Add(new MessageItem(EMBA.Validator.ErrorType.Error, EMBA.Validator.ValidatorType.Row, "「產業別」不存在，請先至「經歷管理」建立「產業別」內容。"));
                    else
                    {
                        if (!dicExperiences["產業別"].Contains(x.GetValue("產業別").Trim()))
                            Messages[x.Position].MessageItems.Add(new MessageItem(EMBA.Validator.ErrorType.Error, EMBA.Validator.ValidatorType.Row, "「產業別」不存在，請先至「經歷管理」建立「產業別」內容。"));
                    }
                }

                //  「部門類別」必須存在
                if (this.SelectedFields.Contains("部門類別") && !string.IsNullOrWhiteSpace(x.GetValue("部門類別")))
                {
                    if (!dicExperiences.ContainsKey("部門類別"))
                        Messages[x.Position].MessageItems.Add(new MessageItem(EMBA.Validator.ErrorType.Error, EMBA.Validator.ValidatorType.Row, "「部門類別」不存在，請先至「經歷管理」建立「部門類別」內容。"));
                    else
                    {
                        if (!dicExperiences["部門類別"].Contains(x.GetValue("部門類別").Trim()))
                            Messages[x.Position].MessageItems.Add(new MessageItem(EMBA.Validator.ErrorType.Error, EMBA.Validator.ValidatorType.Row, "「部門類別」不存在，請先至「經歷管理」建立「部門類別」內容。"));
                    }
                }

                //  「層級別」必須存在
                if (this.SelectedFields.Contains("層級別") && !string.IsNullOrWhiteSpace(x.GetValue("層級別")))
                {
                    if (!dicExperiences.ContainsKey("層級別"))
                        Messages[x.Position].MessageItems.Add(new MessageItem(EMBA.Validator.ErrorType.Error, EMBA.Validator.ValidatorType.Row, "「層級別」不存在，請先至「經歷管理」建立「層級別」內容。"));
                    else
                    {
                        if (!dicExperiences["層級別"].Contains(x.GetValue("層級別").Trim()))
                            Messages[x.Position].MessageItems.Add(new MessageItem(EMBA.Validator.ErrorType.Error, EMBA.Validator.ValidatorType.Row, "「層級別」不存在，請先至「經歷管理」建立「層級別」內容。"));
                    }
                }

                //  「工作地點」必須存在
                if (this.SelectedFields.Contains("工作地點") && !string.IsNullOrWhiteSpace(x.GetValue("工作地點")))
                {
                    if (!dicExperiences.ContainsKey("工作地點"))
                        Messages[x.Position].MessageItems.Add(new MessageItem(EMBA.Validator.ErrorType.Error, EMBA.Validator.ValidatorType.Row, "「工作地點」不存在，請先至「經歷管理」建立「工作地點」內容。"));
                    else
                    {
                        if (!dicExperiences["工作地點"].Contains(x.GetValue("工作地點").Trim()))
                            Messages[x.Position].MessageItems.Add(new MessageItem(EMBA.Validator.ErrorType.Error, EMBA.Validator.ValidatorType.Row, "「工作地點」不存在，請先至「經歷管理」建立「工作地點」內容。"));
                    }
                }

                //  「工作狀態」必須存在
                if (this.SelectedFields.Contains("工作狀態") && !string.IsNullOrWhiteSpace(x.GetValue("工作狀態")))
                {
                    if (!dicExperiences.ContainsKey("工作狀態"))
                        Messages[x.Position].MessageItems.Add(new MessageItem(EMBA.Validator.ErrorType.Error, EMBA.Validator.ValidatorType.Row, "「工作狀態」不存在，請先至「經歷管理」建立「工作狀態」內容。"));
                    else
                    {
                        if (!dicExperiences["工作狀態"].Contains(x.GetValue("工作狀態").Trim()))
                            Messages[x.Position].MessageItems.Add(new MessageItem(EMBA.Validator.ErrorType.Error, EMBA.Validator.ValidatorType.Row, "「工作狀態」不存在，請先至「經歷管理」建立「工作狀態」內容。"));
                    }
                }

                //  「工作起日」必須為日期格式
                if (this.SelectedFields.Contains("工作起日") && !string.IsNullOrWhiteSpace(x.GetValue("工作起日")))
                {
                    DateTime work_begin_date;

                    if (!DateTime.TryParse(x.GetValue("工作起日").Trim(), out work_begin_date))
                        Messages[x.Position].MessageItems.Add(new MessageItem(EMBA.Validator.ErrorType.Error, EMBA.Validator.ValidatorType.Row, "「工作起日」必須為日期格式，範例：1970/8/24。"));
                }

                //  「工作迄日」必須為日期格式
                if (this.SelectedFields.Contains("工作迄日") && !string.IsNullOrWhiteSpace(x.GetValue("工作迄日")))
                {
                    DateTime work_end_date;

                    if (!DateTime.TryParse(x.GetValue("工作迄日").Trim(), out work_end_date))
                        Messages[x.Position].MessageItems.Add(new MessageItem(EMBA.Validator.ErrorType.Error, EMBA.Validator.ValidatorType.Row, "「工作迄日」必須為日期格式，範例：1970/8/24。"));
                }

                //  「登入帳號」必須為有效的電子郵件格式
                if (this.SelectedFields.Contains("登入帳號") && !string.IsNullOrWhiteSpace(x.GetValue("登入帳號")))
                {
                    if (!isValidEmail(x.GetValue("登入帳號").Trim()))
                        Messages[x.Position].MessageItems.Add(new MessageItem(EMBA.Validator.ErrorType.Error, EMBA.Validator.ValidatorType.Row, "「登入帳號」格式不正確。"));
                }

                //  「電子郵件一」必須為有效的電子郵件格式
                if (this.SelectedFields.Contains("電子郵件一") && !string.IsNullOrWhiteSpace(x.GetValue("電子郵件一")))
                {
                    if (!isValidEmail(x.GetValue("電子郵件一").Trim()))
                        Messages[x.Position].MessageItems.Add(new MessageItem(EMBA.Validator.ErrorType.Error, EMBA.Validator.ValidatorType.Row, "「電子郵件一」格式不正確。"));
                }

                //  「電子郵件二」必須為有效的電子郵件格式
                if (this.SelectedFields.Contains("電子郵件二") && !string.IsNullOrWhiteSpace(x.GetValue("電子郵件二")))
                {
                    if (!isValidEmail(x.GetValue("電子郵件二").Trim()))
                        Messages[x.Position].MessageItems.Add(new MessageItem(EMBA.Validator.ErrorType.Error, EMBA.Validator.ValidatorType.Row, "「電子郵件二」格式不正確。"));
                }

                //  「電子郵件三」必須為有效的電子郵件格式
                if (this.SelectedFields.Contains("電子郵件三") && !string.IsNullOrWhiteSpace(x.GetValue("電子郵件三")))
                {
                    if (!isValidEmail(x.GetValue("電子郵件三").Trim()))
                        Messages[x.Position].MessageItems.Add(new MessageItem(EMBA.Validator.ErrorType.Error, EMBA.Validator.ValidatorType.Row, "「電子郵件三」格式不正確。"));
                }

                //  「電子郵件四」必須為有效的電子郵件格式
                if (this.SelectedFields.Contains("電子郵件四") && !string.IsNullOrWhiteSpace(x.GetValue("電子郵件四")))
                {
                    if (!isValidEmail(x.GetValue("電子郵件四").Trim()))
                        Messages[x.Position].MessageItems.Add(new MessageItem(EMBA.Validator.ErrorType.Error, EMBA.Validator.ValidatorType.Row, "「電子郵件四」格式不正確。"));
                }

                //  「電子郵件五」必須為有效的電子郵件格式
                if (this.SelectedFields.Contains("電子郵件五") && !string.IsNullOrWhiteSpace(x.GetValue("電子郵件五")))
                {
                    if (!isValidEmail(x.GetValue("電子郵件五").Trim()))
                        Messages[x.Position].MessageItems.Add(new MessageItem(EMBA.Validator.ErrorType.Error, EMBA.Validator.ValidatorType.Row, "「電子郵件五」格式不正確。"));
                }
            }

            #endregion
        }

        private bool isValidEmail(string email)
        {
            string patternStrict = @"^(([^<>()[\]\\.,;:\s@\""]+"
                       + @"(\.[^<>()[\]\\.,;:\s@\""]+)*)|(\"".+\""))@"
                       + @"((\[[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}"
                       + @"\.[0-9]{1,3}\])|(([a-zA-Z\-0-9]+\.)+"
                       + @"[a-zA-Z]{2,}))$";
            Regex reStrict = new Regex(patternStrict);

            bool isStrictMatch = reStrict.IsMatch(email);
            return isStrictMatch;
        }

        public override ImportAction GetSupportActions()
        {
            //return ImportAction.InsertOrUpdate | ImportAction.Insert | ImportAction.Update;
            //  學生基本資料同步自台大「校務系統」，匯入的方式僅有「修改」。
            return ImportAction.Update;
        }

        public override XDocument GetValidateRule()
        {
            return XDocument.Parse(EMBACore.Properties.Resources.Student_Brief_Import);
        }

        private string x_Status(string status)
        {
            string t_Status = string.Empty;
            switch (status)
            {
                case "一般":
                    t_Status = "在學";
                    break;
                case "休學":
                    t_Status = "休學";
                    break;
                case "退學":
                    t_Status = "退學";
                    break;
                case "畢業或離校":
                    t_Status = "畢業";
                    break;
                case "刪除":
                    t_Status = "刪除";
                    break;
                default:
                    t_Status = "";
                    break;
            }
            return t_Status;
        }

        private string t_Status(string status)
        {
            string t_Status = string.Empty;
            switch (status)
            {
                case "1":
                    t_Status = "在學";
                    break;
                case "4":
                    t_Status = "休學";
                    break;
                case "64":
                    t_Status = "退學";
                    break;
                case "16":
                    t_Status = "畢業";
                    break;
                case "256":
                    t_Status = "刪除";
                    break;
                default:
                    t_Status = "";
                    break;
            }
            return t_Status;
        }

        public override string Import(List<IRowStream> Rows)
        {
            List<StudentRecord> _Students = Student.SelectAll();
            List<Experience> _Experience = Access.Select<Experience>();
            List<EducationBackground> _EducationBackground = Access.Select<EducationBackground>();

            Dictionary<string, int> dicImportInfos = new Dictionary<string, int>();
            List<StudentRecord> updateRecords = new List<StudentRecord>();
            List<StudentRecord> insertRecords = new List<StudentRecord>();
            string message = string.Empty;

            message = WriteToDB(Rows, _Students, _EducationBackground, _Experience);

            return message;
        }

        public override void Prepare(ImportOption Option)
        {
            mOption = Option;
        }

        private string WriteToDB(IEnumerable<IRowStream> Rows, List<StudentRecord> _Students, List<EducationBackground> _EducationBackground, List<Experience> _Experience)
        {
            string message = string.Empty;
            Dictionary<string, StudentRecord> dicStudents = new Dictionary<string, StudentRecord>();

            foreach (StudentRecord x in _Students)
            {
                if (!string.IsNullOrWhiteSpace(x.StudentNumber) && !dicStudents.ContainsKey(x.StudentNumber.Trim().ToUpper()))
                    dicStudents.Add(x.StudentNumber.Trim().ToUpper(), x);
            }

            List<StudentRecord> insertStudentRecords = new List<StudentRecord>();
            List<StudentRecord> updateStudentRecords = new List<StudentRecord>();
            //  1、修改學生基本資料。
            foreach (IRowStream iRow in Rows)
            {
                string student_IDNumber = iRow.GetValue("學號").Trim().ToUpper();

                StudentRecord studentRecord = new StudentRecord();
                    
                if (dicStudents.ContainsKey(student_IDNumber))
                    studentRecord = dicStudents[student_IDNumber];

                studentRecord.StudentNumber = iRow.GetValue("學號").Trim();
                if (mOption.SelectedFields.Contains("姓名") && !string.IsNullOrWhiteSpace(iRow.GetValue("姓名")))
                {
                    studentRecord.Name = iRow.GetValue("姓名").Trim();
                }
                //  宣告 LogAgent
                Log.LogAgent logAgent = new Log.LogAgent();
                logAgent.ActionType = Log.LogActionType.Update;
                logAgent.Set("學生.匯入.基本資料", "", "", Log.LogTargetCategory.Student, studentRecord.ID);

                if (mOption.SelectedFields.Contains("身分證號") && !string.IsNullOrWhiteSpace(iRow.GetValue("身分證號")))
                {
                    //  ID不為空值，代表修改資料，Log 修改前的值(本匯入只有更新，ID肯定不為空值)
                    //if (!string.IsNullOrEmpty(studentRecord.ID))
                    logAgent.SetLogValue("身分證號", studentRecord.IDNumber);

                    studentRecord.IDNumber = iRow.GetValue("身分證號").Trim();
                    //  Log 修改後的值
                    logAgent.SetLogValue("身分證號", studentRecord.IDNumber);
                }
                if (mOption.SelectedFields.Contains("教學分班") && !string.IsNullOrWhiteSpace(iRow.GetValue("教學分班")))
                {
                    if (dicAllClass_Name.ContainsKey(iRow.GetValue("教學分班").Trim().ToUpper()))
                    {
                        string ref_class_id = studentRecord.RefClassID + "";
                        logAgent.SetLogValue("教學分班", dicAllClass_ID.ContainsKey(ref_class_id) ? dicAllClass_ID[ref_class_id].Name : "");
                        studentRecord.RefClassID = dicAllClass_Name[iRow.GetValue("教學分班").Trim().ToUpper()].ID;
                        logAgent.SetLogValue("教學分班", iRow.GetValue("教學分班").Trim());
                    }
                }

                if (mOption.SelectedFields.Contains("出生地") && !string.IsNullOrWhiteSpace(iRow.GetValue("出生地")))
                {
                    logAgent.SetLogValue("出生地", studentRecord.BirthPlace);
                    studentRecord.BirthPlace = iRow.GetValue("出生地").Trim();
                    logAgent.SetLogValue("出生地", studentRecord.BirthPlace);
                }

                if (mOption.SelectedFields.Contains("登入帳號") && !string.IsNullOrWhiteSpace(iRow.GetValue("登入帳號")))
                {
                    logAgent.SetLogValue("登入帳號", studentRecord.SALoginName);
                    studentRecord.SALoginName = iRow.GetValue("登入帳號").Trim();
                    logAgent.SetLogValue("登入帳號", studentRecord.SALoginName);
                }

                if (mOption.SelectedFields.Contains("生日") && !string.IsNullOrWhiteSpace(iRow.GetValue("生日")))
                {
                    logAgent.SetLogValue("生日", studentRecord.SALoginName);
                    studentRecord.Birthday = DateTime.Parse(iRow.GetValue("生日").Trim());
                    logAgent.SetLogValue("生日", studentRecord.SALoginName);
                }

                if (mOption.SelectedFields.Contains("性別") && !string.IsNullOrWhiteSpace(iRow.GetValue("性別")))
                {
                    logAgent.SetLogValue("性別", studentRecord.SALoginName);
                    studentRecord.Gender = iRow.GetValue("性別").Trim();
                    logAgent.SetLogValue("性別", studentRecord.SALoginName);
                }
                //switch (iRow.GetValue("學生狀態").Trim())
                //{
                //    case "在學":
                //        studentRecord.Status = StudentRecord.StudentStatus.一般;
                //        break;
                //    case "休學":
                //        studentRecord.Status = StudentRecord.StudentStatus.休學;
                //        break;
                //    case "刪除":
                //        studentRecord.Status = StudentRecord.StudentStatus.刪除;
                //        break;
                //    case "退學":
                //        studentRecord.Status = StudentRecord.StudentStatus.退學;
                //        break;
                //    case "畢業":
                //        studentRecord.Status = StudentRecord.StudentStatus.畢業或離校;
                //        break;
                //}

                if (string.IsNullOrEmpty(studentRecord.ID))
                    insertStudentRecords.Add(studentRecord);
                else
                    updateStudentRecords.Add(studentRecord);

                dicLogAgents.Add(student_IDNumber, logAgent);
            }
            List<string> insertedStudentIDs = new List<string>();
            if (insertStudentRecords.Count > 0)
            {
                insertedStudentIDs = Student.Insert(insertStudentRecords);

                //foreach (StudentRecord student in insertStudentRecords)
                //{
                //    try
                //    {
                //        Student.Insert(student);
                //    }
                //    catch (Exception ex)
                //    {
                //        MessageBox.Show(ex.Message);
                //    }
                //}
                //insertedStudentIDs = Student.Insert(insertStudentRecords);
            }
            int SuccessCount = 0;

            if (updateStudentRecords.Count > 0)
            {
                try
                {
                    //foreach (StudentRecord student in updateStudentRecords)
                    //    Student.Update(student);
                    SuccessCount = Student.Update(updateStudentRecords);
                }
                catch (Exception e)
                {
                    MessageBox.Show(e.Message);
                }
            }

            //  2、取得新增之學生基本資料
            if (insertedStudentIDs.Count > 0)
            {
                List<StudentRecord> insertedStudentRecords = Student.SelectByIDs(insertedStudentIDs);

                if (insertedStudentRecords.Count > 0)
                {
                        insertedStudentRecords.ForEach((x) =>
                        {
                            if (!dicStudents.ContainsKey(x.StudentNumber.ToUpper()))
                                dicStudents.Add(x.StudentNumber.ToUpper(), x);
                        });
                }
            }

            //  3、更新地址資料。
            message += UpdateAddress(Rows, dicStudents);
            //  4、更新電話資料。
            message += UpdatePhone(Rows, dicStudents);
            //  5、新增或修改學生延伸資料(包含系所組別)。
            message += UpdateBrief2(Rows, dicStudents);
            //  5、新增或修改學歷資料。
            message += UpdateEducationBackground(Rows, dicStudents);
            //  5、新增或修改經歷資料。
            //message += UpdateExperience(Rows, dicStudents);

            return message;
        }

        private string UpdateAddress(IEnumerable<IRowStream> Rows, Dictionary<string, StudentRecord> dicStudents)
        {
            string message = string.Empty;
            List<AddressRecord> oAddresses = Address.SelectByStudentIDs(dicStudents.Values.Select(x=>x.ID));
            Dictionary<string, AddressRecord> dicAddresses = new Dictionary<string, AddressRecord>();
            if (oAddresses.Count > 0)
                dicAddresses = oAddresses.ToDictionary(x => x.RefStudentID);

            List<AddressRecord> updateRecords = new List<AddressRecord>();
            foreach (IRowStream iRow in Rows)
            {
                string student_IDNumber = iRow.GetValue("學號").Trim().ToUpper();
                AddressRecord addressRecord = new AddressRecord();
                if (dicAddresses.ContainsKey(dicStudents[student_IDNumber].ID))
                    addressRecord = dicAddresses[dicStudents[student_IDNumber].ID];

                Log.LogAgent logAgent = dicLogAgents[student_IDNumber];

                addressRecord.RefStudentID = dicStudents[student_IDNumber].ID;

                if (mOption.SelectedFields.Contains("聯絡地址:郵遞區號") && !string.IsNullOrWhiteSpace(iRow.GetValue("聯絡地址:郵遞區號")))
                {
                    logAgent.SetLogValue("聯絡地址:郵遞區號", addressRecord.Mailing.ZipCode);
                    addressRecord.Mailing.ZipCode = iRow.GetValue("聯絡地址:郵遞區號").Trim();
                    logAgent.SetLogValue("聯絡地址:郵遞區號", addressRecord.Mailing.ZipCode);
                }
                if (mOption.SelectedFields.Contains("聯絡地址:縣市") && !string.IsNullOrWhiteSpace(iRow.GetValue("聯絡地址:縣市")))
                {
                    logAgent.SetLogValue("聯絡地址:縣市", addressRecord.Mailing.County);
                    addressRecord.Mailing.County = iRow.GetValue("聯絡地址:縣市").Trim();
                    logAgent.SetLogValue("聯絡地址:縣市", addressRecord.Mailing.County);
                }
                if (mOption.SelectedFields.Contains("聯絡地址:鄉鎮") && !string.IsNullOrWhiteSpace(iRow.GetValue("聯絡地址:鄉鎮")))
                {
                    logAgent.SetLogValue("聯絡地址:鄉鎮", addressRecord.Mailing.Town);
                    addressRecord.Mailing.Town = iRow.GetValue("聯絡地址:鄉鎮").Trim();
                    logAgent.SetLogValue("聯絡地址:鄉鎮", addressRecord.Mailing.Town);
                }
                if (mOption.SelectedFields.Contains("聯絡地址:村里") && !string.IsNullOrWhiteSpace(iRow.GetValue("聯絡地址:村里")))
                {
                    logAgent.SetLogValue("聯絡地址:村里", addressRecord.Mailing.District);
                    addressRecord.Mailing.District = iRow.GetValue("聯絡地址:村里").Trim();
                    logAgent.SetLogValue("聯絡地址:村里", addressRecord.Mailing.District);
                }
                if (mOption.SelectedFields.Contains("聯絡地址:鄰") && !string.IsNullOrWhiteSpace(iRow.GetValue("聯絡地址:鄰")))
                {
                    logAgent.SetLogValue("聯絡地址:鄰", addressRecord.Mailing.Area);
                    addressRecord.Mailing.Area = iRow.GetValue("聯絡地址:鄰").Trim();
                    logAgent.SetLogValue("聯絡地址:鄰", addressRecord.Mailing.Area);
                }
                if (mOption.SelectedFields.Contains("聯絡地址:其它") && !string.IsNullOrWhiteSpace(iRow.GetValue("聯絡地址:其它")))
                {
                    logAgent.SetLogValue("聯絡地址:其它", addressRecord.Mailing.Detail);
                    addressRecord.Mailing.Detail = iRow.GetValue("聯絡地址:其它").Trim();
                    logAgent.SetLogValue("聯絡地址:其它", addressRecord.Mailing.Detail);
                }

                if (mOption.SelectedFields.Contains("住家地址:郵遞區號") && !string.IsNullOrWhiteSpace(iRow.GetValue("住家地址:郵遞區號")))
                {
                    logAgent.SetLogValue("住家地址:郵遞區號", addressRecord.Permanent.ZipCode);
                    addressRecord.Permanent.ZipCode = iRow.GetValue("住家地址:郵遞區號").Trim();
                    logAgent.SetLogValue("住家地址:郵遞區號", addressRecord.Permanent.ZipCode);
                }
                if (mOption.SelectedFields.Contains("住家地址:縣市") && !string.IsNullOrWhiteSpace(iRow.GetValue("住家地址:縣市")))
                {
                    logAgent.SetLogValue("住家地址:縣市", addressRecord.Permanent.County);
                    addressRecord.Permanent.County = iRow.GetValue("住家地址:縣市").Trim();
                    logAgent.SetLogValue("住家地址:縣市", addressRecord.Permanent.County);
                }
                if (mOption.SelectedFields.Contains("住家地址:鄉鎮") && !string.IsNullOrWhiteSpace(iRow.GetValue("住家地址:鄉鎮")))
                {
                    logAgent.SetLogValue("住家地址:鄉鎮", addressRecord.Permanent.Town);
                    addressRecord.Permanent.Town = iRow.GetValue("住家地址:鄉鎮").Trim();
                    logAgent.SetLogValue("住家地址:鄉鎮", addressRecord.Permanent.Town);
                }
                if (mOption.SelectedFields.Contains("住家地址:村里") && !string.IsNullOrWhiteSpace(iRow.GetValue("住家地址:村里")))
                {
                    logAgent.SetLogValue("住家地址:村里", addressRecord.Permanent.District);
                    addressRecord.Permanent.District = iRow.GetValue("住家地址:村里").Trim();
                    logAgent.SetLogValue("住家地址:村里", addressRecord.Permanent.District);
                }
                if (mOption.SelectedFields.Contains("住家地址:鄰") && !string.IsNullOrWhiteSpace(iRow.GetValue("住家地址:鄰")))
                {
                    logAgent.SetLogValue("住家地址:鄰", addressRecord.Permanent.Area);
                    addressRecord.Permanent.Area = iRow.GetValue("住家地址:鄰").Trim();
                    logAgent.SetLogValue("住家地址:鄰", addressRecord.Permanent.Area);
                }
                if (mOption.SelectedFields.Contains("住家地址:其它") && !string.IsNullOrWhiteSpace(iRow.GetValue("住家地址:其它")))
                {
                    logAgent.SetLogValue("住家地址:其它", addressRecord.Permanent.Detail);
                    addressRecord.Permanent.Detail = iRow.GetValue("住家地址:其它").Trim();
                    logAgent.SetLogValue("住家地址:其它", addressRecord.Permanent.Detail);
                }

                if (mOption.SelectedFields.Contains("公司地址:郵遞區號") && !string.IsNullOrWhiteSpace(iRow.GetValue("公司地址:郵遞區號")))
                {
                    logAgent.SetLogValue("公司地址:郵遞區號", addressRecord.Address1.ZipCode);
                    addressRecord.Address1.ZipCode = iRow.GetValue("公司地址:郵遞區號").Trim();
                    logAgent.SetLogValue("公司地址:郵遞區號", addressRecord.Address1.ZipCode);
                }
                if (mOption.SelectedFields.Contains("公司地址:縣市") && !string.IsNullOrWhiteSpace(iRow.GetValue("公司地址:縣市")))
                {
                    logAgent.SetLogValue("公司地址:縣市", addressRecord.Address1.County);
                    addressRecord.Address1.County = iRow.GetValue("公司地址:縣市").Trim();
                    logAgent.SetLogValue("公司地址:縣市", addressRecord.Address1.County);
                }
                if (mOption.SelectedFields.Contains("公司地址:鄉鎮") && !string.IsNullOrWhiteSpace(iRow.GetValue("公司地址:鄉鎮")))
                {
                    logAgent.SetLogValue("公司地址:鄉鎮", addressRecord.Address1.Town);
                    addressRecord.Address1.Town = iRow.GetValue("公司地址:鄉鎮").Trim();
                    logAgent.SetLogValue("公司地址:鄉鎮", addressRecord.Address1.Town);
                }
                if (mOption.SelectedFields.Contains("公司地址:村里") && !string.IsNullOrWhiteSpace(iRow.GetValue("公司地址:村里")))
                {
                    logAgent.SetLogValue("公司地址:村里", addressRecord.Address1.District);
                    addressRecord.Address1.District = iRow.GetValue("公司地址:村里").Trim();
                    logAgent.SetLogValue("公司地址:村里", addressRecord.Address1.District);
                }
                if (mOption.SelectedFields.Contains("公司地址:鄰") && !string.IsNullOrWhiteSpace(iRow.GetValue("公司地址:鄰")))
                {
                    logAgent.SetLogValue("公司地址:鄰", addressRecord.Address1.Area);
                    addressRecord.Address1.Area = iRow.GetValue("公司地址:鄰").Trim();
                    logAgent.SetLogValue("公司地址:鄰", addressRecord.Address1.Area);
                }
                if (mOption.SelectedFields.Contains("公司地址:其它") && !string.IsNullOrWhiteSpace(iRow.GetValue("公司地址:其它")))
                {
                    logAgent.SetLogValue("公司地址:其它", addressRecord.Address1.Detail);
                    addressRecord.Address1.Detail = iRow.GetValue("公司地址:其它").Trim();
                    logAgent.SetLogValue("公司地址:其它", addressRecord.Address1.Detail);
                }

                updateRecords.Add(addressRecord);
            }
            Address.Update(updateRecords);

            return message;
        }

        private string UpdatePhone(IEnumerable<IRowStream> Rows, Dictionary<string, StudentRecord> dicStudents)
        {
            string message = string.Empty;
            List<PhoneRecord> oPhones = Phone.SelectByStudentIDs(dicStudents.Values.Select(x => x.ID));
            Dictionary<string, PhoneRecord> dicPhones = new Dictionary<string, PhoneRecord>();
            if (oPhones.Count > 0)
                dicPhones = oPhones.ToDictionary(x => x.RefStudentID);

            List<PhoneRecord> updateRecords = new List<PhoneRecord>();

            foreach (IRowStream iRow in Rows)
            {
                PhoneRecord phoneRecord = new PhoneRecord();
                string student_IDNumber = iRow.GetValue("學號").Trim().ToUpper();

                if (dicPhones.ContainsKey(dicStudents[student_IDNumber].ID))
                    phoneRecord = dicPhones[dicStudents[student_IDNumber].ID];

                Log.LogAgent logAgent = dicLogAgents[student_IDNumber];

                phoneRecord.RefStudentID = dicStudents[student_IDNumber].ID;

                if (mOption.SelectedFields.Contains("住家電話") && !string.IsNullOrWhiteSpace(iRow.GetValue("住家電話")))
                {
                    logAgent.SetLogValue("住家電話", phoneRecord.Permanent);
                    phoneRecord.Permanent = iRow.GetValue("住家電話").Trim();
                    logAgent.SetLogValue("住家電話", phoneRecord.Permanent);
                }
                if (mOption.SelectedFields.Contains("聯絡電話") && !string.IsNullOrWhiteSpace(iRow.GetValue("聯絡電話")))
                {
                    logAgent.SetLogValue("聯絡電話", phoneRecord.Contact);
                    phoneRecord.Contact = iRow.GetValue("聯絡電話").Trim();
                    logAgent.SetLogValue("聯絡電話", phoneRecord.Contact);
                }
                if (mOption.SelectedFields.Contains("行動電話1") && !string.IsNullOrWhiteSpace(iRow.GetValue("行動電話1")))
                {
                    logAgent.SetLogValue("行動電話1", phoneRecord.Cell);
                    phoneRecord.Cell = iRow.GetValue("行動電話1").Trim();
                    logAgent.SetLogValue("行動電話1", phoneRecord.Cell);
                }
                if (mOption.SelectedFields.Contains("公司電話") && !string.IsNullOrWhiteSpace(iRow.GetValue("公司電話")))
                {
                    logAgent.SetLogValue("公司電話", phoneRecord.Phone1);
                    phoneRecord.Phone1 = iRow.GetValue("公司電話").Trim();
                    logAgent.SetLogValue("公司電話", phoneRecord.Phone1);
                }
                if (mOption.SelectedFields.Contains("行動電話2") && !string.IsNullOrWhiteSpace(iRow.GetValue("行動電話2")))
                {
                    logAgent.SetLogValue("行動電話2", phoneRecord.Phone2);
                    phoneRecord.Phone2 = iRow.GetValue("行動電話2").Trim();
                    logAgent.SetLogValue("行動電話2", phoneRecord.Phone2);
                }
                if (mOption.SelectedFields.Contains("秘書電話") && !string.IsNullOrWhiteSpace(iRow.GetValue("秘書電話")))
                {
                    logAgent.SetLogValue("秘書電話", phoneRecord.Phone3);
                    phoneRecord.Phone3 = iRow.GetValue("秘書電話").Trim();
                    logAgent.SetLogValue("秘書電話", phoneRecord.Phone3);
                }
                updateRecords.Add(phoneRecord);
            }
            Phone.Update(updateRecords);

            return message;
        }

        private string UpdateBrief2(IEnumerable<IRowStream> Rows, Dictionary<string, StudentRecord> dicStudents)
        {
            string message = string.Empty;

            List<StudentBrief2> _StudentBrief2 = Access.Select<StudentBrief2>(string.Format("ref_student_id in ({0})", String.Join(",", dicStudents.Values.Select(x => x.ID))));

            Dictionary<string, StudentBrief2> dicStudentBrief2 = new Dictionary<string, StudentBrief2>();
            _StudentBrief2.ForEach((x) =>
            {
                if (!dicStudentBrief2.ContainsKey(x.StudentID.ToString()))
                    dicStudentBrief2.Add(x.StudentID.ToString(), x);
            });

            foreach (IRowStream iRow in Rows)
            {
                string student_IDNumber = iRow.GetValue("學號").Trim().ToUpper();

                StudentBrief2 studentBrief2;

                if (dicStudentBrief2.ContainsKey(dicStudents[student_IDNumber].ID))
                    studentBrief2 = dicStudentBrief2[dicStudents[student_IDNumber].ID];
                else
                    studentBrief2 = new StudentBrief2();

                Log.LogAgent logAgent = dicLogAgents[student_IDNumber];

                studentBrief2.StudentID = int.Parse(dicStudents[student_IDNumber].ID);
                //  電子郵件要有更多的判斷
                if (mOption.SelectedFields.Contains("電子郵件一") || mOption.SelectedFields.Contains("電子郵件二") || mOption.SelectedFields.Contains("電子郵件三") || mOption.SelectedFields.Contains("電子郵件四") || mOption.SelectedFields.Contains("電子郵件五"))
                {
                    XDocument xDocument = XDocument.Parse("<?xml version='1.0' encoding='utf-8' ?><emails>" + studentBrief2.EmailList + "</emails>");

                    string email1 = (xDocument.Element("emails").Element("email1") == null ? "" : xDocument.Element("emails").Element("email1").Value);
                    string email2 = (xDocument.Element("emails").Element("email2") == null ? "" : xDocument.Element("emails").Element("email2").Value);
                    string email3 = (xDocument.Element("emails").Element("email3") == null ? "" : xDocument.Element("emails").Element("email3").Value);
                    string email4 = (xDocument.Element("emails").Element("email4") == null ? "" : xDocument.Element("emails").Element("email4").Value);
                    string email5 = (xDocument.Element("emails").Element("email5") == null ? "" : xDocument.Element("emails").Element("email5").Value);

                    if (mOption.SelectedFields.Contains("電子郵件一") && !string.IsNullOrWhiteSpace(iRow.GetValue("電子郵件一")))
                    {
                        logAgent.SetLogValue("電子郵件一", email1);
                        email1 = iRow.GetValue("電子郵件一").Trim();
                        logAgent.SetLogValue("電子郵件一", email1);
                    }
                    if (mOption.SelectedFields.Contains("電子郵件二") && !string.IsNullOrWhiteSpace(iRow.GetValue("電子郵件二")))
                    {
                        logAgent.SetLogValue("電子郵件二", email2);
                        email2 = iRow.GetValue("電子郵件二").Trim();
                        logAgent.SetLogValue("電子郵件二", email2);
                    }
                    if (mOption.SelectedFields.Contains("電子郵件三") && !string.IsNullOrWhiteSpace(iRow.GetValue("電子郵件三")))
                    {
                        logAgent.SetLogValue("電子郵件三", email3);
                        email3 = iRow.GetValue("電子郵件三").Trim();
                        logAgent.SetLogValue("電子郵件三", email3);
                    }
                    if (mOption.SelectedFields.Contains("電子郵件四") && !string.IsNullOrWhiteSpace(iRow.GetValue("電子郵件四")))
                    {
                        logAgent.SetLogValue("電子郵件四", email4);
                        email4 = iRow.GetValue("電子郵件四").Trim();
                        logAgent.SetLogValue("電子郵件四", email4);
                    }
                    if (mOption.SelectedFields.Contains("電子郵件五") && !string.IsNullOrWhiteSpace(iRow.GetValue("電子郵件五")))
                    {
                        logAgent.SetLogValue("電子郵件五", email5);
                        email5 = iRow.GetValue("電子郵件五").Trim();
                        logAgent.SetLogValue("電子郵件五", email5);
                    }

                    StringBuilder email = new StringBuilder();
                    email.Append("<email1>" + email1 + "</email1>");
                    email.Append("<email2>" + email2 + "</email2>");
                    email.Append("<email3>" + email3 + "</email3>");
                    email.Append("<email4>" + email4 + "</email4>");
                    email.Append("<email5>" + email5 + "</email5>");

                    studentBrief2.EmailList = email.ToString();
                }

                if (mOption.SelectedFields.Contains("入學年度") && !string.IsNullOrWhiteSpace(iRow.GetValue("入學年度")))
                {
                    logAgent.SetLogValue("入學年度", studentBrief2.EnrollYear);
                    studentBrief2.EnrollYear = iRow.GetValue("入學年度").Trim();
                    logAgent.SetLogValue("入學年度", studentBrief2.EnrollYear);
                }

                //  系所組別
                if (mOption.SelectedFields.Contains("系所組別") && !string.IsNullOrWhiteSpace(iRow.GetValue("系所組別")))
                {
                    logAgent.SetLogValue("系所組別", dicAllDepart_ID.ContainsKey(studentBrief2.DepartmentGroupID.ToString()) ? dicAllDepart_ID[studentBrief2.DepartmentGroupID.ToString()].Name : "");
                    studentBrief2.DepartmentGroupID = int.Parse(dicAllDepart_Name[iRow.GetValue("系所組別").Trim().ToUpper()].UID);
                    logAgent.SetLogValue("系所組別", iRow.GetValue("系所組別").Trim());
                }

                //  年級
                if (mOption.SelectedFields.Contains("年級") && !string.IsNullOrWhiteSpace(iRow.GetValue("年級")))
                {
                    logAgent.SetLogValue("年級", studentBrief2.GradeYear.ToString());
                    studentBrief2.GradeYear = int.Parse(iRow.GetValue("年級").Trim());
                    logAgent.SetLogValue("年級", studentBrief2.GradeYear.ToString());
                }
                //  畢業學年期
                //if (mOption.SelectedFields.Contains("畢業學年期") && !string.IsNullOrWhiteSpace(iRow.GetValue("畢業學年期")))
                //{
                //    studentBrief2.UpdateSchoolYearSemester = iRow.GetValue("畢業學年期").Trim();
                //    studentBrief2.UpdateCode = "G";
                //}

                if (string.IsNullOrEmpty(studentBrief2.UID))
                    _StudentBrief2.Add(studentBrief2);
            }

            //  更新 
            List<string> updatedCourseExtIDs = new List<string>();
            try
            {
                updatedCourseExtIDs = _StudentBrief2.SaveAll();
            }
            catch (System.Exception e)
            {
                System.Windows.Forms.MessageBox.Show(e.Message);
                return e.Message;
            }
            //  RaiseEvent
            if (updatedCourseExtIDs.Count > 0)
            {
                StudentBrief2.RaiseAfterUpdateEvent(this, new ParameterEventArgs(updatedCourseExtIDs));
            }

            return message;
        }

        private string UpdateEducationBackground(IEnumerable<IRowStream> Rows, Dictionary<string, StudentRecord> dicStudents)
        {
            string message = string.Empty;

            List<EducationBackground> _EducationBackground = Access.Select<EducationBackground>(string.Format("ref_student_id in ({0})", String.Join(",", dicStudents.Values.Select(x => x.ID))));

            Dictionary<string, List<EducationBackground>> dicEducationBackground = new Dictionary<string, List<EducationBackground>>();
            _EducationBackground.ForEach((x) =>
            {
                if (!dicEducationBackground.ContainsKey(x.StudentID.ToString()))
                    dicEducationBackground.Add(x.StudentID.ToString(), new List<EducationBackground>());

                dicEducationBackground[x.StudentID.ToString()].Add(x);
            });

            foreach (IRowStream iRow in Rows)
            {
                string student_IDNumber = iRow.GetValue("學號").Trim().ToUpper();

                EducationBackground educationBackground;

                StudentRecord student = dicStudents[student_IDNumber];

                if (dicEducationBackground.ContainsKey(student.ID))
                    educationBackground = dicEducationBackground[student.ID][0];
                else
                    educationBackground = new EducationBackground();

                Log.LogAgent logAgent = dicLogAgents[student_IDNumber];

				educationBackground.StudentID = int.Parse(student.ID);
				educationBackground.IsTop = true;

                if (mOption.SelectedFields.Contains("畢業學校") && !string.IsNullOrWhiteSpace(iRow.GetValue("畢業學校")))
                {
                    logAgent.SetLogValue("畢業學校", educationBackground.SchoolName);
                    educationBackground.SchoolName = iRow.GetValue("畢業學校").Trim();
                    logAgent.SetLogValue("畢業學校", educationBackground.SchoolName);
				}
				if (mOption.SelectedFields.Contains("畢業系所") && !string.IsNullOrWhiteSpace(iRow.GetValue("畢業系所")))
				{
					logAgent.SetLogValue("畢業系所", educationBackground.Department);
					educationBackground.Department = iRow.GetValue("畢業系所").Trim();
					logAgent.SetLogValue("畢業系所", educationBackground.Department);
				}
				if (mOption.SelectedFields.Contains("學位") && !string.IsNullOrWhiteSpace(iRow.GetValue("學位")))
				{
					logAgent.SetLogValue("學位", educationBackground.Degree);
					educationBackground.Degree = iRow.GetValue("學位").Trim();
					logAgent.SetLogValue("學位", educationBackground.Degree);
				}

                if (string.IsNullOrEmpty(educationBackground.UID))
                    _EducationBackground.Add(educationBackground);
            }
            //  更新 
            List<string> updatedCourseExtIDs = new List<string>();
            try
            {
                updatedCourseExtIDs = _EducationBackground.SaveAll();
            }
            catch (System.Exception e)
            {
                System.Windows.Forms.MessageBox.Show(e.Message);
                return e.Message;
            }
            //  RaiseEvent
            if (updatedCourseExtIDs.Count > 0)
            {
                EducationBackground.RaiseAfterUpdateEvent(this, new ParameterEventArgs(updatedCourseExtIDs));
            }

            return message;
        }

        private string UpdateExperience(IEnumerable<IRowStream> Rows, Dictionary<string, StudentRecord> dicStudents)
        {
            string message = string.Empty;

            List<Experience> _Experience = Access.Select<Experience>(string.Format("ref_student_id in ({0})", String.Join(",", dicStudents.Values.Select(x => x.ID))));

            Dictionary<string, List<Experience>> dicExperience = new Dictionary<string, List<Experience>>();
            _Experience.ForEach((x) =>
            {
                if (!dicExperience.ContainsKey(x.StudentID.ToString()))
                    dicExperience.Add(x.StudentID.ToString(), new List<Experience>());

                dicExperience[x.StudentID.ToString()].Add(x);
            });

            foreach (IRowStream iRow in Rows)
            {
                string student_IDNumber = iRow.GetValue("學號").Trim().ToUpper();

                Experience experienceRecord;

                StudentRecord student = dicStudents[student_IDNumber];

                if (dicExperience.ContainsKey(student.ID))
                    experienceRecord = dicExperience[student.ID][0];
                else
                    experienceRecord = new Experience();

                experienceRecord.StudentID = int.Parse(student.ID);
				experienceRecord.WorkStatus = "現職";

                Log.LogAgent logAgent = dicLogAgents[student_IDNumber];
                if (mOption.SelectedFields.Contains("公司名稱") && !string.IsNullOrWhiteSpace(iRow.GetValue("公司名稱")))
                {
                    logAgent.SetLogValue("公司名稱", experienceRecord.Company);
                    experienceRecord.Company = iRow.GetValue("公司名稱").Trim();
                    logAgent.SetLogValue("公司名稱", experienceRecord.Company);
                }

                if (mOption.SelectedFields.Contains("職稱") && !string.IsNullOrWhiteSpace(iRow.GetValue("職稱")))
                {
                    logAgent.SetLogValue("職稱", experienceRecord.Position);
                    experienceRecord.Position = iRow.GetValue("職稱").Trim();
                    logAgent.SetLogValue("職稱", experienceRecord.Position);
                }

                if (mOption.SelectedFields.Contains("產業別") && !string.IsNullOrWhiteSpace(iRow.GetValue("產業別")))
                {
                    logAgent.SetLogValue("產業別", experienceRecord.Industry);
                    experienceRecord.Industry = iRow.GetValue("產業別").Trim();
                    logAgent.SetLogValue("產業別", experienceRecord.Industry);
                }

                if (mOption.SelectedFields.Contains("部門類別") && !string.IsNullOrWhiteSpace(iRow.GetValue("部門類別")))
                {
                    logAgent.SetLogValue("部門類別", experienceRecord.DepartmentCategory);
                    experienceRecord.DepartmentCategory = iRow.GetValue("部門類別").Trim();
                    logAgent.SetLogValue("部門類別", experienceRecord.DepartmentCategory);
                }

                if (mOption.SelectedFields.Contains("層級別") && !string.IsNullOrWhiteSpace(iRow.GetValue("層級別")))
                {
                    logAgent.SetLogValue("層級別", experienceRecord.PostLevel);
                    experienceRecord.PostLevel = iRow.GetValue("層級別").Trim();
                    logAgent.SetLogValue("層級別", experienceRecord.PostLevel);
                }

                if (mOption.SelectedFields.Contains("工作地點") && !string.IsNullOrWhiteSpace(iRow.GetValue("工作地點")))
                {
                    logAgent.SetLogValue("工作地點", experienceRecord.WorkPlace);
                    experienceRecord.WorkPlace = iRow.GetValue("工作地點").Trim();
                    logAgent.SetLogValue("工作地點", experienceRecord.WorkPlace);
                }

				//if (mOption.SelectedFields.Contains("工作狀態") && !string.IsNullOrWhiteSpace(iRow.GetValue("工作狀態")))
				//{
				//	logAgent.SetLogValue("工作狀態", experienceRecord.WorkStatus);
				//	experienceRecord.WorkStatus = iRow.GetValue("工作狀態").Trim();
				//	logAgent.SetLogValue("工作狀態", experienceRecord.WorkStatus);
				//}

                if (mOption.SelectedFields.Contains("工作起日") && !string.IsNullOrWhiteSpace(iRow.GetValue("工作起日")))
                {
                    logAgent.SetLogValue("工作起日", experienceRecord.WorkBeginDate == null ? "" : DateTime.Parse(experienceRecord.WorkBeginDate.Value + "").ToString("yyyy/MM/dd"));
                    experienceRecord.WorkBeginDate = DateTime.Parse(iRow.GetValue("工作起日").Trim());
                    logAgent.SetLogValue("工作起日", experienceRecord.WorkBeginDate == null ? "" : DateTime.Parse(experienceRecord.WorkBeginDate.Value + "").ToString("yyyy/MM/dd"));
                }

                if (mOption.SelectedFields.Contains("工作迄日") && !string.IsNullOrWhiteSpace(iRow.GetValue("工作迄日")))
                {
                    logAgent.SetLogValue("工作迄日", experienceRecord.WorkEndDate == null ? "" : DateTime.Parse(experienceRecord.WorkEndDate.Value + "").ToString("yyyy/MM/dd"));
                    experienceRecord.WorkEndDate = DateTime.Parse(iRow.GetValue("工作迄日").Trim());
                    logAgent.SetLogValue("工作迄日", experienceRecord.WorkEndDate == null ? "" : DateTime.Parse(experienceRecord.WorkEndDate.Value + "").ToString("yyyy/MM/dd"));
                }

                if (string.IsNullOrEmpty(experienceRecord.UID))
                    _Experience.Add(experienceRecord);
            }

            //  更新 
            List<string> updatedCourseExtIDs = new List<string>();

            try
            {
                updatedCourseExtIDs = _Experience.SaveAll();
            }
            catch (System.Exception e)
            {
                System.Windows.Forms.MessageBox.Show(e.Message);
                return e.Message;
            }

            //  RaiseEvent
            if (updatedCourseExtIDs.Count > 0)
            {
                Experience.RaiseAfterUpdateEvent(this, new ParameterEventArgs(updatedCourseExtIDs));
            }
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
            return message;
        }
    }
}