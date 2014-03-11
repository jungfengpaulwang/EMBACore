using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using K12.Data;
using FISCA.Data;
using EMBACore.DataItems;
using EMBACore;
using System.Xml;
using Campus.Configuration;


public partial class Course_Add : FISCA.Presentation.Controls.BaseForm
{
        XmlDocument xmlSystemConfig;
        XmlElement elmSchoolYear;
        XmlElement elmSemester;

        public Course_Add()
        {
            InitializeComponent();
            /*
            this.cboSemester.Items.Clear();
            foreach (SemesterItem semester in SemesterItem.GetSemesterList())
                this.cboSemester.Items.Add(semester);

            this.cboSemester.SelectedItem = SemesterItem.GetSemesterByCode("0");
            this.nudSchoolYear.Value = (DateTime.Today.Year - 1911);
             * */
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            if (txtName.Text.Trim() == "")
                return;

            int SchoolYear, Semester;
            int.TryParse(nudSchoolYear.Value.ToString(), out SchoolYear);
            int.TryParse((cboSemester.Items[cboSemester.SelectedIndex] as SemesterItem).Value, out Semester);

            QueryHelper queryHelper = new QueryHelper();
            string strQuery = String.Format(@"select course_name, school_year, semester from course
Where course_name='{0}' and school_year='{1}' and semester='{2}'", txtName.Text.Trim(), SchoolYear, Semester);

            DataTable dataTable = queryHelper.Select(strQuery);

            if (dataTable != null && dataTable.Rows.Count > 0)
            {
                MessageBox.Show("同學期已有同名課程");
                return;
            }
                
            K12.Data.CourseRecord cre = new K12.Data.CourseRecord();

            cre.SchoolYear = SchoolYear;
            cre.Semester = Semester;
            cre.Name = this.txtName.Text.ToString();            

            string CourseID = K12.Data.Course.Insert(cre);
            //Course.Instance.SyncDataBackground(cre.ID);
            if (chkInputData.Checked == true)
                K12.Presentation.NLDPanels.Course.PopupDetailPane(CourseID);
                //Course.Instance.SyncDataBackground(cr.ID);
            this.Close();
        }

        private void btnExit_Click(object sender, EventArgs e)
        {
            this.Close ();
        }

        private void Course_Add_Load(object sender, EventArgs e)
        {
            this.cboSemester.Items.Clear();

            foreach (SemesterItem semester in SemesterItem.GetSemesterList())
                this.cboSemester.Items.Add(semester);

            xmlSystemConfig = new XmlDocument();
            xmlSystemConfig.LoadXml(Config.App["系統設定"].PreviousData.OuterXml);
            elmSchoolYear = (XmlElement)xmlSystemConfig.DocumentElement.SelectSingleNode("DefaultSchoolYear");
            elmSemester = (XmlElement)xmlSystemConfig.DocumentElement.SelectSingleNode("DefaultSemester");

            this.nudSchoolYear.Value = int.Parse(elmSchoolYear.InnerText);
            this.cboSemester.SelectedItem = SemesterItem.GetSemesterByCode(elmSemester.InnerText);
        }
    }

//public class SemesterItem
//{
//    private SemesterItem(string value, string Name)
//    {
//        this.Name = Name;
//        this.Value = value;
//    }
//    public string Value { get; set; }
//    public string Name { get; set; }
//    public override string ToString()
//    {
//        return this.Name;
//    }

//    private static Dictionary<string, SemesterItem> items;
//    public static List<SemesterItem> GetSemesterList()
//    {
//        if (items == null)
//        {
//            items = new Dictionary<string, SemesterItem>();
//            items.Add("0", new SemesterItem("0", "暑假"));
//            items.Add("1", new SemesterItem("1", "上學期"));
//            items.Add("2", new SemesterItem("2", "下學期"));
//        }
//        return items.Values.ToList<SemesterItem>();
//    }


//    /// <summary>
//    /// 根據代碼取得學期物件。
//    /// EMBA 可能的學期代碼值為： 0 (暑假), 1:上學期  , 2:下學期
//    /// </summary>
//    /// <param name="code"></param>
//    /// <returns></returns>
//    public static SemesterItem GetSemesterByCode(string code)
//    {
//        SemesterItem result = null;
//        if (items == null)
//            GetSemesterList();

//        if (items.ContainsKey(code))
//            result = items[code];

//        return result;
//    }
//}

