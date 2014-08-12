using System;
using System.Collections.Generic;
using System.Data;
using System.Threading;
using System.Threading.Tasks;
using Campus;
using Campus.Windows;
using Customization.Tagging;
using FISCA;
using FISCA.Authentication;
using FISCA.Data;
using FISCA.Permission;
using FISCA.Presentation;
using K12.Data;
using K12.Presentation;

namespace EMBACore
{
    public static class Program
    {
        [ApplicationMain()]
        public static void Main()
        {
            //建立一個包含「學生、班級、教師、課程」的基本 Layout。
            new DesktopLayout().SetupPresentation();
            //年級預設有 3 個年級。
            GradeYear.Defaults = GradeYear.ToGradeYears(1, 2, 3);

            try
            { //設定「使用者、資料庫、全域」的組態提供者。
                Campus.Configuration.Config.Initialize(
                    new Campus.Configuration.UserConfigManager(new Campus.Configuration.ConfigProvider_User(), DSAServices.UserAccount),
                    new Campus.Configuration.ConfigurationManager(new Campus.Configuration.ConfigProvider_App()),
                    new Campus.Configuration.ConfigurationManager(new Campus.Configuration.ConfigProvider_Global()));
            }
            catch (Exception ex)
            {
                RTOut.WriteError(ex);
            }

            CustomizationService.Register<GetStudentStatusList>(() =>
            {
                List<StatusItem> status = new List<StatusItem>();

                status.Add(new StatusItem() { Status = K12.Data.StudentRecord.StudentStatus.一般, Text = "在學" });
                status.Add(new StatusItem() { Status = K12.Data.StudentRecord.StudentStatus.休學, Text = "休學" });
                status.Add(new StatusItem() { Status = K12.Data.StudentRecord.StudentStatus.退學, Text = "退學" });
                status.Add(new StatusItem() { Status = K12.Data.StudentRecord.StudentStatus.畢業或離校, Text = "畢業" });
                status.Add(new StatusItem() { Status = K12.Data.StudentRecord.StudentStatus.刪除, Text = "刪除" });

                return status;
            });

            Catalog button_addSchool = RoleAclSource.Instance["系統"];
            button_addSchool.Add(new RibbonFeature("StartButton0003", "管理學校基本資料"));

            // 管理學校基本資料
            MotherForm.StartMenu["管理學校基本資料"].Image = Properties.Resources.icon64_school_info;
            MotherForm.StartMenu["管理學校基本資料"].Enable = UserAcl.Current["StartButton0003"].Executable;
            MotherForm.StartMenu["管理學校基本資料"].Click += delegate
            {
                //還沒加!
                (new Forms.SchoolInfo()).ShowDialog();
            };

            //學生的「年級班級」檢視。
            NLDPanels.Student.AddView(new Views.Student_GradeClassView());
            //學生的「系所組別」檢視。
            NLDPanels.Student.AddView(new Views.Student_DepartmentGroupView());
            //班級的「年級」檢視。
            NLDPanels.Class.AddView(new Views.Class_GradeView());
            //教師的「班導師」檢視。
            //NLDPanels.Teacher.AddView(new Views.Teacher_AdvisorView()); //EMBA 不需要 (ref 2012/1/5  see zoe's email)

            //同步 UDT_Schmea
            Initialization.UDTInit.Init();

            //初始化學生功能，包括 RibbonBarItem 及其權限
            Initialization.StudentInit.Init();

            //初始化教師功能，包括 RibbonBarItem 及其權限
            Initialization.TeacherInit.Init();

            //初始化班級功能，包括 RibbonBarItem 及其權限
            Initialization.ClassInit.Init();

            //初始化課程功能，包括 RibbonBarItem 及其權限
            Initialization.CourseInit.Init();

            //初始化其它功能，包括 RibbonBarItem 及其權限
            Initialization.OtherInit.Init();

            /* 測試程式碼 */
            CreateListPaneFields();

            MotherForm.TabGotFocus += new EventHandler<TabGotFocusEventArgs>(MotherForm_TabGotFocus);

            //  主畫面標題
            EMBACore.Initialization.OtherInit.InitMotherFormTitle();
        }


        /* 測試程式碼 */

        private static Dictionary<string, DataRow> RowDictionary = null;

        private static ManualResetEventSlim CacheSync = new ManualResetEventSlim(false);

