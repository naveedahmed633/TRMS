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
    public class LeaveSessionInfo
    {
        [Key]
        public int Id { get; set; }
      
        public int EmployeeId { get; set; }

        public int YearId { get; set; }

        public DateTime SessionStartDate { get; set; }

        public DateTime SessionEndDate { get; set; }
    }
}
