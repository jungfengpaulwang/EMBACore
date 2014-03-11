using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FISCA.UDT;

namespace EMBACore.Forms.SeatTable
{
   /// <summary>
    /// 缺曠紀錄
    /// </summary>
    [FISCA.UDT.TableName("ischool.emba.classroom_layout")]
    public class UDT_ClassRoomLayout : ActiveRecord
    {
        /// <summary>
        /// 課程代碼
        /// </summary>
        [EMBACore.UDT.Field(Field = "classroom_name", Indexed = false, Caption = "")]
        public string ClassroomName { get; set; }

        /// <summary>
        /// 教室座位列數
        /// </summary>
        [EMBACore.UDT.Field(Field = "x_count", Indexed = false, Caption = "")]
        public int XCount { get; set; }

        /// <summary>
        /// 教室座位行數
        /// </summary>
        [EMBACore.UDT.Field(Field = "y_count", Indexed = false, Caption = "")]
        public int YCount { get; set; }

        /// <summary>
        /// 座位位置，XML 字串描述座標點
        /// </summary>
        [EMBACore.UDT.Field(Field = "seat_cells", Indexed = false, Caption = "")]
        public string SeatCells { get; set; }

        /// <summary>
        /// 邊界位置，XML 字串描述座標點
        /// </summary>
        [EMBACore.UDT.Field(Field = "boundary_cells", Indexed = false, Caption = "")]
        public string Boundary_cells { get; set; }

        /// <summary>
        /// 匯出的 Excel 樣版，會決定呼叫 Resource 中的那一個 Excel.
        /// </summary>
        [EMBACore.UDT.Field(Field = "excel_template", Indexed = false, Caption = "")]
        public string ExcelTemplate { get; set; }
    }
}
