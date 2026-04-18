namespace DLL_UNIS.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("wWorkShift")]
    public partial class wWorkShift
    {
        [Key]
        [StringLength(2)]
        public string C_Code { get; set; }

        [StringLength(30)]
        public string C_Name { get; set; }

        public int? L_InoutMode { get; set; }

        public int? L_RangeTime1 { get; set; }

        public int? L_RangeTime2 { get; set; }

        public int? L_IgnoreAbsent { get; set; }

        public int? L_MultiRange { get; set; }

        public int? L_LateTime { get; set; }

        public int? L_LackTime { get; set; }

        public int? L_AutoInTime { get; set; }

        public int? L_AutoOutTime { get; set; }

        public int? L_Range1TM1 { get; set; }

        public int? L_Range1TM2 { get; set; }

        public int? L_Range2TM1 { get; set; }

        public int? L_Range2TM2 { get; set; }

        public int? L_Range3TM1 { get; set; }

        public int? L_Range3TM2 { get; set; }

        public int? L_Range4TM1 { get; set; }

        public int? L_Range4TM2 { get; set; }

        public int? L_ExceptExit { get; set; }

        public int? L_ExceptRtnMode { get; set; }

        public int? L_ExceptOut { get; set; }

        public int? L_ExceptInMode { get; set; }

        public int? L_Except1TM1 { get; set; }

        public int? L_Except1TM2 { get; set; }

        public int? L_Except2TM1 { get; set; }

        public int? L_Except2TM2 { get; set; }

        public int? L_Except3TM1 { get; set; }

        public int? L_Except3TM2 { get; set; }

        public int? L_Except4TM1 { get; set; }

        public int? L_Except4TM2 { get; set; }

        public int? L_Except5TM1 { get; set; }

        public int? L_Except5TM2 { get; set; }

        public int? L_SF1Work { get; set; }

        public int? L_SF1Type { get; set; }

        public int? L_SF1Time1 { get; set; }

        public int? L_SF1Time2 { get; set; }

        public int? L_SF1Range { get; set; }

        public int? L_SF1AutoOut { get; set; }

        public int? L_SF1Unit { get; set; }

        public int? L_SF1Min { get; set; }

        public int? L_SF1Max { get; set; }

        public int? L_SF1Rate { get; set; }

        public int? L_SF2Work { get; set; }

        public int? L_SF2Type { get; set; }

        public int? L_SF2Time1 { get; set; }

        public int? L_SF2Time2 { get; set; }

        public int? L_SF2Range { get; set; }

        public int? L_SF2AutoOut { get; set; }

        public int? L_SF2Unit { get; set; }

        public int? L_SF2Min { get; set; }

        public int? L_SF2Max { get; set; }

        public int? L_SF2Rate { get; set; }

        public int? L_SF3Work { get; set; }

        public int? L_SF3Type { get; set; }

        public int? L_SF3Time1 { get; set; }

        public int? L_SF3Time2 { get; set; }

        public int? L_SF3Range { get; set; }

        public int? L_SF3AutoOut { get; set; }

        public int? L_SF3Unit { get; set; }

        public int? L_SF3Min { get; set; }

        public int? L_SF3Max { get; set; }

        public int? L_SF3Rate { get; set; }

        public int? L_SF4Work { get; set; }

        public int? L_SF4Type { get; set; }

        public int? L_SF4Time1 { get; set; }

        public int? L_SF4Time2 { get; set; }

        public int? L_SF4Range { get; set; }

        public int? L_SF4AutoOut { get; set; }

        public int? L_SF4Unit { get; set; }

        public int? L_SF4Min { get; set; }

        public int? L_SF4Max { get; set; }

        public int? L_SF4Rate { get; set; }

        public int? L_SF5Work { get; set; }

        public int? L_SF5Type { get; set; }

        public int? L_SF5Time1 { get; set; }

        public int? L_SF5Time2 { get; set; }

        public int? L_SF5Range { get; set; }

        public int? L_SF5AutoOut { get; set; }

        public int? L_SF5Unit { get; set; }

        public int? L_SF5Min { get; set; }

        public int? L_SF5Max { get; set; }

        public int? L_SF5Rate { get; set; }
    }
}
