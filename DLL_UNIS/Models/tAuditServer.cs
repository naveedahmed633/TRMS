namespace DLL_UNIS.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("tAuditServer")]
    public partial class tAuditServer
    {
        [Key]
        [Column(Order = 0)]
        [StringLength(14)]
        public string C_EventTime { get; set; }

        [Key]
        [Column(Order = 1)]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int L_LogonID { get; set; }

        public int? L_Section { get; set; }

        [StringLength(30)]
        public string C_Target { get; set; }

        public int? L_Process { get; set; }

        public int? L_Detail { get; set; }

        [StringLength(128)]
        public string C_Remark { get; set; }
    }
}
