using FISCA.UDT;
using System;
namespace EMBACore.UDT
{
    [FISCA.UDT.TableName("ischool.emba.paper")]
    public class Paper : ActiveRecord
    {
        internal static void RaiseAfterUpdateEvent(object sender, ParameterEventArgs e)
        {
            if (Paper.AfterUpdate != null)
                Paper.AfterUpdate(sender, e);
        }

        internal static event EventHandler<ParameterEventArgs> AfterUpdate;

        /// <summary>
        /// 學生系統編號
        /// </summary>
        [EMBACore.UDT.Field(Field = "ref_student_id", Indexed = true, Caption = "學生系統編號")]
        public int StudentID { get; set; }

        /// <summary>
        /// 論文題目
        /// </summary>
        [EMBACore.UDT.Field(Field = "paper_name", Indexed = false, Caption = "論文題目")]
        public string PaperName { get; set; }

        /// <summary>
        /// 論文題目英文名稱
        /// </summary>
        [EMBACore.UDT.Field(Field = "paper_english_name", Indexed = false, Caption = "論文題目英文名稱")]
        public string PaperEnglishName { get; set; }

        /// <summary>
        /// 是否公開紙本論文
        /// </summary>
        [EMBACore.UDT.Field(Field = "is_public", Indexed = false, Caption = "是否公開紙本論文")]
        public bool IsPublic { get; set; }

        /// <summary>
        /// 延後公開期限(日期)
        /// </summary>
        [EMBACore.UDT.Field(Field = "published_date", Indexed = false, Caption = "延後公開期限")]
        public string PublishedDate { get; set; }

        /// <summary>
        /// 學年度
        /// </summary>
        [EMBACore.UDT.Field(Field = "school_year", Indexed = false, Caption = "學年度")]
        public string SchoolYear { get; set; }

        /// <summary>
        /// 學期
        /// </summary>
        [EMBACore.UDT.Field(Field = "semester", Indexed = false, Caption = "學期")]
        public string Semester { get; set; }

        /// <summary>
        /// 書籍狀況
        /// </summary>
        [EMBACore.UDT.Field(Field = "description", Indexed = false, Caption = "書籍狀況")]
        public string Description { get; set; }

        /// <summary>
        /// 指導教授，儲存格式為 XML : 
        ///         <Advisor TeacherID="">
        ///             <Name>馬先生</Name>
        ///         </Advisor>
        ///         <Advisor TeacherID="">
        ///             <Name>林小姐</Name>        
        ///         </Advisor>
        ///         ……
        /// </summary>
        [EMBACore.UDT.Field(Field = "advisor_list", Indexed = false, Caption = "指導教授")]
        public string AdvisorList { get; set; }

        public Paper Clone()
        {
            return (this.MemberwiseClone() as Paper);
        }
    }
}