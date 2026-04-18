using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ViewModels
{
    public class PermissionUser
    {
        public string EmployeeCode { get; set; }
        public bool ExportGridData { get; set; }
        public bool DownloadExcel { get; set; }
        public bool ExportPDF { get; set; }
        public bool LeavesMenu { get; set; }
        public bool OvertimeMenu { get; set; }
        public bool AttendanceMenu { get; set; }
        public bool LoanMenu { get; set; }
        public bool PayrollMenu { get; set; }
        public bool ReportsMenu { get; set; }
        public bool ManualMenu { get; set; }

        public int accessGroupId { get; set; }


        public string HREmployeeCode { get; set; }
        public bool HRExportGridData { get; set; }
        public bool HRDownloadExcel { get; set; }
        public bool HRExportPDF { get; set; }
        public bool HREmployeeManagement { get; set; }
        public bool HRHR { get; set; }
        public bool HRLeaves { get; set; }
        public bool HRPayrollLoan { get; set; }
        public bool HROvertime { get; set; }
        public bool HRShiftManagement { get; set; }
        public bool HRRoasterManagement { get; set; }
        public bool HRAttendanceManagement { get; set; }
        public bool HRReport { get; set; }
        public bool HRManual { get; set; }



        public bool LMExportGridData { get; set; }
        public bool LMDownloadExcel { get; set; }
        public bool LMExportPDF { get; set; }
        public bool LMLeaves { get; set; }
        public bool LMOvertime { get; set; }
        public bool LMRoaster { get; set; }
        public bool LMAttendance { get; set; }
        public bool LMReport { get; set; }
        public bool LMManual { get; set; }

        public int HRaccessGroupId { get; set; }
    }
}
