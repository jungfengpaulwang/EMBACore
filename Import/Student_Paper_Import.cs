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

namespace EMBACore.Import
{
    class Student_Paper_Import : ImportWizard
    {
        ImportOption mOption;
        IEnumerable<DataRow> students;
        DataTable dataTableStudent;
        Dictionary<string, string> studentsMapping;

        public Student_Paper_Import()
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

            List<KeyValuePair<int, string>> student_Number = new List<KeyValuePair<int, string>>();

            string strSQL = string.Empty;

            //  抽出匯入資料之「學號」
            Rows.ForEach((x) =>
            {
                student_Number.Add(new KeyValuePair<int, string>(x.Position, x.GetValue("學號").Trim().ToUpper()));
                if (this.SelectedFields.Contains("論文題目"))
                {
                    Encoding.UTF8.GetBytes(x.GetValue("論文題目").Trim()).ToList().ForEach((y) =>
                    {
                        if (y<32)
                            Messages[x.Position].MessageItems.Add(new MessageItem(EMBA.Validator.ErrorType.Error, EMBA.Validator.ValidatorType.Row, "「論文題目」包含代碼為「" + y + "」的特殊字元。"));
                    });
                }
            });

            if (student_Number.Count == 0)
                return;

            //  以匯入學生的學號查得系統中的學生
            strSQL = string.Format(@"select student.id 學生系統編號, student.student_number 學號, student.status 狀態 from student");
            dataTableStudent = queryHelper.Select(strSQL);

            students = dataTableStudent.Rows.Cast<DataRow>();
            //  1、學號必須存在於系統
            foreach (KeyValuePair<int, string> kv in student_Number)
            {
                //  驗證規則已檢查學號不得為空白，故於此處略過不驗
                if (string.IsNullOrWhiteSpace(kv.Value))
                    continue;

                if (students.Where(x => (x["學號"] + "").Trim().ToUpper() == kv.Value.Trim().ToUpper()).Count() == 0)
                    Messages[kv.Key].MessageItems.Add(new MessageItem(EMBA.Validator.ErrorType.Error, EMBA.Validator.ValidatorType.Row, "系統無此學號。"));
            }

            //  2、學生狀態必須為一般(若狀態不為「一般」，則驗證為「Error」)
            //foreach (KeyValuePair<int, string> kv in student_Number)
            //{
            //    foreach (DataRow row in students.Where(x => (x["狀態"] + "") != "1").Where(x => (x["學號"] + "").Trim() == kv.Value.Trim()).Select(x => x))
            //    {
            //        Messages[kv.Key].MessageItems.Add(new MessageItem(EMBA.Validator.ErrorType.Error, EMBA.Validator.ValidatorType.Row, "學生狀態非「一般」。"));
            //    }
            //}

            studentsMapping = new Dictionary<string, string>();
            foreach (DataRow row in students)
                if (!studentsMapping.ContainsKey((row["學號"] + "").Trim().ToUpper()))
                    studentsMapping.Add((row["學號"] + "").Trim().ToUpper(), (row["學生系統編號"] + "").Trim());

            #endregion
        }

