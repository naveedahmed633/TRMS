namespace DLL_UNIS.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("wExceptRecord")]
    public partial class wExceptRecord
    {
        public int? L_UID { get; set; }

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

        [Key]
        [StringLength(8)]
        public string C_WorkDate { get; set; }

        [StringLength(2)]
        public string C_ShiftCode { get; set; }

        [StringLength(30)]
        public string C_ShiftName { get; set; }

        public int? L_ExceptType { get; set; }

        public int? L_ExceptTM1 { get; set; }

        public int? L_ExceptTM2 { get; set; }

        public int? L_ExceptTime { get; set; }
    }
}
