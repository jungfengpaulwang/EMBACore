using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using FISCA.Permission;
using UDTDetailContentBase;
using System.Xml;
using Campus.Windows;
using DevComponents.DotNetBar;
using System.Text.RegularExpressions;

namespace EMBACore.DetailItems
{
    [AccessControl("ischool.EMBA.Student.Student_Email", "電子郵件", "學生>資料項目")]
    public partial class Student_Email : DetailContentImproved
    {
        private SingleUDCDecorator<UDT.StudentBrief2> decDetailBase;
        private List<XmlElement> emails = new List<XmlElement>();
        private Log.LogAgent logAgent;

        public Student_Email()
        {
            InitializeComponent();
            this.Group = "通訊資料";
        }

        private void Student_Email_Load(object sender, EventArgs e)
        {
            this.decDetailBase = new UDTDetailContentBase.SingleUDCDecorator<UDT.StudentBrief2>(this, "StudentID", false);
            this.decDetailBase.AfterDataLoaded += new SingleUDCDecorator<UDT.StudentBrief2>.UDTDetailContentEventHandler(decDetailBase_AfterDataLoaded);
            this.decDetailBase.OnValidatingData += new SingleUDCDecorator<UDT.StudentBrief2>.UDTDetailContentEventHandler(decDetailBase_OnValidatingData);
            this.decDetailBase.OnReadDataError += new SingleUDCDecorator<UDT.StudentBrief2>.UDTDetailContentErrorEventHandler(decDetailBase_OnReadDataError);
            this.decDetailBase.OnSaveDataError += new SingleUDCDecorator<UDT.StudentBrief2>.UDTDetailContentErrorEventHandler(decDetailBase_OnSaveDataError);
            this.decDetailBase.AfterDataSaved += new EventHandler(decDetailBase_AfterDataSaved);
            
            WatchChange(new TextBoxSource(this.txtEmail));

            //this.balloonTip1.SetBalloonCaption(this.btnDuplicate, "!");
            this.balloonTip1.SetBalloonText(this.btnDuplicate, "將此學生所有的 Email 複製到剪貼簿！");
            // this.balloonTip1.CaptionImage = global::WindowsApplication2.Properties.Resources.Warning;
            //this.balloonTip1.ShowBalloon(this.btnDuplicate); 

            //log
            this.logAgent = new Log.LogAgent();
        }

        private void createButtonItems()
        {
            this.btnEmail.SubItems.Clear();

            for (int i = 1; i <= 5; i++)
            {
                ButtonItem bi = new ButtonItem();
                bi.Text = string.Format("電子郵件{0}", i.ToString());
                bi.Tag = this.emails[i - 1];
                bi.Click += new EventHandler(bi_Click);
                this.btnEmail.SubItems.Add(bi);
            }
        }

        void bi_Click(object sender, EventArgs e)
        {
            //1. put the text in textbox into emails
            XmlElement elmPreviousEmail = (XmlElement)this.btnEmail.Tag;
            if (elmPreviousEmail != null)
                elmPreviousEmail.InnerText = this.txtEmail.Text;

            //2. set the next email to textbox
            XmlElement elmEmail = (XmlElement)((ButtonItem)sender).Tag;
            if (elmEmail != null)
            {
                BeginChangeControlData();
                this.txtEmail.Text = elmEmail.InnerText;
                this.btnEmail.Tag = elmEmail;
                this.btnEmail.Text = ((ButtonItem)sender).Text;
                EndChangeControlData();
            }
        }

        protected override void OnPrimaryKeyChangedAsync()
        {
        }

