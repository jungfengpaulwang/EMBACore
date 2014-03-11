using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using EMBA.Import;
using FISCA.UDT;
using FISCA.Data;
using System.Xml.Linq;
using K12.Data;
using EMBA.DocumentValidator;
using EMBA.Validator;

namespace EMBACore.Import
{
    class Student_Experience_Import : ImportWizard
    {
        AccessHelper Access;
        QueryHelper queryHelper;
        ImportOption mOption;
        Dictionary<string, StudentRecord> dicStudents;

        public Student_Experience_Import()
        {
            this.IsSplit = false;
            this.ShowAdvancedForm = false;
            this.ValidateRuleFormater = XDocument.Parse(EMBACore.Properties.Resources.format);
            //this.CustomValidate = null;
            //this.SplitThreadCount = 5;
            //this.SplitSize = 3000;

            dicStudents = new Dictionary<string,StudentRecord>();
            Access = new AccessHelper();
            queryHelper = new QueryHelper();

            this.CustomValidate = (Rows, Messages) =>
            {
                CustomValidator(Rows, Messages);
            };
        }

        public override System.Xml.Linq.XDocument GetValidateRule()
        {
            return XDocument.Parse(EMBACore.Properties.Resources.Student_Experience_Import);
        }

        public override ImportAction GetSupportActions()
        {
            return ImportAction.InsertOrUpdate;
        }

        public override void Prepare(ImportOption Option)
        {
            mOption = Option;
        }

        public void CustomValidator(List<IRowStream> Rows, RowMessages Messages)
        {
            #  region 驗證流程

            List<EMBACore.UDT.ExperienceDataSource> ExperienceDataSources = Access.Select<EMBACore.UDT.ExperienceDataSource>();
            Dictionary<string, List<string>> dicExperiences = new Dictionary<string, List<string>>();
            List<StudentRecord> _Students = Student.SelectAll();
            List<UDT.Experience> _Experience = Access.Select<UDT.Experience>();
            Dictionary<string, List<string>> dicCompany = new Dictionary<string, List<string>>();
            if (_Students.Count == 0)
                throw new Exception("請先建立學生資料。");
            foreach (StudentRecord x in _Students)
            {
                if (!string.IsNullOrWhiteSpace(x.StudentNumber) && !dicStudents.ContainsKey(x.StudentNumber.Trim().ToUpper()))
                    dicStudents.Add(x.StudentNumber.Trim().ToUpper(), x);
            }
            _Experience.ForEach((x) =>
            {
                if (!dicCompany.ContainsKey(x.StudentID.ToString()))
                    dicCompany.Add(x.StudentID.ToString(), new List<string>());

                dicCompany[x.StudentID.ToString()].Add(x.Company);
            });
            foreach (EMBACore.UDT.ExperienceDataSource experienceDataSource in ExperienceDataSources)
            {
                if (!dicExperiences.ContainsKey(experienceDataSource.ItemCategory))
                    dicExperiences.Add(experienceDataSource.ItemCategory, new List<string>());

                dicExperiences[experienceDataSource.ItemCategory].Add(experienceDataSource.Item);
            }

            foreach (IRowStream x in Rows)
            {
                string status = string.Empty;
                string studentNumber = x.GetValue("學號").Trim().ToUpper();

                //  學號必須存在
                if (!dicStudents.ContainsKey(studentNumber))
                {
                    Messages[x.Position].MessageItems.Add(new MessageItem(EMBA.Validator.ErrorType.Error, EMBA.Validator.ValidatorType.Row, "學號不存在。"));
                    status = "error";
                }
                else
                {
                    if (dicCompany.ContainsKey((dicStudents[studentNumber].ID)))
                    {
                        if (dicCompany[dicStudents[studentNumber].ID].Contains(x.GetValue("公司名稱")))
                            status = "update";
                        else
                            status = "insert";
                    }
                    else
                        status = "insert";
                }
                //  職稱為必填
                if (status == "update" && this.SelectedFields.Contains("職稱") && string.IsNullOrWhiteSpace(x.GetValue("職稱")))
                    Messages[x.Position].MessageItems.Add(new MessageItem(EMBA.Validator.ErrorType.Error, EMBA.Validator.ValidatorType.Row, "「職稱」為必填。"));
                if (status == "insert" && string.IsNullOrWhiteSpace(x.GetValue("職稱")))
                    Messages[x.Position].MessageItems.Add(new MessageItem(EMBA.Validator.ErrorType.Error, EMBA.Validator.ValidatorType.Row, "「職稱」為必填。"));

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
            }

            #endregion
        }

