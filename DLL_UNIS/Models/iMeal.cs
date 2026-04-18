namespace DLL_UNIS.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("iMeal")]
    public partial class iMeal
    {
        [Key]
        [StringLength(4)]
        public string C_Code { get; set; }

        [Required]
        [StringLength(30)]
        public string C_Name { get; set; }

        public int L_MealType { get; set; }

        public int? L_MealLimit { get; set; }

        [StringLength(5)]
        public string C_MealTime1 { get; set; }

        [StringLength(5)]
        public string C_MealTime2 { get; set; }

        [StringLength(8)]
        public string C_StatsDate { get; set; }
    }
}
