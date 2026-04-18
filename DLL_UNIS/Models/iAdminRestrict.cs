namespace DLL_UNIS.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("iAdminRestrict")]
    public partial class iAdminRestrict
    {
        public int? L_UID { get; set; }

        [Key]
        [StringLength(4)]
        public string C_AccessGroup { get; set; }
    }
}
