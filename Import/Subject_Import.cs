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
using System.Xml.Linq;

namespace EMBACore.Import
{
    class Subject_Import : ImportWizard
    {
        ImportOption mOption;
        string keyField;
        AccessHelper Access;
        QueryHelper queryHelper;
        public Subject_Import()
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
            if (this.SelectedKeyFields.Contains("課號")) keyField = "課號";
            if (this.SelectedKeyFields.Contains("課程識別碼")) keyField = "課程識別碼";
            if (this.SelectedKeyFields.Contains("課程系統編號")) keyField = "課程系統編號";

            //  若「課程識別碼」不為空白，則必須存在於資料庫中。
            //  若「課號」不為空白，則必須存在於資料庫中。
            DataTable dataTableSubjects = queryHelper.Select("select uid, subject_code, new_subject_code, name from $ischool.emba.subject");
            IEnumerable<DataRow> subjects = dataTableSubjects.Rows.Cast<DataRow>();

            Rows.ForEach((x) =>
            {
                string subject_code = x.GetValue("課程識別碼").Trim();
                string new_subject_code = x.GetValue("課號").Trim();
                string course_id = x.GetValue("課程系統編號").Trim();

                //  「課程識別碼」不可重覆
                if (!string.IsNullOrEmpty(subject_code))
                {
                    if (Rows.Where(y => (y.GetValue("課程識別碼").Trim().ToUpper() == subject_code.ToUpper())).Count() > 1)
                        Messages[x.Position].MessageItems.Add(new MessageItem(EMBA.Validator.ErrorType.Error, EMBA.Validator.ValidatorType.Row, "課程識別碼重覆。"));
                }

                //  「課號」不可重覆
                if (!string.IsNullOrEmpty(new_subject_code))
                {
                    if (Rows.Where(y => (y.GetValue("課號").Trim().ToUpper() == new_subject_code.ToUpper())).Count() > 1)
                        Messages[x.Position].MessageItems.Add(new MessageItem(EMBA.Validator.ErrorType.Error, EMBA.Validator.ValidatorType.Row, "課號重覆。"));
                }

                //  若鍵值為「課程系統編號」，則必須存在於系統中
                if (keyField == "課程系統編號")
                {
                    if (!string.IsNullOrEmpty(course_id))
                    {
                        if (subjects.Where(y => (y["uid"] + "").Trim() == course_id).Count() == 0)
                            Messages[x.Position].MessageItems.Add(new MessageItem(EMBA.Validator.ErrorType.Error, EMBA.Validator.ValidatorType.Row, "系統無此課程系統編號。"));
                    }
                }
            });

            #endregion
        }