        public override string Import(List<EMBA.DocumentValidator.IRowStream> Rows)
        {
            return UpdateExperience(Rows);
        }

        private string UpdateExperience(IEnumerable<IRowStream> Rows)
        {
            string message = string.Empty;
            Dictionary<string, Log.LogAgent> dicLogAgents = new Dictionary<string,Log.LogAgent>();

            List<UDT.Experience> _Experience = Access.Select<UDT.Experience>(string.Format("ref_student_id in ({0})", String.Join(",", dicStudents.Values.Select(x => x.ID))));

            Dictionary<string, List<UDT.Experience>> dicExperience = new Dictionary<string, List<UDT.Experience>>();
            _Experience.ForEach((x) =>
            {
                if (!dicExperience.ContainsKey(x.StudentID.ToString()))
                    dicExperience.Add(x.StudentID.ToString(), new List<UDT.Experience>());

                dicExperience[x.StudentID.ToString()].Add(x);
            });

            foreach (IRowStream iRow in Rows)
            {
                string student_IDNumber = iRow.GetValue("學號").Trim().ToUpper();
                StudentRecord student = dicStudents[student_IDNumber];
                bool bingo = false;

                if (dicExperience.ContainsKey(student.ID))
                {
                    List<UDT.Experience> Experiences = dicExperience[student.ID];

                    foreach (UDT.Experience oExperience in Experiences)
                    {
                        if (iRow.GetValue("公司名稱").Trim().ToLower() == oExperience.Company.Trim().ToLower())
                        {
                            bingo = true;
                            this.SetExperience(iRow, oExperience);
                        }
                    }
                }
                if (bingo)
                    continue;

                UDT.Experience nExperience = new UDT.Experience();
                nExperience.StudentID = int.Parse(student.ID);
                this.SetExperience(iRow, nExperience);
                
                if (!dicExperience.ContainsKey(student.ID))
                    dicExperience.Add(student.ID, new List<UDT.Experience>());

                dicExperience[student.ID].Add(nExperience);                
            }
            List<UDT.Experience> nExperiences = new List<UDT.Experience>();
            dicExperience.Values.ToList().ForEach(x => nExperiences.AddRange(x));
            try
            {
                nExperiences.SaveAll();
            }
            catch (System.Exception e)
            {
                System.Windows.Forms.MessageBox.Show(e.Message);
                return e.Message;
            }
            //  Log 待補
            //Log.LogAgent logAgent = dicLogAgents[student_IDNumber];
            //if (mOption.SelectedFields.Contains("公司名稱") && !string.IsNullOrWhiteSpace(iRow.GetValue("公司名稱")))
            //{
            //    logAgent.SetLogValue("公司名稱", ExperienceRecords.Company);
            //    ExperienceRecords.Company = iRow.GetValue("公司名稱").Trim();
            //    logAgent.SetLogValue("公司名稱", ExperienceRecords.Company);
            //}
            //Log.BatchLogAgent BatchLogAgent = new Log.BatchLogAgent();
            //BatchLogAgent.AddLogAgents(dicLogAgents.Values);
            //BatchLogAgent.Save();

            return message;
        }

