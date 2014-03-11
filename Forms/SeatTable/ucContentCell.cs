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
    public delegate void AssignStudentEventHandler(object sender, AssignStudentEventArgs e);

    public partial class ucContentCell : UserControl
    {
        public event AssignStudentEventHandler OnAssignStudent;
        public event EventHandler OnRemoveStudent;

        private CellCoordinate coord;
        public CellCoordinate Coordinate {
            get {
                return this.coord;
            }
            set {
                this.coord = value;
                if (!this.coord.Enabled)
                {
                    this.label1.Text = this.coord.ContentText;
                    this.pictureBox1.Image = new Bitmap( Properties.Resources.forbidden  , this.pictureBox1.Width, this.pictureBox1.Height);
                    this.ucTransparentPanel1.AllowDrop = false;
                }
            }
        }

        private SeatTableStudent stud;
        public SeatTableStudent Student
        {
            get { return this.stud ;}
            set
            {
                this.stud = value;
                this.label1.Text = stud.Name;
                Image img = null ;

                if (stud.Photo != null) {
                    img = new Bitmap(stud.Photo, this.pictureBox1.Width, this.pictureBox1.Height);
                }
                this.pictureBox1.Image = img;
            }
        }

        

        public ucContentCell()
        {
            InitializeComponent();
        }

        public void ClearData()
        {
            this.stud = null;
            this.label1.Text = "";
            this.pictureBox1.Image = null;
        }

        private void ucContentCell_Load(object sender, EventArgs e)
        {
            this.AllowDrop = true;
            ((Control)this.pictureBox1).AllowDrop = true;
        }

        private void ucTransparentPanel1_DragDrop(object sender, DragEventArgs e)
        {
            SeatTableStudent student = (SeatTableStudent)e.Data.GetData("EMBACore.Forms.SeatTable.SeatTableStudent");

            if (this.OnAssignStudent != null)
            {
                AssignStudentEventArgs args = new AssignStudentEventArgs();
                args.Student = student;
                args.Coordinate = Coordinate;
                this.OnAssignStudent(this, args);
            }

            //this.Student = student;
            //MessageBox.Show(this.Student.Name);
        }

        private void ucTransparentPanel1_DragEnter(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent("EMBACore.Forms.SeatTable.SeatTableStudent") && this.Coordinate.Enabled)
            {
                e.Effect = DragDropEffects.Copy;
            }
            else
            {
                e.Effect = DragDropEffects.None;
            }
        }

        private void ucTransparentPanel1_DoubleClick(object sender, EventArgs e)
        {
            if (this.stud == null)
                return;

            if (this.OnRemoveStudent != null)
                this.OnRemoveStudent(this, EventArgs.Empty);
        }
        
    }

}
