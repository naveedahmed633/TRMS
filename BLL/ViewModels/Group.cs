using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ViewModels
{
    public class Group
    {
        public int id { get; set; }
        public Employee supervisor { get; set; }
        public string group_name { get; set; }
        public string group_description { get; set; }
    }
}
