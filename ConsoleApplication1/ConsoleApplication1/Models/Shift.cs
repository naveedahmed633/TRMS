namespace DLL.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;

    public class Shift
    {
        public int ShiftId { get; set; }
        public int early_time { get; set; }
        public DateTime start_time { get; set; }
        public int late_time { get; set; }
        public int half_day { get; set; }
        public int shift_end { get; set; }
        public int day_end { get; set; }
        public string name { get; set; }
        public bool active { get; set; }

        //public int shift_hours { get; set; }
        public virtual List<Group> Groups { get; set; }
          
    }
}
