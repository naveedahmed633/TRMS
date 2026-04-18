namespace DLL_UNIS.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("tMoney")]
    public partial class tMoney
    {
        [Key]
        [StringLength(4)]
        public string C_Code { get; set; }

        [StringLength(30)]
        public string C_Name { get; set; }

        public int? L_PayUnit { get; set; }

        public double? D_WT1Money { get; set; }

        public double? D_WT2Money { get; set; }

        public double? D_WT3Money { get; set; }

        public double? D_WT4Money { get; set; }

        public double? D_WT5Money { get; set; }

        public double? D_WT6Money { get; set; }
    }
}
