using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Security.Permissions;

namespace EMBACore.Forms
{
    public partial class HelpContentPanel : UserControl
    {
        public HelpContentPanel()
        {
            InitializeComponent();
            //Web.Document.Window.Error += new HtmlElementErrorEventHandler(Window_Error);
        }

        //private void Window_Error(object sender, HtmlElementErrorEventArgs e)
        //{
        //    e.Handled = true;
        //}

        public void Naviate(string url)
        {
            Web.Navigate(url);
        }

        public void GoBack()
        {
            Web.GoBack();
        }

        public void GoForward()
        {
            Web.GoForward();
        }

        public void URLRefresh()
        {
            Web.Refresh();
        }

        public void URLStop()
        {
            Web.Stop();
        }
    }
}
