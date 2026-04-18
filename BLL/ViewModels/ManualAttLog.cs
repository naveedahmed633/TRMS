using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ViewModels
{
    public class ManualAttLog
    {
        [Key]
        public int id { get; set; }

        public string WhoMark { get; set; }

        public string WhoseMark { get; set; }

        public string FromDate { get; set; }

        public string ToDate { get; set; }

        public string Remarks { get; set; }
    }
}
