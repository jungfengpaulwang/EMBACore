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
using System.IO;
using FISCA.Data;
using Aspose.Cells;

namespace EMBACore.Forms.SeatTable
{
    public partial class frmSeatTable : BaseForm
    {
        private int cellWidth = 50;
        private int cellHeight = 55;
        private int boundaryWidth = 25;

        private Dictionary<string, SeatTableStudent> students = new Dictionary<string, SeatTableStudent>();
        private Dictionary<string, CellCoordinate> stud_coords = new Dictionary<string, CellCoordinate>();  //學生的座位
        private ClassRoomLayout layout; //課程的座位表樣版
        private List<ClassRoomLayout> allLayouts;   //所有的座位表樣版
        private Dictionary<CellCoordinate, ucContentCell> dicLayoutCells = new Dictionary<CellCoordinate,ucContentCell>();
        private UDT.CourseExt currentCourseExt;
        private List<string> updatedStudentIDs = new List<string>();    //有更動座位的學生清單。這樣儲存時只要儲存這些學生座位即可，增加效能用的。

        private string currentCourseID;

        public frmSeatTable()
        {
            InitializeComponent();
        }

        private void frmSeatTable_Load(object sender, EventArgs e)
        {
            this.MaximizeBox = true;

            if (K12.Presentation.NLDPanels.Course.SelectedSource.Count < 1)
            {
                MessageBox.Show("請選擇課程");
                return;
            }

            //1. 取得目前選取的課程
            AccessHelper ah = new AccessHelper();
            this.currentCourseID = K12.Presentation.NLDPanels.Course.SelectedSource[0];
            K12.Data.CourseRecord rec = K12.Data.Course.SelectByID(this.currentCourseID);
            this.expandablePanel1.TitleText = rec.Name;

            List<UDT.CourseExt> course_exts = ah.Select<UDT.CourseExt>("ref_course_id=" + this.currentCourseID);
            if (course_exts.Count == 0)
                this.currentCourseExt = null;
            else
            {
                this.currentCourseExt = course_exts[0];
                this.expandablePanel1.TitleText = rec.Name + " ( " + this.currentCourseExt.SubjectCode + " )";
            }

            //2. 取得所有的座位表樣版
            this.allLayouts = new List<ClassRoomLayout>();
            List<UDT_ClassRoomLayout> udtLayouts =(new AccessHelper()).Select<UDT_ClassRoomLayout>();
            foreach(UDT_ClassRoomLayout udtL in udtLayouts)
            {
                ClassRoomLayout lt = new ClassRoomLayout(udtL);
                this.allLayouts.Add(lt);
                this.cboLayouts.Items.Add(lt);
                //this.cboLayouts.SelectedIndex = 0;
            }

            //3. 取得此課程的座位表樣版
             this.layout = this.allLayouts[0];  //預設 layout
            List<UDT.CourseExt> course_ext = ah.Select<UDT.CourseExt>("ref_course_id=" + this.currentCourseID );
            if ((course_ext.Count ==0) || (course_ext[0].ClassroomLayoutID == null)) {
                //採預設 layout，所以不作任何事。
            }
            else {
                foreach(ClassRoomLayout crl in this.allLayouts) {
                    if (this.currentCourseExt.ClassroomLayoutID.ToString() ==  crl.UID ) {
                        this.layout = crl ;
                        break ;
                    }
                }
            }
            this.currentCourseExt.ClassroomLayoutID = int.Parse(this.layout.UID);

            //4. 取得修課學生
            this.GetSCAttendExt();

            //5. 指定課程的座位樣版
            this.cboLayouts.SelectedItem = this.layout ; //會觸發 selectedindex_change 事件
        }

        //把 Layout 畫出來
        private void DrawClassroomLayout()
        {
            //draw content cells;
            this.pnlContainer.SuspendLayout();
            
            this.pnlContainer.Controls.Clear();
            this.dicLayoutCells.Clear();
            this.drawContentCells();

            //draw boundary cells;            
            this.drawBoundaryCells();

            this.pnlContainer.ResumeLayout();
        }

