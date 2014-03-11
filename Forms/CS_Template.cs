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
    public partial class CS_Template : BaseForm
    {
        private UDT.CSConfiguration conf;
        private BrowserWrapper.BrowserProxy proxy = new BrowserWrapper.BrowserProxy();
        private UDT.CSConfiguration.TemplateName TemplateName;
        public CS_Template(UDT.CSConfiguration.TemplateName TemplateName)
        {
            InitializeComponent();

            this.TemplateName = TemplateName;
        }

        private void Email_Content_Template_Load(object sender, EventArgs e)
        {
            conf = UDT.CSConfiguration.GetTemplate(this.TemplateName);
            proxy.Decorate(this.webBrowser1, conf.Content);
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            try
            {
                this.btnSave.Enabled = false;
                conf.Content = webBrowser1.Document.Body.InnerHtml;
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
    }
}
