namespace DLL_UNIS.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("cStaff")]
    public partial class cStaff
    {
        [Key]
        [StringLength(30)]
        public string C_Code { get; set; }

        [StringLength(30)]
        public string C_Name { get; set; }
    }
}
