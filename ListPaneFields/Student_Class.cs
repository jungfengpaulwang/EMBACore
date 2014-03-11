using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Campus.Windows;
using Campus;

namespace EMBACore.ListPaneFields
{
    internal class Student_Class : ListPaneFieldImproved
    {
        public Student_Class()
            : base("班級", "ClassName")
        {
            K12.Data.Student.AfterChange += (x, y) => Reload(y.PrimaryKeys);
            K12.Data.Class.AfterChange += (x, y) => Reload();
        }

        protected override IEnumerable<Value> GetDataAsync(IEnumerable<string> primaryKeys)
        {
            string cmd = "select student.id,class_name from student left join class on student.ref_class_id=class.id where student.id in (@@PrimaryKeys);";
            List<Value> results = new List<Value>();

            cmd = cmd.ReplaceList("@@PrimaryKeys", primaryKeys);
            Backend.Select(cmd).Each(row =>
            {
                string id = row["id"] + "";
                string studNumber = row["class_name"] + "";
                results.Add(new Value(id, studNumber, string.Empty));
            });

            return results;
        }
    }
}
