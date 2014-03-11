using System;
using FISCA.UDT;

namespace EMBACore.UDT
{
    /// <summary>
    /// 等第、積分、分數對照表
    /// </summary>
    [FISCA.UDT.TableName("ischool.emba.grade_score_mapping_table")]
    public class GradeScoreMappingTable : ActiveRecord
    {
        /// <summary>
        /// 等第
        /// </summary>
        [EMBACore.UDT.Field(Field = "grade", Indexed = false, Caption = "等第")]
        public string Grade { get; set; }

        /// <summary>
        /// 積分
        /// </summary>
        [EMBACore.UDT.Field(Field = "gp", Indexed = false, Caption = "積分")]
        public decimal GP { get; set; }

        /// <summary>
        /// 分數
        /// </summary>
        [EMBACore.UDT.Field(Field = "score", Indexed = false, Caption = "分數")]
        public decimal Score { get; set; }

        /// <summary>
        /// 淺層複製自己
        /// </summary>
        /// <returns></returns>
        public GradeScoreMappingTable Clone()
        {
            return (this.MemberwiseClone() as GradeScoreMappingTable);
        }
    }
}