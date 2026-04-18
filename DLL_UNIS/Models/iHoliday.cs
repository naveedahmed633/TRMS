namespace DLL_UNIS.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("iHoliday")]
    public partial class iHoliday
    {
        [Key]
        [StringLength(4)]
        public string C_Code { get; set; }

        [StringLength(4)]
        public string C_Holiday { get; set; }

        [StringLength(30)]
        public string C_DayName { get; set; }
    }
}
