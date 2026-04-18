using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DLL;
namespace ViewModels
{
    /* This View Model used to mark manual attendance 
     
    */
    public class ManualAttendance
    {
        public string employee_code { get; set; }
        // date pickers will pick date only.
        public DateTime? time_in_from { get; set; }
        public DateTime? time_in_to { get; set; }
        public string remarks { get; set; }
    }
}
