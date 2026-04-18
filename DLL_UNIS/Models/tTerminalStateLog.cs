namespace DLL_UNIS.Models
{
    using System;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    /// <summary>Maps to UCDB dbo.event_logs (key: event_time + terminal_id; duplicates may occur in DB).</summary>
    [Table("event_logs")]
    public partial class tTerminalStateLog
    {
        [Key]
        [Column("event_time", Order = 0)]
        public DateTime D_EventTime { get; set; }

        [Key]
        [Column("terminal_id", Order = 1)]
        public int L_TID { get; set; }

        [Column("category")]
        public int? L_Class { get; set; }

        [Column("content")]
        public int? L_Detail { get; set; }

        [StringLength(50)]
        [Column("detail")]
        public string C_EventInfo { get; set; }

        [NotMapped]
        public string C_EventTime { get; set; }

        [NotMapped]
        public int? L_UID { get; set; }

        [NotMapped]
        public int? L_SensorNo { get; set; }

        [NotMapped]
        public int? L_TargetID { get; set; }

        [NotMapped]
        public int? L_Partition { get; set; }

        [NotMapped]
        public int? L_Account { get; set; }

        [NotMapped]
        public int? L_Qualifier { get; set; }
    }
}
