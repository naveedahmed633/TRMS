namespace DLL_UNIS.Models
{
    using System;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    /// <summary>Maps to UCDB dbo.user_mobilekeys.</summary>
    [Table("user_mobilekeys")]
    public partial class iUserMobileKey
    {
        [Key]
        [Column("user_id")]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public long L_UID { get; set; }

        [StringLength(24)]
        [Column("access_period")]
        public string C_ImkeyPeriod { get; set; }

        [StringLength(4)]
        [Column("country_code")]
        public string C_CountryCode { get; set; }

        [StringLength(16)]
        [Column("phone_number")]
        public string C_MobilePhone { get; set; }

        [Column("now_issue")]
        public int? L_NowIssue { get; set; }

        [Column("key_type")]
        public int? L_KeyType { get; set; }

        [Column("issue_count")]
        public int? L_issuecount { get; set; }

        [StringLength(18)]
        [Column("key_no")]
        public string C_KeyNo { get; set; }

        [Column("uuid", TypeName = "varbinary(128)")]
        public byte[] B_UUID { get; set; }

        [Column("state")]
        public int? L_State { get; set; }

        [Column("state_at")]
        public DateTimeOffset? D_StateAt { get; set; }
    }
}
