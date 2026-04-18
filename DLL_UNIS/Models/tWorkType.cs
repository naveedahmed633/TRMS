namespace DLL_UNIS.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("tWorkType")]
    public partial class tWorkType
    {
        [Key]
        [StringLength(4)]
        public string C_Code { get; set; }

        [StringLength(30)]
        public string C_Name { get; set; }

        [StringLength(8)]
        public string C_BasicDay { get; set; }

        [StringLength(4)]
        public string C_HoliCode { get; set; }

        public int? L_SpinCount { get; set; }

        [StringLength(60)]
        public string C_ShiftCode { get; set; }

        [StringLength(2)]
        public string C_HoliShift { get; set; }

        public int? L_WT1Unit { get; set; }

        public int? L_WT1AddTime { get; set; }

        public int? L_WT1AddCondi { get; set; }

        public int? L_WT1DelTime { get; set; }

        public int? L_WT1DelCondi { get; set; }

        public int? L_WT1Min { get; set; }

        public int? L_WT1Max { get; set; }

        public int? L_WT1Rate { get; set; }

        public int? L_WT2Unit { get; set; }

        public int? L_WT2AddTime { get; set; }

        public int? L_WT2AddCondi { get; set; }

        public int? L_WT2DelTime { get; set; }

        public int? L_WT2DelCondi { get; set; }

        public int? L_WT2Min { get; set; }

        public int? L_WT2Max { get; set; }

        public int? L_WT2Rate { get; set; }

        public int? L_WT3Unit { get; set; }

        public int? L_WT3AddTime { get; set; }

        public int? L_WT3AddCondi { get; set; }

        public int? L_WT3DelTime { get; set; }

        public int? L_WT3DelCondi { get; set; }

        public int? L_WT3Min { get; set; }

        public int? L_WT3Max { get; set; }

        public int? L_WT3Rate { get; set; }

        public int? L_WT4Unit { get; set; }

        public int? L_WT4AddTime { get; set; }

        public int? L_WT4AddCondi { get; set; }

        public int? L_WT4DelTime { get; set; }

        public int? L_WT4DelCondi { get; set; }

        public int? L_WT4Min { get; set; }

        public int? L_WT4Max { get; set; }

        public int? L_WT4Rate { get; set; }

        public int? L_WT5Unit { get; set; }

        public int? L_WT5AddTime { get; set; }

        public int? L_WT5AddCondi { get; set; }

        public int? L_WT5DelTime { get; set; }

        public int? L_WT5DelCondi { get; set; }

        public int? L_WT5Min { get; set; }

        public int? L_WT5Max { get; set; }

        public int? L_WT5Rate { get; set; }

        public int? L_WT6Unit { get; set; }

        public int? L_WT6AddTime { get; set; }

        public int? L_WT6AddCondi { get; set; }

        public int? L_WT6DelTime { get; set; }

        public int? L_WT6DelCondi { get; set; }

        public int? L_WT6Min { get; set; }

        public int? L_WT6Max { get; set; }

        public int? L_WT6Rate { get; set; }

        public int? L_ST1AddTime { get; set; }

        public int? L_ST1AddCondi { get; set; }

        public int? L_ST1DelTime { get; set; }

        public int? L_ST1DelCondi { get; set; }

        public int? L_ST1Min { get; set; }

        public int? L_ST1Max { get; set; }

        public int? L_ST1Trans { get; set; }

        public int? L_ST2AddTime { get; set; }

        public int? L_ST2AddCondi { get; set; }

        public int? L_ST2DelTime { get; set; }

        public int? L_ST2DelCondi { get; set; }

        public int? L_ST2Min { get; set; }

        public int? L_ST2Max { get; set; }

        public int? L_ST2Trans { get; set; }

        public int? L_ST3AddTime { get; set; }

        public int? L_ST3AddCondi { get; set; }

        public int? L_ST3DelTime { get; set; }

        public int? L_ST3DelCondi { get; set; }

        public int? L_ST3Min { get; set; }

        public int? L_ST3Max { get; set; }

        public int? L_ST3Trans { get; set; }

        public int? L_ST4AddTime { get; set; }

        public int? L_ST4AddCondi { get; set; }

        public int? L_ST4DelTime { get; set; }

        public int? L_ST4DelCondi { get; set; }

        public int? L_ST4Min { get; set; }

        public int? L_ST4Max { get; set; }

        public int? L_ST4Trans { get; set; }

        public int? L_ST5AddTime { get; set; }

        public int? L_ST5AddCondi { get; set; }

        public int? L_ST5DelTime { get; set; }

        public int? L_ST5DelCondi { get; set; }

        public int? L_ST5Min { get; set; }

        public int? L_ST5Max { get; set; }

        public int? L_ST5Trans { get; set; }

        public int? L_ST6AddTime { get; set; }

        public int? L_ST6AddCondi { get; set; }

        public int? L_ST6DelTime { get; set; }

        public int? L_ST6DelCondi { get; set; }

        public int? L_ST6Min { get; set; }

        public int? L_ST6Max { get; set; }

        public int? L_ST6Trans { get; set; }
    }
}
