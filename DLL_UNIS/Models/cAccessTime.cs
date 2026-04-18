namespace DLL_UNIS.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("cAccessTime")]
    public partial class cAccessTime
    {
        [Key]
        [StringLength(4)]
        public string C_Code { get; set; }

        [StringLength(30)]
        public string C_Name { get; set; }

        public int? L_Flag { get; set; }

        [StringLength(4)]
        public string C_Holiday { get; set; }

        [StringLength(4)]
        public string C_Sun { get; set; }

        [StringLength(4)]
        public string C_Mon { get; set; }

        [StringLength(4)]
        public string C_The { get; set; }

        [StringLength(4)]
        public string C_Wed { get; set; }

        [StringLength(4)]
        public string C_Thu { get; set; }

        [StringLength(4)]
        public string C_Fri { get; set; }

        [StringLength(4)]
        public string C_Sat { get; set; }

        [StringLength(4)]
        public string C_Hol { get; set; }

        [StringLength(255)]
        public string C_Remark { get; set; }
    }
}
