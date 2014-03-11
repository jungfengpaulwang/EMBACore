using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using Campus;
using FISCA.Data;
using K12.Data;

namespace EMBACore.Views
{
    /// <summary>
    /// 班級的年級檢視。
    /// </summary>
    public class Class_GradeView : TreeNavViewBase
    {
        public Class_GradeView()
            : base()
        {
            //InitializeComponent();
            NavText = "年級檢視";

            //當班級資料變更時，重新整理 TreeView 上的資訊。
            //Class.AfterChange += (sender, e) => RenderTreeView();
            RootCaption = "所有班級";
        }

        protected override int KeyCatalogComparer(KeyCatalog x, KeyCatalog y)
        {
            int X, Y;

            if (!int.TryParse(x.Tag + "", out X))
                X = int.MinValue;

            if (!int.TryParse(y.Tag + "", out Y))
                Y = int.MinValue;

            return X.CompareTo(Y);
        }

        protected override void GenerateTreeStruct(KeyCatalog root)
        {
            string cmd =
@"select id,grade_year from class
where class.id in(@PrimaryKeys)";

            StringBuilder primarykeys = new StringBuilder();
            primarykeys.AppendFormat("{0}", "-1"); //如果沒有資料也不會爆掉。
            foreach (string key in Source)
                primarykeys.AppendFormat(",{0}", key);

            cmd = cmd.Replace("@PrimaryKeys", primarykeys.ToString());

            DataTable result = Backend.Select(cmd);

            foreach (DataRow row in result.Rows)
            {
                GradeYear gy = GradeYear.ToGradeYear(row["grade_year"] + "");
                string id = row["id"].ToString();

                if (!root.Subcatalogs.Contains(gy.ChineseTitle))
                {
                    //指定排序因子。
                    if (gy == GradeYear.Undefined) //未分年級要排最後。
                        root[gy.ChineseTitle].Tag = int.MaxValue;
                    else
                        root[gy.ChineseTitle].Tag = gy.Number;
                }

                root[gy.ChineseTitle].AddKey(id);
            }
        }
    }
}
