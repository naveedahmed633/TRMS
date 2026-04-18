using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ViewModels;

namespace BLL.ViewModels
{
    public class FilteredAttendanceReportDepartmentWise
    {
        public FilteredAttendanceReportDepartmentWise()
        {
            logs = new List<ConsolidatedAttendanceDepartmentWise>();
        }
        public int id { get; set; }
        public List<ConsolidatedAttendanceDepartmentWise> logs { get; set; } 
    }
}
