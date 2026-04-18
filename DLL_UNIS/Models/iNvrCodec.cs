namespace DLL_UNIS.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("iNvrCodec")]
    public partial class iNvrCodec
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int L_ChannelID { get; set; }

        [StringLength(128)]
        public string C_Name { get; set; }

        [StringLength(128)]
        public string C_Token { get; set; }

        public int L_CodecID { get; set; }

        public int L_ImageX { get; set; }

        public int L_ImageY { get; set; }

        public int L_Quality { get; set; }

        public int L_FPS { get; set; }

        public int L_BPS { get; set; }
    }
}
