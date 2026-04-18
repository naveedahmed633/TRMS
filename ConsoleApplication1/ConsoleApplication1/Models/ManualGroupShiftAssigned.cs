using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DLL.Models
{
    public class ManualGroupShiftAssigned
    {
        public int ManualGroupShiftAssignedId { get; set; }

        public DateTime? date { get; set; }

        public virtual Employee Employee { get; set; }

        public virtual Group Group { get; set; }

        public virtual Shift Shift { get; set; }

        public string reason { get; set; }

        public bool active { get; set; }

    }
}
