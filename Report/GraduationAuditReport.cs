using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using Aspose.Cells;
using Aspose.Words;
using FISCA.Data;
using FISCA.Presentation.Controls;
using FISCA.UDT;
using K12.Data;
using ReportHelper;

namespace EMBACore.Report
{
    public partial class GraduationAuditReport : BaseForm
    {
        private AccessHelper Access;
        private QueryHelper Query;

        private Dictionary<string, StudentRecord> dicStudents;
        private Dictionary<string, UDT.DepartmentGroup> dicDepartmentGroups;
        private Dictionary<string, UDT.StudentBrief2> dicStudentBrief2;
        private Dictionary<string, UDT.GraduationRequirement> dicGraduationRequirements;
        private Dictionary<string, List<UDT.SubjectSemesterScore>> dicSubjectSemesterScores;
        private Dictionary<int, Dictionary<string, UDT.GraduationSubjectGroupRequirement>> dicGraduationSubjectGroupRequirements;
        private Dictionary<int, Dictionary<int, UDT.GraduationSubjectList>> dicGraduationSubjectLists;
        private Dictionary<string, UDT.Subject> dicSubjects;

        private List<string> SubjectGroups_Sort;
        
        public GraduationAuditReport(IEnumerable<string> StudentIDs)
        {
            InitializeComponent();

            this.circularProgress.IsRunning = true;
            this.circularProgress.Visible = true;
            this.btnPrint.Enabled = false;

            this.Load += new EventHandler(GraduationAuditReport_Load);

            Access = new AccessHelper();
            Query = new QueryHelper();

            this.dicStudents = new Dictionary<string, StudentRecord>();
            this.dicDepartmentGroups = new Dictionary<string, UDT.DepartmentGroup>();
            this.dicStudentBrief2 = new Dictionary<string, UDT.StudentBrief2>();
            this.dicGraduationRequirements = new Dictionary<string, UDT.GraduationRequirement>();
            this.dicSubjectSemesterScores = new Dictionary<string, List<UDT.SubjectSemesterScore>>();
            this.dicGraduationSubjectGroupRequirements = new Dictionary<int, Dictionary<string, UDT.GraduationSubjectGroupRequirement>>();
            this.dicGraduationSubjectLists = new Dictionary<int, Dictionary<int, UDT.GraduationSubjectList>>();
            this.dicSubjects = new Dictionary<string, UDT.Subject>();

            SubjectGroups_Sort = new List<string>() { "核心必修", "核心選修三選二", "組必修" };

            this.InitSchoolYear();
            this.InitSemester();

            this.InitData(StudentIDs);

            dtDueDate.Value = DateTime.Today;
        }

        private void GraduationAuditReport_Load(object sender, EventArgs e)
        {

        }

