using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ViewModels
{
    public class ReportAuditLog
    {
        [Key]
        public int AuditTrailId { get; set; }
        public DLL.Commons.actionCode action_code { get; set; }
        public string description { get; set; }
        public string table { get; set; }
        public string user_code { get; set; }
        public DateTime create_date_adt { get; set; }

        public string str_action_code { get; set; }
        public string str_create_date_adt { get; set; }

    }
}
