namespace DLL_UNIS.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("iACUInfo")]
    public partial class iACUInfo
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int L_TID { get; set; }

        [StringLength(12)]
        public string C_PartitionStatus { get; set; }

        [StringLength(24)]
        public string C_ZoneStatus { get; set; }

        [StringLength(12)]
        public string C_LockStatus { get; set; }

        [StringLength(24)]
        public string C_ReaderStatus { get; set; }

        [StringLength(128)]
        public string C_ReaderVer1 { get; set; }

        [StringLength(128)]
        public string C_ReaderVer2 { get; set; }

        [StringLength(128)]
        public string C_ReaderVer3 { get; set; }

        [StringLength(128)]
        public string C_ReaderVer4 { get; set; }

        [StringLength(128)]
        public string C_ReaderVer5 { get; set; }

        [StringLength(128)]
        public string C_ReaderVer6 { get; set; }

        [StringLength(128)]
        public string C_ReaderVer7 { get; set; }

        [StringLength(128)]
        public string C_ReaderVer8 { get; set; }

        [StringLength(128)]
        public string C_ReaderName0 { get; set; }

        [StringLength(128)]
        public string C_ReaderName1 { get; set; }

        [StringLength(128)]
        public string C_ReaderName2 { get; set; }

        [StringLength(128)]
        public string C_ReaderName3 { get; set; }

        [StringLength(128)]
        public string C_ReaderName4 { get; set; }

        [StringLength(128)]
        public string C_ReaderName5 { get; set; }

        [StringLength(128)]
        public string C_ReaderName6 { get; set; }

        [StringLength(128)]
        public string C_ReaderName7 { get; set; }

        [StringLength(128)]
        public string C_WiegandName1 { get; set; }

        [StringLength(128)]
        public string C_WiegandName2 { get; set; }

        [StringLength(128)]
        public string C_WiegandName3 { get; set; }

        [StringLength(128)]
        public string C_WiegandName4 { get; set; }
    }
}
