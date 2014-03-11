using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FISCA.UDT;

namespace EMBACore.UDT
{
    /// <summary>
    /// 畢業應修科目群組應修科目數及應修學分數
    /// </summary>
    [FISCA.UDT.TableName("ischool.emba.graduation_subject_group_requirement")]
    public class GraduationSubjectGroupRequirement : ActiveRecord
    {
        internal static void RaiseAfterUpdateEvent(object sender, ParameterEventArgs e)
        {
            if (GraduationSubjectGroupRequirement.AfterUpdate != null)
                GraduationSubjectGroupRequirement.AfterUpdate(sender, e);
        }

        internal static event EventHandler<ParameterEventArgs> AfterUpdate;

        /// <summary>
        /// 畢業條件系統編號，參照：GraduationRequirement.UID(ischool.emba.graduation_requirement.uid)
        /// </summary>
        [EMBACore.UDT.Field(Field = "ref_graduation_requirement_id", Indexed = true, Caption = "畢業條件系統編號")]
        public int GraduationRequirementID { get; set; }

        /// <summary>
        /// 畢業應修科目群組
        /// </summary>
        [EMBACore.UDT.Field(Field = "subject_group", Indexed = false, Caption = "畢業應修科目群組")]
        public string SubjectGroup { get; set; }

        /// <summary>
        /// 畢業應修科目群組內最低應修科目數
        /// </summary>
        [EMBACore.UDT.Field(Field = "lowest_subject_count", Indexed = false, Caption = "最低應修科目數")]
        public int LowestSubjectCount { get; set; }

        /// <summary>
        /// 畢業應修科目群組內最低應修學分數
        /// </summary>
        [EMBACore.UDT.Field(Field = "lowest_credit", Indexed = false, Caption = "最低應修學分數")]
        public int LowestCredit { get; set; }
    }
}
