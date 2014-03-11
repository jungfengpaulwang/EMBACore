using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FISCA.UDT;

namespace EMBACore.UDT
{
     [FISCA.UDT.TableName("ischool.emba.student_brief2")]
    class StudentRemark : ActiveRecord
    {
        internal static void RaiseAfterUpdateEvent(object sender, ParameterEventArgs e)
        {
            if (StudentRemark.AfterUpdate != null)
                StudentRemark.AfterUpdate(sender, e);
        }

        internal static event EventHandler<ParameterEventArgs> AfterUpdate;

         /// <summary>
        /// 學生系統編號
         /// </summary>
        [EMBACore.UDT.Field(Field = "ref_student_id", Indexed = false, Caption = "學生系統編號")]
         public int StudentID { get; set; }

         /// <summary>
        /// 備註
         /// </summary>
        [EMBACore.UDT.Field(Field = "remark", Indexed = false, Caption = "備註")]
        public string Remark { get; set; }
    }
}
