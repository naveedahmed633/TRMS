using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLL.ViewModels
{
    public class FutureManualAttedance
    {
        public int Id { get; set; }
        public DateTime from_date { get; set; }
        public DateTime to_date { get; set; }
        public string employee_code { get; set; }
        public string remarks { get; set; }

    }
}
