using System;
using System.Collections.Generic;
using FISCA.Presentation.Controls;
using FISCA.UDT;
using System.Windows.Forms;
using System.Linq;

namespace EMBACore.Forms
{
    public partial class ExperienceDataSourceManagement : BaseForm
    {
        private string _ItemCategory;
        private AccessHelper Access;

        //private List<UDT.ExperienceDataSource> oExperienceDataSources;

        public ExperienceDataSourceManagement(string ItemCategory)
        {
            InitializeComponent();

            this._ItemCategory = ItemCategory;
            Access = new AccessHelper();
            //oExperienceDataSources = new List<UDT.ExperienceDataSource>();

            this.Load += new EventHandler(ExperienceDataSourceManagement_Load);
        }

        public ExperienceDataSourceManagement():this(string.Empty)
        {

        }

        private void ExperienceDataSourceManagement_Load(object sender, EventArgs e)
        {
            this.Text = (string.IsNullOrEmpty(this._ItemCategory) ? "經歷" : this._ItemCategory) + "內容管理";
            this.InitExperienceDataSouce(this._ItemCategory);
            if (!string.IsNullOrEmpty(this._ItemCategory))
                this.dgvData.Columns[0].Visible = false;
            else
                this.dgvData.Columns[1].Visible = true;
        }

        private void InitExperienceDataSouce(string ItemCategory)
        {
            List<UDT.ExperienceDataSource> ExperienceDataSources = new List<UDT.ExperienceDataSource>();
            
            if (string.IsNullOrEmpty(ItemCategory))
                ExperienceDataSources = Access.Select<UDT.ExperienceDataSource>();
            else
                ExperienceDataSources = Access.Select<UDT.ExperienceDataSource>(string.Format("item_category='{0}'", ItemCategory));

            if (ExperienceDataSources.Count > 0)
                ExperienceDataSources = ExperienceDataSources.OrderBy(x => x.ItemCategory).ThenBy(x => x.Item).ToList();

            this.dgvData.Rows.Clear();
            //oExperienceDataSources.Clear();
            foreach (UDT.ExperienceDataSource experienceDataSource in ExperienceDataSources)
            {
                List<object> source = new List<object>();

                source.Add(experienceDataSource.ItemCategory);
                source.Add(experienceDataSource.Item);
                source.Add(experienceDataSource.NotDisplay);

                int idx = this.dgvData.Rows.Add(source.ToArray());
                this.dgvData.Rows[idx].Tag = experienceDataSource;

                //oExperienceDataSources.Add(experienceDataSource);
            }
        }

        private void Save_Click(object sender, EventArgs e)
        {
            List<UDT.ExperienceDataSource> uRecords = new List<UDT.ExperienceDataSource>();
            List<UDT.ExperienceDataSource> nRecords = new List<UDT.ExperienceDataSource>();
            //List<UDT.ExperienceDataSource> dRecords = new List<UDT.ExperienceDataSource>(); 
            foreach (DataGridViewRow dataGridViewRow in this.dgvData.Rows)
            {
                if (dataGridViewRow.IsNewRow)
                    continue;

                if (dataGridViewRow.Tag == null)
                {
                    UDT.ExperienceDataSource experienceDataSource = new UDT.ExperienceDataSource();

                    experienceDataSource.ItemCategory = (string.IsNullOrEmpty(this._ItemCategory) ? dataGridViewRow.Cells[0].Value + "" : this._ItemCategory);
                    experienceDataSource.Item = dataGridViewRow.Cells[1].Value + "";
                    experienceDataSource.NotDisplay = (dataGridViewRow.Cells[2].Value == null ? false : bool.Parse(dataGridViewRow.Cells[2].Value + ""));
                    nRecords.Add(experienceDataSource);
                }
                else
                {
                    UDT.ExperienceDataSource experienceDataSource = dataGridViewRow.Tag as UDT.ExperienceDataSource;

                    experienceDataSource.ItemCategory = (string.IsNullOrEmpty(this._ItemCategory) ? dataGridViewRow.Cells[0].Value + "" : this._ItemCategory);
                    experienceDataSource.Item = dataGridViewRow.Cells[1].Value + "";
                    experienceDataSource.NotDisplay = (dataGridViewRow.Cells[2].Value == null ? false : bool.Parse(dataGridViewRow.Cells[2].Value + ""));
                    uRecords.Add(experienceDataSource);
                }
            }
            nRecords.SaveAll();
            uRecords.SaveAll();
            if (!string.IsNullOrEmpty(this._ItemCategory))
            {
                UDT.ExperienceDataSource.RaiseAfterUpdateEvent();
                this.Close();
            }
            else
                this.InitExperienceDataSouce(this._ItemCategory);
            //oExperienceDataSources.ForEach((x) =>
            //{
            //    if (uRecords.Where(y => y.UID == x.UID).Count() == 0)
            //        dRecords.Add(x);
            //});

        }

        private void Exit_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}