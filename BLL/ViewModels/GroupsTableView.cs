using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ViewModels
{
    public class GroupsTableView
    {
        public int id { get; set; }
        public string group_name { get; set; }
        public string line_manager_code { get; set; }
        public string line_manager_name { get; set; }
        public string action { get; set; }
    }
}
