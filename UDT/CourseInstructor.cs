using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FISCA.UDT;

namespace EMBACore.UDT
{
    /// <summary>
    /// 課程的助教
    /// </summary>
    [FISCA.UDT.TableName("ischool.emba.course_instructor")]
    public class CourseInstructor : ActiveRecord
    {
        internal static void RaiseAfterUpdateEvent(object sender, ParameterEventArgs e)
        {
            if (CourseInstructor.AfterUpdate != null)
                CourseInstructor.AfterUpdate(sender, e);
        }

        internal static event EventHandler<ParameterEventArgs> AfterUpdate;

        /// <summary>
        /// 開課系統編號
        /// </summary>
        [EMBACore.UDT.Field(Field = "ref_course_id", Indexed = false, Caption = "開課系統編號")]
        public int CourseID { get; set; }

        /// <summary>
        /// 教師系統編號
        /// </summary>
        [EMBACore.UDT.Field(Field = "ref_teacher_id", Indexed = false, Caption = "教師系統編號")]
        public int TeacherID { get; set; }

        /// <summary>
        /// 標籤系統編號
        /// </summary>
        [EMBACore.UDT.Field(Field = "ref_tag_id", Indexed = false, Caption = "標籤系統編號")]
        public int TagID { get; set; }

        /// <summary>
        /// 角色。
        /// 這個欄位目前已不再使用，由 tag_teacher 取代。  2012/3/16, kevin.
        /// </summary>
        [EMBACore.UDT.Field(Field = "role", Indexed = false, Caption = "角色")]
        public string Role { get; set; }

        /// <summary>
        /// 成績管理
        /// </summary>
        [EMBACore.UDT.Field(Field = "is_scored", Indexed = false, Caption = "成績管理")]
        public bool IsScored { get; set; }

    }
}
