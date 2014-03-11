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
using System.Text.RegularExpressions;

namespace EMBACore.Import
{
    class Teacher_Import : ImportWizard
    {
        ImportOption mOption;
        string keyField;
        string importType;
        AccessHelper Access;
        QueryHelper queryHelper;
        DataTable dataTable_ImportTarget;
        Dictionary<string, string> dicTeacherIDMappings;

        public Teacher_Import()
        {
            this.IsSplit = false;
            this.ShowAdvancedForm = false;
            this.ValidateRuleFormater = XDocument.Parse(EMBACore.Properties.Resources.format);
            //this.CustomValidate = null;
            //this.SplitThreadCount = 5;
            //this.SplitSize = 3000;

            keyField = string.Empty;
            importType = string.Empty;
            Access = new AccessHelper();
            queryHelper = new QueryHelper();
            dataTable_ImportTarget = new DataTable();
            dicTeacherIDMappings = new Dictionary<string, string>();
            this.CustomValidate = (Rows, Messages) =>
            {
                CustomValidator(Rows, Messages);
            };
        }

        public void CustomValidator(List<IRowStream> Rows, RowMessages Messages)
        {
            #  region 驗證流程
            if (this.SelectedKeyFields.Contains("教師系統編號")) keyField = "教師系統編號";
            if (this.SelectedKeyFields.Contains("姓名") && this.SelectedKeyFields.Contains("暱稱")) keyField = "姓名+暱稱";

            //  若「教師系統編號」為鍵值，則必須存在於資料庫中。
            //  「身分證號+教師狀態」在資料庫的設定為唯一。
            //  「登入帳號」在資料庫的設定為唯一。
            dataTable_ImportTarget = queryHelper.Select("select id, teacher_name, id_number, status, st_login_name, nickname, te.uid from teacher left join $ischool.emba.teacher_ext as te on te.ref_teacher_id=teacher.id");
            IEnumerable<DataRow> ImportTargets = dataTable_ImportTarget.Rows.Cast<DataRow>();
            if (ImportTargets.Count() > 0)
            {
                ImportTargets.ToList().ForEach((x) =>
                {
                    string key_Value = string.Empty;
                    if (keyField == "教師系統編號")
                        key_Value = x["id"] + "";
                    else if (keyField == "姓名+暱稱")
                        key_Value = x["teacher_name"] + "_" + x["nickname"];

                    if (!dicTeacherIDMappings.ContainsKey(key_Value))
                        dicTeacherIDMappings.Add(key_Value, x["id"] + "");
                });
            }
            Rows.ForEach((x) =>
            {
                string teacherID = x.GetValue("教師系統編號").Trim();
                string teacherName = x.GetValue("姓名").Trim();
                string nickName = x.GetValue("暱稱").Trim();
                string loginName = x.GetValue("登入帳號").Trim();
                string idNumber = x.GetValue("身分證號").Trim();
                string status = x.GetValue("教師狀態").Trim();

                //  1、先判斷匯入資料是「新增」還是「更新」
                //  鍵值為「教師系統編號」，匯入方式必為「更新」
                if (keyField == "教師系統編號")
                {
                    //  「教師系統編號」必須存在於資料庫中(機制已驗空白，故略過空白)
                    if (!string.IsNullOrEmpty(teacherID))
                    {
                        if (ImportTargets.Where(y => (y["id"] + "").Trim() == teacherID).Count() == 0)
                            Messages[x.Position].MessageItems.Add(new MessageItem(EMBA.Validator.ErrorType.Error, EMBA.Validator.ValidatorType.Row, "系統無此教師系統編號。"));
                    }
                    importType = "更新";
                }

                //  若「姓名+暱稱」為鍵值，且匯入資料存在於資料庫中，則匯入方式為「更新」，反之為「新增」
                if (keyField == "姓名+暱稱")
                {
                    //  機制已驗空白，故略過空白
                    if (!string.IsNullOrEmpty(teacherName) && !string.IsNullOrEmpty(nickName))
                    {
                        if (ImportTargets.Where(y => ((y["teacher_name"] + "").Trim() == teacherName && (y["nickname"] + "").Trim() == nickName)).Count() == 0)
                            importType = "新增";
                        else
                            importType = "更新";
                    }
                }

                //  「登入帳號」不可重覆
                if (!string.IsNullOrEmpty(loginName))
                {
                    string msg = string.Empty;
                    if (importType == "新增")
                    {
                        IEnumerable<DataRow> teachers = ImportTargets.Where(y => y["st_login_name"] != null).Where(y => (y["st_login_name"] + "").Trim().ToUpper() == loginName.ToUpper());
                        //  資料庫若已存在則為錯誤
                        if (teachers.Count() > 0)
                        {
                            msg = "資料庫已存在登入帳號「" + loginName + "」，擁有人為「" + (teachers.ElementAt(0)["teacher_name"] + "" + teachers.ElementAt(0)["nickname"]) + "」。";
                            Messages[x.Position].MessageItems.Add(new MessageItem(EMBA.Validator.ErrorType.Error, EMBA.Validator.ValidatorType.Row, msg));
                        }
                    }
                    if (importType == "更新" && this.SelectedFields.Contains("登入帳號"))
                    {
                        //  排除本人資料後若已存在則為錯誤
                        IEnumerable<DataRow> teachers = ImportTargets.Where(y => y["st_login_name"] != null).Where(y => (y["st_login_name"] + "").Trim().ToUpper() == loginName.ToUpper());
                        bool error = false;
                        if (teachers.Count() > 0)
                        {
                            if (keyField == "教師系統編號" && (teachers.ElementAt(0)["id"] + "") != teacherID)
                                error = true;
                            if (keyField == "姓名+暱稱" && ((teachers.ElementAt(0)["teacher_name"] + "" + teachers.ElementAt(0)["nickname"]) != (teacherName + nickName)))
                                error = true;

                            if (error)
                            {
                                msg = "資料庫已存在登入帳號「" + loginName + "」，擁有人為「" + (teachers.ElementAt(0)["teacher_name"] + "" + teachers.ElementAt(0)["nickname"]) + "」。";
                                Messages[x.Position].MessageItems.Add(new MessageItem(EMBA.Validator.ErrorType.Error, EMBA.Validator.ValidatorType.Row, msg));
                            }
                        }
                    }
                }

                //  「登入帳號」必須為有效的電子郵件格式
                if (this.SelectedFields.Contains("登入帳號") && !string.IsNullOrWhiteSpace(x.GetValue("登入帳號")))
                {
                    if (!isValidEmail(x.GetValue("登入帳號").Trim()))
                        Messages[x.Position].MessageItems.Add(new MessageItem(EMBA.Validator.ErrorType.Error, EMBA.Validator.ValidatorType.Row, "「登入帳號」格式不正確。"));
                }

                //  「電子信箱」必須為有效的電子郵件格式
                if (this.SelectedFields.Contains("電子信箱") && !string.IsNullOrWhiteSpace(x.GetValue("電子信箱")))
                {
                    if (!isValidEmail(x.GetValue("電子信箱").Trim()))
                        Messages[x.Position].MessageItems.Add(new MessageItem(EMBA.Validator.ErrorType.Error, EMBA.Validator.ValidatorType.Row, "「電子信箱」格式不正確。"));
                }

                //  「身分證號+教師狀態」不可重覆
                //  若「身分證號」為空白則不匯入故亦不驗證
                if (!string.IsNullOrEmpty(idNumber))
                {
                    string msg = string.Empty;
                    if (importType == "新增")
                    {
                        IEnumerable<DataRow> teachers = ImportTargets.Where(y => y["id_number"] != null).Where(y => ((y["id_number"] + "").Trim().ToUpper() == idNumber.ToUpper() && TransferTeacherStatusToString(y["status"] + "") == status));
                        //  資料庫若已存在則為錯誤
                        if (teachers.Count() > 0)
                        {
                            msg = "資料庫已存在身分證號「" + idNumber + "」，擁有人為「" + (teachers.ElementAt(0)["teacher_name"] + "" + teachers.ElementAt(0)["nickname"]) + "」。";
                            Messages[x.Position].MessageItems.Add(new MessageItem(EMBA.Validator.ErrorType.Error, EMBA.Validator.ValidatorType.Row, msg));
                        }
                    }
                    if (importType == "更新" && this.SelectedFields.Contains("身分證號"))
                    {
                        //  排除本人資料後若已存在則為錯誤
                        IEnumerable<DataRow> teachers = ImportTargets.Where(y => y["id_number"] != null).Where(y => ((y["id_number"] + "").Trim().ToUpper() == idNumber.ToUpper() && TransferTeacherStatusToString(y["status"] + "") == status));
                        bool error = false;
                        if (teachers.Count() > 0)
                        {
                            if (keyField == "教師系統編號" && (teachers.ElementAt(0)["id"] + "") != teacherID)
                                error = true;
                            if (keyField == "姓名+暱稱" && ((teachers.ElementAt(0)["teacher_name"] + "" + teachers.ElementAt(0)["nickname"]) != (teacherName + nickName)))
                                error = true;

                            if (error)
                            {
                                msg = "資料庫已存在身分證號「" + idNumber + "」，擁有人為「" + (teachers.ElementAt(0)["teacher_name"] + "" + teachers.ElementAt(0)["nickname"]) + "」。";
                                Messages[x.Position].MessageItems.Add(new MessageItem(EMBA.Validator.ErrorType.Error, EMBA.Validator.ValidatorType.Row, msg));
                            }
                        }
                    }
                }
            });

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

        private string TransferTeacherStatusToString(string status)
        {
            if (status == "1")
                return "一般";
            else if (status == "256")
                return "刪除";
            else return "未定義之教師狀態";
        }

        public override XDocument GetValidateRule()
        {
            return XDocument.Parse(EMBACore.Properties.Resources.Teacher_Import);
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
            List<TeacherRecord> ExistingRecords = Teacher.SelectAll();

            //  要新增的 TeacherRecord 
            List<TeacherRecord> insertRecords = new List<TeacherRecord>();
            //  要更新的 TeacherRecord
            List<TeacherRecord> updateRecords = new List<TeacherRecord>();
            foreach (IRowStream row in Rows)
            {
                string teacherID = row.GetValue("教師系統編號").Trim();
                string teacherName = row.GetValue("姓名").Trim();
                string nickName = row.GetValue("暱稱").Trim();
                string loginName = row.GetValue("登入帳號").Trim();
                string idNumber = row.GetValue("身分證號").Trim();
                string status = row.GetValue("教師狀態").Trim();

                TeacherRecord record = new TeacherRecord();
                IEnumerable<TeacherRecord> filterRecords = new List<TeacherRecord>();
                //  若鍵值不為「教師系統編號」，則查出哪些是要新增的
                if (keyField == "姓名+暱稱")
                {
                    filterRecords = ExistingRecords.Where(x => (x.Name.Trim() == teacherName && x.Nickname.Trim() == nickName));
                }
                if (keyField == "教師系統編號")
                    filterRecords = ExistingRecords.Where(x => x.ID == row.GetValue("教師系統編號").Trim());

                if (filterRecords.Count() > 0)
                    record = filterRecords.ElementAt(0);

                //  寫入：姓名、性別、身分證號、聯絡電話、教師狀態、電子信箱、登入帳號、暱稱
                if (keyField == "姓名+暱稱")
                {
                    record.Name = teacherName;
                    record.Nickname = nickName;
                }

                if (keyField == "教師系統編號")
                {
                    if (mOption.SelectedFields.Contains("姓名") && !string.IsNullOrEmpty(teacherName))
                        record.Name = teacherName;
                    if (mOption.SelectedFields.Contains("暱稱") && !string.IsNullOrEmpty(nickName))
                        record.Nickname = nickName;
                }

                if (mOption.SelectedFields.Contains("性別") && !string.IsNullOrEmpty(row.GetValue("性別").Trim()))
                    record.Gender = row.GetValue("性別").Trim();

                if (mOption.SelectedFields.Contains("身分證號") && !string.IsNullOrEmpty(idNumber))
                    record.IDNumber = idNumber;

                if (mOption.SelectedFields.Contains("聯絡電話") && !string.IsNullOrEmpty(row.GetValue("聯絡電話").Trim()))
                    record.ContactPhone = row.GetValue("聯絡電話").Trim();

                if (mOption.SelectedFields.Contains("教師狀態") && !string.IsNullOrEmpty(status))
                {
                    if (status == "一般")
                        record.Status = TeacherRecord.TeacherStatus.一般;
                    if (status == "刪除")
                        record.Status = TeacherRecord.TeacherStatus.刪除;
                }

                if (mOption.SelectedFields.Contains("電子信箱") && !string.IsNullOrEmpty(row.GetValue("電子信箱").Trim()))
                    record.Email = row.GetValue("電子信箱").Trim();

                if (mOption.SelectedFields.Contains("登入帳號") && !string.IsNullOrEmpty(row.GetValue("登入帳號").Trim()))
                    record.TALoginName = row.GetValue("登入帳號").Trim();

                if (string.IsNullOrEmpty(record.ID))
                    insertRecords.Add(record);
                else
                    updateRecords.Add(record);
            }
            //  新增  Teacher
            List<string> insertedRecordIDs = Teacher.Insert(insertRecords);
            //  更新  Teacher
            Teacher.Update(updateRecords);

            IEnumerable<string> affectedRecordIDs = insertedRecordIDs.Union(updateRecords.Select(x => x.ID));
            List<UDT.TeacherExtVO> existsTeacherExtRecords = Access.Select<UDT.TeacherExtVO>(string.Format("ref_teacher_id in ({0})", string.Join(",", affectedRecordIDs)));
            List<UDT.TeacherExtVO> insertTeacherExtRecords = new List<TeacherExtVO>();
            List<UDT.TeacherExtVO> updateTeacherExtRecords = new List<TeacherExtVO>();

            foreach (IRowStream row in Rows)
            {
                string teacherName = row.GetValue("姓名").Trim();
                string nickName = row.GetValue("暱稱").Trim();

                string teacherID = string.Empty;
                if (keyField == "教師系統編號")
                    teacherID = row.GetValue("教師系統編號").Trim();
                if (keyField == "姓名+暱稱")
                    teacherID = dicTeacherIDMappings[teacherName + "_" + nickName];

                UDT.TeacherExtVO teacherExt = new TeacherExtVO();

                IEnumerable<UDT.TeacherExtVO> filterTeacherExts = existsTeacherExtRecords.Where(x => x.TeacherID.ToString() == teacherID);

                if (filterTeacherExts.Count() > 0)
                    teacherExt = filterTeacherExts.ElementAt(0);

                teacherExt.TeacherID = int.Parse(teacherID);
                if (mOption.SelectedFields.Contains("個人網址") && !string.IsNullOrWhiteSpace(row.GetValue("個人網址"))) teacherExt.WebSiteUrl = row.GetValue("個人網址").Trim();
                if (mOption.SelectedFields.Contains("研究室") && !string.IsNullOrWhiteSpace(row.GetValue("研究室"))) teacherExt.Research = row.GetValue("研究室").Trim();
                if (mOption.SelectedFields.Contains("電話") && !string.IsNullOrWhiteSpace(row.GetValue("電話"))) teacherExt.Phone = row.GetValue("電話").Trim();
                if (mOption.SelectedFields.Contains("研究室電話") && !string.IsNullOrWhiteSpace(row.GetValue("研究室電話"))) teacherExt.OtherPhone = row.GetValue("研究室電話").Trim();
                if (mOption.SelectedFields.Contains("手機") && !string.IsNullOrWhiteSpace(row.GetValue("手機"))) teacherExt.Mobil = row.GetValue("手機").Trim();
                if (mOption.SelectedFields.Contains("備註") && !string.IsNullOrWhiteSpace(row.GetValue("備註"))) teacherExt.Memo = row.GetValue("備註").Trim();
                if (mOption.SelectedFields.Contains("所屬單位") && !string.IsNullOrWhiteSpace(row.GetValue("所屬單位"))) teacherExt.MajorWorkPlace = row.GetValue("所屬單位").Trim();
                if (mOption.SelectedFields.Contains("英文姓名") && !string.IsNullOrWhiteSpace(row.GetValue("英文姓名"))) teacherExt.EnglishName = row.GetValue("英文姓名").Trim();
                if (mOption.SelectedFields.Contains("教師編號") && !string.IsNullOrWhiteSpace(row.GetValue("教師編號"))) teacherExt.NtuSystemNo = row.GetValue("教師編號").Trim();
                if (mOption.SelectedFields.Contains("人事編號") && !string.IsNullOrWhiteSpace(row.GetValue("人事編號"))) teacherExt.EmployeeNo = row.GetValue("人事編號").Trim();
                if (mOption.SelectedFields.Contains("生日") && !string.IsNullOrWhiteSpace(row.GetValue("生日"))) teacherExt.Birthday = row.GetValue("生日").Trim();
                if (mOption.SelectedFields.Contains("戶籍地址") && !string.IsNullOrWhiteSpace(row.GetValue("戶籍地址"))) teacherExt.Address = row.GetValue("戶籍地址").Trim();

                if (teacherExt.RecordStatus == RecordStatus.Insert)
                    insertTeacherExtRecords.Add(teacherExt);
                else
                    updateTeacherExtRecords.Add(teacherExt);
            }
            List<string> insertedIDs = new List<string>();
            try
            {
                insertedIDs = insertTeacherExtRecords.SaveAll();
            }
            catch (System.Exception e)
            {
                System.Windows.Forms.MessageBox.Show(e.Message);
                return e.Message;
            }
            List<string> updatedIDs = new List<string>();
            try
            {
                updatedIDs = updateTeacherExtRecords.SaveAll();
            }
            catch (System.Exception e)
            {
                System.Windows.Forms.MessageBox.Show(e.Message);
                return e.Message;
            }    
            //  RaiseEvent
            if (insertedIDs.Count > 0 || updatedIDs.Count > 0)
            {
                IEnumerable<string> uids = insertedIDs.Union(updatedIDs);
                UDT.TeacherExtVO.RaiseAfterUpdateEvent(this, new ParameterEventArgs(uids));
            }
            return string.Empty;
        }
    }
}