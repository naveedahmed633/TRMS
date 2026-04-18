namespace DLL.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public class PermissionUser
    {
        public int Id { get; set; }
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
    }
}
