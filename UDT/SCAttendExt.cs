using FISCA.UDT;
using System;

namespace EMBACore.UDT
{
    [FISCA.UDT.TableName("ischool.emba.scattend_ext")]
    public class SCAttendExt : ActiveRecord
    {
        internal static void RaiseAfterUpdateEvent(object sender, ParameterEventArgs e)
        {
            if (SCAttendExt.AfterUpdate != null)
                SCAttendExt.AfterUpdate(sender, e);
        }

        internal static event EventHandler<ParameterEventArgs> AfterUpdate;

        /// <summary>
        /// 學生系統編號
        /// </summary>
        [EMBACore.UDT.Field(Field = "ref_student_id", Indexed = true, Caption = "學生系統編號")]
        public int StudentID { get; set; }

        /// <summary>
        /// 開課系統編號
        /// </summary>
        [EMBACore.UDT.Field(Field = "ref_course_id", Indexed = true, Caption = "開課系統編號")]
        public int CourseID { get; set; }

        /// <summary>
        /// 報告小組
        /// </summary>
        [EMBACore.UDT.Field(Field = "report_group", Indexed = false, Caption = "報告小組")]
        public string Group { get; set; }

        /// <summary>
        /// true：停修；預設：false;
        /// </summary>
        [EMBACore.UDT.Field(Field = "is_cancel", Indexed = false, Caption = "是否停修")]
        public bool IsCancel { get; set; }

        /// <summary>
        /// 座位的 X 座標
        /// </summary>
        [EMBACore.UDT.Field(Field = "seat_x", Indexed = false, Caption = "座位的 X 座標")]
        public int? SeatX { get; set; }

        /// <summary>
        /// 座位的  y 座標
        /// </summary>
        [EMBACore.UDT.Field(Field = "seat_y", Indexed = false, Caption = "座位的  y 座標")]
        public int? SeatY { get; set; }

        public SCAttendExt Clone()
        {
            return (this.MemberwiseClone() as SCAttendExt);
        }
    }
}