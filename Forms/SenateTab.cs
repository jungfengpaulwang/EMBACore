using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using FISCA.Presentation;

namespace EMBACore.Forms
{
    public partial class SenateTab : BlankPanel
    {
        public SenateTab()
        {
            InitializeComponent();

            Group = "教務作業";
            helpContentPanel1.Naviate("http://sites.google.com/a/ischool.com.tw/emba_ntu");
            
            this.Load += new System.EventHandler(this.Form_Load);
        }

        public HelpContentPanel helpContentPanel1;

        private static SenateTab _DFM_admin;

        private void Form_Load(object sender, EventArgs e)
        {
        }
        public static SenateTab Instance
        {
            get { if (_DFM_admin == null) _DFM_admin = new SenateTab(); return _DFM_admin; }
        }

        private void InitializeComponent()
        {
            this.helpContentPanel1 = new EMBACore.Forms.HelpContentPanel();
            this.ContentPanePanel.SuspendLayout();
            this.SuspendLayout();
            // 
            // ContentPanePanel
            // 
            this.ContentPanePanel.Controls.Add(this.helpContentPanel1);
            this.ContentPanePanel.Location = new System.Drawing.Point(0, 163);
            this.ContentPanePanel.Size = new System.Drawing.Size(1016, 504);
            // 
            // helpContentPanel1
            // 
            this.helpContentPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.helpContentPanel1.Location = new System.Drawing.Point(0, 0);
            this.helpContentPanel1.Name = "helpContentPanel1";
            this.helpContentPanel1.Size = new System.Drawing.Size(1016, 504);
            this.helpContentPanel1.TabIndex = 0;
            // 
            // SenateTab
            // 
            this.Name = "SenateTab";
            this.ContentPanePanel.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        public void ReSet(string url)
        {
            helpContentPanel1.Naviate(url);
        }

        public void GoBack()
        {
            helpContentPanel1.GoBack();
        }

        public void GoForward()
        {
            helpContentPanel1.GoForward();
        }
        public void URLRefresh()
        {
            helpContentPanel1.URLRefresh();
        }
        public void URLStop()
        {
            helpContentPanel1.URLStop();
        }
    }
}
