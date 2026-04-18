namespace DLL.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public class GroupCalendarOverride
    {
        public int GroupCalendarOverrideId { get; set; }

        public DateTime? date { get; set; }

        public string reason { get; set; }

        public bool isGazettedHoliday { get; set; }

        public bool active { get; set; }

        public virtual GroupCalendar GroupCalendar { get; set; }

        public virtual Shift Shift { get; set; }
    }
}
