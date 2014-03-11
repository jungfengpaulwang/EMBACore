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
using FISCA.UDT;

namespace EMBACore.Forms
{
    public partial class ScoreInputSemesterSetting : BaseForm
    {
        XmlDocument xmlSystemConfig;
        XmlElement elmSchoolYear;
        XmlElement elmSemester;

        Log.LogAgent log;

        public ScoreInputSemesterSetting()
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

            List<UDT.ScoreInputSemester> ScoreInputSemesters = (new AccessHelper()).Select<UDT.ScoreInputSemester>();
            if (ScoreInputSemesters.Count == 0)
            {
                this.nudSchoolYear.Value = int.Parse(elmSchoolYear.InnerText);
                this.cboSemester.SelectedItem = SemesterItem.GetSemesterByCode(elmSemester.InnerText);
            }
            else
            {
                this.nudSchoolYear.Value = decimal.Parse(ScoreInputSemesters.ElementAt(0).SchoolYear.ToString());
                this.cboSemester.SelectedItem = SemesterItem.GetSemesterByCode(ScoreInputSemesters.ElementAt(0).Semester.ToString());
            }

            /* Log */
            log = new Log.LogAgent();
            addLog();            
        }

        private void addLog()
        {
            log.SetLogValue("成績輸入學年度", this.nudSchoolYear.Value.ToString());
            log.SetLogValue("成績輸入學期", (this.cboSemester.SelectedItem == null) ? "" : this.cboSemester.SelectedItem.ToString());
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            if (this.cboSemester.SelectedItem == null)
            {
                Util.ShowMsg("請選擇學期。", "警告");
                return;
            }

            try
            {
                List<UDT.ScoreInputSemester> ScoreInputSemesters = (new AccessHelper()).Select<UDT.ScoreInputSemester>();
                ScoreInputSemesters.ForEach(x => x.Deleted = true);
                ScoreInputSemesters.SaveAll();

                UDT.ScoreInputSemester ScoreInputSemester = new UDT.ScoreInputSemester();
                ScoreInputSemester.SchoolYear = int.Parse(this.nudSchoolYear.Value.ToString());
                ScoreInputSemester.Semester = int.Parse(((SemesterItem)(this.cboSemester.SelectedItem)).Value);
                ScoreInputSemester.Save();
                Util.ShowMsg("儲存成功。", "提示");
            }
            catch (Exception ex)
            {
                Util.ShowMsg(ex.Message, "錯誤");
            }

            try
            {
                this.addLog();
                this.log.Save("設定成績輸入學年期", "", "", Log.LogTargetCategory.SystemSetting, "");
            }
            catch (Exception ex)
            {
                Util.ShowMsg(ex.Message, "錯誤");
            }

            this.Close();
        }        
    }
}
