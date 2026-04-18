namespace DLL.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public class PermissionSuperAdmin
    {
        public int Id { get; set; }
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
    }
}
