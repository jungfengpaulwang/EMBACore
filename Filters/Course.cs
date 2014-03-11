using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Campus;
using Campus.Windows;
using FISCA.Data;
using FISCA.Presentation;
using K12.Data;

namespace EMBACore.Filters
{
    internal class Course
    {
        private NLDPanel Panel { get; set; }

        private bool Inited = false;

        private List<SemesterData> Semesters = new List<SemesterData>();

        private SemesterData? Selected;

        private TaskScheduler UISyncContext = null;

        private List<string> resultCourseIDs = new List<string>();

        private SemesterData semesterData;

        public Course(NLDPanel panel)
        {
            Panel = panel;
        }

        public void Setup()
        {
            if (Inited) return;

            UISyncContext = TaskScheduler.FromCurrentSynchronizationContext();

            K12.Data.Course.AfterChange += Course_AfterChange;

            SetupInternal();

            Inited = true;
        }

        private void Course_AfterChange(object sender, DataChangedEventArgs e)
        {
            CacheProvider.Course.SetOutOfDate("SchoolYear", "Semester");
            CacheProvider.Course.Remove(e.PrimaryKeys);

            SetFilterSourceFromCache();
        }

        private void SetupInternal()
        {
            SetFilterSourceFromCache();
        }

        private void SetFilterSourceFromCache()
        {
            //先把選擇的項目記錄起來。
            Selected = null;
            foreach (MenuButton button in Panel.FilterMenu.SubItems)
            {
                if (!button.Checked) continue;

                SemesterData? semester = button.Tag as SemesterData?;
                if (semester.HasValue)
                    Selected = semester;
            }

            if (CacheProvider.Course.GetOutOfDate("SchoolYear", "Semester"))
            {
                Task task = Task.Factory.StartNew((t) =>
                {
                    QueryHelper backend = new QueryHelper();
                    string cmd = "select school_year,semester from course where not school_year is null and not semester is null group by school_year,semester";

                    //將所有學年度學期列出。
                    Semesters.Clear();
                    backend.Select(cmd).Each(row =>
                    {
                        string sy = row["school_year"] + "";
                        string ss = row["semester"] + "";

                        Semesters.Add(new SemesterData(int.Parse(sy), int.Parse(ss)));
                    });
                    Semesters.Sort(new System.Comparison<SemesterData>((x, y) =>
                    {
                        return x.Order.CompareTo(y.Order);
                    }));

                    cmd = string.Format("select id,school_year,semester from course", School.DefaultSchoolYear, School.DefaultSemester);

                    backend.Select(cmd).Each(row =>
                    {
                        string id = row["id"] + "";
                        string sy = row["school_year"] + "";
                        string ss = row["semester"] + "";

                        CacheProvider.Course.FillProperty(id, "SchoolYear", sy);
                        CacheProvider.Course.FillProperty(id, "Semester", ss);
                    });
                }, TaskScheduler.Default);

                task.ContinueWith((x) =>
                {
                    SetFilterSource();
                }, UISyncContext);
            }
            else
                SetFilterSource();
        }

        private void SetFilterSource()
        {
            foreach (MenuButton button in Panel.FilterMenu.SubItems)
            {
                button.Visible = false;
            }

            if (Selected == null)
                Selected = new SemesterData(int.Parse(School.DefaultSchoolYear), int.Parse(School.DefaultSemester));

            foreach (SemesterData sems in Semesters)
            {
                MenuButton button = Panel.FilterMenu[string.Format("[{0}] [{1}]", sems.SchoolYear, DataItems.SemesterItem.GetSemesterByCode(sems.Semester.ToString()).Name)];
                InitButton(sems, button);

                if (sems == Selected)
                    button.Checked = true;
            }

            MenuButton mb = Panel.FilterMenu["其他學期"];
            InitButton(SemesterData.Undefined, mb);
            if (SemesterData.Undefined == Selected)
                mb.Checked = true;

            if (Selected.Value == SemesterData.Undefined)
                Panel.FilterMenu.Text = "其他學期";
            else
            {
                //  學年度與學期是否顯示中文要請示長官再修改。
                Panel.FilterMenu.Text = string.Format("[{0}] [{1}]", Selected.Value.SchoolYear,  DataItems.SemesterItem.GetSemesterByCode(Selected.Value.Semester.ToString()).Name);
            }

            HashSet<string> result = new HashSet<string>();

            foreach (dynamic course in CacheProvider.Course)
            {
                int sy, ss;

                if (!int.TryParse(course.SchoolYear + "", out sy))
                    sy = -1;

                if (!int.TryParse(course.Semester + "", out ss))
                    ss = -1;

                SemesterData sd;
                if (sy == -1 || ss == -1)
                    sd = SemesterData.Undefined;
                else
                    sd = new SemesterData(sy, ss);

                if (Selected.Value == sd)
                    result.Add(course.Id);
            }
            this.resultCourseIDs = result.ToList();
            this.semesterData = Selected.Value ;
            Panel.SetFilteredSource(this.resultCourseIDs);

        }

        public SemesterData GetFilteredSemesterData()
        {
            return this.semesterData;
        }

        public List<string> GetFiltetedCourseID()
        {
            return this.resultCourseIDs;
        }


        private HashSet<SemesterData> InitedSet = new HashSet<SemesterData>();

        private void InitButton(SemesterData sems, MenuButton button)
        {
            button.Tag = sems;
            button.AutoCheckOnClick = true;
            button.AutoCollapseOnClick = true;
            button.Visible = true;

            if (InitedSet.Contains(sems))
                return;

            button.Click += Button_Click;
            InitedSet.Add(sems);
        }

        private void Button_Click(object sender, EventArgs e)
        {
            foreach (MenuButton button in Panel.FilterMenu.SubItems)
                button.Checked = false;

            MenuButton current = sender as MenuButton;

            if (current != null)
            {
                current.Checked = true;

                if (current.Checked)
                    Selected = (SemesterData)current.Tag;
            }

            SetFilterSourceFromCache();
        }
    }
}
