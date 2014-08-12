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
using FISCA.UDT;
using System.Reflection;
using System.Data;
using EMBACore.Import;
using EMBACore.UDT;
using EMBACore.Export;
using Campus.Windows;
using System.Threading.Tasks;

namespace EMBACore.Initialization
{
    class CourseInit
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

            //NLDPanels.Course.RegisterDetailContent<Course_BasicInfo>();
            //NLDPanels.Course.RegisterDetailContent<Course_Instructor>();
            //NLDPanels.Course.RegisterDetailContent<Course_SCAttend>();

            //NLDPanels.Course.AddDetailBulider<DetailItems.Course_BasicInfo>();
            //NLDPanels.Course.AddDetailBulider<DetailItems.Course_Instructor>();
            //NLDPanels.Course.AddDetailBulider<Course_SCAttend>();

            /*  註冊權限  */
            
            Catalog detail = RoleAclSource.Instance["課程"]["資料項目"];

            detail.Add(new DetailItemFeature(typeof(Course_BasicInfo)));
            detail.Add(new DetailItemFeature(typeof(Course_Instructor)));
            detail.Add(new DetailItemFeature(typeof(Course_SCAttend)));

            if (UserAcl.Current[typeof(Course_BasicInfo)].Viewable)
                NLDPanels.Course.AddDetailBulider<Course_BasicInfo>();
            if (UserAcl.Current[typeof(Course_Instructor)].Viewable)
                NLDPanels.Course.AddDetailBulider<Course_Instructor>();
            if (UserAcl.Current[typeof(Course_SCAttend)].Viewable)
                NLDPanels.Course.AddDetailBulider<Course_SCAttend>();
            #endregion
            
            #region 匯入
            MotherForm.RibbonBarItems["課程", "資料統計"]["匯入"].Image = Properties.Resources.Import_Image;
            MotherForm.RibbonBarItems["課程", "資料統計"]["匯入"].Size = RibbonBarButton.MenuButtonSize.Large;
            
            #region  課程總檔

            Catalog button_importSubject = RoleAclSource.Instance["課程"]["功能按鈕"];
            button_importSubject.Add(new RibbonFeature("Course_Button_ImportSubject", "匯入課程總檔"));
            MotherForm.RibbonBarItems["課程", "資料統計"]["匯入"]["課程相關匯入"]["匯入課程總檔"].Enable = UserAcl.Current["Course_Button_ImportSubject"].Executable;
            MotherForm.RibbonBarItems["課程", "資料統計"]["匯入"]["課程相關匯入"]["匯入課程總檔"].Click += delegate
            {
                new Subject_Import().Execute();
            };
            #endregion

            #region  課程基本資料

            Catalog button_importCourse = RoleAclSource.Instance["課程"]["功能按鈕"];
            button_importCourse.Add(new RibbonFeature("Course_Button_ImportCourse", "匯入課程基本資料"));
            MotherForm.RibbonBarItems["課程", "資料統計"]["匯入"]["課程相關匯入"]["匯入課程基本資料"].Enable = UserAcl.Current["Course_Button_ImportCourse"].Executable;
            MotherForm.RibbonBarItems["課程", "資料統計"]["匯入"]["課程相關匯入"]["匯入課程基本資料"].Click += delegate
            {
                new Course_Import().Execute();
            };
            #endregion

            #region  修課學生

            Catalog button_importSCAttend = RoleAclSource.Instance["課程"]["功能按鈕"];
            button_importSCAttend.Add(new RibbonFeature("Course_Button_ImportSCAttend", "匯入修課學生"));
            MotherForm.RibbonBarItems["課程", "資料統計"]["匯入"]["課程相關匯入"]["匯入修課學生"].Enable = UserAcl.Current["Course_Button_ImportSCAttend"].Executable;
            MotherForm.RibbonBarItems["課程", "資料統計"]["匯入"]["課程相關匯入"]["匯入修課學生"].Click += delegate
            {
                new SCAttendExt_Import().Execute();
            };
            #endregion

            #endregion

            #region 匯出

            #region  課程基本資料(DBF 格式)
            MotherForm.RibbonBarItems["課程", "資料統計"]["匯出"].Image = Properties.Resources.Export_Image;
            MotherForm.RibbonBarItems["課程", "資料統計"]["匯出"].Size = RibbonBarButton.MenuButtonSize.Large;

            Catalog button_exportCourse_DBF = RoleAclSource.Instance["課程"]["功能按鈕"];
            button_exportCourse_DBF.Add(new RibbonFeature("Course_Button_ExportCourse_DBF", "匯出課程檔(DBF 格式)"));
            MotherForm.RibbonBarItems["課程", "資料統計"]["匯出"]["課程相關匯出"]["匯出課程檔(DBF 格式)"].Enable = UserAcl.Current["Course_Button_ExportCourse_DBF"].Executable;

