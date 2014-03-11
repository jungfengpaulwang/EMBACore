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
    public partial class CS_Template_NoSubject : BaseForm
    {
        private UDT.CSConfiguration conf;
        private UDT.CSConfiguration conf_subject;
        //private BrowserWrapper.BrowserProxy proxy = new BrowserWrapper.BrowserProxy();
        private UDT.CSConfiguration.TemplateName TemplateName;

        public CS_Template_NoSubject(UDT.CSConfiguration.TemplateName TemplateName)
        {
            InitializeComponent();

            this.TemplateName = TemplateName;
            this.Text = "編輯「" + this.TemplateName.ToString() + "」文字樣版";
            this.TitleText = "編輯「" + this.TemplateName.ToString() + "」文字樣版";

            webBrowser1.GotFocus += new EventHandler(webBrowser1_GotFocus);
        }

        private void Email_Content_Template_Load(object sender, EventArgs e)
        {
            conf = UDT.CSConfiguration.GetTemplate(this.TemplateName);
            this.Decorate(this.webBrowser1, conf.Content);
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            ErrorProvider errorProvider1 = new ErrorProvider();

            try
            {
                this.btnSave.Enabled = false;
                conf.Content = webBrowser1.Document.Body.InnerHtml.Replace("<div id=\"editable\">", string.Empty);
                List<ActiveRecord> recs = new List<ActiveRecord>();
                recs.Add(conf);
                (new AccessHelper()).UpdateValues(recs);
                MessageBox.Show("儲存成功。");
                //this.Close();
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
                //MessageBox.Show(range.htmlText);
                string content = string.Format("<span style='{0}'>{1}</span>", cssString, range.text);
                range.pasteHTML(content);
            }
        }

        private void btnExit_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void webBrowser1_GotFocus(object sender, EventArgs e)
        {
        }

        private void txtSubject_Enter(object sender, EventArgs e)
        {
        }

        private void Decorate(System.Windows.Forms.WebBrowser browser, string content)
        {
            System.Windows.Forms.WebBrowser WebBrowser1 = browser;

            WebBrowser1.Navigate("about:blank");
            WebBrowser1.Document.OpenNew(false).Write("<html><body><div id=\"editable\">" + content + "</div></body></html>");

            //'turns off document body editing
            foreach (System.Windows.Forms.HtmlElement el in WebBrowser1.Document.All)
            {
                el.SetAttribute("unselectable", "on");
                el.SetAttribute("contenteditable", "false");
            }
            WebBrowser1.Document.Body.All["editable"].SetAttribute("width", "100%");
            WebBrowser1.Document.Body.All["editable"].SetAttribute("height", "100%");
            WebBrowser1.Document.Body.All["editable"].SetAttribute("contenteditable", "true");
            WebBrowser1.Document.Body.All["editable"].Focus();
            WebBrowser1.Document.DomDocument.GetType().GetProperty("designMode").SetValue(WebBrowser1.Document.DomDocument, "On", null);
            WebBrowser1.IsWebBrowserContextMenuEnabled = false;
        }
    }
}
