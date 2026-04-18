namespace DLL_UNIS.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("tChangedInfo")]
    public partial class tChangedInfo
    {
        [Key]
        [Column(Order = 0)]
        [StringLength(14)]
        public string C_CreateTime { get; set; }

        [Key]
        [Column(Order = 1)]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int L_Target { get; set; }

        [Key]
        [Column(Order = 2)]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int L_Procedure { get; set; }

        [Key]
        [Column(Order = 3)]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int L_TargetID { get; set; }

        [StringLength(30)]
        public string C_TargetCode { get; set; }

        public int? L_ClientID { get; set; }
    }
}
