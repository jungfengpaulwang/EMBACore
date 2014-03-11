using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FISCA.UDT;

namespace EMBACore.UDT
{
    /// <summary>
    /// 畢業應修科目清單
    /// </summary>
    [FISCA.UDT.TableName("ischool.emba.graduation_subject_list")]
    public class GraduationSubjectList : ActiveRecord
    {
        internal static void RaiseAfterUpdateEvent(object sender, ParameterEventArgs e)
        {
            if (GraduationSubjectList.AfterUpdate != null)
                GraduationSubjectList.AfterUpdate(sender, e);
        }

        internal static event EventHandler<ParameterEventArgs> AfterUpdate;

        /// <summary>
        /// 畢業條件系統編號，參照：GraduationRequirement.UID(ischool.emba.graduation_requirement.uid)
        /// </summary>
        [EMBACore.UDT.Field(Field = "ref_graduation_requirement_id", Indexed = true, Caption = "畢業條件系統編號")]
        public int GraduationRequirementID { get; set; }

        /// <summary>
        /// 畢業應修科目系統編號(參照 Subject.UID，ischool.emba.subject.uid)
        /// </summary>
        [EMBACore.UDT.Field(Field = "ref_subject_id", Indexed = true, Caption = "畢業應修科目系統編號")]
        public int SubjectID { get; set; }

        /// <summary>
        /// 畢業應修科目群組
        /// </summary>
        [EMBACore.UDT.Field(Field = "subject_group", Indexed = false, Caption = "畢業應修科目群組")]
        public string SubjectGroup { get; set; }

        /// <summary>
        /// 必修認可範圍
        /// </summary>
        [EMBACore.UDT.Field(Field = "prerequisites", Indexed = false, Caption = "必修認可範圍")]
        public string Prerequisites { get; set; }
    }
}
