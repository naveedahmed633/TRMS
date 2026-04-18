namespace DLL.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class PermissionReport
    {
        public int Id { get; set; }
        public string employee_code { get; set; }
        public bool prep_01 { get; set; }
        public bool prep_02 { get; set; }
        public bool prep_03 { get; set; }
        public bool prep_04 { get; set; }

    }
}
