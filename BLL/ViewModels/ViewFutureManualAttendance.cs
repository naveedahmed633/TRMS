using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLL.ViewModels
{
     public class ViewFutureManualAttendance
    {
        public int Id { get; set; }
        public string from_date { get; set; }
        public string to_date { get; set; }
        public string employee_code { get; set; }
        public string remarks { get; set; }

        public string actions { get; set; }
    }
}
