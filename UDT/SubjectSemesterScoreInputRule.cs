using System;
using FISCA.UDT;

namespace EMBACore.UDT
{
    /// <summary>
    /// 成績輸入規則
    /// </summary>
    [FISCA.UDT.TableName("ischool.emba.course.subject_semester_score_input_rule")]
    public class SubjectSemesterScoreInputRule : ActiveRecord
    {
        /// <summary>
        /// 開課系統編號
        /// </summary>
        [EMBACore.UDT.Field(Field = "ref_course_id", Indexed = true, Caption = "開課系統編號")]
        public int CourseID { get; set; }

        /// <summary>
        /// 成績輸入規則：0：等第制，1：P/N 制
        /// </summary>
        [EMBACore.UDT.Field(Field = "input_rule", Indexed = false, Caption = "成績輸入規則")]
        public int InputRule { get; set; }
    }
}
