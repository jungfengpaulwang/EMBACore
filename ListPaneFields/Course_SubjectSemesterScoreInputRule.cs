using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Campus.Windows;
using System.Data;
using Campus;
using FISCA.UDT;

namespace EMBACore.ListPaneFields
{
    internal class Course_SubjectSemesterScoreInputRule : ListPaneFieldImproved
    {
        public static Course_SubjectSemesterScoreInputRule Instance { get; private set; }

        static Course_SubjectSemesterScoreInputRule()
        {
            Instance = new Course_SubjectSemesterScoreInputRule();
        }

        public Course_SubjectSemesterScoreInputRule()
            : base("成績輸入規則", "CourseSubjectSemesterScoreInputRule")
        {
            //K12.Data.Student.AfterChange += (x, y) => Reload(y.PrimaryKeys);
        }

        public void ReloadMe()
        {
            this.Reload();
        }

        protected override IEnumerable<Value> GetDataAsync(IEnumerable<string> primaryKeys)
        {
            //List<UDT.SubjectSemesterScoreInputRule> SubjectSemesterScoreInputRules = (new AccessHelper()).Select<UDT.SubjectSemesterScoreInputRule>(string.Format("ref_course_id in ({0})", string.Join(",", primaryKeys)));

            //List<Value> results = new List<Value>();
            //SubjectSemesterScoreInputRules.ForEach((x) => results.Add(new Value(x.UID, "P/N 制", string.Empty)));

            //return results;

            string cmd = @"select ref_course_id, input_rule from $ischool.emba.course.subject_semester_score_input_rule where ref_course_id in (@@PrimaryKeys);";

            List<Value> results = new List<Value>();

            cmd = cmd.ReplaceList("@@PrimaryKeys", primaryKeys);
            Backend.Select(cmd).Each(row =>
            {
                string id = row["ref_course_id"] + "";
                string CourseSubjectSemesterScoreInputRule = "P/N 制";
                results.Add(new Value(id, CourseSubjectSemesterScoreInputRule, string.Empty));
            });

            return results;
        }
    }
}
