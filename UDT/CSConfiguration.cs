using System;
using FISCA.UDT;
using System.Collections.Generic;
using System.Linq;

namespace EMBACore.UDT
{
    /// <summary>
    /// web 端說明(提醒)文字
    /// </summary>
    [FISCA.UDT.TableName("ischool.emba.cs_configuration")]
    public class CSConfiguration : ActiveRecord
    {
        //internal static void RaiseAfterUpdateEvent(object sender, ParameterEventArgs e)
        //{
        //    if (CSAttend.AfterUpdate != null)
        //        CSAttend.AfterUpdate(sender, e);
        //}

        //internal static event EventHandler<ParameterEventArgs> AfterUpdate;

        public enum TemplateName { 寄信人 = 0, 選課最終確認說明文字 = 1, 第一階段選課結果email通知 = 2, 第二階段選課結果email通知 = 3, 第一階段退選提醒文字 = 4, 第二階段退選提醒文字 = 5, 休學相關說明 = 6, 教師成績輸入說明 = 7, 上傳成績提醒 = 8 };

        /// <summary>
        /// 名稱
        /// <list type="string" caption="寄信人">email_sender</list>
        /// <list type="string" caption="選課最終確認說明文字">cs_final_message</list>
        /// <list type="string" caption="第一階段選課結果email通知">email_content1_template</list>
        /// <list type="string" caption="第二階段選課結果email通知">email_content2_template</list>
        /// <list type="string" caption="第一階段退選提醒文字">cs_cancel1_content_template</list>
        /// <list type="string" caption="第二階段退選提醒文字">cs_cancel2_content_template</list>
        /// <list type="string" caption="休學相關說明">core_dropout_content_template</list>
        /// <list type="string" caption="教師成績輸入說明">teacher_score_input_explanation_template</list>
        /// <list type="string" caption="上傳成績提醒">teacher_score_upload_reminder_template</list>
        /// </summary>
        [Field(Field = "conf_name", Indexed = true, Caption = "名稱")]
        public string Name { get; set; }

        /// <summary>
        /// 內容
        /// </summary>
        [Field(Field = "conf_content", Indexed = false, Caption = "內容")]
        public string Content { get; set; }

        public static UDT.CSConfiguration GetTemplate(TemplateName key)
        {
            List<string> keys = new List<string>() { "email_sender", "cs_final_message", "email_content1_template", "email_content2_template", "cs_cancel1_content_template", "cs_cancel2_content_template", "core_dropout_content_template", "teacher_score_input_explanation_template", "teacher_score_upload_reminder_template" };

            return getConf(keys.ElementAt((int)key), "");
        }

        public static UDT.CSConfiguration GetTemplate(string key)
        {
            return getConf(key, "");
        }

        private static UDT.CSConfiguration getConf(string key, string defaultValue)
        {
            AccessHelper ah = new AccessHelper();

            List<UDT.CSConfiguration> configs = ah.Select<UDT.CSConfiguration>(string.Format("conf_name='{0}'", key.ToString()));

            if (configs.Count < 1)
            {
                UDT.CSConfiguration conf = new CSConfiguration();
                conf.Name = key.ToString();
                conf.Content = defaultValue;
                List<ActiveRecord> recs = new List<ActiveRecord>();
                recs.Add(conf);
                ah.SaveAll(recs);
                configs = ah.Select<UDT.CSConfiguration>(string.Format("conf_name='{0}'", key));
            }

            return configs[0];
        }

        /// <summary>
        /// 淺層複製自己
        /// </summary>
        /// <returns></returns>
        public CSConfiguration Clone()
        {
            return (this.MemberwiseClone() as CSConfiguration);
        }
    }
}