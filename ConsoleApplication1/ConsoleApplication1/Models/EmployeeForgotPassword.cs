namespace DLL.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class EmployeeForgotPassword
    {
        public int Id { get; set; }

        public string EmployeeCode { get; set; }

        public string GuidCode { get; set; }

        public DateTime ExpiryDate { get; set; }

        public DateTime CreateDate { get; set; }

        public DateTime? UpdateDate { get; set; }
    }
}
