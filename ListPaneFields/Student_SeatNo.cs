using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Campus.Windows;
using Campus;

namespace EMBACore.ListPaneFields
{
    internal class Student_SeatNo: ListPaneFieldImproved
    {
        public Student_SeatNo()
            : base("座號","SeatNo" )
        {
            K12.Data.Student.AfterChange += (x, y) => Reload(y.PrimaryKeys);
        }

        protected override void CompareValue(object sender, FISCA.Presentation.CompareValueEventArgs e)
        {
            int x, y;
            if (!int.TryParse(e.Value1.ToString(), out x))
                x = -1;
            if (!int.TryParse(e.Value2.ToString(), out y))
                y = -1;

            e.Result = x.CompareTo(y);
        }

        protected override IEnumerable<Value> GetDataAsync(IEnumerable<string> primaryKeys)
        {
            string cmd = "select student.id,seat_no from student where id in (@@PrimaryKeys);";
            List<Value> results = new List<Value>();

            cmd = cmd.ReplaceList("@@PrimaryKeys", primaryKeys);
            Backend.Select(cmd).Each(row =>
            {
                string id = row["id"] + "";
                string studNumber = row["seat_no"] + "";
                results.Add(new Value(id, studNumber, string.Empty));
            });

            return results;
        }
    }
}
