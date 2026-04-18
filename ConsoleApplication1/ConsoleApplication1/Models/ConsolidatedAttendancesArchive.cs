using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DLL.Models
{
    public class ConsolidatedAttendancesArchive
    {

        public int id { get; set; }

        public int ConsolidatedAttendanceId { get; set; }
        public DateTime? date { get; set; }
        public DateTime? time_in { get; set; }
        public DateTime? time_out { get; set; }
        public string status_in { get; set; }
        public string status_out { get; set; }
        public string final_remarks { get; set; }
        public virtual Employee employee { get; set; }
        public string terminal_in { get; set; }
        public string terminal_out { get; set; }
        public bool active { get; set; }
        //public virtual List<ManualAttendance> manualAttendances { get; set; }

    }
}
