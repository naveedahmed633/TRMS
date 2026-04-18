using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DLL.Models
{
    public class Terminals
    {
        [Key]
        public int Id { get; set; }
        public int L_ID { get; set; }
        public string C_Name { get; set; }
        public string terminal_id { get; set; }

        public string branch_code { get; set; }
        public string branch_name { get; set; }

        public string type { get; set; }
    }
}
