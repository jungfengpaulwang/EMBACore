using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using FISCA.Presentation.Controls;
using FISCA.Presentation;
using FISCA.Permission;
using System.Windows.Forms;
using FISCA.Data;
using K12.Presentation;
using EMBACore.DetailItems;
using FISCA.UDT;
using System.Reflection;
using EMBACore.UDT;
using EMBACore.Import;
using EMBACore.Export;
using Campus.Windows;

namespace EMBACore.Initialization
{
    public class StudentInit
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
            //DetailContentImproved
            NLDPanels.Student.RegisterDetailContent<Student_Brief2>();
            NLDPanels.Student.RegisterDetailContent<Student_Class>();
            NLDPanels.Student.RegisterDetailContent<Student_Experience_Ext>();
            NLDPanels.Student.RegisterDetailContent<Student_EducationBackground>();
            NLDPanels.Student.RegisterDetailContent<Student_Email>();
            NLDPanels.Student.RegisterDetailContent<Student_AddDropCourse>();
            NLDPanels.Student.RegisterDetailContent<Student_SubjectScore>();
            NLDPanels.Student.RegisterDetailContent<Student_DoNotTakeCourseInSummer>();
            NLDPanels.Student.RegisterDetailContent<Student_Remark>();
            NLDPanels.Student.RegisterDetailContent<Student_UpdateRecord>();
            
            /*  註冊權限  */
            Catalog detail = RoleAclSource.Instance["學生"]["資料項目"];

            detail.Add(new DetailItemFeature(typeof(Student_Brief)));
            detail.Add(new DetailItemFeature(typeof(Student_Address)));
            detail.Add(new DetailItemFeature(typeof(Student_Parent)));
            detail.Add(new DetailItemFeature(typeof(Student_Phone)));
            detail.Add(new DetailItemFeature(typeof(Student_PaymentHistory)));
            detail.Add(new DetailItemFeature(typeof(Student_Paper)));

            if (UserAcl.Current[typeof(Student_Brief)].Viewable)
                NLDPanels.Student.AddDetailBulider<Student_Brief>();
            if (UserAcl.Current[typeof(Student_Address)].Viewable)
                NLDPanels.Student.AddDetailBulider<Student_Address>();
            if (UserAcl.Current[typeof(Student_Parent)].Viewable)
                NLDPanels.Student.AddDetailBulider<Student_Parent>();
            if (UserAcl.Current[typeof(Student_Phone)].Viewable)
                NLDPanels.Student.AddDetailBulider<Student_Phone>();
            if (UserAcl.Current[typeof(Student_PaymentHistory)].Viewable)
                NLDPanels.Student.AddDetailBulider<Student_PaymentHistory>();
            if (UserAcl.Current[typeof(Student_Paper)].Viewable)
                NLDPanels.Student.AddDetailBulider<Student_Paper>();

