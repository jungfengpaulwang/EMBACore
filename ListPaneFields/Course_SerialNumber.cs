using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Campus.Windows;
using Campus;

namespace EMBACore.ListPaneFields
{
    class Course_SerialNumber : ListPaneFieldImproved
    {
        public Course_SerialNumber()
            : base("流水號", "SerialNumber")
        {
            K12.Data.Course.AfterChange += (x, y) => Reload(y.PrimaryKeys);
        }

        protected override IEnumerable<Value> GetDataAsync(IEnumerable<string> primaryKeys)
        {
            string cmd = "select ref_course_id,serial_no from $ischool.emba.course_ext where ref_course_id in (@@PrimaryKeys);";
            List<Value> results = new List<Value>();

            cmd = cmd.ReplaceList("@@PrimaryKeys", primaryKeys);
            Backend.Select(cmd).Each(row =>
            {
                string id = row["ref_course_id"].ToString();
                string val = row["serial_no"] + "";
                results.Add(new Value(id, val, string.Empty));
            });

            return results;
        }
    }
}
