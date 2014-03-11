using System;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;
using Campus.Windows;
using FISCA.Data;
using K12.Data;
using System.Windows.Forms;
using FISCA.Permission;
using EMBACore.DataItems;
using EMBACore.UDT;
using FISCA.UDT;

namespace EMBACore.DetailItems
{
    [AccessControl("ischool.EMBA.Student.Detail0003", "系所/班級", "學生>資料項目")]
    public partial class Student_Class : DetailContentImproved
    {
        private StudentRecord Record = null;
        private DepartmentGroup _DepartmentGroup = null;
        private StudentBrief2 _StudentBrief2 = null;

        private List<ClassItem> ClassRowSource = new List<ClassItem>();
        private List<DepartmentGroupItem> DepartmentGroupRowSource = new List<DepartmentGroupItem>();

        private AccessHelper Access = null;

        /*  log  */
        private Log.LogAgent logAgent = new Log.LogAgent();

        public Student_Class()
        {
            InitializeComponent();
            Group = "系所/班級";
        }

        private void Student_Class_Load(object sender, EventArgs e)
        {
            WatchChange(new ComboBoxSource(cboClass, ComboBoxSource.ListenAttribute.Text));
            WatchChange(new ComboBoxSource(cboSeatNo, ComboBoxSource.ListenAttribute.Text));
            WatchChange(new ComboBoxSource(this.cboDepartmentGroup, ComboBoxSource.ListenAttribute.Text));
            WatchChange(new TextBoxSource(txtStudentNumber));
            WatchChange(new TextBoxSource(this.txtClassYear));
            WatchChange(new TextBoxSource(this.txtSchoolYear));
            
            Class.AfterChange += (x, y) => ReInitialize();
        }

        private void LoadRowSource()
        {
            QueryHelper query = new QueryHelper();
            string cmd = "select id,class_name,grade_year from class order by class_name";
            DataTable dt = query.Select(cmd);

            ClassRowSource.Clear();
            ClassRowSource.Add(new ClassItem("", "",""));
            foreach (DataRow row in dt.Rows)
            {
                ClassItem item = new ClassItem(row["id"].ToString(), row["class_name"].ToString(), row["grade_year"].ToString());
                ClassRowSource.Add(item);
            }

            Access = new AccessHelper();
            List<DepartmentGroup> departmentGroups = Access.Select<DepartmentGroup>();
            DepartmentGroupRowSource.Clear();
            DepartmentGroupRowSource.Add(new DepartmentGroupItem("", ""));
            foreach(DepartmentGroup departmentGroup in departmentGroups)
            {
                DepartmentGroupItem item = new DepartmentGroupItem(departmentGroup.UID, departmentGroup.Name);
                DepartmentGroupRowSource.Add(item);
            }
        }

        private void BindClassRowSource(string entranceYear)
        {
            List<ClassItem> data = new List<ClassItem>();
            foreach (ClassItem ci in ClassRowSource)
            {
                if (ci.SchoolYear.Trim() == entranceYear.Trim()  ||
                      ci.ID == "")
                    data.Add(ci);
            }

            //cboClass.Items.Clear();
            cboClass.DataSource =  (data.Count > 1 ) ? data  : ClassRowSource;  //一定會有一筆空白的資料
            cboClass.ValueMember = "ID";
            cboClass.DisplayMember = "Name";
        }

        private void BindDepartmentGroupRowSource(string code)
        {
            List<DepartmentGroupItem> data = new List<DepartmentGroupItem>();
            foreach (DepartmentGroupItem ci in DepartmentGroupRowSource)
            {
                if (ci.Name.Contains(code.Trim()) || ci.ID == "")
                    data.Add(ci);
            }

            cboDepartmentGroup.DataSource = (data.Count > 1) ? data : DepartmentGroupRowSource;
            cboDepartmentGroup.ValueMember = "ID";
            cboDepartmentGroup.DisplayMember = "Name";
        }

        protected override void OnInitializeAsync()
        {
            LoadRowSource();
        }

        protected override void OnInitializeComplete(Exception error)
        {
            if (error != null) //有錯就直接丟出去吧。
                throw error;

            BindClassRowSource("");
            BindDepartmentGroupRowSource("");
        }

        protected override void OnPrimaryKeyChangedAsync()
        {
            Record = Student.SelectByID(PrimaryKey);

            List<StudentBrief2> student_Brief2s = Access.Select<StudentBrief2>(string.Format("ref_student_id = {0}", Record.ID));
            if (student_Brief2s.Count > 0)
                _StudentBrief2 = student_Brief2s[0];
            else
                _StudentBrief2 = new StudentBrief2();

            //List<DepartmentGroup> departmentGroups = Access.Select<DepartmentGroup>(string.Format("uid = {0}", _StudentBrief2.DepartmentGroupID.ToString()));
            /*
            if (departmentGroups.Count > 0)
                _DepartmentGroup = departmentGroups[0];
            else
                _DepartmentGroup = new DepartmentGroup();
             * */
        }

