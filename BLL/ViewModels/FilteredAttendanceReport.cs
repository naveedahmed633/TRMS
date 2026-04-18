using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ViewModels;

namespace BLL.ViewModels
{
    public class FilteredAttendanceReport
    {
        public FilteredAttendanceReport()
        {
            logs = new List<ConsolidatedAttendanceLog>();
        }
        public int id { get; set; }
        public List<ConsolidatedAttendanceLog> logs { get; set; } 
    }
}
