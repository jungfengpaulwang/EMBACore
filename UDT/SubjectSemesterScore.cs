using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FISCA.UDT;

namespace EMBACore.UDT
{
    [FISCA.UDT.TableName("ischool.emba.subject_semester_score")]
    public class SubjectSemesterScore : ActiveRecord
    {
        internal static void RaiseAfterUpdateEvent(object sender, ParameterEventArgs e)
        {
            if (SubjectSemesterScore.AfterUpdate != null)
                SubjectSemesterScore.AfterUpdate(sender, e);
        }

        internal static event EventHandler<ParameterEventArgs> AfterUpdate;

        /// <summary>
        /// 學年度
        /// </summary>
        [EMBACore.UDT.Field(Field = "school_year", Indexed = false, Caption = "學年度")]
        public int? SchoolYear { get; set; }

        /// <summary>
        /// 學期
        /// </summary>
        [EMBACore.UDT.Field(Field = "semester", Indexed = false, Caption = "學期")]
        public int? Semester { get; set; }

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
        /// 課程系統編號
        /// </summary>
        [EMBACore.UDT.Field(Field = "ref_subject_id", Indexed = true, Caption = "課程系統編號")]
        public int SubjectID { get; set; }

        /// <summary>
        /// 課程名稱
        /// </summary>
        [EMBACore.UDT.Field(Field = "subject_name", Indexed = false, Caption = "課程名稱")]
        public string SubjectName { get; set; }

        /// <summary>
        /// 課程識別碼
        /// </summary>
        [EMBACore.UDT.Field(Field = "subject_code", Indexed = false, Caption = "課程識別碼")]
        public string SubjectCode { get; set; }

        /// <summary>
        /// 新課號 (6碼系所代碼+4碼課號)
        /// </summary>
        [EMBACore.UDT.Field(Field = "new_subject_code", Indexed = false, Caption = "課號")]
        public string NewSubjectCode { get; set; }

        /// <summary>
        /// 成績
        /// </summary>
        [EMBACore.UDT.Field(Field = "score", Indexed = false, Caption = "成績")]
        public string Score { get; set; }

        /// <summary>
        /// 學分數
        /// </summary>
        [EMBACore.UDT.Field(Field = "credit", Indexed = false, Caption = "學分數")]
        public int Credit { get; set; }

        /// <summary>
        /// 必選修
        /// </summary>
        [EMBACore.UDT.Field(Field = "is_required", Indexed = false, Caption = "必選修")]
        public bool IsRequired { get; set; }

        /// <summary>
        /// 是否取得學分
        /// </summary>
        [EMBACore.UDT.Field(Field = "is_pass", Indexed = false, Caption = "是否取得學分")]
        public bool IsPass { get; set; }

        /// <summary>
        /// 抵免課程
        /// </summary>
        [EMBACore.UDT.Field(Field = "offset_course", Indexed = false, Caption = "抵免課程")]
        public string OffsetCourse { get; set; }

        /// <summary>
        /// 備註
        /// </summary>
        [EMBACore.UDT.Field(Field = "remark", Indexed = false, Caption = "備註")]
        public string Remark { get; set; }
    }
}
