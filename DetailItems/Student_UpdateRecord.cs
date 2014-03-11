using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Campus.Windows;
using FISCA.Permission;
using System.Net;
using System.IO;
using System.Xml;

namespace EMBACore.DetailItems
{
    [AccessControl("ischool.EMBA.Student_UpdateRecord", "異動紀錄", "學生>資料項目")]
    public partial class Student_UpdateRecord : DetailContentImproved
    {
        System.Xml.XmlDocument xmlDoc;

        public Student_UpdateRecord()
        {
            InitializeComponent();
            this.Group = "異動紀錄(同步資料)";
        }

        private void Student_UpdateRecord_Load(object sender, EventArgs e)
        {            
        }

        protected override void OnPrimaryKeyChangedAsync()
        {
            try
            {
                UTF8Encoding encoding = new UTF8Encoding();
                K12.Data.StudentRecord stud = K12.Data.Student.SelectByID(this.PrimaryKey);
                string postData = "stud_no=" + stud.StudentNumber;

                byte[] data = encoding.GetBytes(postData);

                // Prepare web request...
                HttpWebRequest myRequest =
                  (HttpWebRequest)WebRequest.Create("http://web.management.ntu.edu.tw:1001/emba_uprecords/uprec.aspx");
                myRequest.Method = "POST";
                myRequest.ContentType = "application/x-www-form-urlencoded";
                myRequest.ContentLength = data.Length;
                myRequest.Proxy = new WebProxy();
                Stream newStream = myRequest.GetRequestStream();
                // Send the data.
                newStream.Write(data, 0, data.Length);
                newStream.Close();
                myRequest.GetResponse();

                WebResponse myWebResponse = myRequest.GetResponse();

                Stream myResponseStream = myWebResponse.GetResponseStream();
                StreamReader myStreamReader = new StreamReader(myResponseStream);

                this.xmlDoc = new System.Xml.XmlDocument();
                this.xmlDoc.LoadXml(myStreamReader.ReadToEnd());

                myStreamReader.Close();
                myResponseStream.Close();
                myWebResponse.Close();
            }
            catch (Exception ex)
            {
                //throw new Exception(ex.Message);
            }
        }

        protected override void OnDirtyStatusChanged(ChangeEventArgs e)
        {
            if (UserAcl.Current[this.GetType()].Editable)
                SaveButtonVisible = e.Status == ValueStatus.Dirty;
            else
                this.SaveButtonVisible = false;

            CancelButtonVisible = e.Status == ValueStatus.Dirty;
        }

        protected override void OnPrimaryKeyChangedComplete(Exception error)
        {            
            if (error != null) //有錯就直接丟出去吧。
                throw error;

            string previousTr = "";

            this.dataGridViewX1.Rows.Clear();
            if (this.xmlDoc == null || this.xmlDoc.DocumentElement == null || this.xmlDoc.DocumentElement.SelectNodes("UpdRecord") == null)
                return;
            foreach (XmlElement elm in this.xmlDoc.DocumentElement.SelectNodes("UpdRecord"))
            {
                List<object> rawData = new List<object>();
                rawData.Add(GetNodeText(elm, "reg_no"));
                rawData.Add(ConvertTr1(GetNodeText(elm, "tr1")));
                rawData.Add(ConvertUpdateCode(GetNodeText(elm, "tr2")));

                //如果沒有學年度學期，代表與前一筆相同。
                string tr = GetNodeText(elm, "tr");
                if (string.IsNullOrWhiteSpace(tr))
                    tr = previousTr;
                else
                    previousTr = tr;
                rawData.Add(tr);

                rawData.Add(ConvertDefer(GetNodeText(elm, "defer")));
                rawData.Add(GetNodeText(elm, "r_qr1"));
                rawData.Add(GetNodeText(elm, "r_qr2"));
                rawData.Add(GetNodeText(elm, "r_qr3"));
                rawData.Add(GetNodeText(elm, "r_qr4"));
                rawData.Add(GetNodeText(elm, "r_qr5"));
                rawData.Add(GetNodeText(elm, "r_qr6"));
                rawData.Add(GetNodeText(elm, "r_qr7"));
                rawData.Add(GetNodeText(elm, "r_qr8"));
                rawData.Add(GetNodeText(elm, "create_time"));
                this.dataGridViewX1.Rows.Add(rawData.ToArray<object>());
            }
        }
        private string ConvertTr1(string tr1)
        {
            return (tr1 == "*") ? "否" : "是";
        }

        private string ConvertDefer(string defer)
        {
            return (defer == "*") ? "是" : "否";
        }

        private string ConvertUpdateCode(string updCode)
        {
            string result = "";
            if (updCode.ToUpper() == "G") result = "畢業";
            if (updCode.ToUpper() == "O") result = "退學";
            if (updCode.ToUpper() == "B") result = "休學";
            if (updCode.ToUpper() == "R") result = "復學";

            return result;
        }

        private string GetNodeText(XmlElement elm, string xpath)
        {
            string result = "";
            if (elm != null)
            {
                XmlNode nd = elm.SelectSingleNode(xpath);
                if (nd != null)
                    result = nd.InnerText;
            }
            return result;
        }
    }
}
