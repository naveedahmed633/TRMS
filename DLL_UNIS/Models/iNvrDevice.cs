namespace DLL_UNIS.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("iNvrDevice")]
    public partial class iNvrDevice
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int L_ChannelID { get; set; }

        [StringLength(64)]
        public string C_Name { get; set; }

        [Required]
        [StringLength(16)]
        public string C_IPAddr { get; set; }

        public int L_Port { get; set; }

        [StringLength(64)]
        public string C_ConnectID { get; set; }

        [StringLength(64)]
        public string C_ConnectPwd { get; set; }

        [StringLength(128)]
        public string C_URI { get; set; }

        [StringLength(32)]
        public string C_Vendor { get; set; }

        [StringLength(32)]
        public string C_Model { get; set; }

        [StringLength(32)]
        public string C_FirmwareVer { get; set; }
    }
}
