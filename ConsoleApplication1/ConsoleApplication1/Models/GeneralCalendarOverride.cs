namespace DLL.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public  class GeneralCalendarOverride
    {
        public int GeneralCalendarOverrideId { get; set; }

        public DateTime? date { get; set; }

        public string reason { get; set; }

        public bool isGazettedHoliday { get; set; }

        public bool active { get; set; }

        public virtual Shift Shift { get; set; }
    }
}
