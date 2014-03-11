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

namespace EMBACore.Forms
{
    public partial class Email_Content_Template : BaseForm
    {
        private UDT.AbsenceConfiguration conf;
        private BrowserWrapper.BrowserProxy proxy = new BrowserWrapper.BrowserProxy(); 
        public Email_Content_Template()
        {
            InitializeComponent();
        }

        private void Email_Content_Template_Load(object sender, EventArgs e)
        {
            conf = UDT.AbsenceConfiguration.GetEmailContentTemplate();
            proxy.Decorate(this.webBrowser1, conf.Content);

            /*
            webBrowser1.Navigate("about:blank");
            Application.DoEvents();
            webBrowser1.Document.OpenNew(false).Write("<!doctype foo><html><body>" + conf.Content + "</body></html>");

            foreach (HtmlElement el in webBrowser1.Document.All)
            {

                el.SetAttribute("unselectable", "on");
                el.SetAttribute("contenteditable", "false");
            }
            try
            {
                //webBrowser1.Document.DomDocument.
                webBrowser1.Document.Body.SetAttribute("width", this.Width.ToString() + "px");
                webBrowser1.Document.Body.SetAttribute("height", "100%");
                webBrowser1.Document.Body.SetAttribute("contenteditable", "true");
                //webBrowser1.Document.DomDocument.GetType().GetProperty("designMode").SetValue(webBrowser1.Document.DomDocument, "On", null);
                webBrowser1.IsWebBrowserContextMenuEnabled = false;
            }
            catch (Exception ex)
            {
                Util.ShowMsg("此功能只能在 Win 7 以上的作業系統執行，請確定您的作業系統是 Win 7 以上。 \n" + ex.Message, "錯誤！");
                this.btnSave.Enabled = false;
            }
             * */
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            try
            {
                this.btnSave.Enabled = false;
                conf.Content = webBrowser1.Document.Body.All["editable"].InnerHtml;
                List<ActiveRecord> recs = new List<ActiveRecord>();
                recs.Add(conf);
                (new AccessHelper()).UpdateValues(recs);
                this.btnSave.Enabled = true;
                this.Close();
            }
            catch(Exception ex)
            {
                MsgBox.Show(ex.Message);
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
    }
}
