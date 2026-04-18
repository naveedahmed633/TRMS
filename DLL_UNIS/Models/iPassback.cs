namespace DLL_UNIS.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("iPassback")]
    public partial class iPassback
    {
        public int? L_TID { get; set; }

        public int? L_RID { get; set; }

        [Key]
        [Column(Order = 0)]
        [StringLength(4)]
        public string C_AreaIn { get; set; }

        [Key]
        [Column(Order = 1)]
        [StringLength(4)]
        public string C_AreaOut { get; set; }

        public int? L_Type { get; set; }

        [StringLength(6)]
        public string C_LockoutTime { get; set; }

        [StringLength(255)]
        public string C_Remark { get; set; }
    }
}
