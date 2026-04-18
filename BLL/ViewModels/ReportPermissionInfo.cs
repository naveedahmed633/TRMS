using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ViewModels
{
    public class ReportPermissionInfo
    {
        [Key]
        public int Id { get; set; }
        public string employee_code { get; set; }
        public bool prep_01 { get; set; }
        public bool prep_02 { get; set; }
        public bool prep_03 { get; set; }
        public bool prep_04 { get; set; }
    }
}