        private void InitData(IEnumerable<string> StudentIDs)
        {
            Task task = Task.Factory.StartNew(() =>
            {
                List<StudentRecord> Students = Student.SelectByIDs(StudentIDs);
                Students.ForEach((x) =>
                {
                    if (!this.dicStudents.ContainsKey(x.ID))
                        this.dicStudents.Add(x.ID, x);
                });
                List<UDT.DepartmentGroup> DepartmentGroups = Access.Select<UDT.DepartmentGroup>();
                DepartmentGroups.ForEach((x) =>
                {
                    if (!this.dicDepartmentGroups.ContainsKey(x.UID))
                        this.dicDepartmentGroups.Add(x.UID, x);
                });
                List<UDT.StudentBrief2> StudentBriefs = Access.Select<UDT.StudentBrief2>(string.Format("ref_student_id in ({0})", string.Join(",", StudentIDs)));
                StudentBriefs.ForEach((x) =>
                {
                    if (!this.dicStudentBrief2.ContainsKey(x.StudentID.ToString()))
                        this.dicStudentBrief2.Add(x.StudentID.ToString(), x);
                });
                List<UDT.GraduationRequirement> GraduationRequirements = Access.Select<UDT.GraduationRequirement>();
                GraduationRequirements.ForEach((x) =>
                {
                    if (!this.dicGraduationRequirements.ContainsKey(x.UID))
                        this.dicGraduationRequirements.Add(x.UID, x);
                });
                List<UDT.SubjectSemesterScore> SubjectSemesterScores = Access.Select<UDT.SubjectSemesterScore>(string.Format("ref_student_id in ({0})", string.Join(",", StudentIDs)));
                SubjectSemesterScores.ForEach((x) =>
                {
                    if (!this.dicSubjectSemesterScores.ContainsKey(x.StudentID.ToString()))
                        this.dicSubjectSemesterScores.Add(x.StudentID.ToString(), new List<UDT.SubjectSemesterScore>());

                    dicSubjectSemesterScores[x.StudentID.ToString()].Add(x);
                });
                List<UDT.GraduationSubjectGroupRequirement> GraduationSubjectGroupRequirements = Access.Select<UDT.GraduationSubjectGroupRequirement>();
                GraduationSubjectGroupRequirements.ForEach((x) =>
                {
                    if (!this.dicGraduationSubjectGroupRequirements.ContainsKey(x.GraduationRequirementID))
                        this.dicGraduationSubjectGroupRequirements.Add(x.GraduationRequirementID, new Dictionary<string, UDT.GraduationSubjectGroupRequirement>());

                    if (!this.dicGraduationSubjectGroupRequirements[x.GraduationRequirementID].ContainsKey(x.SubjectGroup))
                        this.dicGraduationSubjectGroupRequirements[x.GraduationRequirementID].Add(x.SubjectGroup, x);
                });
                List<UDT.GraduationSubjectList> GraduationSubjectLists = Access.Select<UDT.GraduationSubjectList>();
                GraduationSubjectLists.ForEach((x) =>
                {
                    if (!this.dicGraduationSubjectLists.ContainsKey(x.GraduationRequirementID))
                        this.dicGraduationSubjectLists.Add(x.GraduationRequirementID, new Dictionary<int, UDT.GraduationSubjectList>());

                    if (!this.dicGraduationSubjectLists[x.GraduationRequirementID].ContainsKey(x.SubjectID))
                        this.dicGraduationSubjectLists[x.GraduationRequirementID].Add(x.SubjectID, x);
                });
                List<UDT.Subject> Subjects = Access.Select<UDT.Subject>();
                if (Subjects.Count > 0)
                    this.dicSubjects = Subjects.ToDictionary(x => x.UID);
            });
            task.ContinueWith((x) =>
            {
                this.circularProgress.IsRunning = false;
                this.circularProgress.Visible = false;

                if (x.Exception != null)
                    MessageBox.Show(x.Exception.InnerException.Message);
                else
                {
                    this.btnPrint.Enabled = true;
                }
            }, System.Threading.CancellationToken.None, TaskContinuationOptions.None, TaskScheduler.FromCurrentSynchronizationContext());
        }

        private void InitSchoolYear()
        {
            int DefaultSchoolYear;
            if (int.TryParse(K12.Data.School.DefaultSchoolYear, out DefaultSchoolYear))
            {
                this.nudSchoolYear.Value = decimal.Parse(DefaultSchoolYear.ToString());
            }
            else
            {
                this.nudSchoolYear.Value = decimal.Parse((DateTime.Today.Year - 1911).ToString());
            }
        }

        private void InitSemester()
        {
            this.cboSemester.DataSource = DataItems.SemesterItem.GetSemesterList();
            this.cboSemester.ValueMember = "Value";
            this.cboSemester.DisplayMember = "Name";

            this.cboSemester.SelectedValue = K12.Data.School.DefaultSemester;
        }

        private void btnExit_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void MailMerge_MergeField(object sender, Aspose.Words.Reporting.MergeFieldEventArgs e)
        {
            #region 科目成績
             
            #endregion
        }

