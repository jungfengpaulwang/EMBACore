using System;
using FISCA.UDT;

namespace EMBACore.UDT
{
    /// <summary>
    /// Mail Log
    /// </summary>
    [FISCA.UDT.TableName("ischool.emba.mail_log")]
    public class MailLog : ActiveRecord
    {    
        /// <summary>
        /// 學年度
        /// </summary>
        [Field(Field = "school_year", Indexed = true, Caption = "學年度")]
        public int SchoolYear { get; set; }

        /// <summary>
        /// 學期
        /// </summary>
        [Field(Field = "semester", Indexed = true, Caption = "學期")]
        public int Semester { get; set; }

        /// <summary>
        /// 寄信帳號
        /// </summary>
        [Field(Field = "user_account", Indexed = false, Caption = "寄信帳號")]
        public string UserAccount { get; set; }

        /// <summary>
        /// 寄件人電子郵件
        /// </summary>
        [Field(Field = "sender_email", Indexed = false, Caption = "寄件人電子郵件")]
        public string SenderEmail { get; set; }

        /// <summary>
        /// 寄件人名稱
        /// </summary>
        [Field(Field = "sender_name", Indexed = false, Caption = "寄件人名稱")]
        public string SenderName { get; set; }

        /// <summary>
        /// 信件主旨
        /// </summary>
        [Field(Field = "mail_subject", Indexed = false, Caption = "信件主旨")]
        public string MailSubject { get; set; }

        /// <summary>
        /// 信件內容
        /// </summary>
        [Field(Field = "mail_content", Indexed = false, Caption = "信件內容")]
        public string MailContent { get; set; }

        /// <summary>
        /// 收件人電子郵件
        /// </summary>
        [Field(Field = "recipient_email_address", Indexed = false, Caption = "收件人電子郵件")]
        public string RecipientEmailAddress { get; set; }

        /// <summary>
        /// 是否為副本
        /// </summary>
        [Field(Field = "is_cc", Indexed = false, Caption = "是否為副本")]
        public bool IsCC { get; set; }

        /// <summary>
        /// 信件類別：發送 Email 提醒通知、發送 Email 再次提醒通知、缺課4次通知、缺課5次通知
        /// </summary>
        [Field(Field = "email_category", Indexed = false, Caption = "信件類別")]
        public string EmailCategory { get; set; }

        /// <summary>
        /// 寄信時間
        /// </summary>
        [Field(Field = "time_stamp", Indexed = false, Caption = "寄信時間")]
        public DateTime TimeStamp { get; set; }

        /// <summary>
        /// 更新寄信狀態(status)時間
        /// </summary>
        [Field(Field = "update_status_time", Indexed = false, Caption = "更新寄信狀態時間")]
        public DateTime? UpdateStatusTime { get; set; }

        /// <summary>
        /// 作業代碼
        /// </summary>
        [Field(Field = "guid", Indexed = false, Caption = "作業代碼")]
        public string GUID { get; set; }

        /// <summary>
        /// 寄信結果代碼
        /// </summary>
        [Field(Field = "status", Indexed = false, Caption = "寄信結果代碼")]
        public string Status { get; set; }

        /// <remarks>
        ///<Result>
        ///     <NoneException>
        ///         <Status></Status>
        ///         <ResultID></ResultID>
        ///         <Email></Email>
        ///     </NoneException>
        ///     <Exception>
        ///         <Status></Status>
        ///         <Code></Code>
        ///         <Name></Name>
        ///         <Message></Message>
        ///         <Source></Source>
        ///         <RejectReason></RejectReason>
        ///         <Email></Email>
        ///     </Exception>
        ///</Result>
        /// </remarks>
        /// <summary>
        /// 寄信結果
        /// </summary>
        [Field(Field = "result", Indexed = false, Caption = "寄信結果")]
        public string Result { get; set; }

        ///<remarks>
        /// <Extension>
        ///     <Student ID=""></Student>
        ///     <Course ID=""></Course>
        ///     <Section ID=""></Section>
        /// </Extension>
        /// </remarks>
        /// <summary>
        /// 延伸資料
        /// </summary>
        [Field(Field = "extension", Indexed = false, Caption = "延伸資料")]
        public string Extension { get; set; }

        /// <summary>
        /// 淺層複製物件
        /// </summary>
        /// <returns></returns>
        //public MailLog Clone()
        //{
        //    return this.MemberwiseClone() as MailLog;
        //}
    }
}
