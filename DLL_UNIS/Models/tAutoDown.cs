namespace DLL_UNIS.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("tAutoDown")]
    public partial class tAutoDown
    {
        [Key]
        [Column(Order = 0)]
        [StringLength(14)]
        public string C_RegTime { get; set; }

        [Key]
        [Column(Order = 1)]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int L_CID { get; set; }

        [Key]
        [Column(Order = 2)]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int L_Index { get; set; }

        public int? L_Target { get; set; }

        public int? L_Process { get; set; }

        public int? L_TID { get; set; }

        public int? L_UID { get; set; }

        [StringLength(4)]
        public string C_AccessGroup { get; set; }

        [StringLength(30)]
        public string C_OfficeCode { get; set; }

        [Key]
        [Column(Order = 3)]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int L_RetryCount { get; set; }

        public int? L_DataCheck { get; set; }
    }
}