        protected override void OnPrimaryKeyChangedComplete(Exception error)
        {
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


        void decDetailBase_AfterDataSaved(object sender, EventArgs e)
        {
            this.decDetailBase.GetCurrentRecord();
            this.decDetailBase.ReloadData();

            /* Log */
            this.AddLog(this.decDetailBase.GetCurrentRecord());
            this.logAgent.Save("基本資料.學生", "", ""
                                            , Log.LogTargetCategory.Student,
                                            this.decDetailBase.GetCurrentRecord().StudentID.ToString());
        }

        void decDetailBase_OnSaveDataError(object sender, UDTDetailContentErrorEventArgs<UDT.StudentBrief2> e)
        {
            Util.ShowMsg("儲存資料時發生錯誤！", this.Group);
        }

        void decDetailBase_OnReadDataError(object sender, UDTDetailContentErrorEventArgs<UDT.StudentBrief2> e)
        {
            Util.ShowMsg("讀取資料時發生錯誤！", this.Group);
        }

        private bool isValidEmail(string email)
        {
            string patternStrict = @"^(([^<>()[\]\\.,;:\s@\""]+"
                       + @"(\.[^<>()[\]\\.,;:\s@\""]+)*)|(\"".+\""))@"
                       + @"((\[[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}"
                       + @"\.[0-9]{1,3}\])|(([a-zA-Z\-0-9]+\.)+"
                       + @"[a-zA-Z]{2,}))$";
            Regex reStrict = new Regex(patternStrict);

            bool isStrictMatch = reStrict.IsMatch(email);
            return isStrictMatch;
        }

        void decDetailBase_OnValidatingData(object sender, UDTDetailContentEventArgs<UDT.StudentBrief2> e)
        {
            if (!string.IsNullOrWhiteSpace(this.txtEmail.Text))
            {
                if (!this.isValidEmail(this.txtEmail.Text.Trim()))
                {
                    e.Canceled = true;
                    (new ErrorProvider()).SetError(this.txtEmail, "不正確的格式。");
                    return;
                }
                else
                    (new ErrorProvider()).SetError(this.txtEmail, "");
            }
            else
                (new ErrorProvider()).SetError(this.txtEmail, "");

            UDT.StudentBrief2 data = e.CurrentTargets[0];

            XmlElement elmFinal = (XmlElement)btnEmail.Tag;
            if (elmFinal != null)
                elmFinal.InnerText = this.txtEmail.Text;

            StringBuilder sb = new StringBuilder();
            foreach (XmlElement elm in this.emails)
            {
                sb.Append(elm.OuterXml);
            }
            data.EmailList = sb.ToString();
        }

        void decDetailBase_AfterDataLoaded(object sender, UDTDetailContentEventArgs<UDT.StudentBrief2> e)
        {
            UDT.StudentBrief2 data = e.CurrentTargets[0];
            
            parseEmail(data.EmailList);

            /* Log */
            this.logAgent.Clear();
            this.logAgent.ActionType = string.IsNullOrWhiteSpace(data.UID) ? Log.LogActionType.AddNew : Log.LogActionType.Update;
            this.AddLog(data);
        }

        private void parseEmail(string emailList)
        {
            if (string.IsNullOrEmpty(emailList))
                emailList = "<?xml version='1.0' encoding='utf-8' ?><emails><email1 /><email2 /><email3 /><email4 /><email5 /></emails>";
            else
                emailList = "<?xml version='1.0' encoding='utf-8' ?><emails>" + emailList + "</emails>";

            this.emails = new List<XmlElement>();
            System.Xml.XmlDocument xmlDoc = new System.Xml.XmlDocument();
            xmlDoc.LoadXml(emailList);
            for (int i = 1; i < 6; i++)
            {
                string key = string.Format("email{0}", i.ToString());
                XmlElement elm = (XmlElement)xmlDoc.DocumentElement.SelectSingleNode(key);
                if (elm == null)
                    elm = xmlDoc.CreateElement(key);

                this.emails.Add(elm);
            }

            this.createButtonItems();
            this.txtEmail.Text = this.emails[0].InnerText;
            this.btnEmail.Text = "電子郵件1";
            this.btnEmail.Tag = this.emails[0];
        }

        void AddLog(UDT.StudentBrief2 udtObj)
        {
            this.logAgent.SetLogValue("畢業學期", udtObj.GraduateSemester);
            this.logAgent.SetLogValue("畢業學年度", udtObj.GraduateYear);

            this.logAgent.SetLogValue("電子郵件", udtObj.EmailList);
        }

        private void txtEmail_TextChanged(object sender, EventArgs e)
        {
            this.SaveButtonVisible = true;
            this.CancelButtonVisible = true;
        }

        private void btnDuplicate_MouseEnter(object sender, EventArgs e)
        {

        }

        private void btnDuplicate_Click(object sender, EventArgs e)
        {
            StringBuilder sb = new StringBuilder();
            foreach (XmlElement elm in this.emails)
            {
                if (!string.IsNullOrWhiteSpace(elm.InnerText))
                {
                    sb.Append(elm.InnerText);
                    sb.Append(";");
                }
            }
            //put to Clipboard !
            if (sb.Length == 0)
                Util.ShowMsg("此學生沒有指定任何 Email！", "複製 Email");
            else
            {
                Clipboard.Clear();
                Clipboard.SetText(sb.ToString());
                Util.ShowMsg("已經把此學生所有的 Email 複製到剪貼簿裡！","複製 Email");
            }
        }
    }
}