            #endregion
        }

        public static void InitRibbonBar()
        {
            #region RibbonBar 學生/編輯

            Catalog button_addStudent = RoleAclSource.Instance["學生"]["功能按鈕"];
            button_addStudent.Add(new RibbonFeature("Student_Button_ApproveAddDropCourseForm", "核准加退選"));
            MotherForm.RibbonBarItems["學生", "加退選"]["核准"].Enable = UserAcl.Current["Student_Button_ApproveAddDropCourseForm"].Executable;
            MotherForm.RibbonBarItems["學生", "加退選"]["核准"].Click += delegate
            {
                (new Forms.ApproveAddDropCourseForm()).ShowDialog();
            };        

            //新增學生
            MotherForm.RibbonBarItems["學生", "編輯"]["新增"].Image = Properties.Resources.btnaddstudent_image;
            MotherForm.RibbonBarItems["學生", "編輯"]["新增"].Size = RibbonBarButton.MenuButtonSize.Large;

            button_addStudent = RoleAclSource.Instance["學生"]["功能按鈕"];
            button_addStudent.Add(new RibbonFeature("Student_Button_AddStudent", "新增學生"));
            MotherForm.RibbonBarItems["學生", "編輯"]["新增"].Enable = UserAcl.Current["Student_Button_AddStudent"].Executable;

            MotherForm.RibbonBarItems["學生", "編輯"]["新增"].Click += delegate
            {
                (new Student_Add()).ShowDialog();
            };
            //刪除學生
            MotherForm.RibbonBarItems["學生", "編輯"]["刪除"].Image = Properties.Resources.btndeletestudent_image;
            MotherForm.RibbonBarItems["學生", "編輯"]["刪除"].Size = RibbonBarButton.MenuButtonSize.Large;

            Catalog button_deleteStudent = RoleAclSource.Instance["學生"]["功能按鈕"];
            button_deleteStudent.Add(new RibbonFeature("Student_Button_DeleteStudent", "刪除學生"));
            MotherForm.RibbonBarItems["學生", "編輯"]["刪除"].Enable = UserAcl.Current["Student_Button_DeleteStudent"].Executable;

            MotherForm.RibbonBarItems["學生", "編輯"]["刪除"].Click += delegate
            {
                if (K12.Presentation.NLDPanels.Student.SelectedSource.Count == 1)
                {
                    K12.Data.StudentRecord studRec = K12.Data.Student.SelectByID(K12.Presentation.NLDPanels.Student.SelectedSource[0]);
                    string msg = string.Format("確定要刪除「{0}」？", studRec.Name);
                    if (FISCA.Presentation.Controls.MsgBox.Show(msg, "刪除學生", MessageBoxButtons.YesNo) == DialogResult.Yes)
                    {
                        QueryHelper queryHelper = new QueryHelper();
                        string strQuery = String.Format(@"Select id, name, status, id_number, student_number From Student 
Where status=256 and (id_number='{0}' or student_number='{1}')", studRec.IDNumber, studRec.StudentNumber);

                        DataTable dataTable = queryHelper.Select(strQuery);

                        if (dataTable == null || dataTable.Rows.Count == 0)
                        {
                            // 修改學生狀態 delete
                            studRec.Status = K12.Data.StudentRecord.StudentStatus.刪除;
                            K12.Data.Student.Update(studRec);
                            //  寫入 log 待處理
                            //PermRecLogProcess prlp = new PermRecLogProcess();
                            //Student.Instance.SyncDataBackground(studRec.ID);
                            //prlp.SaveLog("學籍學生", "刪除學生", "刪除學生，姓名:" + studRec.Name + ",學號:" + studRec.StudentNumber);
                        }
                        else
                        {
                            string mm = string.Empty;

                            foreach (DataRow row in dataTable.Rows)
                            {
                                mm += "學生姓名：" + (row["name"] == null ? "" : row["name"].ToString()) + "、學號：" + (row["student_number"] == null ? "" : row["student_number"].ToString()) + "、身分證號：" + (row["id_number"] == null ? "" : row["id_number"].ToString()) + "\n";
                            }

                            Campus.Windows.MsgBox.Show("狀態為「刪除」的學生已有相同的學號或身分證號，請先修改後再刪除：\n" + mm);
                        }
                    }
                }
                else
                    Campus.Windows.MsgBox.Show("僅能單筆刪除！");



            };
            #endregion

            #region 學生>設定>樣版設定>休學相關說明
            RoleAclSource.Instance["學生"]["功能按鈕"].Add(new RibbonFeature("Button_core_dropout_content_template", "休學相關說明"));

            var templateManager = MotherForm.RibbonBarItems["學生", "設定"]["樣版"];
            templateManager.Size = RibbonBarButton.MenuButtonSize.Large;
            templateManager.Image = Properties.Resources.sandglass_unlock_64;
            templateManager["休學相關說明"].Enable = UserAcl.Current["Button_core_dropout_content_template"].Executable;
            templateManager["休學相關說明"].Click += delegate
            {
                (new Forms.CS_Template(UDT.CSConfiguration.TemplateName.休學相關說明)).ShowDialog();
            };
            #endregion

            #region 指定畢業條件

            MotherForm.RibbonBarItems["學生", "指定"]["畢業條件"].Image = Properties.Resources.calc_save_64;
            MotherForm.RibbonBarItems["學生", "指定"]["畢業條件"].Size = RibbonBarButton.MenuButtonSize.Large;

            Catalog button_assignGraduationRequirement = RoleAclSource.Instance["學生"]["功能按鈕"];
            button_assignGraduationRequirement.Add(new RibbonFeature("Student_Button_AssignGraduationRequirement", "指定畢業條件"));
            MotherForm.RibbonBarItems["學生", "指定"]["畢業條件"].Enable = UserAcl.Current["Student_Button_AssignGraduationRequirement"].Executable;

            MotherForm.RibbonBarItems["學生", "指定"]["畢業條件"].SupposeHasChildern = true;
            MotherForm.RibbonBarItems["學生", "指定"]["畢業條件"].PopupOpen += new EventHandler<PopupOpenEventArgs>(StudentInit_PopupOpen);

            #endregion

            #region 匯入
            MotherForm.RibbonBarItems["學生", "資料統計"]["匯入"].Image = Properties.Resources.Import_Image;
            MotherForm.RibbonBarItems["學生", "資料統計"]["匯入"].Size = RibbonBarButton.MenuButtonSize.Large;

                #region  學生基本資料

            Catalog button_importStudent = RoleAclSource.Instance["學生"]["功能按鈕"];
            button_importStudent.Add(new RibbonFeature("Student_Button_ImportStudent", "匯入學生基本資料"));
            MotherForm.RibbonBarItems["學生", "資料統計"]["匯入"]["學籍相關匯入"]["匯入學生基本資料"].Enable = UserAcl.Current["Student_Button_ImportStudent"].Executable;

            MotherForm.RibbonBarItems["學生", "資料統計"]["匯入"]["學籍相關匯入"]["匯入學生基本資料"].Click += delegate
            {
                new Student_Brief_Import().Execute();
            };
                #endregion

                #region  學生照片

                Catalog button_importStudentPhoto = RoleAclSource.Instance["學生"]["功能按鈕"];
                button_importStudentPhoto.Add(new RibbonFeature("Student_Button_ImportStudentPhoto", "匯入學生照片"));
                MotherForm.RibbonBarItems["學生", "資料統計"]["匯入"]["學籍相關匯入"]["匯入學生照片"].Enable = UserAcl.Current["Student_Button_ImportStudentPhoto"].Executable;

                MotherForm.RibbonBarItems["學生", "資料統計"]["匯入"]["學籍相關匯入"]["匯入學生照片"].Click += delegate
                {
                    (new EMBA.Photo.PhotosBatchImportForm()).ShowDialog();
                };
                #endregion

                #region  學生繳費記錄

                Catalog button_Student_Payment_History_Import = RoleAclSource.Instance["學生"]["功能按鈕"];
                button_Student_Payment_History_Import.Add(new RibbonFeature("Student_Button_ImportPaymentHistory", "匯入學生繳費記錄"));
                MotherForm.RibbonBarItems["學生", "資料統計"]["匯入"]["其它相關匯入"]["匯入學生繳費記錄"].Enable = UserAcl.Current["Student_Button_ImportPaymentHistory"].Executable;

                MotherForm.RibbonBarItems["學生", "資料統計"]["匯入"]["其它相關匯入"]["匯入學生繳費記錄"].Click += delegate
                {
                    (new Student_Payment_History_Import()).Execute();
                };
                #endregion

                #region  學生論文及指導教授

                Catalog button_Student_Paper_Import = RoleAclSource.Instance["學生"]["功能按鈕"];
                button_Student_Paper_Import.Add(new RibbonFeature("Student_Button_ImportPaper", "匯入學生論文及指導教授"));
                MotherForm.RibbonBarItems["學生", "資料統計"]["匯入"]["其它相關匯入"]["匯入學生論文及指導教授"].Enable = UserAcl.Current["Student_Button_ImportPaper"].Executable;

                MotherForm.RibbonBarItems["學生", "資料統計"]["匯入"]["其它相關匯入"]["匯入學生論文及指導教授"].Click += delegate
                {
                    (new Student_Paper_Import()).Execute();
                };
                #endregion

                #region  課程學期成績

                Catalog button_SubjectSemesterScore_Import = RoleAclSource.Instance["學生"]["功能按鈕"];
                button_SubjectSemesterScore_Import.Add(new RibbonFeature("Student_Button_ImportSubjectSemesterScore", "匯入課程學期成績"));
                MotherForm.RibbonBarItems["學生", "資料統計"]["匯入"]["成績相關匯入"]["匯入課程學期成績"].Enable = UserAcl.Current["Student_Button_ImportSubjectSemesterScore"].Executable;

                MotherForm.RibbonBarItems["學生", "資料統計"]["匯入"]["成績相關匯入"]["匯入課程學期成績"].Click += delegate
                {
                    (new SubjectSemesterScore_Import()).Execute();
                };
                #endregion

                #region  經歷(公關室)

                Catalog button_Experience_Import = RoleAclSource.Instance["學生"]["功能按鈕"];
                button_Experience_Import.Add(new RibbonFeature("Student_Button_Experience_Import", "匯入經歷"));
                MotherForm.RibbonBarItems["學生", "資料統計"]["匯入"]["學籍相關匯入"]["匯入學生經歷"].Enable = UserAcl.Current["Student_Button_Experience_Import"].Executable;

                MotherForm.RibbonBarItems["學生", "資料統計"]["匯入"]["學籍相關匯入"]["匯入學生經歷"].Click += delegate
                {
                    (new Student_Experience_Import()).Execute();
                };
                #endregion

            #endregion
            
            #region 匯出

                #region 學生基本資料

            MotherForm.RibbonBarItems["學生", "資料統計"]["匯出"].Image = Properties.Resources.Export_Image;
            MotherForm.RibbonBarItems["學生", "資料統計"]["匯出"].Size = RibbonBarButton.MenuButtonSize.Large;                

            Catalog button_exportStudent = RoleAclSource.Instance["學生"]["功能按鈕"];
            button_exportStudent.Add(new RibbonFeature("Student_Button_ExportStudent", "匯出學生基本資料"));
            MotherForm.RibbonBarItems["學生", "資料統計"]["匯出"]["學籍相關匯出"]["匯出學生基本資料"].Enable = UserAcl.Current["Student_Button_ExportStudent"].Executable;

            MotherForm.RibbonBarItems["學生", "資料統計"]["匯出"]["學籍相關匯出"]["匯出學生基本資料"].Click += delegate
            {
                if (K12.Presentation.NLDPanels.Student.SelectedSource.Count == 0)
                    FISCA.Presentation.Controls.MsgBox.Show("請先選取學生！", "錯誤", MessageBoxButtons.OK, MessageBoxIcon.Error);
                else
                    (new Student_Photo_Export_Excel(K12.Presentation.NLDPanels.Student.SelectedSource)).ShowDialog();
            };

                #endregion

                #region 學生照片

            Catalog button_exportStudentPhoto = RoleAclSource.Instance["學生"]["功能按鈕"];
            button_exportStudentPhoto.Add(new RibbonFeature("Student_Button_ExportStudentPhoto", "匯出學生照片"));
            MotherForm.RibbonBarItems["學生", "資料統計"]["匯出"]["學籍相關匯出"]["匯出學生照片"].Enable = UserAcl.Current["Student_Button_ExportStudentPhoto"].Executable;

            MotherForm.RibbonBarItems["學生", "資料統計"]["匯出"]["學籍相關匯出"]["匯出學生照片"].Click += delegate
            {
                if (K12.Presentation.NLDPanels.Student.SelectedSource.Count == 0)
                    FISCA.Presentation.Controls.MsgBox.Show("請先選取學生！", "錯誤", MessageBoxButtons.OK, MessageBoxIcon.Error);
                else
                    (new EMBA.Photo.PhotosBatchExportForm()).ShowDialog();
            };

                #endregion

                #region 課程學期成績DBF

                Catalog button_exportSubjectSemesterScore = RoleAclSource.Instance["學生"]["功能按鈕"];
                button_exportSubjectSemesterScore.Add(new RibbonFeature("Student_Button_ExportSubjectSemesterScore", "匯出學期成績(DBF 格式)"));
                MotherForm.RibbonBarItems["學生", "資料統計"]["匯出"]["成績相關匯出"]["匯出學期成績(DBF 格式)"].Enable = UserAcl.Current["Student_Button_ExportSubjectSemesterScore"].Executable;

                MotherForm.RibbonBarItems["學生", "資料統計"]["匯出"]["成績相關匯出"]["匯出學期成績(DBF 格式)"].Click += delegate
                {
                    if (K12.Presentation.NLDPanels.Student.SelectedSource.Count == 0)
                        FISCA.Presentation.Controls.MsgBox.Show("請先選取學生！", "錯誤", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    else
                        (new Subject_Score_Export_DBF("STUDENT")).ShowDialog();
                };

                #endregion

                #region 課程學期成績Excel

                Catalog button_exportSubjectSemesterScoreExcel = RoleAclSource.Instance["學生"]["功能按鈕"];
                button_exportSubjectSemesterScoreExcel.Add(new RibbonFeature("Student_Button_ExportSubjectSemesterScoreExcel", "匯出學期成績"));
                MotherForm.RibbonBarItems["學生", "資料統計"]["匯出"]["成績相關匯出"]["匯出學期成績"].Enable = UserAcl.Current["Student_Button_ExportSubjectSemesterScoreExcel"].Executable;

                MotherForm.RibbonBarItems["學生", "資料統計"]["匯出"]["成績相關匯出"]["匯出學期成績"].Click += delegate
                {
                    if (K12.Presentation.NLDPanels.Student.SelectedSource.Count == 0)
                        FISCA.Presentation.Controls.MsgBox.Show("請先選取學生！", "錯誤", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    else
                        (new Subject_Score_Export_Excel()).ShowDialog();
                };

                #endregion

                #region 學生繳費記錄

                Catalog button_paymentHistory = RoleAclSource.Instance["學生"]["功能按鈕"];
                button_paymentHistory.Add(new RibbonFeature("Student_Button_PaymentHistory", "匯出學生繳費記錄"));
                MotherForm.RibbonBarItems["學生", "資料統計"]["匯出"]["其它相關匯出"]["匯出學生繳費記錄"].Enable = UserAcl.Current["Student_Button_PaymentHistory"].Executable;

                MotherForm.RibbonBarItems["學生", "資料統計"]["匯出"]["其它相關匯出"]["匯出學生繳費記錄"].Click += delegate
                {
                    if (K12.Presentation.NLDPanels.Student.SelectedSource.Count == 0)
                        FISCA.Presentation.Controls.MsgBox.Show("請先選取學生！", "錯誤", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    else
                        (new Payment_History_Export_Excel()).Execute();
                };

                #endregion

                #region 論文及指導教授

                Catalog button_Paper_Export = RoleAclSource.Instance["學生"]["功能按鈕"];
                button_Paper_Export.Add(new RibbonFeature("Student_Button_Paper_Export", "匯出論文及指導教授"));
                MotherForm.RibbonBarItems["學生", "資料統計"]["匯出"]["其它相關匯出"]["匯出論文及指導教授"].Enable = UserAcl.Current["Student_Button_Paper_Export"].Executable;

                MotherForm.RibbonBarItems["學生", "資料統計"]["匯出"]["其它相關匯出"]["匯出論文及指導教授"].Click += delegate
                {
                    if (K12.Presentation.NLDPanels.Student.SelectedSource.Count == 0)
                        FISCA.Presentation.Controls.MsgBox.Show("請先選取學生！", "錯誤", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    else
                        (new Paper_Export_Excel()).Execute();
                };

                #endregion

                #region 修課記錄(Excel)

                Catalog button_exportSCAttend = RoleAclSource.Instance["學生"]["功能按鈕"];
                button_exportSCAttend.Add(new RibbonFeature("Student_Button_ExportSCAttend", "匯出修課記錄"));
                MotherForm.RibbonBarItems["學生", "資料統計"]["匯出"]["課程相關匯出"]["匯出修課記錄"].Enable = UserAcl.Current["Student_Button_ExportSCAttend"].Executable;

                MotherForm.RibbonBarItems["學生", "資料統計"]["匯出"]["課程相關匯出"]["匯出修課記錄"].Click += delegate
                {
                    if (K12.Presentation.NLDPanels.Student.SelectedSource.Count == 0)
                        FISCA.Presentation.Controls.MsgBox.Show("請先選取學生！", "錯誤", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    else
                        (new SCAttend_Export_Excel("STUDENT")).ShowDialog();
                };

                #endregion

                #region 修課記錄(DBF)

                Catalog button_exportSCAttend_DBF = RoleAclSource.Instance["學生"]["功能按鈕"];
                button_exportSCAttend_DBF.Add(new RibbonFeature("Student_Button_ExportSCAttend_DBF", "匯出修課記錄(DBF 格式)"));
                MotherForm.RibbonBarItems["學生", "資料統計"]["匯出"]["課程相關匯出"]["匯出修課記錄(DBF 格式)"].Enable = UserAcl.Current["Student_Button_ExportSCAttend_DBF"].Executable;

                MotherForm.RibbonBarItems["學生", "資料統計"]["匯出"]["課程相關匯出"]["匯出修課記錄(DBF 格式)"].Click += delegate
                {
                    if (K12.Presentation.NLDPanels.Student.SelectedSource.Count == 0)
                        FISCA.Presentation.Controls.MsgBox.Show("請先選取學生！", "錯誤", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    else
                        (new SCAttend_Export_DBF("STUDENT")).ShowDialog();
                };

                #endregion

                #region 學歷資料

                Catalog button_exportEducation = RoleAclSource.Instance["學生"]["功能按鈕"];
                button_exportEducation.Add(new RibbonFeature("Student_Button_ExportEducation", "匯出學生學歷資料"));
                MotherForm.RibbonBarItems["學生", "資料統計"]["匯出"]["學籍相關匯出"]["匯出學生學歷資料"].Enable = UserAcl.Current["Student_Button_ExportEducation"].Executable;

                MotherForm.RibbonBarItems["學生", "資料統計"]["匯出"]["學籍相關匯出"]["匯出學生學歷資料"].Click += delegate
                {
                    if (K12.Presentation.NLDPanels.Student.SelectedSource.Count == 0)
                        FISCA.Presentation.Controls.MsgBox.Show("請先選取學生！", "錯誤", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    else
                        (new Student_Education_Export_Excel()).Execute();
                };

                #endregion

                #region 經歷資料

                Catalog button_exportExperience = RoleAclSource.Instance["學生"]["功能按鈕"];
                button_exportExperience.Add(new RibbonFeature("Student_Button_ExportExperience", "匯出學生經歷資料"));
                MotherForm.RibbonBarItems["學生", "資料統計"]["匯出"]["學籍相關匯出"]["匯出學生經歷資料"].Enable = UserAcl.Current["Student_Button_ExportExperience"].Executable;

                MotherForm.RibbonBarItems["學生", "資料統計"]["匯出"]["學籍相關匯出"]["匯出學生經歷資料"].Click += delegate
                {
                    if (K12.Presentation.NLDPanels.Student.SelectedSource.Count == 0)
                        FISCA.Presentation.Controls.MsgBox.Show("請先選取學生！", "錯誤", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    else
                        (new Student_Experience_Export_Excel()).Execute();
                };

                #endregion

                #region 異動記錄

                Catalog button_exportUpdateRecord = RoleAclSource.Instance["學生"]["功能按鈕"];
                button_exportUpdateRecord.Add(new RibbonFeature("Student_Button_ExportUpdateRecord", "匯出學生異動記錄"));
                MotherForm.RibbonBarItems["學生", "資料統計"]["匯出"]["學籍相關匯出"]["匯出學生異動記錄"].Enable = UserAcl.Current["Student_Button_ExportUpdateRecord"].Executable;

                MotherForm.RibbonBarItems["學生", "資料統計"]["匯出"]["學籍相關匯出"]["匯出學生異動記錄"].Click += delegate
                {
                    if (K12.Presentation.NLDPanels.Student.SelectedSource.Count == 0)
                        FISCA.Presentation.Controls.MsgBox.Show("請先選取學生！", "錯誤", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    else
                        (new Export.Student_UpdateRecord_Export_Excel(K12.Presentation.NLDPanels.Student.SelectedSource)).Execute();
                };

                #endregion

            #endregion

            #region 複合查詢

            MotherForm.RibbonBarItems["學生", "資料統計"]["查詢"].Image = Properties.Resources.searchHistory;
            MotherForm.RibbonBarItems["學生", "資料統計"]["查詢"].Size = RibbonBarButton.MenuButtonSize.Large;

            Catalog button_searchStudent = RoleAclSource.Instance["學生"]["功能按鈕"];
            button_searchStudent.Add(new RibbonFeature("Student_Button_SearchStudent", "複合查詢學生資料"));
            MotherForm.RibbonBarItems["學生", "資料統計"]["查詢"]["複合查詢"].Enable = UserAcl.Current["Student_Button_SearchStudent"].Executable;

            MotherForm.RibbonBarItems["學生", "資料統計"]["查詢"]["複合查詢"].Click += delegate
            {
                (new EMBACore.Forms.CompoundQueries()).ShowDialog();
            };

            #endregion
            
            #region 學生>資料統計>報表>畢業生成績審核表
            MotherForm.RibbonBarItems["學生", "資料統計"]["報表"].Size = RibbonBarButton.MenuButtonSize.Large;
            MotherForm.RibbonBarItems["學生", "資料統計"]["報表"].Image = Properties.Resources.paste_64;

            RoleAclSource.Instance["學生"]["功能按鈕"].Add(new RibbonFeature("Button_GraduationAuditReport_Print", "列印畢業生成績審核表"));

            MotherForm.RibbonBarItems["學生", "資料統計"]["報表"]["畢業生成績審核表"].Enable = UserAcl.Current["Button_GraduationAuditReport_Print"].Executable;
            MotherForm.RibbonBarItems["學生", "資料統計"]["報表"]["畢業生成績審核表"].Click += delegate
            {
                IEnumerable<string> StudentIDs = K12.Presentation.NLDPanels.Student.SelectedSource;
                if (StudentIDs.Count() == 0)
                    MessageBox.Show("請先選取學生。");
                else
                    (new EMBACore.Report.GraduationAuditReport(StudentIDs)).ShowDialog();
            };
            #endregion
        }


        /// <summary>
        /// 設定學生查詢條件及行為
        /// </summary>
        public static void InitSearch()
        {
            //設定可查詢欄位
            K12.Presentation.NLDPanels.Student.SearchConditionMenu["姓名"].Checked = true;
            K12.Presentation.NLDPanels.Student.SearchConditionMenu["學號"].Checked = true;
            K12.Presentation.NLDPanels.Student.SearchConditionMenu["姓名"].Click += new EventHandler(Util_Click);
            K12.Presentation.NLDPanels.Student.SearchConditionMenu["學號"].Click += new EventHandler(Util_Click);
            K12.Presentation.NLDPanels.Student.SearchConditionMenu["身分證號"].Click += new EventHandler(Util_Click);
            K12.Presentation.NLDPanels.Student.SearchConditionMenu["公司名稱"].Click += new EventHandler(Util_Click);
            K12.Presentation.NLDPanels.Student.SearchConditionMenu["學校名稱"].Click += new EventHandler(Util_Click);
            K12.Presentation.NLDPanels.Student.SearchConditionMenu["備註"].Click += new EventHandler(Util_Click);
            //設定開始查詢的行為
            K12.Presentation.NLDPanels.Student.Search += new EventHandler<FISCA.Presentation.SearchEventArgs>(Student_Search);
        }

        static void Util_Click(object sender, EventArgs e)
        {
            FISCA.Presentation.MenuButton btn = (FISCA.Presentation.MenuButton)sender;
            btn.Checked = !btn.Checked;
        }

        static void Student_Search(object sender, SearchEventArgs e)
        {
            foreach (FISCA.Presentation.MenuButton btnSearch in K12.Presentation.NLDPanels.Student.SearchConditionMenu.SubItems)
            {
                if (btnSearch.Checked)
                {
                    DataTable dt = FindByQuery(btnSearch.Text, e);

                    if (dt.Rows.Count > 0)
                        dt.Rows.Cast<DataRow>().ToList().ForEach((x) =>
                        {
                            if (!e.Result.Contains(x["id"] + ""))
                                e.Result.Add(x["id"] + "");
                        });
                }
            }
        }

        static DataTable FindByQuery(string fieldName, FISCA.Presentation.SearchEventArgs e)
        {
            string strQuery = string.Empty;

            if (fieldName == "公司名稱")
                strQuery = string.Format("SELECT stu.id FROM student stu inner join $ischool.emba.experience exp on stu.id = exp.ref_student_id WHERE company_name ilike '%{0}%'", e.Condition);
            if (fieldName == "學校名稱")
                strQuery = string.Format("SELECT stu.id FROM student stu inner join $ischool.emba.education_background bg on bg.ref_student_id = stu.id WHERE school_name ilike '%{0}%'", e.Condition);
            if (fieldName == "備註")
                strQuery = string.Format("SELECT stu.id FROM student stu inner join $ischool.emba.student_brief2 bg on bg.ref_student_id = stu.id WHERE remark ilike '%{0}%'", e.Condition);
            if (fieldName == "姓名")
                strQuery = string.Format("SELECT stu.id FROM student as stu where name ilike '%{0}%'", e.Condition);
            if (fieldName == "學號")
                strQuery = string.Format("SELECT stu.id FROM student as stu where student_number ilike '%{0}%'", e.Condition);
            if (fieldName == "身分證號")
                strQuery = string.Format("SELECT stu.id FROM student as stu where id_number ilike '%{0}%'", e.Condition);

            return (new QueryHelper()).Select(strQuery);
        }

        static void StudentInit_PopupOpen(object sender, PopupOpenEventArgs e)
        {
            AccessHelper Access = new AccessHelper();
            List<GraduationRequirement> graduationRequirements = Access.Select<GraduationRequirement>();
            List<DepartmentGroup> departmentGroups = Access.Select<DepartmentGroup>();
            //MenuButton mb = e.VirtualButtons["不指定"];
            e.VirtualButtons["不指定"].Click += delegate
            {
                List<string> selectedStudentIDs = K12.Presentation.NLDPanels.Student.SelectedSource;
                if (selectedStudentIDs.Count == 0)
                {
                    MessageBox.Show("請先選擇學生！");
                    return;
                }
                List<StudentBrief2> student_Brief2s = Access.Select<StudentBrief2>(string.Format("ref_student_id in {0}", "(" + string.Join(",", selectedStudentIDs) + ")"));
                if (student_Brief2s.Count > 0)
                {
                    student_Brief2s.ForEach(x => x.GraduationRequirementID = 0);
                    student_Brief2s.SaveAll();
                }
                ListPaneFields.Student_GraduationRequirement.Instance.ReloadMe();
            };
            int i = 0;
            foreach (GraduationRequirement var in graduationRequirements)
            {
                IEnumerable<DepartmentGroup> iDepartmentGroups = departmentGroups.Where(x => x.UID == var.DepartmentGroupID.ToString());
                string departmentGroup = string.Empty;
                if (iDepartmentGroups.Count()>0)
                    departmentGroup = iDepartmentGroups.Select(x => x.Name).ElementAt(0);

                if (string.IsNullOrEmpty(departmentGroup))
                    continue;

                i++;
                //mb = e.VirtualButtons[departmentGroup][var.Name];
                e.VirtualButtons[departmentGroup][var.Name].Tag = var.UID;
                if (i == 1)
                    e.VirtualButtons[departmentGroup].BeginGroup = true;

                e.VirtualButtons[departmentGroup][var.Name].Click += (s1, e1) =>
                {
                    MenuButton mb = (s1 as MenuButton);
                    List<string> selectedStudentIDs = K12.Presentation.NLDPanels.Student.SelectedSource;
                    if (selectedStudentIDs.Count == 0)
                    {
                        MessageBox.Show("請先選擇學生！");
                        return;
                    }
                    List<StudentBrief2> student_Brief2s = Access.Select<StudentBrief2>(string.Format("ref_student_id in {0}", "(" + string.Join(",", selectedStudentIDs) + ")"));
                    List<StudentBrief2> addStudent_Brief2s = new List<StudentBrief2>();
                    if (student_Brief2s.Count > 0)
                    {
                        student_Brief2s.ForEach(x => x.GraduationRequirementID = int.Parse(mb.Tag.ToString()));
                        student_Brief2s.SaveAll();
                    }

                    foreach (string studentID in selectedStudentIDs)
                    {
                        if (student_Brief2s.Where(x => x.StudentID.ToString() == studentID).Count() == 0)
                        {
                            StudentBrief2 studentBrief2 = new StudentBrief2();
                            studentBrief2.StudentID = int.Parse(studentID);
                            studentBrief2.GraduationRequirementID = int.Parse(mb.Tag.ToString());
                            addStudent_Brief2s.Add(studentBrief2);
                        }
                    }
                    if (addStudent_Brief2s.Count > 0)
                        addStudent_Brief2s.SaveAll();

                    ListPaneFields.Student_GraduationRequirement.Instance.ReloadMe();
                };
            }
        }
    }
}
