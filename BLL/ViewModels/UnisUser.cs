using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ViewModels
{
    public class UnisUser
    {
        [Key]
        public long id { get; set; }
        public string name { get; set; }
        public string empCode { get; set; }
        public string actions { get;set;}
    }

    public class UnisUserList
    {
       
        public long id { get; set; }
        public string name { get; set; }
        public string  type { get; set; }
      
    }
}
