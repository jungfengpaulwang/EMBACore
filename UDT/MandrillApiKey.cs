using System;
using FISCA.UDT;

namespace EMBACore.UDT
{
    /// <summary>
    /// ManDrill APIKey
    /// </summary>
    [FISCA.UDT.TableName("ischool.emba.mandrill_apikey")]
    public class MandrillAPIKey : ActiveRecord
    {
        /// <summary>
        /// API Key
        /// </summary>
        [Field(Field = "apikey", Indexed = false)]
        public string APIKey { get; set; }

        /// <summary>
        /// 淺層複製物件
        /// </summary>
        /// <returns></returns>
        public MandrillAPIKey Clone()
        {
            return this.MemberwiseClone() as MandrillAPIKey;
        }
    }
}
