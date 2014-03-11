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
    class Student_Payment_History_Import : ImportWizard
    {
        ImportOption mOption;
        IEnumerable<DataRow> students;
        DataTable dataTableStudent;

        public Student_Payment_History_Import()
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
            //  先移除匯入驗證機制已驗證之訊息(匯入機制已修正，無需覆蓋已驗證訊息)
            //List<MessageItem> messageItems;
            //foreach (IRowStream Row in Rows)
            //{
            //    //  必要欄位不存在的驗證訊息不移除
            //    if (Row.Position == 0)
            //        continue;

            //    messageItems = new List<MessageItem>();
            //    foreach (MessageItem messageItem in Messages[Row.Position].MessageItems)
            //        messageItems.Add(messageItem);

            //    messageItems.ForEach((x) => Messages[Row.Position].MessageItems.Remove(x));
            //}

            #  region 驗證流程

            QueryHelper queryHelper = new QueryHelper();
            dataTableStudent = new DataTable();

            List<KeyValuePair<int, string>> student_Number = new List<KeyValuePair<int, string>>();

            string strSQL = string.Empty;

            //  抽出匯入資料之「學號」
            Rows.ForEach((x) =>
            {
                student_Number.Add(new KeyValuePair<int, string>(x.Position, x.GetValue("學號").Trim().ToUpper()));
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
                if (students.Where(x => (x["學號"] + "").Trim().ToUpper() == kv.Value.Trim().ToUpper()).Count() == 0)
                    Messages[kv.Key].MessageItems.Add(new MessageItem(EMBA.Validator.ErrorType.Error, EMBA.Validator.ValidatorType.Row, "系統無此學號。"));
            }

            //  2、學生狀態必須為在學(若狀態不為「在學」，則驗證為「Error」)
            //foreach (KeyValuePair<int, string> kv in student_Number)
            //{
            //    foreach (DataRow row in students.Where(x => (x["狀態"] + "") != "1").Where(x => (x["學號"] + "").Trim() == kv.Value.Trim()).Select(x => x))
            //    {
            //        Messages[kv.Key].MessageItems.Add(new MessageItem(EMBA.Validator.ErrorType.Error, EMBA.Validator.ValidatorType.Row, "學生狀態非「在學」。"));
            //    }
            //}

            #endregion
        }

        public override XDocument GetValidateRule()
        {
            return XDocument.Parse(EMBACore.Properties.Resources.Student_Payment_History_Import);
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
            if (Rows.Count==0)
                return string.Empty;
            
            Dictionary<string, string> studentsMapping = new Dictionary<string, string>();
            foreach (DataRow row in students)
                if (!studentsMapping.ContainsKey((row["學號"] + "").Trim().ToUpper()))
                    studentsMapping.Add((row["學號"] + "").Trim().ToUpper(), (row["學生系統編號"] + "").Trim());

            Dictionary<string, int> dicImportInfos = new Dictionary<string, int>();
            List<PaymentHistory> records = new List<PaymentHistory>();
            List<PaymentHistory> updateRecords = new List<PaymentHistory>();
            List<PaymentHistory> insertRecords = new List<PaymentHistory>();
            string message = string.Empty;

            AccessHelper Access = new AccessHelper();
            string studentIDs = String.Join(",", studentsMapping.Values.Select(x => x));
            
            records = Access.Select<PaymentHistory>(string.Format("ref_student_id in ({0})", studentIDs));

            foreach (IRowStream row in Rows)
            {
                string studentNumber = row.GetValue("學號").Trim().ToUpper();

                foreach (string field in mOption.SelectedFields)
                {
                    if (field.IndexOf("學年期") == -1)
                        continue;

                    string schoolYearSemester = field.Split(':').ElementAt(1);
                    string schoolYear = schoolYearSemester.Substring(0, schoolYearSemester.Length - 1);
                    string semester = schoolYearSemester.Last().ToString();
                    string isPaied = row.GetValue(field).Trim();

                    IEnumerable<PaymentHistory> record = records.Where(x => (x.StudentID.ToString() == studentsMapping[studentNumber] && x.SchoolYear.ToString() == schoolYear && x.Semester.ToString() == semester));
                    if (record.Count() == 0)
                    {
                        PaymentHistory insertRecord = new PaymentHistory();

                        insertRecord.StudentID = int.Parse(studentsMapping[studentNumber]);
                        insertRecord.SchoolYear = int.Parse(schoolYear);
                        insertRecord.Semester = int.Parse(semester);
                        insertRecord.IsPaied = int.Parse(isPaied);
                        insertRecord.LastModifiedDate = System.DateTime.Now;

                        insertRecords.Add(insertRecord);
                    }
                    else
                    {
                        record.ElementAt(0).IsPaied = int.Parse(isPaied);
                        record.ElementAt(0).LastModifiedDate = System.DateTime.Now;
                        updateRecords.Add(record.ElementAt(0));
                    }
                }
            }
            //  新增繳費記錄
            List<string> insertedRecords = new List<string>();
            try
            {
                insertedRecords = insertRecords.SaveAll();
            }
            catch (System.Exception e)
            {
                System.Windows.Forms.MessageBox.Show(e.Message);
                return e.Message;
            }    
            //  修改繳費記錄
            List<string> updatedRecords = new List<string>();
            try
            {
                updatedRecords = updateRecords.SaveAll();
            }
            catch (System.Exception e)
            {
                System.Windows.Forms.MessageBox.Show(e.Message);
                return e.Message;
            }    

            //  RaiseEvent
            if (insertedRecords.Count > 0 || updatedRecords.Count > 0)
            {
                IEnumerable<string> uids = insertedRecords.Union(updatedRecords);
                UDT.PaymentHistory.RaiseAfterUpdateEvent(this, new ParameterEventArgs(uids));
            }
            return message;
        }
    }
}
