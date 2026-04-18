namespace DLL_UNIS.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("tConnectServer")]
    public partial class tConnectServer
    {
        [Key]
        [StringLength(30)]
        public string C_SystemAddr { get; set; }

        [StringLength(128)]
        public string C_SystemName { get; set; }
    }
}
