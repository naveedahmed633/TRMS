namespace DLL_UNIS.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("tVisitor")]
    public partial class tVisitor
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int L_UID { get; set; }

        [StringLength(30)]
        public string C_Office { get; set; }

        [StringLength(30)]
        public string C_Post { get; set; }

        [StringLength(30)]
        public string C_Target { get; set; }

        [StringLength(255)]
        public string C_Goal { get; set; }

        [StringLength(30)]
        public string C_Company { get; set; }

        [StringLength(255)]
        public string C_Info { get; set; }

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
