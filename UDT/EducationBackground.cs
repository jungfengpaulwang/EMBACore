using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FISCA.UDT;

namespace EMBACore.UDT
{
    [FISCA.UDT.TableName("ischool.emba.education_background")]
    public class EducationBackground : ActiveRecord
    {
        internal static void RaiseAfterUpdateEvent(object sender, ParameterEventArgs e)
        {
            if (EducationBackground.AfterUpdate != null)
                EducationBackground.AfterUpdate(sender, e);
        }

        internal static event EventHandler<ParameterEventArgs> AfterUpdate;

        /// <summary>
        /// 學生系統編號
        /// </summary>
        [EMBACore.UDT.Field(Field = "ref_student_id", Indexed = false, Caption = "學生系統編號")]
        public int StudentID { get; set; }

        /// <summary>
        /// 學校名稱
        /// </summary>
        [EMBACore.UDT.Field(Field = "school_name", Indexed = false, Caption = "學校名稱")]
        public string SchoolName { get; set; }

        /// <summary>
        /// 系所
        /// </summary>
        [EMBACore.UDT.Field(Field = "department", Indexed = false, Caption = "系所")]
        public string Department { get; set; }

        /// <summary>
        /// 學位
        /// </summary>
        [EMBACore.UDT.Field(Field = "degree", Indexed = false, Caption = "學位")]
        public string Degree { get; set; }

        /// <summary>
        /// 是否最高學歷？
        /// </summary>
        [EMBACore.UDT.Field(Field = "is_top", Indexed = false, Caption = "是否為最高學歷")]
        public bool IsTop { get; set; }

        /// <summary>
        /// 異動資料者
        /// </summary>
        [EMBACore.UDT.Field(Field = "action_by", Indexed = false, Caption = "異動資料者")]
        public string ActionBy { get; set; }

        /// <summary>
        /// 最後更新日期
        /// </summary>
        [EMBACore.UDT.Field(Field = "time_stamp", Indexed = false, Caption = "最後更新日期")]
        public DateTime TimeStamp { get; set; }
    }
}
