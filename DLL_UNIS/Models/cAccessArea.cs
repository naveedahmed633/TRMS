namespace DLL_UNIS.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("cAccessArea")]
    public partial class cAccessArea
    {
        [Key]
        [StringLength(4)]
        public string C_Code { get; set; }

        [StringLength(30)]
        public string C_Name { get; set; }

        public int? L_Flag { get; set; }

        [StringLength(255)]
        public string C_Remark { get; set; }

        [StringLength(4)]
        public string C_AccessTime { get; set; }

        [StringLength(4)]
        public string C_AccessTime2 { get; set; }

        [StringLength(4)]
        public string C_AccessTime3 { get; set; }

        [StringLength(4)]
        public string C_AccessTime4 { get; set; }
    }
}
