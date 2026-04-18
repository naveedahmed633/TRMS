using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ViewModels
{
    public class CreateGroup
    {
        public string group_id { get; set; }
        public string group_name { get; set; }
        public string line_manager { get; set; }

        public string follows_general_calendar { get; set; }
        
        public List<String> group_employees { get; set; }

        public List<String> group_shifts { get; set; }
    }

    public class CreateSLMGroup
    {
        public string super_line_manager { get; set; }

        public List<string> group_employees { get; set; }
    }
}