        public override XDocument GetValidateRule()
        {
            return XDocument.Parse(EMBACore.Properties.Resources.Student_Paper_Import);
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
            if (Rows.Count == 0)
                return string.Empty;

            Dictionary<string, int> dicImportInfos = new Dictionary<string, int>();
            List<Paper> records = new List<Paper>();
            string message = string.Empty;

            AccessHelper Access = new AccessHelper();
            string studentIDs = String.Join(",", studentsMapping.Values.Select(x => x));

            records = Access.Select<Paper>(string.Format("ref_student_id in ({0})", studentIDs)); 
            Log.BatchLogAgent BatchLogAgent = new Log.BatchLogAgent();
            foreach (IRowStream row in Rows)
            {
                string studentNumber = row.GetValue("學號").Trim().ToUpper();

                IEnumerable<Paper> record = records.Where(x => (x.StudentID.ToString() == studentsMapping[studentNumber]));
                
                Paper p = new Paper();
                if (record.Count() > 0)
                    p=record.ElementAt(0);

                p.StudentID = int.Parse(studentsMapping[studentNumber]);

                Log.LogAgent logAgent = new Log.LogAgent();
                if (string.IsNullOrEmpty(p.UID))
                    logAgent.ActionType = Log.LogActionType.AddNew;
                else
                    logAgent.ActionType = Log.LogActionType.Update;

                logAgent.Set("學生.匯入.指導教授及論文", "", "", Log.LogTargetCategory.Student, p.StudentID.ToString());

                if (this.mOption.SelectedFields.Contains("論文題目") && !string.IsNullOrWhiteSpace(row.GetValue("論文題目")))
                {
                    if (!string.IsNullOrEmpty(p.UID))
                        logAgent.SetLogValue("論文題目", p.PaperName);
                    p.PaperName = row.GetValue("論文題目").Trim();
                    logAgent.SetLogValue("論文題目", p.PaperName);
                }
                //if (this.mOption.SelectedFields.Contains("論文英文名稱")) p.PaperEnglishName = row.GetValue("論文英文名稱").Trim();
                if (this.mOption.SelectedFields.Contains("是否公開紙本論文"))
                {
                    if (!string.IsNullOrEmpty(p.UID))                        
                        logAgent.SetLogValue("是否公開紙本論文", p.IsPublic ? "是" : "否");
                    p.IsPublic = ((row.GetValue("是否公開紙本論文").Trim() == "是") ? true : false);
                    logAgent.SetLogValue("是否公開紙本論文", p.IsPublic ? "是" : "否");
                }

                if (this.mOption.SelectedFields.Contains("延後公開期限") && !string.IsNullOrWhiteSpace(row.GetValue("延後公開期限")))
                {
                    if (!string.IsNullOrEmpty(p.UID))
                        logAgent.SetLogValue("延後公開期限", p.PublishedDate);
                    p.PublishedDate = row.GetValue("延後公開期限").Trim();
                    logAgent.SetLogValue("延後公開期限", p.PublishedDate);
                }
                if (this.mOption.SelectedFields.Contains("書籍狀況") && !string.IsNullOrWhiteSpace(row.GetValue("書籍狀況")))
                {
                    if (!string.IsNullOrEmpty(p.UID))
                        logAgent.SetLogValue("書籍狀況", p.Description);
                    p.Description = row.GetValue("書籍狀況").Trim();
                    logAgent.SetLogValue("書籍狀況", p.Description);
                }
                if (this.mOption.SelectedFields.Contains("學年度") && !string.IsNullOrWhiteSpace(row.GetValue("學年度")))
                {
                    if (!string.IsNullOrEmpty(p.UID))
                        logAgent.SetLogValue("學年度", p.SchoolYear);
                    p.SchoolYear = row.GetValue("學年度").Trim();
                    logAgent.SetLogValue("學年度", p.SchoolYear);
                }
                if (this.mOption.SelectedFields.Contains("學期") && !string.IsNullOrWhiteSpace(row.GetValue("學期")))
                {
                    if (!string.IsNullOrEmpty(p.UID))
                        logAgent.SetLogValue("學期", p.Semester);
                    p.Semester = row.GetValue("學期").Trim();
                    logAgent.SetLogValue("學期", p.Semester);
                }
                if (mOption.SelectedFields.Contains("指導教授1") || mOption.SelectedFields.Contains("指導教授2") || mOption.SelectedFields.Contains("指導教授3"))
                {
                    XDocument xDocument = XDocument.Parse("<?xml version='1.0' encoding='utf-8' ?><Advisors>" + p.AdvisorList + "</Advisors>");
                    
                    string advisor1 = string.Empty;
                    string advisor2 = string.Empty;
                    string advisor3 = string.Empty;

                    if (xDocument.Element("Advisors").Elements("Advisor").Count() > 0)
                    {
                        advisor1 = (xDocument.Element("Advisors").Elements("Advisor").ElementAt(0) == null ? "" : xDocument.Element("Advisors").Elements("Advisor").ElementAt(0).Value);
                        advisor2 = (xDocument.Element("Advisors").Elements("Advisor").ElementAt(1) == null ? "" : xDocument.Element("Advisors").Elements("Advisor").ElementAt(1).Value);
                        advisor3 = (xDocument.Element("Advisors").Elements("Advisor").ElementAt(2) == null ? "" : xDocument.Element("Advisors").Elements("Advisor").ElementAt(2).Value);
                    }                        

                    if (mOption.SelectedFields.Contains("指導教授1") && !string.IsNullOrWhiteSpace(row.GetValue("指導教授1")))
                    {
                        if (!string.IsNullOrEmpty(p.UID))
                            logAgent.SetLogValue("指導教授1", advisor1);
                        advisor1 = row.GetValue("指導教授1").Trim();
                        logAgent.SetLogValue("指導教授1", advisor1);
                    }
                    if (mOption.SelectedFields.Contains("指導教授2") && !string.IsNullOrWhiteSpace(row.GetValue("指導教授2")))
                    {
                        if (!string.IsNullOrEmpty(p.UID))
                            logAgent.SetLogValue("指導教授2", advisor2);
                        advisor2 = row.GetValue("指導教授2").Trim();
                        logAgent.SetLogValue("指導教授2", advisor2);
                    }
                    if (mOption.SelectedFields.Contains("指導教授3") && !string.IsNullOrWhiteSpace(row.GetValue("指導教授3")))
                    {
                        if (!string.IsNullOrEmpty(p.UID))
                            logAgent.SetLogValue("指導教授3", advisor3);
                        advisor3 = row.GetValue("指導教授3").Trim();
                        logAgent.SetLogValue("指導教授3", advisor3);
                    }
                    StringBuilder advisorList = new StringBuilder();
                    advisorList.Append("<Advisor TeacherID=''><Name>" + advisor1 + "</Name></Advisor>");
                    advisorList.Append("<Advisor TeacherID=''><Name>" + advisor2 + "</Name></Advisor>");
                    advisorList.Append("<Advisor TeacherID=''><Name>" + advisor3 + "</Name></Advisor>");
                    p.AdvisorList = advisorList.ToString();
                }
                if (string.IsNullOrEmpty(p.UID))
                    records.Add(p);
                //try
                //{
                //    p.Save();
                //}
                //catch (Exception ex)
                //{
                //    System.Windows.Forms.MessageBox.Show(ex.Message);
                //}
                BatchLogAgent.AddLogAgent(logAgent);
            }
            //  更新 
            List<string> updatedCourseExtIDs = new List<string>();

            try
            {
                updatedCourseExtIDs = records.SaveAll();
            }
            catch (Exception e)
            {
                System.Windows.Forms.MessageBox.Show(e.Message);
                return e.Message;
            }

            //  RaiseEvent
            if (updatedCourseExtIDs.Count > 0)
            {
                Paper.RaiseAfterUpdateEvent(this, new ParameterEventArgs(updatedCourseExtIDs));
            }
            try
            {
                BatchLogAgent.Save();
            }
            catch (Exception e)
            {
                System.Windows.Forms.MessageBox.Show(e.Message);
                return string.Empty;
            }
            return message;
        }
    }
}
