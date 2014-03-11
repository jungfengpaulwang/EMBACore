using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FISCA.Presentation.Controls;
using FISCA.Presentation;
using FISCA.Permission;
using System.Windows.Forms;
using FISCA.Data;
using K12.Presentation;
using EMBACore.DetailItems;
using System.Data;
using EMBACore.Export;

namespace EMBACore.Initialization
{
    class TeacherInit
    {
        public static void Init()
        {
            //1. Add Detail Buttons
            InitDetailContent();

            //2. Add Ribbon Buttons
            InitRibbonBar();

            //3. Add Search Condition
            InitSearch();          
        }

        public static void InitDetailContent()
        {
            #region 資料項目

            /*  註冊權限  */
            Catalog detail = RoleAclSource.Instance["教師"]["資料項目"];

            detail.Add(new DetailItemFeature(typeof(Teacher_Brief)));
            detail.Add(new DetailItemFeature(typeof(TeacherExtDetail)));

            if (UserAcl.Current[typeof(Teacher_Brief)].Viewable)
                NLDPanels.Teacher.AddDetailBulider<Teacher_Brief>();
            if (UserAcl.Current[typeof(TeacherExtDetail)].Viewable)
                NLDPanels.Teacher.AddDetailBulider<TeacherExtDetail>();
            #endregion
        }

        public static void InitRibbonBar()
        {
            #region RibbonBar 教師/編輯

            //新增教師
            MotherForm.RibbonBarItems["教師", "編輯"]["新增"].Image = Properties.Resources.btnAddTeacher;
            MotherForm.RibbonBarItems["教師", "編輯"]["新增"].Size = RibbonBarButton.MenuButtonSize.Large;

            Catalog button_add = RoleAclSource.Instance["教師"]["功能按鈕"];
            button_add.Add(new RibbonFeature("Teacher_Button_AddTeacher", "新增教師"));
            MotherForm.RibbonBarItems["教師", "編輯"]["新增"].Enable = UserAcl.Current["Teacher_Button_AddTeacher"].Executable;

            MotherForm.RibbonBarItems["教師", "編輯"]["新增"].Click += delegate
            {
                (new Teacher_Add()).ShowDialog();
            };
            //刪除教師
            MotherForm.RibbonBarItems["教師", "編輯"]["刪除"].Image = Properties.Resources.btnDeleteTeacher;
            MotherForm.RibbonBarItems["教師", "編輯"]["刪除"].Size = RibbonBarButton.MenuButtonSize.Large;

            Catalog button_delete = RoleAclSource.Instance["教師"]["功能按鈕"];
            button_delete.Add(new RibbonFeature("Teacher_Button_DeleteTeacher", "刪除教師"));
            MotherForm.RibbonBarItems["教師", "編輯"]["刪除"].Enable = UserAcl.Current["Teacher_Button_DeleteTeacher"].Executable;

            MotherForm.RibbonBarItems["教師", "編輯"]["刪除"].Click += delegate
            {
                if (K12.Presentation.NLDPanels.Teacher.SelectedSource.Count == 1)
                {
                    K12.Data.TeacherRecord deleteRec = K12.Data.Teacher.SelectByID(K12.Presentation.NLDPanels.Teacher.SelectedSource[0]);
                    deleteRec.Status = K12.Data.TeacherRecord.TeacherStatus.刪除;

                    string msg = string.Format("確定要刪除「{0}」？", deleteRec.Name);
                    if (FISCA.Presentation.Controls.MsgBox.Show(msg, "刪除教師", MessageBoxButtons.YesNo) == DialogResult.Yes)
                    {
                        K12.Data.Teacher.Update(deleteRec);
                        //Teacher.Instance.SyncDataBackground(deleteRec.ID);
                        //PermRecLogProcess prlp = new PermRecLogProcess();
                        //prlp.SaveLog("學籍.教師", "刪除教師", "刪除教師,姓名:" + deleteRec.Name);
                    }
                    else
                        return;
                }
                else
                    MsgBox.Show("僅能單筆刪除！");
            };

            #endregion

            #region 匯入

                #region  教師基本資料
                MotherForm.RibbonBarItems["教師", "資料統計"]["匯入"].Image = Properties.Resources.Import_Image;
                MotherForm.RibbonBarItems["教師", "資料統計"]["匯入"].Size = RibbonBarButton.MenuButtonSize.Large;

                Catalog button_importTeacher = RoleAclSource.Instance["教師"]["功能按鈕"];
                button_importTeacher.Add(new RibbonFeature("Teacher_Button_ImportTeacher", "匯入教師基本資料"));
                MotherForm.RibbonBarItems["教師", "資料統計"]["匯入"]["匯入教師基本資料"].Enable = UserAcl.Current["Teacher_Button_ImportTeacher"].Executable;

                MotherForm.RibbonBarItems["教師", "資料統計"]["匯入"]["匯入教師基本資料"].Click += delegate
                {
                    (new Import.Teacher_Import()).Execute();
                };
                #endregion

            #endregion

            #region 匯出

            #region  教師基本資料
            MotherForm.RibbonBarItems["教師", "資料統計"]["匯出"].Image = Properties.Resources.Export_Image;
            MotherForm.RibbonBarItems["教師", "資料統計"]["匯出"].Size = RibbonBarButton.MenuButtonSize.Large;

            Catalog button_exportTeacher = RoleAclSource.Instance["教師"]["功能按鈕"];
            button_exportTeacher.Add(new RibbonFeature("Teacher_Button_ExportTeacher", "匯出教師基本資料"));
            MotherForm.RibbonBarItems["教師", "資料統計"]["匯出"]["匯出教師基本資料"].Enable = UserAcl.Current["Teacher_Button_ExportTeacher"].Executable;

            MotherForm.RibbonBarItems["教師", "資料統計"]["匯出"]["匯出教師基本資料"].Click += delegate
            {
                if (K12.Presentation.NLDPanels.Teacher.SelectedSource.Count == 0)
                    FISCA.Presentation.Controls.MsgBox.Show("請先選取教師！", "錯誤", MessageBoxButtons.OK, MessageBoxIcon.Error);
                else
                    (new Teacher_Brief_Export_Excel()).Execute();
            };
            #endregion

            #endregion

            #region 查詢

                #region 曾指導過的學生及其論文

                MotherForm.RibbonBarItems["教師", "資料統計"]["查詢"].Image = Properties.Resources.searchHistory;
                MotherForm.RibbonBarItems["教師", "資料統計"]["查詢"].Size = RibbonBarButton.MenuButtonSize.Large;

                Catalog button_queryStudentPaperByAdvisor = RoleAclSource.Instance["教師"]["功能按鈕"];
                button_queryStudentPaperByAdvisor.Add(new RibbonFeature("Teacher_Button_QueryStudentPaperByAdvisor", "查詢教師曾指導過的學生及其論文"));
                MotherForm.RibbonBarItems["教師", "資料統計"]["查詢"]["曾指導過的學生及其論文"].Enable = UserAcl.Current["Teacher_Button_QueryStudentPaperByAdvisor"].Executable;

                MotherForm.RibbonBarItems["教師", "資料統計"]["查詢"]["曾指導過的學生及其論文"].Click += delegate
                {
                    (new Forms.QueryAdvisor()).ShowDialog();
                };

                #endregion

            #endregion
        }

