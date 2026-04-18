using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DLL.Models
{
    public class LogMessage
    {
        [Key]
        public long LogId { get; set; }

        public string LogTitle { get; set; }

        public DateTime LogDateTime { get; set; }

        public string LogMessageText { get; set; }
    }
}
