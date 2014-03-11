using System;
using System.Collections.Generic;
using FISCA.UDT;

namespace EMBACore.UDT
{
    /// <summary>
    /// 系所組別
    /// </summary>
    [FISCA.UDT.TableName("ischool.emba.department_group")]
    public class DepartmentGroup : ActiveRecord
    {
        internal static void RaiseAfterUpdateEvent(object sender, ParameterEventArgs e)
        {
            if (DepartmentGroup.AfterUpdate != null)
                DepartmentGroup.AfterUpdate(sender, e);
        }

        internal static event EventHandler<ParameterEventArgs> AfterUpdate;

        /// <summary>
        /// 系所組別名稱
        /// </summary>
        [EMBACore.UDT.Field(Field = "name", Indexed = false, Caption = "系所組別名稱")]
        public string Name { get; set; }

        /// <summary>
        /// 系所組別英文名稱
        /// </summary>
        [EMBACore.UDT.Field(Field = "english_name", Indexed = false, Caption = "系所組別英文名稱")]
        public string EnglishName { get; set; }

        /// <summary>
        /// 系所組別代碼
        /// </summary>
        [EMBACore.UDT.Field(Field = "code", Indexed = false, Caption = "系所組別代碼")]
        public string Code { get; set; }

        /// <summary>
        /// 系所組別排列順序
        /// </summary>
        [EMBACore.UDT.Field(Field = "order", Indexed = false, Caption = "系所組別排列順序")]
        public string Order { get; set; }
    }
}