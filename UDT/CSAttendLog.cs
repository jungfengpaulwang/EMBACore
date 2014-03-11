using System;
using FISCA.UDT;

namespace EMBACore.UDT
{
    /// <summary>
    /// 選課歷史記錄
    /// </summary>
    [FISCA.UDT.TableName("ischool.emba.cs_attend_log")]
    public class CSAttendLog : ActiveRecord
    {
        internal static void RaiseAfterUpdateEvent(object sender, ParameterEventArgs e)
        {
            if (CSAttendLog.AfterUpdate != null)
                CSAttendLog.AfterUpdate(sender, e);
        }

        internal static event EventHandler<ParameterEventArgs> AfterUpdate;

        /// <summary>
        /// 課程系統編號
        /// </summary>
        [EMBACore.UDT.Field(Field = "ref_course_id", Indexed = true, Caption = "課程系統編號")]
        public int CourseID { get; set; }

        /// <summary>
        /// 學生系統編號
        /// </summary>
        [EMBACore.UDT.Field(Field = "ref_student_id", Indexed = true, Caption = "學生系統編號")]
        public int StudentID { get; set; }

        /// <summary>
        /// 次別
        /// </summary>
        [EMBACore.UDT.Field(Field = "item", Indexed = true, Caption = "次別")]
        public int Item { get; set; }

        /// <summary>
        /// 行為-->insert；delete
        /// </summary>
        [EMBACore.UDT.Field(Field = "action", Indexed = false, Caption = "次別")]
        public string Action { get; set; }

        /// <summary>
        /// 行為人-->web：student；desktop：staff
        /// </summary>
        [EMBACore.UDT.Field(Field = "action_by", Indexed = false, Caption = "次別")]
        public string ActionBy { get; set; }

        /// <summary>
        /// 淺層複製自己
        /// </summary>
        /// <returns></returns>
        public CSAttendLog Clone()
        {
            return (this.MemberwiseClone() as CSAttendLog);
        }
    }
}