using System;
using FISCA.UDT;

namespace EMBACore.UDT
{
    /// <summary>
    /// 缺曠紀錄
    /// </summary>
    [FISCA.UDT.TableName("ischool.emba.absence")]
    public class Absence : ActiveRecord
    {
        internal static void RaiseAfterUpdateEvent(object sender, ParameterEventArgs e)
        {
            if (Absence.AfterUpdate != null)
                Absence.AfterUpdate(sender, e);
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
        /// 上課時間表系統編號(Calendar Section ID)
        /// </summary>
        [EMBACore.UDT.Field(Field = "ref_section_id", Indexed = false, Caption = "上課時間表系統編號")]
        public int SectionID { get; set; }

        /// <summary>
        /// 是否補課
        /// </summary>
        [EMBACore.UDT.Field(Field = "is_make_up", Indexed = false, Caption = "是否補課")]
        public bool IsMakeUp { get; set; }

        /// <summary>
        /// 補課描述
        /// </summary>
        [EMBACore.UDT.Field(Field = "make_up_description", Indexed = false, Caption = "補課描述")]
        public string MakeUpDescription { get; set; }

        /// <summary>
        /// 發出電子郵件時間，若有值，代表已經發出email 通知了！
        /// </summary>
        [EMBACore.UDT.Field(Field = "email_time", Indexed = false, Caption = "發出電子郵件時間")]
        public string EmailTime { get; set; }

        /// <summary>
        /// 通知當時所寄送的電子郵件 (紀錄用的)
        /// </summary>
        [EMBACore.UDT.Field(Field = "target_email", Indexed = false, Caption = "通知對象電子郵件")]
        public string TargetEmail { get; set; }

    }
}
