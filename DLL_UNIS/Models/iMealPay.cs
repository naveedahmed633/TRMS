namespace DLL_UNIS.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("iMealPay")]
    public partial class iMealPay
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int L_MealType { get; set; }

        public int? L_Menu1 { get; set; }

        public int? L_Menu2 { get; set; }

        public int? L_Menu3 { get; set; }

        public int? L_Menu4 { get; set; }
    }
}
