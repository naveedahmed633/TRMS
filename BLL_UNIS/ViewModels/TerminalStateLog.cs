using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLL_UNIS.ViewModels
{
    public class TerminalStateLog
    {
        public decimal C_EventTime { get; set; }

        public int? L_TID { get; set; }

        public int? L_UID { get; set; }

        public int? L_Class { get; set; }

        public int? L_Detail { get; set; }
    }
}
