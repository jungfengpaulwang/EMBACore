using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Campus.Windows;
using System.Data;
using Campus;

namespace EMBACore.ListPaneFields
{
    internal class Student_GraduationRequirement : ListPaneFieldImproved
    {
        public static Student_GraduationRequirement Instance { get; private set; }

        static Student_GraduationRequirement()
        {
            Instance = new Student_GraduationRequirement();
        }

        public Student_GraduationRequirement()
            : base("畢業條件", "GraduationRequirementName")
        {
            //K12.Data.Student.AfterChange += (x, y) => Reload(y.PrimaryKeys);
        }

        public void ReloadMe()
        {
            this.Reload();
        }

        protected override IEnumerable<Value> GetDataAsync(IEnumerable<string> primaryKeys)
        {
            string cmd = @"select $ischool.emba.student_brief2.ref_student_id id, $ischool.emba.department_group.name departmentgroup, $ischool.emba.graduation_requirement.name graduationrequirement from $ischool.emba.student_brief2 join $ischool.emba.graduation_requirement on $ischool.emba.student_brief2.ref_graduation_requirement_id=$ischool.emba.graduation_requirement.uid join $ischool.emba.department_group on $ischool.emba.department_group.uid=$ischool.emba.graduation_requirement.ref_department_group_id
 where $ischool.emba.student_brief2.ref_student_id in (@@PrimaryKeys);";

            List<Value> results = new List<Value>();

            cmd = cmd.ReplaceList("@@PrimaryKeys", primaryKeys);
            Backend.Select(cmd).Each(row =>
            {
                string id = row["id"] + "";
                string GraduationRequirementName = (row["departmentgroup"] + "") + "-" + (row["graduationrequirement"] + "");
                results.Add(new Value(id, GraduationRequirementName, string.Empty));
            });

            return results;
        }
    }
}
