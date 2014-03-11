using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FISCA.UDT;

namespace EMBACore.UDT
{
    /// <summary>
    /// 課程加退選
    /// </summary>
    [FISCA.UDT.TableName("ischool.emba.course_add_drop")]
    public class AddDropCourse: ActiveRecord
    {
        internal static void RaiseAfterUpdateEvent(object sender, ParameterEventArgs e)
        {
            if (AddDropCourse.AfterUpdate != null)
                AddDropCourse.AfterUpdate(sender, e);
        }

        internal static event EventHandler<ParameterEventArgs> AfterUpdate;

        /// <summary>
        /// 開課系統編號
        /// </summary>
        [EMBACore.UDT.Field(Field = "ref_course_id", Indexed = false, Caption = "開課系統編號")]
        public int CourseID { get; set; }

        /// <summary>
        /// 學生系統編號
        /// </summary>
        [EMBACore.UDT.Field(Field = "ref_student_id", Indexed = false, Caption = "學生系統編號")]
        public int StudentID { get; set; }

        /// <summary>
        /// 類型， A : 加選， D: 退選
        /// </summary>
        [EMBACore.UDT.Field(Field = "add_or_drop", Indexed = false, Caption = "類型")]
        public string AddOrDrop { get; set; }

        /// <summary>
        /// 確認日期 (若 null 表示尚未確認)，格式為 yyyy/mm/dd
        /// </summary>
        [EMBACore.UDT.Field(Field = "confirm_date", Indexed = false, Caption = "確認日期")]
        public string ConfirmDate { get; set; }
    }
}
