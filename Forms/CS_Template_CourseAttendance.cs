using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using FISCA.Presentation.Controls;
using FISCA.UDT;
using mshtml;
using DevComponents.DotNetBar;

namespace EMBACore.Forms
{
    public partial class CS_Template_CourseAttendance : BaseForm
    {
        private UDT.CSConfiguration conf;
        private UDT.CSConfiguration conf_subject;
        private string TemplateName_Subfix = "course-attend-reminder";
        private bool formload;
        private decimal attend_no;
        private object CurrentObject;

        public CS_Template_CourseAttendance()
        {
            InitializeComponent();

            webBrowser1.GotFocus += new EventHandler(webBrowser1_GotFocus);
            this.btnStudentName.Click += new System.EventHandler(this.btn_Click);
            this.btnCourseName.Click += new System.EventHandler(this.btn_Click);
            this.btnSchoolYear.Click += new System.EventHandler(this.btn_Click);
            this.btnSemester.Click += new System.EventHandler(this.btn_Click);
            this.btnAttendNo.Click += new System.EventHandler(this.btn_Click);
            this.btnAttendTime.Click += new System.EventHandler(this.btn_Click);
            this.btnTotalPeriod.Click += new System.EventHandler(this.btn_Click);

            this.Load += new System.EventHandler(this.Form_Load);
            this.attend_no = 4;
            this.formload = false;
        }

        private void btn_Click(object sender, EventArgs e)
        {
            if (this.CurrentObject.GetType() == this.webBrowser1.GetType())
            {
                IHTMLDocument2 htmlDocument = webBrowser1.Document.DomDocument as IHTMLDocument2;

                IHTMLSelectionObject currentSelection = htmlDocument.selection;

                if (currentSelection != null)
                {
                    IHTMLTxtRange range = currentSelection.createRange() as IHTMLTxtRange;
                    range.text = "[[" + (sender as ButtonItem).Text.Trim() + "]]";
                    //MessageBox.Show(range.htmlText);
                    //string content = string.Format("<span>{1}</span>", range.text);
                    range.pasteHTML(range.text);
                }
            }
            else
            {
                if (this.txtSubject.SelectedText.Length > 0)
                    this.txtSubject.Text = this.txtSubject.Text.Replace(this.txtSubject.SelectedText, "[[" + (sender as ButtonItem).Text.Trim() + "]]");
                else
                {
                    if (this.txtSubject.Text.Length > 0)
                    {
                        int selectionStart = this.txtSubject.SelectionStart;
                        int textLength = this.txtSubject.TextLength;

                        string leftSubject = string.Empty;
                        string rightSubject = string.Empty;

                        if (selectionStart == 0)
                        {
                            leftSubject = string.Empty;
                            rightSubject = txtSubject.Text;
                        }
                        else if (selectionStart == textLength)
                        {
                            leftSubject = txtSubject.Text;
                            rightSubject = string.Empty;
                        }
                        else
                        {
                            leftSubject = this.txtSubject.Text.Substring(0, selectionStart);
                            rightSubject = this.txtSubject.Text.Substring(selectionStart, textLength - leftSubject.Length);
                        }
                        this.txtSubject.Text = leftSubject + "[[" + (sender as ButtonItem).Text.Trim() + "]]" + rightSubject;

                        //this.txtSubject.SelectionStart = selectionStart;
                    }
                    else
                        this.txtSubject.Text = "[[" + (sender as ButtonItem).Text.Trim() + "]]";
                }
            }
        }

        private void InitAttendNo()
        {
            this.nudAttendNo.Value = this.attend_no;
        }

        private void Form_Load(object sender, EventArgs e)
        {
            this.formload = false;

            this.InitAttendNo();

            this.formload = true;

            this.SetConf();

            this.Decorate(this.webBrowser1, conf.Content);
        }

        private void SetConf()
        {
            if (!this.formload)
                return;

            string TemplateName = this.TemplateName_Subfix + "-" + this.nudAttendNo.Value;
            conf = UDT.CSConfiguration.GetTemplate(TemplateName);
            conf_subject = UDT.CSConfiguration.GetTemplate(this.TemplateName_Subfix + "-" + this.nudAttendNo.Value + "_subject");
            this.txtSubject.Text = conf_subject.Content;
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            ErrorProvider errorProvider1 = new ErrorProvider();

            try
            {
                this.btnSave.Enabled = false;
                conf.Content = webBrowser1.Document.Body.InnerHtml.Replace("<div id=\"editable\">", string.Empty);
                conf_subject.Content = this.txtSubject.Text.Trim();
                List<ActiveRecord> recs = new List<ActiveRecord>();
                recs.Add(conf);
                recs.Add(conf_subject);
                (new AccessHelper()).UpdateValues(recs);
                MessageBox.Show("儲存成功。");
            }
            catch (Exception ex)
            {
                MsgBox.Show(ex.Message);
            }
            finally
            {
                this.btnSave.Enabled = true;
            }
        }

        private void colorPickerDropDown1_SelectedColorChanged(object sender, EventArgs e)
        {
            this.SetCss("color:" + ColorTranslator.ToHtml(colorPickerDropDown1.SelectedColor));
        }

        private void SetCss(string cssString)
        {
            IHTMLDocument2 htmlDocument = webBrowser1.Document.DomDocument as IHTMLDocument2;

            IHTMLSelectionObject currentSelection = htmlDocument.selection;

            if (currentSelection != null)
            {
                IHTMLTxtRange range = currentSelection.createRange() as IHTMLTxtRange;
                string content = string.Format("<span style='{0}'>{1}</span>", cssString, range.text);
                range.pasteHTML(content);
            }
        }

        private void btnExit_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void Decorate(System.Windows.Forms.WebBrowser browser, string content)
        {
            string html_code = string.IsNullOrEmpty(content) ? "&nbsp;" : content;
            browser.Stop();
            if (browser.Document != null)
            {
                browser.Document.OpenNew(true);
                browser.Document.Write(html_code);
            }
            else
            {
                browser.DocumentText = html_code;
            }
            Application.DoEvents();
            if (browser.Document != null)
            {
                browser.Document.Focus();
                browser.Document.DomDocument.GetType().GetProperty("designMode").SetValue(browser.Document.DomDocument, "On", null);
            }
            browser.IsWebBrowserContextMenuEnabled = false;
        }

        private void nudAttendNo_ValueChanged(object sender, EventArgs e)
        {
            if (!this.formload)
                return;

            this.SetConf();
            this.Decorate(this.webBrowser1, conf.Content);
        }

        private void txtSubject_Enter(object sender, EventArgs e)
        {
            this.CurrentObject = this.txtSubject;
        }

        private void webBrowser1_GotFocus(object sender, EventArgs e)
        {
            this.CurrentObject = this.webBrowser1;
        }
    }
}
