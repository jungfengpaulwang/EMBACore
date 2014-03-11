using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Campus.Windows;
using Campus;

namespace EMBACore.ListPaneFields
{
    internal class Student_Name: ListPaneFieldImproved
    {
        public Student_Name()
            : base("姓名", "Name")
        {
            K12.Data.Student.AfterChange += (x, y) => Reload(y.PrimaryKeys);
        }

        protected override IEnumerable<Value> GetDataAsync(IEnumerable<string> primaryKeys)
        {
            string cmd = "select student.id,name from student where id in (@@PrimaryKeys);";
            List<Value> results = new List<Value>();

            cmd = cmd.ReplaceList("@@PrimaryKeys", primaryKeys);
            Backend.Select(cmd).Each(row =>
            {
                string id = row["id"] + "";
                string studNumber = row["name"] + "";
                results.Add(new Value(id, studNumber, string.Empty));
            });

            return results;
        }
    }
}
