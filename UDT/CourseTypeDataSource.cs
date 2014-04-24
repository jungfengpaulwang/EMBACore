using System;
using FISCA.UDT;

namespace EMBACore.UDT
{
    [FISCA.UDT.TableName("ischool.emba.course_type_data_source")]
    public class CourseTypeDataSource : ActiveRecord
    {
        internal static void RaiseAfterUpdateEvent()
        {
            if (CourseTypeDataSource.AfterUpdate != null)
                CourseTypeDataSource.AfterUpdate(null, EventArgs.Empty);
        }

        internal static event EventHandler AfterUpdate;

        /// <summary>
        /// 課程類別
        /// </summary>
        [EMBACore.UDT.Field(Field = "course_type", Indexed = false, Caption = "課程類別")]
        public string CourseType { get; set; }

        /// <summary>
        /// 不顯示：true-->不顯示；false-->顯示(預設)
        /// </summary>
        [EMBACore.UDT.Field(Field = "not_display", Indexed = false, Caption = "不顯示")]
        public bool NotDisplay { get; set; }
    }
}