        //報表產生完成後，儲存並且開啟
        private void Completed(string inputReportName, Document inputDoc)
        {
            SaveFileDialog sd = new SaveFileDialog();
            sd.Title = "另存新檔";
            sd.FileName = inputReportName + DateTime.Now.ToString("yyyy-MM-dd_HH_mm_ss") + ".doc";
            sd.Filter = "Word檔案 (*.doc)|*.doc|所有檔案 (*.*)|*.*";
            sd.AddExtension = true;
            if (sd.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    inputDoc.Save(sd.FileName, Aspose.Words.SaveFormat.Doc);
                    System.Diagnostics.Process.Start(sd.FileName);
                }
                catch
                {
                    MsgBox.Show("指定路徑無法存取。", "建立檔案失敗", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
            }
        }

        private void btnPrint_Click(object sender, EventArgs e)
        {
        }

        private void btnSubjectSemesterScoreStatistics_Click(object sender, EventArgs e)
        {
            this.btnPrint.Enabled = false;
            int SchoolYear = int.Parse(this.nudSchoolYear.Value + "");
            int Semester = int.Parse((this.cboSemester.SelectedItem as EMBACore.DataItems.SemesterItem).Value);

            Task<Document> task = Task<Document>.Factory.StartNew(() =>
            {
                MemoryStream template = new MemoryStream(Properties.Resources.EMBA_研究生成績審核表_樣版);
                Document doc = new Document();
                doc.Sections.Clear();
                IEnumerable<StudentRecord> Students = this.dicStudents.Values.OrderBy(x => x.StudentNumber.ToLower());
                foreach (StudentRecord eachStudent in Students)
                {
                    Document eachStudentDoc = new Document(template, "", LoadFormat.Doc, "");
                    Dictionary<string, object> mergeKeyValue = new Dictionary<string, object>();

                    #region 學生基本資料
                    mergeKeyValue.Add("學號", eachStudent.StudentNumber);
                    mergeKeyValue.Add("姓名", eachStudent.Name);

                    UDT.DepartmentGroup DepartmentGroup = new UDT.DepartmentGroup();
                    if (this.dicStudentBrief2.ContainsKey(eachStudent.ID))
                    {
                        if (this.dicDepartmentGroups.ContainsKey(this.dicStudentBrief2[eachStudent.ID].DepartmentGroupID.ToString()))
                            DepartmentGroup = this.dicDepartmentGroups[this.dicStudentBrief2[eachStudent.ID].DepartmentGroupID.ToString()];
                    }
                    mergeKeyValue.Add("系所組別代碼", DepartmentGroup.Code);
                    mergeKeyValue.Add("組別", DepartmentGroup.Name);
                    #endregion

                    mergeKeyValue.Add("學年度", SchoolYear);
                    mergeKeyValue.Add("學期", Semester);

                    List<UDT.SubjectSemesterScore> SubjectSemesterScores = new List<UDT.SubjectSemesterScore>();
                    if (this.dicSubjectSemesterScores.ContainsKey(eachStudent.ID))
                        SubjectSemesterScores = this.dicSubjectSemesterScores[eachStudent.ID];

                    //mergeKeyValue.Add("抵免紀錄_上學期", SubjectSemesterScores.Where(x => !string.IsNullOrWhiteSpace(x.OffsetCourse)).Where(x => x.Semester == 1).Sum(x => x.Credit));
                    //mergeKeyValue.Add("抵免紀錄_下學期", SubjectSemesterScores.Where(x => !string.IsNullOrWhiteSpace(x.OffsetCourse)).Where(x => x.Semester == 2).Sum(x => x.Credit));
                    mergeKeyValue.Add("抵免紀錄", SubjectSemesterScores.Where(x => !string.IsNullOrWhiteSpace(x.OffsetCourse)).Sum(x => x.Credit));

                    List<UDT.SubjectSemesterScore> SubjectSemesterScores_Current_Semester = new List<UDT.SubjectSemesterScore>();
                    if (SubjectSemesterScores.Count > 0)
                        SubjectSemesterScores_Current_Semester = SubjectSemesterScores.Where(x=>string.IsNullOrEmpty(x.OffsetCourse)).Where(x => x.SchoolYear.HasValue).Where(x => x.Semester.HasValue).Where(x => (x.SchoolYear.Value == SchoolYear && x.Semester.Value == Semester)).OrderBy(x => x.SchoolYear.Value).ThenBy(x => x.Semester.Value).ToList();

                    for (int i = 1; i < 11; i++)
                    {
                        mergeKeyValue.Add("本學期必修科目_" + i, string.Empty);
                        mergeKeyValue.Add("本學期必修學分_" + i, string.Empty);
                    }
                    mergeKeyValue.Add("本學期選修總學分", string.Empty);

                    int credit_is_not_requred_total = 0;
                    int index = 0;
                    foreach (UDT.SubjectSemesterScore SubjectSemesterScore in SubjectSemesterScores_Current_Semester)
                    {
                        if (!SubjectSemesterScore.IsRequired)
                            credit_is_not_requred_total += SubjectSemesterScore.Credit;
                        else
                        {
                            index += 1;
                            if (mergeKeyValue.ContainsKey("本學期必修科目_" + index))
                                mergeKeyValue["本學期必修科目_" + index] = SubjectSemesterScore.SubjectName;

                            if (mergeKeyValue.ContainsKey("本學期必修學分_" + index))
                                mergeKeyValue["本學期必修學分_" + index] = SubjectSemesterScore.Credit;
                        }
                    }
                    mergeKeyValue["本學期選修總學分"] = credit_is_not_requred_total;

                    List<UDT.SubjectSemesterScore> SubjectSemesterScores_Not_Current_Semester = new List<UDT.SubjectSemesterScore>();
                    if (SubjectSemesterScores.Count > 0)
                        SubjectSemesterScores_Not_Current_Semester = SubjectSemesterScores.Where(x => string.IsNullOrEmpty(x.OffsetCourse)).Where(x => x.SchoolYear.HasValue).Where(x => x.Semester.HasValue).Where(x => !(x.SchoolYear.Value == SchoolYear && x.Semester.Value == Semester)).OrderBy(x => x.SchoolYear.Value).ThenBy(x => x.Semester.Value).ToList();

                    for (int i = 1; i < 8; i++)
                    {
                        mergeKeyValue.Add("修業歷程之學年_" + i, string.Empty);

                        mergeKeyValue.Add("修業歷程之學期_" + i + "_" + 0, string.Empty);
                        mergeKeyValue.Add("修業歷程之學期_" + i + "_" + 1, string.Empty);
                        mergeKeyValue.Add("修業歷程之學期_" + i + "_" + 2, string.Empty);

                        mergeKeyValue.Add("修業歷程之實得學分_" + i + "_" + 0, 0);
                        mergeKeyValue.Add("修業歷程之實得學分_" + i + "_" + 1, 0);
                        mergeKeyValue.Add("修業歷程之實得學分_" + i + "_" + 2, 0);
                    }
                    int idx = 0;
                    int school_year = 0;
                    int credit_total = 0;
                    foreach (UDT.SubjectSemesterScore SubjectSemesterScore in SubjectSemesterScores_Not_Current_Semester)
                    {
                        if (school_year != SubjectSemesterScore.SchoolYear.Value)
                        {
                            idx += 1;
                            school_year = SubjectSemesterScore.SchoolYear.Value;
                        }
                        if (mergeKeyValue.ContainsKey("修業歷程之學年_" + idx))
                            mergeKeyValue["修業歷程之學年_" + idx] = SubjectSemesterScore.SchoolYear + "學年度";

                        if (mergeKeyValue.ContainsKey("修業歷程之學期_" + idx + "_" + SubjectSemesterScore.Semester.Value))
                            mergeKeyValue["修業歷程之學期_" + idx + "_" + SubjectSemesterScore.Semester.Value] = EMBACore.DataItems.SemesterItem.GetSemesterByCode(SubjectSemesterScore.Semester + "").Name;

                        if (string.IsNullOrEmpty(SubjectSemesterScore.OffsetCourse) && !SubjectSemesterScore.IsPass)
                            continue;

                        if (mergeKeyValue.ContainsKey("修業歷程之實得學分_" + idx + "_" + SubjectSemesterScore.Semester))
                            mergeKeyValue["修業歷程之實得學分_" + idx + "_" + SubjectSemesterScore.Semester] = int.Parse(mergeKeyValue["修業歷程之實得學分_" + idx + "_" + SubjectSemesterScore.Semester] + "") + SubjectSemesterScore.Credit;

                        credit_total += SubjectSemesterScore.Credit;
                    }

                    mergeKeyValue.Add("實得總學分", credit_total + SubjectSemesterScores.Where(x => !string.IsNullOrWhiteSpace(x.OffsetCourse)).Sum(x => x.Credit));
                    mergeKeyValue.Add("應修最低畢業學分數", string.Empty);

                    if (this.dicStudentBrief2.ContainsKey(eachStudent.ID))
                    {
                        if (this.dicGraduationRequirements.ContainsKey(this.dicStudentBrief2[eachStudent.ID].GraduationRequirementID + ""))
                            mergeKeyValue["應修最低畢業學分數"] = this.dicGraduationRequirements[this.dicStudentBrief2[eachStudent.ID].GraduationRequirementID + ""].RequiredCredit;
                    }

                    DateTime print_date;
                    if (!DateTime.TryParse(this.dtDueDate.Text, out print_date))
                        print_date = DateTime.Today;

                    mergeKeyValue.Add("列印日期_年", print_date.Year - 1911);
                    mergeKeyValue.Add("列印日期_月", print_date.Month.ToString("00"));
                    mergeKeyValue.Add("列印日期_日", print_date.Day.ToString("00"));


                    eachStudentDoc.MailMerge.MergeField += new Aspose.Words.Reporting.MergeFieldEventHandler(MailMerge_MergeField);
                    eachStudentDoc.MailMerge.RemoveEmptyParagraphs = true;

                    List<string> keys = new List<string>();
                    List<object> values = new List<object>();

                    foreach (string key in mergeKeyValue.Keys)
                    {
                        keys.Add(key);
                        values.Add(mergeKeyValue[key]);
                    }
                    eachStudentDoc.MailMerge.Execute(keys.ToArray(), values.ToArray());

                    doc.Sections.Add(doc.ImportNode(eachStudentDoc.Sections[0], true));
                }
                return doc;
            });
            task.ContinueWith((x) =>
            {
                this.btnPrint.Enabled = true;

                if (x.Exception != null)
                    MessageBox.Show(x.Exception.InnerException.Message);
                else
                    Completed("畢業生成績審核表", x.Result);
            }, System.Threading.CancellationToken.None, TaskContinuationOptions.None, TaskScheduler.FromCurrentSynchronizationContext());
        }

        private List<DataBindedSheet> GetDataBindedSheets(StudentRecord Student)
        {
            UDT.StudentBrief2 StudentBrief2 = new UDT.StudentBrief2();
            if (this.dicStudentBrief2.ContainsKey(Student.ID))
                StudentBrief2 = this.dicStudentBrief2[Student.ID];

            UDT.DepartmentGroup DepartmentGroup = new UDT.DepartmentGroup();
            if (this.dicStudentBrief2.ContainsKey(Student.ID))
            {
                if (this.dicDepartmentGroups.ContainsKey(StudentBrief2.DepartmentGroupID.ToString()))
                    DepartmentGroup = this.dicDepartmentGroups[StudentBrief2.DepartmentGroupID.ToString()];
            }

            Dictionary<int, UDT.GraduationSubjectList> dicGraduationSubjectLists = new Dictionary<int, UDT.GraduationSubjectList>();
            if (this.dicStudentBrief2.ContainsKey(Student.ID))
            {
                if (this.dicGraduationSubjectLists.ContainsKey(StudentBrief2.GraduationRequirementID))
                    dicGraduationSubjectLists = this.dicGraduationSubjectLists[StudentBrief2.GraduationRequirementID];
            }

            Dictionary<string, UDT.GraduationSubjectGroupRequirement> dicGraduationSubjectGroups = new Dictionary<string, UDT.GraduationSubjectGroupRequirement>();
            if (this.dicStudentBrief2.ContainsKey(Student.ID))
            {
                if (this.dicGraduationSubjectGroupRequirements.ContainsKey(StudentBrief2.GraduationRequirementID))
                    dicGraduationSubjectGroups = this.dicGraduationSubjectGroupRequirements[StudentBrief2.GraduationRequirementID];
            }

            //GraduationSubjectList

            Workbook wb = new Workbook();
            MemoryStream ms = new MemoryStream(Properties.Resources.EMBA_歷年成績表_樣版);
            wb.Open(ms);

            List<DataBindedSheet> DataBindedSheets = new List<DataBindedSheet>();

            DataBindedSheet DataBindedSheet = new DataBindedSheet();
            DataBindedSheet.Worksheet = wb.Worksheets["PageHeader"];
            DataBindedSheet.DataTables = new List<DataTable>();
            DataBindedSheet.DataTables.Add(Student.StudentNumber.ToDataTable("學號", "學號"));
            DataBindedSheet.DataTables.Add(Student.Name.ToDataTable("姓名", "姓名"));
            DataBindedSheet.DataTables.Add(Student.Gender.ToDataTable("性別", "性別"));

            if (!Student.Birthday.HasValue)
            {
                DataBindedSheet.DataTables.Add("".ToDataTable("出生日期", "出生日期"));
            }
            else
            {
                DateTime birthday;
                if (DateTime.TryParse(Student.Birthday.Value + "", out birthday))
                    DataBindedSheet.DataTables.Add(birthday.ToString("yyyy/MM/dd").ToDataTable("出生日期", "出生日期"));
                else
                    DataBindedSheet.DataTables.Add("".ToDataTable("出生日期", "出生日期"));
            }

            DataBindedSheet.DataTables.Add(StudentBrief2.EnrollYear.ToDataTable("入學年度", "入學年度"));
            DataBindedSheet.DataTables.Add(DepartmentGroup.Name.ToDataTable("系所組別", "系所組別"));
            DataBindedSheets.Add(DataBindedSheet);

            Dictionary<string, KeyValuePair> dicSubjectGroupGredits = new Dictionary<string, KeyValuePair>();
            if (this.dicSubjectSemesterScores.ContainsKey(Student.ID))
            {
                List<UDT.SubjectSemesterScore> SubjectSemesterScores = this.dicSubjectSemesterScores[Student.ID];
                SubjectSemesterScores.ForEach((x) =>
                {
                    if (string.IsNullOrEmpty(x.NewSubjectCode))
                    {
                        if (this.dicSubjects.ContainsKey(x.SubjectID.ToString()))
                            x.NewSubjectCode = this.dicSubjects[x.SubjectID.ToString()].NewSubjectCode;
                    }
                });
                SubjectSemesterScores = SubjectSemesterScores.OrderBy(x => x.SchoolYear.HasValue ? x.SchoolYear.Value : 0).ThenBy(x => x.Semester.HasValue ? x.Semester.Value : 0).ThenBy(x => x.NewSubjectCode).ToList();

                int credit_total = 0;
                foreach (UDT.SubjectSemesterScore SubjectSemesterScore in SubjectSemesterScores)
                {
                    DataBindedSheet = new DataBindedSheet();
                    DataBindedSheet.Worksheet = wb.Worksheets["DataSection"];
                    DataBindedSheet.DataTables = new List<DataTable>();
                    DataBindedSheet.DataTables.Add((SubjectSemesterScore.SchoolYear.HasValue ? SubjectSemesterScore.SchoolYear.Value + "" : "").ToDataTable("學年度", "學年度"));
                    DataBindedSheet.DataTables.Add((SubjectSemesterScore.Semester.HasValue ? SubjectSemesterScore.Semester.Value + "" : "").ToDataTable("學期", "學期"));
                    DataBindedSheet.DataTables.Add(SubjectSemesterScore.NewSubjectCode.ToDataTable("課號", "課號"));

                    DataBindedSheet.DataTables.Add(SubjectSemesterScore.SubjectCode.ToDataTable("課程識別碼", "課程識別碼"));
                    DataBindedSheet.DataTables.Add(SubjectSemesterScore.SubjectName.ToDataTable("課程名稱", "課程名稱"));
                    DataBindedSheet.DataTables.Add(SubjectSemesterScore.Score.ToDataTable("成績", "成績"));
                    DataBindedSheet.DataTables.Add(SubjectSemesterScore.Credit.ToDataTable("學分數", "學分數"));

                    if (dicGraduationSubjectLists.ContainsKey(SubjectSemesterScore.SubjectID))
                    {
                        string SubjectGroup = dicGraduationSubjectLists[SubjectSemesterScore.SubjectID].SubjectGroup;
                        DataBindedSheet.DataTables.Add(SubjectGroup.ToDataTable("群組別", "群組別"));
                        if (!dicSubjectGroupGredits.ContainsKey(SubjectGroup))
                            dicSubjectGroupGredits.Add(SubjectGroup, new KeyValuePair());

                        if (dicGraduationSubjectGroups.ContainsKey(SubjectGroup))
                            dicSubjectGroupGredits[SubjectGroup].Key = dicGraduationSubjectGroups[SubjectGroup].LowestCredit;
                        if (SubjectSemesterScore.IsPass)
                        {
                            if (dicSubjectGroupGredits[SubjectGroup].Value == null)
                                dicSubjectGroupGredits[SubjectGroup].Value = SubjectSemesterScore.Credit;
                            else
                                dicSubjectGroupGredits[SubjectGroup].Value += SubjectSemesterScore.Credit;
                        }
                    }
                    else
                        DataBindedSheet.DataTables.Add("".ToDataTable("群組別", "群組別"));

                    if (!string.IsNullOrEmpty(SubjectSemesterScore.OffsetCourse) || SubjectSemesterScore.IsPass)
                    {
                        DataBindedSheet.DataTables.Add("已取得學分".ToDataTable("備註", "備註"));
                    }
                    else
                        DataBindedSheet.DataTables.Add("未取得學分".ToDataTable("備註", "備註"));

                    if (SubjectSemesterScore.IsPass && string.IsNullOrEmpty(SubjectSemesterScore.OffsetCourse))
                        credit_total += SubjectSemesterScore.Credit;

                    DataBindedSheets.Add(DataBindedSheet);
                }

                int offset_credit = SubjectSemesterScores.Where(x => !string.IsNullOrWhiteSpace(x.OffsetCourse)).Sum(x => x.Credit);
                DataBindedSheet = new DataBindedSheet();
                DataBindedSheet.Worksheet = wb.Worksheets["PageFooter-Header"];
                DataBindedSheet.DataTables = new List<DataTable>();
                DataBindedSheet.DataTables.Add(credit_total.ToDataTable("修習及格學分", "修習及格學分"));
                DataBindedSheet.DataTables.Add(offset_credit.ToDataTable("抵免學分", "抵免學分"));
                DataBindedSheet.DataTables.Add((credit_total + offset_credit).ToDataTable("實得總學分", "實得總學分"));
                DataBindedSheets.Add(DataBindedSheet);

                List<UDT.GraduationSubjectGroupRequirement> GraduationSubjectGroupRequirements = dicGraduationSubjectGroups.Values.ToList();
                GraduationSubjectGroupRequirements.Sort(
                delegate(UDT.GraduationSubjectGroupRequirement x, UDT.GraduationSubjectGroupRequirement y)
                {
                    int index_x = SubjectGroups_Sort.IndexOf(x.SubjectGroup);
                    int index_y = SubjectGroups_Sort.IndexOf(y.SubjectGroup);
                    if (index_x < 0)
                        index_x = int.MaxValue;
                    if (index_y < 0)
                        index_y = int.MaxValue;

                    return index_x.CompareTo(index_y);
                });

                foreach (UDT.GraduationSubjectGroupRequirement GraduationSubjectGroupRequirement in GraduationSubjectGroupRequirements)
                {
                    string SubjectGroup = GraduationSubjectGroupRequirement.SubjectGroup;
                    DataBindedSheet = new DataBindedSheet();
                    DataBindedSheet.Worksheet = wb.Worksheets["PageFooter-DataSection"];
                    DataBindedSheet.DataTables = new List<DataTable>();
                    DataBindedSheet.DataTables.Add(SubjectGroup.ToDataTable("群組別", "群組別"));

                    int gCredit = 0;

                    if (dicSubjectGroupGredits.ContainsKey(SubjectGroup))
                        if (dicSubjectGroupGredits[SubjectGroup].Value.HasValue)
                            gCredit = dicSubjectGroupGredits[SubjectGroup].Value.Value;

                    DataBindedSheet.DataTables.Add((gCredit + "").ToDataTable("已取得學分數", "已取得學分數"));

                    int sCredit = GraduationSubjectGroupRequirement.LowestCredit;
                    if (dicSubjectGroupGredits.ContainsKey(SubjectGroup))
                    {
                        if (sCredit > gCredit)
                            DataBindedSheet.DataTables.Add((sCredit - gCredit).ToDataTable("不足學分數", "不足學分數"));
                        else
                            DataBindedSheet.DataTables.Add("0".ToDataTable("不足學分數", "不足學分數"));
                    }
                    else
                    {
                        DataBindedSheet.DataTables.Add(sCredit.ToDataTable("不足學分數", "不足學分數"));
                    }
                    DataBindedSheet.DataTables.Add(sCredit.ToDataTable("應修學分數", "應修學分數"));

                    DataBindedSheets.Add(DataBindedSheet);
                }

                DataBindedSheet = new DataBindedSheet();
                DataBindedSheet.Worksheet = wb.Worksheets["PageFooter-Footer"];
                DataBindedSheet.DataTables = new List<DataTable>();
                DataBindedSheets.Add(DataBindedSheet);
            }

            return DataBindedSheets;
        }

        private Workbook GenerateWorkbook(StudentRecord Student)
        {
            Workbook workbook = new Workbook();
            workbook.Worksheets.Cast<Worksheet>().ToList().ForEach(x => workbook.Worksheets.RemoveAt(x.Name));

            List<DataBindedSheet> TemplateSheets = this.GetDataBindedSheets(Student);

            int instanceSheetIndex = workbook.Worksheets.Add();
            workbook.Worksheets[instanceSheetIndex].Name = Student.StudentNumber + "-" + Student.Name;
                //workbook.Worksheets.AddCopy(TemplateSheets.ElementAt(0).Worksheet.Name);   
            workbook.Worksheets[instanceSheetIndex].Copy(TemplateSheets.ElementAt(0).Worksheet);
            Worksheet instanceSheet = workbook.Worksheets[instanceSheetIndex];

            int i = 0;
            DataSet dataSet = new DataSet();
            dataSet.Tables.AddRange(TemplateSheets.ElementAt(0).DataTables.ToArray());
            DocumentHelper.GenerateSheet(dataSet, instanceSheet, i, null);
            if (TemplateSheets.Count > 1)
            {
                TemplateSheets.RemoveAt(0);
                foreach (DataBindedSheet sheet in TemplateSheets)
                {
                    dataSet = new DataSet();
                    dataSet.Tables.AddRange(sheet.DataTables.ToArray());
                    i = instanceSheet.Cells.MaxRow + 1;
                    DocumentHelper.CloneTemplate(instanceSheet, sheet.Worksheet, i);
                    DocumentHelper.GenerateSheet(dataSet, instanceSheet, i, null);
                }
            }

            //  移除樣版檔
            TemplateSheets.ForEach(x => workbook.Worksheets.RemoveAt(x.Worksheet.Name));

            //  移除報表中的變數
            DocumentHelper.RemoveReportVariable(workbook);

            // 置換工作表名稱中的保留字
            workbook.Worksheets.Cast<Worksheet>().ToList().ForEach((x) =>
            {
                x.Name = x.Name.Replace("：", "꞉").Replace(":", "꞉").Replace("/", "⁄").Replace("／", "⁄").Replace(@"\", "∖").Replace("＼", "∖").Replace("?", "_").Replace("？", "_").Replace("*", "✻").Replace("＊", "✻").Replace("[", "〔").Replace("〔", "〔").Replace("]", "〕").Replace("〕", "〕");
            });
            return workbook;

        }

        private void btnSubjectSemesterScoreHistory_Click(object sender, EventArgs e)
        {
            this.circularProgress.Visible = true;
            this.circularProgress.IsRunning = true;
            this.btnPrint.Enabled = false;
            Task<Workbook> task = Task<Workbook>.Factory.StartNew(() =>
            {
                Workbook new_workbook = new Workbook();
                foreach (string key in this.dicStudents.Keys)
                {
                    Workbook wb = this.GenerateWorkbook(this.dicStudents[key]);
                    new_workbook.Combine(wb);
                    new_workbook.Worksheets.Cast<Worksheet>().ToList().ForEach((x) =>
                    {
                        if (x.Cells.MaxDataColumn == 0 && x.Cells.MaxDataRow == 0)
                            new_workbook.Worksheets.RemoveAt(x.Index);
                    });
                }
                return new_workbook;
            });

            task.ContinueWith((x) =>
            {
                this.circularProgress.Visible = false;
                this.circularProgress.IsRunning = false;
                this.btnPrint.Enabled = true;
                if (x.Exception != null)
                {
                    MessageBox.Show(x.Exception.InnerException.Message);
                    return;
                }

                System.Windows.Forms.SaveFileDialog sd = new System.Windows.Forms.SaveFileDialog();
                sd.Title = "另存新檔";
                sd.FileName = "歷年成績表" + DateTime.Now.ToString(" yyyy-MM-dd_HH_mm_ss") + ".xls";
                sd.Filter = "Excel檔案 (*.xls)|*.xls|所有檔案 (*.*)|*.*";
                sd.AddExtension = true;
                if (sd.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                {
                    try
                    {
                        x.Result.Save(sd.FileName, FileFormatType.Excel2003);
                        System.Diagnostics.Process.Start(sd.FileName);
                    }
                    catch
                    {
                        FISCA.Presentation.Controls.MsgBox.Show("指定路徑無法存取。", "建立檔案失敗", System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Error);
                        return;
                    }
                }
            }, System.Threading.CancellationToken.None, TaskContinuationOptions.None, TaskScheduler.FromCurrentSynchronizationContext());
        }
    }

    public class DataBindedSheet
    {
        public Worksheet Worksheet { get; set; }
        public List<DataTable> DataTables { get; set; }
    }

    public class KeyValuePair
    {
        public int? Key { get; set; }
        public int? Value { get; set; }
    }
}
//PageHeader：學號、姓名、性別、出生日期、入學年度、系所組別
//DataSection：學年度、學期、課號、課程識別碼、課程名稱、成績、學分、群組別、備註
//PageFooter-Header
//PageFooter-DataSection：群組別、應修學分數、已取得學分數、不足學分數
//PageFooter-Footer：修習及格學分、抵免學分、實得總學分