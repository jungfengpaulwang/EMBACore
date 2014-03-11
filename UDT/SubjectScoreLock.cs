using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FISCA.UDT;

namespace EMBACore.UDT
{
    [FISCA.UDT.TableName("ischool.emba.subject_score_lock")]
    public class SubjectScoreLock : ActiveRecord
    {
        internal static void RaiseAfterUpdateEvent(object sender, ParameterEventArgs e)
        {
            if (SubjectScoreLock.AfterUpdate != null)
                SubjectScoreLock.AfterUpdate(sender, e);
        }

        internal static event EventHandler<ParameterEventArgs> AfterUpdate;

        /// <summary>
        /// 學年度
        /// </summary>
        [EMBACore.UDT.Field(Field = "school_year", Indexed = false, Caption = "學年度")]
        public int SchoolYear { get; set; }

        /// <summary>
        /// 學期
        /// </summary>
        [EMBACore.UDT.Field(Field = "semester", Indexed = false, Caption = "學期")]
        public int Semester { get; set; }

        /// <summary>
        /// 是否鎖定
        /// </summary>
        [EMBACore.UDT.Field(Field = "is_locked", Indexed = false, Caption = "是否鎖定")]
        public bool IsLocked { get; set; }
    }
}
