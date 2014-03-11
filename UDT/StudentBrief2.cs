using System;
using System.Collections.Generic;
using FISCA.UDT;


namespace EMBACore.UDT
{
     [FISCA.UDT.TableName("ischool.emba.student_brief2")]
    public class StudentBrief2: ActiveRecord
    {
        internal static void RaiseAfterUpdateEvent(object sender, ParameterEventArgs e)
        {
            if (StudentBrief2.AfterUpdate != null)
                StudentBrief2.AfterUpdate(sender, e);
        }

        internal static event EventHandler<ParameterEventArgs> AfterUpdate;

         /// <summary>
        ///    學生系統編號 
         /// </summary>
        [EMBACore.UDT.Field(Field = "ref_student_id", Indexed = false, Caption = "學生系統編號")]
        public int StudentID { get; set; }

         /// <summary>
         /// 入學年度
         /// </summary>
        [EMBACore.UDT.Field(Field = "enroll_year", Indexed = false, Caption = "入學年度")]
        public string EnrollYear { get; set; }

        /// <summary>
        /// 畢業學年度
        /// </summary>
        [EMBACore.UDT.Field(Field = "graduate_year", Indexed = false, Caption = "畢業學年度")]
        public string GraduateYear { 
            get { 
                return (this.UpdateCode == "G") ? this.UpdateSchoolYearSemester.Substring(0, 3) : "" ; 
            }             
        }

        /// <summary>
        /// 畢業學期
        /// </summary>
        [EMBACore.UDT.Field(Field = "graduate_semester", Indexed = false, Caption = "畢業學期")]
        public string GraduateSemester
        {
            get
            {
                return (this.UpdateCode == "G") ? this.UpdateSchoolYearSemester.Substring(3, 1) : "";
            }
        }
         
         /// <summary>
        /// email 清單，儲存格是為 XML : <email>abc@...</email><email>def@...</email>
         /// </summary>
        [EMBACore.UDT.Field(Field = "email_list", Indexed = false, Caption = "電子郵件")]
        public string EmailList { get; set; }
         
         /// <summary>
        /// 學生畢業條件，參照：GraduationRequirement.UID(ischool.emba.graduation_requirement.uid)
         /// </summary>
        [EMBACore.UDT.Field(Field = "ref_graduation_requirement_id", Indexed = true, Caption = "畢業條件系統編號")]
        public int GraduationRequirementID { get; set; }

        /// <summary>
        /// 學生所屬系所組別參照，參照：DepartmentGroup.UID(ischool.emba.department_group.uid)
        /// </summary>
        [EMBACore.UDT.Field(Field = "ref_department_group_id", Indexed = true, Caption = "系所組別系統編號")]
        public int DepartmentGroupID { get; set; }

        /// <summary>
        /// 學生所屬系所組別代碼，這欄位會由校務系統匯入。
        /// </summary>
        [EMBACore.UDT.Field(Field = "department_group_code", Indexed = false, Caption = "系所組別代碼")]
        public string DepartmentGroupCode { get; set; }

         /// <summary>
         /// 學生年級，這欄位會由校務系統匯入。
         /// </summary>
        [EMBACore.UDT.Field(Field = "grade_year", Indexed = false, Caption = "學生年級")]
        public int GradeYear { get; set; }

        /// <summary>
        /// 目前是否在校，這欄位會由校務系統匯入。
        /// </summary>
        [EMBACore.UDT.Field(Field = "is_in_school", Indexed = false, Caption = "是否在校")]
        public bool IsInSchool { get; set; }

        /// <summary>
        /// 轉系學年度學期，這欄位會由校務系統匯入。
        /// </summary>
        [EMBACore.UDT.Field(Field = "transfer_dept_schoolyear_semester", Indexed = false, Caption = "轉系學年度學期")]
        public string TransferDeptSchoolYearSemester { get; set; }

        /// <summary>
        /// 轉系前原系所，這欄位會由校務系統匯入。
        /// </summary>
        [EMBACore.UDT.Field(Field = "transfer_previous_dept", Indexed = false, Caption = "轉系前原系所")]
        public string TransferPreviousDept { get; set; }