        protected override void OnPrimaryKeyChangedComplete(Exception error)
        {
            if (Record == null)
                throw new Exception("資料可能已經被其他人刪除。");

            if (error != null) //有錯就直接丟出去吧。
                throw error;

            ErrorTip.Clear();
            FillEmptySeatNos();

            txtStudentNumber.Text = Record.StudentNumber;
            txtSchoolYear.Text = _StudentBrief2.EnrollYear;
            txtClassYear.Text = _StudentBrief2.GradeYear.ToString() ;
            this.BindClassRowSource(_StudentBrief2.EnrollYear == null ? string.Empty : _StudentBrief2.EnrollYear.Trim());
            cboClass.SelectedValue = Record.RefClassID;
            //Code.Text = _DepartmentGroup.Code.Trim();
            Code.Text = _StudentBrief2.DepartmentGroupCode;
            this.BindDepartmentGroupRowSource(Code.Text);
            cboDepartmentGroup.SelectedValue = _StudentBrief2.DepartmentGroupID.ToString(); //(_DepartmentGroup == null ? null : _DepartmentGroup.UID);
            cboSeatNo.Text = Record.SeatNo + "";

            /* log */
            this.logAgent.Clear();
            this.AddLog(this.Record, _StudentBrief2);
            this.logAgent.ActionType = Log.LogActionType.Update;

            ResetDirtyStatus();
        }

        protected override void OnDirtyStatusChanged(ChangeEventArgs e)
        {
            if (UserAcl.Current[this.GetType()].Editable)
                SaveButtonVisible = e.Status == ValueStatus.Dirty;
            else
                this.SaveButtonVisible = false;

            CancelButtonVisible = e.Status == ValueStatus.Dirty;
        }

        protected override void OnValidateData(Dictionary<Control, string> errors)
        {
            if (cboClass.SelectedIndex < 0)
                errors.Add(cboClass, "請選擇「教學分班」中的項目。");

            if (cboDepartmentGroup.SelectedIndex < 0)
                errors.Add(cboDepartmentGroup, "請選擇「系所組別」中的項目。");

            if (!string.IsNullOrWhiteSpace(cboSeatNo.Text))
            {
                int seatno;
                if (!int.TryParse(cboSeatNo.Text, out seatno))
                    errors.Add(cboSeatNo, "座號請輸入圍範內數字(1~100)。");
            }

            if (!string.IsNullOrWhiteSpace(this.txtSchoolYear.Text))
            {
                int enrollYear;
                bool result = int.TryParse(txtSchoolYear.Text.Trim(), out enrollYear);
                if (!result)
                    errors.Add(txtSchoolYear, "「入學年度」請填入阿拉伯數字。");
                else
                {
                    if (string.IsNullOrWhiteSpace(this.cboClass.Text))
                    {
                        if (!errors.ContainsKey(this.cboClass))
                            errors.Add(this.cboClass, "請選擇教學分班！");
                        else
                            errors[this.cboClass] = "請選擇教學分班！";
                    }
                }
            }
        }

        protected override void OnSaveData()
        {
            QueryHelper queryHelper = new QueryHelper();
            DataTable dataTable = queryHelper.Select(string.Format("select class_name, student.name as name, student.id as id, student_number from student left join class on class.id=student.ref_class_id where student_number ilike ('{0}')", txtStudentNumber.Text.Trim()));

            string msg = string.Empty;
            foreach (DataRow row in dataTable.Rows)
            {
                if ((row["id"] + "") != PrimaryKey)
                {
                    string className = row["class_name"] + "";
                    string studentName = row["name"] + "";
                    msg += string.Format("教學分班：{0}，姓名：{1}，已佔用此學號！", className, studentName) + "\n";
                }
            }
            if (!string.IsNullOrEmpty(msg))
            {
                MsgBox.Show(msg);
                return;
            }
            int seatno;
            if (int.TryParse(cboSeatNo.Text, out seatno))
                Record.SeatNo = int.Parse(cboSeatNo.Text);
            else
                Record.SeatNo = null;

            Record.RefClassID = cboClass.SelectedValue + "";
            Record.StudentNumber = txtStudentNumber.Text.Trim();

            

            int enrollYear = 0;
            int.TryParse(txtSchoolYear.Text.Trim(), out enrollYear);
            _StudentBrief2.EnrollYear = enrollYear.ToString();
            //_StudentBrief2.GraduateYear = this.txtClassYear.Text.Trim();  //2012/5/25 kevin, 此欄位為唯獨。

            int departmentGroupID = 0;
            int.TryParse(this.cboDepartmentGroup.SelectedValue + "", out departmentGroupID);
            _StudentBrief2.DepartmentGroupID = departmentGroupID;

            Student.Update(Record);
            _StudentBrief2.Save();

            /* log */
            this.AddLog(this.Record, _StudentBrief2);
            this.logAgent.ActionType = Log.LogActionType.Update;

            ResetDirtyStatus();
                        
            this.logAgent.Save("學生的班級資訊", "修改：", "", Log.LogTargetCategory.Student, this.Record.ID);
        }

