using System;
using FISCA.UDT;

namespace EMBACore.UDT
{
    /// <summary>
    /// 成績輸入學年期
    /// </summary>
    [FISCA.UDT.TableName("ischool.emba.score_input_semester")]
    public class ScoreInputSemester : ActiveRecord
    {
        /// <summary>
        /// 學年度
        /// </summary>
        [Field(Field = "school_year", Indexed = false, Caption = "學年度")]
        public int SchoolYear { get; set; }

        /// <summary>
        /// 學期
        /// </summary>
        [Field(Field = "semester", Indexed = false, Caption = "學期")]
        public int Semester { get; set; }

        /// <summary>
        /// 淺層複製自己
        /// </summary>
        /// <returns></returns>
        public ScoreInputSemester Clone()
        {
            return (this.MemberwiseClone() as ScoreInputSemester);
        }
    }
}