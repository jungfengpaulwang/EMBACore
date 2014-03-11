using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FISCA.LogAgent;
using FISCA.UDT;
using System.Reflection;

namespace EMBACore.Log
{
    /// <summary>
    /// Log記錄轉換者
    /// </summary>
    public class LogAgent
    {
        Dictionary<string, LogValue> _LogValueDict;
        private string _ActionBy;
        private string _PrefixString;
        private string _SubfixString;
        private LogTargetCategory _LogTargetCategory;
        private string _TargetID;
        private string _TargetDetail;
        private string _LogDetail;

        public LogAgent()
        {
            this._LogValueDict = new Dictionary<string, LogValue>();
            this.InitInstanceVariable();
        }

        private void InitInstanceVariable()
        {
            this._ActionBy = string.Empty;
            this._PrefixString = string.Empty;
            this._SubfixString = string.Empty;
            this._TargetID = string.Empty;
            this._LogDetail = string.Empty;
            this._TargetDetail = string.Empty;
        }
        /// <summary>
        /// 設定Log值
        /// </summary>
        /// <param name="Name"></param>
        /// <param name="Value"></param>
        public void SetLogValue(string Name, string Value)
        {
            if (!_LogValueDict.ContainsKey(Name))
            {
                LogValue lv = new LogValue();
                lv.Name = Name;
                lv.OldValue = Value;
                lv.NewValue = "";
                _LogValueDict.Add(Name, lv);
            }
            else
            {
                _LogValueDict[Name].NewValue = Value;
            }
        }

        /// <summary>
        /// 取得LogValueList
        /// </summary>
        /// <returns></returns>
        public List<LogValue> getLogValueList()
        {
            return _LogValueDict.Values.ToList();
        }
        
        /// <summary>
        /// 請除LogValueList值
        /// </summary>
        public void Clear()
        {
            _LogValueDict.Clear();
            this.InitInstanceVariable();
        }

        /// <summary>
        /// 新增、修改或刪除
        /// </summary>
        public LogActionType ActionType { get; set; }

        public string ActionBy
        {
            get { return this._ActionBy; }
            private set { this._ActionBy = value; }
        }

        public string PrefixString
        {
            get { return this._PrefixString; }
            private set { this._PrefixString = value; }
        }

        public string SubfixString
        {
            get { return this._SubfixString; }
            private set { this._SubfixString = value; }
        }

        public LogTargetCategory LogTargetCategory
        {
            get { return this._LogTargetCategory; }
            private set { this._LogTargetCategory = value; }
        }

        public string TargetID
        {
            get { return this._TargetID; }
            private set { this._TargetID = value; }
        }

        public string TargetDetail { get; set; }

        public string LogDetail
        {
            get { return this._LogDetail; }
            private set { this._LogDetail = value; }
        }

        public void Set(string ActionBy, string PrefixString, string SubfixString, LogTargetCategory LogTargetCategory)
        {            
            this.Set(ActionBy, PrefixString, SubfixString, LogTargetCategory, string.Empty);
        }

        public void Set(string ActionBy, string PrefixString, string SubfixString, LogTargetCategory LogTargetCategory, string TargetID)
        {
            this.ActionBy = ActionBy;
            this.PrefixString = PrefixString;
            this.SubfixString = SubfixString;
            this.LogTargetCategory = LogTargetCategory;
            this.TargetID = TargetID;

            StringBuilder sb = new StringBuilder();
            sb.Append(PrefixString);
            foreach (LogValue lv in this.getLogValueList())
            {
                //  ischool desktop log 機制乃記錄使用者對於資料之異動，若資料無異動即不應記錄。
                if (!lv.isSame())
                {
                    if (ActionType == LogActionType.Delete)
                    {
                        sb.Append(lv.getDeleteString1());
                    }
                    else if (ActionType == LogActionType.Update)
                    {
                        sb.Append(lv.getChangeString1());
                    }
                    else if (ActionType == LogActionType.AddNew)
                    {
                        sb.Append(lv.getInsertString1());
                    }
                    sb.Append("\n");
                }
            }
            sb.Append(SubfixString);
            this.LogDetail = sb.ToString();
        }

        /// <summary>
        /// 單筆儲存 Log
        /// </summary>
        /// <param name="ActionBy">來源，格式範例：學生.資料項目.經歷；課程.匯入.課程基本資料</param>        
        /// <param name="prefixString">Log 詳細資料的前置字串</param>        
        /// <param name="subfixString">Log 詳細資料的後置字串</param>        
        /// <param name="targetCategory">資料類別，若內容為 { student, teacher, class, course } 4者之1，則該筆 Log 可於 { 學生, 班級, 教師, 課程 } 對應頁籤中查得</param>
        /// <param name="targetID">被影響的 ID</param>
        public void Save(string ActionBy,  string prefixString, string subfixString, LogTargetCategory targetCategory, string targetID)
        {
            this.Set(ActionBy, prefixString, subfixString, targetCategory, targetID);
            string target_detail = this.GetTargetDetail(targetCategory, targetID);

            string action_type = string.Empty;
            if (ActionType == LogActionType.Delete)
                action_type = "刪除";
            else if (ActionType == LogActionType.Update)
                action_type = "修改";
            else if (ActionType == LogActionType.AddNew)
                action_type = "新增";

            if (!string.IsNullOrWhiteSpace(this.LogDetail))
                FISCA.LogAgent.ApplicationLog.Log(ActionBy, action_type, targetCategory.ToString().ToLower(), targetID, target_detail + this.LogDetail);
        }

        private string GetTargetDetail(LogTargetCategory targetCategory, string targetID)
        {
            if (string.IsNullOrEmpty(targetID))
                return this.TargetDetail;

            string target = string.Empty;
            switch (targetCategory)
            {
                case LogTargetCategory.Student:
                    K12.Data.StudentRecord StudentRecod = K12.Data.Student.SelectByID(targetID);
                    target = "學生「" + StudentRecod.Name + "」，學號「" + StudentRecod.StudentNumber + "」\n";
                    break;
                case LogTargetCategory.Class:
                    K12.Data.ClassRecord ClassRecod = K12.Data.Class.SelectByID(targetID);
                    target = "班級「" + ClassRecod.Name + "」\n";
                    break;
                case LogTargetCategory.Teacher:
                    K12.Data.TeacherRecord TeacherRecod = K12.Data.Teacher.SelectByID(targetID);
                    target = "教師「" + TeacherRecod.Name + "」，暱稱「" + TeacherRecod.Nickname + "」\n";
                    break;
                case LogTargetCategory.Course:
                    K12.Data.CourseRecord CourseRecod = K12.Data.Course.SelectByID(targetID);
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
    } 
    // end of class

    public enum LogActionType
    {
        AddNew,
        Update,
        Delete
    }

    public enum LogTargetCategory
    {
        Student,
        Teacher,
        Class,
        Course,
        SystemSetting
    }
}