        private void drawBoundaryCells()
        {
            foreach (CellCoordinate cell in this.layout.BoundaryCells)
            {
                ucBourdaryCell p = new ucBourdaryCell();
                p.Coordinate = cell;

                p.Width = boundaryWidth;
                p.Height = cellHeight;
                p.Top = (this.layout.YCount - cell.Y - 1) * cellHeight;
                if (cell.X == 0)
                    p.Left = 0;
                else
                    p.Left = boundaryWidth + cellWidth * (this.layout.XCount);

                p.BorderStyle = BorderStyle.FixedSingle;
                p.BackColor = Color.White;

                this.pnlContainer.Controls.Add(p);
            }
        }
        private void drawContentCells()
        {
            
            foreach (CellCoordinate cell in this.layout.SeatCells)
            {
                ucContentCell p = new ucContentCell();
                p.Coordinate = cell;
                p.Width = cellWidth;
                p.Height = cellHeight;
                p.Top = (this.layout.YCount - cell.Y - 1) * cellHeight;
                p.Left = boundaryWidth + cellWidth * (cell.X - 1);
                p.BackColor = Color.White;
                p.BorderStyle = BorderStyle.FixedSingle;
                p.OnAssignStudent += new AssignStudentEventHandler(p_OnAssignStudent);
                p.OnRemoveStudent += new EventHandler(p_OnRemoveStudent);
                this.pnlContainer.Controls.Add(p);
                this.dicLayoutCells.Add(cell, p);
            }
        }

        void p_OnRemoveStudent(object sender, EventArgs e)
        {
            ucContentCell cell = (ucContentCell)sender;

            string origStudID = cell.Student.ID;
            cell.Student.SeatX = "";
            cell.Student.SeatY = "";
            cell.ClearData();

            if (!updatedStudentIDs.Contains(origStudID))
                updatedStudentIDs.Add(origStudID);    //把有修改的學生ID紀錄起來，以備儲存時候用。

            this.stud_coords.Remove(origStudID);
            this.fillListBox();
        }

        void p_OnAssignStudent(object sender, AssignStudentEventArgs e)
        {
            //1. 檢查目地端是否有指定學生，
            ucContentCell targetCell = this.dicLayoutCells[e.Coordinate];
            if (targetCell.Student != null)
            {
                string msg = string.Format("是否將此座位從『{0}』換成『{1}』？", targetCell.Student.Name, e.Student.Name );
                if (MessageBox.Show(msg, "注意", MessageBoxButtons.YesNo, MessageBoxIcon.Information) == System.Windows.Forms.DialogResult.No)
                    return;
                else  //清除位置上原本的學生
                {
                    string origStudID = targetCell.Student.ID;
                    targetCell.Student.SeatX = "";
                    targetCell.Student.SeatY = "";
                    targetCell.ClearData();

                    if (!updatedStudentIDs.Contains(origStudID))
                        updatedStudentIDs.Add(origStudID);    //把有修改的學生ID紀錄起來，以備儲存時候用。

                    this.stud_coords.Remove(origStudID);
                }
            }

            //2. 檢查此學生是否有排過座位，如果是，則把原來的座位資訊清除，然後指定到新座位。
            string studID = e.Student.ID;
            if (this.stud_coords.ContainsKey(studID))
            {
                //清除這位學生原來座位的資訊。
                CellCoordinate old_coord = this.stud_coords[studID];
                ucContentCell oldCell = this.dicLayoutCells[old_coord];
                if (oldCell.Student != null)
                {
                    string oldStudID = oldCell.Student.ID;
                    oldCell.Student.SeatX = "";
                    oldCell.Student.SeatY = "";
                    oldCell.ClearData();

                    if (!updatedStudentIDs.Contains(oldStudID))
                        updatedStudentIDs.Add(oldStudID);    //把刪除位置的學生ID紀錄起來，以備儲存時候用。

                    if (this.stud_coords.ContainsKey(oldStudID))
                        this.stud_coords.Remove(oldStudID);
                }
            }

            //3. 指定到新位置
            ucContentCell newCell = this.dicLayoutCells[e.Coordinate];
            newCell.Student = e.Student;
            e.Student.SeatX = e.Coordinate.X.ToString();
            e.Student.SeatY = e.Coordinate.Y.ToString();
            this.stud_coords.Add(studID, e.Coordinate);
            if (!updatedStudentIDs.Contains(e.Student.ID))
                updatedStudentIDs.Add(e.Student.ID);    //把刪除位置的學生ID紀錄起來，以備儲存時候用。

            //4. Refresh Listbox
            this.fillListBox();
        }

