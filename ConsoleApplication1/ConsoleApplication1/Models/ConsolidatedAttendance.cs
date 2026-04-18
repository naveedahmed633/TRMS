using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DLL.Models
{
    public class ConsolidatedAttendance
    {
        public int ConsolidatedAttendanceId { get; set; }
        public DateTime ? date { get; set; }
        public DateTime ? time_in { get; set; }
        public DateTime ? time_out { get; set; }
        public string status_in { get; set; }   
        public string status_out { get; set; }
        public string final_remarks { get; set; }
        public string campusname { get; set; }
        public string description { get; set; }
        public virtual Employee employee { get; set; }
       // public virtual OrganizationCampus orgcampus { get; set; }
        public string terminal_in { get; set; }
        public string terminal_out { get; set; }
        public bool active { get; set; }
        public int overtime { get; set; }
        public int overtime_status { get; set; }
        public int latetime { get; set; }
        public bool is_payroll_synced { get; set; }
        public virtual List<ManualAttendance> manualAttendances { get; set; }

        //public static ManualAttendance obj { get; set; }

    }
}