        /// <summary>
        /// 設定教師查詢條件及行為
        /// </summary>
        public static void InitSearch()
        {
            //設定可查詢欄位：教師姓名、教師編號、所屬單位、帳號
            K12.Presentation.NLDPanels.Teacher.SearchConditionMenu["教師姓名"].Checked = true;
            K12.Presentation.NLDPanels.Teacher.SearchConditionMenu["教師編號"].Checked = true;
            K12.Presentation.NLDPanels.Teacher.SearchConditionMenu["所屬單位"].Checked = true;
            K12.Presentation.NLDPanels.Teacher.SearchConditionMenu["帳號"].Checked = true;
            K12.Presentation.NLDPanels.Teacher.SearchConditionMenu["教師姓名"].Click += new EventHandler(Util_Click);
            K12.Presentation.NLDPanels.Teacher.SearchConditionMenu["教師編號"].Click += new EventHandler(Util_Click);
            K12.Presentation.NLDPanels.Teacher.SearchConditionMenu["所屬單位"].Click += new EventHandler(Util_Click);
            K12.Presentation.NLDPanels.Teacher.SearchConditionMenu["帳號"].Click += new EventHandler(Util_Click);
            //設定開始查詢的行為
            K12.Presentation.NLDPanels.Teacher.Search += new EventHandler<SearchEventArgs>(Teacher_Search);
        }

        static void Util_Click(object sender, EventArgs e)
        {
            FISCA.Presentation.MenuButton btn = (FISCA.Presentation.MenuButton)sender;
            btn.Checked = !btn.Checked;
        }

        static void Teacher_Search(object sender, SearchEventArgs e)
        {
            foreach (FISCA.Presentation.MenuButton btnSearch in K12.Presentation.NLDPanels.Teacher.SearchConditionMenu.SubItems)
            {
                if (btnSearch.Checked)
                {
                    DataTable dt = FindByTeacherQuery(btnSearch.Text, e);

                    if (dt.Rows.Count > 0)
                        dt.Rows.Cast<DataRow>().ToList().ForEach((x) => 
                        {
                            if (!e.Result.Contains(x["teacher_id"] + ""))
                                e.Result.Add(x["teacher_id"] + "");
                        });
                }
            }
        }

        static DataTable FindByTeacherQuery(string fieldName, FISCA.Presentation.SearchEventArgs e)
        {
            string strQuery = string.Empty;

            if (fieldName == "教師姓名")
                strQuery = string.Format("select teacher.id as teacher_id from teacher where teacher_name like '%{0}%'", e.Condition);
            if (fieldName == "所屬單位")
                strQuery = string.Format("select ref_teacher_id as teacher_id from $ischool.emba.teacher_ext where major_work_place like '%{0}%'", e.Condition);
            if (fieldName == "教師編號")
                strQuery = string.Format("select ref_teacher_id as teacher_id from $ischool.emba.teacher_ext where employee_no like '%{0}%'", e.Condition);
            if (fieldName == "帳號")
                strQuery = string.Format("select id as teacher_id from teacher where st_login_name like '%{0}%'", e.Condition);

            return (new QueryHelper()).Select(strQuery);
        }
    }
}
