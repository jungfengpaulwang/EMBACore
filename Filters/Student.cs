using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FISCA.Presentation;
using System.Threading.Tasks;
using FISCA.Data;
using Campus.Windows;
using System.Diagnostics;

namespace EMBACore.Filters
{
    internal class Student
    {
        private NLDPanel Panel { get; set; }

        private bool Inited = false;

        private List<Status> FilterList = new List<Status>();

        private TaskScheduler UISyncContext = null;

        public Student(NLDPanel panel)
        {
            Panel = panel;

            FilterList.Add(new Status("在學", 1));
            FilterList.Add(new Status("休學", 4));
            FilterList.Add(new Status("退學", 64));
            FilterList.Add(new Status("畢業", 16));
            FilterList.Add(new Status("刪除", 256));
        }

        public void Setup()
        {
            if (Inited) return;

            UISyncContext = TaskScheduler.FromCurrentSynchronizationContext();

            K12.Data.Student.AfterChange += Student_AfterChange;

            SetupInternal();

            Inited = true;
        }

        private void Student_AfterChange(object sender, K12.Data.DataChangedEventArgs e)
        {
            CacheProvider.Student.SetOutOfDate("StatusValue");
            SetFilterSourceFromCache();
        }

        private void SetupInternal()
        {
            foreach (Status each in FilterList)
            {
                MenuButton button = Panel.FilterMenu[each.Name];
                button.AutoCollapseOnClick = false;
                button.AutoCheckOnClick = true;
                button.Tag = each;
            }
            Panel.FilterMenu[FilterList[0].Name].Checked = true;

            Panel.FilterMenu.PopupClose += new EventHandler(FilterMenu_PopupClose);

            SetFilterSourceFromCache();
        }

        private void SetFilterSourceFromCache()
        {
            if (CacheProvider.Student.GetOutOfDate("StatusValue"))
            {
                Task task = Task.Factory.StartNew((x) =>
                {
                    QueryHelper backend = new QueryHelper();
                    string cmd = "select id,status from student";

                    backend.Select(cmd).Each(row =>
                    {
                        string id = row["id"] + "";
                        string status = row["status"] + "";

                        CacheProvider.Student.FillProperty(id, "StatusValue", status);
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
            int status = 0;
            //取得哪些狀況被選擇了。
            foreach (MenuButton button in Panel.FilterMenu.SubItems)
            {
                if (!button.Checked) continue;

                Status s = button.Tag as Status;
                if (s != null)
                    status |= s.Value;
            }

            HashSet<string> result = new HashSet<string>();

            foreach (dynamic each in CacheProvider.Student)
            {
                int v;
                if (int.TryParse(each.StatusValue + "", out v))
                {
                    if ((status & v) == v)
                        result.Add(each.Id);
                }
            }

            Panel.SetFilteredSource(result.ToList());
        }

        private void FilterMenu_PopupClose(object sender, EventArgs e)
        {
            SetFilterSourceFromCache();
        }

        private class Status
        {
            public Status(string name, int value)
            {
                Name = name;
                Value = value;
            }

            public string Name { get; private set; }

            public int Value { get; private set; }
        }
    }
}
