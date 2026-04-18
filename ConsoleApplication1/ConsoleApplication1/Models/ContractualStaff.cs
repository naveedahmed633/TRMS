using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DLL.Models
{
    public class ContractualStaff
    {

        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]

        public int ContractualStaffId { get; set; }

        [StringLength(50)]
        [Index(IsUnique = true)]
        public string employee_code { get; set; }

        public string employee_name { get; set; }

        public string email { get; set; }

        public string address { get; set; }

        public string mobile_no { get; set; }

        public bool active { get; set; }

        public string department { get; set; }

        public string designation { get; set; }

        public string function { get; set; }

        public string grade { get; set; }

        public string Group { get; set; }

        public string location { get; set; }

        public string region { get; set; }

        public string company { get; set; }

        public DateTime? date_of_joining { get; set; }

        public DateTime? date_of_leaving { get; set; }

        public virtual CS_AttendanceLog persistent_log { get; set; }
    }
}
