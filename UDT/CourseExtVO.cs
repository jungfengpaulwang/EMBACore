using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace EMBACore.UDT
{
    /// <summary>
    /// 紀錄在UDT中的課程額外資訊的Value Object
    /// </summary>
    [FISCA.UDT.TableName("ischool.emba.course_ext")]
    public class CourseExt : FISCA.UDT.ActiveRecord
    {
        internal static void RaiseAfterUpdateEvent(object sender, ParameterEventArgs e)
        {
            if (CourseExt.AfterUpdate != null)
                CourseExt.AfterUpdate(sender, e);
        }

        internal static event EventHandler<ParameterEventArgs> AfterUpdate;

        /// <summary>
        /// 開課系統編號
        /// </summary>
        [EMBACore.UDT.Field(Field = "ref_course_id", Indexed = true, Caption = "開課系統編號")]
        public int CourseID { get; set; }

        /// <summary>
        /// 課程系統編號
        /// </summary>
        [EMBACore.UDT.Field(Field = "ref_subject_id", Indexed = true, Caption = "課程系統編號")]
        public int SubjectID { get; set; }

        /// <summary>
        /// 課程識別碼
        /// </summary>
        [EMBACore.UDT.Field(Field = "subject_code", Indexed = false, Caption = "課程識別碼")]
        public string SubjectCode { get; set; }

        /// <summary>
        /// 課程類別
        /// </summary>
        [EMBACore.UDT.Field(Field = "course_type", Indexed = false, Caption = "課程類別")]
        public string CourseType { get; set; }

        /// <summary>
        /// 必選修
        /// </summary>
        [EMBACore.UDT.Field(Field = "is_required", Indexed = false, Caption = "必選修")]
        public bool IsRequired { get; set; }

        /// <summary>
        /// 修課人數上限
        /// </summary>
        [EMBACore.UDT.Field(Field = "capacity", Indexed = false, Caption = "修課人數上限")]
        public int Capacity { get; set; }

        /// <summary>
        /// 流水號
        /// </summary>
        [EMBACore.UDT.Field(Field = "serial_no", Indexed = false, Caption = "流水號")]
        public int SerialNo { get; set; }

        /// <summary>
        /// 開課班次，值為 空白、01, 02, 03
        /// </summary>
        [EMBACore.UDT.Field(Field = "class_name", Indexed = false, Caption = "開課班次")]
        public string ClassName { get; set; }

        /// <summary>
        /// 提交的學期成績是否確認不再變更？
        /// </summary>
        [EMBACore.UDT.Field(Field = "score_confirmed", Indexed = false, Caption = "是否學期成績不再變更")]
        public bool ScoreConfirmed { get; set; }

        /// <summary>
        /// 課號 (6碼系所代碼+4碼課號)
        /// </summary>
        [EMBACore.UDT.Field(Field = "new_subject_code", Indexed = false, Caption = "課號")]
        public string NewSubjectCode { get; set; }

        /// <summary>
        /// 教室編號 (排座位表用的)
        /// </summary>
        [EMBACore.UDT.Field(Field = "ref_classroom_layout_id", Indexed = false, Caption = "教室編號")]
        public int? ClassroomLayoutID { get; set; }

        /// <summary>
        /// 上課時間
        /// </summary>
        [EMBACore.UDT.Field(Field = "course_time_info", Indexed = false, Caption = "上課時間")]
        public string CourseTimeInfo { get; set; }

        /// <summary>
        /// 教室
        /// </summary>
        [EMBACore.UDT.Field(Field = "classroom", Indexed = false, Caption = "教室")]
        public string Classroom { get; set; }

        /// <summary>
        /// 課程大綱
        /// </summary>
        [EMBACore.UDT.Field(Field = "syllabus", Indexed = false, Caption = "課程大綱")]
        public string Syllabus { get; set; }

        /// <summary>
        /// 備註
        /// </summary>
        [EMBACore.UDT.Field(Field = "memo", Indexed = false, Caption = "備註")]
        public string Memo { get; set; }
    }
}
