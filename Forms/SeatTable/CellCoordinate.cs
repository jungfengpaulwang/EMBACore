using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace EMBACore.Forms.SeatTable
{
    public class CellCoordinate
    {
        public int X {get; set ;}
        public int Y {get; set; }
        public string ContentText { get; set; }

        private bool enabled = true;    //default
        public bool Enabled {
            get
            {
                return this.enabled;
            }
            set {
                this.enabled = value;
            }
        }

        public CellCoordinate(int x, int y) : this(x, y, "")
        {            
        }

        public CellCoordinate(int x, int y, string text) : this(x,y,text,true)
        {            
        }

        public CellCoordinate(int x, int y, string text ,bool enabled)
        {
            this.X = x;
            this.Y = y;
            this.ContentText = text;
            
            this.Enabled = enabled;
        }
    }
}