        public override XDocument GetValidateRule()
        {
            return XDocument.Parse(EMBACore.Properties.Resources.Subject_Import);
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
            List<UDT.Subject> ExistingSubjectRecords = Access.Select<UDT.Subject>();

            //  要新增的 CourseRecord 
            List<Subject> insertSubjectRecords = new List<Subject>();
            //  要更新的 CourseRecord
            List<Subject> updateSubjectRecords = new List<Subject>();
            foreach (IRowStream row in Rows)
            {
                Subject subjectRecord = new Subject();
                IEnumerable<Subject> filterSubjectRecords = new List<Subject>();
                //  若鍵值不為「課程系統編號」，則查出哪些課程是要新增的
                if (keyField != "課程系統編號")
                {
                    if (keyField == "課程識別碼")
                    {
                        filterSubjectRecords = ExistingSubjectRecords.Where(x => (x.SubjectCode.Trim().ToUpper() == row.GetValue("課程識別碼").Trim().ToUpper()));
                    }
                    if (keyField == "課號")
                    {
                        filterSubjectRecords = ExistingSubjectRecords.Where(x => (x.NewSubjectCode.Trim().ToUpper() == row.GetValue("課號").Trim().ToUpper()));
                    }                    
                }
                else
                    filterSubjectRecords = ExistingSubjectRecords.Where(x => x.UID == row.GetValue("課程系統編號").Trim());

                if (filterSubjectRecords.Count() > 0)
                    subjectRecord = filterSubjectRecords.ElementAt(0);

                //  寫入：課程中文名稱	課程英文名稱	課程識別碼	開課系所	系所代碼	學分數	內容簡介	網頁連結	必修	備註	課號    
                if (mOption.SelectedFields.Contains("課程中文名稱"))
                {
                    //  課程中文名稱不允許空白
                    subjectRecord.Name = row.GetValue("課程中文名稱").Trim();
                }
                if (mOption.SelectedFields.Contains("課程英文名稱") && !string.IsNullOrWhiteSpace(row.GetValue("課程英文名稱")))
                {
                    subjectRecord.EnglishName = row.GetValue("課程英文名稱").Trim();
                }
                if (keyField == "課程識別碼")
                    subjectRecord.SubjectCode = row.GetValue("課程識別碼").Trim();
                else
                {
                    if (mOption.SelectedFields.Contains("課程識別碼") && !string.IsNullOrWhiteSpace(row.GetValue("課程識別碼")))
                    {
                        subjectRecord.SubjectCode = row.GetValue("課程識別碼").Trim();
                    }
                }
                if (keyField == "課號")
                    subjectRecord.NewSubjectCode = row.GetValue("課號").Trim();
                else
                {
                    if (mOption.SelectedFields.Contains("課號") && !string.IsNullOrWhiteSpace(row.GetValue("課號")))
                    {
                        subjectRecord.SubjectCode = row.GetValue("課號").Trim();
                    }
                }
                if (mOption.SelectedFields.Contains("開課系所") && !string.IsNullOrWhiteSpace(row.GetValue("開課系所")))
                {
                    subjectRecord.DeptName = row.GetValue("開課系所").Trim();
                }
                if (mOption.SelectedFields.Contains("系所代碼") && !string.IsNullOrWhiteSpace(row.GetValue("系所代碼")))
                {
                    subjectRecord.DeptCode = row.GetValue("系所代碼").Trim();
                }
                if (mOption.SelectedFields.Contains("學分數"))
                {
                    int credit = 0;
                    int.TryParse(row.GetValue("學分數").Trim(), out credit);
                    //  學分數必須有值
                    subjectRecord.Credit = credit;
                }
                //  若有選擇「必選修」，則「必選修」必定有資料(已驗證)
                if (mOption.SelectedFields.Contains("必選修"))
                {
                    subjectRecord.IsRequired = (row.GetValue("必選修").Trim() == "必修" ? true : false);
                }
                if (mOption.SelectedFields.Contains("內容簡介") && !string.IsNullOrWhiteSpace(row.GetValue("內容簡介")))
                {
                    subjectRecord.Description = row.GetValue("內容簡介").Trim();
                }
                if (mOption.SelectedFields.Contains("網頁連結") && !string.IsNullOrWhiteSpace(row.GetValue("網頁連結")))
                {
                    subjectRecord.WebUrl = row.GetValue("網頁連結").Trim();
                }
                if (mOption.SelectedFields.Contains("備註") && !string.IsNullOrWhiteSpace(row.GetValue("備註")))
                {
                    subjectRecord.Remark = row.GetValue("備註").Trim();
                }

                if (subjectRecord.RecordStatus == RecordStatus.Insert)
                    insertSubjectRecords.Add(subjectRecord);
                else
                    updateSubjectRecords.Add(subjectRecord);
            }
            //  新增科目
            List<string> insertedSubjectIDs = new List<string>();
            try
            {
                insertedSubjectIDs = insertSubjectRecords.SaveAll();
            }
            catch (System.Exception e)
            {
                System.Windows.Forms.MessageBox.Show(e.Message);
                return e.Message;
            }    
            //  更新科目
            List<string> updatedSubjectIDs = new List<string>();
            try
            {
                updatedSubjectIDs = updateSubjectRecords.SaveAll();
            }
            catch (System.Exception e)
            {
                System.Windows.Forms.MessageBox.Show(e.Message);
                return e.Message;
            }    

            //  RaiseEvent
            if (insertedSubjectIDs.Count > 0 || updatedSubjectIDs.Count > 0)
            {
                IEnumerable<string> uids = insertedSubjectIDs.Union(updatedSubjectIDs);
                UDT.Subject.RaiseAfterUpdateEvent();
            }
            return string.Empty;
        }
    }
}