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
using System.Dynamic;

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
            #region 本學期應修及格方可畢業之科目及學分
            //13字數12號，14字數11號，15字數10號
            if (e.FieldName.IndexOf("本學期必修科目") >= 0)
            {
                DocumentBuilder builder = new DocumentBuilder(e.Document);
                builder.MoveToField(e.Field, true);
                string dn = e.FieldValue + "";
                e.Field.Remove();

                //builder.Font.Size = (dn.Length <= 10) ? 12 : (12 - (dn.Length - 10));
                Aspose.Words.Cell SCell = (Aspose.Words.Cell)builder.CurrentParagraph.ParentNode;
                Paragraph p = SCell.FirstParagraph;
                var par = SCell.FirstParagraph;
                var run = new Run(e.Document);
                run.Text = dn;
                run.Font.Name = "標楷體";                
                if (dn.Length > 10)
                {
                    par.ParagraphFormat.Alignment = ParagraphAlignment.Distributed;
                    run.Font.Size = (dn.Length <= 13) ? 12 : (12 - (dn.Length - 13));
                }
                par.Runs.Add(run);

                //  以下的寫法皆為無效
                //SCell.CellFormat.ClearFormatting();
                //Aspose.Words.Style style = null;
                //if (e.Document.Styles["MyStyle1"] == null)
                //    style = e.Document.Styles.Add(StyleType.Paragraph, "MyStyle1");
                //else
                //    style = e.Document.Styles["MyStyle1"];
                //style.ParagraphFormat.Alignment = ParagraphAlignment.Distributed;
                //style.ParagraphFormat.KeepTogether = true;

                //p.ParagraphFormat.Style = style;
                //builder.Write(e.FieldValue + "");
                
              
                //string dn = e.FieldValue as string;
                //builder.Write(dn);
                //SCell.FirstParagraph.ParagraphFormat.Alignment = ParagraphAlignment.Distributed | ParagraphAlignment.Left;
                //SCell.CellFormat.WrapText = false;
                //builder.CellFormat.FitText = true;
                //builder.CellFormat.WrapText = false;
                //SCell.CellFormat.Shading.BackgroundPatternColor = System.Drawing.Color.FromArgb(198, 217, 241);
            }
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
            this.circularProgress.Visible = true;
            this.circularProgress.IsRunning = true;
            this.btnPrint.Enabled = false;
            int SchoolYear = int.Parse(this.nudSchoolYear.Value + "");
            int Semester = int.Parse((this.cboSemester.SelectedItem as EMBACore.DataItems.SemesterItem).Value);

            Task<Document> task = Task<Document>.Factory.StartNew(() =>
            {
                MemoryStream template = new MemoryStream(Properties.Resources.EMBA_研究生成績審核表_樣版);
                Document doc = new Document();
                doc.Sections.Clear();
                IEnumerable<StudentRecord> Students = this.dicStudents.Values.OrderBy(x => x.StudentNumber.ToLower());
                foreach (StudentRecord Student in Students)
                {
                    UDT.StudentBrief2 StudentBrief2 = new UDT.StudentBrief2();
                    if (this.dicStudentBrief2.ContainsKey(Student.ID))
                        StudentBrief2 = this.dicStudentBrief2[Student.ID];

                    Dictionary<int, UDT.GraduationSubjectList> dicGraduationSubjectLists = new Dictionary<int, UDT.GraduationSubjectList>();
                    if (this.dicStudentBrief2.ContainsKey(Student.ID))
                    {
                        if (this.dicGraduationSubjectLists.ContainsKey(StudentBrief2.GraduationRequirementID))
                            dicGraduationSubjectLists = this.dicGraduationSubjectLists[StudentBrief2.GraduationRequirementID];
                    }
                    //  非停修生修課記錄：學年度	學期	課號	課程識別碼		課程名稱
                    string SQL = string.Format(@"Select subject.uid as subject_id, course.school_year, course.semester, subject.name as subject_name From $ischool.emba.scattend_ext as se join course on course.id=se.ref_course_id
join student on student.id=se.ref_student_id
join $ischool.emba.course_ext as ce on ce.ref_course_id=course.id
join $ischool.emba.subject as subject on subject.uid=ce.ref_subject_id where student.id={0} and course.school_year={1} and course.semester={2} order by ce.serial_no", Student.ID, SchoolYear, Semester);
                    DataTable dataTable = Query.Select(SQL);
                    Dictionary<string, dynamic> dicSCAttends = new Dictionary<string, dynamic>();
                    foreach (DataRow row in dataTable.Rows)
                    {
                        dynamic o = new ExpandoObject();

                        o.SchoolYear = SchoolYear;
                        o.Semester = Semester;
                        o.SubjectName = row["subject_name"] + "";
                        o.SubjectID = row["subject_id"] + "";

                        dicSCAttends.Add(SchoolYear + "-" + Semester + "-" + row["subject_id"] + "", o);
                    }

                    Document eachStudentDoc = new Document(template, "", LoadFormat.Doc, "");
                    Dictionary<string, object> mergeKeyValue = new Dictionary<string, object>();

                    #region 學生基本資料
                    mergeKeyValue.Add("學號", Student.StudentNumber);
                    mergeKeyValue.Add("姓名", Student.Name);

                    UDT.DepartmentGroup DepartmentGroup = new UDT.DepartmentGroup();
                    if (this.dicStudentBrief2.ContainsKey(Student.ID))
                    {
                        if (this.dicDepartmentGroups.ContainsKey(this.dicStudentBrief2[Student.ID].DepartmentGroupID.ToString()))
                            DepartmentGroup = this.dicDepartmentGroups[this.dicStudentBrief2[Student.ID].DepartmentGroupID.ToString()];
                    }
                    mergeKeyValue.Add("系所組別代碼", DepartmentGroup.Code);
                    mergeKeyValue.Add("組別", DepartmentGroup.Name);
                    #endregion

                    mergeKeyValue.Add("學年度", SchoolYear);
                    mergeKeyValue.Add("學期", Semester);

                    List<UDT.SubjectSemesterScore> SubjectSemesterScores = new List<UDT.SubjectSemesterScore>();
                    if (this.dicSubjectSemesterScores.ContainsKey(Student.ID))
                        SubjectSemesterScores = this.dicSubjectSemesterScores[Student.ID];

                    //  重覆修課                    
                    Dictionary<int, List<UDT.SubjectSemesterScore>> dicDuplicateSubjectSemesterScores = new Dictionary<int, List<UDT.SubjectSemesterScore>>();
                    foreach (UDT.SubjectSemesterScore SubjectSemesterScore in SubjectSemesterScores)
                    {
                        //  是否重覆修課
                        if (!dicDuplicateSubjectSemesterScores.ContainsKey(SubjectSemesterScore.SubjectID))
                            dicDuplicateSubjectSemesterScores.Add(SubjectSemesterScore.SubjectID, new List<UDT.SubjectSemesterScore>());
                        dicDuplicateSubjectSemesterScores[SubjectSemesterScore.SubjectID].Add(SubjectSemesterScore);
                    }

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
                    List<dynamic> present_subject_lists = new List<dynamic>();
                    foreach (string key in dicSCAttends.Keys)
                    {
                        dynamic o = dicSCAttends[key];
                        string SubjectName = o.SubjectName + "";

                        int SubjectID = int.Parse(o.SubjectID + "");

                        dynamic oo = new ExpandoObject();

                        oo.IsDeptRequired = false;
                        oo.SubjectName = SubjectName;
                        oo.SubjectID = SubjectID;
                        oo.SubjectGroup = string.Empty;

                        if (dicGraduationSubjectLists.ContainsKey(SubjectID))
                        {
                            UDT.GraduationSubjectList GraduationSubjectList = dicGraduationSubjectLists[SubjectID];
                            oo.SubjectGroup = GraduationSubjectList.SubjectGroup;
                            if (string.IsNullOrEmpty(GraduationSubjectList.SubjectGroup))
                                oo.IsRequired = false;
                            else
                                oo.IsRequired = true;
                            if (GraduationSubjectList.IsDeptRequired)
                            {
                                oo.IsDeptRequired = true;
                            }
                        }
                        else
                        {
                            oo.IsRequired = false;
                        }
                        present_subject_lists.Add(oo);
                    }

                    int credit_is_not_requred_total = 0;
                    int credit_current = 0;
                    int index = 0;
                    foreach (dynamic o in present_subject_lists.Where(x => x.IsRequired == false))
                    {
                        string SubjectID = o.SubjectID + "";
                        if (this.dicSubjects.ContainsKey(SubjectID))
                        {
                            credit_is_not_requred_total += this.dicSubjects[SubjectID].Credit;
                        }
                    }
                    foreach(dynamic o in present_subject_lists.Where(x=>x.IsDeptRequired == true))
                    {
                        index += 1;
                        if (mergeKeyValue.ContainsKey("本學期必修科目_" + index))
                            mergeKeyValue["本學期必修科目_" + index] = o.SubjectName + "";

                        string SubjectID = o.SubjectID + "";
                        if (this.dicSubjects.ContainsKey(SubjectID))
                        {
                            if (mergeKeyValue.ContainsKey("本學期必修學分_" + index))
                                mergeKeyValue["本學期必修學分_" + index] = this.dicSubjects[SubjectID].Credit;

                            credit_current += this.dicSubjects[SubjectID].Credit;
                        }
                    }
                    var group_subjects = present_subject_lists.Where(x => x.IsDeptRequired == false).Where(x => !string.IsNullOrEmpty(x.SubjectGroup + "")).GroupBy(x=>x.SubjectGroup);
                    foreach (var groupOfStudents in group_subjects)
                    {
                        index += 1;
                        if (mergeKeyValue.ContainsKey("本學期必修科目_" + index))
                            mergeKeyValue["本學期必修科目_" + index] = string.Join("或", groupOfStudents.Select(x=>x.SubjectName + ""));

                        List<int> Credits = new List<int>();
                        foreach(var g in groupOfStudents)
                        {
                            string SubjectID = g.SubjectID + "";

                            if (this.dicSubjects.ContainsKey(SubjectID))
                            {
                                if (!Credits.Contains(this.dicSubjects[SubjectID].Credit))
                                {
                                    Credits.Add(this.dicSubjects[SubjectID].Credit); 
                                    credit_current += this.dicSubjects[SubjectID].Credit;
                                }
                            }
                        } 
                        if (mergeKeyValue.ContainsKey("本學期必修學分_" + index))
                            mergeKeyValue["本學期必修學分_" + index] = string.Join("或", Credits);
                    }


                   
                    //foreach (UDT.SubjectSemesterScore SubjectSemesterScore in SubjectSemesterScores_Current_Semester)
                    //{
                    //    if (!SubjectSemesterScore.IsRequired)
                    //        credit_is_not_requred_total += SubjectSemesterScore.Credit;
                    //    else
                    //    {
                    //        index += 1;
                    //        if (mergeKeyValue.ContainsKey("本學期必修科目_" + index))
                    //            mergeKeyValue["本學期必修科目_" + index] = SubjectSemesterScore.SubjectName;

                    //        if (mergeKeyValue.ContainsKey("本學期必修學分_" + index))
                    //            mergeKeyValue["本學期必修學分_" + index] = SubjectSemesterScore.Credit;
                    //    }
                    //}

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

                        mergeKeyValue.Add("畢業修業歷程之實得學分_" + i + "_" + 0, 0);
                        mergeKeyValue.Add("畢業修業歷程之實得學分_" + i + "_" + 1, 0);
                        mergeKeyValue.Add("畢業修業歷程之實得學分_" + i + "_" + 2, 0);

                        mergeKeyValue.Add("不計入畢業學分_" + i + "_" + 0, 0);
                        mergeKeyValue.Add("不計入畢業學分_" + i + "_" + 1, 0);
                        mergeKeyValue.Add("不計入畢業學分_" + i + "_" + 2, 0);
                    }
                    int idx = 0;
                    int school_year = 0;
                    int credit_total = 0;
                    Dictionary<int, int> dicSchoolYearMappings = new Dictionary<int, int>();
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

                        if (mergeKeyValue.ContainsKey("畢業修業歷程之實得學分_" + idx + "_" + SubjectSemesterScore.Semester))
                            mergeKeyValue["畢業修業歷程之實得學分_" + idx + "_" + SubjectSemesterScore.Semester] = int.Parse(mergeKeyValue["畢業修業歷程之實得學分_" + idx + "_" + SubjectSemesterScore.Semester] + "") + SubjectSemesterScore.Credit;

                        credit_total += SubjectSemesterScore.Credit;

                        if (!dicSchoolYearMappings.ContainsKey(school_year))
                            dicSchoolYearMappings.Add(school_year, idx);
                    }

                    mergeKeyValue.Add("實得總學分", credit_total + SubjectSemesterScores.Where(x => !string.IsNullOrWhiteSpace(x.OffsetCourse)).Sum(x => x.Credit));
                    mergeKeyValue.Add("畢業實得總學分", credit_total + SubjectSemesterScores.Where(x => !string.IsNullOrWhiteSpace(x.OffsetCourse)).Sum(x => x.Credit));
                    mergeKeyValue.Add("應修最低畢業學分數", string.Empty);

                    if (this.dicStudentBrief2.ContainsKey(Student.ID))
                    {
                        if (this.dicGraduationRequirements.ContainsKey(this.dicStudentBrief2[Student.ID].GraduationRequirementID + ""))
                            mergeKeyValue["應修最低畢業學分數"] = this.dicGraduationRequirements[this.dicStudentBrief2[Student.ID].GraduationRequirementID + ""].RequiredCredit;
                    }

                    DateTime print_date;
                    if (!DateTime.TryParse(this.dtDueDate.Text, out print_date))
                        print_date = DateTime.Today;

                    mergeKeyValue.Add("列印日期_年", print_date.Year - 1911);
                    mergeKeyValue.Add("列印日期_月", print_date.Month.ToString("00"));
                    mergeKeyValue.Add("列印日期_日", print_date.Day.ToString("00"));
                    
                    //  重覆修課
                    int total_credit_reduce = 0;
                    foreach (int SubjectID in dicDuplicateSubjectSemesterScores.Keys)
                    {
                        if (dicDuplicateSubjectSemesterScores[SubjectID].Count < 2)
                            continue;

                        List<UDT.SubjectSemesterScore> DuplicateSubjectSemesterScores = dicDuplicateSubjectSemesterScores[SubjectID];
                        bool init = false;
                        foreach (UDT.SubjectSemesterScore SubjectSemesterScore in DuplicateSubjectSemesterScores.OrderBy(x=>(x.SchoolYear.HasValue ? x.SchoolYear.Value : 0)).ThenBy(x=>(x.Semester.HasValue ? x.Semester.Value : 0)))
                        {
                            if (init)
                            {
                                if (dicSchoolYearMappings.ContainsKey((SubjectSemesterScore.SchoolYear.HasValue ? SubjectSemesterScore.SchoolYear.Value : 0)))
                                {
                                    mergeKeyValue["不計入畢業學分_" + dicSchoolYearMappings[(SubjectSemesterScore.SchoolYear.HasValue ? SubjectSemesterScore.SchoolYear.Value : 0)] + "_" + (SubjectSemesterScore.Semester.HasValue ? SubjectSemesterScore.Semester.Value : 0)] = SubjectSemesterScore.Credit;
                                    total_credit_reduce += SubjectSemesterScore.Credit;
                                    int oCredit = int.Parse(mergeKeyValue["畢業修業歷程之實得學分_" + dicSchoolYearMappings[(SubjectSemesterScore.SchoolYear.HasValue ? SubjectSemesterScore.SchoolYear.Value : 0)] + "_" + (SubjectSemesterScore.Semester.HasValue ? SubjectSemesterScore.Semester.Value : 0)] + "");
                                    mergeKeyValue["畢業修業歷程之實得學分_" + dicSchoolYearMappings[(SubjectSemesterScore.SchoolYear.HasValue ? SubjectSemesterScore.SchoolYear.Value : 0)] + "_" + (SubjectSemesterScore.Semester.HasValue ? SubjectSemesterScore.Semester.Value : 0)] = oCredit - SubjectSemesterScore.Credit;
                                }
                            }

                            init = true;
                        }
                    }
                    mergeKeyValue.Add("不計入畢業總學分", total_credit_reduce);
                    mergeKeyValue["畢業實得總學分"] = int.Parse(mergeKeyValue["畢業實得總學分"] + "") - total_credit_reduce;

                    int lowest_credit = 0;
                    int.TryParse(mergeKeyValue["應修最低畢業學分數"] + "", out lowest_credit);
                    int credit_differ = lowest_credit - int.Parse(mergeKeyValue["畢業實得總學分"] + "");
                    if (credit_differ > 0)
                        mergeKeyValue["本學期選修總學分"] = credit_differ - credit_current; // credit_is_not_requred_total;
                    else
                        mergeKeyValue["本學期選修總學分"] = 0;

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
                this.circularProgress.Visible = false;
                this.circularProgress.IsRunning = false;
                this.btnPrint.Enabled = true;

                if (x.Exception != null)
                    MessageBox.Show(x.Exception.InnerException.Message);
                else
                    Completed("畢業生成績審核表", x.Result);
            }, System.Threading.CancellationToken.None, TaskContinuationOptions.None, TaskScheduler.FromCurrentSynchronizationContext());
        }

        private List<DataBindedSheet> GetDataBindedSheets(StudentRecord Student, int SchoolYear, int Semester)
        {
            //  非停修生修課記錄：學年度	學期	課號	課程識別碼		課程名稱
            string SQL = string.Format(@"Select subject.uid as subject_id, course.school_year, course.semester, subject.new_subject_code, subject.subject_code, subject.name as subject_name From $ischool.emba.scattend_ext as se join course on course.id=se.ref_course_id
join student on student.id=se.ref_student_id
join $ischool.emba.course_ext as ce on ce.ref_course_id=course.id
join $ischool.emba.subject as subject on subject.uid=ce.ref_subject_id where student.id={0} and course.school_year={1} and course.semester={2} order by ce.serial_no", Student.ID, SchoolYear, Semester);
            DataTable dataTable = Query.Select(SQL);
            Dictionary<string, dynamic> dicSCAttends = new Dictionary<string, dynamic>();
            foreach(DataRow row in dataTable.Rows)
            {
                dynamic o = new ExpandoObject();

                o.SchoolYear = SchoolYear;
                o.Semester = Semester;
                o.NewSubjectCode = row["new_subject_code"] + "";
                o.SubjectCode = row["subject_code"] + "";
                o.SubjectName = row["subject_name"] + "";
                o.SubjectID = row["subject_id"] + "";

                dicSCAttends.Add(SchoolYear + "-" + Semester + "-" + row["subject_id"] + "", o);
            }

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
                Dictionary<int, List<UDT.SubjectSemesterScore>> dicDuplicateSubjectSemesterScores = new Dictionary<int, List<UDT.SubjectSemesterScore>>();
                List<string> present_subject_ids = new List<string>();
                foreach (UDT.SubjectSemesterScore SubjectSemesterScore in SubjectSemesterScores)
                {
                    //  是否重覆修課
                    if (!dicDuplicateSubjectSemesterScores.ContainsKey(SubjectSemesterScore.SubjectID))
                        dicDuplicateSubjectSemesterScores.Add(SubjectSemesterScore.SubjectID, new List<UDT.SubjectSemesterScore>());
                    dicDuplicateSubjectSemesterScores[SubjectSemesterScore.SubjectID].Add(SubjectSemesterScore);

                    DataBindedSheet = new DataBindedSheet();
                    DataBindedSheet.Worksheet = wb.Worksheets["DataSection"];
                    DataBindedSheet.DataTables = new List<DataTable>();
                    DataBindedSheet.DataTables.Add((SubjectSemesterScore.SchoolYear.HasValue ? SubjectSemesterScore.SchoolYear.Value + "" : "").ToDataTable("學年度", "學年度"));
                    DataBindedSheet.DataTables.Add((SubjectSemesterScore.Semester.HasValue ? SubjectSemesterScore.Semester.Value + "" : "").ToDataTable("學期", "學期"));
                    DataBindedSheet.DataTables.Add(SubjectSemesterScore.NewSubjectCode.ToDataTable("課號", "課號"));

                    DataBindedSheet.DataTables.Add(SubjectSemesterScore.SubjectCode.ToDataTable("課程識別碼", "課程識別碼"));
                    DataBindedSheet.DataTables.Add(SubjectSemesterScore.SubjectName.ToDataTable("課程名稱", "課程名稱"));
                    DataBindedSheet.DataTables.Add(SubjectSemesterScore.Score.ToDataTable("成績", "成績"));

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
                        if (this.dicSubjects.ContainsKey(SubjectSemesterScore.SubjectID.ToString()))
                            DataBindedSheet.DataTables.Add(this.dicSubjects[SubjectSemesterScore.SubjectID.ToString()].Credit.ToDataTable("學分數", "學分數"));
                        else
                            DataBindedSheet.DataTables.Add(SubjectSemesterScore.Credit.ToDataTable("學分數", "學分數"));
                    }
                    else
                    {
                        DataBindedSheet.DataTables.Add("".ToDataTable("群組別", "群組別"));
                        DataBindedSheet.DataTables.Add(SubjectSemesterScore.Credit.ToDataTable("學分數", "學分數"));
                    }

                    if (!string.IsNullOrEmpty(SubjectSemesterScore.OffsetCourse) || SubjectSemesterScore.IsPass)
                    {
                        DataBindedSheet.DataTables.Add("已取得學分".ToDataTable("備註", "備註"));
                    }
                    else
                        DataBindedSheet.DataTables.Add("未取得學分".ToDataTable("備註", "備註"));

                    if (SubjectSemesterScore.IsPass && string.IsNullOrEmpty(SubjectSemesterScore.OffsetCourse))
                        credit_total += SubjectSemesterScore.Credit;

                    DataBindedSheets.Add(DataBindedSheet);
                    present_subject_ids.Add(string.Format("{0}-{1}-{2}", (SubjectSemesterScore.SchoolYear.HasValue ? SubjectSemesterScore.SchoolYear.Value + "" : ""), (SubjectSemesterScore.Semester.HasValue ? SubjectSemesterScore.Semester.Value + "" : ""), SubjectSemesterScore.SubjectID));
                }
                foreach(string key in dicSCAttends.Keys)
                {
                    if (present_subject_ids.Contains(key))
                        continue;
                    
                    dynamic o = dicSCAttends[key];

                    DataBindedSheet = new DataBindedSheet();
                    DataBindedSheet.Worksheet = wb.Worksheets["DataSection"];
                    DataBindedSheet.DataTables = new List<DataTable>();

                    int intSchoolYear = 0;
                    int intSemester = 0;
                    int.TryParse(o.SchoolYear + "", out intSchoolYear);
                    int.TryParse(o.Semester + "", out intSemester);
                    string NewSubjectCode = o.NewSubjectCode + "";
                    string SubjectCode = o.SubjectCode + "";
                    string SubjectName = o.SubjectName + "";
                    DataBindedSheet.DataTables.Add(intSchoolYear.ToDataTable("學年度", "學年度"));
                    DataBindedSheet.DataTables.Add(intSemester.ToDataTable("學期", "學期"));
                    DataBindedSheet.DataTables.Add(NewSubjectCode.ToDataTable("課號", "課號"));
                    DataBindedSheet.DataTables.Add(SubjectCode.ToDataTable("課程識別碼", "課程識別碼"));
                    DataBindedSheet.DataTables.Add(SubjectName.ToDataTable("課程名稱", "課程名稱"));
                    DataBindedSheet.DataTables.Add("".ToDataTable("成績", "成績"));
                    DataBindedSheet.DataTables.Add("".ToDataTable("學分數", "學分數"));

                    int SubjectID = 0;
                    int.TryParse(o.SubjectID + "", out SubjectID);
                    if (dicGraduationSubjectLists.ContainsKey(SubjectID))
                    {
                        string SubjectGroup = dicGraduationSubjectLists[SubjectID].SubjectGroup;
                        DataBindedSheet.DataTables.Add(SubjectGroup.ToDataTable("群組別", "群組別"));
                        if (!dicSubjectGroupGredits.ContainsKey(SubjectGroup))
                            dicSubjectGroupGredits.Add(SubjectGroup, new KeyValuePair());

                        if (dicGraduationSubjectGroups.ContainsKey(SubjectGroup))
                            dicSubjectGroupGredits[SubjectGroup].Key = dicGraduationSubjectGroups[SubjectGroup].LowestCredit;
                    }
                    else
                    {
                        DataBindedSheet.DataTables.Add("".ToDataTable("群組別", "群組別"));
                    }
                    DataBindedSheet.DataTables.Add("本學期修課".ToDataTable("備註", "備註"));

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

                //  重覆修課
                List<UDT.SubjectSemesterScore> DuplicateSubjectSemesterScores = new List<UDT.SubjectSemesterScore>();
                foreach (int SubjectID in dicDuplicateSubjectSemesterScores.Keys)
                {
                    if (dicDuplicateSubjectSemesterScores[SubjectID].Count > 1)
                        DuplicateSubjectSemesterScores.AddRange(dicDuplicateSubjectSemesterScores[SubjectID]);
                }

                if (DuplicateSubjectSemesterScores.Count > 1)
                {
                    DataBindedSheet = new DataBindedSheet();
                    DataBindedSheet.Worksheet = wb.Worksheets["Duplicate-Header"];
                    DataBindedSheet.DataTables = new List<DataTable>();
                    DataBindedSheets.Add(DataBindedSheet);
                    foreach (UDT.SubjectSemesterScore SubjectSemesterScore in DuplicateSubjectSemesterScores)
                    {
                        DataBindedSheet = new DataBindedSheet();
                        DataBindedSheet.Worksheet = wb.Worksheets["Duplicate-DataSection"];
                        DataBindedSheet.DataTables = new List<DataTable>();
                        DataBindedSheet.DataTables.Add((SubjectSemesterScore.SchoolYear.HasValue ? SubjectSemesterScore.SchoolYear.Value + "" : "").ToDataTable("學年度", "學年度"));
                        DataBindedSheet.DataTables.Add((SubjectSemesterScore.Semester.HasValue ? SubjectSemesterScore.Semester.Value + "" : "").ToDataTable("學期", "學期"));
                        DataBindedSheet.DataTables.Add(SubjectSemesterScore.NewSubjectCode.ToDataTable("課號", "課號"));

                        DataBindedSheet.DataTables.Add(SubjectSemesterScore.SubjectCode.ToDataTable("課程識別碼", "課程識別碼"));
                        DataBindedSheet.DataTables.Add(SubjectSemesterScore.SubjectName.ToDataTable("課程名稱", "課程名稱"));
                        DataBindedSheet.DataTables.Add(SubjectSemesterScore.Score.ToDataTable("成績", "成績"));
                        //DataBindedSheet.DataTables.Add(SubjectSemesterScore.Credit.ToDataTable("學分數", "學分數"));

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
                            if (this.dicSubjects.ContainsKey(SubjectSemesterScore.SubjectID.ToString()))
                                DataBindedSheet.DataTables.Add(this.dicSubjects[SubjectSemesterScore.SubjectID.ToString()].Credit.ToDataTable("學分數", "學分數"));
                            else
                                DataBindedSheet.DataTables.Add(SubjectSemesterScore.Credit.ToDataTable("學分數", "學分數"));
                        }
                        else
                        {
                            DataBindedSheet.DataTables.Add("".ToDataTable("群組別", "群組別"));
                            DataBindedSheet.DataTables.Add(SubjectSemesterScore.Credit.ToDataTable("學分數", "學分數"));
                        }

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
                    DataBindedSheet = new DataBindedSheet();
                    DataBindedSheet.Worksheet = wb.Worksheets["Duplicate-Footer"];
                    DataBindedSheet.DataTables = new List<DataTable>();
                    DataBindedSheets.Add(DataBindedSheet);
                }
            }

            return DataBindedSheets;
        }

        private Workbook GenerateWorkbook(StudentRecord Student, int SchoolYear, int Semester)
        {
            Workbook workbook = new Workbook();
            workbook.Worksheets.Cast<Worksheet>().ToList().ForEach(x => workbook.Worksheets.RemoveAt(x.Name));

            List<DataBindedSheet> TemplateSheets = this.GetDataBindedSheets(Student, SchoolYear, Semester);

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
            int SchoolYear = int.Parse(this.nudSchoolYear.Value + "");
            int Semester = int.Parse((this.cboSemester.SelectedItem as EMBACore.DataItems.SemesterItem).Value);
            Task<Workbook> task = Task<Workbook>.Factory.StartNew(() =>
            {
                Workbook new_workbook = new Workbook();
                foreach (string key in this.dicStudents.Keys)
                {
                    Workbook wb = this.GenerateWorkbook(this.dicStudents[key], SchoolYear, Semester);
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