            MotherForm.RibbonBarItems["課程", "資料統計"]["匯出"]["課程相關匯出"]["匯出課程檔(DBF 格式)"].Click += delegate
            {
                if (K12.Presentation.NLDPanels.Course.SelectedSource.Count == 0)
                    FISCA.Presentation.Controls.MsgBox.Show("請先選取課程！", "錯誤", MessageBoxButtons.OK, MessageBoxIcon.Error);
                else
                    (new Course_Brief_Export_DBF()).Execute();
            };
            #endregion

            #region  課程檔(Exel 格式)

            Catalog button_exportCourse_Excel_1 = RoleAclSource.Instance["課程"]["功能按鈕"];
            button_exportCourse_Excel_1.Add(new RibbonFeature("Course_Button_ExportCourse_Excel_1", "匯出課程檔(Excel 格式)"));
            MotherForm.RibbonBarItems["課程", "資料統計"]["匯出"]["課程相關匯出"]["匯出課程檔(Excel 格式)"].Enable = UserAcl.Current["Course_Button_ExportCourse_Excel_1"].Executable;

            MotherForm.RibbonBarItems["課程", "資料統計"]["匯出"]["課程相關匯出"]["匯出課程檔(Excel 格式)"].Click += delegate
            {
                if (K12.Presentation.NLDPanels.Course.SelectedSource.Count == 0)
                    FISCA.Presentation.Controls.MsgBox.Show("請先選取課程！", "錯誤", MessageBoxButtons.OK, MessageBoxIcon.Error);
                else
                    (new Course_Brief_Export_Excel_1()).Execute();
            };
            #endregion

            #region  課程基本資料(Excel 格式)
            Catalog button_exportCourse_Excel = RoleAclSource.Instance["課程"]["功能按鈕"];
            button_exportCourse_Excel.Add(new RibbonFeature("Course_Button_ExportCourse_Excel", "匯出課程基本資料"));
            MotherForm.RibbonBarItems["課程", "資料統計"]["匯出"]["課程相關匯出"]["匯出課程基本資料"].Enable = UserAcl.Current["Course_Button_ExportCourse_Excel"].Executable;

            MotherForm.RibbonBarItems["課程", "資料統計"]["匯出"]["課程相關匯出"]["匯出課程基本資料"].Click += delegate
            {
                if (K12.Presentation.NLDPanels.Course.SelectedSource.Count == 0)
                    FISCA.Presentation.Controls.MsgBox.Show("請先選取課程！", "錯誤", MessageBoxButtons.OK, MessageBoxIcon.Error);
                else
                    (new Course_Brief_Export_Excel()).Execute();
            };
            #endregion

            #region 課程總檔

            Catalog button_exportSubject = RoleAclSource.Instance["課程"]["功能按鈕"];
            button_exportSubject.Add(new RibbonFeature("Course_Button_ExportSubject", "匯出課程總檔"));
            MotherForm.RibbonBarItems["課程", "資料統計"]["匯出"]["課程相關匯出"]["匯出課程總檔"].Enable = UserAcl.Current["Course_Button_ExportSubject"].Executable;

            MotherForm.RibbonBarItems["課程", "資料統計"]["匯出"]["課程相關匯出"]["匯出課程總檔"].Click += delegate
            {
                (new Subject_Export_Excel()).Execute();
            };



            #endregion

            #region 課程成績(以「課程識別碼」或「課號」比對)

            Catalog button_exportSubjectSemesterScore = RoleAclSource.Instance["課程"]["功能按鈕"];
            button_exportSubjectSemesterScore.Add(new RibbonFeature("Course_Button_ExportSubjectSemesterScoreExcel", "匯出學期成績"));
            MotherForm.RibbonBarItems["課程", "資料統計"]["匯出"]["成績相關匯出"]["匯出學期成績"].Enable = UserAcl.Current["Course_Button_ExportSubjectSemesterScoreExcel"].Executable;

            MotherForm.RibbonBarItems["課程", "資料統計"]["匯出"]["成績相關匯出"]["匯出學期成績"].Click += delegate
            {
                if (K12.Presentation.NLDPanels.Course.SelectedSource.Count == 0)
                    FISCA.Presentation.Controls.MsgBox.Show("請先選取課程！", "錯誤", MessageBoxButtons.OK, MessageBoxIcon.Error);
                else
                    (new Course_Score_Export_Excel()).ShowDialog();
            };

            #endregion

            #region 課程學期成績DBF

