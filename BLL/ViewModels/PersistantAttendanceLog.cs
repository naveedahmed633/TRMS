using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ViewModels
{
    /**
     * This is a special view model and is only used when 
     * the page for employee creation is loaded. Its main purpose
     * is to fill the different drop downs with appropriate values.
     */
    public class PersistentAttendanceLog
    {
        public int att_id { get; set; }
        public DateTime? time_in { get; set; }
        public DateTime? time_out { get; set; }
        public DateTime? time_start { get; set; }
        public DateTime? time_end { get; set; }
        public bool active { get; set; }
    }
}
