using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace EMBACore.Forms.SeatTable
{
    public class AssignStudentEventArgs: EventArgs
    {
        public AssignStudentEventArgs()
            : base()
        {
        }
        public SeatTableStudent Student { get; set; }

        public CellCoordinate Coordinate {get; set ;}
    }
}
