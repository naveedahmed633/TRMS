using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ViewModels
{
    public class UserTracking
    {

        public int HaTransitId { get; set; }
        public string C_Name { get; set; }
        public string C_Unique { get; set; }
        public string C_Date { get; set; }
        public string C_Time { get; set; }
        public int L_UID { get; set; }
        public int? L_TID { get; set; }
        public bool active { get; set; }
        public string TerminalName { get; set; }

    }
}
