using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FISCA.UDT;
using FISCA.Presentation;
using FISCA.Permission;
using Campus.Configuration;
using Campus;
using System.Xml;

namespace EMBACore.Initialization
{
    public class OtherInit
    {
        public static void Init()
        {
            //1. Add Detail Buttons
            InitDetailContent();

            //2. Add Ribbon Buttons
            InitRibbonBar();

            //3. Add Search Condition
            InitSearch();

            //4. 主畫面標題
            InitMotherFormTitle();
        }

        public static void InitMotherFormTitle()
        {
            dynamic SchoolBrief = (XmlObject)Config.App["學校資訊"].PreviousData.OuterXml;
            XmlDocument xmlSystemConfig = new XmlDocument();
            xmlSystemConfig.LoadXml(Config.App["系統設定"].PreviousData.OuterXml);
            XmlElement elmSchoolYear = (XmlElement)xmlSystemConfig.DocumentElement.SelectSingleNode("DefaultSchoolYear");
            XmlElement elmSemester = (XmlElement)xmlSystemConfig.DocumentElement.SelectSingleNode("DefaultSemester");

            MotherForm.Form.Text = "ischool<" + SchoolBrief.ChineseName + " " + elmSchoolYear.InnerText + "學年度" + EMBACore.DataItems.SemesterItem.GetSemesterByCode(elmSemester.InnerText) + "><" + FISCA.Authentication.DSAServices.AccessPoint + ">";
        }

        public static void InitDetailContent()
        {

        }

        public static void InitRibbonBar()
        {
            MotherForm.AddPanel(new Forms.SenateTab());

            #region 教務作業

            #region 設定畢業條件

            Catalog button_GraduationRequirement = RoleAclSource.Instance["教務作業"]["功能按鈕"];
            button_GraduationRequirement.Add(new RibbonFeature("Button_GraduationRequirement", "設定畢業條件"));

            var templateManager = MotherForm.RibbonBarItems["教務作業", "基本設定"]["設定"];
            templateManager.Size = RibbonBarButton.MenuButtonSize.Large;
            templateManager.Image = Properties.Resources.sandglass_unlock_64;
            templateManager["設定畢業條件"].Enable = UserAcl.Current["Button_GraduationRequirement"].Executable;
            templateManager["設定畢業條件"].Click += delegate
            {
                (new EMBACore.SetCSIdentity()).ShowDialog();
            };

            #endregion

            #region 設定系所組別

            Catalog button_DepartmentGroup = RoleAclSource.Instance["教務作業"]["功能按鈕"];
            button_DepartmentGroup.Add(new RibbonFeature("Button_DepartmentGroup", "系所組別管理"));

            var vDepartmentGroup = MotherForm.RibbonBarItems["教務作業", "基本設定"]["管理"];
            vDepartmentGroup.Size = RibbonBarButton.MenuButtonSize.Large;
            vDepartmentGroup.Image = Properties.Resources.network_lock_64;
            vDepartmentGroup["系所組別管理"].Enable = UserAcl.Current["Button_DepartmentGroup"].Executable;
            vDepartmentGroup["系所組別管理"].Click += delegate
            {
                (new EMBACore.Forms.DepartmentGroupForm()).ShowDialog();
            };

            #endregion

            #region 經歷內容管理

            Catalog button_ExperienceDataSourceMangagement = RoleAclSource.Instance["教務作業"]["功能按鈕"];
            button_ExperienceDataSourceMangagement.Add(new RibbonFeature("Button_ExperienceDataSourceMangagement", "經歷內容管理"));

            var vExperienceDataSourceMangagement = MotherForm.RibbonBarItems["教務作業", "基本設定"]["管理"];
            //vExperienceDataSourceMangagement.Size = RibbonBarButton.MenuButtonSize.Large;
            //vExperienceDataSourceMangagement.Image = Properties.Resources.network_lock_64;
            vExperienceDataSourceMangagement["經歷內容管理"].Enable = UserAcl.Current["Button_ExperienceDataSourceMangagement"].Executable;
            vExperienceDataSourceMangagement["經歷內容管理"].Click += delegate
            {
                (new EMBACore.Forms.ExperienceDataSourceManagement()).ShowDialog();
            };

            #endregion

            #region 課程類別管理

            Catalog button_CourseTypeDataSourceMangagement = RoleAclSource.Instance["教務作業"]["功能按鈕"];
            button_CourseTypeDataSourceMangagement.Add(new RibbonFeature("Button_DataTypeDataSourceMangagement", "課程類別管理"));

            var vCourseTypeDataSourceMangagement = MotherForm.RibbonBarItems["教務作業", "基本設定"]["管理"];
            //vExperienceDataSourceMangagement.Size = RibbonBarButton.MenuButtonSize.Large;
            //vExperienceDataSourceMangagement.Image = Properties.Resources.network_lock_64;
            vCourseTypeDataSourceMangagement["課程類別管理"].Enable = UserAcl.Current["Button_DataTypeDataSourceMangagement"].Executable;
            vCourseTypeDataSourceMangagement["課程類別管理"].Click += delegate
            {
                (new EMBACore.Forms.CourseTypeDataSourceManagement()).ShowDialog();
            };

            #endregion

            #endregion
        }

        public static void InitSearch()
        {

        }
    }
}