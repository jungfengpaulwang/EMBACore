using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FISCA.LogAgent;

namespace EMBACore.Log
{
    class BatchLogAgent
    {
        private List<LogAgent> _LogAgents;

        public BatchLogAgent()
        {
            this._LogAgents = new List<LogAgent>();
        }

        public void AddLogAgent(LogAgent logAgent)
        {
            this._LogAgents.Add(logAgent);
        }

        public void AddLogAgents(IEnumerable<LogAgent> logAgents)
        {
            foreach(LogAgent agent in logAgents)
                this._LogAgents.Add(agent);
        }

        public void Clear()
        {
            this._LogAgents.Clear();
        }

        public int Count 
        {
            get { return this._LogAgents.Count; }
        }

        private string GetTargetDetail(LogTargetCategory targetCategory, object dTarget)
        {
            string target = string.Empty;
            switch (targetCategory)
            {
                case LogTargetCategory.Student:
                    K12.Data.StudentRecord StudentRecod = dTarget as K12.Data.StudentRecord;
                    target = "學生「" + StudentRecod.Name + "」，學號「" + StudentRecod.StudentNumber + "」\n";
                    break;
                case LogTargetCategory.Class:
                    K12.Data.ClassRecord ClassRecod = dTarget as K12.Data.ClassRecord;
                    target = "班級「" + ClassRecod.Name + "」\n";
                    break;
                case LogTargetCategory.Teacher:
                    K12.Data.TeacherRecord TeacherRecod = dTarget as K12.Data.TeacherRecord;
                    target = "教師「" + TeacherRecod.Name + "」，暱稱「" + TeacherRecod.Nickname + "」\n";
                    break;
                case LogTargetCategory.Course:
                    K12.Data.CourseRecord CourseRecod = dTarget as K12.Data.CourseRecord;
                    target = "課程「" + CourseRecod.Name + "」，學年度「" + (CourseRecod.SchoolYear + "") + "」，學期「" + EMBACore.DataItems.SemesterItem.GetSemesterByCode(CourseRecod.Semester + "").Name + "」\n";
                    break;
                case LogTargetCategory.SystemSetting:
                    target = "學年度學期\n";
                    break;
                default:
                    target = "UFO\n";
                    break;
            }
            return target;
        }

        public void Save()
        {
            if (this._LogAgents.Count == 0)
                return;

            List<string> StudentIDs = new List<string>();
            List<string> CourseIDs = new List<string>();
            List<string> ClassIDs = new List<string>();
            List<string> TeacherIDs = new List<string>();
            foreach (LogAgent logAgent in this._LogAgents)
            {
                if (logAgent.LogTargetCategory == LogTargetCategory.Student)
                    StudentIDs.Add(logAgent.TargetID);
                else if (logAgent.LogTargetCategory == LogTargetCategory.Class)
                    ClassIDs.Add(logAgent.TargetID);
                else if (logAgent.LogTargetCategory == LogTargetCategory.Course)
                    CourseIDs.Add(logAgent.TargetID);
                else if (logAgent.LogTargetCategory == LogTargetCategory.Teacher)
                    TeacherIDs.Add(logAgent.TargetID);
            }
            List<K12.Data.StudentRecord> StudentRecords = K12.Data.Student.SelectByIDs(StudentIDs.Distinct());
            List<K12.Data.ClassRecord> ClassRecords = K12.Data.Class.SelectByIDs(ClassIDs.Distinct());
            List<K12.Data.TeacherRecord> TeacherRecords = K12.Data.Teacher.SelectByIDs(TeacherIDs.Distinct());
            List<K12.Data.CourseRecord> CourseRecords = K12.Data.Course.SelectByIDs(CourseIDs.Distinct());
            Dictionary<string, K12.Data.StudentRecord> dicStudentRecords = new Dictionary<string, K12.Data.StudentRecord>();
            Dictionary<string, K12.Data.ClassRecord> dicClassRecords = new Dictionary<string, K12.Data.ClassRecord>();
            Dictionary<string, K12.Data.TeacherRecord> dicTeacherRecords = new Dictionary<string, K12.Data.TeacherRecord>();
            Dictionary<string, K12.Data.CourseRecord> dicCourseRecords = new Dictionary<string, K12.Data.CourseRecord>();
            if (StudentRecords.Count > 0)
                dicStudentRecords = StudentRecords.ToDictionary(x => x.ID);
            if (ClassRecords.Count > 0)
                dicClassRecords = ClassRecords.ToDictionary(x => x.ID);
            if (TeacherRecords.Count > 0)
                dicTeacherRecords = TeacherRecords.ToDictionary(x => x.ID);
            if (CourseRecords.Count > 0)
                dicCourseRecords = CourseRecords.ToDictionary(x => x.ID);
            
            LogSaver logBatch = ApplicationLog.CreateLogSaverInstance();
            foreach (LogAgent logAgent in this._LogAgents)
            {
                if (string.IsNullOrWhiteSpace(logAgent.LogDetail))
                    continue;

                string target_detail = string.Empty;
                if (string.IsNullOrEmpty(logAgent.TargetID))
                    target_detail = logAgent.TargetDetail;
                else
                {
                    if (logAgent.LogTargetCategory == LogTargetCategory.Student)
                        target_detail = this.GetTargetDetail(logAgent.LogTargetCategory, dicStudentRecords[logAgent.TargetID]);
                    else if (logAgent.LogTargetCategory == LogTargetCategory.Class)
                        target_detail = this.GetTargetDetail(logAgent.LogTargetCategory, dicClassRecords[logAgent.TargetID]);
                    else if (logAgent.LogTargetCategory == LogTargetCategory.Course)
                        target_detail = this.GetTargetDetail(logAgent.LogTargetCategory, dicCourseRecords[logAgent.TargetID]);
                    else if (logAgent.LogTargetCategory == LogTargetCategory.Teacher)
                        target_detail = this.GetTargetDetail(logAgent.LogTargetCategory, dicTeacherRecords[logAgent.TargetID]);
                }

                string action_type = string.Empty;
                if (logAgent.ActionType == LogActionType.Delete)
                    action_type = "刪除";
                else if (logAgent.ActionType == LogActionType.Update)
                    action_type = "修改";
                else if (logAgent.ActionType == LogActionType.AddNew)
                    action_type = "新增";

                logBatch.AddBatch(logAgent.ActionBy, action_type, logAgent.LogTargetCategory.ToString().ToLower(), logAgent.TargetID, target_detail + logAgent.LogDetail);
            }
            logBatch.LogBatch(true);
        }
    }
}