            Catalog button_exportCourseSubjectSemesterScore = RoleAclSource.Instance["課程"]["功能按鈕"];
            button_exportCourseSubjectSemesterScore.Add(new RibbonFeature("Course_Button_ExportSubjectSemesterScore", "匯出學期成績(DBF 格式)"));
            MotherForm.RibbonBarItems["課程", "資料統計"]["匯出"]["成績相關匯出"]["匯出學期成績(DBF 格式)"].Enable = UserAcl.Current["Course_Button_ExportSubjectSemesterScore"].Executable;

            MotherForm.RibbonBarItems["課程", "資料統計"]["匯出"]["成績相關匯出"]["匯出學期成績(DBF 格式)"].Click += delegate
            {
                if (K12.Presentation.NLDPanels.Course.SelectedSource.Count == 0)
                    FISCA.Presentation.Controls.MsgBox.Show("請先選取課程！", "錯誤", MessageBoxButtons.OK, MessageBoxIcon.Error);
                else
                    (new Subject_Score_Export_DBF("COURSE")).ShowDialog();
            };

            #endregion

            #region 修課記錄(Excel)

            Catalog button_exportSCAttend = RoleAclSource.Instance["課程"]["功能按鈕"];
            button_exportSCAttend.Add(new RibbonFeature("Course_Button_ExportSCAttend", "匯出修課記錄"));
            MotherForm.RibbonBarItems["課程", "資料統計"]["匯出"]["課程相關匯出"]["匯出修課記錄"].Enable = UserAcl.Current["Course_Button_ExportSCAttend"].Executable;

            MotherForm.RibbonBarItems["課程", "資料統計"]["匯出"]["課程相關匯出"]["匯出修課記錄"].Click += delegate
            {
                if (K12.Presentation.NLDPanels.Course.SelectedSource.Count == 0)
                    FISCA.Presentation.Controls.MsgBox.Show("請先選取課程！", "錯誤", MessageBoxButtons.OK, MessageBoxIcon.Error);
                else
                    (new SCAttend_Export_Excel("COURSE")).ShowDialog();
            };

            #endregion

            #region 修課記錄(DBF)

            Catalog button_exportSCAttend_DBF = RoleAclSource.Instance["課程"]["功能按鈕"];
            button_exportSCAttend_DBF.Add(new RibbonFeature("Course_Button_ExportSCAttend_DBF", "匯出修課記錄(DBF 格式)"));
            MotherForm.RibbonBarItems["課程", "資料統計"]["匯出"]["課程相關匯出"]["匯出修課記錄(DBF 格式)"].Enable = UserAcl.Current["Course_Button_ExportSCAttend_DBF"].Executable;

            MotherForm.RibbonBarItems["課程", "資料統計"]["匯出"]["課程相關匯出"]["匯出修課記錄(DBF 格式)"].Click += delegate
            {
                if (K12.Presentation.NLDPanels.Course.SelectedSource.Count == 0)
                    FISCA.Presentation.Controls.MsgBox.Show("請先選取課程！", "錯誤", MessageBoxButtons.OK, MessageBoxIcon.Error);
                else
                    (new SCAttend_Export_DBF("COURSE")).ShowDialog();
            };

            #endregion

            #region 選課名單

            MotherForm.RibbonBarItems["課程", "資料統計"]["報表"].Image = Properties.Resources.paste_64;
            MotherForm.RibbonBarItems["課程", "資料統計"]["報表"].Size = RibbonBarButton.MenuButtonSize.Large;

            Catalog button_PrintCourseList = RoleAclSource.Instance["課程"]["功能按鈕"];
            button_PrintCourseList.Add(new RibbonFeature("Course_Button_PrintCourseList", "列印選課名單"));
            MotherForm.RibbonBarItems["課程", "資料統計"]["報表"]["課程相關報表"]["列印選課名單"].Enable = UserAcl.Current["Course_Button_PrintCourseList"].Executable;

            MotherForm.RibbonBarItems["課程", "資料統計"]["報表"]["課程相關報表"]["列印選課名單"].Click += delegate
            {
                //if (K12.Presentation.NLDPanels.Course.SelectedSource.Count == 0)
                //    FISCA.Presentation.Controls.MsgBox.Show("請先選取課程！", "錯誤", MessageBoxButtons.OK, MessageBoxIcon.Error);
                //else
                //    (new EMBA.Print.Student_CourseList()).ShowDialog();
                (new EMBA.Print.Course_Student_List(K12.Presentation.NLDPanels.Course.SelectedSource)).Execute();
            };

            #endregion

            #region 點名單