        private void cboClass_SelectedValueChanged(object sender, EventArgs e)
        {
            if (IsInitialized)
                FillEmptySeatNos();
        }

        #region 產生空座號
        private void FillEmptySeatNos()
        {
            string classId = "0";

            Circular.IsRunning = true;
            Circular.Visible = true;

            HashSet<int> EmptySeatNos = new HashSet<int>();

            if (!string.IsNullOrWhiteSpace(cboClass.SelectedValue + ""))
                classId = (cboClass.SelectedValue.ToString());

            Task task = new Task(() =>
            {//非同步讀取空座號資料。
                EmptySeatNos = GetEmptySeatNos(classId);
            });
            task.Start(TaskScheduler.Default);

            task.ContinueWith(x =>
            {
                if (x.IsFaulted)
                    cboSeatNo.Items.Add("錯誤!");
                else
                {
                    cboSeatNo.Items.Clear();
                    cboSeatNo.Items.Add("");

                    //如果目前學生的座號不包含，就加入。
                    if (Record.SeatNo.HasValue && !EmptySeatNos.Contains(Record.SeatNo.Value))
                        EmptySeatNos.Add(Record.SeatNo.Value);

                    List<object> seatnos = new List<object>();
                    foreach (int item in EmptySeatNos)
                        seatnos.Add(item);

                    seatnos.Sort();

                    cboSeatNo.Items.AddRange(seatnos.ToArray());
                }

                Circular.Visible = false;
            }, UISyncContext);
        }

        private HashSet<int> GetEmptySeatNos(string classId)
        {
            if (string.IsNullOrWhiteSpace(classId)) return new HashSet<int>();

            //目前有的座號。
            List<int> currents = new List<int>();
            string cmd = string.Format("select seat_no from student where ref_class_id='{0}' group by seat_no order by seat_no", classId);
            Backend.Select(cmd).Each(row =>
            {
                int seatno;
                if (int.TryParse(row["seat_no"].ToString(), out seatno))
                    currents.Add(seatno);
            });

            //算人數。
            int count = 0;
            cmd = string.Format("select count(*) count from student where ref_class_id='{0}'", classId);
            Backend.Select(cmd).Each(row =>
            {
                count = int.Parse(row["count"].ToString());
            });

            HashSet<int> allseatno = new HashSet<int>();
            for (int i = 1; i <= count; i++)
                allseatno.Add(i);

            allseatno.ExceptWith(currents);

            return allseatno;
        }
        #endregion

        private void cboClass_TextChanged(object sender, EventArgs e)
        {
            //確保使用者在「輸入」班級名稱時，SelectedIndex 會在正確的項目上。
            int found = cboClass.FindStringExact(cboClass.Text);

            if (found >= 0)
                cboClass.SelectedIndex = found;
        }

        private void cboDepartmentGroup_TextChanged(object sender, EventArgs e)
        {
            //確保使用者在「輸入」班級名稱時，SelectedIndex 會在正確的項目上。
            int found = cboDepartmentGroup.FindStringExact(cboDepartmentGroup.Text);

            if (found >= 0)
                cboDepartmentGroup.SelectedIndex = found;
        }


         void AddLog(StudentRecord obj, UDT.StudentBrief2 obj2)
        {
            this.logAgent.SetLogValue("班級", (obj.Class == null ? "" : obj.Class.Name));
            this.logAgent.SetLogValue("座號", (obj.SeatNo == null ? "" : obj.SeatNo.ToString()));
            this.logAgent.SetLogValue("學號", obj.StudentNumber + "");
            this.logAgent.SetLogValue("年級", obj2.GradeYear + "");
            this.logAgent.SetLogValue("入學年度", obj2.EnrollYear + "");
            this.logAgent.SetLogValue("系所組別", this.cboDepartmentGroup.Text);
        }

         private void txtSchoolYear_Leave(object sender, EventArgs e)
         {
             this.BindClassRowSource(this.txtSchoolYear.Text.Trim());
             this.cboClass.SelectedValue = "";
         }

         private void Code_Leave(object sender, EventArgs e)
         {

         }
    }

    public class DepartmentGroupItem
    {
        public DepartmentGroupItem(string id, string name)
        {
            ID = id;
            Name = name;
        }

        public string ID { get; set; }

        public string Name { get; set; }
    }
}
