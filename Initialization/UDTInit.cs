using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FISCA.UDT;

namespace EMBACore.Initialization
{
    public class UDTInit
    {
        public static void Init() 
        {
            #region 模組啟用先同步Schmea

            SchemaManager Manager = new SchemaManager(FISCA.Authentication.DSAServices.DefaultConnection);

            Manager.SyncSchema(new EMBACore.UDT.Absence());
            Manager.SyncSchema(new EMBACore.UDT.AddDropCourse());
            Manager.SyncSchema(new EMBACore.UDT.CourseExt());
            Manager.SyncSchema(new EMBACore.UDT.CourseInstructor());
            Manager.SyncSchema(new EMBACore.UDT.CourseTypeDataSource());
            Manager.SyncSchema(new EMBACore.UDT.DepartmentGroup());
            Manager.SyncSchema(new EMBACore.UDT.DoNotTakeCourseInSummer());
            Manager.SyncSchema(new EMBACore.UDT.EducationBackground());
            Manager.SyncSchema(new EMBACore.UDT.Experience());
            Manager.SyncSchema(new EMBACore.UDT.ExperienceDataSource());
            Manager.SyncSchema(new EMBACore.UDT.GradeScoreMappingTable());
            Manager.SyncSchema(new EMBACore.UDT.GraduationRequirement());
            Manager.SyncSchema(new EMBACore.UDT.GraduationSubjectGroupRequirement());
            Manager.SyncSchema(new EMBACore.UDT.GraduationSubjectList());
            Manager.SyncSchema(new EMBACore.UDT.MailLog());
            Manager.SyncSchema(new EMBACore.UDT.Paper());
            Manager.SyncSchema(new EMBACore.UDT.PaymentHistory());
            Manager.SyncSchema(new EMBACore.UDT.SCAttendExt());
            Manager.SyncSchema(new EMBACore.UDT.ScoreInputSemester());
            Manager.SyncSchema(new EMBACore.UDT.StudentBrief2());
            Manager.SyncSchema(new EMBACore.UDT.StudentRemark());
            Manager.SyncSchema(new EMBACore.UDT.Subject());
            Manager.SyncSchema(new EMBACore.UDT.SubjectScoreLock());
            Manager.SyncSchema(new EMBACore.UDT.SubjectSemesterScore());
            Manager.SyncSchema(new EMBACore.UDT.SyncStudentLock());
            Manager.SyncSchema(new EMBACore.UDT.TeacherExtVO());
            Manager.SyncSchema(new EMBACore.UDT.SubjectSemesterScoreInputRule());

            #endregion
        }
    }
}
