namespace DLL.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public class GroupCalendar
    {
        public int GroupCalendarId { get; set; }

        public int year { get; set; }

        public string name { get; set; }
        
        public bool active { get; set; }
        public virtual Shift sunday { get; set; }
        public virtual Shift monday { get; set; }
        public virtual Shift tuesday { get; set; }
        public virtual Shift wednesday { get; set; }
        public virtual Shift thursday { get; set; }
        public virtual Shift friday { get; set; }
        public virtual Shift saturday { get; set; }
        public virtual Group Group { get; set; }
        public virtual Shift generalShift { get; set; }

        public virtual List<GroupCalendarOverride> calendarOverrides { get; set; }
        
    }
}
