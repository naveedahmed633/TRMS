namespace DLL_UNIS.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("tEmploye")]
    public partial class tEmploye
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int L_UID { get; set; }

        [StringLength(8)]
        public string C_IncludeDate { get; set; }

        [StringLength(8)]
        public string C_RetiredDate { get; set; }

        [StringLength(30)]
        public string C_Office { get; set; }

        [StringLength(30)]
        public string C_Post { get; set; }

        [StringLength(30)]
        public string C_Staff { get; set; }

        [StringLength(4)]
        public string C_Authority { get; set; }

        [StringLength(4)]
        public string C_Work { get; set; }

        [StringLength(4)]
        public string C_Money { get; set; }

        [StringLength(4)]
        public string C_Meal { get; set; }

        [StringLength(255)]
        public string C_Phone { get; set; }

        [StringLength(255)]
        public string C_Email { get; set; }

        [StringLength(255)]
        public string C_Address { get; set; }

        [StringLength(255)]
        public string C_Remark { get; set; }
    }
}