        private static void CreateListPaneFields()
        {
            CacheSync.Reset();
            Task task = Task.Factory.StartNew((x) =>
            {
                QueryHelper query = new QueryHelper();
                string cmd = @"
select student.id,name,class_name,seat_no,student_number
from student left join class on student.ref_class_id=class.id";

                DataTable dt = query.Select(cmd);
                CacheProvider.Student.Clear();
                foreach (DataRow row in dt.Rows)
                {
                    CacheProvider.Student.FillProperty(row["id"].ToString(), "Name", row["name"].ToString());
                    CacheProvider.Student.FillProperty(row["id"].ToString(), "ClassName", row["class_name"].ToString());
                    CacheProvider.Student.FillProperty(row["id"].ToString(), "SeatNo", row["seat_no"].ToString());
                    CacheProvider.Student.FillProperty(row["id"].ToString(), "StudentNumber", row["student_number"].ToString());
                }
            }, TaskScheduler.Default);

            task.ContinueWith((xx) =>
            {
                CacheSync.Set();

                NLDPanels.Student.CompareSource += (sender, e) =>
                {
                    string seatX = CacheProvider.Student[e.Value1].SeatNo;
                    string seatY = CacheProvider.Student[e.Value2].SeatNo;
                    int x, y;

                    if (!int.TryParse(seatX, out x))
                        x = 0;

                    if (!int.TryParse(seatY, out y))
                        y = 0;

                    e.Result = x.CompareTo(y);
                };

            }, TaskScheduler.FromCurrentSynchronizationContext());

            ListPaneField field = new ListPaneField("學生系統編號", false);
            NLDPanels.Student.AddListPaneField(field);
            field.GetVariable += (sender, e) => { e.Value = e.Key; };
           
            //班級
            field = new ListPaneField("系統編號");
            NLDPanels.Class.AddListPaneField(field);
            field.GetVariable += (sender, e) => { e.Value = e.Key; };

            //教師
            field = new ListPaneField("系統編號");
            NLDPanels.Teacher.AddListPaneField(field);
            field.GetVariable += (sender, e) => { e.Value = e.Key; };

            //課程
            field = new ListPaneField("系統編號");
            NLDPanels.Course.AddListPaneField(field);
            field.GetVariable += (sender, e) => { e.Value = e.Key; };

            //學生相關欄位。
            new ListPaneFields.Student_Name() { CacheProvider = CacheProvider.Student }.Register(NLDPanels.Student);
            new ListPaneFields.Student_Class() { CacheProvider = CacheProvider.Student }.Register(NLDPanels.Student);
            new ListPaneFields.Student_SeatNo() { CacheProvider = CacheProvider.Student }.Register(NLDPanels.Student);
            new ListPaneFields.Student_StudentNumber() { CacheProvider = CacheProvider.Student }.Register(NLDPanels.Student);
            new ListPaneFields.Student_Gender() { CacheProvider = CacheProvider.Student }.Register(NLDPanels.Student);

            ListPaneFields.Student_GraduationRequirement.Instance.Register(NLDPanels.Student);

            //new ListPaneFields.Student_GraduationRequirement().Register(NLDPanels.Student);

            //班級相關欄位。
            new ListPaneFields.Class_ClassName() { CacheProvider = CacheProvider.Class }.Register(NLDPanels.Class);

            //教師相關欄位。
            new ListPaneFields.Teacher_Name() { CacheProvider = CacheProvider.Teacher }.Register(NLDPanels.Teacher);

            //課程相關欄位。
            new ListPaneFields.Course_CourseName() { CacheProvider = CacheProvider.Course }.Register(NLDPanels.Course);
            new ListPaneFields.Course_ClassName() { CacheProvider = CacheProvider.Course }.Register(NLDPanels.Course);
            new ListPaneFields.Course_SerialNumber() { CacheProvider = CacheProvider.Course }.Register(NLDPanels.Course);
            //new ListPaneFields.Course_SubjectSemesterScoreInputRule() { CacheProvider = CacheProvider.Course }.Register(NLDPanels.Course);
            ListPaneFields.Course_SubjectSemesterScoreInputRule.Instance.Register(NLDPanels.Course);
        }

        #region SetFilteredSource
        private static bool ClassLoaded = false;
        private static void MotherForm_TabGotFocus(object sender, TabGotFocusEventArgs e)
        {
            if (e.TabName == "學生")
                Filters.FilterService.Student.Setup();

            if (e.TabName == "教師")
                Filters.FilterService.Teacher.Setup();

            if (e.TabName == "課程")
                Filters.FilterService.Course.Setup();

            if (e.TabName == "班級" && !ClassLoaded)
            {
                ReloadClassID();
                Class.AfterChange += (s, args) => ReloadClassID();
            }
        }

        private static void ReloadClassID()
        {
            MotherForm.SetStatusBarMessage("讀取資料清單中...");
            List<string> class_keys = null;
            Task task = Task.Factory.StartNew((x) =>
            {
                class_keys = LoadClassKeys();
            }, TaskScheduler.Default);

            task.ContinueWith((x) =>
            {
                NLDPanels.Class.SetFilteredSource(class_keys);
                MotherForm.SetStatusBarMessage("");
                ClassLoaded = true;
            }, TaskScheduler.FromCurrentSynchronizationContext());
        }

        private static List<string> LoadClassKeys()
        {
            try
            {
                QueryHelper query = new QueryHelper();

                string cmd = @"select class.id from class";

                DataTable result = query.Select(cmd);

                List<string> keys = new List<string>();
                foreach (DataRow row in result.Rows)
                    keys.Add(row["id"].ToString());

                return keys;
            }
            catch (Exception ex)
            {
                RTOut.WriteError(ex);
                return new List<string>();
            }
        }
        #endregion
    }
}
