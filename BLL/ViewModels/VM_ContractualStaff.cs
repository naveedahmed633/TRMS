using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLL.ViewModels
{
    public  class VM_ContractualStaff
    {
        public int ContractualStaffId { get; set; }

        public string employee_code { get; set; }

        public string employee_name { get; set; }

        public string email { get; set; }

        public string address { get; set; }

        public string mobile_no { get; set; }

        public bool active { get; set; }

        public string department { get; set; }

        public string designation { get; set; }

        public string function { get; set; }

        public string grade { get; set; }

        public string Group { get; set; }

        public string location { get; set; }

        public string region { get; set; }

        public string company { get; set; }
        public string date_of_joining { get; set; }
        public string date_of_leaving { get; set; }
    }
}
