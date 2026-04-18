using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DLL.Models
{
    public class GeoPhencingTerminal
    {
        public int id { get; set; }

        public int EmployeeId { get; set; }

        public string BranchesList { get; set; }

        public string TerminalsList { get; set; }

        public DateTime TrmCreateDate { get; set; }
    }
}
