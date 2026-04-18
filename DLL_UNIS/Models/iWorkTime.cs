namespace DLL_UNIS.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("iWorkTime")]
    public partial class iWorkTime
    {
        [Key]
        [StringLength(30)]
        public string C_Code { get; set; }

        [StringLength(8)]
        public string C_Mon { get; set; }

        [StringLength(8)]
        public string C_Tue { get; set; }

        [StringLength(8)]
        public string C_Wed { get; set; }

        [StringLength(8)]
        public string C_Thu { get; set; }

        [StringLength(8)]
        public string C_Fri { get; set; }

        [StringLength(8)]
        public string C_Sat { get; set; }

        [StringLength(8)]
        public string C_Sun { get; set; }
    }
}
