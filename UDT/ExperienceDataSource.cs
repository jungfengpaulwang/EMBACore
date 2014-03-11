using System;
using FISCA.UDT;

namespace EMBACore.UDT
{
    [FISCA.UDT.TableName("ischool.emba.experience_data_source")]
    public class ExperienceDataSource : ActiveRecord
    {
        internal static void RaiseAfterUpdateEvent()
        {
            if (ExperienceDataSource.AfterUpdate != null)
                ExperienceDataSource.AfterUpdate(null, EventArgs.Empty);
        }

        internal static event EventHandler AfterUpdate;

        /// <summary>
        /// 類別：產業別、部門類別、層級別、工作地點
        /// </summary>
        [EMBACore.UDT.Field(Field = "item_category", Indexed = false, Caption = "類別")]
        public string ItemCategory { get; set; }

        /// <summary>
        /// 項目
        /// </summary>
        [EMBACore.UDT.Field(Field = "item", Indexed = false, Caption = "項目")]
        public string Item { get; set; }

        /// <summary>
        /// 不顯示：true-->不顯示；false-->顯示(預設)
        /// </summary>
        [EMBACore.UDT.Field(Field = "not_display", Indexed = false, Caption = "不顯示")]
        public bool NotDisplay { get; set; }
    }
}