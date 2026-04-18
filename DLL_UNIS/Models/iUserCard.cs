namespace DLL_UNIS.Models
{
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    /// <summary>Maps to UCDB dbo.user_rfcards.</summary>
    [Table("user_rfcards")]
    public partial class iUserCard
    {
        [Key]
        [StringLength(40)]
        [Column("card_num")]
        public string C_CardNum { get; set; }

        [Column("user_id")]
        public long L_UID { get; set; }

        [NotMapped]
        public int? L_DataCheck { get; set; }
    }
}
