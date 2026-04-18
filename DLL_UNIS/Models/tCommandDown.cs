namespace DLL_UNIS.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("tCommandDown")]
    public partial class tCommandDown
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
        public int L_TID { get; set; }

        [Key]
        [Column(Order = 3)]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int L_Index { get; set; }

        public int? L_UID { get; set; }

        public int? L_CMD { get; set; }

        public int? L_DataType { get; set; }

        public int? L_DataLen { get; set; }

        [Column(TypeName = "image")]
        public byte[] B_Data { get; set; }

        [Key]
        [Column(Order = 4)]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int L_RetryCount { get; set; }
    }
}
