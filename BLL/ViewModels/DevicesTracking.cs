using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ViewModels
{
    public class DevicesTracking
    {
        [Key]
        public int id { get; set; }
        public string Name { get; set; }
        public string C_Unique { get; set; }
        public string DeviceName { get; set; }
        public int? L_TID { get; set; }
        public string Status { get; set; }
        public int branchCode { get; set; }
        public string branchName { get; set; }
        public string date { get; set; }
    }
    public class DevicesTracking2
    {

        public int id { get; set; }
        public string Name { get; set; }
        public string C_Unique { get; set; }
        public string DeviceName { get; set; }
        public int L_ID { get; set; }
        public string Status { get; set; }

        public string date { get; set; }
    }
}
