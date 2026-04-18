using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ViewModels
{
    public class GeoPhencingTerminal
    {
        [Key]
        public int id { get; set; }

        public int EmployeeId { get; set; }

        public string BranchesList { get; set; }

        public string TerminalsList { get; set; }

        public DateTime TrmCreateDate { get; set; }

        public string EmployeeCode { get; set; }

        public string FullName { get; set; }

        public string strTrmCreateDate { get; set; }

        public string actions { get; set; }

        public List<string> lstBranches { get; set; }

        public List<string> lstTerminals { get; set; }

        public string date { get; set; }
        public string time_in { get; set; }
        public string time_out { get; set; }
        public string status_in { get; set; }
        public string status_out { get; set; }
        public string final_remarks { get; set; }
        public string employee_code { get; set; }
        public string employee_first_name { get; set; }
        public string employee_last_name { get; set; }
        public string terminal_in { get; set; }
        public string terminal_out { get; set; }

        public int overtime { get; set; }
        public string overtime_status { get; set; }
        public string str_overtime { get; set; }

        public string overtime2 { get; set; }

        public string action { get; set; }

    }
}