        //取得修課學生清單
        private void GetSCAttendExt()
        {
            this.students = new Dictionary<string, SeatTableStudent>();
            this.stud_coords = new Dictionary<string, CellCoordinate>();

           //取得修課學生的系統編號、學號、姓名、座位座標等。
            string strSQL = string.Format("select stu.id, stu.student_number, stu.name, stu.freshman_photo, ext.seat_x, ext.seat_y, ext.uid from $ischool.emba.scattend_ext ext inner join student stu on ext.ref_student_id = stu.id where ext.ref_course_id={0}", this.currentCourseID);
            QueryHelper q = new QueryHelper();
            DataTable dt = q.Select(strSQL);
            foreach (DataRow dr in dt.Rows)
            {
                string studID = dr["id"].ToString();
                string studNumber = dr["student_number"].ToString();
                string name = dr["name"].ToString();
                string freshman_photo = dr["freshman_photo"].ToString();
                string seat_x = dr["seat_x"].ToString();
                string seat_y = dr["seat_y"].ToString();
                string uid = dr["uid"].ToString();

                SeatTableStudent stud = new SeatTableStudent(studID, studNumber,  name, Base64ToImage(freshman_photo), seat_x, seat_y, uid);
                // this.listBox1.Items.Add(stud);
                this.students.Add(stud.ID, stud);
            }

            this.fillListBox();
            this.fillStudentSeatPosition();
        }

        //將學生的座位資訊填到格子裡
        private void fillStudentSeatPosition()
        {
            this.stud_coords.Clear();
            foreach (SeatTableStudent stud in this.students.Values)
            {
                CellCoordinate coord = this.layout.GetCell(stud.SeatX, stud.SeatY);
                if (coord != null)
                {
                    this.stud_coords.Add(stud.ID, coord);
                    if (this.dicLayoutCells.ContainsKey(coord))
                    {
                        this.dicLayoutCells[coord].Student = stud;
                    }
                }
            }
        }
        
        //將修課學生資訊填入 Listbox
        private void fillListBox()
        {
            this.listBox1.Items.Clear();
            List<SeatTableStudent> students = this.students.Values.ToList<SeatTableStudent>();
            students.Sort(delegate(SeatTableStudent stu1, SeatTableStudent stu2) {
                return  stu1.StudentNumber.CompareTo(stu2.StudentNumber);
            });
            
            foreach (SeatTableStudent stud in students)
                this.listBox1.Items.Add(stud);
        }

        private Image Base64ToImage(string base64)
        {
            Image image = null;
            try
            {
                byte[] imageBytes = Convert.FromBase64String(base64);
                MemoryStream ms = new MemoryStream(imageBytes, 0, imageBytes.Length);
                // Convert byte[] to Image
                ms.Write(imageBytes, 0, imageBytes.Length);
                image = Image.FromStream(ms, true);
            }
            catch (Exception)
            {
            }
            return image;
        }

        private void listBox1_MouseDown(object sender, MouseEventArgs e)
        {
            int indexOfItem = listBox1.IndexFromPoint(e.X, e.Y);
            if (indexOfItem >= 0 && indexOfItem
              < listBox1.Items.Count)  // check we clicked down on a string
            {
                // Set allowed DragDropEffect to Copy selected 
                // from DragDropEffects enumberation of None, Move, All etc.
                SeatTableStudent data = (SeatTableStudent)listBox1.Items[indexOfItem];
                listBox1.DoDragDrop(data, DragDropEffects.Copy);
            }
        }

        private void cboLayouts_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cboLayouts.SelectedItem == null)
                return ;

            this.layout = (ClassRoomLayout)cboLayouts.SelectedItem;
            this.pnlContainer.Text = this.layout.ClassRoomName;

            //畫出課程教室座位分佈圖
            DrawClassroomLayout();

            //將修課學生的座位資訊填入座位樣版中
            this.fillStudentSeatPosition();

