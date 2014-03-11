using EMBA;
using EMBACore.Import;
using EMBA.DocumentValidator;

namespace EMBACore.Import
{
    public class RowValidatorFactory : IRowValidatorFactory
    {
        #region IRowValidatorFactory 成員

        /// <summary>
        /// 根據 typeName 建立對應的RowValidator
        /// </summary>
        /// <param name="typeName"></param>
        /// <param name="validatorDescription"></param>
        /// <returns></returns>
        public IRowVaildator CreateRowValidator(string typeName, System.Xml.XmlElement validatorDescription)
        {
            switch (typeName.ToUpper())
            {
                case "UNIQUE_STUDENTIDNUMBER_STATUS":
                    return new Unique_StudentIDNumber_Status();
                default:
                    return null;
            }
        }
        #endregion
    }
}
