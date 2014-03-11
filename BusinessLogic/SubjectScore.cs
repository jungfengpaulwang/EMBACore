using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace EMBACore.BusinessLogic
{
    public class SubjectScore
    {
        public static Dictionary<string, bool> IsPass(List<UDT.SubjectSemesterScore> SubjectSemesterScores)
        {
            Dictionary<string, bool> dicScores = new Dictionary<string, bool>();
            SubjectSemesterScores.ForEach(x => dicScores.Add(x.UID, IsPass(x)));
            return dicScores;
        }

        public static bool IsPass(UDT.SubjectSemesterScore SubjectSemesterScore)
        {            
            return SubjectScore.IsPass(SubjectSemesterScore.Score);
        }

        public static Dictionary<string, bool> IsPass(List<UDT.SCAttendExt> SCAttendExts)
        {
            Dictionary<string, bool> dicScores = new Dictionary<string, bool>();
            SCAttendExts.ForEach(x => dicScores.Add(x.UID, IsPass(x)));
            return dicScores;
        }

        public static bool IsPass(UDT.SCAttendExt SCAttendExt)
        {
            return !SCAttendExt.IsCancel;
        }

        public static bool IsPass(string Score)
        {
            string[] pass_scores = new string[] { "A+", "A", "A-", "B+", "B", "B-" };
            if (pass_scores.Contains(Score))
                return true;
            else
                return false;
        }
    }
}
