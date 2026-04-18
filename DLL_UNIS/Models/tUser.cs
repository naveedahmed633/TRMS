namespace DLL_UNIS.Models
{
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    /// <summary>Maps to UCDB dbo.users (legacy property names preserved for app code).</summary>
    [Table("users")]
    public partial class tUser
    {
        [Key]
        [Column("user_id")]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public long L_ID { get; set; }

        [StringLength(200)]
        [Column("name")]
        public string C_Name { get; set; }

        [StringLength(112)]
        [Column("unique_id")]
        public string C_Unique { get; set; }

        [NotMapped]
        public int? L_Type { get; set; }

        [NotMapped]
        [StringLength(14)]
        public string C_RegDate { get; set; }

        [NotMapped]
        public int? L_OptDateLimit { get; set; }

        [NotMapped]
        [StringLength(16)]
        public string C_DateLimit { get; set; }

        [NotMapped]
        public int? L_AccessType { get; set; }

        [NotMapped]
        [StringLength(64)]
        public string C_Password { get; set; }

        [NotMapped]
        public int? L_Identify { get; set; }

        [NotMapped]
        public int? L_VerifyLevel { get; set; }

        [NotMapped]
        [StringLength(4)]
        public string C_AccessGroup { get; set; }

        [NotMapped]
        [StringLength(4)]
        public string C_PassbackStatus { get; set; }

        [NotMapped]
        public int? L_VOIPUsed { get; set; }

        [NotMapped]
        public int? L_DoorOpen { get; set; }

        [NotMapped]
        public int? L_AutoAnswer { get; set; }

        [NotMapped]
        public int? L_EnableMeta1 { get; set; }

        [NotMapped]
        public int? L_RingCount1 { get; set; }

        [NotMapped]
        [StringLength(20)]
        public string C_LoginID1 { get; set; }

        [NotMapped]
        [StringLength(36)]
        public string C_SipAddr1 { get; set; }

        [NotMapped]
        public int? L_EnableMeta2 { get; set; }

        [NotMapped]
        public int? L_RingCount2 { get; set; }

        [NotMapped]
        [StringLength(20)]
        public string C_LoginID2 { get; set; }

        [NotMapped]
        [StringLength(36)]
        public string C_SipAddr2 { get; set; }

        [NotMapped]
        [StringLength(128)]
        public string C_UserMessage { get; set; }

        [NotMapped]
        public int? L_Blacklist { get; set; }

        [NotMapped]
        public int? L_IsNotice { get; set; }

        [NotMapped]
        [StringLength(128)]
        public string C_Notice { get; set; }

        [NotMapped]
        [StringLength(14)]
        public string C_PassbackTime { get; set; }

        [NotMapped]
        public int? L_ExceptPassback { get; set; }

        [NotMapped]
        public int? L_DataCheck { get; set; }

        [NotMapped]
        public int? L_Partition { get; set; }

        [NotMapped]
        public int? L_FaceIdentify { get; set; }

        [NotMapped]
        public byte[] B_DuressFinger { get; set; }

        [NotMapped]
        public int? L_AuthValue { get; set; }

        [NotMapped]
        [StringLength(64)]
        public string C_RemotePW { get; set; }

        [NotMapped]
        public int? L_WrongCount { get; set; }

        [NotMapped]
        public int? L_LogonLocked { get; set; }

        [NotMapped]
        [StringLength(14)]
        public string C_LogonDateTime { get; set; }

        [NotMapped]
        [StringLength(14)]
        public string C_UdatePassword { get; set; }

        [NotMapped]
        [StringLength(1)]
        public string C_MustChgPwd { get; set; }

        [NotMapped]
        public int? L_RegServer { get; set; }
    }
}
