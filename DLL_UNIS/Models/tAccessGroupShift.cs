namespace DLL_UNIS.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("tAccessGroupShift")]
    public partial class tAccessGroupShift
    {
        [Key]
        [Column(Order = 0)]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int L_AdminID { get; set; }

        [Key]
        [Column(Order = 1)]
        [StringLength(8)]
        public string C_Date { get; set; }

        [Key]
        [Column(Order = 2)]
        [StringLength(6)]
        public string C_Time { get; set; }

        [Key]
        [Column(Order = 3)]
        [StringLength(4)]
        public string C_Before { get; set; }

        [Key]
        [Column(Order = 4)]
        [StringLength(4)]
        public string C_After { get; set; }

        public int? L_Result { get; set; }
    }
}
