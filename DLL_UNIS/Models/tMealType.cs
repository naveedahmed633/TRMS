namespace DLL_UNIS.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("tMealType")]
    public partial class tMealType
    {
        [Key]
        [StringLength(4)]
        public string C_Code { get; set; }

        [StringLength(30)]
        public string C_Name { get; set; }

        public int? L_DayLimit { get; set; }

        public int? L_MonthLimit { get; set; }

        [StringLength(19)]
        public string C_Period { get; set; }
    }
}