            //填入修課學生
            this.fillListBox();
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            List<ActiveRecord> records = new List<ActiveRecord>();
            //1. Course Ext
            if (this.currentCourseExt == null)
            {
                this.currentCourseExt = new UDT.CourseExt();
                this.currentCourseExt.CourseID = int.Parse(this.currentCourseID);
            }
            //更新 CourseExt 的 ClassroomTemplateID 
            this.currentCourseExt.ClassroomLayoutID = int.Parse(this.layout.UID);
            records.Add(this.currentCourseExt);

            //2. 更新座位的學生
            if (this.updatedStudentIDs.Count > 0)
            {
                List<string> uids = new List<string>();
                foreach (string studID in this.updatedStudentIDs)
                {
                    SeatTableStudent stud = this.students[studID];
                    uids.Add(stud.CourseExt_UID); //
                }

                AccessHelper ah = new AccessHelper();
                List<UDT.SCAttendExt> exts = ah.Select<UDT.SCAttendExt>(uids);

                foreach (UDT.SCAttendExt ext in exts)
                {
                    SeatTableStudent stud = this.students[ext.StudentID.ToString()];
                    if (string.IsNullOrWhiteSpace(stud.SeatX))
                        ext.SeatX = null;
                    else
                        ext.SeatX = int.Parse(stud.SeatX);

                    if (string.IsNullOrWhiteSpace(stud.SeatY))
                        ext.SeatY = null;
                    else
                        ext.SeatY = int.Parse(stud.SeatY);

                    records.Add(ext);
                }
            }

            (new AccessHelper()).SaveAll(records);
            Util.ShowMsg("儲存完成","");
        }

        //列印
        private void btnPrint_Click(object sender, EventArgs e)
        {
            //0. 使用者選擇檔名
            SaveFileDialog sfd = new SaveFileDialog();
            sfd.Title = "另存新檔";
            sfd.FileName = "座位表_" + this.layout.ClassRoomName +  "_" + this.expandablePanel1.TitleText + ".xls";
            sfd.Filter = "Excel 2003 相容檔案 (*.xls)|*.xls|所有檔案 (*.*)|*.*";
            DialogResult dr = sfd.ShowDialog();
            if (dr != System.Windows.Forms.DialogResult.OK)
                return;
            
            //1. Open Excel File
            Aspose.Cells.Workbook wb = new Aspose.Cells.Workbook();
            //  讀取樣版檔
            if (this.layout.GetUDTLayout().ExcelTemplate =="B")
                wb.Open(new MemoryStream(EMBACore.Properties.Resources.座位表_100B));
            else
                wb.Open(new MemoryStream(EMBACore.Properties.Resources.座位表_100A));

            //  讀取樣版工作表
            Worksheet templateSheet = wb.Worksheets[0];

            // 複製樣版
            int instanceSheetIndex = wb.Worksheets.AddCopy("範本");
            Worksheet instanceSheet = wb.Worksheets[instanceSheetIndex];
            instanceSheet.Name = this.expandablePanel1.TitleText;

            //填入學生座位
            foreach (string studID in this.stud_coords.Keys)
            {
                //取得學生資料
                if (this.students.ContainsKey(studID))
                {
                    SeatTableStudent stud = this.students[studID];

                    //取得學生位置座標
                    CellCoordinate coord = this.stud_coords[studID];

                    //將座標轉換為 Excel 格子位置
                    int offsetX = coord.X;
                    int offsetY = (this.layout.YCount - coord.Y -1) * 2 + 1 ;

                    //放置照片
                    if (stud.Photo != null)
                    {
                        MemoryStream ms = new MemoryStream();
                        stud.Photo.Save(ms, System.Drawing.Imaging.ImageFormat.Png);
                        Cell cell = instanceSheet.Cells[offsetY, offsetX];
                        instanceSheet.Pictures.Add(cell.Row, cell.Column, cell.Row + 1, cell.Column + 1, ms);
                    }                  
                    //寫入姓名
                    instanceSheet.Cells[offsetY+1, offsetX].PutValue(stud.Name);
                }

            } //end of foreach loop

            try
            {
                wb.Worksheets.RemoveAt("範本");
                wb.Save(sfd.FileName);
                if (System.IO.File.Exists(sfd.FileName))
                    System.Diagnostics.Process.Start(sfd.FileName);                
            }
            catch (Exception ex)
            {
                Util.ShowMsg(ex.Message,"注意");
            }

        }// end of function

    }
}
