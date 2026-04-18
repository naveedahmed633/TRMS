namespace DLL_UNIS.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("tMealEnter")]
    public partial class tMealEnter
    {
        [Key]
        [Column(Order = 0)]
        [StringLength(8)]
        public string C_Date { get; set; }

        [Key]
        [Column(Order = 1)]
        [StringLength(6)]
        public string C_Time { get; set; }

        [Key]
        [Column(Order = 2)]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int L_TID { get; set; }

        [Key]
        [Column(Order = 3)]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int L_UID { get; set; }

        public int? L_MealType { get; set; }

        [StringLength(2)]
        public string C_Menu { get; set; }

        public int? L_MealPay { get; set; }

        [StringLength(2)]
        public string C_Reason { get; set; }

        [StringLength(1)]
        public string C_Upmode { get; set; }

        public int? L_MealCount { get; set; }

        [StringLength(8)]
        public string C_StatsDate { get; set; }
    }
}