        private void SetExperience(IRowStream iRow, UDT.Experience Experience)
        {
            Experience.Company = iRow.GetValue("公司名稱").Trim();

            if (!string.IsNullOrWhiteSpace(iRow.GetValue("職稱")))
            {
                if ((Experience.ActionBy + "").ToLower() == "student")
                {
                    if (string.IsNullOrWhiteSpace(Experience.Position))
                        Experience.Position = iRow.GetValue("職稱").Trim();
                }
                else
                    Experience.Position = iRow.GetValue("職稱").Trim();
            }
            if (!string.IsNullOrWhiteSpace(iRow.GetValue("產業別")))
            {
                if ((Experience.ActionBy + "").ToLower() == "student")
                {
                    if (string.IsNullOrWhiteSpace(Experience.Industry))
                        Experience.Industry = iRow.GetValue("產業別").Trim();
                }
                else
                    Experience.Industry = iRow.GetValue("產業別").Trim();
            }
            if (!string.IsNullOrWhiteSpace(iRow.GetValue("部門類別")))
            {
                if ((Experience.ActionBy + "").ToLower() == "student")
                {
                    if (string.IsNullOrWhiteSpace(Experience.DepartmentCategory))
                        Experience.DepartmentCategory = iRow.GetValue("部門類別").Trim();
                }
                else
                    Experience.DepartmentCategory = iRow.GetValue("部門類別").Trim();
            }
            if (!string.IsNullOrWhiteSpace(iRow.GetValue("層級別")))
            {
                if ((Experience.ActionBy + "").ToLower() == "student")
                {
                    if (string.IsNullOrWhiteSpace(Experience.PostLevel))
                        Experience.PostLevel = iRow.GetValue("層級別").Trim();
                }
                else
                    Experience.PostLevel = iRow.GetValue("層級別").Trim();
            }
            if (!string.IsNullOrWhiteSpace(iRow.GetValue("工作地點")))
            {
                if ((Experience.ActionBy + "").ToLower() == "student")
                {
                    if (string.IsNullOrWhiteSpace(Experience.WorkPlace))
                        Experience.WorkPlace = iRow.GetValue("工作地點").Trim();
                }
                else
                    Experience.WorkPlace = iRow.GetValue("工作地點").Trim();
            }
            if (!string.IsNullOrWhiteSpace(iRow.GetValue("工作狀態")))
            {
                if ((Experience.ActionBy + "").ToLower() == "student")
                {
                    if (string.IsNullOrWhiteSpace(Experience.WorkStatus))
                        Experience.WorkStatus = iRow.GetValue("工作狀態").Trim();
                }
                else
                    Experience.WorkStatus = iRow.GetValue("工作狀態").Trim();
            }

            DateTime work_begin_date;
            DateTime work_end_date;

            if (DateTime.TryParse(iRow.GetValue("工作起日").Trim(), out work_begin_date))
            {
                if ((Experience.ActionBy + "").ToLower() == "student")
                {
                    if (!Experience.WorkBeginDate.HasValue)
                        Experience.WorkBeginDate = work_begin_date;
                }
                else
                    Experience.WorkBeginDate = work_begin_date;
            }

            if (DateTime.TryParse(iRow.GetValue("工作迄日").Trim(), out work_end_date))
            {
                if ((Experience.ActionBy + "").ToLower() == "student")
                {
                    if (!Experience.WorkEndDate.HasValue)
                        Experience.WorkEndDate = work_end_date;
                }
                else
                    Experience.WorkEndDate = work_end_date;
            }

            if (!string.IsNullOrWhiteSpace(iRow.GetValue("公關連絡人")))
            {
                if ((Experience.ActionBy + "").ToLower() == "student")
                {
                    if (string.IsNullOrWhiteSpace(Experience.Publicist))
                        Experience.Publicist = iRow.GetValue("公關連絡人").Trim();
                }
                else
                    Experience.Publicist = iRow.GetValue("公關連絡人").Trim();
            }
            if (!string.IsNullOrWhiteSpace(iRow.GetValue("公關室電話")))
            {
                if ((Experience.ActionBy + "").ToLower() == "student")
                {
                    if (string.IsNullOrWhiteSpace(Experience.PublicRelationsOfficeTelephone))
                        Experience.PublicRelationsOfficeTelephone = iRow.GetValue("公關室電話").Trim();
                }
                else
                    Experience.PublicRelationsOfficeTelephone = iRow.GetValue("公關室電話").Trim();
            }
            if (!string.IsNullOrWhiteSpace(iRow.GetValue("公關室傳真")))
            {
                if ((Experience.ActionBy + "").ToLower() == "student")
                {
                    if (string.IsNullOrWhiteSpace(Experience.PublicRelationsOfficeFax))
                        Experience.PublicRelationsOfficeFax = iRow.GetValue("公關室傳真").Trim();
                }
                else
                    Experience.PublicRelationsOfficeFax = iRow.GetValue("公關室傳真").Trim();
            }
            if (!string.IsNullOrWhiteSpace(iRow.GetValue("公關Email")))
            {
                if ((Experience.ActionBy + "").ToLower() == "student")
                {
                    if (string.IsNullOrWhiteSpace(Experience.PublicistEmail))
                        Experience.PublicistEmail = iRow.GetValue("公關Email").Trim();
                }
                else
                    Experience.PublicistEmail = iRow.GetValue("公關Email").Trim();
            }
            if (!string.IsNullOrWhiteSpace(iRow.GetValue("公司網址")))
            {
                if ((Experience.ActionBy + "").ToLower() == "student")
                {
                    if (string.IsNullOrWhiteSpace(Experience.CompanyWebsite))
                        Experience.CompanyWebsite = iRow.GetValue("公司網址").Trim();
                }
                else
                    Experience.CompanyWebsite = iRow.GetValue("公司網址").Trim();
            }
            Experience.TimeStamp = DateTime.Now;
        }
    }
}
