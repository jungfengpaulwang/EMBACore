using FISCA.UDT;
using System;

namespace EMBACore.UDT
{
    [FISCA.UDT.TableName("ischool.emba.payment_history")]
    public class PaymentHistory : ActiveRecord
    {
        internal static void RaiseAfterUpdateEvent(object sender, ParameterEventArgs e)
        {
            if (PaymentHistory.AfterUpdate != null)
                PaymentHistory.AfterUpdate(sender, e);
        }

        internal static event EventHandler<ParameterEventArgs> AfterUpdate;

        /// <summary>
        /// 學生系統編號
        /// </summary>
        [EMBACore.UDT.Field(Field = "ref_student_id", Indexed = true, Caption = "學生系統編號")]
        public int StudentID { get; set; }

        /// <summary>
        /// 學年度
        /// </summary>
        [EMBACore.UDT.Field(Field = "school_year", Indexed = false, Caption = "學年度")]
        public int SchoolYear { get; set; }
        
        /// <summary>
        /// 學期：0-->夏季學期；1-->第1學期；2-->第2學期
        /// </summary>
        [EMBACore.UDT.Field(Field = "semester", Indexed = false, Caption = "學期")]
        public int Semester { get; set; }

        /// <summary>
        /// 繳費狀態：0-->未繳；1-->已繳
        /// </summary>
        [EMBACore.UDT.Field(Field = "is_paied", Indexed = false, Caption = "繳費狀態")]
        public int IsPaied { get; set; }

        /// <summary>
        /// 更新日期：繳費記錄異動時，由系統自動寫入
        /// </summary>
        [EMBACore.UDT.Field(Field = "last_modified_date", Indexed = false, Caption = "更新日期")]
        public DateTime? LastModifiedDate { get; set; }

        public PaymentHistory Clone()
        {
            return (this.MemberwiseClone() as PaymentHistory);
        }
    }
}