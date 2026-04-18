namespace DLL_UNIS.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("tNvrChannelConfig")]
    public partial class tNvrChannelConfig
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int L_ChannelID { get; set; }

        public int L_IsActive { get; set; }

        [Required]
        [StringLength(16)]
        public string C_IPAddr { get; set; }

        public int L_Port { get; set; }

        [StringLength(64)]
        public string C_ConnectID { get; set; }

        [StringLength(64)]
        public string C_ConnectPwd { get; set; }

        public int L_ImageX { get; set; }

        public int L_ImageY { get; set; }

        public int L_FPS { get; set; }

        public int L_BPS { get; set; }

        [StringLength(128)]
        public string C_MediaAddr { get; set; }

        public int L_PreRecord { get; set; }

        public int L_PostRecord { get; set; }

        public int L_RecType { get; set; }

        [StringLength(128)]
        public string C_Uri { get; set; }

        [StringLength(128)]
        public string C_MediaUri { get; set; }
    }
}
