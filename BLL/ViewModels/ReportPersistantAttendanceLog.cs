using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ViewModels
{
    /*
       *This View Model is used to load the view attendance page 
       *The View Model contains all attribute that will show on View attendance page
    */

    public class ReportPersistentAttendanceLog
    {
        [Key]
        public int id { get; set; }
        public string date { get; set; }
        public string employee_first_name { get; set; }
        public string employee_last_name { get; set; }
        public string employee_code { get; set; }
        public string time_in { get; set; }
        public string time_out { get; set; }
        public string terminal_in { get; set; }
        public string terminal_out { get; set; }

    }



    public class ReportPersistentAttendanceLogDD
    {
        [Key]
        public int id { get; set; }
        public DateTime? date { get; set; }
        public string employee_first_name { get; set; }
        public string employee_last_name { get; set; }
        public string employee_code { get; set; }
        public DateTime? time_in { get; set; }
        public DateTime? time_out { get; set; }
        public string terminal_in { get; set; }
        public string terminal_out { get; set; }


    }
}
