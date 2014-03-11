using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Campus.Windows;
using Campus;

namespace EMBACore.ListPaneFields
{
    internal class Course_ClassName : ListPaneFieldImproved    
    {
        public Course_ClassName()
            : base("開課班次", "ClassName")
        {
            K12.Data.Course.AfterChange += (x, y) => Reload(y.PrimaryKeys); 
        }

        protected override IEnumerable<Value> GetDataAsync(IEnumerable<string> primaryKeys)
        {
            string cmd = "select ref_course_id , class_name from $ischool.emba.course_ext where ref_course_id in (@@PrimaryKeys);";
            List<Value> results = new List<Value>();

            cmd = cmd.ReplaceList("@@PrimaryKeys", primaryKeys);
            Backend.Select(cmd).Each(row =>
            {
                string id = row["ref_course_id"] + "";
                string class_name = row["class_name"] + "";
                results.Add(new Value(id, class_name, string.Empty));
            });

            return results;
        }
    }
}
