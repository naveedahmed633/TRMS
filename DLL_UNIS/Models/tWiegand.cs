namespace DLL_UNIS.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("tWiegand")]
    public partial class tWiegand
    {
        [Key]
        [StringLength(4)]
        public string C_Code { get; set; }

        [StringLength(30)]
        public string C_Name { get; set; }

        public int? L_Type { get; set; }

        [Column(TypeName = "image")]
        public byte[] B_FormatData { get; set; }
    }
}
