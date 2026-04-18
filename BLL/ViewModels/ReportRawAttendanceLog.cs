using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ViewModels
{
    public class ReportRawAttendanceLog
    {
        [Key]
        public int id { get; set; }
        public string date { get; set; }
        public string employee_first_name { get; set; }
        public string employee_last_name { get; set; }
        public string employee_code { get; set; }
        public string time { get; set; }


    }
}
