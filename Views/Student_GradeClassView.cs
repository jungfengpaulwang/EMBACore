using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Campus;
using DevComponents.AdvTree;
using FISCA.Data;
using FISCA.Presentation;
using K12.Data;

namespace EMBACore.Views
{
    /// <summary>
    /// 學生按「年級、班級」檢視。
    /// </summary>
    public partial class Student_GradeClassView : TreeNavViewBase
    {
        /// <summary>
        /// 建立年班檢視。
        /// </summary>
        public Student_GradeClassView()
            : base()
        {
            InitializeComponent();
            NavText = "班級檢視";

            //當有人變更班級資料時。
            //Class.AfterUpdate += (sender, e) => RenderTreeView();
            //當有人變更學生資料時。
            //Student.AfterChange += (sender, e) => RenderTreeView();
        }

        #region KeyCatalog 排序處理

        private CustomStringComparer NameComparer = new CustomStringComparer();

        protected override int KeyCatalogComparer(KeyCatalog x, KeyCatalog y)
        {
            if (x.Tag != null && y.Tag != null)
            {//按年級排序。
                GradeYear gyX = GradeYear.ToGradeYear(x.Tag + "");
                GradeYear gyY = GradeYear.ToGradeYear(y.Tag + "");
                return gyX.Number.CompareTo(gyY.Number);
            }
            else //按名字排序。
                return NameComparer.Compare(x.Name, y.Name);
        }

        #endregion

        protected override void GenerateTreeStruct(KeyCatalog root)
        {
            QueryHelper query = new QueryHelper();

            string cmd =
@"select student.id,class_name,class.grade_year
from student left join class on student.ref_class_id=class.id
where student.id in(@PrimaryKeys)";

            StringBuilder primarykeys = new StringBuilder();
            primarykeys.AppendFormat("{0}", "-1"); //如果沒有資料也不會爆掉。
            foreach (string key in Source)
                primarykeys.AppendFormat(",{0}", key);

            cmd = cmd.Replace("@PrimaryKeys", primarykeys.ToString());

            DataTable result = query.Select(cmd);

            root.Clear();
            foreach (DataRow row in result.Rows)
            {
                GradeYear gy = GradeYear.ToGradeYear(row["grade_year"] + "");
                string className = row["class_name"].ToString();
                string id = row["id"].ToString();

                if (string.IsNullOrWhiteSpace(className))
                    className = "未分班";

                if (!root.Subcatalogs.Contains(gy.ChineseTitle))
                {
                    //指定排序因子。
                    if (gy == GradeYear.Undefined) //未分年級要排最後。
                        root[gy.ChineseTitle].Tag = int.MaxValue;
                    else
                        root[gy.ChineseTitle].Tag = gy.Number;
                }

                root[gy.ChineseTitle][className].AddKey(id);
            }
        }
    }
}