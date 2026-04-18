using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ViewModels
{
    public class LeaveSession
    {
        public int id { get; set; }

        public int EmployeeId { get; set; }

        public string EmployeeCode { get; set; }

        public int YearId { get; set; }

        public DateTime SessionStartDate { get; set; }

        public DateTime SessionEndDate { get; set; }

        public string actions { get; set; }

        public string str_SessionStartDate { get; set; }
        public string str_EmpTypeName { get; set; }

        public string str_SessionEndDate { get; set; }

        public string fullname { get; set; }

        public int SickLeaves { get; set; }
        public int CasualLeaves { get; set; }
        public int AnnualLeaves { get; set; }
        public int OtherLeaves { get; set; }

        public int LeaveType01 { get; set; }
        public int LeaveType02 { get; set; }
        public int LeaveType03 { get; set; }
        public int LeaveType04 { get; set; }
        public int LeaveType05 { get; set; }
        public int LeaveType06 { get; set; }
        public int LeaveType07 { get; set; }
        public int LeaveType08 { get; set; }
        public int LeaveType09 { get; set; }
        public int LeaveType10 { get; set; }
        public int LeaveType11 { get; set; }
    }
}
