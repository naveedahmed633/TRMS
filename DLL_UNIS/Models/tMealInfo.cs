namespace DLL_UNIS.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("tMealInfo")]
    public partial class tMealInfo
    {
        [Key]
        [Column(Order = 0)]
        [StringLength(4)]
        public string C_Code1 { get; set; }

        [Key]
        [Column(Order = 1)]
        [StringLength(4)]
        public string C_Code2 { get; set; }
    }
}
