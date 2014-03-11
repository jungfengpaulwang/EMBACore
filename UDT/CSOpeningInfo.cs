using System;
using FISCA.UDT;

namespace EMBACore.UDT
{
    /// <summary>
    /// 選課開放資訊
    /// </summary>
    [FISCA.UDT.TableName("ischool.emba.cs_opening_info")]
    public class CSOpeningInfo : ActiveRecord
    {
        /// <summary>
        /// 階段別-->0：加退選、1：第一階段、2：第二階段
        /// </summary>
        [Field(Field = "item", Indexed = true, Caption = "階段別")]
        public int Item { get; set; }

        /// <summary>
        /// 開始時間
        /// </summary>
        [Field(Field = "begin_time", Indexed = false, Caption = "開始時間")]
        public DateTime BeginTime { get; set; }

        /// <summary>
        /// 結束時間
        /// </summary>
        [Field(Field = "end_time", Indexed = false, Caption = "結束時間")]
        public DateTime EndTime { get; set; }

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
        public CSOpeningInfo Clone()
        {
            return (this.MemberwiseClone() as CSOpeningInfo);
        }
    }
}