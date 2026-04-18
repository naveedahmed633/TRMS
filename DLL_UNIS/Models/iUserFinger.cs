namespace DLL_UNIS.Models
{
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    /// <summary>Maps to UCDB dbo.user_fps.</summary>
    [Table("user_fps")]
    public partial class iUserFinger
    {
        [Key]
        [Column("user_id")]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public long L_UID { get; set; }

        [Column("fir_size")]
        public int L_FirSize { get; set; }

        [Column("fir", TypeName = "varbinary(max)")]
        public byte[] B_TextFIR { get; set; }

        [Column("enc_type")]
        public int? L_EncType { get; set; }

        [Column("brand_type")]
        public int? L_BrandType { get; set; }

        [Column("version")]
        public int? L_FpVersion { get; set; }

        [NotMapped]
        public int? L_IsWideChar { get; set; }
    }
}
