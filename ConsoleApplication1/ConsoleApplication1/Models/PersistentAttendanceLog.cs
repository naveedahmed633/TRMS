namespace DLL.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public class PersistentAttendanceLog
    {
        // one to one relation, every employee
        // has one and only one persistant attendance log
        [Key,ForeignKey("Employee")]
        public int PersistentAttendanceLogId { get; set; }

        [MaxLength(50)]
        public string employee_code { get; set; }

        public DateTime? time_in { get; set; }
        public DateTime? time_out { get; set; }
        public DateTime? time_start { get; set; }
        public DateTime? time_end { get; set; }
        public DateTime? start_time { get; set; }
        public DateTime? late_time { get; set; }
        public DateTime? half_day { get; set; }
        public DateTime? shift_end { get; set; }
        public string terminal_in { get; set; }
        public string terminal_out { get; set; }
        public bool dirtyBit { get; set; }
        public bool active { get; set; }

        public virtual Employee Employee { get; set; }
    }
}
