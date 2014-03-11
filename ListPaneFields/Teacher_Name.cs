using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Campus.Windows;
using Campus;

namespace EMBACore.ListPaneFields
{
    internal class Teacher_Name : ListPaneFieldImproved
    {
        public Teacher_Name()
            : base("姓名", "TeacherName")
        {
            K12.Data.Teacher.AfterChange += (x, y) => Reload(y.PrimaryKeys);
        }

        protected override IEnumerable<Value> GetDataAsync(IEnumerable<string> primaryKeys)
        {
            string cmd = "select id,teacher_name from teacher where id in (@@PrimaryKeys);";
            List<Value> results = new List<Value>();

            cmd = cmd.ReplaceList("@@PrimaryKeys", primaryKeys);
            Backend.Select(cmd).Each(row =>
            {
                string id = row["id"] + "";
                string val = row["teacher_name"] + "";
                results.Add(new Value(id, val, string.Empty));
            });

            return results;
        }
    }
}
