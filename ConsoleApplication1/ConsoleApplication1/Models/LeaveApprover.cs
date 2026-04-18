namespace DLL.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public class LeaveApprover
    {
        public int Id { get; set; }

        public int EmployeeId { get; set; }

        public string EmployeeCode { get; set; }

        public string FirstName { get; set; }

        public string LasName { get; set; }
    }
}
