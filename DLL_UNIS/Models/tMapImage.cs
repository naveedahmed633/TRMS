namespace DLL_UNIS.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("tMapImage")]
    public partial class tMapImage
    {
        [Key]
        [StringLength(4)]
        public string C_Code { get; set; }

        [StringLength(30)]
        public string C_Name { get; set; }

        [StringLength(255)]
        public string C_FileName { get; set; }

        public int? L_FileSize { get; set; }

        [Column(TypeName = "image")]
        public byte[] B_FileData { get; set; }
    }
}
