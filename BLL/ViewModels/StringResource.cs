using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLL.ViewModels
{
    class StringResource
    {
        public long Id { get; set; }
        public string ValueID { get; set; }
        public string ValueText { get; set; }
        public string SelectLanguage { get; set; }
        public bool Active_Status { get; set; }
    }
}
