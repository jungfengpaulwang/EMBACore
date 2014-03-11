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
    /// 學生按「系所組別」檢視。
    /// </summary>
    public partial class Student_DepartmentGroupView : TreeNavViewBase
    {
        /// <summary>
        /// 建立年班檢視。
        /// </summary>
        public Student_DepartmentGroupView()
            : base()
        {
            InitializeComponent();
            NavText = "系所檢視";

            //當有人變更班級資料時。
            //Class.AfterUpdate += (sender, e) => RenderTreeView();
            //當有人變更學生資料時。
            //Student.AfterChange += (sender, e) => RenderTreeView();
            //  當有人變更系所組別資料時
            //UDT.DepartmentGroup.AfterUpdate += (sender, e) => RenderTreeView(true);

        }

        #region KeyCatalog 排序處理

        private CustomStringComparer NameComparer = new CustomStringComparer();

        protected override int KeyCatalogComparer(KeyCatalog x, KeyCatalog y)
        {
            return (x.Tag + "").CompareTo(y.Tag + "");
            //int X, Y;

            //if (!int.TryParse(x.Tag + "", out X))
            //    X = int.MinValue;

            //if (!int.TryParse(y.Tag + "", out Y))
            //    Y = int.MinValue;

            //return X.CompareTo(Y);
        }

        #endregion

        protected override void GenerateTreeStruct(KeyCatalog root)
        {
            QueryHelper query = new QueryHelper();

            string cmd = @"select student.id, dg.name, class.grade_year, student.status, dg.order from student
left join $ischool.emba.student_brief2 as sb on student.id=sb.ref_student_id
left join $ischool.emba.department_group dg on dg.uid=sb.ref_department_group_id
left join class on class.id=student.ref_class_id
where student.id in(@PrimaryKeys)
order by dg.order, dg.name, class.grade_year";

            StringBuilder primarykeys = new StringBuilder();
            primarykeys.AppendFormat("{0}", "-1"); //如果沒有資料也不會爆掉。
            foreach (string key in Source)
                primarykeys.AppendFormat(",{0}", key);

            cmd = cmd.Replace("@PrimaryKeys", primarykeys.ToString());

            DataTable result = Backend.Select(cmd);

            //KeyCatalog advisor = root.Subcatalogs["系所組別"];
            //KeyCatalog unadvisor = root.Subcatalogs["未設定系所組"];
            //KeyCatalog deleted = root.Subcatalogs["刪除學生"];

            //設定排序因子。
            //advisor.Tag = 0; //排前面。
            //unadvisor.Tag = 1;
            //deleted.Tag = 2;

            root.Subcatalogs["未設定"].Tag = int.MaxValue;
            foreach (DataRow row in result.Rows)
            {
                string department = (string.IsNullOrEmpty((row["name"] + "")) ? "未設定" : (row["name"] + ""));
                //KeyCatalog dg = advisor.Subcatalogs[department];
                GradeYear gy = GradeYear.ToGradeYear(row["grade_year"] + "");
                //string classid = row["classid"] + "";
                string id = row["id"] + "";
                string stauts = row["status"] + "";

                //root.Subcatalogs[department].AddKey(id);
                //if (stauts == "256")
                //    deleted.AddKey(id);
                //else if (string.IsNullOrWhiteSpace(classid))
                //    unadvisor.AddKey(id);
                //else
                //{
                //if (!root.Subcatalogs[department].Subcatalogs.Contains(gy.ChineseTitle))
                //{
                //指定排序因子。
                if (gy == GradeYear.Undefined) //未分年級要排最後。
                    root.Subcatalogs[department][gy.ChineseTitle].Tag = int.MaxValue;
                else
                {
                    root.Subcatalogs[department].Tag = row["order"] + "";
                    root.Subcatalogs[department][gy.ChineseTitle].Tag = row["grade_year"] + "";
                }
                root.Subcatalogs[department][gy.ChineseTitle].AddKey(id);
                //}
            }
        }
    }
}