namespace DLL_UNIS.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("cTimezone")]
    public partial class cTimezone
    {
        [Key]
        [StringLength(4)]
        public string C_Code { get; set; }

        [Required]
        [StringLength(30)]
        public string C_Name { get; set; }

        public int? L_Flag { get; set; }

        [StringLength(255)]
        public string C_Remark { get; set; }

        public int? L_AuthType { get; set; }

        public int? L_AuthValue { get; set; }
    }
}
