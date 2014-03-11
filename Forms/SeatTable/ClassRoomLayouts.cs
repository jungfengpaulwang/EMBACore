using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FISCA.UDT;

namespace EMBACore.Forms.SeatTable
{
    class ClassRoomLayouts
    {
        private static List<ClassRoomLayout> layouts;
        private static Dictionary<string, ClassRoomLayout> dicLayouts;

        public static void LoadData()
        {
            AccessHelper ah = new AccessHelper();
            foreach (UDT_ClassRoomLayout udtLayout in ah.Select<UDT_ClassRoomLayout>())
            {
                ClassRoomLayout layout = new ClassRoomLayout(udtLayout);
                layouts.Add(layout);
                dicLayouts.Add(layout.UID, layout);
            }
        }

        public static ClassRoomLayout GetLayout(string layoutID)
        {
            ClassRoomLayout result = null;
            if (dicLayouts.ContainsKey(layoutID))
                result = dicLayouts[layoutID];

            return result;
        }
    }
}
