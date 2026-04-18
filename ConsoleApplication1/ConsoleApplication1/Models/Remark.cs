namespace DLL.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public class Remark
    {
        public int RemarkId { get; set; }

        public string description { get; set; }

        public string short_hand { get; set; }

        public bool active { get; set; }

    }
}
