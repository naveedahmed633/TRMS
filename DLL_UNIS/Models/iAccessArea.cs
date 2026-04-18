namespace DLL_UNIS.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("iAccessArea")]
    public partial class iAccessArea
    {
        [Key]
        [Column(Order = 0)]
        [StringLength(4)]
        public string C_Code { get; set; }

        [Key]
        [Column(Order = 1)]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int L_TID { get; set; }
    }
}
