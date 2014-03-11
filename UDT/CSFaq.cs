using System;
using FISCA.UDT;

namespace EMBACore.UDT
{
    /// <summary>
    /// 選課說明
    /// </summary>
    [FISCA.UDT.TableName("ischool.emba.cs_faq")]
    public class CSFaq : ActiveRecord
    {
        internal static void RaiseAfterUpdateEvent(object sender, ParameterEventArgs e)
        {
            if (CSFaq.AfterUpdate != null)
                CSFaq.AfterUpdate(sender, e);
        }

        internal static event EventHandler<ParameterEventArgs> AfterUpdate;

        /// <summary>
        /// 題號
        /// </summary>
        [EMBACore.UDT.Field(Field = "display_order", Indexed = false, Caption = "題號")]
        public int Item { get; set; }

        /// <summary>
        /// 標題
        /// </summary>
        [EMBACore.UDT.Field(Field = "title", Indexed = false, Caption = "標題")]
        public string Title { get; set; }

        /// <summary>
        /// 內容
        /// </summary>
        [EMBACore.UDT.Field(Field = "content", Indexed = false, Caption = "內容")]
        public string Content { get; set; }

        /// <summary>
        /// 淺層複製自己
        /// </summary>
        /// <returns></returns>
        public CSFaq Clone()
        {
            return (this.MemberwiseClone() as CSFaq);
        }
    }
}