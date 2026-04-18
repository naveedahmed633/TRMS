using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLL.ViewModels
{
    public class Terminals
    {
        [Key]
        public int id { get; set; }

        public int l_id { get; set; }
        public string name { get; set; }
        public string terminal_id { get; set; }
        
        public string branch_code { get; set; }
        public string branch_name { get; set; }

        public string action { get; set; }
        public string type { get; set; }
    }
}
