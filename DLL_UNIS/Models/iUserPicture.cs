namespace DLL_UNIS.Models
{
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    /// <summary>Maps to UCDB dbo.user_pictures.</summary>
    [Table("user_pictures")]
    public partial class iUserPicture
    {
        [Key]
        [Column("user_id")]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public long L_UID { get; set; }

        [Column("image_type")]
        public int L_ImageType { get; set; }

        [Column("image_size")]
        public int L_ImageSize { get; set; }

        [Column("image_data", TypeName = "varbinary(max)")]
        public byte[] B_Picture { get; set; }

        [Column("thumb_size")]
        public int? L_ThumbSize { get; set; }

        [Column("thumb_data", TypeName = "varbinary(7999)")]
        public byte[] B_ThumbData { get; set; }
    }
}
