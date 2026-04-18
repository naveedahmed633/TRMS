namespace DLL_UNIS.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("cOffice")]
    public partial class cOffice
    {
        [Key]
        [StringLength(30)]
        public string c_code { get; set; }

        [StringLength(30)]
        public string c_name { get; set; }
    }
}
