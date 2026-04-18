namespace DLL_UNIS.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("tAccessShiftSchedule")]
    public partial class tAccessShiftSchedule
    {
        [Key]
        [StringLength(4)]
        public string C_AccessGroup { get; set; }

        [StringLength(8)]
        public string C_BasicDate { get; set; }

        public int? L_SpinDays { get; set; }

        [StringLength(120)]
        public string C_ShiftCode { get; set; }
    }
}
