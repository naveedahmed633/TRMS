namespace DLL.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public class ManualAttendance
    {

        public int ManualAttendanceId { get; set; }
        
        public bool active { get; set; }

        //public virtual ConsolidatedAttendance ConsolidatedAttendance{ get; set; }
        public virtual ConsolidatedAttendance ConsolidatedAttendance { get; set; }

        public virtual Employee employee { get; set; }

        public string remarks { get; set; }

        public string WhoMark { get; set; }

    }
}
