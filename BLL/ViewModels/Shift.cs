using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ViewModels
{
    public class Shift
    {


        public int id { get; set; }

        public string name { get; set; }

        public string early_time { get; set; }
        public string start_time { get; set; }
        public string late_time { get; set; }
        public string half_day { get; set; }
        public string shift_end { get; set; }
        public string day_end { get; set; }
        //public string shift_hours { get; set; }
        
    }
}
