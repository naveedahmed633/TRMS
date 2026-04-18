namespace DLL_UNIS.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("wWorkSummary")]
    public partial class wWorkSummary
    {
        [Key]
        [Column(Order = 0)]
        [StringLength(8)]
        public string C_SumDate { get; set; }

        [Key]
        [Column(Order = 1)]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int L_UID { get; set; }

        [StringLength(30)]
        public string C_Name { get; set; }

        [StringLength(30)]
        public string C_Unique { get; set; }

        [StringLength(30)]
        public string C_OfficeCode { get; set; }

        [StringLength(30)]
        public string C_OfficeName { get; set; }

        [StringLength(30)]
        public string C_PostCode { get; set; }

        [StringLength(30)]
        public string C_PostName { get; set; }

        [StringLength(30)]
        public string C_StaffCode { get; set; }

        [StringLength(30)]
        public string C_StaffName { get; set; }

        [StringLength(4)]
        public string C_WorkCode { get; set; }

        [StringLength(30)]
        public string C_WorkName { get; set; }

        [StringLength(8)]
        public string C_Period1 { get; set; }

        [StringLength(8)]
        public string C_Period2 { get; set; }

        public int? L_Latetime { get; set; }

        public int? L_LackTime { get; set; }

        public int? L_ST1Late { get; set; }

        public int? L_ST1Lack { get; set; }

        public int? L_ST1Time { get; set; }

        public int? L_ST2Late { get; set; }

        public int? L_ST2Lack { get; set; }

        public int? L_ST2Time { get; set; }

        public int? L_ST3Late { get; set; }

        public int? L_ST3Lack { get; set; }

        public int? L_ST3Time { get; set; }

        public int? L_ST4Late { get; set; }

        public int? L_ST4Lack { get; set; }

        public int? L_ST4Time { get; set; }

        public int? L_ST5Late { get; set; }

        public int? L_ST5Lack { get; set; }

        public int? L_ST5Time { get; set; }

        public int? L_ST6Late { get; set; }

        public int? L_ST6Lack { get; set; }

        public int? L_ST6Time { get; set; }

        public double? D_PayMoney { get; set; }

        public int? L_Modify { get; set; }

        [StringLength(255)]
        public string C_Remark { get; set; }
    }
}
