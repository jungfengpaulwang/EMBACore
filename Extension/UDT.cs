using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FISCA.UDT;
using System.Reflection;

namespace EMBACore.Extension.UDT
{
    public static class UDT
    {
        /// <summary>
        /// 儲存 UDT 資料並記錄 Log
        /// </summary>
        /// <typeparam name="T">泛型介面</typeparam>
        /// <param name="ActiveRecords">繼承自 ActiveRecord 之 UDT 物件</param>
        /// <param name="ActionBy">來源，格式範例：學生.資料項目.經歷</param>        
        /// <param name="prefixString">Log 詳細資料的前置字串</param>        
        /// <param name="subfixString">Log 詳細資料的後置字串</param>        
        /// <param name="targetCategory">資料類別，若內容為 { student, teacher, class, course } 4者之1，則該筆 Log 可於 { 學生, 班級, 教師, 課程 } 對應頁籤中查得</param>
        /// <param name="targetID">被影響的 ID</param>
        /// <returns></returns>
        /// <example>
        /// List<UDT.Paper> Papers;
        /// Papers.SaveAllWithLog("學生.資料項目.指導教授及論文", "", "", Log.LogTargetCategory.Student, PrimaryKey);
        /// </example>
        public static List<string> SaveAllWithLog<T>(this IEnumerable<T> ActiveRecords, string ActionBy, string PrefixString, string SubfixString, Log.LogTargetCategory LogTargetCategory, string TargetName) where T:ActiveRecord
        {
            Log.BatchLogAgent BatchLogAgents = new Log.BatchLogAgent();
            foreach (ActiveRecord record in ActiveRecords)
            {
                FieldInfo FieldInfo_FieldValues = record.GetType().BaseType.GetField("_FieldValues", BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance);
                if (FieldInfo_FieldValues == null)
                    continue;

                Dictionary<string, object> _FieldValues = new Dictionary<string, object>();
                if (FieldInfo_FieldValues.GetValue(record) == null)
                    continue;

                _FieldValues = FieldInfo_FieldValues.GetValue(record) as Dictionary<string, object>;
                string TargetID = string.Empty;
                for (int i = 0; i < _FieldValues.Count; i++)
                {
                    string Field_Name = _FieldValues.ElementAt(i).Key;                  //  UDT 物件所代表資料表的欄位名稱
                    object Field_OldValue = _FieldValues.ElementAt(i).Value;        //  Active Record 之舊值
                    object Field_NewValue = null;                                                        //  Active Record 之新值
                    
                    FieldInfo[] FieldInfo_Fields = record.GetType().GetFields(BindingFlags.Public | BindingFlags.Instance | BindingFlags.NonPublic);

                    if (FieldInfo_Fields != null && FieldInfo_Fields.Count() > 0)
                        Field_NewValue = FieldInfo_Fields[i].GetValue(record);

                    string Field_Caption = string.Empty;                                            //  UDT 物件所代表資料表的欄位中文名稱
                    PropertyInfo[] PropertyInfos = record.GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance | BindingFlags.NonPublic);
                    if (PropertyInfos != null && PropertyInfos.Count() > 0)
                    {
                        object[] CustomAttributes = PropertyInfos[i].GetCustomAttributes(true);
                        if (CustomAttributes != null && CustomAttributes.Count() > 0)
                        {
                            foreach (object CustomAttribute in CustomAttributes)
                            {
                                if (CustomAttribute.GetType() == typeof(EMBACore.UDT.FieldAttribute))
                                    Field_Caption = (CustomAttribute as EMBACore.UDT.FieldAttribute).Caption;
                            }
                        }
                    }
                    Log.LogAgent LogAgent = new Log.LogAgent();
                    if (record.RecordStatus == RecordStatus.Delete)
                    {
                        LogAgent.ActionType = Log.LogActionType.Delete;
                        LogAgent.SetLogValue(Field_Caption, Field_NewValue + "");
                    }
                    if (record.RecordStatus == RecordStatus.Insert)
                    {
                        LogAgent.ActionType = Log.LogActionType.AddNew;
                        LogAgent.SetLogValue(Field_Caption, Field_NewValue + "");
                    }
                    if (record.RecordStatus == RecordStatus.Update)
                    {
                        LogAgent.ActionType = Log.LogActionType.Update;
                        LogAgent.SetLogValue(Field_Caption, Field_OldValue + "");
                        LogAgent.SetLogValue(Field_Caption, Field_NewValue + "");
                    }
                    if (TargetName == Field_Caption)
                        TargetID = Field_NewValue + "";

                    LogAgent.Set(ActionBy, PrefixString, SubfixString, LogTargetCategory, TargetID);
                    BatchLogAgents.AddLogAgent(LogAgent);
                }
            }
            List<string> SavedUIDs = ActiveRecords.SaveAll();
            BatchLogAgents.Save();
            return SavedUIDs;
        }
    }
}