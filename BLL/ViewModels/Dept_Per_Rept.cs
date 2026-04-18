using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ViewModels
{
    public class Term_Group
    {
        public string branch_code { get; set; }
        public string branch_name { get; set; }

    }

    public class Dept_Per_Rept
    {
        public int DeptID { get; set; }
        public string DepartmentName { get; set; }

        public int PresentCount { get; set; }
        public int LeaveCount { get; set; }
        public int AbsentCount { get; set; }

        public int OnTimeCount { get; set; }
        public int LateCount { get; set; }

        public int OutTimeCount { get; set; }
        public int EarlyCount { get; set; }

        public decimal PresentPercent { get; set; }
        public decimal LeavePercent { get; set; }
        public decimal AbsentPercent { get; set; }

        public decimal OnTimePercent { get; set; }
        public decimal LatePercent { get; set; }

        public decimal OutTimePercent { get; set; }
        public decimal EarlyPercent { get; set; }

        //public string Per_PresentCount { get; set; }
        //public string Per_LeaveCount { get; set; }
        //public string Per_AbsentCount { get; set; }

        //public string Per_OnTimeCount { get; set; }
        //public string Per_LateCount { get; set; }
        //public string Per_EarlyCount { get; set; }
    }
}
