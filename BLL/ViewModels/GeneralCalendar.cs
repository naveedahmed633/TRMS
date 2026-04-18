using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ViewModels
{
    public class GeneralCalendar
    {
        public int id { get; set; }

        public int year { get; set; }

        public Shift Shift { get; set; }

        public Shift Shift1 { get; set; }

        public Shift Shift2 { get; set; }

        public Shift Shift3 { get; set; }

        public Shift Shift4 { get; set; }

        public Shift Shift5 { get; set; }

        public Shift Shift6 { get; set; }

        public Shift generalShift { get; set; }

        public GeneralCalendarOverride[] generalOverrides { get; set; }

    }
}
