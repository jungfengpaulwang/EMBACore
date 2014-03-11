using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using FISCA.UDT;
using System.Reflection;
using DevComponents.DotNetBar.Controls;
using EMBACore.DataItems;

namespace EMBACore
{
    class Util
    {
        public static void ShowMsg(string msg, string source)
        {            
            FISCA.Presentation.Controls.MsgBox.Show(msg, source, MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        public static string toValidXmlString(string old)
        {
            return old.Replace("&amp;", "&").Replace("&", "&amp;");
        }

        /// <summary>
        /// 根據 UDT 上的欄位名稱取得齊對應的資料表欄位名稱
        /// </summary>
        /// <param name="pkFieldName"></param>
        /// <returns></returns>
        public static string GetUDTDBField<T>(string PropertyName)
        {
            string result = "";
            Type type = typeof(T);
            foreach (PropertyInfo f in type.GetProperties())
            {
                string fieldName = f.Name;  //取得屬性欄位名稱
                if (PropertyName.Equals(fieldName))
                {  //取得
                    foreach (object attribute in f.GetCustomAttributes(true))
                    {
                        FISCA.UDT.FieldAttribute field = (attribute as FISCA.UDT.FieldAttribute);
                        if (field != null)
                        {
                            result = field.Field;
                            break;
                        }
                    }
                    break;
                }
            } // end for

            return result;
        }


        public static void InitSemesterCombobox(ComboBoxEx cboSemester)
        {
            cboSemester.Items.Clear();
            foreach (SemesterItem semester in SemesterItem.GetSemesterList())
                cboSemester.Items.Add(semester);

            cboSemester.SelectedItem = SemesterItem.GetSemesterByCode(K12.Data.School.DefaultSemester);
        }

        public static void InitSchoolYearNumberUpDown(NumericUpDown nudSchoolYear)
        {
            nudSchoolYear.Maximum = 999;
            nudSchoolYear.Value = decimal.Parse(K12.Data.School.DefaultSchoolYear);
        }

        /// <summary>
        /// 判斷是否取得學分
        /// </summary>
        /// <param name="score"></param>
        /// <returns></returns>
        public static bool IsPass(string score)
        {
            bool result = false;
            if (!string.IsNullOrWhiteSpace(score))
            {
                int s = 0;
                if (int.TryParse(score, out s))
                {
                    result = (s >= 70);         //70分以上
                }
                else
                {
                    List<string> passGrade = new List<string>();
                    passGrade.AddRange(new string[] { "A+", "A", "A-", "B+", "B", "B-" });
                    result = passGrade.Contains(score.ToUpper());   //B- 以上
                }
            }
            return result;
        }

        public static bool IsValidScore(string score)
        {
            if (string.IsNullOrWhiteSpace(score))
                return true;  //允許成績是空白
            
            bool result = false;            
            
            List<string> allowScores = new List<string>();
            allowScores.AddRange(new string[] { "A+", "A", "A-", "B+", "B", "B-", "C+", "C", "C-", "F", "X" });
            //先判斷是否是數字
            int outInt = 0;
            result = int.TryParse(score, out outInt);

            //如果不是數字，則判斷是否是有效的文字？
            if (!result)            
                result = allowScores.Contains(score.ToUpper());            

            return result;
        }

    }
}
