using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FISCA.UDT;

namespace EMBACore.UDT
{
    /// <summary>
    /// 夏季學期未修課紀錄
    /// </summary>
    [FISCA.UDT.TableName("ischool.emba.do_not_take_summer_course")]
    class DoNotTakeCourseInSummer : ActiveRecord
    {
        internal static void RaiseAfterUpdateEvent(object sender, ParameterEventArgs e)
        {
            if (DoNotTakeCourseInSummer.AfterUpdate != null)
                DoNotTakeCourseInSummer.AfterUpdate(sender, e);
        }

        internal static event EventHandler<ParameterEventArgs> AfterUpdate;

        /// <summary>
        /// 學生系統編號
        /// </summary>
        [EMBACore.UDT.Field(Field = "ref_student_id", Indexed = false, Caption = "學生系統編號")]
        public int StudentID { get; set; }

        /// <summary>
        /// 地底筆紀錄
        /// </summary>
        [EMBACore.UDT.Field(Field = "rec_no", Indexed = false, Caption = "記錄項次")]
        public int RecNo  { get; set; }

        /// <summary>
        /// 學年度
        /// </summary>
        [EMBACore.UDT.Field(Field = "school_year", Indexed = false, Caption = "學年度")]
        public string SchoolYear { get; set; }

        /// <summary>
        /// 備註
        /// </summary>
        [EMBACore.UDT.Field(Field = "remark", Indexed = false, Caption = "備註")]
        public string Remark { get; set; }
        
    }
}
