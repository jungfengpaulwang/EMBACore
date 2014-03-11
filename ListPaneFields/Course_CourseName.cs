using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Campus.Windows;
using Campus;

namespace EMBACore.ListPaneFields
{
    internal class Course_CourseName : ListPaneFieldImproved
    {
        public Course_CourseName()
            : base("名稱", "CourseName")
        {
            K12.Data.Course.AfterChange += (x, y) => Reload(y.PrimaryKeys);
        }

        protected override IEnumerable<Value> GetDataAsync(IEnumerable<string> primaryKeys)
        {
            string cmd = "select id,course_name from course where id in (@@PrimaryKeys);";
            List<Value> results = new List<Value>();

            cmd = cmd.ReplaceList("@@PrimaryKeys", primaryKeys);
            Backend.Select(cmd).Each(row =>
            {
                string id = row["id"] + "";
                string val = row["course_name"] + "";
                results.Add(new Value(id, val, string.Empty));
            });

            return results;
        }
    }
}
