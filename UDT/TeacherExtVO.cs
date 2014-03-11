using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FISCA.UDT;
using System.ComponentModel;

namespace EMBACore.UDT
{
    /// <summary>
    /// 紀錄在UDT中的教師額外資訊的Value Object
    /// </summary>
    [FISCA.UDT.TableName("ischool.emba.teacher_ext")]
    class TeacherExtVO : FISCA.UDT.ActiveRecord
    {
        internal static void RaiseAfterUpdateEvent(object sender, ParameterEventArgs e)
        {
            if (TeacherExtVO.AfterUpdate != null)
                TeacherExtVO.AfterUpdate(sender, e);
        }

        internal static event EventHandler<ParameterEventArgs> AfterUpdate;

        /// <summary>
        /// 教師系統編號
        /// </summary>
        [EMBACore.UDT.Field(Field="ref_teacher_id", Indexed=true, Caption = "教師系統編號")]
        public int TeacherID { get; set; }

        /// <summary>
        /// 生日
        /// </summary>
        [EMBACore.UDT.Field(Field = "birthday", Indexed = false, Caption = "生日")]
        public string Birthday { get; set; }

        /// <summary>
        /// 戶籍地址
        /// </summary>
        [EMBACore.UDT.Field(Field = "address", Indexed = false, Caption = "戶籍地址")]
        public string Address { get; set; }

        /// <summary>
        /// 手機
        /// </summary>
        [EMBACore.UDT.Field(Field = "mobil", Indexed = false, Caption = "手機")]
        public string Mobil { get; set; }

        /// <summary>
        /// 研究室電話
        /// </summary>
        [EMBACore.UDT.Field(Field = "other_phone", Indexed = false, Caption = "研究室電話")]
        public string OtherPhone { get; set; }

        /// <summary>
        /// 電話
        /// </summary>
        [EMBACore.UDT.Field(Field = "phone", Indexed = false, Caption = "電話")]
        public string Phone { get; set; }

        /// <summary>
        /// 所屬單位
        /// </summary>
        [EMBACore.UDT.Field(Field = "major_work_place", Indexed = false, Caption = "所屬單位")]
        public string MajorWorkPlace { get; set; }

        /// <summary>
        /// 研究室
        /// </summary>
        [EMBACore.UDT.Field(Field = "research", Indexed = false, Caption = "研究室")]
        public string Research { get; set; }

        /// <summary>
        /// 個人網址
        /// </summary>
        [EMBACore.UDT.Field(Field = "website_url", Indexed = false, Caption = "個人網址")]
        public string WebSiteUrl { get; set; }

        /// <summary>
        /// 備註
        /// </summary>
        [EMBACore.UDT.Field(Field = "memo", Indexed = false, Caption = "備註")]
        public string Memo { get; set; }

        /// <summary>
        /// 英文姓名
        /// </summary>
        [EMBACore.UDT.Field(Field = "english_name", Indexed = false, Caption = "英文姓名")]
        public string EnglishName { get; set; }

        /// <summary>
        /// 人事編號
        /// </summary>
        [EMBACore.UDT.Field(Field = "employee_no", Indexed = false, Caption = "人事編號")]
        public string EmployeeNo { get; set; }

        /// <summary>
        /// 台大校務系統教師編號
        /// </summary>
        [Description("台大校務系統的教師編號，便於日後對照用")]
        [EMBACore.UDT.Field(Field = "ntu_system_no", Indexed = false, Caption = "台大校務系統教師編號")]
        public string NtuSystemNo { get; set; }

        public TeacherExtVO Clone()
        {
            return (this.MemberwiseClone() as TeacherExtVO);
        }
    }
}
