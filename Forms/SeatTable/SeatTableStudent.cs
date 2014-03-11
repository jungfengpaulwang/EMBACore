using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace EMBACore.Forms.SeatTable
{
    [Serializable()]
    public class SeatTableStudent : ISerializable
    {
        public string ID{get; set;}
        public string Name {get; set; }
        public string StudentNumber {get; set; }
        public System.Drawing.Image Photo {get; set;}
        public string SeatX { get; set; }
        public string SeatY { get; set; }
        public string CourseExt_UID { get; set; }

        public SeatTableStudent(string id, string studNumber, string name, System.Drawing.Image photo , string seatX, string seatY, string uid)
        {
            this.ID = id ;
            this.Name = name;
            this.Photo = photo ;
            this.StudentNumber = studNumber;
            this.SeatX = seatX;
            this.SeatY = seatY;
            this.CourseExt_UID = uid;
        }

        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue("ID", this.ID);
            info.AddValue("Name", this.Name);
            info.AddValue("Photo", this.Photo);
        }

        public override string ToString()
        {
            return string.Format("{0} {1} {2}", this.StudentNumber, this.Name, (string.IsNullOrEmpty(SeatX) ? "" : string.Format("({0}, {1})", SeatX, SeatY)) )  ;
        }
    }
}
