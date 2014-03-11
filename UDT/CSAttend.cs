using System;
using FISCA.UDT;

namespace EMBACore.UDT
{
    /// <summary>
    /// 選課記錄
    /// </summary>
    [FISCA.UDT.TableName("ischool.emba.cs_attend")]
    public class CSAttend : ActiveRecord
    {
        internal static void RaiseAfterUpdateEvent(object sender, ParameterEventArgs e)
        {
            if (CSAttend.AfterUpdate != null)
                CSAttend.AfterUpdate(sender, e);
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
        /// 淺層複製自己
        /// </summary>
        /// <returns></returns>
        public CSAttend Clone()
        {
            return (this.MemberwiseClone() as CSAttend);
        }
    }
}