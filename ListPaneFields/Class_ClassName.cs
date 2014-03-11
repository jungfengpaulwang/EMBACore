using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Campus.Windows;
using Campus;

namespace EMBACore.ListPaneFields
{
    internal class Class_ClassName : ListPaneFieldImproved
    {
        public Class_ClassName()
            : base("名稱", "ClassName")
        {
            K12.Data.Class.AfterChange += (x, y) => Reload(y.PrimaryKeys);
        }

        protected override IEnumerable<Value> GetDataAsync(IEnumerable<string> primaryKeys)
        {
            string cmd = "select id,class_name from class where id in (@@PrimaryKeys);";
            List<Value> results = new List<Value>();

            cmd = cmd.ReplaceList("@@PrimaryKeys", primaryKeys);
            Backend.Select(cmd).Each(row =>
            {
                string id = row["id"] + "";
                string val = row["class_name"] + "";
                results.Add(new Value(id, val, string.Empty));
            });

            return results;
        }
    }
}
