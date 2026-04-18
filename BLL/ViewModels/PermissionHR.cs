using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ViewModels
{
    public class PermissionHR
    {
        public string EmployeeCode { get; set; }
        public bool ExportGridData { get; set; }
        public bool DownloadExcel { get; set; }
        public bool ExportPDF { get; set; }
        public bool EmployeeManagement { get; set; }
        public bool HR { get; set; }
        public bool Leaves { get; set; }
        public bool PayrollLoan { get; set; }
        public bool Overtime { get; set; }
        public bool ShiftManagement { get; set; }
        public bool RoasterManagement { get; set; }
        public bool AttendanceManagement { get; set; }
        public bool Report { get; set; }
        public bool Manual { get; set; }

        public int accessGroupId { get; set; }
    }
}
