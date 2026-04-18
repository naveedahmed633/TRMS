namespace DLL_UNIS.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("wSpecialShift")]
    public partial class wSpecialShift
    {
        [Key]
        [Column(Order = 0)]
        [StringLength(8)]
        public string C_WorkDate { get; set; }

        [Key]
        [Column(Order = 1)]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int L_UID { get; set; }

        [StringLength(2)]
        public string C_ShiftCode { get; set; }
    }
}
