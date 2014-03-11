using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FISCA.UDT;
using System.ComponentModel;

namespace EMBACore.UDT
{
    [FISCA.UDT.TableName("ischool.emba.experience")]
    public class Experience: ActiveRecord
    {
        private DateTime? time_stamp;
        internal static void RaiseAfterUpdateEvent(object sender, ParameterEventArgs e)
        {
            if (Experience.AfterUpdate != null)
                Experience.AfterUpdate(sender, e);
        }

        internal static event EventHandler<ParameterEventArgs> AfterUpdate;

        /// <summary>
        /// 學生系統編號
        /// </summary>
        [EMBACore.UDT.Field(Field = "ref_student_id", Indexed = false, Caption = "學生系統編號")]        
        public int StudentID { get; set; }

        /// <summary>
        /// 公司名稱
        /// </summary>
        [EMBACore.UDT.Field(Field = "company_name", Indexed = false, Caption = "公司名稱")]
        public string Company { get; set; }

        /// <summary>
        /// 職稱
        /// </summary>
        [EMBACore.UDT.Field(Field = "position", Indexed = false, Caption = "職稱")]
        public string Position { get; set; }

        /// <summary>
        /// 產業別
        /// </summary>
        [EMBACore.UDT.Field(Field = "industry", Indexed = false, Caption = "產業別")]
        public string Industry { get; set; }

        /// <summary>
        /// 部門類別
        /// </summary>
        [EMBACore.UDT.Field(Field = "department_category", Indexed = false, Caption = "部門類別")]
        public string DepartmentCategory { get; set; }

        /// <summary>
        /// 層級別
        /// </summary>
        [EMBACore.UDT.Field(Field = "post_level", Indexed = false, Caption = "層級別")]
        public string PostLevel { get; set; }

        /// <summary>
        /// 工作地點
        /// </summary>
        [EMBACore.UDT.Field(Field = "work_place", Indexed = false, Caption = "工作地點")]
        public string WorkPlace { get; set; }

        /// <summary>
        /// 工作狀態：現職、退休、已離職。
        /// </summary>
        [EMBACore.UDT.Field(Field = "work_status", Indexed = false, Caption = "工作狀態")]
        public string WorkStatus { get; set; }

        /// <summary>
        /// 工作起日
        /// </summary>
        [EMBACore.UDT.Field(Field = "work_begin_date", Indexed = false, Caption = "工作起日")]
        public DateTime? WorkBeginDate { get; set; }

        /// <summary>
        /// 工作迄日
        /// </summary>
        [EMBACore.UDT.Field(Field = "work_end_date", Indexed = false, Caption = "工作迄日")]
        public DateTime? WorkEndDate { get; set; }

        /// <summary>
        /// 是否現職？(新版廢除不用，改為「工作狀態：work_status」)
        /// </summary>
        [EMBACore.UDT.Field(Field = "is_current", Indexed = false, Caption = "是否為現職")]
        public bool IsCurrent { get; set; }

        /// <summary>
        /// 公關連絡人
        /// </summary>
        [EMBACore.UDT.Field(Field = "publicist", Indexed = false, Caption = "公關連絡人")]
        public string Publicist { get; set; }

        /// <summary>
        /// 公關室電話
        /// </summary>
        [EMBACore.UDT.Field(Field = "public_relations_office_telephone", Indexed = false, Caption = "公關室電話")]
        public string PublicRelationsOfficeTelephone { get; set; }

        /// <summary>
        /// 公關室傳真
        /// </summary>
        [EMBACore.UDT.Field(Field = "public_relations_office_fax", Indexed = false, Caption = "公關室傳真")]
        public string PublicRelationsOfficeFax { get; set; }

        /// <summary>
        /// 公關 Email
        /// </summary>
        [EMBACore.UDT.Field(Field = "publicist_email", Indexed = false, Caption = "公關 Email")]
        public string PublicistEmail { get; set; }

        /// <summary>
        /// 公司網址
        /// </summary>
        [EMBACore.UDT.Field(Field = "company_website", Indexed = false, Caption = "公司網址")]
        public string CompanyWebsite { get; set; }

        /// <summary>
        /// 異動資料者
        /// </summary>
        [EMBACore.UDT.Field(Field = "action_by", Indexed = false, Caption = "異動資料者")]
        public string ActionBy { get; set; }

        /// <summary>
        /// 最後更新日期
        /// </summary>
        [EMBACore.UDT.Field(Field = "time_stamp", Indexed = false, Caption = "最後更新日期")]
        public DateTime? TimeStamp 
        {
            get { return time_stamp; }
            set { time_stamp = value; } 
        }
    }
}
