using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FISCA.UDT;

namespace EMBACore.UDT
{
    /// <summary>
    /// 同步作業不蓋檔欄位
    /// </summary>
    [FISCA.UDT.TableName("ischool.emba.sync_student_lock")]
    public class SyncStudentLock : ActiveRecord
    {
        /// <summary>
        /// 學生系統編號
        /// </summary>
        [EMBACore.UDT.Field(Field = "ref_student_id", Indexed = true, Caption = "學生系統編號")]
        public int StudentID { get; set; }

        /// <summary>
        /// 鎖定欄位名稱：學生姓名記錄為「student_name」
        /// </summary>
        [EMBACore.UDT.Field(Field = "field_name", Indexed = false, Caption = "鎖定欄位名稱")]
        public string FieldName { get; set; }
    }
}