        /// <summary>
        /// 入學前畢業學校，這欄位會由校務系統匯入。
        /// </summary>
        [EMBACore.UDT.Field(Field = "previous_school", Indexed = false, Caption = "入學前畢業學校")]
        public string PreviousSchool { get; set; }
        
        /// <summary>
        /// 國籍，這欄位會由校務系統匯入。
        /// </summary>
        [EMBACore.UDT.Field(Field = "nationality", Indexed = false, Caption = "國籍")]
        public string Nationality { get; set; }

        /// <summary>
        /// 是否是延修生，這欄位會由校務系統匯入。
        /// </summary>
        [EMBACore.UDT.Field(Field = "is_delay", Indexed = false, Caption = "是否為延修生")]
        public bool IsDelay { get; set; }

        /// <summary>
        /// 最後異動代碼，這欄位會由校務系統匯入。
        /// O: 退學  B: 休學   R: 復學   G: 畢業
        /// </summary>
        [EMBACore.UDT.Field(Field = "update_code", Indexed = false, Caption = "最後異動代碼")]
        public string UpdateCode { get; set; }

        /// <summary>
        /// 最後異動學年度學期，這欄位會由校務系統匯入。
        /// </summary>
        [EMBACore.UDT.Field(Field = "update_schoolyear_semester", Indexed = false, Caption = "最後異動學年度學期")]
        public string UpdateSchoolYearSemester { get; set; }

        /// <summary>
        ///退學原因代碼，這欄位會由校務系統匯入。
        /// </summary>
        [EMBACore.UDT.Field(Field = "drop_out_code", Indexed = false, Caption = "退學原因代碼")]
        public string DropOutCode { get; set; }

        /// <summary>
        ///身分別代碼，這欄位會由校務系統匯入。
        /// </summary>
        [EMBACore.UDT.Field(Field = "id_code", Indexed = false, Caption = "身分別代碼")]
        public string IDCode { get; set; }

        /// <summary>
        ///休學紀錄一，這欄位會由校務系統匯入。
        /// </summary>
        [EMBACore.UDT.Field(Field = "suspension1", Indexed = false, Caption = "休學紀錄一")]
        public string Suspension1 { get; set; }

        /// <summary>
        ///休學紀錄二，這欄位會由校務系統匯入。
        /// </summary>
        [EMBACore.UDT.Field(Field = "suspension2", Indexed = false, Caption = "休學紀錄二")]
        public string Suspension2 { get; set; }

        /// <summary>
        ///休學紀錄三，這欄位會由校務系統匯入。
        /// </summary>
        [EMBACore.UDT.Field(Field = "suspension3", Indexed = false, Caption = "休學紀錄三")]
        public string Suspension3 { get; set; }

        /// <summary>
        ///休學紀錄四，這欄位會由校務系統匯入。
        /// </summary>
        [EMBACore.UDT.Field(Field = "suspension4", Indexed = false, Caption = "休學紀錄四")]
        public string Suspension4 { get; set; }

        /// <summary>
        ///休學紀錄五，這欄位會由校務系統匯入。
        /// </summary>
        [EMBACore.UDT.Field(Field = "suspension5", Indexed = false, Caption = "休學紀錄五")]
        public string Suspension5 { get; set; }

        /// <summary>
        ///休學紀錄六，這欄位會由校務系統匯入。
        /// </summary>
        [EMBACore.UDT.Field(Field = "suspension6", Indexed = false, Caption = "休學紀錄六")]
        public string Suspension6 { get; set; }

        /// <summary>
        ///休學紀錄七，這欄位會由校務系統匯入。
        /// </summary>
        [EMBACore.UDT.Field(Field = "suspension7", Indexed = false, Caption = "休學紀錄七")]
        public string Suspension7 { get; set; }

        /// <summary>
        ///休學紀錄八，這欄位會由校務系統匯入。
        /// </summary>
        [EMBACore.UDT.Field(Field = "suspension8", Indexed = false, Caption = "休學紀錄八")]
        public string Suspension8 { get; set; }

        /// <summary>
        ///紀錄時間。
        /// </summary>
        [EMBACore.UDT.Field(Field = "create_time", Indexed = false, Caption = "紀錄時間")]
        public DateTime? CreateTime { get; set; }

    }
}
