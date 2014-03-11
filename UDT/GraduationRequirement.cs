using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FISCA.UDT;

namespace EMBACore.UDT
{

    /// <summary>
    /// 畢業條件
    /// </summary>
    [FISCA.UDT.TableName("ischool.emba.graduation_requirement")]
    public class GraduationRequirement : ActiveRecord
    {
        internal static void RaiseAfterUpdateEvent(object sender, ParameterEventArgs e)
        {
            if (GraduationRequirement.AfterUpdate != null)
                GraduationRequirement.AfterUpdate(sender, e);
        }

        internal static event EventHandler<ParameterEventArgs> AfterUpdate;

        /// <summary>
        /// 畢業條件名稱
        /// </summary>
        [EMBACore.UDT.Field(Field = "name", Indexed = false, Caption = "畢業條件名稱")]
        public string Name { get; set; }

        /// <summary>
        /// 系所組別系統編號(參照 DepartmentGroup.UID)
        /// </summary>
        [EMBACore.UDT.Field(Field = "ref_department_group_id", Indexed = true, Caption = "系所組別系統編號")]
        public int DepartmentGroupID { get; set; }

        /// <summary>
        /// 系訂必修學分數
        /// </summary>
        [EMBACore.UDT.Field(Field = "department_credit", Indexed = false, Caption = "系訂必修學分數")]
        public int DepartmentCredit { get; set; }

        /// <summary>
        /// 選修學分數
        /// </summary>
        [EMBACore.UDT.Field(Field = "elective_credit", Indexed = false, Caption = "選修學分數")]
        public int ElectiveCredit { get; set; }

        /// <summary>
        /// 應修學分數
        /// </summary>
        [EMBACore.UDT.Field(Field = "required_credit", Indexed = false, Caption = "應修學分數")]
        public int RequiredCredit { get; set; }
    }
}