            Catalog button_PrintRollCall = RoleAclSource.Instance["課程"]["功能按鈕"];
            button_PrintRollCall.Add(new RibbonFeature("Course_Button_PrintRollCall", "列印點名單"));
            MotherForm.RibbonBarItems["課程", "資料統計"]["報表"]["課程相關報表"]["列印點名單"].Enable = UserAcl.Current["Course_Button_PrintRollCall"].Executable;

            MotherForm.RibbonBarItems["課程", "資料統計"]["報表"]["課程相關報表"]["列印點名單"].Click += delegate
            {
                if (K12.Presentation.NLDPanels.Course.SelectedSource.Count == 0)
                    FISCA.Presentation.Controls.MsgBox.Show("請先選取課程！", "錯誤", MessageBoxButtons.OK, MessageBoxIcon.Error);
                else
                    (new EMBA.Print.Course_RollCallReport()).ShowDialog();
            };

            #endregion

            #endregion

            #region 課程>設定>成績輸入學年期
            Catalog button_OpeningInfoSetting = RoleAclSource.Instance["課程"]["功能按鈕"];
            button_OpeningInfoSetting.Add(new RibbonFeature("Button_ScoreInputSemesterSetting", "設定成績輸入學年期"));

            var templateManager = MotherForm.RibbonBarItems["課程", "設定"];
            //templateManager.Size = RibbonBarButton.MenuButtonSize.Large;
            //templateManager.Image = Properties.Resources.sandglass_unlock_64;
            templateManager["成績輸入學年期"].Enable = UserAcl.Current["Button_ScoreInputSemesterSetting"].Executable;
            templateManager["成績輸入學年期"].Click += delegate
            {
                (new Forms.ScoreInputSemesterSetting()).ShowDialog();
            };
            #endregion
            
            #region 指定畢業條件

            MotherForm.RibbonBarItems["課程", "指定"]["成績輸入規則"].Image = Properties.Resources.calc_save_64;
            MotherForm.RibbonBarItems["課程", "指定"]["成績輸入規則"].Size = RibbonBarButton.MenuButtonSize.Large;

            Catalog button_assignGraduationRequirement = RoleAclSource.Instance["課程"]["功能按鈕"];
            button_assignGraduationRequirement.Add(new RibbonFeature("Course_Button_AssignSubjectSemesterScoreInputRule", "成績輸入規則"));
            MotherForm.RibbonBarItems["課程", "指定"]["成績輸入規則"].Enable = UserAcl.Current["Course_Button_AssignSubjectSemesterScoreInputRule"].Executable;

            MotherForm.RibbonBarItems["課程", "指定"]["成績輸入規則"].SupposeHasChildern = true;
            MotherForm.RibbonBarItems["課程", "指定"]["成績輸入規則"].PopupOpen += new EventHandler<PopupOpenEventArgs>(CourseInit_PopupOpen);

