namespace DLL_UNIS.Models
{
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    /// <summary>Maps to UCDB dbo.terminals (legacy property names where still used).</summary>
    [Table("terminals")]
    public partial class tTerminal
    {
        [Key]
        [Column("terminal_id")]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int L_ID { get; set; }

        [StringLength(50)]
        [Column("name")]
        public string C_Name { get; set; }

        [Column("status")]
        public int? L_Status { get; set; }

        [Column("ip_address")]
        public byte[] B_IpAddress { get; set; }

        [Column("mac_address")]
        public byte[] B_MacAddress { get; set; }

        [Column("type")]
        public int? L_Type { get; set; }

        [Column("func_type")]
        public int? L_FnWork { get; set; }

        [StringLength(255)]
        [Column("version")]
        public string C_Version { get; set; }

        [Column("remote_door")]
        public int? L_RemoteCtrl { get; set; }

        [Column("utc_index")]
        public int? L_tzIndex { get; set; }

        [Column("group_code")]
        public int L_GroupCode { get; set; }

        [StringLength(255)]
        [Column("description")]
        public string C_Remark { get; set; }

        [Column("auth_type")]
        public int? L_AuthType { get; set; }

        [StringLength(20)]
        [Column("external_id")]
        public string C_ExternalId { get; set; }

        [Column("register_flag")]
        public int? L_RegisterFlag { get; set; }

        [Column("core_flag")]
        public int? L_CoreFlag { get; set; }

        [Column("use_auth")]
        public byte? L_UseAuth { get; set; }

        [Column("healthid_flag")]
        public int? L_HealthidFlag { get; set; }

        [NotMapped]
        public int? L_FnMeal { get; set; }

        [NotMapped]
        public int? L_FnSchool { get; set; }

        [NotMapped]
        [StringLength(30)]
        public string C_Office { get; set; }

        [NotMapped]
        [StringLength(255)]
        public string C_Place { get; set; }

        [NotMapped]
        [StringLength(14)]
        public string C_RegDate { get; set; }

        [NotMapped]
        public int? L_CommType { get; set; }

        [NotMapped]
        [StringLength(255)]
        public string C_IPAddr { get; set; }

        [NotMapped]
        public int? L_IPPort { get; set; }

        [NotMapped]
        public int? L_ComPort { get; set; }

        [NotMapped]
        public int? L_Baudrate { get; set; }

        [NotMapped]
        public int? L_Passback { get; set; }

        [NotMapped]
        [StringLength(4)]
        public string C_AreaIn { get; set; }

        [NotMapped]
        [StringLength(4)]
        public string C_AreaOut { get; set; }

        [NotMapped]
        [StringLength(14)]
        public string C_lastup { get; set; }

        [NotMapped]
        public int? L_tzUseFlag { get; set; }

        [NotMapped]
        public int? L_tzBias { get; set; }

        [NotMapped]
        [StringLength(128)]
        public string C_tzKeyName { get; set; }

        [NotMapped]
        public int? L_InstallType { get; set; }

        [NotMapped]
        public int? L_VTerminalID { get; set; }

        [NotMapped]
        public int? L_DataCheck { get; set; }

        [NotMapped]
        public int? L_SoftPassback { get; set; }

        [NotMapped]
        [StringLength(12)]
        public string C_MacAddr { get; set; }

        [NotMapped]
        [StringLength(12)]
        public string C_PartitionStatus { get; set; }

        [NotMapped]
        [StringLength(24)]
        public string C_ZoneStatus { get; set; }

        [NotMapped]
        [StringLength(12)]
        public string C_LockStatus { get; set; }

        [NotMapped]
        [StringLength(24)]
        public string C_ReaderStatus { get; set; }

        [NotMapped]
        [StringLength(128)]
        public string C_ReaderVer1 { get; set; }

        [NotMapped]
        [StringLength(128)]
        public string C_ReaderVer2 { get; set; }

        [NotMapped]
        [StringLength(128)]
        public string C_ReaderVer3 { get; set; }

        [NotMapped]
        [StringLength(128)]
        public string C_ReaderVer4 { get; set; }

        [NotMapped]
        [StringLength(128)]
        public string C_ReaderVer5 { get; set; }

        [NotMapped]
        [StringLength(128)]
        public string C_ReaderVer6 { get; set; }

        [NotMapped]
        [StringLength(128)]
        public string C_ReaderVer7 { get; set; }

        [NotMapped]
        [StringLength(128)]
        public string C_ReaderVer8 { get; set; }

        [NotMapped]
        public int? L_DVRID { get; set; }

        [NotMapped]
        public int? L_Chnl1 { get; set; }

        [NotMapped]
        public int? L_Chnl2 { get; set; }

        [NotMapped]
        public int? L_NvrChannel1 { get; set; }

        [NotMapped]
        public int? L_NvrChannel2 { get; set; }

        [NotMapped]
        public int? L_NvrChannel3 { get; set; }

        [NotMapped]
        public int? L_NvrChannel4 { get; set; }
    }
}
