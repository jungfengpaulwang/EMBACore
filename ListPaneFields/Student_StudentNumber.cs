using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Campus.Windows;
using System.Data;
using Campus;

namespace EMBACore.ListPaneFields
{
    internal class Student_StudentNumber : ListPaneFieldImproved
    {
        public Student_StudentNumber()
            : base("學號", "StudentNumber")
        {
            K12.Data.Student.AfterChange += (x, y) => Reload(y.PrimaryKeys);
        }

        protected override IEnumerable<Value> GetDataAsync(IEnumerable<string> primaryKeys)
        {
            string cmd = "select id,student_number from student where id in (@@PrimaryKeys);";
            List<Value> results = new List<Value>();

            cmd = cmd.ReplaceList("@@PrimaryKeys", primaryKeys);
            Backend.Select(cmd).Each(row =>
            {
                string id = row["id"] + "";
                string studNumber = row["student_number"] + "";
                results.Add(new Value(id, studNumber, string.Empty));
            });

            return results;
        }
    }
}