            #endregion
        }

        static void CourseInit_PopupOpen(object sender, PopupOpenEventArgs e)
        {
            //MenuButton mb = e.VirtualButtons["不指定"];
            e.VirtualButtons["不指定"].Click += delegate
            {
                List<string> selectedIDs = K12.Presentation.NLDPanels.Course.SelectedSource;
                if (selectedIDs.Count == 0)
                {
                    MessageBox.Show("請先選擇課程！");
                    return;
                }
                Task task = Task.Factory.StartNew(() =>
                {
                    List<UDT.SubjectSemesterScoreInputRule> SubjectSemesterScoreInputRules = (new AccessHelper()).Select<UDT.SubjectSemesterScoreInputRule>(string.Format("ref_course_id in ({0})", string.Join(",", selectedIDs)));
                    if (SubjectSemesterScoreInputRules.Count > 0)
                    {
                        SubjectSemesterScoreInputRules.ForEach(x => x.Deleted = true);
                        SubjectSemesterScoreInputRules.SaveAll();
                    }
                });
                task.ContinueWith((x) =>
                {
                    ListPaneFields.Course_SubjectSemesterScoreInputRule.Instance.ReloadMe();

                }, TaskScheduler.FromCurrentSynchronizationContext());
            };
            e.VirtualButtons["P/N 制"].Tag = 1;
            e.VirtualButtons["P/N 制"].BeginGroup = true;
            e.VirtualButtons["P/N 制"].Click += (s1, e1) =>
            {
                List<string> selectedIDs = K12.Presentation.NLDPanels.Course.SelectedSource;
                if (selectedIDs.Count == 0)
                {
                    MessageBox.Show("請先選擇課程！");
                    return;
                }
                Task task = Task.Factory.StartNew(() =>
                {
                    List<UDT.SubjectSemesterScoreInputRule> SubjectSemesterScoreInputRules = (new AccessHelper()).Select<UDT.SubjectSemesterScoreInputRule>(string.Format("ref_course_id in ({0})", string.Join(",", selectedIDs)));
                    Dictionary<string, UDT.SubjectSemesterScoreInputRule> dicSubjectSemesterScoreInputRules = new Dictionary<string, SubjectSemesterScoreInputRule>();
                    if (SubjectSemesterScoreInputRules.Count > 0)
                        dicSubjectSemesterScoreInputRules = SubjectSemesterScoreInputRules.ToDictionary(x => x.CourseID.ToString());

                    List<UDT.SubjectSemesterScoreInputRule> Insert_SubjectSemesterScoreInputRules = new List<SubjectSemesterScoreInputRule>();
                    foreach (string CourseID in selectedIDs)
                    {
                        if (!dicSubjectSemesterScoreInputRules.ContainsKey(CourseID))
                        {
                            UDT.SubjectSemesterScoreInputRule SubjectSemesterScoreInputRule = new UDT.SubjectSemesterScoreInputRule();
                            SubjectSemesterScoreInputRule.CourseID = int.Parse(CourseID);
                            SubjectSemesterScoreInputRule.InputRule = 1;
                            Insert_SubjectSemesterScoreInputRules.Add(SubjectSemesterScoreInputRule);
                        }
                    }
                    Insert_SubjectSemesterScoreInputRules.SaveAll();
                });
                task.ContinueWith((x) =>
                {
                    ListPaneFields.Course_SubjectSemesterScoreInputRule.Instance.ReloadMe();
                }, TaskScheduler.FromCurrentSynchronizationContext());
            };
        }

        static void SetButtonPermission(RibbonBarButton btn, string featureCode, string categoryName)         
        {
            Catalog cat = RoleAclSource.Instance["課程"]["功能按鈕"];
            cat.Add(new RibbonFeature(featureCode, categoryName));
            btn.Enable = UserAcl.Current[featureCode].Executable;
        }

        public static void InitRibbonBar()
        {
            #region RibbonBar 課程/編輯

            //新增課程
            MotherForm.RibbonBarItems["課程", "編輯"]["新增"].Image = Properties.Resources.btnAddCourse;
            MotherForm.RibbonBarItems["課程", "編輯"]["新增"].Size = RibbonBarButton.MenuButtonSize.Large;

            Catalog button_add = RoleAclSource.Instance["課程"]["功能按鈕"];
            button_add.Add(new RibbonFeature("Course_Button_AddCourse", "新增課程"));
            MotherForm.RibbonBarItems["課程", "編輯"]["新增"].Enable = UserAcl.Current["Course_Button_AddCourse"].Executable;

            MotherForm.RibbonBarItems["課程", "編輯"]["新增"].Click += delegate
            {
                (new Course_Add()).ShowDialog();
            };
            //刪除課程
            MotherForm.RibbonBarItems["課程", "編輯"]["刪除"].Image = Properties.Resources.btnDeleteCourse;
            MotherForm.RibbonBarItems["課程", "編輯"]["刪除"].Size = RibbonBarButton.MenuButtonSize.Large;

            Catalog button_delete = RoleAclSource.Instance["課程"]["功能按鈕"];
            button_delete.Add(new RibbonFeature("Course_Button_DeleteCourse", "刪除課程"));
            MotherForm.RibbonBarItems["課程", "編輯"]["刪除"].Enable = UserAcl.Current["Course_Button_DeleteCourse"].Executable;

            MotherForm.RibbonBarItems["課程", "編輯"]["刪除"].Click += delegate
            {
                if (K12.Presentation.NLDPanels.Course.SelectedSource.Count == 1)
                {
                    List<K12.Data.StudentRecord> selectedStudents = new List<K12.Data.StudentRecord>();
                    K12.Data.CourseRecord deleteRec = K12.Data.Course.SelectByID(K12.Presentation.NLDPanels.Course.SelectedSource[0]);
                    
                    AccessHelper Access = new AccessHelper();
                    List<SCAttendExt> scattendExts = Access.Select<SCAttendExt>(string.Format("ref_course_id = '{0}'", deleteRec.ID));
                    if (scattendExts != null && scattendExts.Count>0)
                        selectedStudents = K12.Data.Student.SelectByIDs(scattendExts.Select(x => x.StudentID.ToString()));
                    int attendStudentCount_一般 = 0;
                    int attendStudentCount_休學 = 0;

                    if (selectedStudents.Count > 0)
                    {
                        attendStudentCount_一般 = selectedStudents.Where(x => x.Status == K12.Data.StudentRecord.StudentStatus.一般).Count();
                        attendStudentCount_休學 = selectedStudents.Where(x => x.Status == K12.Data.StudentRecord.StudentStatus.休學).Count();
                    }

                    if (attendStudentCount_一般 > 0 || attendStudentCount_休學 > 0)
                    {
                        FISCA.Presentation.Controls.MsgBox.Show(deleteRec.Name + " 有" + (attendStudentCount_一般 + attendStudentCount_休學) + "位修課學生，請先移除修課學生後再刪除課程.");
                        return;
                    }
                    else
                    {
                        string msg = string.Format("確定要刪除「{0}」？", deleteRec.Name);
                        if (FISCA.Presentation.Controls.MsgBox.Show(msg, "刪除課程", MessageBoxButtons.YesNo) == DialogResult.Yes)
                        {
                            if (scattendExts != null && scattendExts.Count > 0)
                            {
                                scattendExts.ForEach(x => x.Deleted = true);
                                scattendExts.SaveAll();
                            }
                            if (selectedStudents.Count() > 0)
                            {
                                List<K12.Data.SCETakeRecord> sceList = K12.Data.SCETake.SelectByStudentAndCourse(selectedStudents.Select(x=>x.ID), new List<string>() { deleteRec.ID });
                                K12.Data.SCETake.Delete(sceList);
                            }

                            K12.Data.Course.Delete(deleteRec);
                            //CourseRecordEditor crd = Course.Instance.Items[record.ID].GetEditor();
                            //crd.Remove = true;
                            //crd.Save();
                            // 加這主要是重新整理
                            //EMBACore.Filters.Course.SyncDataBackground(deleteRec.ID);
                            //Filters.FilterService.Course.Setup();
                        }
                    }
                }
                else
                    FISCA.Presentation.Controls.MsgBox.Show("僅能單筆刪除！");
            };

            //MotherForm.RibbonBarItems["課程", "編輯"]["匯入"].Click += delegate
            //{
            //    (new Forms.ImportCourse_And_SCAttend()).ShowDialog();
            //};

            /*  設定目前學年度學期 */
            RibbonBarButton btnSemester = K12.Presentation.NLDPanels.Course.RibbonBarItems["設定"]["學期設定"];
            SetButtonPermission(btnSemester, "ischool.EMBA.Course_Button_SetSemester", "學期設定");
            btnSemester.Click += delegate
            {
                (new Forms.SemesterSetting()).ShowDialog();
            };
            /*  管理科目 課程>設定>課程總檔*/
            RibbonBarButton btnSubject = K12.Presentation.NLDPanels.Course.RibbonBarItems["設定"]["課程總檔"];
            SetButtonPermission(btnSubject, "ischool.EMBA.Course_Button_ManageSubject", "課程總檔");
            btnSubject.Click += delegate
            {
                (new Forms.SubjectForm()).ShowDialog();
            };
            /*  課程管理 */
            RibbonBarButton btnCourseMangagement = K12.Presentation.NLDPanels.Course.RibbonBarItems["設定"]["開課"];
            SetButtonPermission(btnCourseMangagement, "ischool.EMBA.Course_Button_CourseManagement", "開課");
            btnCourseMangagement.Click += delegate
            {
                (new Forms.GenerateCourse()).ShowDialog();
            };

            /* 管理學期成績 */
            RibbonBarButton btnScore = K12.Presentation.NLDPanels.Course.RibbonBarItems["成績"]["學期成績"];
            SetButtonPermission(btnScore, "ischool.EMBA.Course_Button_SubjectScore", "學期成績");
            btnScore.Click += delegate
            {
                (new Forms.SubjectScore()).ShowDialog();
            };

            RibbonBarButton btnScoreLock = K12.Presentation.NLDPanels.Course.RibbonBarItems["成績"]["鎖定/開放成績輸入"];
            SetButtonPermission(btnScoreLock, "ischool.EMBA.Course_Button_SubjectScoreForm", "鎖定/開放成績輸入");
            btnScoreLock.Click += delegate
            {
                (new Forms.SubjectScoreLockForm()).ShowDialog();
            };

            /* 管理座位表 */
            RibbonBarButton btnSeatTable = K12.Presentation.NLDPanels.Course.RibbonBarItems["缺課紀錄"]["管理座位表"];
            SetButtonPermission(btnSeatTable, "ischool.EMBA.Course_Button_ManageSeatTable", "管理座位表");
            btnSeatTable.Click += delegate
            {
                (new Forms.SeatTable.frmSeatTable()).ShowDialog();
            };

            /* 管理缺曠紀錄 */
            RibbonBarButton btnAbsence = K12.Presentation.NLDPanels.Course.RibbonBarItems["缺課紀錄"]["缺課登錄暨 Email 通知"];
            SetButtonPermission(btnAbsence, "ischool.EMBA.Course_Button_ManageAbsence", "缺課登錄暨 Email 通知");
            btnAbsence.Click += delegate
            {
                (new Forms.CourseAttendance()).ShowDialog();
            };

            /* 管理缺曠紀錄 Email 通知 */
            //RibbonBarButton btnAbsenceEmail = K12.Presentation.NLDPanels.Course.RibbonBarItems["缺課紀錄"]["Email 通知"];
            //SetButtonPermission(btnAbsenceEmail, "ischool.EMBA.Course_Button_ManageAbsenceEmail", "缺曠紀錄Email 通知");
            //btnAbsenceEmail.Click += delegate
            //{
            //    (new Forms.CourseAttendance_EmailNotification()).ShowDialog();
            //};

            /* 管理 Email 寄件者帳號 */
            //RibbonBarButton btnEmailSender = K12.Presentation.NLDPanels.Course.RibbonBarItems["缺課紀錄"]["管理 Email寄件者"];
            //SetButtonPermission(btnEmailSender, "ischool.EMBA.Course_Button_ManageEmailSender", "管理 Email 寄件者");
            //btnEmailSender.Click += delegate
            //{
            //    (new Forms.Email_Sender()).ShowDialog();
            //};

            /* 管理 Email 內容樣版 */
            //RibbonBarButton btnEmailTemplate = K12.Presentation.NLDPanels.Course.RibbonBarItems["缺課紀錄"]["管理 Email內容樣版"];
            //SetButtonPermission(btnEmailTemplate, "ischool.EMBA.Course_Button_ManageEmailTemplate", "管理 Email 內容樣版");
            //btnEmailTemplate.Click += delegate
            //{
            //    (new Forms.Email_Content_Template()).ShowDialog();
            //};

            /* 管理 教師成績輸入說明 文字樣版 */
            RibbonBarButton btnScoreInputTemplate = K12.Presentation.NLDPanels.Course.RibbonBarItems["成績"]["管理教師成績輸入說明樣版"];
            SetButtonPermission(btnScoreInputTemplate, "ischool.EMBA.Course_Button_TeacherScoreInputTemplate", "管理教師成績輸入說明樣版");
            btnScoreInputTemplate.Click += delegate
            {
                (new EMBACore.Forms.CS_Template_NoSubject(CSConfiguration.TemplateName.教師成績輸入說明)).ShowDialog();
            };

            /* 管理 上傳成績提醒 文字樣版 */
            RibbonBarButton btnScoreUploadTemplate = K12.Presentation.NLDPanels.Course.RibbonBarItems["成績"]["管理上傳成績提醒樣版"];
            SetButtonPermission(btnScoreUploadTemplate, "ischool.EMBA.Course_Button_TeacherScoreUploadReminderTemplate", "管理上傳成績提醒樣版");
            btnScoreUploadTemplate.Click += delegate
            {
                (new EMBACore.Forms.CS_Template_NoSubject(CSConfiguration.TemplateName.上傳成績提醒)).ShowDialog();
            };
            #endregion

            #region 課程>缺課紀錄>管理缺課通知內容樣版
            RoleAclSource.Instance["課程"]["功能按鈕"].Add(new RibbonFeature("ischool.EMBA.Course_Button_ManageEmailTemplate", "管理缺課通知內容樣版"));

            MotherForm.RibbonBarItems["課程", "缺課紀錄"]["設定"].Size = RibbonBarButton.MenuButtonSize.Large;
            MotherForm.RibbonBarItems["課程", "缺課紀錄"]["設定"].Image = Properties.Resources.sandglass_unlock_64;

            var templateManager = MotherForm.RibbonBarItems["課程", "缺課紀錄"]["設定"]["樣版設定"];
            templateManager["管理缺課通知內容樣版"].Enable = UserAcl.Current["ischool.EMBA.Course_Button_ManageEmailTemplate"].Executable;
            templateManager["管理缺課通知內容樣版"].Click += delegate
            {
                (new Forms.CS_Template_CourseAttendance()).ShowDialog();
            };
            #endregion

            #region 課程>缺課紀錄>管理「寄發勾選日期之缺補課記錄」內容樣版
            RoleAclSource.Instance["課程"]["功能按鈕"].Add(new RibbonFeature("ischool.EMBA.Course_Button_ManageEmailTemplate_ByDate", "管理「寄發勾選日期之缺補課記錄」內容樣版"));

            var templateManager_ByDate = MotherForm.RibbonBarItems["課程", "缺課紀錄"]["設定"]["樣版設定"];
            templateManager_ByDate["管理「寄發勾選日期之缺補課記錄」內容樣版"].Enable = UserAcl.Current["ischool.EMBA.Course_Button_ManageEmailTemplate_ByDate"].Executable;
            templateManager_ByDate["管理「寄發勾選日期之缺補課記錄」內容樣版"].Click += delegate
            {
                (new Forms.CS_Template_CourseAttendance_ByDate()).ShowDialog();
            };
            #endregion

            #region 課程>缺課紀錄>查詢缺課紀錄
            RoleAclSource.Instance["課程"]["功能按鈕"].Add(new RibbonFeature("ischool.EMBA.Course_QueryCourseAttendance_EmailNotification", "查詢缺課紀錄"));

            var btnQueryAbsenceRecord = MotherForm.RibbonBarItems["課程", "缺課紀錄"];
            btnQueryAbsenceRecord["查詢缺課紀錄"].Enable = UserAcl.Current["ischool.EMBA.Course_QueryCourseAttendance_EmailNotification"].Executable;
            btnQueryAbsenceRecord["查詢缺課紀錄"].Click += delegate
            {
                (new Forms.frmQueryCourseAttendance_EmailNotification()).ShowDialog();
            };
            #endregion

            #region 課程>成績>未確認並上傳成績教師偵測
            RoleAclSource.Instance["課程"]["功能按鈕"].Add(new RibbonFeature("ischool.EMBA.Course_QueryNotYetUploadSubjectSemesterScoreTeacherDetector", "偵測未確認並上傳成績教師"));

            var btnNotYetUploadSubjectSemesterScoreTeacherDetector = MotherForm.RibbonBarItems["課程", "成績"];
            btnNotYetUploadSubjectSemesterScoreTeacherDetector["未確認並上傳成績教師偵測"].Enable = UserAcl.Current["ischool.EMBA.Course_QueryNotYetUploadSubjectSemesterScoreTeacherDetector"].Executable;
            btnNotYetUploadSubjectSemesterScoreTeacherDetector["未確認並上傳成績教師偵測"].Click += delegate
            {
                (new Forms.NotYetUploadSubjectSemesterScoreTeacherDetector()).ShowDialog();
            };
            #endregion

        }

        /// <summary>
        /// 設定課程查詢條件及行為
        /// </summary>
        public static void InitSearch()
        {
            //設定可查詢欄位
            K12.Presentation.NLDPanels.Course.SearchConditionMenu["課程名稱"].Checked = true;
            K12.Presentation.NLDPanels.Course.SearchConditionMenu["課程識別碼"].Checked = true;
            K12.Presentation.NLDPanels.Course.SearchConditionMenu["課號"].Checked = true;
            K12.Presentation.NLDPanels.Course.SearchConditionMenu["課程名稱"].Click += new EventHandler(Util_Click);
            K12.Presentation.NLDPanels.Course.SearchConditionMenu["課程識別碼"].Click += new EventHandler(Util_Click);
            K12.Presentation.NLDPanels.Course.SearchConditionMenu["課號"].Click += new EventHandler(Util_Click);
            //設定開始查詢的行為
            K12.Presentation.NLDPanels.Course.Search += new EventHandler<SearchEventArgs>(Course_Search);
        }

        static void Util_Click(object sender, EventArgs e)
        {
            FISCA.Presentation.MenuButton btn = (FISCA.Presentation.MenuButton)sender;
            btn.Checked = !btn.Checked;
        }

        static void Course_Search(object sender, SearchEventArgs e)
        {
            foreach (FISCA.Presentation.MenuButton btnSearch in K12.Presentation.NLDPanels.Course.SearchConditionMenu.SubItems)
            {
                if (btnSearch.Checked)
                {
                    DataTable dt = FindByCourseQuery(btnSearch.Text, e);

                    if (dt.Rows.Count > 0)
                        dt.Rows.Cast<DataRow>().ToList().ForEach((x) =>
                        {
                            if (!e.Result.Contains(x["course_id"] + ""))
                                e.Result.Add(x["course_id"] + "");
                        });
                }
            }
        }

        static DataTable FindByCourseQuery(string fieldName, FISCA.Presentation.SearchEventArgs e)
        {
            string strQuery = string.Empty;

            if (fieldName == "課程名稱")
                strQuery = string.Format("select course.id as course_id from course where course_name like '%{0}%'", e.Condition);
            if (fieldName == "課程識別碼")
                strQuery = string.Format("select ref_course_id as course_id from $ischool.emba.course_ext where subject_code like '%{0}%'", e.Condition);
            if (fieldName == "課號")
                strQuery = string.Format("select ref_course_id as course_id from $ischool.emba.course_ext where new_subject_code like '%{0}%'", e.Condition);

            return (new QueryHelper()).Select(strQuery);
        }
    }
}
