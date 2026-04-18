using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DLL.Models
{
     public class CS_PersistentLog
    {
         public int CS_PersistentLogId { get; set; }
         public DateTime? date { get; set; }
         public ContractualStaff employee { get; set; }
         public DateTime? time_in { get; set; }
         public DateTime? time_out { get; set; }
         public string terminal_in { get; set; }
         public string terminal_out { get; set; }
         public string remarks { get; set; }
         public bool active { get; set; }
    }
}
