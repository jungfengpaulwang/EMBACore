using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace EMBACore.Forms.SeatTable
{
    public partial class ucBourdaryCell : UserControl
    {
        private CellCoordinate coord;
        public CellCoordinate Coordinate
        { 
            get  {return this.coord ;} 
            set  {
                this.coord = value;
                this.panelEx1.Text = this.coord.ContentText;
            } 
        }

        public ucBourdaryCell()
        {
            InitializeComponent();
        }

        private void ucBourdaryCell_Load(object sender, EventArgs e)
        {

        }

        
    }
}
