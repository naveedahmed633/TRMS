using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DLL.Models
{
     public class CS_AttendanceLog
    {
        // one to one relation, every employee
        // has one and only one persistant attendance log
        [Key, ForeignKey("ContractualStaff")]
        public int CS_AttendanceLogId { get; set; }

        [MaxLength(50)]
        public string employee_code { get; set; }

        public virtual ContractualStaff ContractualStaff { get; set; }

        public DateTime? date { get; set; }

        public DateTime? time_in { get; set; }

        public DateTime? time_out { get; set; }

        public string terminal_in { get; set; }
        
        public string terminal_out { get; set; }
        
        public bool dirtyBit { get; set; }
        
        public bool active { get; set; }
    }

}
