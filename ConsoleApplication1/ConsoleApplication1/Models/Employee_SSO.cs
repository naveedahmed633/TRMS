namespace DLL.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public class Employee_SSO
    {
        public int Id { get; set; }

        [StringLength(50)]
        [Index(IsUnique = true)]
        public string ComputerName { get; set; }

        [StringLength(50)]
        [Index(IsUnique = true)]
        public string Employee_Code { get; set; }

        public string EPassword { get; set; }
    }
}
