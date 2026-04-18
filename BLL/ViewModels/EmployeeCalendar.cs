using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ViewModels
{
    public class EmployeeCalendar
    {
        public string employee_id { get; set; }
        public List<ViewModels.GeneralCalendarOverride> overrides { get; set; }
    }
}
