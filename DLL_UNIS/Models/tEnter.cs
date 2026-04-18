namespace DLL_UNIS.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("tEnter")]
    public partial class tEnter
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

        [StringLength(30)]
        public string C_Name { get; set; }

        [StringLength(20)]
        public string C_Unique { get; set; }

        [StringLength(30)]
        public string C_Office { get; set; }

        [StringLength(30)]
        public string C_Post { get; set; }

        [StringLength(24)]
        public string C_Card { get; set; }

        public int? L_UserType { get; set; }

        public int? L_Mode { get; set; }

        public int? L_MatchingType { get; set; }

        public int? L_Result { get; set; }

        public int? L_IsPicture { get; set; }

        public int? L_Device { get; set; }

        public int? L_OverCount { get; set; }

        [StringLength(8)]
        public string C_Property { get; set; }

        public int? L_JobCode { get; set; }

        public int? L_Etc { get; set; }

        public int? L_Trans { get; set; }

        public double? D_Latitude { get; set; }

        public double? D_Longitude { get; set; }

        [StringLength(15)]
        public string C_MobilePhone { get; set; }

        public int? L_NvrChannel1 { get; set; }

        public int? L_NvrChannel2 { get; set; }

        public int? L_NvrChannel3 { get; set; }

        public int? L_NvrChannel4 { get; set; }
    }
}
