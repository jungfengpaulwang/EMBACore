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
    /// 教師的班導師檢視
    /// </summary>
    public class Teacher_AdvisorView : TreeNavViewBase
    {
        public Teacher_AdvisorView()
            : base()
        {
            NavText = "班導師檢視";
            //當班級資料變更時，重新整理 TreeView 上的資訊。
            //Teacher.AfterChange += (sender, e) => RenderTreeView();
            ShowRoot = false;
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
@"select teacher.id,class.id classid,class.grade_year,teacher.status
from teacher left join class on teacher.id=class.ref_teacher_id
where teacher.id in(@PrimaryKeys)";

            StringBuilder primarykeys = new StringBuilder();
            primarykeys.AppendFormat("{0}", "-1"); //如果沒有資料也不會爆掉。
            foreach (string key in Source)
                primarykeys.AppendFormat(",{0}", key);

            cmd = cmd.Replace("@PrimaryKeys", primarykeys.ToString());

            DataTable result = Backend.Select(cmd);

            KeyCatalog advisor = root.Subcatalogs["班導師"];
            KeyCatalog unadvisor = root.Subcatalogs["非班導師"];
            KeyCatalog deleted = root.Subcatalogs["刪除教師"];
            
            //設定排序因子。
            advisor.Tag = 0; //排前面。
            unadvisor.Tag = 1;
            deleted.Tag = 2;

            foreach (DataRow row in result.Rows)
            {
                GradeYear gy = GradeYear.ToGradeYear(row["grade_year"] + "");
                string classid = row["classid"] + "";
                string id = row["id"] + "";
                string stauts = row["status"] + "";

                if (stauts == "256")
                    deleted.AddKey(id);
                else if (string.IsNullOrWhiteSpace(classid))
                    unadvisor.AddKey(id);
                else
                {
                    if (!advisor.Subcatalogs.Contains(gy.ChineseTitle))
                    {
                        //指定排序因子。
                        if (gy == GradeYear.Undefined) //未分年級要排最後。
                            advisor[gy.ChineseTitle].Tag = int.MaxValue;
                        else
                            advisor[gy.ChineseTitle].Tag = gy.Number;
                    }
                    advisor[gy.ChineseTitle].AddKey(id);
                }
            }
        }
    }
}
