using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FISCA.UDT;
using System.Xml;

namespace EMBACore.Forms.SeatTable
{
    public class ClassRoomLayout
    {
        public string UID { get { return this.udtLayout.UID; } }
        public string ClassRoomName { get; set; }
        public int XCount { get; set; }
        public int YCount  { get; set; }
        public List<CellCoordinate> SeatCells { get; set; }
        public List<CellCoordinate> BoundaryCells { get; set; }

        private UDT_ClassRoomLayout udtLayout;

        private Dictionary<string, CellCoordinate> dicSeatCells ;

        public ClassRoomLayout()
        {
        }

        public ClassRoomLayout(UDT_ClassRoomLayout udtLayout)
        {
            this.ClassRoomName = udtLayout.ClassroomName;
            this.XCount = udtLayout.XCount;
            this.YCount = udtLayout.YCount;
            fillSeatCells(udtLayout.SeatCells);
            fillBoundayCells(udtLayout.Boundary_cells);
            this.udtLayout = udtLayout;
        }

        private void fillSeatCells(string cellXml)
        {
            this.SeatCells = new List<CellCoordinate>();
            this.dicSeatCells = new Dictionary<string, CellCoordinate>();
            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.LoadXml("<Points>" + cellXml + "</Points>");
            foreach (XmlElement elmPoint in xmlDoc.DocumentElement.SelectNodes("P"))
            {
                bool enabled = true ;
                if (!string.IsNullOrWhiteSpace(elmPoint.GetAttribute("enabled"))) 
                {
                    if (elmPoint.GetAttribute("enabled").ToUpper() == "FALSE")
                        enabled = false  ;
                }
                CellCoordinate cell = new CellCoordinate(int.Parse(elmPoint.GetAttribute("x")), int.Parse(elmPoint.GetAttribute("y")), elmPoint.GetAttribute("content"), enabled);
                this.SeatCells.Add(cell);
                string key = cell.X.ToString() + "_" + cell.Y.ToString();
                if (!this.dicSeatCells.ContainsKey(key))
                    this.dicSeatCells.Add(key, cell);
            }
        }

        public CellCoordinate GetCell(int x, int y)
        {
            CellCoordinate result = null;

            string key = x.ToString() + "_" + y.ToString();
            if (this.dicSeatCells.ContainsKey(key))
                result = this.dicSeatCells[key];

            return result;
        }
        public CellCoordinate GetCell(string x, string y)
        {
            CellCoordinate result = null;
            if (!string.IsNullOrWhiteSpace(x) && !string.IsNullOrWhiteSpace(y))
            {
                string key = x.ToString() + "_" + y.ToString();
                if (this.dicSeatCells.ContainsKey(key))
                    result = this.dicSeatCells[key];
            }

            return result;
        }

        public UDT_ClassRoomLayout GetUDTLayout()
        {
            return this.udtLayout;
        }

        private void fillBoundayCells(string cellXml)
        {
            this.BoundaryCells = new List<CellCoordinate>();
            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.LoadXml("<Points>" + cellXml + "</Points>");
            foreach (XmlElement elmPoint in xmlDoc.DocumentElement.SelectNodes("P"))
            {
                CellCoordinate cell = new CellCoordinate(int.Parse(elmPoint.GetAttribute("x")), int.Parse(elmPoint.GetAttribute("y")), elmPoint.GetAttribute("content"));
                this.BoundaryCells.Add(cell);
            }
        }

        public override string ToString()
        {
            return this.ClassRoomName;
        }
    }
}
