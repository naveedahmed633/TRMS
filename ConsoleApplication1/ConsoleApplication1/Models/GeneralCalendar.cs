namespace DLL.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public  class GeneralCalendar
    {
        public int GeneralCalendarId { get; set; }

        public int year { get; set; }

        public string name { get; set; }

        public bool active { get; set; }

        public virtual Shift Shift { get; set; }

        public virtual Shift Shift1 { get; set; }

        public virtual Shift Shift2 { get; set; }

        public virtual Shift Shift3 { get; set; }

        public virtual Shift Shift4 { get; set; }

        public virtual Shift Shift5 { get; set; }

        public virtual Shift Shift6 { get; set; }

        public virtual Shift generalShift { get; set;}

        public virtual List<GeneralCalendarOverride> calendarOverrides { get; set; }

    }
}
