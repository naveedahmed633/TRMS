using DLL.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TimeTune;

namespace ViewModels
{
    /*
     * This View is used to load the finalized present
     * and absent report
     */
    public class LeaveType
    {
        public int Id { get; set; }
        public string LeaveTypeText { get; set; }
        public int LeaveDefaultCount { get; set; }
        public int LeaveMaxCount { get; set; }
        public bool IsActive { get; set; }
        public string IsActiveText { get; set; }
        public DateTime CreateDateTime { get; set; }
        public DateTime UpdateDateTime { get; set; }
        public string CreateDateTimeText { get; set; }
        public string UpdateDateTimeText { get; set; }
        public string actions { get; set; }
    }
}
