using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLL.ViewModels
{
    public class ServicesViewModel
    {
        public string serviceName { get; set; }
        public string Description { get; set; }
        public string status { get; set; }
        public string on_action { get; set; }
        public string off_action { get; set; }
    }
}
