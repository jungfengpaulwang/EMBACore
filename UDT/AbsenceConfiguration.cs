using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FISCA.UDT;

namespace EMBACore.UDT
{
    /// <summary>
    /// 缺曠設定
    /// </summary>
    [FISCA.UDT.TableName("ischool.emba.absence_configuration")]
    public class AbsenceConfiguration : ActiveRecord
    {
        internal static void RaiseAfterUpdateEvent(object sender, ParameterEventArgs e)
        {
            if (AbsenceConfiguration.AfterUpdate != null)
                AbsenceConfiguration.AfterUpdate(sender, e);
        }

        internal static event EventHandler<ParameterEventArgs> AfterUpdate;

        /// <summary>
        /// 名稱
        /// </summary>
        [EMBACore.UDT.Field(Field = "conf_name", Indexed = false, Caption = "名稱")]
        public string Name { get; set; }

        /// <summary>
        /// 內容
        /// </summary>
        [EMBACore.UDT.Field(Field = "conf_content", Indexed = false, Caption = "內容")]
        public string Content { get; set; }

        public static UDT.AbsenceConfiguration GetEmailContentTemplate()
        {

            string key = "email_content_template";
            return getConf(key, "親愛的[[Name]]您好：");
        }

        public static UDT.AbsenceConfiguration GetEmailSenderInfo()
        {
            string key = "email_sender";
            return getConf(key, "");
        }

        private static UDT.AbsenceConfiguration getConf(string key, string defaultValue)
        {
            AccessHelper ah = new AccessHelper();

            
            List<UDT.AbsenceConfiguration> configs = ah.Select<UDT.AbsenceConfiguration>(string.Format("conf_name='{0}'", key));

            if (configs.Count < 1)
            {
                UDT.AbsenceConfiguration conf = new AbsenceConfiguration();
                conf.Name = key;
                conf.Content = defaultValue;
                List<ActiveRecord> recs = new List<ActiveRecord>();
                recs.Add(conf);
                ah.SaveAll(recs);
                configs = ah.Select<UDT.AbsenceConfiguration>(string.Format("conf_name='{0}'", key));
            }

            return configs[0];
        }
    }
}
