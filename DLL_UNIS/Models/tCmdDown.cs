namespace DLL_UNIS.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("tCmdDown")]
    public partial class tCmdDown
    {
        [Key]
        [Column(Order = 0)]
        [StringLength(14)]
        public string C_RegTime { get; set; }

        [Key]
        [Column(Order = 1)]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int L_TID { get; set; }

        [Key]
        [Column(Order = 2)]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int L_UID { get; set; }

        [StringLength(14)]
        public string C_Time { get; set; }

        [Column(TypeName = "image")]
        public byte[] B_Data { get; set; }

        public int? L_Retry { get; set; }
    }
}
