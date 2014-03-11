using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using FISCA.Presentation.Controls;
using Campus;
using Campus.Configuration;
using FISCA.DSAClient;
using System.Xml;
using EMBACore.DataItems;

namespace EMBACore.Forms
{
    public partial class SemesterSetting : BaseForm
    {
        XmlDocument xmlSystemConfig;
        XmlElement elmSchoolYear;
        XmlElement elmSemester;

        Log.LogAgent log;

        public SemesterSetting()
        {
            InitializeComponent();
        }

        private void SemesterInfo_Load(object sender, EventArgs e)
        {
            this.cboSemester.Items.Clear();

            foreach(SemesterItem semester in SemesterItem.GetSemesterList())
                this.cboSemester.Items.Add(semester);

            xmlSystemConfig = new XmlDocument();
            xmlSystemConfig.LoadXml(Config.App["系統設定"].PreviousData.OuterXml);
            elmSchoolYear = (XmlElement)xmlSystemConfig.DocumentElement.SelectSingleNode("DefaultSchoolYear");
            elmSemester = (XmlElement)xmlSystemConfig.DocumentElement.SelectSingleNode("DefaultSemester");

            this.nudSchoolYear.Value = int.Parse(elmSchoolYear.InnerText);
            this.cboSemester.SelectedItem = SemesterItem.GetSemesterByCode(elmSemester.InnerText);

            /* Log */
            log = new Log.LogAgent();
            addLog();            
        }

        private void addLog()
        {
            log.SetLogValue("學年度", this.nudSchoolYear.Value.ToString());
            log.SetLogValue("學期", (this.cboSemester.SelectedItem == null) ? "" : this.cboSemester.SelectedItem.ToString());
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            if (this.cboSemester.SelectedItem == null)
            {
                Util.ShowMsg("請選擇學期！", "注意！");
                return;
            }

            try
            {
                elmSchoolYear.InnerText = this.nudSchoolYear.Value.ToString();
                elmSemester.InnerText = ((SemesterItem)this.cboSemester.SelectedItem).Value;

                ConfigData cd = Config.App["系統設定"];
                cd.PreviousData = K12.Data.XmlHelper.LoadXml(xmlSystemConfig.OuterXml);
                cd.Save();

                //  主畫面標題(包含學年度與學期，故學年度與學期變更時要重新設定主畫面標題)
                EMBACore.Initialization.OtherInit.InitMotherFormTitle();
            }
            catch (Exception ex)
            {
                Util.ShowMsg("儲存學年度學期時發生錯誤！", "注意！");
            }

            try
            {
                this.addLog();
                this.log.Save("設定學年度學期", "", "", Log.LogTargetCategory.SystemSetting, "");
            }
            catch (Exception ex)
            {
                Util.ShowMsg("儲存系統日置時發生錯誤！", "注意！");
            }

            this.Close();
        }        
    }
}
