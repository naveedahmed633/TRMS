using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ViewModels
{
    public class MonthlyReport
    {
        public string employeeCode { get; set; }
        public string first_name { get; set; }
        public string last_name { get; set; }
        public string dept_name { get;set; }
        public string desg_name { get; set; }
        public string camp_name { get; set; }
        public int present { get; set; }
        public int ontime { get; set; }
        public int late { get; set; }
        public int leave { get; set; }
        public int earlyGone { get; set; }
        public int absent { get; set; }
    }
}
