using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MarkAttendance1
{
    public class TempPersistentLog
    {
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
        public bool dirtyBit { get; set; }
        public bool active { get; set; }
    }
}
