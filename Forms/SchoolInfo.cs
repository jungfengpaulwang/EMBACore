using System;
using Campus.Configuration;
using FISCA;
using FISCA.Presentation.Controls;
using Campus;
using K12.Data;

namespace EMBACore.Forms
{
    public partial class SchoolInfo : BaseForm
    {
        dynamic SchoolBrief = null;
        dynamic SemesterDefault = null;

        public SchoolInfo()
        {
            InitializeComponent();
        }

        private void SchoolInfoMangement_Load(object sender, EventArgs e)
        {
            try
            {
                SchoolBrief = (XmlObject)Config.App["學校資訊"].PreviousData.OuterXml;
                SemesterDefault = (XmlObject)Config.App["系統設定"].PreviousData.OuterXml;

                txtCode.Text = SchoolBrief.Code;
                txtChineseName.Text = SchoolBrief.ChineseName;
                txtEnglishName.Text = SchoolBrief.EnglishName;
                txtAddress.Text = SchoolBrief.Address;
                txtEnglishAddress.Text = SchoolBrief.EnglishAddress;
                txtTelephone.Text = SchoolBrief.Telephone;
                txtFax.Text = SchoolBrief.Fax;
                txtChancellorChineseName.Text = SchoolBrief.ChancellorChineseName;
                txtChancellorEnglishName.Text = SchoolBrief.ChancellorEnglishName;
                txtEduDirectorName.Text = SchoolBrief.EduDirectorName;
                txtStuDirectorName.Text = SchoolBrief.StuDirectorName;
                txtCEO.Text = SchoolBrief.CEO;

                intSchoolYear.Value = SemesterDefault.DefaultSchoolYear.IntVal();
                intSemester.Value = SemesterDefault.DefaultSemester.IntVal();
            }
            catch (Exception ex)
            {
                RTOut.WriteError(ex);
            }
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            SchoolBrief.Code = txtCode.Text.Trim();
            SchoolBrief.ChineseName = txtChineseName.Text.Trim();
            SchoolBrief.EnglishName = txtEnglishName.Text.Trim();
            SchoolBrief.Address = txtAddress.Text.Trim();
            SchoolBrief.EnglishAddress = txtEnglishAddress.Text.Trim();
            SchoolBrief.Telephone = txtTelephone.Text.Trim();
            SchoolBrief.Fax = txtFax.Text.Trim();
            SchoolBrief.ChancellorChineseName = txtChancellorChineseName.Text.Trim();
            SchoolBrief.ChancellorEnglishName = txtChancellorEnglishName.Text.Trim();
            SchoolBrief.EduDirectorName = txtEduDirectorName.Text.Trim();
            SchoolBrief.StuDirectorName = txtStuDirectorName.Text.Trim();
            SchoolBrief.CEO = txtCEO.Text.Trim();

            SemesterDefault.DefaultSchoolYear = intSchoolYear.Value;
            SemesterDefault.DefaultSemester = intSemester.Value;

            ConfigData cd = Config.App["學校資訊"];
            cd.PreviousData = XmlHelper.LoadXml(SchoolBrief.ToString());
            cd.Save();

            cd = Config.App["系統設定"];
            cd.PreviousData = XmlHelper.LoadXml(SemesterDefault.ToString());
            try
            {
                cd.Save();
                MsgBox.Show("資料儲存成功！");
            }
            catch (Exception ex)
            {
                MsgBox.Show(ex.Message);
            }
            finally
            {
                this.Close();
            }
        }

        private void btnExit_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }

}
