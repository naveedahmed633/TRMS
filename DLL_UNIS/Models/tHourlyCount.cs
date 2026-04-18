namespace DLL_UNIS.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("tHourlyCount")]
    public partial class tHourlyCount
    {
        [Key]
        [Column(Order = 0)]
        [StringLength(8)]
        public string C_Date { get; set; }

        [Key]
        [Column(Order = 1)]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int L_Time { get; set; }

        [Key]
        [Column(Order = 2)]
        [StringLength(30)]
        public string C_Office { get; set; }

        [Key]
        [Column(Order = 3)]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int L_TID { get; set; }

        public int L_SensorNo { get; set; }

        [Key]
        [Column(Order = 4)]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int L_Zone { get; set; }

        public int? L_InCount { get; set; }

        public int? L_OutCount { get; set; }

        [StringLength(14)]
        public string C_CreateDate { get; set; }
    }
}
