namespace DLL.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public class AttendanceRemarksRegister
    {
        public int AttendanceRemarksRegisterId { get; set; }

        public DateTime? time_in { get; set; }

        public DateTime? time_out { get; set; }

        public int? remarks_r_id { get; set; }

        public bool active { get; set; }

        public virtual Remark Remark { get; set; }
    }
}
