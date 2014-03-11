using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using EMBA.Export;
using FISCA.Data;
using Aspose.Cells;

namespace EMBACore.Export
{
    public partial class Course_Brief_Export_Excel_1
    {
        private ExportProxyForm exportProxyForm;
        public Course_Brief_Export_Excel_1()
        {
        }

        public void Execute()
        {
            string querySQL = string.Format(@"select course.id as 課程系統編號, subject.dept_code as 系所代碼, ce.subject_code as 課程識別碼, ce.class_name as 開課班次, course.credit as 學分數, ce.is_required as 必選修, course.course_name as 開課, subject.eng_name as 課程英文名稱,  te.employee_no as 員工代碼, te.ntu_system_no as 教師代碼, teacher.teacher_name as 授課教師, te.english_name as 授課教師英文名稱, subject.remark as 備註, ce.new_subject_code as 課號, teacher.id as 教師系統編號 from course
left join $ischool.emba.course_ext as ce on ce.ref_course_id=course.id
left join $ischool.emba.subject as subject on subject.uid= ce.ref_subject_id
left join $ischool.emba.course_instructor as ci on course.id=ci.ref_course_id
left join teacher on teacher.id=ci.ref_teacher_id
left join $ischool.emba.teacher_ext as te on teacher.id=te.ref_teacher_id where course.id in ({0}) order by course.id", String.Join(",", K12.Presentation.NLDPanels.Course.SelectedSource));

            exportProxyForm = new ExportProxyForm(querySQL, true, false, "課程系統編號", new List<string>() { "課程系統編號", "教師系統編號" }, null);
            //exportProxyForm.HideSemesterControls();
            exportProxyForm.Text = "匯出課程檔(Excel 格式)";
            exportProxyForm.lblExplanation.Text = "";
            exportProxyForm.chkSelectAll.Visible = false;
            exportProxyForm.FieldContainer.Enabled = false;
            exportProxyForm.FieldContainer.Items.Clear();
            exportProxyForm.chkSelectAll.Visible = false;
            exportProxyForm.Size = new System.Drawing.Size(exportProxyForm.Size.Width, 90);
            exportProxyForm.SizeChanged += (x, y) => { exportProxyForm.Size = new System.Drawing.Size(exportProxyForm.Size.Width, 90); };

            DialogResult dr = exportProxyForm.ShowDialog();

            if (dr == DialogResult.Cancel)
                return;

            if (dr == DialogResult.OK)
            {
                BackgroundWorker _BGWLoadData = exportProxyForm.SalvageOperation;
                _BGWLoadData.RunWorkerCompleted += new RunWorkerCompletedEventHandler(_BGWLoadData_RunWorkerCompleted);
            }
        }

        private void _BGWLoadData_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            DataTable dataTable = e.Result as DataTable;
            Dictionary<string, List<string>> dicCourseTeachers = new Dictionary<string, List<string>>();
            
            //  授課教師
            DataTable dataTable_Teacher = (new QueryHelper()).Select(string.Format(@"select course.id as ref_course_id, teacher.id as ref_teacher_id, course.course_name, teacher.teacher_name from course join $ischool.emba.course_instructor as ci on course.id=ci.ref_course_id join teacher on teacher.id=ci.ref_teacher_id where teacher.id in (SELECT  distinct teacher.id FROM course LEFT JOIN $ischool.emba.course_instructor ON $ischool.emba.course_instructor.ref_course_id = course.id LEFT JOIN teacher ON teacher.id = $ischool.emba.course_instructor.ref_teacher_id LEFT JOIN tag_teacher ON tag_teacher.ref_teacher_id = teacher.id LEFT JOIN tag ON tag.id = tag_teacher.ref_tag_id WHERE tag.category = 'Teacher' AND tag.prefix = '教師') order by course.course_name, teacher.teacher_name"));
            foreach (DataRow row in dataTable_Teacher.Rows)
            {
                if (!dicCourseTeachers.ContainsKey(row["ref_course_id"] + ""))
                    dicCourseTeachers.Add(row["ref_course_id"] + "", new List<string>());

                dicCourseTeachers[row["ref_course_id"] + ""].Add(row["ref_teacher_id"] + "");
            }

            dataTable.TableName = "授課教師";

            if (dataTable == null || dataTable.Rows.Count == 0)
            {
                FISCA.Presentation.Controls.MsgBox.Show("無資料可匯出。", "欄位空白", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                return;
            }

            //string fileName = "課程檔";
            //string filePath = string.Empty;
            //string message = string.Empty; 
            
            //System.Windows.Forms.FolderBrowserDialog folder = new FolderBrowserDialog();
            //do
            //{
            //    DialogResult dr = folder.ShowDialog();
            //    if (dr == DialogResult.OK)
            //        filePath = folder.SelectedPath;
            //    if (dr == DialogResult.Cancel)
            //        return;
            //} while (!System.IO.Directory.Exists(filePath));

            DBF_Table dbf_Table = new DBF_Table();

            //  加入 dbf 欄位  
            dbf_Table.Fields.Add(new DBF_Field("ser_no", FieldType.Char, 250, 0, false));
            dbf_Table.Fields.Add(new DBF_Field("co_chg", FieldType.Char, 250, 0, false));
            dbf_Table.Fields.Add(new DBF_Field("dpt_code", FieldType.Char, 250, 0, false));
            dbf_Table.Fields.Add(new DBF_Field("year", FieldType.Char, 250, 0, false));
            dbf_Table.Fields.Add(new DBF_Field("cou_code", FieldType.Char, 250, 0, false));
            dbf_Table.Fields.Add(new DBF_Field("class", FieldType.Char, 250, 0, false));
            dbf_Table.Fields.Add(new DBF_Field("credit", FieldType.Char, 250, 0, false));
            dbf_Table.Fields.Add(new DBF_Field("tlec", FieldType.Char, 250, 0, false));
            dbf_Table.Fields.Add(new DBF_Field("tlab", FieldType.Char, 250, 0, false));
            dbf_Table.Fields.Add(new DBF_Field("forh", FieldType.Char, 250, 0, false));
            dbf_Table.Fields.Add(new DBF_Field("sel_code", FieldType.Char, 250, 0, false));
            dbf_Table.Fields.Add(new DBF_Field("cou_cname", FieldType.Char, 250, 0, false));
            dbf_Table.Fields.Add(new DBF_Field("cou_ename", FieldType.Char, 250, 0, false));
            dbf_Table.Fields.Add(new DBF_Field("tea_seq", FieldType.Char, 250, 0, false));
            dbf_Table.Fields.Add(new DBF_Field("tea_code", FieldType.Char, 250, 0, false));
            dbf_Table.Fields.Add(new DBF_Field("tea_cname", FieldType.Char, 250, 0, false));
            dbf_Table.Fields.Add(new DBF_Field("tea_ename", FieldType.Char, 250, 0, false));
            dbf_Table.Fields.Add(new DBF_Field("clsrom_1", FieldType.Char, 250, 0, false));
            dbf_Table.Fields.Add(new DBF_Field("clsrom_2", FieldType.Char, 250, 0, false));
            dbf_Table.Fields.Add(new DBF_Field("clsrom_3", FieldType.Char, 250, 0, false));
            dbf_Table.Fields.Add(new DBF_Field("clsrom_4", FieldType.Char, 250, 0, false));
            dbf_Table.Fields.Add(new DBF_Field("clsrom_5", FieldType.Char, 250, 0, false));
            dbf_Table.Fields.Add(new DBF_Field("clsrom_6", FieldType.Char, 250, 0, false));
            dbf_Table.Fields.Add(new DBF_Field("st1", FieldType.Char, 250, 0, false));
            dbf_Table.Fields.Add(new DBF_Field("day1", FieldType.Char, 250, 0, false));
            dbf_Table.Fields.Add(new DBF_Field("st2", FieldType.Char, 250, 0, false));
            dbf_Table.Fields.Add(new DBF_Field("day2", FieldType.Char, 250, 0, false));
            dbf_Table.Fields.Add(new DBF_Field("st3", FieldType.Char, 250, 0, false));
            dbf_Table.Fields.Add(new DBF_Field("day3", FieldType.Char, 250, 0, false));
            dbf_Table.Fields.Add(new DBF_Field("st4", FieldType.Char, 250, 0, false));
            dbf_Table.Fields.Add(new DBF_Field("day4", FieldType.Char, 250, 0, false));
            dbf_Table.Fields.Add(new DBF_Field("st5", FieldType.Char, 250, 0, false));
            dbf_Table.Fields.Add(new DBF_Field("day5", FieldType.Char, 250, 0, false));
            dbf_Table.Fields.Add(new DBF_Field("st6", FieldType.Char, 250, 0, false));
            dbf_Table.Fields.Add(new DBF_Field("day6", FieldType.Char, 250, 0, false));
            dbf_Table.Fields.Add(new DBF_Field("limit", FieldType.Char, 250, 0, false));
            dbf_Table.Fields.Add(new DBF_Field("tno", FieldType.Char, 250, 0, false));
            dbf_Table.Fields.Add(new DBF_Field("eno", FieldType.Char, 250, 0, false));
            dbf_Table.Fields.Add(new DBF_Field("co_select", FieldType.Char, 250, 0, false));
            dbf_Table.Fields.Add(new DBF_Field("sno", FieldType.Char, 250, 0, false));
            dbf_Table.Fields.Add(new DBF_Field("mark", FieldType.Char, 250, 0, false));
            dbf_Table.Fields.Add(new DBF_Field("co_rep", FieldType.Char, 250, 0, false));
            dbf_Table.Fields.Add(new DBF_Field("co_tp", FieldType.Char, 250, 0, false));
            dbf_Table.Fields.Add(new DBF_Field("co_gmark", FieldType.Char, 250, 0, false));
            dbf_Table.Fields.Add(new DBF_Field("co_eng", FieldType.Char, 250, 0, false));
            dbf_Table.Fields.Add(new DBF_Field("grpno", FieldType.Char, 250, 0, false));
            dbf_Table.Fields.Add(new DBF_Field("initsel", FieldType.Char, 250, 0, false));
            dbf_Table.Fields.Add(new DBF_Field("outside", FieldType.Char, 250, 0, false));
            dbf_Table.Fields.Add(new DBF_Field("pre_course", FieldType.Char, 250, 0, false));
            dbf_Table.Fields.Add(new DBF_Field("dpt_abbr", FieldType.Char, 250, 0, false));
            dbf_Table.Fields.Add(new DBF_Field("cou_teacno", FieldType.Char, 250, 0, false));
            dbf_Table.Fields.Add(new DBF_Field("chgitem", FieldType.Char, 250, 0, false));
            dbf_Table.Fields.Add(new DBF_Field("engmark", FieldType.Char, 250, 0, false));

            DataTable newDataTable = dataTable.Clone();
            DataRow dRow = dataTable.NewRow();
            foreach (DataRow dataRow in dataTable.Rows)
            {
                string teacher_id = dataRow["教師系統編號"] + "";
                string course_id = dataRow["課程系統編號"] + "";

                if (dicCourseTeachers.ContainsKey(course_id))
                {
                    if (!string.IsNullOrEmpty(teacher_id))
                    {
                        if (!dicCourseTeachers[course_id].Contains(teacher_id))
                            continue;
                    }
                }

                if (newDataTable.Rows.Count == 0)
                    newDataTable.ImportRow(dataRow);
                else
                {
                    if (course_id == dRow["課程系統編號"].ToString())
                    {
                        if (!string.IsNullOrWhiteSpace(dataRow["授課教師"].ToString()))
                        {
                            if (!string.IsNullOrWhiteSpace(newDataTable.Rows[newDataTable.Rows.Count - 1]["授課教師"].ToString()))
                                newDataTable.Rows[newDataTable.Rows.Count - 1]["授課教師"] += "；" + dataRow["授課教師"].ToString().Trim();
                            else
                                newDataTable.Rows[newDataTable.Rows.Count - 1]["授課教師"] = dataRow["授課教師"].ToString().Trim();
                        }

                        if (!string.IsNullOrWhiteSpace(dataRow["授課教師英文名稱"].ToString()))
                        {
                            if (!string.IsNullOrWhiteSpace(newDataTable.Rows[newDataTable.Rows.Count - 1]["授課教師英文名稱"].ToString()))
                                newDataTable.Rows[newDataTable.Rows.Count - 1]["授課教師英文名稱"] += "；" + dataRow["授課教師英文名稱"].ToString().Trim();
                            else
                                newDataTable.Rows[newDataTable.Rows.Count - 1]["授課教師英文名稱"] = dataRow["授課教師英文名稱"].ToString().Trim();
                        }

                        if (!string.IsNullOrWhiteSpace(dataRow["員工代碼"].ToString()))
                        {
                            if (!string.IsNullOrWhiteSpace(newDataTable.Rows[newDataTable.Rows.Count - 1]["員工代碼"].ToString()))
                                newDataTable.Rows[newDataTable.Rows.Count - 1]["員工代碼"] += "；" + dataRow["員工代碼"].ToString().Trim();
                            else
                                newDataTable.Rows[newDataTable.Rows.Count - 1]["員工代碼"] = dataRow["員工代碼"].ToString().Trim();
                        }

                        if (!string.IsNullOrWhiteSpace(dataRow["教師代碼"].ToString()))
                        {
                            if (!string.IsNullOrWhiteSpace(newDataTable.Rows[newDataTable.Rows.Count - 1]["教師代碼"].ToString()))
                                newDataTable.Rows[newDataTable.Rows.Count - 1]["教師代碼"] += "；" + dataRow["教師代碼"].ToString().Trim();
                            else
                                newDataTable.Rows[newDataTable.Rows.Count - 1]["教師代碼"] = dataRow["教師代碼"].ToString().Trim();
                        }
                    }
                    else
                    {
                        newDataTable.ImportRow(dataRow);
                    }
                }
                dRow = dataRow;
            }

            //  加入 dbf 資料
            foreach (DataRow row in newDataTable.Rows)
            {
                DBF_Row dbf_row = dbf_Table.NewRow();

                dbf_row["ser_no"] = "";
                dbf_row["co_chg"] = "";
                dbf_row["dpt_code"] = (row["系所代碼"] + "");
                dbf_row["year"] = "";
                dbf_row["cou_code"] = (row["課程識別碼"] + "");
                dbf_row["class"] = (row["開課班次"] + "");
                dbf_row["credit"] = (row["學分數"] + "");
                dbf_row["tlec"] = "0";
                dbf_row["tlab"] = "0";
                dbf_row["forh"] = "";
                dbf_row["sel_code"] = ((row["必選修"] + "").ToUpper() == "FALSE" ? "7" : "3");
                dbf_row["cou_cname"] = (row["開課"] + "");
                dbf_row["cou_ename"] = (row["課程英文名稱"] + "");
                dbf_row["tea_seq"] = (row["員工代碼"] + "");
                dbf_row["tea_code"] = (row["教師代碼"] + "");
                dbf_row["tea_cname"] = (row["授課教師"] + "");
                dbf_row["tea_ename"] = (row["授課教師英文名稱"] + "");
                dbf_row["clsrom_1"] = "";
                dbf_row["clsrom_2"] = "";
                dbf_row["clsrom_3"] = "";
                dbf_row["clsrom_4"] = "";
                dbf_row["clsrom_5"] = "";
                dbf_row["clsrom_6"] = "";
                dbf_row["st1"] = "";
                dbf_row["day1"] = "";
                dbf_row["st2"] = "";
                dbf_row["day2"] = "";
                dbf_row["st3"] = "";
                dbf_row["day3"] = "";
                dbf_row["st4"] = "";
                dbf_row["day4"] = "";
                dbf_row["st5"] = "";
                dbf_row["day5"] = "";
                dbf_row["st6"] = "";
                dbf_row["day6"] = "";
                dbf_row["limit"] = "";
                dbf_row["tno"] = "";
                dbf_row["eno"] = "0";
                dbf_row["co_select"] = "2";
                dbf_row["sno"] = "0";
                dbf_row["mark"] = (row["備註"] + "");
                dbf_row["co_rep"] = "0";
                dbf_row["co_tp"] = "1";
                dbf_row["co_gmark"] = "";
                dbf_row["co_eng"] = "";
                dbf_row["grpno"] = "0";
                dbf_row["initsel"] = "";
                dbf_row["outside"] = "0";
                dbf_row["pre_course"] = "";

                //  解析課號(6+4)
                string s6 = string.Empty;
                string s4 = string.Empty;
                string s = (row["課號"] + "");
                if (s.Length >= 4)
                    s4 = s.Substring(s.Length - 4, 4);
                else
                    s4 = s;

                s6 = s.Substring(0, s.Length - s4.Length);

                dbf_row["dpt_abbr"] = s6;
                dbf_row["cou_teacno"] = s4;
                dbf_row["chgitem"] = "";
                dbf_row["engmark"] = "";

                dbf_Table.Rows.Add(dbf_row);
            }

            try
            {
                Workbook wb = dbf_Table.ToDataTable().ToWorkbook(true);
                wb.Save(true, "匯出課程檔(Excel 格式)");
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }

            this.Dispose();
        }

        private void Dispose()
        {
            exportProxyForm.Dispose();
        }
    }
}
