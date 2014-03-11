using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FISCA.UDT;
using System.ComponentModel;

namespace EMBACore.UDT
{
    [FISCA.UDT.TableName("ischool.emba.subject")]
    public class Subject : ActiveRecord
    {
        internal static void RaiseAfterUpdateEvent()
        {
            if (Subject.AfterUpdate != null)
                Subject.AfterUpdate(null, EventArgs.Empty);
        }

        internal static event EventHandler AfterUpdate;
        /// <summary>
        /// 課程名稱
        /// </summary>
        [EMBACore.UDT.Field(Field = "name", Indexed = false, Caption = "課程名稱")]
        public string Name { get; set; }

        /// <summary>
        /// 課程英文名稱
        /// </summary>
        [EMBACore.UDT.Field(Field = "eng_name", Indexed = false, Caption = "課程英文名稱")]
        public string EnglishName { get; set; }

        /// <summary>
        /// 課程識別碼，原有的。
        /// </summary>
        [EMBACore.UDT.Field(Field = "subject_code", Indexed = true, Caption = "課程識別碼")]
        public string SubjectCode { get; set; }
        
        /// <summary>
        /// 新課號 (6碼系所代碼+4碼課號)
        /// </summary>
        [EMBACore.UDT.Field(Field = "new_subject_code", Indexed = false, Caption = "課號")]
        public string NewSubjectCode { get; set; }

        /// <summary>
        /// 開課系所
        /// </summary>
        [EMBACore.UDT.Field(Field = "dept_name", Indexed = false, Caption = "開課系所")]
        public string DeptName { get; set; }

        /// <summary>
        /// 開課系所代碼
        /// </summary>
        [EMBACore.UDT.Field(Field = "dept_code", Indexed = true, Caption = "開課系所代碼")]
        public string DeptCode { get; set; }

        /// <summary>
        /// 學分數
        /// </summary>
        [EMBACore.UDT.Field(Field = "credit", Indexed = false, Caption = "學分數")]
        public int Credit { get; set; }

        /// <summary>
        /// 內容簡介
        /// </summary>
        [EMBACore.UDT.Field(Field = "description", Indexed = false, Caption = "內容簡介")]
        public string Description { get; set; }

        /// <summary>
        /// 網頁連結
        /// </summary>
        [EMBACore.UDT.Field(Field = "web_url", Indexed = false, Caption = "網頁連結")]
        public string WebUrl { get; set; }

        /// <summary>
        /// 必選修別，是否必修？
        /// </summary>
        [EMBACore.UDT.Field(Field = "is_required", Indexed = false, Caption = "必選修")]
        public bool IsRequired { get; set; }

        /// <summary>
        /// 備註
        /// </summary>
        [EMBACore.UDT.Field(Field = "remark", Indexed = false, Caption = "備註")]
        public string Remark { get; set; }

        /// <summary>
        /// 科目代碼(EMBA稱為 課程代碼)，是由 [dept_abbr + " " + cou_teacno] 組成
        /// </summary>
        public string SubjectCode1
        {
            get {
                string result = "";
                if (!string.IsNullOrWhiteSpace(this.SubjectCode))
                {
                    string[] codes = this.SubjectCode.Split(new char[] { ' ' });
                    if (codes.Length > 0)
                        result = codes[0];
                }
                return result ;
            }
        }

        public string SubjectCode2
        {
            get
            {
                string result = "";
                if (!string.IsNullOrWhiteSpace(this.SubjectCode))
                {
                    string[] codes = this.SubjectCode.Split(new char[] { ' ' });
                    if (codes.Length > 1)
                        result = codes[1];
                }
                return result;
            }
        }

    }
}